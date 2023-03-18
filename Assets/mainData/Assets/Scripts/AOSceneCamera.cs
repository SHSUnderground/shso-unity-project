using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("Rendering/AO Scene Camera")]
[RequireComponent(typeof(DynamicShadowController))]
public class AOSceneCamera : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private const float TargetFPS = 30f;

	private RenderTexture[] DOFTemporaryBuffer = new RenderTexture[2];

	private RenderTexture FrameDiffuse;

	private RenderTexture FrameDepth;

	public Shader DepthShader;

	private Camera WorkingCamera;

	private DynamicShadowController ShadowController;

	public bool DepthOfField = true;

	private Material DOFMaterial;

	public Shader DOFShader;

	public float DOFTarget = 15f;

	public float DOFRange = 7f;

	public float DOFFalloff = 10f;

	public float DOFRadius = 2.5f;

	public int DOFPasses = 4;

	private Texture2D[] DOFNoise;

	public int DOFNoiseWidth = 64;

	public bool DOFPointSample;

	private int DOFNoiseOffset;

	public bool DOFAnimateNoise = true;

	public Color BackgroundColor = new Color(1f, 1f, 1f, 1f);

	private int ActualCullingMask;

	private bool EnableAutoPerfAdjustment;

	private Vector2 RenderTargetSize;

	private int TickCounter;

	private int PreDOFMask
	{
		get
		{
			return (!DepthOfField) ? ActualCullingMask : (ActualCullingMask & -268435459);
		}
	}

	private int PostDOFMask
	{
		get
		{
			return DepthOfField ? (ActualCullingMask & ~PreDOFMask) : 0;
		}
	}

	private CameraClearFlags ActualClearFlags
	{
		get
		{
			return (base.camera.clearFlags == CameraClearFlags.Skybox) ? CameraClearFlags.Skybox : CameraClearFlags.Color;
		}
	}

	private void Start()
	{
		ActualCullingMask = base.camera.cullingMask;
		base.camera.cullingMask = 0;
		ShadowController = Utils.GetComponent<DynamicShadowController>(base.gameObject);
		try
		{
			if (DepthShader == null)
			{
				DepthShader = Shader.Find("Hidden/Render Depth");
			}
			if (DOFShader == null || DOFShader.name == "Hidden/Render Depth")
			{
				DOFShader = Shader.Find("Hidden/Depth Of Field");
			}
		}
		catch (Exception ex)
		{
			CspUtils.DebugLogWarning("AOSceneCamera shader correction failed with error: " + ex.Message);
		}
		if (DOFShader != null)
		{
			DOFMaterial = new Material(DOFShader);
		}
		RenderTargetSize = new Vector2(base.camera.pixelWidth, base.camera.pixelHeight);
		if (EnableAutoPerfAdjustment)
		{
			StartCoroutine(AdjustResolution());
		}
		DepthOfField = GraphicsOptions.DOF;
	}

	private void OnEnable()
	{
		if (AppShell.Instance != null)
		{
			AppShell.Instance.EventMgr.AddListener<GUIResizeMessage>(OnResize);
		}
	}

	private void OnDisable()
	{
		if (AppShell.Instance != null)
		{
			AppShell.Instance.EventMgr.RemoveListener<GUIResizeMessage>(OnResize);
		}
		ReleaseTextures();
		if (WorkingCamera != null)
		{
			UnityEngine.Object.Destroy(WorkingCamera.gameObject);
			WorkingCamera = null;
		}
	}

	private void OnResize(GUIResizeMessage msg)
	{
		RenderTargetSize = new Vector2(base.camera.pixelWidth, base.camera.pixelHeight);
	}

	private void Update()
	{
		DepthOfField &= (DOFMaterial != null);
		if (DOFShader != DOFMaterial.shader)
		{
			DOFMaterial = new Material(DOFShader);
		}
		TickCounter++;
	}

	private RenderTexture MakeBuffer(RenderTextureFormat format)
	{
		return RenderTexture.GetTemporary((int)RenderTargetSize.x, (int)RenderTargetSize.y, 24, format);
	}

	private void BindDepthInput(RenderTexture texture)
	{
		Shader.SetGlobalTexture("_GlobalDepthTexture", texture);
	}

	private void OnPreCull()
	{
		ReleaseTextures();
		if (WorkingCamera == null)
		{
			WorkingCamera = new GameObject("AOSceneCamera Child").AddComponent<Camera>();
			WorkingCamera.enabled = false;
			WorkingCamera.gameObject.hideFlags = HideFlags.HideAndDontSave;
		}
		if (DOFTemporaryBuffer[0] == null)
		{
			for (int i = 0; i < DOFTemporaryBuffer.Length; i++)
			{
				DOFTemporaryBuffer[i] = MakeBuffer(RenderTextureFormat.ARGB32);
			}
			FrameDiffuse = MakeBuffer(RenderTextureFormat.ARGB32);
			FrameDepth = MakeBuffer(RenderTextureFormat.Depth);
		}
		WorkingCamera.CopyFrom(base.camera);
		WorkingCamera.transform.parent = base.transform;
		Shader.SetGlobalVector("_Target_TexelSize", new Vector4(1f / RenderTargetSize.x, 1f / RenderTargetSize.y, 0f, 0f));
		RenderFrame();
	}

	private void RenderFrame()
	{
		Shader.SetGlobalFloat("_GrabPassBugFix", 1f);
		if (!RenderShadowScene())
		{
			RenderNonShadowScene();
			if (DepthOfField)
			{
				RenderDepthBuffer();
			}
		}
		RenderDOF();
		RenderPostDOF();
		Shader.SetGlobalFloat("_GrabPassBugFix", 0f);
	}

	private bool RenderShadowScene()
	{
		if (ShadowController == null || !ShadowController.enabled)
		{
			return false;
		}
		DynamicShadowController.Renderer renderer = ShadowController.CreateRenderer(WorkingCamera);
		if (renderer == null)
		{
			return false;
		}
		using (renderer)
		{
			WorkingCamera.cullingMask = (0x2FF7BFED & PreDOFMask);
			WorkingCamera.backgroundColor = BackgroundColor;
			WorkingCamera.clearFlags = ActualClearFlags;
			WorkingCamera.targetTexture = FrameDepth;
			WorkingCamera.SetReplacementShader(DepthShader, "RenderType");
			WorkingCamera.Render();
			WorkingCamera.ResetReplacementShader();
			BindDepthInput(FrameDepth);
			WorkingCamera.targetTexture = FrameDiffuse;
			WorkingCamera.Render();
			WorkingCamera.clearFlags = CameraClearFlags.Nothing;
			renderer.RenderShadows();
			WorkingCamera.cullingMask = (int.MinValue | (0x10080012 & PreDOFMask));
			WorkingCamera.RenderDontRestore();
			if (DepthOfField)
			{
				WorkingCamera.targetTexture = FrameDepth;
				WorkingCamera.SetReplacementShader(DepthShader, "RenderType");
				WorkingCamera.Render();
				WorkingCamera.ResetReplacementShader();
				WorkingCamera.targetTexture = FrameDiffuse;
			}
		}
		return true;
	}

	private void RenderNonShadowScene()
	{
		WorkingCamera.cullingMask = PreDOFMask;
		WorkingCamera.backgroundColor = BackgroundColor;
		WorkingCamera.clearFlags = ActualClearFlags;
		WorkingCamera.targetTexture = FrameDiffuse;
		WorkingCamera.Render();
	}

	private void RenderDepthBuffer()
	{
		WorkingCamera.cullingMask = PreDOFMask;
		WorkingCamera.backgroundColor = Color.black;
		WorkingCamera.clearFlags = CameraClearFlags.Color;
		WorkingCamera.targetTexture = FrameDepth;
		WorkingCamera.SetReplacementShader(DepthShader, "RenderType");
		WorkingCamera.Render();
		WorkingCamera.ResetReplacementShader();
		WorkingCamera.targetTexture = FrameDiffuse;
	}

	private void RenderDOF()
	{
		if (!DepthOfField)
		{
			return;
		}
		int num = Mathf.Max(DOFPasses + (DOFPasses - 1), 1);
		if (DOFNoise == null || DOFNoise.Length != num || DOFNoise[0].width != DOFNoiseWidth)
		{
			DOFNoise = new Texture2D[num];
			for (int i = 0; i < DOFNoise.Length; i++)
			{
				DOFNoise[i] = MakeDOFNoiseTexture(DOFNoiseWidth, DOFNoiseWidth);
			}
		}
		DOFPasses = Math.Max(DOFPasses, 1);
		float num2 = 1f / DOFFalloff;
		float y = (0f - DOFTarget) * num2;
		float z = (0f - DOFRange) * num2;
		Shader.SetGlobalVector("_DOFParams", new Vector4(base.camera.farClipPlane * num2, y, z, DOFRadius));
		Shader.SetGlobalVector("_NoiseSize", new Vector4((float)FrameDiffuse.width / (float)DOFNoise[0].width, (float)FrameDiffuse.height / (float)DOFNoise[0].height, 0f, 0f));
		FrameDepth.filterMode = FilterMode.Point;
		if (DOFPointSample)
		{
			FrameDiffuse.filterMode = FilterMode.Point;
			RenderTexture[] dOFTemporaryBuffer = DOFTemporaryBuffer;
			foreach (RenderTexture renderTexture in dOFTemporaryBuffer)
			{
				renderTexture.filterMode = FilterMode.Point;
			}
		}
		RenderTexture renderTexture2 = (DOFPasses % 2 != 1) ? FrameDiffuse : DOFTemporaryBuffer[0];
		BindDepthInput(FrameDepth);
		for (int k = 0; k < DOFPasses; k++)
		{
			bool flag = k == 0;
			bool flag2 = k == DOFPasses - 1;
			Shader.SetGlobalTexture("_Noise", DOFNoise[(k + DOFNoiseOffset) % DOFNoise.Length]);
			RenderTexture source = (!flag) ? DOFTemporaryBuffer[(k - 1) % 2] : FrameDiffuse;
			RenderTexture dest = (!flag2) ? DOFTemporaryBuffer[k % 2] : renderTexture2;
			Graphics.Blit(source, dest, DOFMaterial);
		}
		if (renderTexture2 != FrameDiffuse)
		{
			Graphics.Blit(renderTexture2, FrameDiffuse);
		}
		if (DOFPointSample)
		{
			FrameDiffuse.filterMode = FilterMode.Bilinear;
			RenderTexture[] dOFTemporaryBuffer2 = DOFTemporaryBuffer;
			foreach (RenderTexture renderTexture3 in dOFTemporaryBuffer2)
			{
				renderTexture3.filterMode = FilterMode.Bilinear;
			}
		}
		FrameDepth.filterMode = FilterMode.Bilinear;
		if (DOFAnimateNoise)
		{
			DOFNoiseOffset++;
		}
	}

	private void RenderPostDOF()
	{
		if (PostDOFMask != 0)
		{
			WorkingCamera.cullingMask = PostDOFMask;
			WorkingCamera.clearFlags = CameraClearFlags.Nothing;
			WorkingCamera.Render();
		}
	}

	private static Texture2D MakeDOFNoiseTexture(int width, int height)
	{
		Texture2D texture2D = new Texture2D(width, height);
		for (int i = 0; i < height; i++)
		{
			for (int j = 0; j < width; j++)
			{
				Vector2 insideUnitCircle = UnityEngine.Random.insideUnitCircle;
				while (insideUnitCircle.magnitude < 1.401298E-44f)
				{
					insideUnitCircle = UnityEngine.Random.insideUnitCircle;
				}
				insideUnitCircle /= insideUnitCircle.magnitude;
				Vector2 insideUnitCircle2 = UnityEngine.Random.insideUnitCircle;
				while (insideUnitCircle2.magnitude < 1.401298E-44f)
				{
					insideUnitCircle2 = UnityEngine.Random.insideUnitCircle;
				}
				insideUnitCircle2 /= insideUnitCircle2.magnitude;
				texture2D.SetPixel(j, i, new Color(Mathf.Abs(insideUnitCircle.x), Mathf.Abs(insideUnitCircle.y), Mathf.Abs(insideUnitCircle2.x), Mathf.Abs(insideUnitCircle2.y)));
			}
		}
		texture2D.Apply();
		texture2D.filterMode = FilterMode.Point;
		return texture2D;
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit(FrameDiffuse, destination);
		ReleaseTextures();
	}

	private void ReleaseTextures()
	{
		if (FrameDiffuse != null)
		{
			for (int i = 0; i < DOFTemporaryBuffer.Length; i++)
			{
				RenderTexture.ReleaseTemporary(DOFTemporaryBuffer[i]);
				DOFTemporaryBuffer[i] = null;
			}
			RenderTexture.ReleaseTemporary(FrameDiffuse);
			FrameDiffuse = null;
			RenderTexture.ReleaseTemporary(FrameDepth);
			FrameDepth = null;
		}
	}

	private IEnumerator AdjustResolution()
	{
		yield return new WaitForSeconds(1f);
		Vector2 baseResolution = new Vector2(base.camera.pixelWidth, base.camera.pixelHeight);
		bool baseDOF = DepthOfField;
		int baseDOFPasses = DOFPasses;
		bool baseShadowsEnabled = ShadowController.enabled;
		Action[] resolution = (Action[])(object)new Action[4]
		{
			(Action)(object)(Action)delegate
			{
				RenderTargetSize = baseResolution;
			},
			(Action)(object)(Action)delegate
			{
				RenderTargetSize = baseResolution * 0.75f;
			},
			(Action)(object)(Action)delegate
			{
				RenderTargetSize = baseResolution * 0.5f;
			},
			(Action)(object)(Action)delegate
			{
				RenderTargetSize = baseResolution * 0.25f;
			}
		};
		Action[] dofPasses = (Action[])(object)new Action[3]
		{
			(Action)(object)(Action)delegate
			{
				DOFPasses = baseDOFPasses;
			},
			(Action)(object)(Action)delegate
			{
				DOFPasses = baseDOFPasses / 2;
			},
			(Action)(object)(Action)delegate
			{
				DOFPasses = 1;
			}
		};
		Action[] dof = (Action[])(object)new Action[2]
		{
			(Action)(object)(Action)delegate
			{
				DepthOfField = baseDOF;
			},
			(Action)(object)(Action)delegate
			{
				DepthOfField = false;
			}
		};
		Action[] shadows = (Action[])(object)new Action[2]
		{
			(Action)(object)(Action)delegate
			{
				ShadowController.enabled = baseShadowsEnabled;
			},
			(Action)(object)(Action)delegate
			{
				ShadowController.enabled = false;
			}
		};
		Action[][] strategies = new Action[4][]
		{
			resolution,
			dofPasses,
			dof,
			shadows
		};
		int[] currentLevels = new int[4];
		int currentStrategy = 0;
		while (true)
		{
			int startSampleTicks = TickCounter;
			float startSampleTime = Time.realtimeSinceStartup;
			yield return new WaitForSeconds(1f);
			int endSampleTicks = TickCounter;
			float endSampleTime = Time.realtimeSinceStartup;
			float sampleFPS = (float)(endSampleTicks - startSampleTicks) / (endSampleTime - startSampleTime);
			if (sampleFPS < 28.5f)
			{
				if (currentStrategy >= strategies.Length)
				{
					continue;
				}
				if (currentLevels[currentStrategy] < strategies[currentStrategy].Length - 1)
				{
					strategies[currentStrategy][++currentLevels[currentStrategy]].Invoke();
					yield return new WaitForSeconds(1f);
					int startAdjTicks2 = TickCounter;
					float startAdjTime2 = Time.realtimeSinceStartup;
					yield return new WaitForSeconds(1f);
					int endAdjTicks2 = TickCounter;
					float endAdjTime2 = Time.realtimeSinceStartup;
					float adjFPS2 = (float)(endAdjTicks2 - startAdjTicks2) / (endAdjTime2 - startAdjTime2);
					if (adjFPS2 < sampleFPS * 1.1f)
					{
						strategies[currentStrategy][--currentLevels[currentStrategy]].Invoke();
						currentStrategy++;
						yield return new WaitForSeconds(1f);
					}
				}
				else
				{
					currentStrategy++;
				}
			}
			else
			{
				if (!(sampleFPS > 33f))
				{
					continue;
				}
				if (currentStrategy < strategies.Length && currentLevels[currentStrategy] > 0)
				{
					strategies[currentStrategy][--currentLevels[currentStrategy]].Invoke();
					yield return new WaitForSeconds(1f);
					int startAdjTicks = TickCounter;
					float startAdjTime = Time.realtimeSinceStartup;
					yield return new WaitForSeconds(1f);
					int endAdjTicks = TickCounter;
					float endAdjTime = Time.realtimeSinceStartup;
					float adjFPS = (float)(endAdjTicks - startAdjTicks) / (endAdjTime - startAdjTime);
					if (adjFPS < 28.5f)
					{
						strategies[currentStrategy][++currentLevels[currentStrategy]].Invoke();
						yield return new WaitForSeconds(15f);
					}
				}
				else if (currentStrategy > 0)
				{
					currentStrategy--;
				}
			}
		}
	}
}
