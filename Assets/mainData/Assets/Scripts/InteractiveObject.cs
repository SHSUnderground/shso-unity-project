using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Interactive Object/Interactive Object")]
public class InteractiveObject : MonoBehaviour, ICaptureHandler, IInputHandler
{
	public enum StateIdx
	{
		Model,
		Hidden,
		Disable,
		Enable,
		Highlight,
		NoOverlay,
		Proximity,
		Hover,
		Emote,
		PowerEmote,
		Click,
		CollisionProxy,
		Last
	}

	public class RootArray
	{
		protected GameObject[] roots;

		public GameObject this[StateIdx idx]
		{
			get
			{
				return roots[(int)idx];
			}
			set
			{
				roots[(int)idx] = value;
			}
		}

		public RootArray()
		{
			roots = new GameObject[12];
		}
	}

	internal class BaseState : IDisposable, IShsState
	{
		internal InteractiveObject owner;

		internal StateIdx rootIdx;

		public BaseState(InteractiveObject owner, StateIdx rootIdx)
		{
			this.owner = owner;
			this.rootIdx = rootIdx;
		}

		public virtual void Enter(Type previousState)
		{
			owner.ActiveStateBranch(rootIdx);
		}

		public virtual void Update()
		{
		}

		public virtual void Leave(Type nextState)
		{
		}

		public virtual void Dispose()
		{
			owner = null;
		}
	}

	internal class OverlayState : BaseState
	{
		public OverlayState(InteractiveObject owner, StateIdx rootIdx)
			: base(owner, rootIdx)
		{
		}

		public override void Enter(Type previousState)
		{
			owner.ActivateOverlayBranch(rootIdx);
		}
	}

	internal class HiddenState : BaseState
	{
		public HiddenState(InteractiveObject owner)
			: base(owner, StateIdx.Hidden)
		{
		}

		public override void Enter(Type previousState)
		{
			base.Enter(previousState);
			foreach (GameObject collider in owner.colliders)
			{
				collider.SetActiveRecursively(false);
			}
		}

		public override void Leave(Type nextState)
		{
			base.Leave(nextState);
			foreach (GameObject collider in owner.colliders)
			{
				collider.SetActiveRecursively(true);
			}
		}
	}

	internal class DisableState : BaseState
	{
		public DisableState(InteractiveObject owner)
			: base(owner, StateIdx.Disable)
		{
		}
	}

	internal class EnableState : BaseState
	{
		public EnableState(InteractiveObject owner)
			: base(owner, StateIdx.Enable)
		{
		}
	}

	internal class HighlightState : BaseState
	{
		public HighlightState(InteractiveObject owner)
			: base(owner, StateIdx.Highlight)
		{
		}
	}

	internal class AutoExitState : OverlayState
	{
		internal float countdown;

		public AutoExitState(InteractiveObject owner, StateIdx rootIdx)
			: base(owner, rootIdx)
		{
		}

		public override void Enter(Type previousState)
		{
			base.Enter(previousState);
			countdown = owner.GetBranchLength(owner.roots[rootIdx]);
		}

		public override void Update()
		{
			base.Update();
			countdown -= Time.deltaTime;
			if (countdown <= 0f)
			{
				owner.GotoBestState();
			}
		}
	}

	internal class NoOverlayState : OverlayState
	{
		public NoOverlayState(InteractiveObject owner)
			: base(owner, StateIdx.NoOverlay)
		{
		}
	}

	internal class ProximityState : OverlayState
	{
		public ProximityState(InteractiveObject owner)
			: base(owner, StateIdx.Proximity)
		{
		}

		public override void Enter(Type previousState)
		{
			if (owner.rolloverPlayer != null)
			{
				owner.fsmOverlay.GotoState<HoverState>();
				return;
			}
			base.Enter(previousState);
			if (owner.highlightOnProximity)
			{
				owner.GotoState(StateIdx.Highlight);
			}
		}

		public override void Update()
		{
			base.Update();
			if (owner.playerInProximity == null)
			{
				owner.GotoBestState();
			}
		}
	}

	internal class HoverState : OverlayState
	{
		public HoverState(InteractiveObject owner)
			: base(owner, StateIdx.Hover)
		{
		}

