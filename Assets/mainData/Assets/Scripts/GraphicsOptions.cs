using System.Text;
using UnityEngine;

public class GraphicsOptions
{
	public enum GraphicsQuality
	{
		Low,
		Medium,
		High,
		Excellent
	}

	public enum ShadowLevel
	{
		None,
		Blob,
		Projected
	}

	protected static int profileVersion;

	protected static GameObject blobShadowPrefab;

	protected static bool suppressEvent;

	protected static bool dofEnabled;

	protected static bool prestigeEffectsEnabled = true;

	protected static ShadowLevel shadowLevel;

	protected static int maxLod;

	protected static GraphicsQuality modelQuality;

	protected static QualityLevel renderFidelity;

	public static bool DOF
	{
		get
		{
			return dofEnabled;
		}
		set
		{
			if (dofEnabled != value)
			{
				SetDOF(value);
			}
		}
	}

	public static bool IsDOFAvailable
	{
		get
		{
			return SystemInfo.supportsShadows;
		}
	}

	public static bool PrestigeEffects
	{
		get
		{
			return prestigeEffectsEnabled;
		}
		set
		{
			if (prestigeEffectsEnabled != value)
			{
				SetPrestige(value);
			}
		}
	}

	public static ShadowLevel Shadows
	{
		get
		{
			return shadowLevel;
		}
		set
		{
			if (shadowLevel != value)
			{
				SetShadowLevel(value);
			}
		}
	}

	public static bool IsProjectedShadowAvailable
	{
		get
		{
			return SystemInfo.supportsShadows;
		}
	}

	public static int MaxLod
	{
		get
		{
			return maxLod;
		}
	}

	public static GraphicsQuality ModelQuality
	{
		get
		{
			return modelQuality;
		}
		set
		{
			if (modelQuality != value)
			{
				SetModelQuality(value);
			}
		}
	}

	public static QualityLevel RenderFidelity
	{
		get
		{
			return renderFidelity;
		}
		set
		{
			if (renderFidelity != value)
			{
				SetRenderFidelity(value);
			}
		}
	}

	public static void Load(int version)
	{
		blobShadowPrefab = (GameObject)Resources.Load("Character/BlobShadow");
		suppressEvent = true;
		profileVersion = Mathf.Clamp(version, 1, int.MaxValue);
		SetDOF((ShsPlayerPrefs.GetInt(ShsPlayerPrefs.Keys.DOF) > 0) ? true : false);
		SetPrestige((ShsPlayerPrefs.GetInt(ShsPlayerPrefs.Keys.DisablePrestigeEffects) <= 0) ? true : false);
		SetShadowLevel((ShadowLevel)Mathf.Clamp(ShsPlayerPrefs.GetInt(ShsPlayerPrefs.Keys.ShadowLevel), 0, 2));
		SetRenderFidelity((QualityLevel)ShsPlayerPrefs.GetInt(ShsPlayerPrefs.Keys.RenderFidelity, 4));
		SetModelQuality((GraphicsQuality)ShsPlayerPrefs.GetInt(ShsPlayerPrefs.Keys.GraphicsQuality, 3));
		if (ShsPlayerPrefs.GetInt(ShsPlayerPrefs.Keys.ProfiledGraphics) < profileVersion)
		{
			Reset();
		}
		suppressEvent = false;
		FireChangedEvent();
	}

	public static void Reset()
	{
		suppressEvent = true;
		ShsPlayerPrefs.SetInt(ShsPlayerPrefs.Keys.ProfiledGraphics, profileVersion);
		int max = 3;
		if (!SystemInfo.supportsImageEffects || !SystemInfo.supportsShadows || !SystemInfo.supportsVertexPrograms || !SystemInfo.supportsRenderTextures)
		{
			max = Mathf.Clamp(0, 0, max);
		}
		if (SystemInfo.graphicsShaderLevel < 30)
		{
			max = Mathf.Clamp(0, 0, max);
		}
		max = ((SystemInfo.graphicsMemorySize <= 64) ? Mathf.Clamp(0, 0, max) : ((SystemInfo.graphicsMemorySize <= 128) ? Mathf.Clamp(1, 0, max) : ((SystemInfo.graphicsMemorySize > 256) ? Mathf.Clamp(3, 0, max) : Mathf.Clamp(2, 0, max))));
		switch ((SystemInfo.graphicsPixelFillrate < 0) ? Mathf.Clamp(3, 0, max) : ((SystemInfo.graphicsPixelFillrate <= 3000) ? Mathf.Clamp(1, 0, max) : ((SystemInfo.graphicsPixelFillrate > 10000) ? Mathf.Clamp(3, 0, max) : Mathf.Clamp(2, 0, max))))
		{
		default:
			SetLowDefaults();
			break;
		case 1:
			SetMediumDefaults();
			break;
		case 2:
			SetHighDefaults();
			break;
		case 3:
			SetExcellentDefaults();
			break;
		}
		suppressEvent = false;
		FireChangedEvent();
	}

