using UnityEngine;

public class HqAIProxy : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public HqRoom2 spawnRoom;

	public string characterName;

	public GameObject aiObject;

	public CharacterSpawn spawner;

	protected AIControllerHQ aiControllerComponent;

	protected Vector3 groundLocation;

	protected TransactionMonitor spawnTransaction;

	public bool SpawnCompleted
	{
		get
		{
			if (spawnTransaction != null)
			{
				return spawnTransaction.result != TransactionMonitor.ExitCondition.NotExited;
			}
			return false;
		}
	}

	public bool SpawnSucceeded
	{
		get
		{
			if (spawnTransaction != null)
			{
				return spawnTransaction.result == TransactionMonitor.ExitCondition.Success;
			}
			return false;
		}
	}

	public bool CanSwap
	{
		get
		{
			if (aiControllerComponent != null)
			{
				return aiControllerComponent.IsReadyForSwap;
			}
			return false;
		}
	}

	public static GameObject CreateProxy(string characterName)
	{
		GameObject characterPrefab = HqController2.Instance.GetCharacterPrefab(characterName);
		if (characterPrefab != null)
		{
			GameObject gameObject = Object.Instantiate(characterPrefab) as GameObject;
			if (gameObject != null)
			{
				if (gameObject.animation != null)
				{
					foreach (AnimationState item in gameObject.animation)
					{
						item.speed = 0f;
					}
				}
				BoxCollider boxCollider = Utils.AddComponent<BoxCollider>(gameObject);
				boxCollider.size = new Vector3(1f, 2f, 1f);
				boxCollider.center = new Vector3(0f, 1f, 0f);
				boxCollider.isTrigger = true;
				Rigidbody rigidbody = Utils.AddComponent<Rigidbody>(gameObject);
				rigidbody.mass = 3f;
				Utils.AddComponent<PhysicsInit>(gameObject);
				Utils.AddComponent<PlayAnimation>(gameObject);
				Utils.AddComponent<HqObject2>(gameObject);
				Utils.AddComponent<HqAIProxy>(gameObject);
				return gameObject;
			}
		}
		return null;
	}

	public void Spawn(string characterName, Vector3 position, HqRoom2 spawnRoom)
	{
		this.characterName = characterName;
		this.spawnRoom = spawnRoom;
		spawnRoom.AddProxy(this);
		groundLocation = position;
		RaycastHit hitInfo;
		if (Physics.Linecast(position, new Vector3(position.x, position.y - spawnRoom.ceilingHeight, position.z), out hitInfo, 106496))
		{
			groundLocation = hitInfo.point;
		}
		if (spawner == null)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load("Spawners/RemoteSpawner"), new Vector3(0f, 10000f, 0f), Quaternion.identity) as GameObject;
			spawner = (gameObject.GetComponent(typeof(CharacterSpawn)) as CharacterSpawn);
		}
		spawner.CharacterName = characterName;
		spawner.IsLocal = true;
		spawner.IsPlayer = false;
		spawner.IsAI = true;
		spawner.IsBoss = false;
		spawner.SpawnOnStart = true;
		spawner.DestroyOnSpawn = true;
		spawner.IsNetworked = false;
		spawner.prefabPlayerBillboard = null;
		spawner.onSpawnCallback += OnAISpawned;
		spawner.onTransactionInitialized += OnSpawnTransactionInitialized;
	}

	protected void OnSpawnTransactionInitialized(CharacterSpawn spawner, TransactionMonitor spawnTransaction)
	{
		this.spawnTransaction = spawnTransaction;
	}

	public void Swap()
	{
		if (aiControllerComponent != null)
		{
			bool paused = HqController2.Instance.State != typeof(HqController2.HqControllerFlinga);
			if (aiControllerComponent.Initialize(paused, spawnRoom))
			{
				aiControllerComponent.gameObject.transform.position = new Vector3(groundLocation.x, groundLocation.y, groundLocation.z);
				aiControllerComponent.gameObject.transform.rotation = base.gameObject.transform.rotation;
				AppShell.Instance.EventMgr.Fire(this, new HQHeroPlacedMessage(characterName, true));
				spawnRoom.SpawnAI(aiControllerComponent);
				HqController2.Instance.onEntitySpawned(aiObject);
			}
			DestroyProxy();
		}
	}

	public void DestroyProxy()
	{
		spawnRoom.RemoveProxy(this);
		spawnRoom.DelItem(base.gameObject);
		Object.Destroy(base.gameObject);
	}

	protected void OnAISpawned(GameObject go)
	{
		if (!SpawnSucceeded)
		{
			Object.Destroy(go);
			return;
		}
		aiObject = go;
		go.transform.position = new Vector3(0f, 10000f, 0f);
		AIControllerHQ[] components = Utils.GetComponents<AIControllerHQ>(aiObject, Utils.SearchChildren, true);
		if (components.Length > 0)
		{
			aiControllerComponent = components[0];
		}
	}
}