		public override void Enter(Type previousState)
		{
			base.Enter(previousState);
			if (owner.rolloverPlayer != null && owner.highlightOnHover && (owner.highlightOnHoverAtAnyDistance || owner.WillAcceptMouseClick(owner.rolloverPlayer)))
			{
				owner.GotoState(StateIdx.Highlight);
			}
		}

		public override void Leave(Type nextState)
		{
			base.Leave(nextState);
			owner.InternalSetCursor(GUICursorManager.CursorType.Normal);
		}

		public override void Update()
		{
			base.Update();
			if (owner.rolloverPlayer == null)
			{
				owner.GotoBestState();
				return;
			}
			if (owner.highlightOnHover)
			{
				BaseState baseState = (BaseState)owner.fsmBase.GetCurrentStateObject();
				if (baseState.rootIdx < StateIdx.Highlight && (owner.highlightOnHoverAtAnyDistance || owner.WillAcceptMouseClick(owner.rolloverPlayer)))
				{
					owner.GotoState(StateIdx.Highlight);
				}
			}
			BaseState baseState2 = owner.fsmBase.GetCurrentStateObject() as BaseState;
			if (baseState2.rootIdx > StateIdx.Disable)
			{
				owner.InternalSetCursor(GUICursorManager.CursorType.Interactable);
			}
		}
	}

	internal class EmoteState : AutoExitState
	{
		public EmoteState(InteractiveObject owner)
			: base(owner, StateIdx.Emote)
		{
		}
	}

	internal class PowerEmoteState : AutoExitState
	{
		public PowerEmoteState(InteractiveObject owner)
			: base(owner, StateIdx.PowerEmote)
		{
		}
	}

	internal class ClickState : AutoExitState
	{
		protected bool blockTillDone;

		protected bool done;

		public ClickState(InteractiveObject owner)
			: base(owner, StateIdx.Click)
		{
			blockTillDone = false;
			done = false;
		}

		public override void Enter(Type previousState)
		{
			base.Enter(previousState);
		}

		public override void Update()
		{
			if (blockTillDone)
			{
				if (done)
				{
					owner.GotoBestState();
				}
			}
			else
			{
				base.Update();
			}
		}

		public void DoClick(GameObject player)
		{
			blockTillDone = false;
			done = false;
			List<InteractiveObjectController> list = new List<InteractiveObjectController>(1);
			InteractiveObjectController[] controllers = owner.controllers;
			foreach (InteractiveObjectController interactiveObjectController in controllers)
			{
				if (interactiveObjectController.CanPlayerUse(player))
				{
					list.Add(interactiveObjectController);
				}
				else
				{
					interactiveObjectController.AttemptedInvalidUse(player);
				}
			}
			if (list.Count <= 0)
			{
				return;
			}
			InteractiveObjectController interactiveObjectController2 = list[UnityEngine.Random.Range(0, list.Count)];
			if (interactiveObjectController2.StartWithPlayer(player, OnDone))
			{
				NetworkComponent component = Utils.GetComponent<NetworkComponent>(player);
				if (component != null)
				{
					NetActionInteractiveObjectController action = new NetActionInteractiveObjectController(player, interactiveObjectController2);
					component.QueueNetAction(action);
				}
				else
				{
					CspUtils.DebugLog("No network component attached to the interactive object; usage will not be communicated");
				}
				blockTillDone = true;
			}
		}

		protected void OnDone(GameObject player, InteractiveObjectController.CompletionStateEnum completionState)
		{
			done = true;
			if (player == GameController.GetController().LocalPlayer && completionState != 0)
			{
			}
		}

		private string GetInteractiveObjectInGameURI()
		{
			string text = string.Empty;
			GameObject gameObject = owner.gameObject;
			while (gameObject != null && gameObject.name != null)
			{
				text = gameObject.name + "|" + text;
				gameObject = ((!(gameObject.transform.parent != null)) ? null : gameObject.transform.parent.gameObject);
			}
			text = text.Substring(0, text.Length - 1);
			GameController controller = GameController.GetController();
			string str = string.Empty;
			if (controller != null)
			{
				str = GameController.GetController().name + ":";
			}
			if (controller is SocialSpaceController)
			{
				str = str + ((SocialSpaceController)controller).ZoneName + "|";
			}
			return str + text;
		}
	}

	private const int FirstState = 1;

	private const int LastState = 4;

	private const int FirstOverlay = 5;

	private const int LastOverlay = 10;

	public bool hidden;

	public bool playerOnly = true;

