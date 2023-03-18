using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private enum DisableRenderModeEnum
	{
		MainCameraDirect,
		CameraLiteManager
	}

	public enum ControllerType
	{
		None,
		FrontEnd,
		Test,
		Brawler,
		CardHub,
		CardGame,
		CardGameSA,
		HeadQuarters,
		SocialSpace,
		WorldMap,
		Cerebro,
		DeckBuilder,
		Debug_TimLand,
		Debug_AthanLand,
		Debug_AndyLand,
		GameShell,
		Fallback,
		CardGameDeckTest,
		RailsBrawler,
		RailsGameWorld,
		RailsHq,
		ArcadeShell
	}

	public bool isTestScene = true;

	public GameObject AppShellPrefab;

	public ControllerType controllerType;

	public TransactionMonitor StartTransaction;

	public float StartTransactionTimeout = float.MaxValue;

	protected ScenarioEventManager scenarioEventManager;

	protected ICharacterCache characterCache;

	protected List<KeyCodeEntry> debugKeys;

	private int sceneDisableRenderRequestStack;

	private GameObject cameraShuntObject;

	private DisableRenderModeEnum disableRenderMode;

	private int DisabledCullingMaskValue;

	protected GameObject localPlayer;

	protected bool bCallControllerReadyFromStart = true;

	public bool GuiFullScreenOverlayEnabled
	{
		get
		{
			return sceneDisableRenderRequestStack > 0;
		}
	}

	public virtual GameObject LocalPlayer
	{
		get
		{
			return localPlayer;
		}
		set
		{
			localPlayer = value;
			if (localPlayer == null || localPlayer.name != "mr_placeholder")
			{
				AppShell.Instance.EventMgr.Fire(this, new LocalPlayerChangedMessage(localPlayer));
			}
		}
	}

	public virtual string LocalCharacter
	{
		get
		{
			if (localPlayer == null)
			{
				return null;
			}
			SpawnData component = Utils.GetComponent<SpawnData>(localPlayer);
			if (component != null)
			{
				return component.spawner.CharacterName;
			}
			return null;
		}
	}

	public virtual ICharacterCache CharacterCache
	{
		get
		{
			return characterCache;
		}
	}

	protected void GCLog(string x)
	{
	}

	public virtual void Awake()
	{
		if (AppShell.Instance == null)
		{
			UnityEngine.Object @object = UnityEngine.Object.Instantiate(AppShellPrefab);
			@object.name = "_AppShell";
		}
		scenarioEventManager = new ScenarioEventManager();
		StartTransaction = AppShell.Instance.TransitionHandler.CurrentTransactionContext.Transaction;
	}

	protected virtual void OnStartTransactionComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		StartTransaction = null;
	}

	public virtual void Start()
	{
		AppShell.Instance.EventMgr.Fire(this, new NewControllerLoadingMessage(this));
		if (bCallControllerReadyFromStart)
		{
			ControllerReady();
		}
		AppShell.Instance.OnOldControllerUnloading += OnOldControllerUnloading;
	}

	public virtual void WatchMe(string titleText)
	{
		WaitWatcherEvent.SpawnWaitWatcher spawnWaitWatcher = new WaitWatcherEvent.SpawnWaitWatcher(StartTransaction);
		spawnWaitWatcher.titleText = titleText;
		WatchMe(spawnWaitWatcher);
	}

	public virtual void WatchMe(WaitWatcherEvent.SpawnWaitWatcher wwe)
	{
		if (AppShell.Instance.LaunchTransaction != null)
		{
			if (!isTestScene)
			{
				CspUtils.DebugLog("Adding controller transaction to appshell launch transaction.");
				AppShell.Instance.LaunchTransitionTransaction.AddChild(StartTransaction);
			}
			else
			{
				CspUtils.DebugLog("test mode. complete all launch transactions realted to login and redirect.");
				AppShell.Instance.LaunchLoginTransaction.CompleteStep("initialize");
			}
			AppShell.Instance.LaunchTransitionTransaction.CompleteStep("initialize");
		}
		else
		{
			AppShell.Instance.EventMgr.Fire(this, wwe);
		}
	}

	public virtual void OnEnable()
	{
		debugKeys = new List<KeyCodeEntry>();
		RegisterDebugKeys();
	}

	public virtual void OnDisable()
	{
		UnregisterDebugKeys();
		AppShell.Instance.EventMgr.Fire(this, new GameControllerExitedMessage(this));
		AppShell.Instance.OnOldControllerUnloading -= OnOldControllerUnloading;
		if (scenarioEventManager != null)
		{
			scenarioEventManager.Destroy();
			scenarioEventManager = null;
		}
	}

	protected virtual void RegisterDebugKeys()
	{
		KeyCodeEntry keyCodeEntry = new KeyCodeEntry(KeyCode.T, true, true, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, ParticleCount);
	}

	protected virtual void UnregisterDebugKeys()
	{
		if (debugKeys != null)
		{
			foreach (KeyCodeEntry debugKey in debugKeys)
			{
				SHSDebugInput.Inst.RemoveKeyListener(debugKey);
			}
		}
	}

	private void ParticleCount(SHSKeyCode code)
	{
		ParticleEmitter[] array = UnityEngine.Object.FindObjectsOfType(typeof(ParticleEmitter)) as ParticleEmitter[];
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		ParticleEmitter[] array2 = array;
		foreach (ParticleEmitter particleEmitter in array2)
		{
			if (particleEmitter.enabled && particleEmitter.gameObject.active)
			{
				num2 += particleEmitter.particleCount;
				if (particleEmitter.emit)
				{
					num++;
					num3 += (int)particleEmitter.maxEmission;
				}
			}
		}
		CspUtils.DebugLog("Emitters: " + num + "  Potential Particles: " + num3 + "  Current Particles: " + num2);
	}

	public void ControllerReady()
	{
		AppShell.Instance.EventMgr.Fire(this, new NewControllerReadyMessage(this));
		GC.Collect();
	}

	public virtual bool AddPlayerCharacterSpawner(CharacterSpawn possibleSpawner)
	{
		return true;
	}

	public virtual bool AllowTargetlessAttacks()
	{
		return true;
	}

	public virtual void OnOldControllerUnloading(AppShell.GameControllerTypeData currentGameData, AppShell.GameControllerTypeData newGameData)
	{
		if (currentGameData != null && newGameData != null)
		{
			AppShell.Instance.EventReporter.ReportTransition((currentGameData.sceneName == null) ? string.Empty : currentGameData.sceneName, (newGameData.sceneName == null) ? string.Empty : newGameData.sceneName);
		}
	}

	public virtual void OnNotificationResult(Hashtable msg)
	{
	}

	public virtual void OnCutSceneStart()
	{
		if ((bool)LocalPlayer)
		{
			LocalPlayer.SendMessage("OnCutSceneStart");
		}
	}

	public virtual void OnCutSceneEnd()
	{
		if ((bool)LocalPlayer)
		{
			LocalPlayer.SendMessage("OnCutSceneEnd");
		}
	}

	public virtual void DisableSceneRendering()
	{
		if (sceneDisableRenderRequestStack++ != 0)
		{
			return;
		}
		if (CameraLiteManager.Instance == null)
		{
			disableRenderMode = DisableRenderModeEnum.MainCameraDirect;
			GameObject gameObject = GameObject.FindGameObjectWithTag("MainCamera");
			if (gameObject != null)
			{
				DisabledCullingMaskValue = gameObject.camera.cullingMask;
				gameObject.camera.cullingMask = 0;
			}
		}
		else
		{
			disableRenderMode = DisableRenderModeEnum.CameraLiteManager;
			cameraShuntObject = new GameObject("Camera Shunt");
			CameraLite camera = cameraShuntObject.AddComponent<CameraLite>();
			cameraShuntObject.transform.position = new Vector3(-1000f, -1000f, -1000f);
			Vector3 forward = new Vector3(0f, -1f, 0f);
			cameraShuntObject.transform.rotation = Quaternion.LookRotation(forward);
			CameraLiteManager.Instance.OverrideCamera(camera, -1f);
		}
	}

	public virtual void EnableSceneRendering()
	{
		if (--sceneDisableRenderRequestStack != 0)
		{
			return;
		}
		switch (disableRenderMode)
		{
		case DisableRenderModeEnum.CameraLiteManager:
			CameraLiteManager.Instance.OverrideCamera(null, -1f);
			UnityEngine.Object.Destroy(cameraShuntObject);
			break;
		case DisableRenderModeEnum.MainCameraDirect:
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("MainCamera");
			if (gameObject != null)
			{
				gameObject.camera.cullingMask = DisabledCullingMaskValue;
			}
			break;
		}
		}
	}

	public static GameController GetController()
	{
		GameObject gameObject = GameObject.FindWithTag("GameController");
		if (gameObject == null)
		{
			CspUtils.DebugLog("Game controller prefab not found.");
			return null;
		}
		return Utils.GetComponent<GameController>(gameObject);
	}
}
