using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("Rendering/Dynamic Shadow Controller")]
public class DynamicShadowController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public interface Renderer : IDisposable
	{
		void RenderShadows();
	}

	private class SingleRenderer : IDisposable, Renderer
	{
		private const int VERTICES_PER_BOX = 8;

		private const int INDICES_PER_BOX = 36;

		private const int MaxBufferSize = 1024;

		private DynamicShadowTarget[] Targets = new DynamicShadowTarget[0];

		private Vector3[] TargetCenters = new Vector3[0];

		private Vector3[] TargetExtents = new Vector3[0];

		private int[] OldLayers = new int[0];

		private Camera FinalCamera;

		private RenderTexture ShadowBuffer;

		private RenderTexture GaussianBuffer;

		private Mesh ShadowMesh;

		private DynamicShadowController Controller;

		private int TilesX = 1;

		private int TilesY = 1;

		private int TileSize
		{
			get
			{
				return GetTileSize(Controller);
			}
		}

		private int BlurGutterSize
		{
			get
			{
				return (int)(Controller.BlurWidth * 2f + 0.999f);
			}
		}

		public SingleRenderer(DynamicShadowController controller, DynamicShadowTarget[] targets, Camera camera)
		{
			Controller = controller;
			Targets = targets;
			OldLayers = new int[Targets.Length];
			FinalCamera = camera;
			TargetCenters = new Vector3[Targets.Length];
			TargetExtents = new Vector3[Targets.Length];
			for (int i = 0; i < Targets.Length; i++)
			{
				GetExtents(Targets[i], out TargetCenters[i], out TargetExtents[i]);
			}
			CreateShadowBuffer();
			RenderShadowBuffer();
			CreateShadowMesh();
		}

		void IDisposable.Dispose()
		{
			ReleaseShadowBuffer(true);
			for (int i = 0; i < Targets.Length; i++)
			{
				Targets[i].gameObject.layer = OldLayers[i];
			}
			Targets = new DynamicShadowTarget[0];
			if (ShadowMesh != null)
			{
				ShadowMesh.Clear();
				Controller.MeshPool.Push(ShadowMesh);
				ShadowMesh = null;
			}
		}

		void Renderer.RenderShadows()
		{
			Controller.ShadowMeshObject.transform.parent = null;
			Controller.ShadowMeshObject.active = true;
			int cullingMask = FinalCamera.cullingMask;
			FinalCamera.cullingMask = 1073741824;
			Controller.ShadowMeshObject.transform.position = default(Vector3);
			Controller.ShadowMeshObject.transform.rotation = Quaternion.identity;
			Controller.ShadowMeshFilter.sharedMesh = ShadowMesh;
			Shader.SetGlobalTexture("_ShadowTex", ShadowBuffer);
			Shader.SetGlobalFloat("_TileSize", TileSize);
			Shader.SetGlobalFloat("_BlurGutterSize", BlurGutterSize);
			FinalCamera.SetReplacementShader(Controller.AOProjection, "RenderType");
			FinalCamera.Render();
			FinalCamera.ResetReplacementShader();
			Controller.ShadowMeshFilter.sharedMesh = null;
			FinalCamera.cullingMask = cullingMask;
			Controller.ShadowMeshObject.active = false;
			Controller.ShadowMeshObject.transform.parent = Controller.transform;
			Controller.ShadowMeshObject.transform.localPosition = default(Vector3);
		}

		private void GetExtents(DynamicShadowTarget target, out Vector3 center, out Vector3 extents)
		{
			center = default(Vector3);
			extents = default(Vector3);
			CharacterController component = Utils.GetComponent<CharacterController>(target.gameObject, Utils.SearchParents);
			if (component != null)
			{
				center = component.transform.TransformPoint(new Vector3(0f, component.height / 2f, 0f));
				float magnitude = target.renderer.bounds.extents.magnitude;
				extents = new Vector3(magnitude, component.height / 2f, magnitude);
			}
			else if (target.renderer != null)
			{
				center = target.renderer.bounds.center;
				extents = target.renderer.bounds.extents;
			}
		}

		private void RenderShadowBuffer()
		{
			Camera shadowCamera = Controller.ShadowCamera;
			shadowCamera.clearFlags = CameraClearFlags.Color;
			shadowCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
			shadowCamera.cullingMask = 0;
			shadowCamera.targetTexture = ShadowBuffer;
			shadowCamera.orthographic = true;
			shadowCamera.nearClipPlane = Controller.AORadius;
			shadowCamera.farClipPlane = Controller.AORadius * 3f;
			shadowCamera.rect = new Rect(0f, 0f, 1f, 1f);
			shadowCamera.Render();
			shadowCamera.clearFlags = CameraClearFlags.Nothing;
			try
			{
				shadowCamera.cullingMask = int.MinValue;
				for (int i = 0; i < Targets.Length; i++)
				{
					DynamicShadowTarget dynamicShadowTarget = Targets[i];
					OldLayers[i] = dynamicShadowTarget.gameObject.layer;
					dynamicShadowTarget.gameObject.layer = 31;
					int latestVisibleFrame = dynamicShadowTarget.LatestVisibleFrame;
					try
					{
						if (!(dynamicShadowTarget.renderer == null))
						{
							Vector3 a = TargetCenters[i];
							Vector3 vector = TargetExtents[i];
							Vector3 vector2 = vector;
							shadowCamera.orthographicSize = vector2.z;
							shadowCamera.aspect = vector2.x / vector2.z;
							shadowCamera.transform.rotation = Quaternion.LookRotation(new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, 1f));
							Vector3 position = a + new Vector3(0f, -1f, 0f) * (vector.y - Controller.AORadius * 2f);
							shadowCamera.transform.position = position;
							int num = i % TilesX * TileSize + BlurGutterSize;
							int num2 = i / TilesX * TileSize + BlurGutterSize;
							int num3 = TileSize - BlurGutterSize * 2;
							shadowCamera.rect = new Rect((float)num / (float)ShadowBuffer.width, (float)num2 / (float)ShadowBuffer.height, (float)num3 / (float)ShadowBuffer.width, (float)num3 / (float)ShadowBuffer.height);
							Plane plane = new Plane(new Vector3(0f, 1f, 0f), a + new Vector3(0f, -1f, 0f) * vector.y);
							Vector3 normal = plane.normal;
							float x = normal.x;
							Vector3 normal2 = plane.normal;
							float y = normal2.y;
							Vector3 normal3 = plane.normal;
							Shader.SetGlobalVector("_ProjectionPlane", new Vector4(x, y, normal3.z, plane.distance));
							Shader.SetGlobalVector("_AORadius", new Vector4(Controller.AORadius, Mathf.Clamp01(1f - Controller.AOContrast), 1f / Controller.AORadius, 0f));
							shadowCamera.Render();
						}
					}
					finally
					{
						dynamicShadowTarget.gameObject.layer = OldLayers[i];
						dynamicShadowTarget.LatestVisibleFrame = latestVisibleFrame;
					}
				}
			}
			finally
			{
				shadowCamera.targetTexture = null;
			}
			if (Controller.BlurMaterial != null)
			{
				ApplyGaussian(3f, ShadowBuffer);
			}
		}

		public void MarkTargets()
		{
			DynamicShadowTarget[] targets = Targets;
			foreach (DynamicShadowTarget dynamicShadowTarget in targets)
			{
				dynamicShadowTarget.gameObject.layer = 31;
			}
		}

		private void CreateShadowMesh()
		{
			if (Controller.MeshPool.Count > 0)
			{
				ShadowMesh = Controller.MeshPool.Pop();
			}
			else
			{
				ShadowMesh = new Mesh();
			}
			Vector3[] array = new Vector3[Targets.Length * 8];
			Vector2[] uv = new Vector2[array.Length];
			Vector2[] uv2 = new Vector2[array.Length];
			Color[] array2 = new Color[array.Length];
			int[] array3 = new int[Targets.Length * 36];
			for (int i = 0; i < Targets.Length; i++)
			{
				AddShadowMeshGeometry(i, array, uv, uv2, array2, array3);
			}
			ShadowMesh.vertices = array;
			ShadowMesh.uv = uv;
			ShadowMesh.uv2 = uv2;
			ShadowMesh.colors = array2;
			ShadowMesh.triangles = array3;
			ShadowMesh.RecalculateBounds();
		}

		private void AddShadowMeshGeometry(int targetIndex, Vector3[] pos, Vector2[] uv, Vector2[] uv2, Color[] color, int[] tri)
		{
			Vector3 vector = TargetExtents[targetIndex];
			Vector3 a = TargetCenters[targetIndex] + new Vector3(0f, -1f, 0f) * vector.y;
			Vector3 b = new Vector3(1f, 0f, 0f) * vector.x;
			Vector3 b2 = new Vector3(0f, 0f, 1f) * vector.z;
			Vector3 b3 = new Vector3(0f, 1f, 0f) * Controller.AORadius;
			int num = targetIndex * 8;
			pos[num] = a + b + b3 + b2;
			pos[num + 1] = a - b + b3 + b2;
			pos[num + 2] = a - b - b3 + b2;
			pos[num + 3] = a + b - b3 + b2;
			pos[num + 4] = a + b + b3 - b2;
			pos[num + 5] = a - b + b3 - b2;
			pos[num + 6] = a - b - b3 - b2;
			pos[num + 7] = a + b - b3 - b2;
			int num2 = targetIndex * 36;
			tri[num2] = num;
			tri[num2 + 1] = num + 1;
			tri[num2 + 2] = num + 2;
			tri[num2 + 3] = num;
			tri[num2 + 4] = num + 2;
			tri[num2 + 5] = num + 3;
			tri[num2 + 6] = num;
			tri[num2 + 7] = num + 3;
			tri[num2 + 8] = num + 4;
			tri[num2 + 9] = num + 4;
			tri[num2 + 10] = num + 3;
			tri[num2 + 11] = num + 7;
			tri[num2 + 12] = num + 2;
			tri[num2 + 13] = num + 1;
			tri[num2 + 14] = num + 5;
			tri[num2 + 15] = num + 2;
			tri[num2 + 16] = num + 5;
			tri[num2 + 17] = num + 6;
			tri[num2 + 18] = num + 1;
			tri[num2 + 19] = num;
			tri[num2 + 20] = num + 4;
			tri[num2 + 21] = num + 1;
			tri[num2 + 22] = num + 4;
			tri[num2 + 23] = num + 5;
			tri[num2 + 24] = num + 3;
			tri[num2 + 25] = num + 2;
			tri[num2 + 26] = num + 6;
			tri[num2 + 27] = num + 3;
			tri[num2 + 28] = num + 6;
			tri[num2 + 29] = num + 7;
			tri[num2 + 30] = num + 7;
			tri[num2 + 31] = num + 6;
			tri[num2 + 32] = num + 5;
			tri[num2 + 33] = num + 7;
			tri[num2 + 34] = num + 5;
			tri[num2 + 35] = num + 4;
			Vector2 vector2 = new Vector2(targetIndex % TilesX, targetIndex / TilesX);
			Vector2 vector3 = new Vector2(vector.x, vector.z);
			for (int i = 0; i < 8; i++)
			{
				uv[num + i] = vector3;
				uv2[num + i] = vector2;
			}
			color[num] = new Color(1f, 1f, 0f, 0f);
			color[num + 1] = new Color(0f, 1f, 0f, 0f);
			color[num + 2] = new Color(0f, 0f, 0f, 0f);
			color[num + 3] = new Color(1f, 0f, 0f, 0f);
			color[num + 4] = new Color(1f, 1f, 1f, 0f);
			color[num + 5] = new Color(0f, 1f, 1f, 0f);
			color[num + 6] = new Color(0f, 0f, 1f, 0f);
			color[num + 7] = new Color(1f, 0f, 1f, 0f);
		}

		private void ReleaseShadowBuffer(bool partial)
		{
			if (ShadowBuffer != null)
			{
				if (Controller.DebugShadowBuffers && !partial)
				{
					DynamicShadowTarget[] targets = Targets;
					foreach (DynamicShadowTarget dynamicShadowTarget in targets)
					{
						dynamicShadowTarget.ShadowBuffer = null;
					}
				}
				else if (!Controller.DebugShadowBuffers)
				{
					RenderTexture.ReleaseTemporary(ShadowBuffer);
				}
				ShadowBuffer = null;
			}
			if (GaussianBuffer != null)
			{
				RenderTexture.ReleaseTemporary(GaussianBuffer);
				GaussianBuffer = null;
			}
		}

		private void CreateShadowBuffer()
		{
			TilesX = (int)Mathf.Sqrt(Targets.Length);
			TilesY = TilesX;
			while (TilesX * TilesY < Targets.Length)
			{
				if (TilesX == TilesY)
				{
					TilesX++;
				}
				else
				{
					TilesY++;
				}
			}
			int num = TilesX * TileSize;
			int num2 = TilesY * TileSize;
			if (Controller.DebugShadowBuffers)
			{
				ShadowBuffer = Targets[0].ShadowBuffer;
				if (ShadowBuffer != null && (ShadowBuffer.width != num || ShadowBuffer.height != num2))
				{
					ReleaseShadowBuffer(false);
				}
				if (ShadowBuffer == null)
				{
					ShadowBuffer = new RenderTexture(num, num2, 24);
					DynamicShadowTarget[] targets = Targets;
					foreach (DynamicShadowTarget dynamicShadowTarget in targets)
					{
						dynamicShadowTarget.ShadowBuffer = ShadowBuffer;
					}
				}
			}
			else
			{
				ShadowBuffer = RenderTexture.GetTemporary(num, num2, 24);
			}
			ShadowBuffer.wrapMode = TextureWrapMode.Clamp;
			ShadowBuffer.isPowerOfTwo = false;
			GaussianBuffer = RenderTexture.GetTemporary(num, num2, 24);
			GaussianBuffer.wrapMode = TextureWrapMode.Clamp;
			GaussianBuffer.isPowerOfTwo = false;
		}

		private void ApplyGaussian(float strength, RenderTexture source)
		{
			double[] array = new double[15]
			{
				0.0,
				0.0,
				0.0,
				0.0,
				0.0,
				0.0,
				0.0,
				1.0,
				0.0,
				0.0,
				0.0,
				0.0,
				0.0,
				0.0,
				0.0
			};
			for (int i = 1; i < 6; i++)
			{
				double num = (double)strength * (double)i / 5.0;
				array[5 - i + 2] = (array[5 + i + 2] = Math.Exp((0.0 - num) * num / 2.0));
			}
			double num2 = 0.0;
			for (int j = 0; j < array.Length; j++)
			{
				num2 = Math.Max(num2, array[j]);
			}
			for (int k = 0; k < array.Length; k++)
			{
				array[k] /= num2;
			}
			Controller.BlurMaterial.SetFloat("_Width", Controller.BlurWidth);
			Controller.BlurMaterial.SetFloat("_Horizontal", 1f);
			Controller.BlurMaterial.SetVector("_Weights0", new Vector4((float)array[0], (float)array[1], (float)array[2], (float)array[3]));
			Controller.BlurMaterial.SetVector("_Weights1", new Vector4((float)array[4], (float)array[5], (float)array[6], (float)array[7]));
			Controller.BlurMaterial.SetVector("_Weights2", new Vector4((float)array[8], (float)array[9], (float)array[10], (float)array[11]));
			Controller.BlurMaterial.SetVector("_Weights3", new Vector4((float)array[12], (float)array[13], (float)array[14], 0f));
			ShaderUtil.Blit(source, GaussianBuffer, Controller.BlurMaterial);
			Controller.BlurMaterial.SetFloat("_Horizontal", 0f);
			ShaderUtil.Blit(GaussianBuffer, source, Controller.BlurMaterial);
		}

		private static int GetTileSize(DynamicShadowController controller)
		{
			return Mathf.Max(Mathf.Min(controller.Resolution, 1024), 1);
		}

		public static int GetMaxTargets(DynamicShadowController controller)
		{
			int num = 1024 / GetTileSize(controller);
			return Mathf.Max(num * num, 1);
		}
	}

	private class MultiRenderer : IDisposable, Renderer
	{
		private Renderer[] Renderers;

		public MultiRenderer(Renderer[] renderers)
		{
			Renderers = renderers;
		}

		void Renderer.RenderShadows()
		{
			Renderer[] renderers = Renderers;
			foreach (Renderer renderer in renderers)
			{
				renderer.RenderShadows();
			}
		}

		void IDisposable.Dispose()
		{
			Renderer[] renderers = Renderers;
			foreach (Renderer renderer in renderers)
			{
				renderer.Dispose();
			}
		}
	}

	private static Dictionary<DynamicShadowTarget, bool> shadowTargets;

	[HideInInspector]
	public Camera ShadowCamera;

	[HideInInspector]
	public GameObject ShadowMeshObject;

	[HideInInspector]
	public MeshFilter ShadowMeshFilter;

	[HideInInspector]
	public Stack<Mesh> MeshPool = new Stack<Mesh>();

	public int Resolution = 128;

	public float AORadius = 0.3f;

	public float AOContrast = 0.5f;

	public Shader AODepth;

	public Shader AOProjection;

	public float BlurWidth = 7f;

	public Shader BlurShader;

	private Material BlurMaterial;

	public bool DebugShadowBuffers;

	public static void RegisterShadowTarget(DynamicShadowTarget target)
	{
		try
		{
			if (shadowTargets != null)
			{
				shadowTargets.Add(target, true);
			}
		}
		catch (ArgumentException)
		{
		}
	}

	public static void UnregisterShadowTarget(DynamicShadowTarget target)
	{
		if (shadowTargets != null)
		{
			shadowTargets.Remove(target);
		}
	}

	public void Awake()
	{
		if (shadowTargets == null)
		{
			shadowTargets = new Dictionary<DynamicShadowTarget, bool>();
		}
	}

	public void OnDisable()
	{
		if (shadowTargets != null)
		{
			shadowTargets.Clear();
		}
	}

	public void Start()
	{
		ShadowCamera = Utils.AddComponent<Camera>(new GameObject("Shadow Camera"));
		ShadowCamera.enabled = false;
		ShadowCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
		ShadowCamera.transform.parent = base.transform;
		ShadowMeshObject = new GameObject("Shadow Mesh");
		ShadowMeshObject.hideFlags = HideFlags.HideAndDontSave;
		ShadowMeshObject.transform.parent = base.transform;
		ShadowMeshObject.active = true;
		ShadowMeshObject.layer = 30;
		ShadowMeshObject.hideFlags = HideFlags.HideAndDontSave;
		ShadowMeshFilter = Utils.AddComponent<MeshFilter>(ShadowMeshObject);
		MeshRenderer meshRenderer = Utils.AddComponent<MeshRenderer>(ShadowMeshObject);
		meshRenderer.castShadows = false;
		meshRenderer.receiveShadows = false;
		if (BlurShader != null)
		{
			BlurMaterial = new Material(BlurShader);
			BlurMaterial.hideFlags = HideFlags.HideAndDontSave;
		}
		base.enabled = (GraphicsOptions.Shadows == GraphicsOptions.ShadowLevel.Projected);
	}

	public void Update()
	{
		ShadowCamera.SetReplacementShader(AODepth, "RenderType");
		MeshRenderer component = Utils.GetComponent<MeshRenderer>(ShadowMeshObject);
		if (component.material.shader != AOProjection)
		{
			Material material = new Material(AOProjection);
			material.hideFlags = HideFlags.HideAndDontSave;
			component.material = material;
		}
	}

	public Renderer CreateRenderer(Camera camera)
	{
		int num = 0;
		foreach (DynamicShadowTarget key in shadowTargets.Keys)
		{
			if (key != null && key.LatestVisibleFrame >= Time.frameCount - 1)
			{
				num++;
			}
		}
		if (num == 0)
		{
			return null;
		}
		DynamicShadowTarget[] array = new DynamicShadowTarget[num];
		num = 0;
		foreach (DynamicShadowTarget key2 in shadowTargets.Keys)
		{
			if (key2 != null && key2.LatestVisibleFrame >= Time.frameCount - 1)
			{
				array[num++] = key2;
			}
		}
		int num2 = SingleRenderer.GetMaxTargets(this);
		if (DebugShadowBuffers)
		{
			num2 = 1;
		}
		if (array.Length <= num2)
		{
			SingleRenderer singleRenderer = new SingleRenderer(this, array, camera);
			singleRenderer.MarkTargets();
			return singleRenderer;
		}
		SingleRenderer[] array2 = new SingleRenderer[(array.Length + num2 - 1) / num2];
		for (int i = 0; i < array2.Length; i++)
		{
			DynamicShadowTarget[] array3 = new DynamicShadowTarget[Mathf.Min(num2, array.Length - i * num2)];
			Array.Copy(array, i * num2, array3, 0, array3.Length);
			array2[i] = new SingleRenderer(this, array3, camera);
		}
		SingleRenderer[] array4 = array2;
		foreach (SingleRenderer singleRenderer2 in array4)
		{
			singleRenderer2.MarkTargets();
		}
		return new MultiRenderer(array2);
	}
}