	public bool highlightOnProximity = true;

	public bool highlightOnHover = true;

	public bool hoverOnlyInProximity = true;

	public bool highlightOnHoverAtAnyDistance;

	public bool clickAcceptedForEnable;

	public bool clickAcceptedForDisable;

	public float maxInteractRange = -1f;

	public bool pemoteLocalPlayerOnly;

	public bool checkAngle;

	public bool flipAngle;

	public float maxAngle = 45f;

	public bool checkHeight = true;

	public string interactionType = string.Empty;

	protected RootArray roots;

	protected InteractiveObjectController[] controllers;

	protected ShsFSM fsmBase;

	protected ShsFSM fsmOverlay;

	protected GameObject rolloverPlayer;

	protected GameObject localPlayer;

	protected GameObject playerInProximity;

	protected List<GameObject> colliders;

	protected GameObject lastPowerEmotePlayer;

	private ICaptureManager manager;

	public InteractiveObjectController[] Controllers
	{
		get
		{
			return controllers;
		}
	}

	public SHSInput.InputRequestorType InputRequestorType
	{
		get
		{
			return SHSInput.InputRequestorType.World;
		}
	}

	public bool CanHandleInput
	{
		get
		{
			return true;
		}
	}

	public ICaptureManager Manager
	{
		get
		{
			return manager;
		}
		set
		{
			manager = value;
		}
	}

	public void GetFsmForInspector(out ShsFSM fsmBase, out ShsFSM fsmOverlay)
	{
		fsmBase = this.fsmBase;
		fsmOverlay = this.fsmOverlay;
	}

