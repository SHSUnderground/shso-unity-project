using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceholderSpawnInterrupt
{
	protected class SwapContext
	{
		public NewEntityMessage info;

		public GameObject placeholder;

		public GameObject real;
	}

	public delegate void CharacterSpawned(CharacterGlobals player);

	private CharacterSpawned _OnCharacterSpawned;

	protected Dictionary<int, SwapContext> contexts = new Dictionary<int, SwapContext>();

	protected string placeholderName;

	protected List<GameObject> swappers = new List<GameObject>();

	protected List<CharacterSpawn> activeSpawners = new List<CharacterSpawn>();

	public static float debug_swapDelay;

	public CharacterSpawned OnCharacterSpawned
	{
		get
		{
			return _OnCharacterSpawned;
		}
		set
		{
			_OnCharacterSpawned = value;
		}
	}

	public PlaceholderSpawnInterrupt(string placeholderName)
	{
		this.placeholderName = placeholderName;
	}

	public bool OnRemotePlayerSpawned(NewEntityMessage entityInfo)
	{
		if ((entityInfo.spawnType & CharacterSpawn.Type.Player) == 0)
		{
			return false;
		}
		SpawnPlaceholder(entityInfo);
		return true;
	}

	protected void SpawnPlaceholder(NewEntityMessage newEntity)
	{
		SwapContext swapContext = new SwapContext();
		swapContext.info = newEntity;
		contexts[newEntity.goNetId.ChildId] = swapContext;
		Spawn(newEntity, placeholderName, OnPlaceholderSpawned, false);
	}

	protected void OnPlaceholderSpawned(GameObject obj)
	{
		SwapContext context = GetContext(obj);
		if (context != null)
		{
			context.placeholder = obj;
			Spawn(context.info, context.info.modelName, OnRealSpawned, true);
		}
	}

	protected SwapContext GetContext(GameObject obj)
	{
		NetworkComponent component = Utils.GetComponent<NetworkComponent>(obj, Utils.SearchChildren);
		if (component != null)
		{
			return contexts[component.goNetId.ChildId];
		}
		CspUtils.DebugLog("Remote character spawned without a network component attached: deleting character");
		Object.Destroy(obj);
		return null;
	}

	protected void OnRealSpawned(GameObject obj)
	{
		SwapContext context = GetContext(obj);
		if (context != null)
		{
			context.real = obj;
			Utils.ActivateTree(obj, false);
			obj.transform.position = new Vector3(0f, 10000f, 0f);
			GameObject gameObject = new GameObject("PlaceholderSwapper");
			CoroutineContainer coroutineContainer = Utils.AddComponent<CoroutineContainer>(gameObject);
			swappers.Add(gameObject);
			coroutineContainer.StartCoroutine(SwapCharacter(gameObject, context));
		}
	}

	protected IEnumerator SwapCharacter(GameObject swapFacilitator, SwapContext context)
	{
		if (debug_swapDelay > 0f)
		{
			yield return new WaitForSeconds(debug_swapDelay);
		}
		if (context.placeholder == null)
		{
			CancelSwap(context, swapFacilitator);
			yield break;
		}
		BehaviorManager behaviorMan = Utils.GetComponent<BehaviorManager>(context.placeholder);
		while (!(behaviorMan.getBehavior() is BehaviorMovement) || !behaviorMan.currentBehaviorInterruptible(typeof(BehaviorMovement)))
		{
			yield return 0;
			if (behaviorMan == null)
			{
				CancelSwap(context, swapFacilitator);
				yield break;
			}
		}
		context.real.transform.position = context.placeholder.transform.position;
		context.real.transform.rotation = context.placeholder.transform.rotation;
		Object.Destroy(context.placeholder);
		Utils.ActivateTree(context.real, true);
		DisablePlayerCollision(context.real);
		contexts.Remove(context.info.goNetId.ChildId);
		swappers.Remove(swapFacilitator);
		Object.Destroy(swapFacilitator);
		activeSpawners.RemoveAll(delegate(CharacterSpawn spawn)
		{
			return spawn == null;
		});
		if (OnCharacterSpawned != null)
		{
			OnCharacterSpawned(Utils.GetComponent<CharacterGlobals>(context.real));
		}
	}

	protected void DisablePlayerCollision(GameObject obj)
	{
		Object[] array = Object.FindObjectsOfType(typeof(CharacterController));
		for (int i = 0; i < array.Length; i++)
		{
			CharacterController characterController = (CharacterController)array[i];
			if (characterController != obj.collider)
			{
				Physics.IgnoreCollision(characterController, obj.collider);
			}
		}
	}

	protected void Spawn(NewEntityMessage newEntity, string characterName, CharacterSpawn.OnSpawnCallback onFinished, bool copyMotionDataToPlaceholder)
	{
		GameObject gameObject = Object.Instantiate(Resources.Load("Spawners/RemoteSpawner"), newEntity.pos, newEntity.rot) as GameObject;
		CharacterSpawn characterSpawn = gameObject.GetComponent(typeof(CharacterSpawn)) as CharacterSpawn;
		
		
		if (characterSpawn == null)
			CspUtils.DebugLog("characterSpawn is null!!!");
		else
			CspUtils.DebugLog("characterSpawn is NOT null!!!");

		CharacterSpawn component = (CharacterSpawn)(gameObject.GetComponent(typeof(CharacterSpawn)));  // CSP - try this instead of above line.			
		if (component != null)
			CspUtils.DebugLog("component is NOT null!!!");
		else {
			CspUtils.DebugLog("component is null!!!");
			Component [] components = gameObject.GetComponents(typeof(Component));  // CSP - loop over all components because single get not working.
			for (int i = 0; i < components.Length; ++i) {
				//CspUtils.DebugLog("component name = " + components[i].name);
				//CspUtils.DebugLog("component type = " + components[i].GetType());	
				if (components[i].GetType().ToString() == "CharacterSpawn") {
					//CspUtils.DebugLog("component found!");
					component = (CharacterSpawn)(components[i]);
					break;
				}
			}
		}
		characterSpawn = component;
		
		
		
		characterSpawn.SetCharacterName(characterName);
		characterSpawn.IsLocal = false;
		characterSpawn.IsPlayer = ((newEntity.spawnType & CharacterSpawn.Type.Player) != 0);
		characterSpawn.IsAI = ((newEntity.spawnType & CharacterSpawn.Type.AI) != 0);
		characterSpawn.IsBoss = ((newEntity.spawnType & CharacterSpawn.Type.Boss) != 0);
		characterSpawn.SpawnOnStart = true;
		characterSpawn.DestroyOnSpawn = true;
		characterSpawn.goNetId = newEntity.goNetId;
		characterSpawn.netExtraData = newEntity.extraData;
		characterSpawn.onSpawnCallback += onFinished;
		if (copyMotionDataToPlaceholder)
		{
			SwapContext swapContext = contexts[characterSpawn.goNetId.ChildId];
			GameObject placeholder = swapContext.placeholder;
			CharacterMotionController mc = Utils.GetComponent<CharacterMotionController>(placeholder);
			characterSpawn.onRealCharacterDataLoaded = delegate(DataWarehouse data)
			{
				mc.InitializePlaceholderFromData(data);
			};
		}
		if (newEntity.extraData.ContainsKey("RecordHistory"))
		{
			characterSpawn.RecordHistory = true;
		}
		else
		{
			characterSpawn.RecordHistory = false;
		}
		activeSpawners.Add(characterSpawn);
	}

	public void CancelAll()
	{
		foreach (CharacterSpawn activeSpawner in activeSpawners)
		{
			if (activeSpawner != null)
			{
				activeSpawner.StopAllCoroutines();
				Object.Destroy(activeSpawner.gameObject);
			}
		}
		foreach (GameObject swapper in swappers)
		{
			Utils.GetComponent<CoroutineContainer>(swapper).StopAllCoroutines();
			Object.Destroy(swapper);
		}
		foreach (SwapContext value in contexts.Values)
		{
			CancelContext(value);
		}
		contexts.Clear();
	}

	protected void CancelContext(SwapContext context)
	{
		if (context.placeholder != null)
		{
			Object.Destroy(context.placeholder);
		}
		if (context.real != null)
		{
			Object.Destroy(context.real);
		}
	}

	protected void CancelSwap(SwapContext context, GameObject swapFacilitator)
	{
		CancelContext(context);
		if (swapFacilitator != null)
		{
			Object.Destroy(swapFacilitator);
		}
	}
}
