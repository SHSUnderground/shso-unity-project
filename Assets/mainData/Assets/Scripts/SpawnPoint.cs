using UnityEngine;

public class SpawnPoint : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	[HideInInspector]
	public string group = "start";

	public SpawnPointGroup owner;

	public float cameraBlend = -1f;

	public CameraLite cameraOverride;

	public bool cameraStartOnMe;

	public bool keepCameraOnStack;

	public void Awake()
	{
		CspUtils.DebugLog("SpawnPoint gameObject.name="  + gameObject.name);
		CspUtils.DebugLog("SpawnPoint group="  + group);
		foreach (Transform item in base.transform)
		{
			Object.Destroy(item.gameObject);
		}
	}

	public void Start()
	{
		group = "start";  // CSP - override inspector value

		if (owner != null)
		{
			if (cameraOverride == null)
			{
				cameraOverride = owner.cameraOverride;
			}
			if (cameraBlend == -1f)
			{
				cameraBlend = owner.cameraBlend;
			}
			if (!cameraStartOnMe)
			{
				cameraStartOnMe = owner.cameraStartOnMe;
			}
		}
		if (cameraOverride != null || cameraBlend != -1f)
		{
			cameraStartOnMe = false;
		}
		if (SocialSpaceController.Instance != null)
		{
			SocialSpaceController.Instance.AddSpawnPoint(this);
		}
	}

	public void SpawnPlayer(string characterName, CharacterSpawn.OnSpawnCallback spawnCallback)
	{
		SpawnPlayer(characterName, spawnCallback, null, null);
	}

	public void SpawnPlayer(string characterName, CharacterSpawn.OnSpawnCallback spawnCallback, CharacterSpawn.OnTransactionInitialized transactionCallback, GameObject placeholder)
	{
		CspUtils.DebugLog("SpawnPlayer characterName=" + characterName);
		GameObject gameObject = Object.Instantiate(Resources.Load("Spawners/RemoteSpawner"), base.transform.position, base.transform.rotation) as GameObject;
		if (gameObject == null)
			CspUtils.DebugLog("SpawnPlayer gameObject is null!");
		CharacterSpawn characterSpawn = gameObject.GetComponent(typeof(CharacterSpawn)) as CharacterSpawn;
		if (characterSpawn != null)
			CspUtils.DebugLog("characterSpawn is NOT null!!!");
		else {
			CspUtils.DebugLog("characterSpawn is null!!!");
			Component [] components = gameObject.GetComponents(typeof(Component));  // CSP - loop over all components because single get not working.
			for (int i = 0; i < components.Length; ++i) {
				CspUtils.DebugLog("component name = " + components[i].name);
				CspUtils.DebugLog("component type = " + components[i].GetType());	
				if (components[i].GetType().ToString() == "CharacterSpawn") {
					CspUtils.DebugLog("characterSpawn found!");
					characterSpawn = (CharacterSpawn)(components[i]);
					break;
				}
			}
		}


		characterSpawn.SetCharacterName(characterName);
		characterSpawn.IsNetworked = true;
		characterSpawn.IsLocal = true;
		characterSpawn.IsPlayer = true;
		characterSpawn.IsAI = false;
		characterSpawn.IsBoss = false;
		characterSpawn.SpawnOnStart = true;
		characterSpawn.DestroyOnSpawn = true;
		characterSpawn.netExtraData = null;
		characterSpawn.RecordHistory = false;
		characterSpawn.ForceLocalPlayerSpawnOnStart = true;
		CspUtils.DebugLog("spawnpoint placeholder: " + placeholder);
		if (placeholder != null)
		{
			CharacterMotionController mc = Utils.GetComponent<CharacterMotionController>(placeholder);
			CspUtils.DebugLog("spawnpoint mc: " + mc);
			characterSpawn.onRealCharacterDataLoaded = delegate(DataWarehouse data)
			{
				mc.InitializePlaceholderFromData(data);
			};
		}
		CspUtils.DebugLog("spawnpoint spawnCallback: " + spawnCallback);
		if (spawnCallback != null)
		{
			characterSpawn.onSpawnCallback += spawnCallback;
		}
		CspUtils.DebugLog("spawnpoint keepCameraOnStack: " + keepCameraOnStack);
		if (!keepCameraOnStack)
		{
			characterSpawn.onSpawnCallback += RestoreCamera;
		}
	
		characterSpawn.onSpawnCallback += OnFinishedSpawningPlayer;
		CspUtils.DebugLog("spawnpoint transactionCallback: " + transactionCallback);
		if (transactionCallback != null)
		{
			characterSpawn.onTransactionInitialized += transactionCallback;
		}
	}

	public void SpawnNPC(string characterName)
	{
		CspUtils.DebugLog("Spawning NPC: " + characterName);
		GameObject gameObject = Object.Instantiate(Resources.Load("Spawners/RemoteSpawner"), base.transform.position, base.transform.rotation) as GameObject;
		CharacterSpawn characterSpawn = gameObject.GetComponent(typeof(CharacterSpawn)) as CharacterSpawn;
		characterSpawn.SetCharacterName(characterName);
		characterSpawn.IsNetworked = false;
		characterSpawn.IsLocal = true;
		characterSpawn.IsPlayer = true;
		characterSpawn.IsAI = false;
		characterSpawn.IsNpc = true;
		characterSpawn.IsBoss = false;
		characterSpawn.SpawnOnStart = true;
		characterSpawn.DestroyOnSpawn = true;
		characterSpawn.netExtraData = null;
		characterSpawn.RecordHistory = false;
		characterSpawn.ForceLocalPlayerSpawnOnStart = true;
	}

	public void SetCamera()
	{
		if (cameraOverride != null)
		{
			CameraLiteManager.Instance.PushCamera(cameraOverride, -1f);
		}
		else
		{
			if (!cameraStartOnMe)
			{
				return;
			}
			CameraLiteSocialSpace cameraLiteSocialSpace = CameraLiteManager.Instance.GetCurrentCamera() as CameraLiteSocialSpace;
			if (cameraLiteSocialSpace != null)
			{
				CameraTarget component = Utils.GetComponent<CameraTarget>(cameraLiteSocialSpace);
				if (component != null)
				{
					component.Target = base.transform;
					cameraLiteSocialSpace.Reset();
				}
			}
			else
			{
				CspUtils.DebugLog("The starting camera is not a CameraLiteSocialSpace");
			}
		}
	}

	protected void RestoreCamera(GameObject newPlayer)
	{
		if (cameraOverride != null)
		{
			CameraLiteManager.Instance.PopCamera(cameraBlend);
		}
	}

	protected void OnFinishedSpawningPlayer(GameObject newPlayer)
	{
		base.gameObject.SendMessage("OnPlayerSpawned", newPlayer, SendMessageOptions.DontRequireReceiver);
	}
}