	public void Start()
	{
		rolloverPlayer = null;
		playerInProximity = null;
		roots = new RootArray();
		foreach (Transform item in base.transform)
		{
			switch (item.gameObject.name)
			{
			case "Model":
				roots[StateIdx.Model] = item.gameObject;
				break;
			case "OnHidden":
				roots[StateIdx.Hidden] = item.gameObject;
				break;
			case "OnDisable":
				roots[StateIdx.Disable] = item.gameObject;
				break;
			case "OnEnable":
				roots[StateIdx.Enable] = item.gameObject;
				break;
			case "OnHighlight":
				roots[StateIdx.Highlight] = item.gameObject;
				break;
			case "OnHover":
				roots[StateIdx.Hover] = item.gameObject;
				break;
			case "OnProximity":
				roots[StateIdx.Proximity] = item.gameObject;
				break;
			case "OnEmote":
				roots[StateIdx.Emote] = item.gameObject;
				break;
			case "OnPowerEmote":
				roots[StateIdx.PowerEmote] = item.gameObject;
				break;
			case "OnClick":
				roots[StateIdx.Click] = item.gameObject;
				break;
			case "CollisionProxy":
				roots[StateIdx.CollisionProxy] = item.gameObject;
				break;
			}
		}
		Component[] componentsInChildren = GetComponentsInChildren(typeof(Component), true);
		foreach (Component component in componentsInChildren)
		{
			IInteractiveObjectChild interactiveObjectChild = component as IInteractiveObjectChild;
			if (interactiveObjectChild != null)
			{
				interactiveObjectChild.Initialize(this, roots[StateIdx.Model]);
			}
		}
		colliders = new List<GameObject>(1);
		if (base.gameObject != null)
		{
			Collider component2 = Utils.GetComponent<Collider>(base.gameObject);
			if (component2 != null && !(component2 is CharacterController))
			{
				GameObject gameObject = new GameObject("RootTrigger");
				gameObject.hideFlags = HideFlags.DontSave;
				if (component2.isTrigger)
				{
					gameObject.layer = 9;
				}
				else
				{
					gameObject.layer = 20;
				}
				gameObject.transform.parent = base.gameObject.transform;
				gameObject.transform.position = component2.transform.position;
				gameObject.transform.rotation = component2.transform.rotation;
				gameObject.transform.localScale = component2.transform.localScale;
				CopyColliderTo(component2, gameObject);
				InteractiveObjectForwarder interactiveObjectForwarder = Utils.AddComponent<InteractiveObjectForwarder>(gameObject);
				interactiveObjectForwarder.SetOwner(this, InteractiveObjectForwarder.Options.TiggerRolloverClick);
				UnityEngine.Object.Destroy(component2);
				colliders.Add(gameObject);
			}
		}
		if (roots[StateIdx.CollisionProxy] != null)
		{
			GameObject gameObject2 = roots[StateIdx.CollisionProxy];
			Collider component3 = Utils.GetComponent<Collider>(gameObject2);
			if (component3.isTrigger)
			{
				gameObject2.layer = 9;
			}
			else
			{
				gameObject2.layer = 20;
			}
			InteractiveObjectForwarder interactiveObjectForwarder2 = Utils.AddComponent<InteractiveObjectForwarder>(gameObject2);
			interactiveObjectForwarder2.SetOwner(this, InteractiveObjectForwarder.Options.TiggerRolloverClick);
			colliders.Add(gameObject2);
		}
		if (roots[StateIdx.Click] != null)
		{
			Collider component4 = Utils.GetComponent<Collider>(roots[StateIdx.Click]);
			if (component4 != null)
			{
				GameObject gameObject3 = new GameObject("ClickTrigger");
				gameObject3.hideFlags = HideFlags.DontSave;
				if (component4.isTrigger)
				{
					gameObject3.layer = 9;
				}
				else
				{
					gameObject3.layer = 20;
				}
				gameObject3.transform.parent = base.gameObject.transform;
				gameObject3.transform.position = component4.transform.position;
				gameObject3.transform.rotation = component4.transform.rotation;
				gameObject3.transform.localScale = component4.transform.localScale;
				CopyColliderTo(component4, gameObject3);
				InteractiveObjectForwarder interactiveObjectForwarder3 = Utils.AddComponent<InteractiveObjectForwarder>(gameObject3);
				interactiveObjectForwarder3.SetOwner(this, InteractiveObjectForwarder.Options.RolloverClick);
				UnityEngine.Object.Destroy(component4);
				colliders.Add(gameObject3);
			}
		}
		if (roots[StateIdx.Proximity] != null)
		{
			Collider component5 = Utils.GetComponent<Collider>(roots[StateIdx.Proximity]);
			if (component5 != null)
			{
				GameObject gameObject4 = new GameObject("ProximityTrigger");
				gameObject4.layer = component5.gameObject.layer;
				gameObject4.hideFlags = HideFlags.DontSave;
				gameObject4.transform.parent = base.gameObject.transform;
				gameObject4.transform.position = component5.transform.position;
				gameObject4.transform.rotation = component5.transform.rotation;
				gameObject4.transform.localScale = component5.transform.localScale;
				CopyColliderTo(component5, gameObject4);
				InteractiveObjectForwarder interactiveObjectForwarder4 = Utils.AddComponent<InteractiveObjectForwarder>(gameObject4);
				interactiveObjectForwarder4.SetOwner(this, InteractiveObjectForwarder.Options.Trigger);
				UnityEngine.Object.Destroy(component5);
				colliders.Add(gameObject4);
			}
		}
		controllers = base.gameObject.GetComponentsInChildren<InteractiveObjectController>();
		InteractiveObjectController[] array = controllers;
		foreach (InteractiveObjectController interactiveObjectController in array)
		{
			interactiveObjectController.Initialize(this, roots[StateIdx.Model]);
		}
		fsmBase = new ShsFSM();
		fsmBase.AddState(new HiddenState(this));
		fsmBase.AddState(new DisableState(this));
		fsmBase.AddState(new EnableState(this));
		fsmBase.AddState(new HighlightState(this));
		fsmOverlay = new ShsFSM();
		fsmOverlay.AddState(new NoOverlayState(this));
		fsmOverlay.AddState(new HoverState(this));
		fsmOverlay.AddState(new ProximityState(this));
		fsmOverlay.AddState(new EmoteState(this));
		fsmOverlay.AddState(new PowerEmoteState(this));
		fsmOverlay.AddState(new ClickState(this));
		GotoBestState();
	}

	public void Update()
	{
		fsmOverlay.Update();
		fsmBase.Update();
	}

	public void SetHidden(bool hidden)
	{
		if (hidden != this.hidden)
		{
			if (hidden)
			{
				rolloverPlayer = null;
			}
			this.hidden = hidden;
			GotoBestState();
		}
	}

	public void SetHiddenOn()
	{
		SetHidden(true);
	}

	public void SetHiddenOff()
	{
		SetHidden(false);
	}

	public void ResetForNewPlayer()
	{
		GotoBestState();
	}