	public static void AddPlayerShadow(GameObject target)
	{
		switch (Shadows)
		{
		case ShadowLevel.None:
			break;
		case ShadowLevel.Blob:
			AddBlobShadow(target);
			break;
		case ShadowLevel.Projected:
			AddProjectedShadow(target);
			break;
		}
	}

	public static Resolution FindBestResolution()
	{
		StringBuilder stringBuilder = new StringBuilder();
		float num = (float)Screen.currentResolution.width / (float)Screen.currentResolution.height;
		stringBuilder.AppendFormat("Monitor Res: {0} x {1}  {2}", Screen.currentResolution.width, Screen.currentResolution.height, Screen.currentResolution.refreshRate);
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Game Window: {0} x {1}", Screen.width, Screen.height);
		stringBuilder.AppendLine();
		int num2 = 1020;
		int num3 = 0;
		float num4 = float.MaxValue;
		int num5 = int.MaxValue;
		Resolution[] resolutions = Screen.resolutions;
		for (int i = 0; i < resolutions.Length; i++)
		{
			Resolution resolution = resolutions[i];
			float num6 = (float)resolution.width / (float)resolution.height;
			float num7 = Mathf.Abs(num6 - num);
			int num8 = Mathf.Abs(resolution.width - num2);
			if (num7 <= num4)
			{
				if (num7 < num4 || num8 <= num5)
				{
					stringBuilder.AppendFormat("  C: {0} x {1} aspectDelta = {2}, resDelta = {3}", resolution.width, resolution.height, num7, num8);
					stringBuilder.AppendLine();
					num3 = i;
					num4 = num7;
					num5 = num8;
				}
				else
				{
					stringBuilder.AppendFormat("  R: {0} x {1} aspectDelta = {2}, resDelta = {3}", resolution.width, resolution.height, num7, num8);
					stringBuilder.AppendLine();
				}
			}
		}
		stringBuilder.AppendFormat("Final Selection: {0} x {1}", resolutions[num3].width, resolutions[num3].height);
		stringBuilder.AppendLine();
		CspUtils.DebugLog(stringBuilder.ToString());
		return resolutions[num3];
	}

	protected static void SetLowDefaults()
	{
		CspUtils.DebugLog("Setting ultra low graphics options");
		RenderFidelity = QualityLevel.Fast;
		DOF = false;
		Shadows = ShadowLevel.None;
		PrestigeEffects = false;
		ModelQuality = GraphicsQuality.Low;
	}

	protected static void SetMediumDefaults()
	{
		CspUtils.DebugLog("Setting low graphics options");
		RenderFidelity = QualityLevel.Simple;
		DOF = false;
		Shadows = ShadowLevel.None;
		PrestigeEffects = false;
		ModelQuality = GraphicsQuality.Medium;
	}

	protected static void SetHighDefaults()
	{
		CspUtils.DebugLog("Setting medium graphics options");
		RenderFidelity = QualityLevel.Good;
		DOF = false;
		Shadows = ShadowLevel.Blob;
		PrestigeEffects = true;
		ModelQuality = GraphicsQuality.High;
	}

	protected static void SetExcellentDefaults()
	{
		CspUtils.DebugLog("Setting high graphics options");
		RenderFidelity = QualityLevel.Beautiful;
		DOF = true;
		Shadows = ShadowLevel.Projected;
		PrestigeEffects = true;
		ModelQuality = GraphicsQuality.Excellent;
	}

	protected static void FireChangedEvent()
	{
		if (AppShell.Instance != null && !suppressEvent)
		{
			AppShell.Instance.EventMgr.Fire(null, new GraphicsOptionsChange());
		}
	}

	protected static void SetDOF(bool value)
	{
		if (!IsDOFAvailable)
		{
			value = false;
		}
		dofEnabled = value;
		if (dofEnabled)
		{
			ShsPlayerPrefs.SetInt(ShsPlayerPrefs.Keys.DOF, 1);
		}
		else
		{
			ShsPlayerPrefs.SetInt(ShsPlayerPrefs.Keys.DOF, 0);
		}
		Object[] array = Object.FindSceneObjectsOfType(typeof(AOSceneCamera));
		foreach (Object @object in array)
		{
			AOSceneCamera aOSceneCamera = (AOSceneCamera)@object;
			aOSceneCamera.DepthOfField = dofEnabled;
		}
		FireChangedEvent();
	}

	protected static void SetPrestige(bool value)
	{
		prestigeEffectsEnabled = value;
		if (prestigeEffectsEnabled)
		{
			ShsPlayerPrefs.SetInt(ShsPlayerPrefs.Keys.DisablePrestigeEffects, 0);
		}
		else
		{
			ShsPlayerPrefs.SetInt(ShsPlayerPrefs.Keys.DisablePrestigeEffects, 1);
		}
		FireChangedEvent();
	}

	protected static void SetModelQuality(GraphicsQuality value)
	{
		modelQuality = value;
		maxLod = (int)(GraphicsOptions.GraphicsQuality.Excellent - modelQuality);
		ShsPlayerPrefs.SetInt(ShsPlayerPrefs.Keys.GraphicsQuality, (int)modelQuality);
		FireChangedEvent();
	}

