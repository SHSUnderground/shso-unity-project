using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class AIControllerHQ : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum AiMode
	{
		Sim,
		Active
	}

	public enum Mood
	{
		Happy = 20,
		Pleasant = 40,
		Content = 60,
		Indifferent = 80,
		Disgruntled = 90,
		Enraged = 100
	}

	internal class PotentialActivity
	{
		public HqItem item;

		public int calculatedAffinity;

		public int cumulativeAffinity;
	}

	public class LogMask
	{
		public const int None = 0;

		public const int ChooseActivity = 1;

		public const int CheckingItem = 2;

		public const int ItemUsability = 4;

		public const int ChooseActivityVerbose = 8;

		public const int ChangingRooms = 16;

		public const int All = 31;
	}

	private const int DEFAULT_ROOM_AFFINITY = 3;

	private const int DEFAULT_ROOM_TIMEOUT = 3;

	private const float MAX_INTERACT_DIST_SQRD = 10f;

	private const float MIN_INTERACT_DIST_SQRD = 4f;

	private const float USE_COOLDOWN = 5f;

	private const float INTERACTION_WAIT_TIME = 10f;

	private const float DEFAULT_MASS = 3f;

	private const int DEFAULT_CONSUMABLE_AFFINITY = 10;

	private const int DEFAULT_ITEM_AFFINITY = 3;

	private const float MOPE_MIN_TIME = 5f;

	private const float MOPE_MAX_TIME = 10f;

	private const float THROW_COOLDOWN = 5f;

	protected AIControllerHQData aiData;

	protected HqRoom2 currentRoom;

	protected HqRoom2 spawnRoom;

	protected CharacterGlobals charGlobals;

	protected BehaviorManager behaviorManager;

	protected bool paused;

	protected AiMode currentMode;

	protected PathFinder currentPathfinder;

	protected HqObject2 placeHolderObject;

	protected bool flingaObjectIsVisible;

	protected Bounds localBounds;

	protected bool inJail;

	protected Vector3 lastKnownPosition = Vector3.zero;

	protected bool isReadyForSwap;

	protected Renderer[] renderers;

	protected HqRoom2 lastRoom;

	protected bool selected;

	private LodCharacter lodCharacter;

	private HqHappinessEffect happinessEffect;

	private float currentHunger;

	protected float currentActivityTime;

	protected float currentActivityStartTime;

	protected float currentRoomTime;

	protected float currentRoomEndTime;

	protected HqItem.Claim currentClaim;

	protected EntryPoint currentItemEntryPoint;

	protected float lastInteractionTime;

	protected AIControllerHQ interactingWith;

	protected HqItem currentActivityItem;

	protected HqItem lastUsedActivityItem;

	protected List<HqItem> itemsInControl = new List<HqItem>();

	protected Dictionary<HqItem, float> itemUseTimes = new Dictionary<HqItem, float>();

	protected bool needNewActivity;

	protected CharacterDefinition characterDefinition;

	protected GameObject attachedObject;

	protected float nextThrowTime;

	protected bool unloading;

	public static bool showBehaviors;

	private string _CurrentCaption;

	private int CurrentLogMask = 1;

	public bool IsReadyForSwap
	{
		get
		{
			return isReadyForSwap;
		}
	}

	public bool Paused
	{
		get
		{
			return paused;
		}
		set
		{
			bool flag = paused;
			paused = value;
			if (paused && !flag)
			{
				BehaviorBase behavior = BehaviorManager.getBehavior();
				if (behavior != null)
				{
					behavior.Paused = true;
				}
				foreach (AnimationState item in base.animation)
				{
					item.speed = 0f;
				}
				if (FlingaObject != null)
				{
					FlingaObject.GotoPlacementMode(true);
				}
				needNewActivity = false;
			}
			else
			{
				if (paused || !flag)
				{
					return;
				}
				foreach (AnimationState item2 in base.animation)
				{
					item2.speed = 1f;
				}
				if (IsInActiveRoom)
				{
					BehaviorBase behavior2 = BehaviorManager.getBehavior();
					Type type = behavior2.GetType();
					if (type == typeof(BehaviorPaused) || type == typeof(BehaviorMope))
					{
						needNewActivity = true;
					}
					if (type == typeof(BehaviorSimulate))
					{
						needNewActivity = true;
						BehaviorSimulate behaviorSimulate = behavior2 as BehaviorSimulate;
						if (behaviorSimulate.currentClaim != null)
						{
							Log("Reusing BehaviorSimulate claim on " + currentActivityItem.gameObject.name, 4);
							currentActivityItem = behaviorSimulate.currentClaim.item;
							currentClaim = behaviorSimulate.currentClaim;
							currentClaim.Claimer = this;
						}
					}
					if (needNewActivity)
					{
						ChooseActivity();
					}
					else
					{
						behavior2.Paused = false;
					}
				}
				else
				{
					BehaviorSimulate behaviorSimulate2 = ChangeBehavior<BehaviorSimulate>(true);
					behaviorSimulate2.Initialize(this, OnEnterActiveRoom);
				}
				DeactivateFlinga();
			}
		}
	}

	public HqRoom2 LastRoom
	{
		get
		{
			return lastRoom;
		}
	}

	public HqRoom2 CurrentRoom
	{
		get
		{
			return currentRoom;
		}
		set
		{
			if (currentRoom != value)
			{
				if (currentRoom != null && value != null)
				{
					Log("Hero is changing rooms from " + currentRoom.Id + " to " + value.Id, 16);
				}
				lastRoom = currentRoom;
				if (itemsInControl != null && itemsInControl.Count > 0)
				{
					foreach (HqItem item in itemsInControl)
					{
						ReleaseControl(item);
					}
				}
				if (currentRoom != null)
				{
					currentRoom.RemoveAI(this);
				}
				currentRoom = value;
				currentRoomTime = 0f;
				if (currentRoom != null)
				{
					currentRoom.AddAI(this);
					if (IsInActiveRoom)
					{
						currentPathfinder = currentRoom.FindClosestPathFinder(base.gameObject.transform.position);
						Activate();
					}
					else
					{
						DeActivate();
						if (HqController2.Instance.Input.IsHeroCamTarget(this))
						{
							lastKnownPosition = currentRoom.RandomDoor;
						}
						else
						{
							lastKnownPosition = currentRoom.RandomLocation;
						}
					}
					AppShell.Instance.EventMgr.Fire(base.gameObject, new HQAIRoomChanged(base.gameObject, currentRoom));
				}
			}
			if (lastUsedActivityItem != null)
			{
				lastUsedActivityItem = null;
			}
		}
	}

	public HqRoom2 SpawnRoom
	{
		get
		{
			return spawnRoom;
		}
		set
		{
			spawnRoom = value;
		}
	}

	public AiMode Mode
	{
		get
		{
			return currentMode;
		}
	}

	public string InventoryIcon
	{
		get
		{
			if (CharGlobals.definitionData != null)
			{
				return CharGlobals.definitionData.InventoryIconName;
			}
			return null;
		}
	}

	public string Caption
	{
		get
		{
			return _CurrentCaption;
		}
		set
		{
			if (_CurrentCaption != value)
			{
				TextMesh component = Utils.GetComponent<TextMesh>(base.gameObject, Utils.SearchChildren);
				if (component != null)
				{
					component.text = value;
				}
				_CurrentCaption = value;
			}
		}
	}

	public HqObject2 FlingaObject
	{
		get
		{
			return placeHolderObject;
		}
	}

	public PathFinder Pathfinder
	{
		get
		{
			if (currentPathfinder == null)
			{
				currentPathfinder = CurrentRoom.FindClosestPathFinder(base.gameObject.transform.position);
			}
			if (currentPathfinder == null)
			{
				CspUtils.DebugLog("Could not find any pathfinder for " + base.gameObject.name + " in " + CurrentRoom.Id);
			}
			return currentPathfinder;
		}
	}

	public string CharacterName
	{
		get
		{
			if (CharGlobals == null)
			{
				CspUtils.DebugLog("charGlobals is null! " + base.gameObject.name);
				return null;
			}
			if (CharGlobals.definitionData != null)
			{
				return CharGlobals.definitionData.CharacterName;
			}
			return null;
		}
	}

	public int Strength
	{
		get
		{
			return aiData.strength;
		}
	}

	public bool InJail
	{
		get
		{
			return inJail;
		}
		set
		{
			if (inJail != value)
			{
				inJail = value;
			}
		}
	}

	public bool IsFlingaObjectActive
	{
		get
		{
			return flingaObjectIsVisible;
		}
	}

	public AIControllerHQData AIData
	{
		get
		{
			return aiData;
		}
	}

	public bool Selected
	{
		get
		{
			return selected;
		}
		set
		{
			selected = value;
			SelectedObjectController component = Utils.GetComponent<SelectedObjectController>(Camera.main);
			if (component != null)
			{
				if (selected)
				{
					component.AddObject(base.gameObject);
				}
				else
				{
					component.DelObject(base.gameObject);
				}
			}
		}
	}

	public HqItem CurrentActivityItem
	{
		get
		{
			return currentActivityItem;
		}
		set
		{
			if (!IsAngry || CanConsume(value))
			{
				HqItem.Claim nextAvailableClaim = value.NextAvailableClaim;
				if (nextAvailableClaim == null)
				{
					value.CancelUsers();
				}
				SetActivityItem(value);
				ChooseActivity();
			}
		}
	}

	public Texture2D Icon
	{
		get
		{
			if (characterDefinition != null)
			{
				return GUIManager.Instance.LoadTexture("characters_bundle|token_" + characterDefinition.CharacterName);
			}
			return null;
		}
	}

	public Mood CurrentMood
	{
		get
		{
			foreach (int value in Enum.GetValues(typeof(Mood)))
			{
				if (CurrentHunger <= (float)value)
				{
					return (Mood)value;
				}
			}
			return Mood.Happy;
		}
	}

	public bool IsAngry
	{
		get
		{
			return CurrentMood == Mood.Disgruntled || CurrentMood == Mood.Enraged;
		}
	}

	public bool CanPickUp
	{
		get
		{
			if (IsFlingaObjectActive)
			{
				return false;
			}
			if (!base.enabled)
			{
				return false;
			}
			if (CurrentRoom != HqController2.Instance.ActiveRoom)
			{
				return false;
			}
			return true;
		}
	}

	public bool IsInActiveRoom
	{
		get
		{
			return CurrentRoom == HqController2.Instance.ActiveRoom;
		}
	}

	public HqItem.Claim CurrentClaim
	{
		get
		{
			return currentClaim;
		}
	}

	protected CharacterGlobals CharGlobals
	{
		get
		{
			if (charGlobals == null)
			{
				charGlobals = Utils.GetComponent<CharacterGlobals>(base.gameObject);
			}
			return charGlobals;
		}
		set
		{
			charGlobals = value;
		}
	}

	protected BehaviorManager BehaviorManager
	{
		get
		{
			if (behaviorManager == null && CharGlobals != null)
			{
				behaviorManager = CharGlobals.behaviorManager;
			}
			if (behaviorManager == null)
			{
				CspUtils.DebugLog("BehaviorManager is null for " + base.gameObject.name);
			}
			return behaviorManager;
		}
		set
		{
			behaviorManager = value;
		}
	}

	private Renderer[] Renderers
	{
		get
		{
			if (renderers == null)
			{
				renderers = Utils.GetComponents<Renderer>(base.gameObject, Utils.SearchChildren, true);
			}
			return renderers;
		}
	}

	private Texture2D MoodTexture
	{
		get
		{
			return GUIManager.Instance.LoadTexture(aiData.mood_icons[CurrentMood]);
		}
	}

	protected LodCharacter LodCharacter
	{
		get
		{
			if (lodCharacter == null)
			{
				lodCharacter = Utils.GetComponent<LodCharacter>(base.gameObject, Utils.SearchChildren);
			}
			return lodCharacter;
		}
		set
		{
			lodCharacter = value;
		}
	}

	protected virtual float CurrentHunger
	{
		get
		{
			return currentHunger;
		}
		set
		{
			currentHunger = value;
		}
	}

	public bool IsInDefaultBehavior
	{
		get
		{
			BehaviorBase behavior = BehaviorManager.getBehavior();
			Type type = behavior.GetType();
			return type == typeof(BehaviorWander);
		}
	}

	protected float CurrentRoomEndTime
	{
		get
		{
			return currentRoomEndTime;
		}
		set
		{
			currentRoomEndTime = value;
		}
	}

	protected bool IsMovingToGoal
	{
		get
		{
			BehaviorBase behavior = BehaviorManager.getBehavior();
			if (behavior != null && (behavior.GetType() == typeof(BehaviorChangeRooms) || behavior.GetType() == typeof(BehaviorPathTo)))
			{
				return true;
			}
			return false;
		}
	}

	protected bool CanInteract
	{
		get
		{
			if (!IsAngry && Time.time - lastInteractionTime > 10f)
			{
				BehaviorBase behavior = BehaviorManager.getBehavior();
				return behavior != null && currentActivityItem == null && behavior.allowInterrupt(typeof(BehaviorInteract));
			}
			return false;
		}
	}

	public void DeactivateFlinga()
	{
		if (flingaObjectIsVisible)
		{
			base.gameObject.transform.position = FlingaObject.gameObject.transform.position;
			if (IsInActiveRoom)
			{
				Activate();
			}
			Utils.ActivateTree(FlingaObject.gameObject, false);
			flingaObjectIsVisible = false;
			if (LodCharacter != null)
			{
				LodCharacter.enabled = true;
			}
		}
	}

	public void Activate()
	{
		currentMode = AiMode.Active;
		StartRendering();
	}

	public void StartRendering()
	{
		if (attachedObject != null)
		{
			Utils.ActivateTree(attachedObject, true);
		}
		Renderer[] array = Renderers;
		foreach (Renderer renderer in array)
		{
			if (renderer != null)
			{
				renderer.enabled = true;
			}
		}
		EffectSequence[] components = Utils.GetComponents<EffectSequence>(base.gameObject, Utils.SearchChildren, true);
		EffectSequence[] array2 = components;
		foreach (EffectSequence effectSequence in array2)
		{
			if (effectSequence != null)
			{
				effectSequence.gameObject.active = true;
				effectSequence.enabled = true;
			}
		}
		base.enabled = true;
		if (LodCharacter != null)
		{
			LodCharacter.enabled = true;
		}
		Transform transform = Utils.FindNodeInChildren(base.gameObject.transform, "ClickBox");
		if (transform != null)
		{
			Utils.ActivateTree(transform.gameObject, true);
		}
		if (happinessEffect != null)
		{
			happinessEffect.gameObject.active = true;
		}
		BehaviorManager.setMotionEnabled(true);
		HqController2.Instance.SetCollisionOnAIController(this);
	}

	public bool Initialize(bool paused, HqRoom2 initialRoom)
	{
		if (initialRoom == null)
		{
			CspUtils.DebugLog("initialRoom is null!");
			return false;
		}
		if (aiData == null)
		{
			CspUtils.DebugLog(CharacterName + " has no hq ai data.");
			aiData = new AIControllerHQData();
			aiData.InitializeDefaults();
		}
		InitLocalBounds();
		if (placeHolderObject == null)
		{
			CreateDefaultFlingaObject();
		}
		flingaObjectIsVisible = false;
		ISHSCounterType counter = AppShell.Instance.CounterManager.GetCounter("HeroHungerCounter");
		if (counter != null)
		{
			CurrentHunger = counter.GetCurrentValue(CharacterName);
		}
		AssetBundle assetBundle = HqController2.Instance.GetAssetBundle("HQ/hq_shared");
		if (assetBundle != null)
		{
			GameObject gameObject = assetBundle.Load("HappinessEffect_prefab") as GameObject;
			if (gameObject != null)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject) as GameObject;
				if (gameObject2 != null)
				{
					Utils.SetLayerTree(gameObject2, 2);
					happinessEffect = Utils.GetComponent<HqHappinessEffect>(gameObject2);
					if (happinessEffect != null)
					{
						happinessEffect.AiController = this;
					}
				}
			}
		}
		CurrentRoom = initialRoom;
		currentActivityTime = 0f;
		CurrentRoomEndTime = 3f;
		int value = 1;
		foreach (KeyValuePair<string, HqRoom2> room in HqController2.Instance.Rooms)
		{
			if (!aiData.room_affinities.ContainsKey(room.Key))
			{
				aiData.room_affinities.Add(room.Key, 3);
			}
		}
		if (aiData.room_affinities.TryGetValue(currentRoom.Id, out value))
		{
			CurrentRoomEndTime = value * 3;
		}
		BehaviorManager.ChangeDefaultBehavior("BehaviorWander");
		if (paused)
		{
			ChangeBehavior<BehaviorPaused>(true);
		}
		Paused = paused;
		ChooseActivity();
		if (IsInActiveRoom)
		{
			PlaceAt(base.gameObject.transform.position);
		}
		else
		{
			lastKnownPosition = Vector3.zero;
		}
		AppShell.Instance.EventMgr.AddListener<HqAILogMessage>(OnHqAILogMessage);
		AppShell.Instance.EventMgr.AddListener<HqAISetHungerMessage>(OnHqAISetHungerMessage);
		return true;
	}

	private void CreateDefaultFlingaObject()
	{
		GameObject gameObject = new GameObject();
		gameObject.name = base.gameObject.name + "_flinga_obj";
		Animation animation = Utils.AddComponent<Animation>(gameObject);
		animation.playAutomatically = true;
		if (base.gameObject.animation != null && base.gameObject.animation.GetClip("movement_idle") != null)
		{
			animation.AddClip(base.gameObject.animation.GetClip("movement_idle"), "movement_idle");
			animation.clip = animation.GetClip("movement_idle");
		}
		BoxCollider boxCollider = Utils.AddComponent<BoxCollider>(gameObject);
		boxCollider.size = new Vector3(1f, 2f, 1f);
		boxCollider.center = new Vector3(0f, 1f, 0f);
		boxCollider.isTrigger = true;
		Rigidbody rigidbody = Utils.AddComponent<Rigidbody>(gameObject);
		rigidbody.mass = 3f;
		Utils.AddComponent<PhysicsInit>(gameObject);
		Utils.AddComponent<PlayAnimation>(gameObject);
		placeHolderObject = Utils.AddComponent<HqObject2>(gameObject);
		placeHolderObject.AIController = this;
		for (int i = 0; i < base.gameObject.transform.childCount; i++)
		{
			Transform child = base.gameObject.transform.GetChild(i);
			if (Utils.GetComponent<Collider>(child) == null)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(child.gameObject) as GameObject;
				gameObject2.name = child.gameObject.name;
				if (gameObject2 != null)
				{
					gameObject2.transform.parent = gameObject.transform;
				}
			}
		}
		gameObject.transform.position = base.gameObject.transform.position;
		gameObject.transform.rotation = base.gameObject.transform.rotation;
		Utils.ActivateTree(gameObject, false);
		flingaObjectIsVisible = false;
	}

	public void Awaken()
	{
		if (lastKnownPosition == Vector3.zero)
		{
			Vector3 vector = currentRoom.RandomLocation;
			if (vector == Vector3.zero)
			{
				CspUtils.DebugLog(CharacterName + " could not find an open location to awaken at. Waking up at door");
				vector = currentRoom.RandomDoor;
			}
			CharGlobals.motionController.teleportTo(vector);
		}
		else
		{
			CharGlobals.motionController.teleportTo(lastKnownPosition);
		}
		currentPathfinder = currentRoom.FindClosestPathFinder(base.gameObject.transform.position);
		CurrentRoomEndTime = 3f;
		int value = 1;
		if (aiData.room_affinities.TryGetValue(currentRoom.Id, out value))
		{
			CurrentRoomEndTime = value * 3;
		}
		if (InJail)
		{
			BehaviorDetention behaviorDetention = ChangeBehavior<BehaviorDetention>(false);
			behaviorDetention.Initialize(OnResumeActivity, 10f);
			return;
		}
		BehaviorBase behavior = BehaviorManager.getBehavior();
		if (!paused && behavior.GetType() == typeof(BehaviorSimulate))
		{
			BehaviorSimulate behaviorSimulate = behavior as BehaviorSimulate;
			if (behaviorSimulate.currentClaim != null)
			{
				currentActivityItem = behaviorSimulate.currentClaim.item;
				currentClaim = behaviorSimulate.currentClaim;
				currentClaim.Claimer = this;
			}
			ChooseActivity();
		}
	}

	public void DeActivate()
	{
		currentMode = AiMode.Sim;
		lastKnownPosition = base.gameObject.transform.position;
		BehaviorManager.setMotionEnabled(false);
		StopRendering();
		BehaviorBase behavior = BehaviorManager.getBehavior();
		if (paused)
		{
			behavior.Paused = true;
		}
		else if (behavior.GetType() != typeof(BehaviorSimulate))
		{
			BehaviorSimulate behaviorSimulate = ChangeBehavior<BehaviorSimulate>(true);
			if (behaviorSimulate != null)
			{
				behaviorSimulate.Initialize(this, OnEnterActiveRoom);
			}
		}
		if (LodCharacter != null)
		{
			LodCharacter.enabled = false;
		}
	}

	public void StopRendering()
	{
		if (attachedObject != null)
		{
			Utils.ActivateTree(attachedObject, false);
		}
		EffectSequence[] components = Utils.GetComponents<EffectSequence>(base.gameObject, Utils.SearchChildren, true);
		EffectSequence[] array = components;
		foreach (EffectSequence effectSequence in array)
		{
			if (effectSequence != null)
			{
				effectSequence.gameObject.active = false;
				effectSequence.enabled = false;
			}
		}
		Renderer[] array2 = Renderers;
		foreach (Renderer renderer in array2)
		{
			if (renderer != null)
			{
				renderer.enabled = false;
			}
		}
		Transform transform = Utils.FindNodeInChildren(base.gameObject.transform, "ClickBox");
		if (transform != null)
		{
			Utils.ActivateTree(transform.gameObject, false);
		}
		base.enabled = false;
		if (FlingaObject != null && FlingaObject.gameObject != null)
		{
			Utils.ActivateTree(FlingaObject.gameObject, false);
			flingaObjectIsVisible = false;
		}
		if (LodCharacter != null)
		{
			LodCharacter.enabled = false;
		}
		if (happinessEffect != null)
		{
			happinessEffect.gameObject.active = false;
		}
		ReleaseAllItems();
	}

	public bool IsObstacleBlocking(GameObject obj)
	{
		HqItem component = Utils.GetComponent<HqItem>(obj, Utils.SearchParents);
		if (component == null)
		{
			return true;
		}
		if (component.IsDestroyed)
		{
			return false;
		}
		if (currentActivityItem != null && component == currentActivityItem)
		{
			return true;
		}
		if (lastUsedActivityItem != null && component == lastUsedActivityItem)
		{
			return true;
		}
		return true;
	}

	public void Sim()
	{
		if (!Paused)
		{
			UpdateHunger();
			if (!InJail)
			{
			}
		}
	}

	public void Despawn()
	{
		if (currentRoom != null)
		{
			currentRoom.RemoveAI(this);
		}
		if (HqController2.Instance != null)
		{
			HqController2.Instance.DespawnAI(this);
		}
		CharGlobals.spawnData.Despawn(EntityDespawnMessage.despawnType.defeated);
		if (placeHolderObject != null)
		{
			UnityEngine.Object.Destroy(placeHolderObject.gameObject);
			placeHolderObject = null;
		}
		foreach (HqItem item in itemsInControl)
		{
			item.ReleaseControl(this);
		}
		itemsInControl.Clear();
		SaveHungerValue();
		AppShell.Instance.EventMgr.RemoveListener<HqAILogMessage>(OnHqAILogMessage);
		AppShell.Instance.EventMgr.RemoveListener<HqAISetHungerMessage>(OnHqAISetHungerMessage);
		AppShell.Instance.EventMgr.Fire(this, new HQHeroPlacedMessage(CharacterName, false));
	}

	public virtual void SaveHungerValue()
	{
		ISHSCounterType counter = AppShell.Instance.CounterManager.GetCounter("HeroHungerCounter");
		if (counter != null)
		{
			counter.SetCounter(CharacterName, Convert.ToInt32(Mathf.Floor(CurrentHunger)), SHSCounterType.ReportingMethodEnum.WebService);
		}
	}

	public virtual void InitializeFromData(DataWarehouse data)
	{
		DataWarehouse data2 = data.GetData("ai_hq_controller");
		if (data2 == null)
		{
			return;
		}
		aiData = Utils.XmlDeserialize<AIControllerHQData>(data2);
		if (aiData != null)
		{
			if (aiData.flinga_placeholder != null)
			{
				string[] array = aiData.flinga_placeholder.Split('|');
				AppShell.Instance.BundleLoader.FetchAssetBundle(array[0], OnFlingaObjectLoaded, array[1]);
			}
			aiData.InitializeDefaults();
		}
		AppShell.Instance.DataManager.LoadGameData("Characters/" + aiData.name, OnCharacterDataLoaded);
	}

	public void Interrupt()
	{
		ResetActivity();
		ChangeBehavior<BehaviorPaused>(true);
	}

	public void PickUpAI()
	{
		ResetActivity();
		ChangeBehavior<BehaviorPaused>(true);
		if (FlingaObject != null)
		{
			FlingaObject.gameObject.transform.position = base.gameObject.transform.position;
			FlingaObject.gameObject.transform.rotation = base.gameObject.transform.rotation;
			Paused = true;
			StopRendering();
			needNewActivity = true;
			Utils.ActivateTree(FlingaObject.gameObject, true);
			flingaObjectIsVisible = true;
			ReleaseAllItems();
			if (LodCharacter != null)
			{
				LodCharacter.enabled = false;
			}
		}
		if (base.gameObject.collider != null)
		{
			base.gameObject.collider.isTrigger = true;
		}
	}

	public void DropAI()
	{
		currentPathfinder = CurrentRoom.FindClosestPathFinder(FlingaObject.gameObject.transform.position);
		if (base.gameObject.collider != null)
		{
			base.gameObject.collider.isTrigger = false;
		}
		if (HqController2.Instance.State == typeof(HqController2.HqControllerFlinga))
		{
			DeactivateFlinga();
			Paused = false;
		}
		else
		{
			Paused = true;
		}
	}

	public void DoGrandEntrance()
	{
		BehaviorIntroHero behaviorIntroHero = ChangeBehavior<BehaviorIntroHero>(true);
		if (behaviorIntroHero != null)
		{
			behaviorIntroHero.Initialize(OnWanderEnd, Pathfinder, IsObstacleBlocking);
		}
	}

	public void PlaceAt(Vector3 target)
	{
		Vector3 min = localBounds.min;
		Vector3 max = localBounds.max;
		Vector3 center = localBounds.center;
		Vector3[] array = new Vector3[9];
		array[0] = new Vector3(min.x, min.y, min.z);
		array[1] = new Vector3(min.x, min.y, max.z);
		array[2] = new Vector3(max.x, min.y, min.z);
		array[3] = new Vector3(max.x, min.y, max.z);
		array[4] = new Vector3(center.x, min.y, center.z);
		array[5] = (array[0] + array[1]) * 0.5f;
		array[5].y = min.y;
		array[6] = (array[1] + array[3]) * 0.5f;
		array[6].y = min.y;
		array[7] = (array[3] + array[2]) * 0.5f;
		array[7].y = min.y;
		array[8] = (array[2] + array[0]) * 0.5f;
		array[8].y = min.y;
		float num = float.MinValue;
		Vector3[] array2 = array;
		foreach (Vector3 b in array2)
		{
			Vector3 origin = target + b;
			origin.y = CurrentRoom.ceilingHeight;
			Ray ray = new Ray(origin, Vector3.down);
			int layerMask = 1155584;
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo, CurrentRoom.ceilingHeight * 2f, layerMask))
			{
				Vector3 point = hitInfo.point;
				if (point.y > num)
				{
					Vector3 point2 = hitInfo.point;
					num = point2.y;
				}
			}
		}
		base.gameObject.transform.position = new Vector3(target.x, num, target.z);
		if (FlingaObject != null)
		{
			FlingaObject.gameObject.transform.position = base.gameObject.transform.position;
		}
	}

	public void DoEmote(sbyte emoteId)
	{
		BehaviorEmote behaviorEmote = ChangeBehavior<BehaviorEmote>(false);
		if (behaviorEmote != null && !behaviorEmote.Initialize(emoteId))
		{
			ChooseActivity();
		}
	}

	public void StopEmote()
	{
		ChooseActivity();
	}

	public void DoIdleEmote()
	{
		BehaviorEmote behaviorEmote = ChangeBehavior<BehaviorEmote>(false);
		if (behaviorEmote != null && aiData.mood_emotes.ContainsKey(CurrentMood))
		{
			List<AIControllerHQData.emote> list = aiData.mood_emotes[CurrentMood];
			int index = UnityEngine.Random.Range(0, list.Count);
			sbyte id = list[index].id;
			behaviorEmote.Initialize(id, OnIdleEmoteOver);
		}
		else
		{
			ChooseActivity();
		}
	}

	public void ReleaseControl(HqItem obj)
	{
		if (itemsInControl.Contains(obj))
		{
			itemsInControl.Remove(obj);
		}
		obj.ReleaseControl(this);
	}

	public void ReleaseCurrentItem()
	{
		if (currentActivityItem != null)
		{
			ReleaseControl(currentActivityItem);
		}
		currentActivityItem = null;
		if (currentClaim != null)
		{
			currentClaim.ReleaseClaim(this);
			currentClaim = null;
		}
	}

	public void OnUnload()
	{
		unloading = true;
	}

	public void OnDisable()
	{
		foreach (HqItem item in itemsInControl)
		{
			if (item != null && item.gameObject != null)
			{
				item.ReleaseControl(this);
				if (!unloading)
				{
					Utils.ActivateTree(item.gameObject, true);
					item.gameObject.transform.parent = CurrentRoom.gameObject.transform;
				}
			}
		}
		itemsInControl.Clear();
	}

	public void ReactToExplosion(Vector3 position, float force)
	{
		for (int num = itemsInControl.Count - 1; num >= 0; num--)
		{
			ReleaseControl(itemsInControl[num]);
		}
		BehaviorBase behavior = CharGlobals.behaviorManager.getBehavior();
		if (behavior.GetType() == typeof(BehaviorHitByObject))
		{
			OnResumeActivity(null);
			return;
		}
		BehaviorHitByObject behaviorHitByObject = ChangeBehavior<BehaviorHitByObject>(true);
		if (behaviorHitByObject != null)
		{
			if (!behaviorHitByObject.Initialize(position, force, base.transform.position - position, false, true, OnResumeActivity))
			{
				OnResumeActivity(null);
			}
			else if (interactingWith != null)
			{
				interactingWith.InteractionDone();
				interactingWith = null;
			}
		}
	}

	[AnimTag("projectilefire")]
	public void OnProjectileFireAnimTag(AnimationEvent evt)
	{
		IAnimTagListener animTagListener = BehaviorManager.getBehavior() as IAnimTagListener;
		if (animTagListener != null)
		{
			animTagListener.OnProjectileFireAnimTag(evt);
		}
	}

	public void GoToRoom(HqRoom2 room)
	{
		ReleaseAllItems();
		if (room == HqController2.Instance.ActiveRoom)
		{
			Vector3 vector = room.RandomLocation;
			if (vector == Vector3.zero)
			{
				Log("Could not find an open location to go to. Going to door instead", 16);
				vector = room.RandomDoor;
			}
			charGlobals.motionController.teleportTo(vector);
		}
		else
		{
			charGlobals.motionController.teleportTo(base.gameObject.transform.position + Vector3.up * 100f);
			FlingaObject.transform.position = base.gameObject.transform.position + Vector3.up * 100f;
		}
		CurrentRoom = room;
	}

	public T ChangeBehavior<T>(bool force) where T : BehaviorBase
	{
		T val = (T)null;
		val = ((!force) ? (BehaviorManager.requestChangeBehavior(typeof(T), false) as T) : (BehaviorManager.forceChangeBehavior(typeof(T)) as T));
		if (val != null)
		{
			val.Paused = Paused;
		}
		foreach (HqItem item in itemsInControl)
		{
			ReleaseControl(item);
		}
		if (val == null)
		{
			Log("Cannot change behavior to " + typeof(T).ToString() + "!", 1);
		}
		else
		{
			Log("Changed behavior to " + typeof(T).ToString(), 1);
		}
		return val;
	}

	public void TerminateObjectUse(GameObject obj)
	{
		Log("Canceling using " + obj.name, 1);
		ReleaseCurrentItem();
		BehaviorManager.cancelBehavior();
		ChooseActivity();
	}

	public HqItem FindItemToEat(HqRoom2 room)
	{
		if (room != CurrentRoom && room.RoomState == HqRoom2.AccessState.Locked)
		{
			return null;
		}
		HqItem hqItem = null;
		foreach (HqItem aIUsableItem in room.AIUsableItems)
		{
			if (!(aIUsableItem == null))
			{
				if (itemUseTimes.ContainsKey(aIUsableItem))
				{
					float num = Time.time - itemUseTimes[aIUsableItem];
					if (num < 5f)
					{
						continue;
					}
					itemUseTimes.Remove(aIUsableItem);
				}
				if (CanConsume(aIUsableItem) && aIUsableItem.NextAvailableClaim != null)
				{
					if (hqItem == null)
					{
						hqItem = aIUsableItem;
					}
					else if ((aIUsableItem.gameObject.transform.position - base.gameObject.transform.position).sqrMagnitude < (hqItem.gameObject.transform.position - base.gameObject.transform.position).sqrMagnitude)
					{
						hqItem = aIUsableItem;
					}
				}
			}
		}
		return hqItem;
	}

	public void AttachObject(GameObject obj, string attachNode)
	{
		if (attachedObject != null)
		{
			DetachObject(attachedObject);
		}
		if (attachNode != null)
		{
			Transform transform = Utils.FindNodeInChildren(base.gameObject.transform, attachNode);
			if (transform != null)
			{
				Utils.AttachGameObject(transform.gameObject, obj);
				attachedObject = obj;
			}
		}
		else
		{
			Utils.AttachGameObject(base.gameObject, obj);
			attachedObject = obj;
		}
	}

	public void DetachObject(GameObject obj)
	{
		if (obj == attachedObject && obj != null)
		{
			obj.transform.parent = CurrentRoom.gameObject.transform;
			attachedObject = null;
		}
	}

	public bool IsUsingItem(GameObject obj)
	{
		if (currentActivityItem != null && obj == currentActivityItem.gameObject && behaviorManager.getBehavior() is BehaviorUseItem)
		{
			return true;
		}
		return false;
	}

	public void ForceRoomChange(HqRoom2 room)
	{
		if (!(room != currentRoom))
		{
			return;
		}
		ResetActivity();
		BehaviorChangeRooms behaviorChangeRooms = ChangeBehavior<BehaviorChangeRooms>(false);
		if (behaviorChangeRooms != null)
		{
			behaviorChangeRooms.Initialize(currentRoom, room, Pathfinder, IsObstacleBlocking, OnChangedRoom, OnChangeRoomCanceled);
			if (behaviorChangeRooms.Status == BehaviorChangeRooms.PathStatus.PathNotFound)
			{
				Log("Could not path to exit destination of the room. Canceling room change.", 16);
				DoDefaultBehavior();
			}
		}
	}

	private void ReleaseAllItems()
	{
		foreach (HqItem item in itemsInControl)
		{
			ReleaseControl(item);
		}
		itemsInControl.Clear();
	}

	protected void TakeControl(HqItem obj)
	{
		Log("Taking control of " + obj.gameObject.name, 1);
		if (obj.TakeControl(this) && itemsInControl.Contains(obj))
		{
			itemsInControl.Add(obj);
		}
	}

	protected void Update()
	{
		BehaviorBase behavior = BehaviorManager.getBehavior();
		if (showBehaviors)
		{
			if (behavior != null)
			{
				Caption = behavior.GetType().ToString();
			}
		}
		else if (Caption != null)
		{
			Caption = null;
		}
		if (lastUsedActivityItem != null)
		{
			Bounds bounds = default(Bounds);
			Collider[] components = Utils.GetComponents<Collider>(lastUsedActivityItem, Utils.SearchChildren);
			Collider[] array = components;
			foreach (Collider collider in array)
			{
				bounds.Encapsulate(collider.bounds);
			}
			bounds.Encapsulate(base.gameObject.collider.bounds);
			float sqrMagnitude = (lastUsedActivityItem.transform.position - base.gameObject.transform.position).sqrMagnitude;
			if (sqrMagnitude > bounds.extents.sqrMagnitude)
			{
				Collider[] array2 = components;
				foreach (Collider collider2 in array2)
				{
					Physics.IgnoreCollision(collider2, base.gameObject.collider, false);
				}
				lastUsedActivityItem = null;
			}
		}
		if (FlingaObject != null && FlingaObject.State != typeof(HqObject2.HqObjectFlingaSelected) && flingaObjectIsVisible)
		{
			DropAI();
		}
		if (Paused)
		{
			return;
		}
		if (!HqController2.Instance.IsInPlayMode())
		{
			Paused = true;
			return;
		}
		currentRoomTime += Time.deltaTime;
		UpdateHunger();
		if (CanInteract)
		{
			AIControllerHQ aIToInteractWith = GetAIToInteractWith();
			if (aIToInteractWith != null)
			{
				InitiateInteraction(aIToInteractWith);
				interactingWith = aIToInteractWith;
			}
		}
		else if (currentActivityItem != null)
		{
			if (!IsCurrentActivityItemStillUseable())
			{
				AddToUsedItems(currentActivityItem);
				ReleaseCurrentItem();
				CharGlobals.behaviorManager.endBehavior();
				ChooseActivity();
			}
		}
		else
		{
			if (CurrentMood <= Mood.Content)
			{
				return;
			}
			Type type = CharGlobals.behaviorManager.getBehavior().GetType();
			if (type == typeof(BehaviorMope) || type == typeof(BehaviorWander))
			{
				HqItem hqItem = FindItemToEat(HqController2.Instance.ActiveRoom);
				if (hqItem != null)
				{
					SetActivityItem(hqItem);
					ChooseActivity();
				}
			}
		}
	}

	private bool IsCurrentActivityItemStillUseable()
	{
		if (currentActivityItem == null)
		{
			return false;
		}
		if (currentActivityItem.IsDestroyed)
		{
			return false;
		}
		if (currentActivityItem.IsMoving && behaviorManager.getBehavior().GetType() != typeof(BehaviorEatItem) && behaviorManager.getBehavior().GetType() != typeof(BehaviorThrowItem))
		{
			return false;
		}
		bool result = true;
		HqObject2 component = Utils.GetComponent<HqObject2>(currentActivityItem.gameObject);
		if (component != null)
		{
			if (component.State == typeof(HqObject2.HqObjectFlingaSelected) || !component.IsUsableByAI)
			{
				Log("Can no longer use " + currentActivityItem.gameObject.name + " because it is not usable by AI.", 4);
				result = false;
			}
		}
		else if (currentClaim != null && currentClaim.dockPoint != null)
		{
			if (currentActivityItem is PlacedItem)
			{
				if (!currentActivityItem.IsUpright(currentClaim.dockPoint))
				{
					Log("Can no longer use " + currentActivityItem.gameObject.name + " because item is not upright", 4);
					result = false;
				}
				if (!CheckItemLocation(currentClaim.dockPoint.transform.position, currentActivityItem))
				{
					Log("Can no longer use " + currentActivityItem.gameObject.name + " because item is not upright", 4);
					result = false;
				}
			}
			else if (!IsLocationClear(currentClaim.dockPoint.transform.position, currentActivityItem))
			{
				Log("Can no longer use " + currentActivityItem.gameObject.name + " because dockpoint location is blocked.", 4);
				result = false;
			}
		}
		return result;
	}

	private void UpdateHunger()
	{
		Mood currentMood = CurrentMood;
		if (CurrentHunger < aiData.hunger_max)
		{
			CurrentHunger += aiData.hunger_rate / 60f * Time.deltaTime;
		}
		if (CurrentHunger > aiData.hunger_max)
		{
			CurrentHunger = aiData.hunger_max;
		}
		if (CurrentMood != currentMood)
		{
			SaveHungerValue();
		}
	}

	protected void Start()
	{
		isReadyForSwap = true;
	}

	protected void InitLocalBounds()
	{
		bool flag = false;
		Quaternion rotation = Quaternion.identity;
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		if (!flag)
		{
			MeshFilter component = Utils.GetComponent<MeshFilter>(base.gameObject, Utils.SearchChildren);
			if (component != null)
			{
				flag = true;
				rotation = base.transform.rotation;
				vector = component.mesh.bounds.min;
				vector2 = component.mesh.bounds.max;
			}
		}
		if (!flag)
		{
			Collider component2 = Utils.GetComponent<Collider>(base.gameObject);
			if (component2 != null)
			{
				flag = true;
				vector = component2.bounds.min - base.transform.position;
				vector2 = component2.bounds.max - base.transform.position;
			}
		}
		if (!flag)
		{
			SkinnedMeshRenderer component3 = Utils.GetComponent<SkinnedMeshRenderer>(base.gameObject, Utils.SearchChildren);
			if (component3 != null)
			{
				flag = true;
				vector = component3.sharedMesh.bounds.min;
				vector2 = component3.sharedMesh.bounds.max;
			}
		}
		if (!flag)
		{
			CspUtils.DebugLog("Unable to initialize local bounds");
		}
		Vector3[] array = new Vector3[8]
		{
			rotation * new Vector3(vector.x, vector.y, vector.z),
			rotation * new Vector3(vector.x, vector.y, vector2.z),
			rotation * new Vector3(vector.x, vector2.y, vector.z),
			rotation * new Vector3(vector.x, vector2.y, vector2.z),
			rotation * new Vector3(vector2.x, vector.y, vector.z),
			rotation * new Vector3(vector2.x, vector.y, vector2.z),
			rotation * new Vector3(vector2.x, vector2.y, vector.z),
			rotation * new Vector3(vector2.x, vector2.y, vector2.z)
		};
		localBounds = new Bounds(array[0], Vector3.zero);
		for (int i = 1; i < array.Length; i++)
		{
			localBounds.Encapsulate(array[i]);
		}
	}

	protected bool IsItemUsable(HqItem placedItem)
	{
		if (placedItem.NextAvailableClaim == null)
		{
			return false;
		}
		if (placedItem.IsDestroyed)
		{
			return false;
		}
		if (placedItem.IsMoving)
		{
			return false;
		}
		if (itemUseTimes.ContainsKey(placedItem))
		{
			float num = Time.time - itemUseTimes[placedItem];
			if (num < 5f)
			{
				Log(placedItem.gameObject.name + " is not usable because it was used recently.", 4);
				return false;
			}
			itemUseTimes.Remove(placedItem);
		}
		if (placedItem.ItemDefinition != null && (CanConsume(placedItem) || CanUse(placedItem)))
		{
			HqItem.Claim nextAvailableClaim = placedItem.NextAvailableClaim;
			if (nextAvailableClaim == null)
			{
				Log("Cannot use " + placedItem.name + " DockPoint is null!", 4);
				return false;
			}
			if (placedItem.ItemDefinition != null && placedItem.ItemDefinition.UseInfo != null && nextAvailableClaim.dockPoint != null && placedItem.ItemDefinition.UseInfo.GetUseByDockPointName(nextAvailableClaim.dockPoint.name) == null)
			{
				return false;
			}
			return true;
		}
		return false;
	}

	protected void ResetActivity()
	{
		if (interactingWith != null)
		{
			interactingWith.InteractionDone();
			interactingWith = null;
		}
		ReleaseCurrentItem();
		ReleaseAllItems();
	}

	protected void ChooseActivityByDistance()
	{
		Log("Choosing activity by distance.", 1);
		if (currentActivityItem == null)
		{
			currentActivityItem = FindTheClosestUsableItem();
		}
		if (currentActivityItem != null)
		{
			if (currentClaim != null)
			{
				if (currentClaim.item != currentActivityItem)
				{
					currentClaim.ReleaseClaim(this);
					currentClaim = currentActivityItem.NextAvailableClaim;
				}
			}
			else
			{
				currentClaim = currentActivityItem.NextAvailableClaim;
			}
			if (currentClaim != null)
			{
				currentClaim.Claimer = this;
			}
			currentActivityTime = 10f;
			GoToPlacedItem(currentActivityItem);
		}
		else if (currentRoomTime >= CurrentRoomEndTime && CurrentRoom.RoomState == HqRoom2.AccessState.Unlocked)
		{
			List<HqRoom2> list = new List<HqRoom2>();
			foreach (KeyValuePair<string, HqRoom2> room in HqController2.Instance.Rooms)
			{
				if (room.Value != currentRoom && room.Value.RoomState == HqRoom2.AccessState.Unlocked)
				{
					list.Add(room.Value);
				}
			}
			currentActivityItem = FindItemToUse(list);
			if (currentActivityItem != null)
			{
				currentClaim = currentActivityItem.GetClosestAvailableClaim(base.gameObject.transform.position);
				if (currentClaim != null)
				{
					currentClaim.Claimer = this;
				}
				if (aiData.room_affinities.ContainsKey(currentActivityItem.Room.Id))
				{
					CurrentRoomEndTime = aiData.room_affinities[currentActivityItem.Room.Id] * 3;
					Log("Current room time " + CurrentRoomEndTime, 8);
				}
				else
				{
					CurrentRoomEndTime = 3f;
				}
				GoToPlacedItem(currentActivityItem);
			}
			else
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (HqRoom2 item in list)
				{
					int value = 3;
					aiData.room_affinities.TryGetValue(item.Id, out value);
					dictionary[item.Id] = value;
				}
				FindSomethingToDo(dictionary);
			}
		}
		else
		{
			DoDefaultBehavior();
		}
	}

	private HqItem FindTheClosestUsableItem()
	{
		HqItem hqItem = null;
		float num = float.MaxValue;
		bool flag = false;
		foreach (HqItem aIUsableItem in CurrentRoom.AIUsableItems)
		{
			if (!(aIUsableItem == null))
			{
				Log("Checking item " + aIUsableItem.gameObject.name, 2);
				bool flag2 = CanConsume(aIUsableItem);
				if (IsItemUsable(aIUsableItem))
				{
					Log(aIUsableItem.gameObject.name + " is useable.", 2);
					float sqrMagnitude = (aIUsableItem.gameObject.transform.position - base.gameObject.transform.position).sqrMagnitude;
					bool flag3 = false;
					if (hqItem == null)
					{
						flag3 = true;
						flag = flag2;
					}
					else if (!flag && flag2)
					{
						flag3 = true;
					}
					else if (flag && flag2 && sqrMagnitude < num)
					{
						flag3 = true;
					}
					else if (sqrMagnitude < num)
					{
						flag3 = true;
					}
					if (flag3)
					{
						Log("Setting closest item to " + aIUsableItem.gameObject.name + " distance is " + sqrMagnitude, 2);
						hqItem = aIUsableItem;
						num = sqrMagnitude;
						flag = flag2;
					}
				}
			}
		}
		return hqItem;
	}

	protected virtual void ChooseActivity()
	{
		if (Paused)
		{
			Log(" needs new activity when unpaused.", 1);
			needNewActivity = true;
		}
		else
		{
			if (currentRoom != HqController2.Instance.ActiveRoom && InJail)
			{
				return;
			}
			if (IsAngry && (currentActivityItem == null || currentActivityItem.HungerValue <= 0f))
			{
				HqItem hqItem = FindItemToEat(HqController2.Instance.ActiveRoom);
				if (hqItem != null)
				{
					Log("Found item to eat: " + hqItem.gameObject.name, 1);
					SetActivityItem(hqItem);
				}
				else
				{
					HqItem hqItem2 = FindItemToThrowOrDestroy(HqController2.Instance.ActiveRoom);
					if (hqItem2 != null)
					{
						Log("Found item to throw or destroy: " + hqItem2.gameObject.name, 1);
						TakeControl(hqItem2);
						if (currentClaim != null)
						{
							currentClaim.ReleaseClaim(this);
							currentClaim = null;
						}
						currentActivityItem = hqItem2;
						if (PathToItem(hqItem2, OnApproachItemDestroyArrived, OnApproachItemDestroyCanceled))
						{
							return;
						}
						AddToUsedItems(currentActivityItem);
						ReleaseCurrentItem();
					}
				}
				if (currentActivityItem == null)
				{
					DoDefaultBehavior();
					return;
				}
			}
			ChooseActivityByDistance();
		}
	}

	internal void AddToUsedItems(HqItem item)
	{
		itemUseTimes[item] = Time.time;
	}

	internal HqItem FindItemToUse(List<HqRoom2> roomList)
	{
		HqItem hqItem = null;
		foreach (HqRoom2 room in roomList)
		{
			foreach (HqItem aIUsableItem in room.AIUsableItems)
			{
				if (!(aIUsableItem == null) && IsItemUsable(aIUsableItem) && aIUsableItem.ItemDefinition != null)
				{
					if (CanConsume(aIUsableItem))
					{
						if (hqItem == null || aIUsableItem.HungerValue > hqItem.HungerValue)
						{
							hqItem = aIUsableItem;
						}
					}
					else if (hqItem == null || aIUsableItem.Room == HqController2.Instance.ActiveRoom)
					{
						hqItem = aIUsableItem;
					}
				}
			}
		}
		return hqItem;
	}

	internal PotentialActivity ChooseActivityBasedOnAffinity(List<HqRoom2> roomList)
	{
		Log("Choosing activity based on affinity.", 1);
		int num = 0;
		List<PotentialActivity> list = new List<PotentialActivity>();
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		List<PotentialActivity> list2 = new List<PotentialActivity>();
		foreach (HqRoom2 room in roomList)
		{
			int num2 = 0;
			int value = 3;
			aiData.room_affinities.TryGetValue(room.Id, out value);
			num2 = value;
			if (room == HqController2.Instance.ActiveRoom)
			{
				num *= 2;
			}
			Log("Considering items in room " + room.Id + " - room affinity = " + value, 8);
			foreach (HqItem aIUsableItem in room.AIUsableItems)
			{
				if (!(aIUsableItem == null))
				{
					Log("Considering " + aIUsableItem.name, 8);
					if (IsItemUsable(aIUsableItem) && aIUsableItem.ItemDefinition != null)
					{
						int num3 = 0;
						int num4 = 0;
						int num5 = 0;
						if (aIUsableItem.ItemDefinition.ItemKeywords != null)
						{
							foreach (KeyValuePair<int, ItemKeyword> itemKeyword in aIUsableItem.ItemDefinition.ItemKeywords)
							{
								int value2 = 0;
								aiData.item_keyword_affinities.TryGetValue(itemKeyword.Key, out value2);
								num3 += itemKeyword.Value.Strength;
								num4 += value2 * itemKeyword.Value.Strength;
								if (CanConsume(aIUsableItem) && num4 == 0)
								{
									num4 = 10;
								}
							}
						}
						if (aIUsableItem.HungerValue > 0f)
						{
							Log("Found a consumable " + aIUsableItem.gameObject.name, 8);
							num4 += (int)aIUsableItem.HungerValue;
						}
						num5 = ((num3 <= 0) ? 3 : (num4 / num3));
						num5 += value;
						num += num5;
						Log("Found potential activity " + aIUsableItem.ItemDefinition.Name + " = " + num5 + " ( " + num + " )", 8);
						PotentialActivity potentialActivity = new PotentialActivity();
						potentialActivity.calculatedAffinity = num5;
						potentialActivity.cumulativeAffinity = num;
						potentialActivity.item = aIUsableItem;
						list.Add(potentialActivity);
						if (CanConsume(aIUsableItem))
						{
							list2.Add(potentialActivity);
						}
						num2 += num5;
					}
				}
			}
			dictionary.Add(room.Id, num2);
		}
		if (list.Count == 0)
		{
			Log("Could not find any activities", 1);
			return null;
		}
		PotentialActivity potentialActivity2 = null;
		if (list2.Count > 0)
		{
			foreach (PotentialActivity item in list)
			{
				if (item.item != null && item.item.HungerValue > 0f && (potentialActivity2 == null || item.item.HungerValue > potentialActivity2.item.HungerValue))
				{
					potentialActivity2 = item;
				}
				else if (potentialActivity2 == null || item.cumulativeAffinity > potentialActivity2.cumulativeAffinity)
				{
					Log("Found a consumable item: " + item.item.gameObject.name, 8);
					potentialActivity2 = item;
				}
			}
		}
		else
		{
			int num6 = UnityEngine.Random.Range(1, num);
			foreach (PotentialActivity item2 in list)
			{
				if (item2.cumulativeAffinity >= num6)
				{
					Log("Found an activity " + item2.item.gameObject.name, 8);
					potentialActivity2 = item2;
					break;
				}
			}
		}
		if (potentialActivity2 != null)
		{
			Log("Chosen activity is " + potentialActivity2.item.ItemDefinition.Name + " for a duration of " + potentialActivity2.calculatedAffinity + " seconds, in room " + potentialActivity2.item.Room.Id, 1);
			return potentialActivity2;
		}
		return null;
	}

	protected bool PathToItem(HqItem item, BehaviorApproach.OnArrive onArriveCallback, BehaviorApproach.OnCanceled onCancelCallback)
	{
		Vector3 vector = item.gameObject.transform.position;
		Quaternion rotation = Quaternion.LookRotation(base.gameObject.transform.forward);
		bool adjustDestination = true;
		if (item.EntryPoints != null && item.EntryPoints.Length > 0)
		{
			bool flag = false;
			Component[] entryPoints = item.EntryPoints;
			for (int i = 0; i < entryPoints.Length; i++)
			{
				EntryPoint entryPoint = (EntryPoint)entryPoints[i];
				if (!entryPoint.IsBlocked && CheckItemLocation(entryPoint.transform.position, item))
				{
					flag = true;
					vector = entryPoint.transform.position;
					rotation = Quaternion.LookRotation(entryPoint.FacingDirection);
					adjustDestination = false;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		else if (currentClaim != null)
		{
			vector = currentClaim.Position;
			rotation = ((!(currentClaim.dockPoint != null)) ? Quaternion.LookRotation(item.transform.position - base.gameObject.transform.position) : Quaternion.LookRotation(currentClaim.dockPoint.FacingDirection));
			adjustDestination = false;
		}
		Log("Pathing to object " + item.gameObject.name + " Pos x:" + vector.x + " y:" + vector.y + " z:" + vector.z, 8);
		if (Pathfinder != null)
		{
			BehaviorPathTo behaviorPathTo = ChangeBehavior<BehaviorPathTo>(true);
			if (behaviorPathTo != null)
			{
				HqObject2 component = Utils.GetComponent<HqObject2>(item.gameObject);
				Bounds bounds = default(Bounds);
				if (item.gameObject.collider != null)
				{
					bounds = item.gameObject.collider.bounds;
				}
				else if (component != null)
				{
					bounds = component.Bounds;
				}
				if (bounds.size.sqrMagnitude > 0f && bounds.Contains(vector))
				{
					Log("Trying to adjust destination since it is inside object.", 8);
					Ray ray = new Ray(base.gameObject.transform.position, vector - base.gameObject.transform.position);
					float distance;
					if (bounds.IntersectRay(ray, out distance) && distance > 0f)
					{
						Vector3 normalized = ray.direction.normalized;
						normalized *= distance;
						vector = base.gameObject.transform.position + normalized;
						adjustDestination = true;
					}
				}
				Vector3 start = vector + new Vector3(0f, CharGlobals.characterController.height, 0f);
				Vector3 vector2 = vector - new Vector3(0f, CharGlobals.characterController.height, 0f);
				bool flag2 = false;
				RaycastHit hitInfo;
				if (Physics.Linecast(start, vector2, out hitInfo, 100608) && (hitInfo.point - vector).magnitude <= CharGlobals.characterController.height)
				{
					vector = hitInfo.point;
					flag2 = true;
				}
				if (!flag2)
				{
					Log("Could not find floor below item " + item.gameObject.name + " at " + vector + " down " + vector2 + ". Canceling pathing.", 8);
					return false;
				}
				behaviorPathTo.Initialize(vector, rotation, item, adjustDestination, onArriveCallback, onCancelCallback, 1f, 2f, true, false, Pathfinder, IsObstacleBlocking);
				if (behaviorPathTo.Status == BehaviorPathTo.PathStatus.PathNotFound || behaviorPathTo.Status == BehaviorPathTo.PathStatus.PathError)
				{
					PathNode pathNode = Pathfinder.ClosestPathNode(base.gameObject.transform.position);
					if (pathNode != null)
					{
						Log(base.gameObject.name + " could not find a path to item " + item.gameObject.name + " trying to teleport to closest path node", 1);
						CharGlobals.motionController.teleportTo(pathNode.transform.position);
						behaviorPathTo.Initialize(vector, rotation, item, adjustDestination, onArriveCallback, onCancelCallback, 1f, 2f, true, false, Pathfinder, IsObstacleBlocking);
					}
					if (behaviorPathTo.Status == BehaviorPathTo.PathStatus.PathNotFound || behaviorPathTo.Status == BehaviorPathTo.PathStatus.PathError)
					{
						Log(base.gameObject.name + " could not find a path to item " + item.gameObject.name + " Canceling moving to item.", 1);
						return false;
					}
				}
				return true;
			}
		}
		CspUtils.DebugLog(base.gameObject.name + " cannot path to " + item.ItemDefinition.Name + " because AI has no pathfinder.");
		return false;
	}

	protected void GoToPlacedItem(HqItem item)
	{
		bool flag = currentMode == AiMode.Active;
		bool flag2 = item.Room == HqController2.Instance.ActiveRoom;
		if (flag && flag2)
		{
			if (!PathToItem(item, OnApproachItemArrived, OnApproachItemCanceled))
			{
				AddToUsedItems(currentActivityItem);
				ReleaseCurrentItem();
				DoDefaultBehavior();
			}
			return;
		}
		if (flag)
		{
			if (Pathfinder != null && item.Room.RoomState == HqRoom2.AccessState.Unlocked)
			{
				BehaviorChangeRooms behaviorChangeRooms = ChangeBehavior<BehaviorChangeRooms>(false);
				if (behaviorChangeRooms != null)
				{
					behaviorChangeRooms.Initialize(currentRoom, item.Room, Pathfinder, IsObstacleBlocking, OnChangedRoom, OnChangeRoomCanceled);
					if (behaviorChangeRooms.Status == BehaviorChangeRooms.PathStatus.PathNotFound)
					{
						Log("Could not path to exit destination of the room. Canceling room change.", 16);
						DoDefaultBehavior();
					}
				}
			}
			else
			{
				CspUtils.DebugLog(base.gameObject.name + " is trying to go to a room it shouldn't: " + item.Room.name);
				DoDefaultBehavior();
			}
			return;
		}
		if (flag2)
		{
			Vector3 randomDoor = item.Room.RandomDoor;
			CharGlobals.motionController.teleportTo(randomDoor);
			CurrentRoom = item.Room;
			if (Time.time - CurrentRoomEndTime < currentActivityTime)
			{
				CurrentRoomEndTime = currentActivityTime;
			}
			if (!PathToItem(item, OnApproachItemArrived, OnApproachItemCanceled))
			{
				AddToUsedItems(currentActivityItem);
				ReleaseCurrentItem();
				DoDefaultBehavior();
			}
			return;
		}
		if (currentClaim != null)
		{
			CharGlobals.motionController.teleportTo(currentClaim.Position);
			CharGlobals.motionController.setDestination(currentClaim.Position);
			if (currentClaim.dockPoint != null)
			{
				base.gameObject.transform.rotation = Quaternion.LookRotation(currentClaim.dockPoint.FacingDirection);
			}
		}
		else
		{
			CharGlobals.motionController.teleportTo(item.gameObject.transform.position);
			CharGlobals.motionController.setDestination(item.gameObject.transform.position);
		}
		currentActivityStartTime = Time.time;
		if (Time.time - CurrentRoomEndTime < currentActivityTime)
		{
			CurrentRoomEndTime = currentActivityTime;
		}
	}

	protected void FindSomethingToDo(Dictionary<string, int> totalRoomAffinities)
	{
		if (currentRoom.RoomState == HqRoom2.AccessState.Unlocked)
		{
			string text = currentRoom.Id;
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			int num = 0;
			foreach (KeyValuePair<string, int> totalRoomAffinity in totalRoomAffinities)
			{
				if (totalRoomAffinity.Key != currentRoom.Id)
				{
					num += totalRoomAffinity.Value;
					dictionary.Add(totalRoomAffinity.Key, num);
				}
			}
			int num2 = UnityEngine.Random.Range(0, num);
			foreach (KeyValuePair<string, int> item in dictionary)
			{
				if (item.Value >= num2)
				{
					text = item.Key;
					break;
				}
			}
			CurrentRoomEndTime = 3 * aiData.room_affinities[text];
			if (text == currentRoom.Id)
			{
				DoDefaultBehavior();
				return;
			}
			CurrentRoomEndTime = aiData.room_affinities[text] * 3;
			Log("Decided to change rooms to " + text + " from " + currentRoom.Id + " for " + CurrentRoomEndTime + " seconds", 1);
			currentActivityTime = CurrentRoomEndTime;
			if (currentMode == AiMode.Active)
			{
				if (Pathfinder != null && Pathfinder.HasADoor && HqController2.Instance.Rooms[text].RoomState == HqRoom2.AccessState.Unlocked)
				{
					BehaviorChangeRooms behaviorChangeRooms = ChangeBehavior<BehaviorChangeRooms>(false);
					if (behaviorChangeRooms != null)
					{
						behaviorChangeRooms.Initialize(currentRoom, HqController2.Instance.Rooms[text], Pathfinder, IsObstacleBlocking, OnChangedRoom, OnChangeRoomCanceled);
						if (behaviorChangeRooms.Status == BehaviorChangeRooms.PathStatus.PathNotFound)
						{
							Log("Could not path to exit destination of the room. Canceling room change.", 16);
							DoDefaultBehavior();
						}
					}
				}
				else
				{
					DoDefaultBehavior();
				}
				return;
			}
			HqRoom2 hqRoom = HqController2.Instance.Rooms[text];
			Vector3 randomDoor = hqRoom.RandomDoor;
			CharGlobals.motionController.teleportTo(randomDoor);
			CurrentRoom = hqRoom;
		}
		DoDefaultBehavior();
	}

	protected AIControllerHQ GetAIToInteractWith()
	{
		if (aiData.relationship_emotes == null)
		{
			return null;
		}
		if (IsInActiveRoom)
		{
			foreach (AIControllerHQ item in currentRoom.AIInRoom)
			{
				if (item != this && item.CanInteract)
				{
					float sqrMagnitude = (item.gameObject.transform.position - base.gameObject.transform.position).sqrMagnitude;
					if (sqrMagnitude < 10f && sqrMagnitude > 4f)
					{
						Vector3 position = base.gameObject.transform.position;
						position.y = CharGlobals.characterController.height / 2f;
						Vector3 position2 = item.gameObject.transform.position;
						position2.y = item.CharGlobals.characterController.height / 2f;
						RaycastHit hitInfo;
						if (!Physics.Linecast(position, position2, out hitInfo, 4694016) || !(hitInfo.collider != null) || !(hitInfo.collider.gameObject != base.gameObject))
						{
							return item;
						}
					}
				}
			}
		}
		return null;
	}

	protected void InitiateInteraction(AIControllerHQ aiToInteractWith)
	{
		CharGlobals.motionController.setDestination(base.gameObject.transform.position);
		lastInteractionTime = Time.time;
		AIControllerHQData.RelationshipType relationshipType = GetRelationshipType(aiToInteractWith);
		sbyte emoteId = 0;
		if (aiData.relationship_emotes != null && aiData.relationship_emotes.ContainsKey(relationshipType))
		{
			List<AIControllerHQData.emote> list = aiData.relationship_emotes[relationshipType];
			int index = UnityEngine.Random.Range(0, list.Count);
			emoteId = list[index].id;
		}
		BehaviorInteract behaviorInteract = ChangeBehavior<BehaviorInteract>(false);
		behaviorInteract.Initialize(aiToInteractWith.gameObject, OnAIInteractionDone, OnAIInteractionEvent, emoteId);
		if (aiToInteractWith.CanInteract)
		{
			aiToInteractWith.InitiateInteraction(this);
		}
	}

	protected AIControllerHQData.RelationshipType GetRelationshipType(AIControllerHQ otherAI)
	{
		if (otherAI.aiData.affiliation == aiData.affiliation)
		{
			return AIControllerHQData.RelationshipType.like;
		}
		return AIControllerHQData.RelationshipType.dislike;
	}

	protected void DoDefaultBehavior()
	{
		Log("Will do DefaultBehavior", 1);
		ReleaseCurrentItem();
		currentActivityStartTime = Time.time;
		currentActivityTime = 9f;
		if (IsInActiveRoom)
		{
			if (InJail)
			{
				BehaviorDetention behaviorDetention = ChangeBehavior<BehaviorDetention>(false);
				behaviorDetention.Initialize(OnResumeActivity, 10f);
				return;
			}
			BehaviorWander behaviorWander = ChangeBehavior<BehaviorWander>(false);
			if (behaviorWander != null)
			{
				behaviorWander.Initialize(OnWanderEnd, Pathfinder, IsObstacleBlocking);
			}
		}
		else
		{
			BehaviorSimulate behaviorSimulate = ChangeBehavior<BehaviorSimulate>(true);
			if (behaviorSimulate != null)
			{
				behaviorSimulate.Initialize(this, OnEnterActiveRoom);
			}
		}
	}

	protected bool CanUse(HqItem pi)
	{
		Log("Checking to see if " + CharacterName + " can use " + pi.gameObject.name, 4);
		HqObject2 component = Utils.GetComponent<HqObject2>(pi.gameObject);
		if (component != null && !component.IsUsableByAI)
		{
			Log("Can't use " + pi.name + " because it is not useable by AI", 4);
			return false;
		}
		if (pi.Room != HqController2.Instance.ActiveRoom)
		{
			return true;
		}
		if (pi.ItemDefinition != null && pi.ItemDefinition.UseInfo != null)
		{
			if (pi.ItemDefinition.UseInfo.Uses == null || pi.ItemDefinition.UseInfo.Uses.Count == 0)
			{
				Log("Can't use " + pi.name + " because it has no useInfo or uses", 4);
				return false;
			}
			bool flag = false;
			if (pi.EntryPoints != null && pi.EntryPoints.Length > 0)
			{
				Log(pi.gameObject.name + " has " + pi.EntryPoints.Length + " entry points.", 4);
				for (int i = 0; i < pi.EntryPoints.Length; i++)
				{
					EntryPoint entryPoint = pi.EntryPoints[i] as EntryPoint;
					Log("Checking entry point " + entryPoint.gameObject.name, 4);
					bool flag2 = CheckItemLocation(entryPoint.transform.position, pi);
					Log(entryPoint.gameObject.name + " isValidLocation = " + flag2 + " IsBlocked = " + entryPoint.IsBlocked, 4);
					if (!entryPoint.IsBlocked && flag2)
					{
						Log("Found a valid entry point:" + entryPoint.gameObject.name + " on " + pi.name, 4);
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					Log("Cannot find a valid entry point on " + pi.name, 4);
					return false;
				}
			}
			foreach (Use value in pi.ItemDefinition.UseInfo.Uses.Values)
			{
				DockPoint dockPointByName = pi.GetDockPointByName(value.DockPointName);
				if (dockPointByName == null)
				{
					Log("Could not find dock point " + value.DockPointName + " on hqObject " + pi.name, 4);
				}
				else
				{
					if (pi is HqFixedItem)
					{
						return IsLocationClear(dockPointByName.transform.position, pi);
					}
					if (dockPointByName != null && pi.IsUpright(dockPointByName))
					{
						if (currentClaim != null && currentClaim.dockPoint != null && dockPointByName == currentClaim.dockPoint)
						{
							return true;
						}
						if (flag)
						{
							return true;
						}
						if (CheckItemLocation(dockPointByName.transform.position, pi))
						{
							return true;
						}
						Log("Found no ground under dock point on " + pi.ItemDefinition.Name + " dock point:" + value.DockPointName, 4);
					}
					if (!pi.IsUpright(dockPointByName))
					{
						Log("Cannot use " + value.DockPointName, 4);
					}
				}
			}
		}
		Log("PlacedItem is not usable by AI!", 4);
		return false;
	}

	protected bool CheckItemLocation(Vector3 location, HqItem item)
	{
		Log("Checking location for " + CharacterName + " on object " + item.gameObject.name, 4);
		Vector3 start = location + new Vector3(0f, CharGlobals.characterController.height, 0f);
		Vector3 end = location - new Vector3(0f, CharGlobals.characterController.height, 0f);
		float num = CharGlobals.characterController.height;
		if (item.ItemDefinition != null && item.ItemDefinition.UseInfo != null)
		{
			foreach (Use value in item.ItemDefinition.UseInfo.Uses.Values)
			{
				if (value.PostureType == Use.Posture.sit)
				{
					location = new Vector3(location.x, location.y + 0.5f, location.z);
					num = 0.5f;
					break;
				}
			}
		}
		RaycastHit hitInfo;
		if (Physics.Linecast(start, end, out hitInfo, 100608))
		{
			float magnitude = (hitInfo.point - location).magnitude;
			if (magnitude <= num)
			{
				return IsLocationClear(location, item);
			}
		}
		Log("There is no ground below location for " + CharacterName + " to stand on!", 4);
		return false;
	}

	private bool IsLocationClear(Vector3 location, HqItem item)
	{
		int layerMask = -2218245;
		Collider[] array = Physics.OverlapSphere(location, CharGlobals.characterController.radius, layerMask);
		if (array != null && array.Length > 0)
		{
			Collider[] array2 = array;
			foreach (Collider collider in array2)
			{
				if ((!(collider.gameObject != null) || !(Utils.GetComponent<HqKillZone>(collider.gameObject) != null)) && collider != item.gameObject.collider && collider.gameObject.transform.parent != item.gameObject.transform)
				{
					Log(CharacterName + " cannot use " + item.gameObject.name + " no room available to stand. Hit " + collider.gameObject.name, 4);
					return false;
				}
			}
		}
		return true;
	}

	protected bool CanConsume(HqItem pi)
	{
		HqObject2 component = Utils.GetComponent<HqObject2>(pi.gameObject);
		if (component != null && component.State == typeof(HqObject2.HqObjectFlingaSelected))
		{
			return false;
		}
		if (pi.IsInAIControl)
		{
			return false;
		}
		if (pi.IsMoving)
		{
			return false;
		}
		if (pi.Room != HqController2.Instance.ActiveRoom)
		{
			return false;
		}
		if (pi.HungerValue > 0f && CurrentHunger > 0f)
		{
			return true;
		}
		return false;
	}

	protected bool CanDestroy(GameObject obj)
	{
		if (IsAngry)
		{
			HqObject2 component = Utils.GetComponent<HqObject2>(obj);
			if (component != null)
			{
				HqItem item = CurrentRoom.GetItem(component.PlacedId);
				if (item != null)
				{
					return item.CanDestroy(this);
				}
			}
		}
		return false;
	}

	protected bool CanThrow(GameObject obj)
	{
		if (Time.time < nextThrowTime)
		{
			return false;
		}
		foreach (AIControllerHQ item2 in CurrentRoom.AIInRoom)
		{
			BehaviorThrowItem behaviorThrowItem = item2.behaviorManager.getBehavior() as BehaviorThrowItem;
			if (behaviorThrowItem != null)
			{
				return false;
			}
		}
		if (!InJail && IsAngry)
		{
			HqObject2 component = Utils.GetComponent<HqObject2>(obj);
			if (component != null)
			{
				PhysicsInit component2 = Utils.GetComponent<PhysicsInit>(component.gameObject);
				if (component2 != null && component2.IsImmobile)
				{
					return false;
				}
				HqItem item = CurrentRoom.GetItem(component.PlacedId);
				if (item != null)
				{
					return item.CanThrow(this);
				}
				CspUtils.DebugLog("Cannot throw item because it does not have a weight");
			}
		}
		return false;
	}

	protected void UseItem(HqItem item)
	{
		bool flag = true;
		if (item == null)
		{
			flag = false;
		}
		if (flag)
		{
			if (currentClaim != null)
			{
				CharGlobals.motionController.teleportTo(currentClaim.Position);
				CharGlobals.motionController.setDestination(currentClaim.Position);
				if (currentClaim.dockPoint != null)
				{
					base.gameObject.transform.rotation = Quaternion.LookRotation(currentClaim.dockPoint.FacingDirection);
				}
			}
			currentActivityStartTime = Time.time;
			if (Time.time - CurrentRoomEndTime < currentActivityTime)
			{
				CurrentRoomEndTime = currentActivityTime;
			}
			if (item.ItemDefinition == null)
			{
				return;
			}
			if (item.ItemDefinition.UseInfo != null && currentClaim != null && currentClaim.dockPoint != null)
			{
				Log("AI is going to use " + item.name, 1);
				if (IsAngry && item.HungerValue <= 0f)
				{
					CspUtils.DebugLog(CharacterName + " is trying to use an item when angry!");
					ReleaseCurrentItem();
					ChooseActivity();
				}
				else if (!item.Use(this, currentClaim.dockPoint, OnDoneUsingItem))
				{
					ChooseActivity();
				}
			}
			else
			{
				if (item.HungerValue == 0f)
				{
					return;
				}
				Log("AI is going to eat " + item.name, 1);
				if (!item.IsInAIControl || item.AIInControl == this)
				{
					BehaviorEatItem behaviorEatItem = ChangeBehavior<BehaviorEatItem>(false);
					if (behaviorEatItem != null)
					{
						behaviorEatItem.Initialize(item.gameObject, OnItemEaten, OnDoneEating);
						TakeControl(currentActivityItem);
					}
				}
				else
				{
					Log("AI cannot eat " + item.name + " because AI is in control!", 1);
				}
			}
		}
		else
		{
			ChooseActivity();
		}
	}

	protected void OnFlingaObjectLoaded(AssetBundleLoadResponse response, object extraData)
	{
		if (response.Error != null && response.Error != string.Empty)
		{
			CspUtils.DebugLog("The following error occurred while loading character assets for <" + response.Path + ">: " + response.Error);
			return;
		}
		if (response.Bundle == null)
		{
			CspUtils.DebugLog("Asset bundle is missing for <" + response.Path + ">: " + response.Error);
			return;
		}
		string text = extraData as string;
		if (text == null)
		{
			return;
		}
		GameObject gameObject = response.Bundle.Load(text) as GameObject;
		if (!(gameObject != null))
		{
			return;
		}
		GameObject g = UnityEngine.Object.Instantiate(gameObject) as GameObject;
		HqObject2 component = Utils.GetComponent<HqObject2>(g);
		if (component != null)
		{
			bool flag = false;
			if (placeHolderObject != null)
			{
				flag = placeHolderObject.gameObject.active;
				UnityEngine.Object.Destroy(placeHolderObject.gameObject);
			}
			placeHolderObject = component;
			placeHolderObject.AIController = this;
			placeHolderObject.transform.position = base.transform.position;
			if (!flag)
			{
				Utils.ActivateTree(placeHolderObject.gameObject, false);
				flingaObjectIsVisible = false;
			}
		}
	}

	protected void OnCharacterDataLoaded(GameDataLoadResponse response, object extraData)
	{
		if (response.Error != null && response.Error != string.Empty)
		{
			CspUtils.DebugLog("The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
			return;
		}
		DataWarehouse data = response.Data;
		characterDefinition = new CharacterDefinition();
		characterDefinition.InitializeFromData(data);
	}

	protected HqItem FindItemToThrowOrDestroy(HqRoom2 room)
	{
		HqItem hqItem = null;
		foreach (HqItem placedItem in room.PlacedItems)
		{
			if (!(placedItem == null) && !(placedItem.HungerValue > 0f) && !placedItem.IsMoving && (CanDestroy(placedItem.gameObject) || CanThrow(placedItem.gameObject)))
			{
				if (hqItem == null)
				{
					hqItem = placedItem;
				}
				else if ((placedItem.gameObject.transform.position - base.gameObject.transform.position).sqrMagnitude < (hqItem.gameObject.transform.position - base.gameObject.transform.position).sqrMagnitude)
				{
					hqItem = placedItem;
				}
			}
		}
		return hqItem;
	}

	private bool DestroyOrThrowObject(HqItem hqItem)
	{
		if (hqItem == null)
		{
			return false;
		}
		Log(CharacterName + " is going to destroy or throw " + hqItem.gameObject.name, 1);
		CharGlobals.motionController.stopGently();
		hqItem.CancelUsers();
		if (CanDestroy(hqItem.gameObject))
		{
			Log(CharacterName + " decided to destroy " + hqItem.gameObject.name, 1);
			BehaviorInteract behaviorInteract = ChangeBehavior<BehaviorInteract>(true);
			if (behaviorInteract != null)
			{
				if (hqItem != null)
				{
					TakeControl(hqItem);
				}
				HqObject2 component = Utils.GetComponent<HqObject2>(hqItem.gameObject);
				if (component != null)
				{
					HqController2.Instance.AIIsTakingOverItem(component);
					if (component.gameObject.rigidbody != null)
					{
						component.gameObject.rigidbody.isKinematic = true;
						component.gameObject.rigidbody.useGravity = false;
						component.gameObject.active = false;
						component.gameObject.active = true;
					}
				}
				behaviorInteract.Initialize(hqItem.gameObject, OnResumeActivity, OnDestroyEvent, aiData.destroy_sequence_id);
				return true;
			}
		}
		else if (CanThrow(hqItem.gameObject))
		{
			Log(CharacterName + " decided to throw " + hqItem.gameObject.name, 1);
			BehaviorThrowItem behaviorThrowItem = ChangeBehavior<BehaviorThrowItem>(true);
			if (behaviorThrowItem != null)
			{
				TakeControl(hqItem);
				HqObject2 component2 = Utils.GetComponent<HqObject2>(hqItem.gameObject);
				if (component2 != null)
				{
					HqController2.Instance.AIIsTakingOverItem(component2);
				}
				behaviorThrowItem.Initialize(hqItem.gameObject, OnDoneThrowing);
				return true;
			}
		}
		return false;
	}

	protected void SetActivityItem(HqItem item)
	{
		if (!(item != null) || !(currentActivityItem != item))
		{
			return;
		}
		HqItem hqItem = currentActivityItem;
		HqItem.Claim claim = currentClaim;
		currentClaim = null;
		if (!CanUse(item) && !CanConsume(item))
		{
			CspUtils.DebugLog("Could not force " + CharacterName + " to use " + item.gameObject.name);
			currentClaim = claim;
			return;
		}
		ReleaseCurrentItem();
		CspUtils.DebugLog("Setting " + CharacterName + " currentActivityItem to " + item.gameObject.name);
		currentActivityItem = item;
		currentClaim = currentActivityItem.NextAvailableClaim;
		if (currentClaim == null)
		{
			currentActivityItem = hqItem;
			currentClaim = claim;
			return;
		}
		if (claim != null)
		{
			claim.ReleaseClaim(this);
		}
		currentClaim.Claimer = this;
	}

	protected void FindHighestTransform(Transform parent, ref Transform currentHighest)
	{
		if (!(currentHighest == null))
		{
			Vector3 position = parent.position;
			float y = position.y;
			Vector3 position2 = currentHighest.position;
			if (!(y > position2.y))
			{
				goto IL_0032;
			}
		}
		currentHighest = parent;
		goto IL_0032;
		IL_0032:
		for (int i = 0; i < parent.childCount; i++)
		{
			Transform child = parent.GetChild(i);
			FindHighestTransform(child, ref currentHighest);
		}
	}

	protected void OnWanderEnd()
	{
		DoIdleEmote();
	}

	protected void OnEnterActiveRoom(GameObject obj)
	{
		if (obj != null)
		{
			HqItem component = Utils.GetComponent<HqItem>(obj, Utils.SearchChildren);
			if (component != null)
			{
				if (currentClaim != null)
				{
					currentClaim.ReleaseClaim(this);
					currentClaim = null;
				}
				currentClaim = component.NextAvailableClaim;
				if (currentClaim != null && (currentClaim.Claimer == null || currentClaim.Claimer == this))
				{
					currentClaim.Claimer = this;
					currentActivityItem = component;
					currentActivityTime = 10f;
					GoToPlacedItem(component);
					return;
				}
				ReleaseCurrentItem();
			}
		}
		DoDefaultBehavior();
	}

	protected void OnApproachItemArrived(GameObject obj)
	{
		if (!(currentActivityItem != null))
		{
			return;
		}
		HqObject2 component = Utils.GetComponent<HqObject2>(obj);
		if (component != null && component.State == typeof(HqObject2.HqObjectFlingaSelected))
		{
			ChooseActivity();
			return;
		}
		if (currentClaim != null)
		{
			Collider[] components = Utils.GetComponents<Collider>(currentActivityItem, Utils.SearchChildren);
			Collider[] array = components;
			foreach (Collider collider in array)
			{
				if (collider.bounds.Contains(currentClaim.Position))
				{
					Physics.IgnoreCollision(collider, base.gameObject.collider);
				}
			}
			float sqrMagnitude = (currentClaim.Position - base.gameObject.transform.position).sqrMagnitude;
			if (sqrMagnitude > 1f)
			{
				CharGlobals.motionController.teleportTo(currentClaim.Position);
			}
		}
		UseItem(currentActivityItem);
	}

	protected void OnApproachItemCanceled(GameObject obj)
	{
		BehaviorPathTo behaviorPathTo = CharGlobals.behaviorManager.getBehavior() as BehaviorPathTo;
		if (behaviorPathTo != null && behaviorPathTo.Status == BehaviorPathTo.PathStatus.ItemMoved && currentActivityItem != null && IsCurrentActivityItemStillUseable())
		{
			ChooseActivity();
			return;
		}
		if (currentActivityItem != null)
		{
			AddToUsedItems(currentActivityItem);
			Collider[] components = Utils.GetComponents<Collider>(currentActivityItem, Utils.SearchChildren);
			Collider[] array = components;
			foreach (Collider collider in array)
			{
				Physics.IgnoreCollision(collider, base.gameObject.collider, false);
			}
		}
		ReleaseCurrentItem();
		ChooseActivity();
	}

	protected void OnApproachItemDestroyArrived(GameObject obj)
	{
		if (!DestroyOrThrowObject(currentActivityItem))
		{
			if (currentActivityItem != null)
			{
				AddToUsedItems(currentActivityItem);
				ReleaseCurrentItem();
			}
			ChooseActivity();
		}
	}

	protected void OnApproachItemDestroyCanceled(GameObject obj)
	{
		BehaviorPathTo behaviorPathTo = CharGlobals.behaviorManager.getBehavior() as BehaviorPathTo;
		if (behaviorPathTo != null && behaviorPathTo.Status == BehaviorPathTo.PathStatus.ItemMoved && currentActivityItem != null && IsCurrentActivityItemStillUseable())
		{
			ChooseActivity();
			return;
		}
		if (currentActivityItem != null)
		{
			AddToUsedItems(currentActivityItem);
			Collider[] components = Utils.GetComponents<Collider>(currentActivityItem, Utils.SearchChildren);
			Collider[] array = components;
			foreach (Collider collider in array)
			{
				Physics.IgnoreCollision(collider, base.gameObject.collider, false);
			}
		}
		ReleaseCurrentItem();
		ChooseActivity();
	}

	protected void OnChangeRoomCanceled(GameObject obj)
	{
		Log("Canceled changing rooms.", 16);
		CurrentRoomEndTime = 3f;
		if (aiData.room_affinities.ContainsKey(CurrentRoom.Id))
		{
			CurrentRoomEndTime = aiData.room_affinities[CurrentRoom.Id] * 3;
		}
		ReleaseCurrentItem();
		ChooseActivity();
	}

	protected void OnChangedRoom(GameObject obj)
	{
		if (currentActivityItem != null)
		{
			GoToPlacedItem(currentActivityItem);
		}
		CurrentRoomEndTime = 3f;
		if (aiData.room_affinities.ContainsKey(CurrentRoom.Id))
		{
			CurrentRoomEndTime = aiData.room_affinities[CurrentRoom.Id] * 3;
		}
		currentPathfinder = null;
	}

	protected void OnDoneUsingItem(GameObject obj)
	{
		if (currentActivityItem != null)
		{
			HqExplosive component = Utils.GetComponent<HqExplosive>(currentActivityItem.gameObject);
			if (component != null)
			{
				component.Explode();
			}
			Log("Done using item " + currentActivityItem.gameObject.name, 1);
			if (currentActivityItem != null)
			{
				if (currentActivityItem.HungerValue > 0f)
				{
					CurrentHunger -= currentActivityItem.HungerValue;
					if (CurrentHunger < 0f)
					{
						CurrentHunger = 0f;
					}
					SaveHungerValue();
				}
				if (currentActivityItem.EntryPoints != null && currentActivityItem.EntryPoints.Length > 0)
				{
					bool flag = false;
					Component[] entryPoints = currentActivityItem.EntryPoints;
					for (int i = 0; i < entryPoints.Length; i++)
					{
						EntryPoint entryPoint = (EntryPoint)entryPoints[i];
						if (!entryPoint.IsBlocked && CheckItemLocation(entryPoint.transform.position, currentActivityItem))
						{
							flag = true;
							CharGlobals.motionController.teleportTo(entryPoint.transform.position);
							CharGlobals.motionController.rotateTowards(entryPoint.FacingDirection * -1f);
							break;
						}
					}
					if (!flag)
					{
						PathFinder pathFinder = CurrentRoom.FindClosestPathFinder(base.gameObject.transform.position);
						if (pathFinder != null)
						{
							PathNode pathNode = pathFinder.ClosestPathNode(base.gameObject.transform.position);
							if (pathNode == null)
							{
								pathNode = pathFinder.RandomPathNode();
							}
							CharGlobals.motionController.teleportTo(pathNode.transform.position);
						}
					}
				}
			}
			AddToUsedItems(currentActivityItem);
			lastUsedActivityItem = currentActivityItem;
		}
		ReleaseCurrentItem();
		ChooseActivity();
	}

	protected void OnItemEaten(GameObject obj)
	{
		if (obj != null)
		{
			HqItem component = Utils.GetComponent<HqItem>(obj, Utils.SearchChildren);
			if (component != null && component.HungerValue > 0f)
			{
				CurrentHunger -= component.HungerValue;
				if (CurrentHunger < 0f)
				{
					CurrentHunger = 0f;
				}
				SaveHungerValue();
			}
			HqExplosive component2 = Utils.GetComponent<HqExplosive>(obj);
			if (component2 != null)
			{
				component2.Explode();
			}
			else
			{
				CurrentRoom.ConsumeItem(obj);
			}
		}
		ReleaseCurrentItem();
	}

	protected void OnDoneEating(GameObject obj)
	{
		ReleaseAllItems();
		ChooseActivity();
	}

	protected void OnCollisionEnter(Collision collisionInfo)
	{
		if (collisionInfo.gameObject == null || (collisionInfo.gameObject.rigidbody != null && collisionInfo.gameObject.rigidbody.velocity.sqrMagnitude < 1f))
		{
			return;
		}
		HqObject2 component = Utils.GetComponent<HqObject2>(collisionInfo.gameObject, Utils.SearchChildren);
		if (!(component != null))
		{
			return;
		}
		float num = 0f;
		HqItem item = CurrentRoom.GetItem(component.PlacedId);
		if (item != null)
		{
			num = item.ItemDefinition.Weight;
		}
		float num2 = num * collisionInfo.relativeVelocity.magnitude / (float)aiData.strength;
		if (!(num2 > 2f))
		{
			return;
		}
		BehaviorHitByObject behaviorHitByObject = ChangeBehavior<BehaviorHitByObject>(false);
		if (behaviorHitByObject != null)
		{
			if (!behaviorHitByObject.Initialize(collisionInfo.gameObject.transform.position, num2, collisionInfo.relativeVelocity, true, false, OnResumeActivity))
			{
				OnResumeActivity(null);
			}
			else if (interactingWith != null)
			{
				interactingWith.InteractionDone();
				interactingWith = null;
			}
		}
	}

	protected void OnResumeActivity(GameObject obj)
	{
		if (obj != null)
		{
			HqItem component = Utils.GetComponent<HqItem>(obj);
			if (component != null)
			{
				ReleaseControl(component);
			}
		}
		if (currentActivityItem != null)
		{
			GoToPlacedItem(currentActivityItem);
		}
		else
		{
			ChooseActivity();
		}
	}

	protected void OnIdleEmoteOver(GameObject obj)
	{
		BehaviorMope behaviorMope = ChangeBehavior<BehaviorMope>(false);
		if (behaviorMope != null)
		{
			float max = 10f * ((float)CurrentMood / 100f);
			float min = 5f * ((float)CurrentMood / 100f);
			float idleTime = UnityEngine.Random.Range(min, max);
			behaviorMope.Initialize(OnResumeActivity, idleTime);
		}
	}

	protected void OnDestroyEvent(GameObject objectToDestroy, string eventName, float value)
	{
		if (eventName == "destroy" && objectToDestroy != null)
		{
			Log("Destroying " + objectToDestroy.name, 8);
			HqObject2 component = Utils.GetComponent<HqObject2>(objectToDestroy, Utils.SearchChildren);
			if (component != null)
			{
				HqExplosive component2 = Utils.GetComponent<HqExplosive>(component);
				if (component2 != null)
				{
					component2.Explode();
				}
				else if (component.State == typeof(HqObject2.HqObjectFlinga) || component.State == typeof(HqObject2.HqObjectFlingaSelected))
				{
					HqItem item = CurrentRoom.GetItem(component.PlacedId);
					if (item != null)
					{
						ReleaseControl(item);
						item.Destroy();
					}
				}
			}
		}
		ReleaseCurrentItem();
	}

	protected void OnDoneThrowing(GameObject objectThrown)
	{
		nextThrowTime = Time.time + 5f;
		ReleaseCurrentItem();
		if (objectThrown != null)
		{
			HqItem component = Utils.GetComponent<HqItem>(objectThrown, Utils.SearchChildren);
			if (component != null)
			{
				ReleaseControl(component);
			}
		}
		ChooseActivity();
	}

	protected void OnAIInteractionEvent(GameObject objectInteractedWith, string eventName, float value)
	{
	}

	protected void OnAIInteractionDone(GameObject objectInteractedWith)
	{
		if (interactingWith != null)
		{
			interactingWith.InteractionDone();
			interactingWith = null;
			InteractionDone();
		}
	}

	protected void InteractionDone()
	{
		if (currentActivityItem != null)
		{
			if (!PathToItem(currentActivityItem, OnApproachItemArrived, OnApproachItemCanceled))
			{
				AddToUsedItems(currentActivityItem);
				ReleaseCurrentItem();
				DoDefaultBehavior();
			}
		}
		else
		{
			ChooseActivity();
		}
	}

	public void Log(string logMessage, int Mask)
	{
		if ((Mask & CurrentLogMask) != Mask)
		{
		}
	}

	public void OnHqAILogMessage(HqAILogMessage message)
	{
		if (message.Character.ToLower().CompareTo(CharacterName) == 0 || message.Character.ToLower().CompareTo("all") == 0)
		{
			try
			{
				Type typeFromHandle = typeof(LogMask);
				FieldInfo[] fields = typeFromHandle.GetFields(BindingFlags.Static | BindingFlags.Public);
				FieldInfo[] array = fields;
				foreach (FieldInfo fieldInfo in array)
				{
					if (fieldInfo.Name.ToLower() == message.LogChannel.ToLower())
					{
						int num = (int)fieldInfo.GetValue(null);
						if ((CurrentLogMask & num) != num)
						{
							CurrentLogMask |= num;
						}
						else
						{
							CurrentLogMask &= ~num;
						}
					}
				}
			}
			catch
			{
				CspUtils.DebugLog("Could not find log message channel.");
			}
		}
	}

	public void OnHqAISetHungerMessage(HqAISetHungerMessage message)
	{
		if (message.Character.ToLower().CompareTo(CharacterName) == 0 || message.Character.ToLower().CompareTo("all") == 0)
		{
			CurrentHunger = message.HungerValue;
			if (CurrentHunger < 0f)
			{
				CurrentHunger = 0f;
			}
			SaveHungerValue();
		}
	}
}