	public void AddCollider(GameObject additionalCollider)
	{
		colliders.Add(additionalCollider);
	}

	public GameObject GetRoot(StateIdx state)
	{
		return roots[state];
	}

	public void SetRoot(StateIdx state, GameObject newRoot)
	{
		GameObject root = GetRoot(state);
		roots[state] = newRoot;
		InteractiveObjectController[] array = controllers;
		foreach (InteractiveObjectController interactiveObjectController in array)
		{
			interactiveObjectController.OnRootChanged(state, root, newRoot);
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		if (fsmBase == null || fsmBase.GetCurrentState() == typeof(HiddenState))
		{
			return;
		}
		SpawnData spawnData = other.gameObject.GetComponent(typeof(SpawnData)) as SpawnData;
		if (playerOnly && (!(spawnData != null) || spawnData.spawnType != CharacterSpawn.Type.LocalPlayer))
		{
			return;
		}
		playerInProximity = other.gameObject;
		if (fsmBase.GetCurrentState() != typeof(DisableState))
		{
			BaseState baseState = (BaseState)fsmOverlay.GetCurrentStateObject();
			if (baseState == null || baseState.rootIdx < StateIdx.Proximity)
			{
				fsmOverlay.GotoState<ProximityState>();
			}
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (playerInProximity == other.gameObject)
		{
			playerInProximity = null;
		}
	}

	public void OnMouseRolloverEnter(object data)
	{
		Type currentState = fsmBase.GetCurrentState();
		if (currentState == typeof(HiddenState) || currentState == typeof(DisableState))
		{
			return;
		}
		MouseRollover mouseRollover = data as MouseRollover;
		rolloverPlayer = mouseRollover.character;
		if (!hoverOnlyInProximity || (hoverOnlyInProximity && fsmOverlay.GetCurrentState() == typeof(ProximityState)))
		{
			BaseState baseState = (BaseState)fsmOverlay.GetCurrentStateObject();
			if (baseState.rootIdx < StateIdx.Hover)
			{
				GotoState(StateIdx.Hover);
			}
		}
		if (highlightOnHover && highlightOnHoverAtAnyDistance && fsmBase.GetCurrentState() != typeof(DisableState))
		{
			GotoState(StateIdx.Highlight);
		}
	}

	public void OnMouseRolloverExit()
	{
		rolloverPlayer = null;
		if (highlightOnHover && highlightOnHoverAtAnyDistance)
		{
			GotoBestState();
		}
	}

	public bool OnMouseClick(GameObject player)
	{
		if (WillAcceptMouseClick(player))
		{
			fsmOverlay.GotoState<ClickState>();
			ClickState clickState = fsmOverlay.GetCurrentStateObject() as ClickState;
			if (clickState != null)
			{
				clickState.DoClick(player);
			}
			if (interactionType == "door")
			{
				CspUtils.DebugLog("USING A DOOR " + base.gameObject.name);
				if (AppShell.Instance != null && AppShell.Instance.Profile != null)
				{
					if (AchievementManager.shouldReportAchievementEvent("generic_event", "open_door", string.Empty))
					{
						AppShell.Instance.delayedAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "generic_event", "open_door", string.Empty, 3f);
					}
					if (base.gameObject.name.ToLower().Contains("seasonal"))
					{
						AppShell.Instance.delayedAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "social_zone", base.gameObject.name, string.Empty, 3f);
						if (base.gameObject.name == "Seasonal_MaskDoor_frankie" && AppShell.Instance.Profile.SelectedCostume == "frankenstein" && PetDataManager.getCurrentPet() == 291839)
						{
							AppShell.Instance.delayedAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "social_zone", "halloween_triple_frank", string.Empty, 3f);
						}
					}
					if (base.gameObject.name == "Aunt_Mays")
					{
						AppShell.Instance.delayedAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "social_zone", "aunt_may_door", string.Empty, 3f);
					}
				}
			}
			else
			{
				CspUtils.DebugLog(base.gameObject.name);
				if (base.gameObject.name == "IceBox")
				{
					AppShell.Instance.delayedAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "social_zone", "vv_captain_america", string.Empty, 3f);
				}
			}
			return true;
		}
		return false;
	}

	public void ForceActivate(GameObject player)
	{
		fsmOverlay.GotoState<ClickState>();
		ClickState clickState = fsmOverlay.GetCurrentStateObject() as ClickState;
		if (clickState != null)
		{
			clickState.DoClick(player);
		}
	}

	public void OnPowerEmote(GameObject player)
	{
		if (!pemoteLocalPlayerOnly || !(player != GameController.GetController().LocalPlayer))
		{
			AppShell.Instance.EventMgr.Fire(this, new InteractiveObjectUsedMessage(this));
			BaseState baseState = (BaseState)fsmOverlay.GetCurrentStateObject();
			if (baseState == null || baseState.rootIdx < StateIdx.PowerEmote)
			{
				lastPowerEmotePlayer = player;
				GotoState(StateIdx.PowerEmote);
			}
		}
	}

	public GameObject GetLastPowerEmotePlayer()
	{
		return lastPowerEmotePlayer;
	}

	protected void CopyColliderTo(Collider src, GameObject dst)
	{
		if (src.GetType() == typeof(BoxCollider))
		{
			BoxCollider boxCollider = (BoxCollider)src;
			BoxCollider boxCollider2 = Utils.AddComponent<BoxCollider>(dst);
			boxCollider2.isTrigger = true;
			boxCollider2.center = boxCollider.center;
			boxCollider2.size = boxCollider.size;
		}
		else if (src.GetType() == typeof(SphereCollider))
		{
			SphereCollider sphereCollider = (SphereCollider)src;
			SphereCollider sphereCollider2 = Utils.AddComponent<SphereCollider>(dst);
			sphereCollider2.isTrigger = true;
			sphereCollider2.center = sphereCollider.center;
			sphereCollider2.radius = sphereCollider.radius;
		}
		else if (src.GetType() == typeof(CapsuleCollider))
		{
			CapsuleCollider capsuleCollider = (CapsuleCollider)src;
			CapsuleCollider capsuleCollider2 = Utils.AddComponent<CapsuleCollider>(dst);
			capsuleCollider2.isTrigger = true;
			capsuleCollider2.center = capsuleCollider.center;
			capsuleCollider2.radius = capsuleCollider.radius;
			capsuleCollider2.height = capsuleCollider.height;
			capsuleCollider2.direction = capsuleCollider.direction;
		}
		else if (src.GetType() == typeof(MeshCollider))
		{
			MeshCollider meshCollider = (MeshCollider)src;
			MeshCollider meshCollider2 = Utils.AddComponent<MeshCollider>(dst);
			meshCollider2.isTrigger = true;
			meshCollider2.sharedMesh = meshCollider.sharedMesh;
			meshCollider2.smoothSphereCollisions = meshCollider.smoothSphereCollisions;
			meshCollider2.convex = meshCollider.convex;
		}
	}

	public void GotoBestState()
	{
		StateIdx stateIdx = StateIdx.NoOverlay;
		StateIdx stateIdx2 = StateIdx.Hidden;
		GameObject player = GameController.GetController().LocalPlayer;
		if (!hidden)
		{
			if (controllers.Length > 0)
			{
				InteractiveObjectController[] array = controllers;
				foreach (InteractiveObjectController interactiveObjectController in array)
				{
					StateIdx stateForPlayer = interactiveObjectController.GetStateForPlayer(player);
					if (stateForPlayer > stateIdx2)
					{
						stateIdx2 = stateForPlayer;
					}
				}
			}
			else
			{
				stateIdx2 = StateIdx.Enable;
			}
			switch (stateIdx2)
			{
			case StateIdx.Hidden:
				GotoState(StateIdx.Hidden);
				GotoState(StateIdx.NoOverlay);
				return;
			case StateIdx.Disable:
				GotoState(StateIdx.Disable);
				GotoState(StateIdx.NoOverlay);
				return;
			}
			if (playerInProximity != null && StateIdx.Proximity > stateIdx)
			{
				stateIdx = StateIdx.Proximity;
			}
			if (rolloverPlayer != null)
			{
				if (!hoverOnlyInProximity || (hoverOnlyInProximity && stateIdx == StateIdx.Proximity))
				{
					stateIdx = StateIdx.Hover;
				}
				if (highlightOnHover && highlightOnHoverAtAnyDistance)
				{
					stateIdx2 = StateIdx.Highlight;
				}
			}
			if (stateIdx == StateIdx.Proximity && highlightOnProximity)
			{
				stateIdx2 = StateIdx.Highlight;
			}
			if (stateIdx == StateIdx.Hover && highlightOnHover)
			{
				stateIdx2 = StateIdx.Highlight;
			}
			GotoState(stateIdx2);
			GotoState(stateIdx);
		}
		else
		{
			GotoState(StateIdx.NoOverlay);
			GotoState(StateIdx.Hidden);
		}
	}

	protected void GotoState(StateIdx idx)
	{
		switch (idx)
		{
		case StateIdx.Hidden:
			fsmBase.GotoState<HiddenState>();
			break;
		case StateIdx.Disable:
			fsmBase.GotoState<DisableState>();
			break;
		case StateIdx.Enable:
			fsmBase.GotoState<EnableState>();
			break;
		case StateIdx.Highlight:
			fsmBase.GotoState<HighlightState>();
			break;
		case StateIdx.NoOverlay:
			fsmOverlay.GotoState<NoOverlayState>();
			break;
		case StateIdx.Proximity:
			fsmOverlay.GotoState<ProximityState>();
			break;
		case StateIdx.Hover:
			fsmOverlay.GotoState<HoverState>();
			break;
		case StateIdx.Emote:
			fsmOverlay.GotoState<EmoteState>();
			break;
		case StateIdx.PowerEmote:
			fsmOverlay.GotoState<PowerEmoteState>();
			break;
		case StateIdx.Click:
			fsmOverlay.GotoState<ClickState>();
			break;
		default:
			CspUtils.DebugLog("Unexpected state: " + idx.ToString());
			break;
		}
	}

	protected void ActiveStateBranch(StateIdx idxOn)
	{
		for (int i = 1; i <= 4; i++)
		{
			StateIdx stateIdx = (StateIdx)i;
			GameObject gameObject = roots[stateIdx];
			if (gameObject != null)
			{
				if (stateIdx == idxOn)
				{
					Utils.ForEachTree(gameObject, TurnOn);
					gameObject.BroadcastMessage("Triggered", this, SendMessageOptions.DontRequireReceiver);
				}
				else if (idxOn == StateIdx.Hidden)
				{
					Utils.ForEachTree(gameObject, TurnOffHard);
				}
				else
				{
					Utils.ForEachTree(gameObject, TurnOffSoft);
				}
			}
		}
	}

	protected void ActivateOverlayBranch(StateIdx idxOn)
	{
		for (int i = 5; i <= 10; i++)
		{
			StateIdx stateIdx = (StateIdx)i;
			GameObject gameObject = roots[stateIdx];
			if (gameObject != null)
			{
				if (stateIdx == idxOn)
				{
					Utils.ForEachTree(gameObject, TurnOn);
					gameObject.BroadcastMessage("Triggered", this, SendMessageOptions.DontRequireReceiver);
				}
				else
				{
					Utils.ForEachTree(gameObject, TurnOffSoft);
				}
			}
		}
	}

	protected void TurnOn(GameObject o)
	{
		o.active = true;
		if (o.particleEmitter != null)
		{
			o.particleEmitter.emit = true;
		}
	}

	protected void TurnOffSoft(GameObject o)
	{
		if (o.particleEmitter != null)
		{
			RequestSoftDisable component = Utils.GetComponent<RequestSoftDisable>(o);
			if (component != null)
			{
				o.particleEmitter.emit = false;
			}
			else
			{
				o.active = false;
			}
		}
		else
		{
			o.active = false;
		}
	}

	protected void TurnOffHard(GameObject o)
	{
		o.active = false;
	}

	protected float GetBranchLength(GameObject obj)
	{
		float num = 0f;
		if (obj != null)
		{
			Component[] componentsInChildren = obj.GetComponentsInChildren(typeof(Component));
			foreach (Component component in componentsInChildren)
			{
				IInteractiveObjectChild interactiveObjectChild = component as IInteractiveObjectChild;
				if (interactiveObjectChild != null)
				{
					num = Mathf.Max(num, interactiveObjectChild.GetLength());
				}
			}
		}
		return num;
	}

	public bool WillAcceptMouseClick(GameObject player)
	{
		if (controllers.Length > 0)
		{
			InteractiveObjectController[] array = controllers;
			foreach (InteractiveObjectController interactiveObjectController in array)
			{
				if (interactiveObjectController.ShouldIgnoreMouseClick(player))
				{
					return false;
				}
			}
		}
		CombatController component = player.GetComponent<CombatController>();
		if (component != null && component.IsInteractRestricted)
		{
			return false;
		}
		bool flag = true;
		BaseState baseState = (BaseState)fsmBase.GetCurrentStateObject();
		switch (baseState.rootIdx)
		{
		case StateIdx.Hidden:
		case StateIdx.Disable:
			flag = !clickAcceptedForDisable;
			break;
		case StateIdx.Highlight:
			flag = false;
			break;
		case StateIdx.Enable:
			flag = !clickAcceptedForEnable;
			break;
		}
		if (flag)
		{
			return false;
		}
		baseState = (BaseState)fsmOverlay.GetCurrentStateObject();
		switch (baseState.rootIdx)
		{
		case StateIdx.NoOverlay:
		case StateIdx.Proximity:
		case StateIdx.Hover:
			flag = false;
			break;
		case StateIdx.Emote:
		case StateIdx.PowerEmote:
		case StateIdx.Click:
			flag = true;
			break;
		}
		if (flag)
		{
			return false;
		}
		if (maxInteractRange > 0f)
		{
			float sqrMagnitude = (player.transform.position - base.transform.position).sqrMagnitude;
			if (sqrMagnitude > maxInteractRange * maxInteractRange)
			{
				return false;
			}
		}
		if (checkAngle)
		{
			float num = Mathf.Cos(maxAngle * ((float)Math.PI / 180f));
			foreach (GameObject collider in colliders)
			{
				InteractiveObjectForwarder component2 = Utils.GetComponent<InteractiveObjectForwarder>(collider);
				if (!(component2 == null) && (component2.options & InteractiveObjectForwarder.Options.Click) != 0)
				{
					Vector3 lhs = (!flipAngle) ? collider.transform.forward : (-collider.transform.up);
					lhs.y = 0f;
					lhs.Normalize();
					Vector3 rhs = player.transform.position - collider.collider.bounds.center;
					rhs.y = 0f;
					rhs.Normalize();
					if (Vector3.Dot(lhs, rhs) < num)
					{
						return false;
					}
				}
			}
		}
		if (checkHeight)
		{
			Vector3 position = base.transform.position;
			float y = position.y;
			Vector3 position2 = player.transform.position;
			float num2 = Math.Abs(y - position2.y);
			if (num2 > 3f)
			{
				return false;
			}
		}
		return true;
	}

	protected void InternalSetCursor(GUICursorManager.CursorType newType)
	{
		GUICursorManager.CursorType cursorType = GUIManager.Instance.CursorManager.GetCursorType();
		GUICursorManager.CursorType cursorType2;
		if (newType == GUICursorManager.CursorType.Interactable)
		{
			if (rolloverPlayer == null || cursorType == GUICursorManager.CursorType.Click)
			{
				return;
			}
			cursorType2 = (WillAcceptMouseClick(rolloverPlayer) ? GUICursorManager.CursorType.Interactable : GUICursorManager.CursorType.Normal);
		}
		else
		{
			cursorType2 = newType;
		}
		if (cursorType2 != cursorType)
		{
			GUIManager.Instance.CursorManager.SetCursorType(cursorType2);
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (maxInteractRange > 0f)
		{
			Gizmos.color = new Color(0f, 0.3f, 0.7f, 0.2f);
			Gizmos.DrawSphere(base.transform.position, maxInteractRange);
		}
	}

	public Dictionary<KeyCodeEntry, SHSInput.KeyEventDelegate> GetKeyList(GUIControl.KeyInputState inputState)
	{
		throw new NotImplementedException();
	}

	public void ConfigureKeyBanks()
	{
		throw new NotImplementedException();
	}

	public bool IsDescendantHandler(IInputHandler handler)
	{
		return this == handler;
	}

	public CaptureHandlerResponse HandleCapture(SHSKeyCode code)
	{
		if (code.source != this)
		{
			return CaptureHandlerResponse.Block;
		}
		if (code.code == KeyCode.Mouse0 || code.code == KeyCode.Mouse1)
		{
			Manager.CaptureHandlerGotInput(this);
		}
		return CaptureHandlerResponse.Passthrough;
	}

	public void OnCaptureAcquired()
	{
		CspUtils.DebugLog(ToString() + " got captured!");
	}

	public void OnCaptureUnacquired()
	{
		CspUtils.DebugLog(ToString() + " got uncaptured!");
	}
}