	protected static void SetRenderFidelity(QualityLevel level)
	{
		renderFidelity = level;
		ShsPlayerPrefs.SetInt(ShsPlayerPrefs.Keys.RenderFidelity, (int)renderFidelity);
		if (Application.platform == RuntimePlatform.OSXWebPlayer)
		{
			renderFidelity = (QualityLevel)Mathf.Clamp((int)renderFidelity, 0, 2);
			CspUtils.DebugLog("Running on Mac, forcing render fidelity to " + renderFidelity.ToString());
		}
		QualitySettings.currentLevel = renderFidelity;
		FireChangedEvent();
	}

	protected static void SetShadowLevel(ShadowLevel value)
	{
		if (value == ShadowLevel.Projected && !IsProjectedShadowAvailable)
		{
			value = ShadowLevel.Blob;
		}
		shadowLevel = value;
		ShsPlayerPrefs.SetInt(ShsPlayerPrefs.Keys.ShadowLevel, (int)shadowLevel);
		Object[] players = Object.FindSceneObjectsOfType(typeof(ShsCharacterController));
		switch (shadowLevel)
		{
		case ShadowLevel.None:
			RemoveBlobShadows(players);
			RemoveProjectedShadows(players);
			break;
		case ShadowLevel.Blob:
			RemoveBlobShadows(players);
			RemoveProjectedShadows(players);
			AddBlobShadows(players);
			break;
		case ShadowLevel.Projected:
			RemoveBlobShadows(players);
			RemoveProjectedShadows(players);
			AddProjectedShadows(players);
			break;
		}
		if (HqController2.Instance != null)
		{
			HqController2.Instance.ResetShadows();
		}
		FireChangedEvent();
	}

	protected static void AddBlobShadows(Object[] players)
	{
		for (int i = 0; i < players.Length; i++)
		{
			ShsCharacterController shsCharacterController = (ShsCharacterController)players[i];
			AddBlobShadow(shsCharacterController.gameObject);
		}
	}

	protected static void AddBlobShadow(GameObject target)
	{
		if (!(GameController.GetController() is HqController2))
		{
			SpawnData componentInChildren = target.GetComponentInChildren<SpawnData>();
			if (!(componentInChildren != null) || (componentInChildren.spawnType & CharacterSpawn.Type.NPC) == 0)
			{
				GameObject child = Object.Instantiate(blobShadowPrefab) as GameObject;
				Utils.AttachGameObject(target.gameObject, child);
			}
		}
	}

	protected static void RemoveBlobShadows(Object[] players)
	{
		for (int i = 0; i < players.Length; i++)
		{
			ShsCharacterController shsCharacterController = (ShsCharacterController)players[i];
			BlobShadowController componentInChildren = shsCharacterController.GetComponentInChildren<BlobShadowController>();
			if (componentInChildren != null && componentInChildren.gameObject != null)
			{
				Projector componentInChildren2 = componentInChildren.gameObject.GetComponentInChildren<Projector>();
				if (componentInChildren2 != null)
				{
					Object.Destroy(componentInChildren2.gameObject);
				}
			}
			if (componentInChildren != null)
			{
				Object.Destroy(componentInChildren);
			}
		}
	}

	protected static void AddProjectedShadows(Object[] players)
	{
		if (Camera.main != null)
		{
			DynamicShadowController component = Camera.main.GetComponent<DynamicShadowController>();
			if (component != null)
			{
				component.enabled = true;
			}
		}
		for (int i = 0; i < players.Length; i++)
		{
			ShsCharacterController shsCharacterController = (ShsCharacterController)players[i];
			AddProjectedShadow(shsCharacterController.gameObject);
		}
	}

	protected static void AddProjectedShadow(GameObject target)
	{
		if (GameController.GetController() is HqController2)
		{
			return;
		}
		SpawnData componentInChildren = target.GetComponentInChildren<SpawnData>();
		if (!(componentInChildren != null) || (componentInChildren.spawnType & CharacterSpawn.Type.NPC) == 0)
		{
			SkinnedMeshRenderer[] componentsInChildren = target.GetComponentsInChildren<SkinnedMeshRenderer>(true);
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in componentsInChildren)
			{
				skinnedMeshRenderer.gameObject.AddComponent<DynamicShadowTarget>();
			}
		}
	}

	protected static void RemoveProjectedShadows(Object[] players)
	{
		for (int i = 0; i < players.Length; i++)
		{
			ShsCharacterController shsCharacterController = (ShsCharacterController)players[i];
			DynamicShadowTarget[] componentsInChildren = shsCharacterController.GetComponentsInChildren<DynamicShadowTarget>(true);
			foreach (DynamicShadowTarget obj in componentsInChildren)
			{
				Object.Destroy(obj);
			}
		}
		if (Camera.main != null)
		{
			DynamicShadowController component = Camera.main.GetComponent<DynamicShadowController>();
			if (component != null)
			{
				component.enabled = false;
			}
		}
	}
}
