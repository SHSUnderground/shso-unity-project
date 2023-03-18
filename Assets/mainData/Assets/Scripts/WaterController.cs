using UnityEngine;

[AddComponentMenu("Rendering/Water Controller")]
public class WaterController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public LayerMask intersectionLayers = 2109440;

	public Shader intersectionFrontShader;

	public Shader intersectionBackShader;

	public Shader differenceShader;

	public Shader speedUpdateShader;

	public Shader integrationShader;

	public Shader averageShader;

	private bool previousWaterSetting;

	private Camera _workingCamera;

	private RenderTexture _impulseTexture;

	private RenderTexture[] _intersectionTexture;

	private RenderTexture[] _waveTexture;

	private RenderTexture[] _speedTexture;

	private RenderTexture _averageTexture;

	private int _oldTexelsPerMeter;

	public int texelsPerMeter = 100;

	public float waveSpeedDampening = 0.99f;

	public float waveHeightDampening = 0.99f;

	public float waveGravityPressure = 0.2f;

	public float waveFriction = 0.99f;

	public float wavePersistence = 30f;

	public float waveInstability = 100f;

	public float maximumDepth = 10f;

	private int _frameToggle;

	private bool _frameReset = true;

	private Material _differenceMaterial;

	private Material _speedUpdateMaterial;

	private Material _integrationMaterial;

	private Material _averageMaterial;

	private int _currentFrame = -1;

	public void OnWillRenderObject()
	{
		if (base.enabled && (bool)base.renderer && (bool)base.renderer.material && base.renderer.enabled && Time.frameCount != _currentFrame)
		{
			_currentFrame = Time.frameCount;
			int layer = base.gameObject.layer;
			base.gameObject.layer = 4;
			try
			{
				if (UpdateMaterials())
				{
					Camera workingCamera = GetWorkingCamera();
					UpdateCamera(workingCamera);
					UpdateTextures();
					int frameToggle = _frameToggle;
					int num = frameToggle ^ 1;
					workingCamera.targetTexture = _intersectionTexture[frameToggle];
					workingCamera.backgroundColor = Color.black;
					workingCamera.SetReplacementShader(intersectionBackShader, "RenderType");
					workingCamera.clearFlags = CameraClearFlags.Color;
					workingCamera.Render();
					workingCamera.SetReplacementShader(intersectionFrontShader, "RenderType");
					workingCamera.clearFlags = CameraClearFlags.Nothing;
					workingCamera.Render();
					if (_frameReset)
					{
						workingCamera.targetTexture = null;
						Graphics.Blit(_intersectionTexture[frameToggle], _intersectionTexture[num]);
						workingCamera.clearFlags = CameraClearFlags.Color;
						workingCamera.backgroundColor = Color.gray;
						workingCamera.cullingMask = 0;
						workingCamera.targetTexture = _waveTexture[num];
						workingCamera.Render();
						workingCamera.targetTexture = _speedTexture[num];
						workingCamera.Render();
					}
					workingCamera.targetTexture = null;
					_differenceMaterial.SetTexture("_OldTex", _intersectionTexture[num]);
					Graphics.Blit(_intersectionTexture[frameToggle], _impulseTexture, _differenceMaterial);
					_differenceMaterial.SetTexture("_OldTex", null);
					_averageMaterial.SetFloat("_Horizontal", 1f);
					Graphics.Blit(_waveTexture[num], _waveTexture[frameToggle], _averageMaterial);
					_averageMaterial.SetFloat("_Horizontal", 0f);
					Graphics.Blit(_waveTexture[frameToggle], _averageTexture, _averageMaterial);
					_speedUpdateMaterial.SetTexture("_HeightTex", _waveTexture[num]);
					_speedUpdateMaterial.SetTexture("_ImpulseTex", _impulseTexture);
					_speedUpdateMaterial.SetTexture("_AverageTex", _averageTexture);
					_speedUpdateMaterial.SetFloat("_Dampening", waveSpeedDampening);
					_speedUpdateMaterial.SetFloat("_DeltaTime", Time.deltaTime);
					_speedUpdateMaterial.SetFloat("_GravityPressure", waveGravityPressure);
					_speedUpdateMaterial.SetFloat("_Friction", waveFriction);
					_speedUpdateMaterial.SetFloat("_Persistence", wavePersistence);
					Graphics.Blit(_speedTexture[num], _speedTexture[frameToggle], _speedUpdateMaterial);
					_speedUpdateMaterial.SetTexture("_HeightTex", null);
					_speedUpdateMaterial.SetTexture("_ImpulseTex", null);
					_speedUpdateMaterial.SetTexture("_AverageTex", null);
					_integrationMaterial.SetTexture("_SpeedTex", _speedTexture[frameToggle]);
					_integrationMaterial.SetTexture("_ImpulseTex", _impulseTexture);
					_integrationMaterial.SetFloat("_Dampening", waveHeightDampening);
					_integrationMaterial.SetFloat("_DeltaTime", Mathf.Clamp(Time.deltaTime, 0.015625f, 0.0625f));
					_integrationMaterial.SetFloat("_Instability", waveInstability);
					Graphics.Blit(_waveTexture[num], _waveTexture[frameToggle], _integrationMaterial);
					_integrationMaterial.SetTexture("_SpeedTex", null);
					_integrationMaterial.SetTexture("_ImpulseTex", null);
					base.renderer.material.SetTexture("_WaveTex", _waveTexture[frameToggle]);
					if (!_frameReset)
					{
						_frameToggle ^= 1;
					}
					else
					{
						_frameReset = false;
					}
				}
			}
			finally
			{
				base.gameObject.layer = layer;
			}
		}
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
		if ((bool)base.renderer)
		{
			Material material = base.renderer.material;
			if ((bool)material && material.HasProperty("_WaveTex"))
			{
				material.SetTexture("_WaveTex", null);
			}
		}
		DestroyMaterials();
		DestroyTextures();
		if ((bool)_workingCamera)
		{
			Object.Destroy(_workingCamera);
			_workingCamera = null;
		}
	}

	private Camera GetWorkingCamera()
	{
		Camera workingCamera = _workingCamera;
		if ((bool)workingCamera)
		{
			return workingCamera;
		}
		GameObject gameObject = new GameObject("WaterController " + GetInstanceID() + " working camera", typeof(Camera));
		gameObject.transform.parent = base.transform;
		workingCamera = gameObject.camera;
		workingCamera.enabled = false;
		gameObject.hideFlags = HideFlags.HideAndDontSave;
		workingCamera.backgroundColor = Color.black;
		workingCamera.nearClipPlane = 0.01f;
		workingCamera.orthographic = true;
		_workingCamera = workingCamera;
		return workingCamera;
	}

	private void UpdateCamera(Camera cam)
	{
		cam.cullingMask = ((int)intersectionLayers & -17);
		if ((cam.cullingMask & 0x203000) != 0)
		{
			cam.cullingMask |= int.MinValue;
		}
		Bounds bounds = base.renderer.bounds;
		Transform transform = cam.transform;
		Vector3 center = bounds.center;
		float x = center.x;
		Vector3 max = bounds.max;
		float y = max.y + cam.nearClipPlane;
		Vector3 center2 = bounds.center;
		transform.position = new Vector3(x, y, center2.z);
		cam.transform.rotation = Quaternion.LookRotation(new Vector3(0f, -1f, 0f), new Vector3(0f, 0f, 1f));
		Vector3 extents = bounds.extents;
		float x2 = extents.x;
		Vector3 extents2 = bounds.extents;
		cam.aspect = x2 / extents2.z;
		Vector3 extents3 = bounds.extents;
		cam.orthographicSize = extents3.z;
		cam.farClipPlane = maximumDepth;
	}

	private void UpdateTextures()
	{
		if (_oldTexelsPerMeter != texelsPerMeter)
		{
			DestroyTextures();
			float num = texelsPerMeter;
			Vector3 extents = base.renderer.bounds.extents;
			int sizeX = Mathf.Min(Mathf.RoundToInt(num * extents.x), 1024);
			float num2 = texelsPerMeter;
			Vector3 extents2 = base.renderer.bounds.extents;
			int sizeY = Mathf.Min(Mathf.RoundToInt(num2 * extents2.z), 1024);
			int instanceID = GetInstanceID();
			_impulseTexture = MakeTexture(sizeX, sizeY, instanceID, "_Impulse", false);
			_averageTexture = MakeTexture(sizeX, sizeY, instanceID, "_Average", false);
			_speedTexture = new RenderTexture[2]
			{
				MakeTexture(sizeX, sizeY, instanceID, "_Velocity0", false),
				MakeTexture(sizeX, sizeY, instanceID, "_Velocity1", false)
			};
			_waveTexture = new RenderTexture[2]
			{
				MakeTexture(sizeX, sizeY, instanceID, "_Wave0", false),
				MakeTexture(sizeX, sizeY, instanceID, "_Wave1", false)
			};
			_intersectionTexture = new RenderTexture[2]
			{
				MakeTexture(sizeX, sizeY, instanceID, "_Intersection0", true),
				MakeTexture(sizeX, sizeY, instanceID, "_Intersection1", true)
			};
			_oldTexelsPerMeter = texelsPerMeter;
		}
	}

	private static RenderTexture MakeTexture(int sizeX, int sizeY, int instanceID, string suffix, bool zbuffer)
	{
		RenderTexture renderTexture = new RenderTexture(sizeX, sizeY, zbuffer ? 24 : 0);
		renderTexture.name = "__WaterController_" + instanceID + suffix;
		renderTexture.wrapMode = TextureWrapMode.Clamp;
		renderTexture.isPowerOfTwo = false;
		renderTexture.filterMode = FilterMode.Bilinear;
		renderTexture.hideFlags = HideFlags.DontSave;
		return renderTexture;
	}

	private void DestroyTextures()
	{
		_impulseTexture = DestroyTexture(_impulseTexture);
		_averageTexture = DestroyTexture(_averageTexture);
		_waveTexture = DestroyTextureArray(_waveTexture);
		_speedTexture = DestroyTextureArray(_speedTexture);
		_intersectionTexture = DestroyTextureArray(_intersectionTexture);
		_frameReset = true;
		_oldTexelsPerMeter = 0;
	}

	private static RenderTexture DestroyTexture(RenderTexture texture)
	{
		if (texture != null)
		{
			Object.Destroy(texture);
		}
		return null;
	}

	private static RenderTexture[] DestroyTextureArray(RenderTexture[] textureArray)
	{
		if (textureArray != null)
		{
			foreach (RenderTexture texture in textureArray)
			{
				DestroyTexture(texture);
			}
		}
		return null;
	}

	private bool UpdateMaterials()
	{
		_differenceMaterial = UpdateMaterial(_differenceMaterial, differenceShader);
		_speedUpdateMaterial = UpdateMaterial(_speedUpdateMaterial, speedUpdateShader);
		_integrationMaterial = UpdateMaterial(_integrationMaterial, integrationShader);
		_averageMaterial = UpdateMaterial(_averageMaterial, averageShader);
		return (bool)_differenceMaterial && (bool)_speedUpdateMaterial && (bool)_integrationMaterial;
	}

	private void DestroyMaterials()
	{
		_differenceMaterial = DestroyMaterial(_differenceMaterial);
		_speedUpdateMaterial = DestroyMaterial(_speedUpdateMaterial);
		_integrationMaterial = DestroyMaterial(_integrationMaterial);
		_averageMaterial = DestroyMaterial(_averageMaterial);
	}

	private static Material DestroyMaterial(Material material)
	{
		if (material != null)
		{
			Object.Destroy(material);
		}
		return null;
	}

	private static Material UpdateMaterial(Material oldMaterial, Shader shader)
	{
		if ((oldMaterial == null || oldMaterial.shader != shader) && shader != null)
		{
			return new Material(shader);
		}
		return oldMaterial;
	}
}
