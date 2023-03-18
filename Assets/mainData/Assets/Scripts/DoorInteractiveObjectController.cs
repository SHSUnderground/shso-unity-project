using System.Collections;
using UnityEngine;

public class DoorInteractiveObjectController : InteractiveObjectController
{
	protected class PlayerBlob
	{
		protected DoorInteractiveObjectController owner;

		protected GameObject player;

		protected BehaviorManager behaviorManager;

		protected bool isLocalPlayer;

		protected bool haveChangedCameras;

		protected OnDone onDone;

		public PlayerBlob(DoorInteractiveObjectController owner, GameObject player, OnDone onDone)
		{
			this.owner = owner;
			this.player = player;
			haveChangedCameras = false;
			isLocalPlayer = Utils.IsLocalPlayer(player);
			this.onDone = onDone;
		}

		public bool Start()
		{
			behaviorManager = player.GetComponent<BehaviorManager>();
			if (behaviorManager == null)
			{
				CspUtils.DebugLog("Player should always have a BehaviorManager");
				return false;
			}
			BehaviorApproach behaviorApproach = behaviorManager.requestChangeBehavior(typeof(BehaviorApproachUninterruptable), false) as BehaviorApproach;
			if (behaviorApproach == null)
			{
				return false;
			}
			if (isLocalPlayer)
			{
				GameObject[] noFadeList = owner.noFadeList;
				foreach (GameObject gameObject in noFadeList)
				{
					if (gameObject != null)
					{
						gameObject.layer = 0;
						continue;
					}
					GameObject interactiveObjectParent = GetInteractiveObjectParent(owner.gameObject);
					CspUtils.DebugLogWarning("Door " + ((!(interactiveObjectParent != null)) ? owner.gameObject.name : interactiveObjectParent.name) + " fade list is broken");
				}
			}
			CharacterMotionController component = player.GetComponent<CharacterMotionController>();
			if (component != null)
			{
				component.DisableNetUpdates(true);
			}
			Physics.IgnoreCollision(owner.enterDoorCollider, player.GetComponent<CharacterController>(), true);
			behaviorApproach.Initialize(owner.enterDoorState.InsidePoint.transform.position, owner.enterDoorState.InsidePoint.transform.rotation, false, ArrivedEnterDoor, CancelEnterDoor, 0.15f, 2f, false, true);
			owner.enterDoorState.Open();
			if (isLocalPlayer && owner.enterCamera != null && owner.cameraMgr != null)
			{
				haveChangedCameras = true;
				owner.cameraMgr.PushCamera(owner.enterCamera, owner.blendToEnter);
			}
			ShsAudioSource.PlayAutoSound(owner.soundEffect, owner.transform);
			return true;
		}

		protected GameObject GetInteractiveObjectParent(GameObject go)
		{
			if (go.transform.parent == null)
			{
				return null;
			}
			if (go.GetComponent<InteractiveObject>() != null)
			{
				return go;
			}
			return GetInteractiveObjectParent(go.transform.parent.gameObject);
		}

		protected void ArrivedEnterDoor(GameObject player)
		{
			owner.enterDoorState.Close();
			Physics.IgnoreCollision(owner.enterDoorCollider, player.GetComponent<CharacterController>(), false);
			owner.StartCoroutine(WaitAndTeleport());
		}

		protected void CancelEnterDoor(GameObject player)
		{
			player.transform.position = owner.enterDoorState.InsidePoint.transform.position;
			CharacterMotionController component = player.GetComponent<CharacterMotionController>();
			component.teleportTo(player.transform.position);
			component.setDestination(player.transform.position);
			ArrivedEnterDoor(player);
		}

		protected IEnumerator WaitAndTeleport()
		{
			if (!(player != null))
			{
				yield break;
			}
			yield return new WaitForSeconds(owner.enterDoorState.AnimLength);
			if (isLocalPlayer && owner.exitCamera != null && owner.cameraMgr != null)
			{
				if (haveChangedCameras)
				{
					owner.cameraMgr.ReplaceCamera(owner.exitCamera, owner.blendToExit);
				}
				else
				{
					owner.cameraMgr.PushCamera(owner.exitCamera, owner.blendToExit);
				}
				haveChangedCameras = true;
			}
			CharacterMotionController mc = player.GetComponent<CharacterMotionController>();
			mc.teleportTo(owner.exitDoorState.InsidePoint.transform.position);
			mc.setDestination(owner.exitDoorState.InsidePoint);
			Physics.IgnoreCollision(owner.exitDoorCollider, player.GetComponent<CharacterController>(), true);
			BehaviorApproach approach = behaviorManager.requestChangeBehavior(typeof(BehaviorApproachUninterruptable), false) as BehaviorApproach;
			approach.Initialize(owner.exitDoorState.OutsidePoint.transform.position, owner.exitDoorState.OutsidePoint.transform.rotation, false, ArrivedExitDoor, CancelExitDoor, 0f, 2f, false, true);
			owner.exitDoorState.Open();
		}

		protected void ArrivedExitDoor(GameObject player)
		{
			owner.exitDoorState.Close();
			Physics.IgnoreCollision(owner.exitDoorCollider, player.GetComponent<CharacterController>(), false);
			if (haveChangedCameras)
			{
				owner.cameraMgr.PopCamera(owner.blendToPlayer);
			}
			CharacterMotionController component = player.GetComponent<CharacterMotionController>();
			if (component != null)
			{
				component.DisableNetUpdates(false);
			}
			if (isLocalPlayer)
			{
				for (int i = 0; i < owner.oldLayer.Length; i++)
				{
					if (owner.noFadeList[i] != null)
					{
						owner.noFadeList[i].layer = owner.oldLayer[i];
					}
				}
			}
			if (onDone != null)
			{
				onDone(player, CompletionStateEnum.Success);
			}
		}

		protected void CancelExitDoor(GameObject player)
		{
			player.transform.position = owner.exitDoorState.OutsidePoint.transform.position;
			CharacterMotionController component = player.GetComponent<CharacterMotionController>();
			component.teleportTo(player.transform.position);
			component.setDestination(player.transform.position);
			ArrivedExitDoor(player);
		}
	}

	protected class DoorObject
	{
		protected enum State
		{
			Closed,
			Closing,
			Opening,
			Open
		}

		protected string animName = "Take 001";

		protected GameObject door;

		protected DockPoint insideDockPoint;

		protected DockPoint outsideDockPoint;

		protected State state;

		protected bool closeRequested;

		public GameObject InsidePoint
		{
			get
			{
				return insideDockPoint.gameObject;
			}
		}

		public GameObject OutsidePoint
		{
			get
			{
				return outsideDockPoint.gameObject;
			}
		}

		public float AnimLength
		{
			get
			{
				return door.animation[animName].clip.length;
			}
		}

		public DoorObject(GameObject door)
		{
			this.door = door;
			state = State.Closed;
			closeRequested = false;
			DockPoint[] components = Utils.GetComponents<DockPoint>(door, Utils.SearchChildren);
			foreach (DockPoint dockPoint in components)
			{
				if (dockPoint.gameObject.name.Contains("use"))
				{
					insideDockPoint = dockPoint;
				}
				else if (dockPoint.gameObject.name.Contains("entry"))
				{
					outsideDockPoint = dockPoint;
				}
			}
			if (insideDockPoint == null)
			{
				CspUtils.DebugLog("No inside dock point on " + door.name);
			}
			if (outsideDockPoint == null)
			{
				CspUtils.DebugLog("No outside dock point on " + door.name);
			}
			if (insideDockPoint == null)
			{
				foreach (Transform item in door.transform)
				{
					if (item.gameObject.name.Contains("use"))
					{
						insideDockPoint = Utils.AddComponent<DockPoint>(item.gameObject);
						break;
					}
				}
			}
			if (outsideDockPoint == null)
			{
				foreach (Transform item2 in door.transform)
				{
					if (item2.gameObject.name.Contains("entry"))
					{
						outsideDockPoint = Utils.AddComponent<DockPoint>(item2.gameObject);
						break;
					}
				}
			}
			AnimationState animationState = door.animation[animName];
			if (animationState != null)
			{
				animationState.enabled = false;
				animationState.time = 0f;
				animationState.speed = 1f;
			}
		}

		public void Open()
		{
			closeRequested = false;
			switch (state)
			{
			case State.Opening:
			case State.Open:
				break;
			case State.Closed:
			{
				AnimationState animationState2 = door.animation[animName];
				animationState2.time = 0f;
				animationState2.speed = 1f;
				animationState2.wrapMode = WrapMode.ClampForever;
				door.animation.Play(animName);
				state = State.Opening;
				break;
			}
			case State.Closing:
			{
				AnimationState animationState = door.animation[animName];
				animationState.speed = 1f;
				animationState.wrapMode = WrapMode.ClampForever;
				door.animation.Play(animName);
				state = State.Opening;
				break;
			}
			}
		}

		public void Close()
		{
			closeRequested = true;
			if (state == State.Open)
			{
				AnimationState animationState = door.animation[animName];
				animationState.time = animationState.clip.length;
				animationState.speed = -1f;
				animationState.wrapMode = WrapMode.ClampForever;
				door.animation.Play(animName);
				state = State.Closing;
			}
		}

		public void Update()
		{
			switch (state)
			{
			case State.Closed:
				break;
			case State.Open:
				if (closeRequested)
				{
					Close();
				}
				break;
			case State.Closing:
			{
				AnimationState animationState2 = door.animation[animName];
				if (animationState2.time <= 0f)
				{
					animationState2.enabled = false;
					state = State.Closed;
				}
				break;
			}
			case State.Opening:
			{
				AnimationState animationState = door.animation[animName];
				if (animationState.time >= animationState.clip.length)
				{
					animationState.enabled = false;
					state = State.Open;
				}
				break;
			}
			}
		}
	}

	public CameraLiteManager cameraMgr;

	public GameObject enterDoor;

	public GameObject exitDoor;

	public CameraLite enterCamera;

	public CameraLite exitCamera;

	public float blendToEnter = 0.5f;

	public float blendToExit = -1f;

	public float blendToPlayer = 1f;

	public GameObject soundEffect;

	public GameObject[] noFadeList;

	protected int[] oldLayer;

	protected DoorObject enterDoorState;

	protected DoorObject exitDoorState;

	protected Collider enterDoorCollider;

	protected Collider exitDoorCollider;

	public void Start()
	{
		CspUtils.DebugLog("XXXXX - DoorInteractiveObjectController is deprecated <" + base.gameObject.name + ">");
		if ((bool)enterDoor)
		{
			enterDoorState = new DoorObject(enterDoor);
		}
		if ((bool)exitDoor)
		{
			exitDoorState = new DoorObject(exitDoor);
		}
		if (cameraMgr == null)
		{
			cameraMgr = CameraLiteManager.Instance;
		}
		if (noFadeList == null)
		{
			return;
		}
		oldLayer = new int[noFadeList.Length];
		for (int i = 0; i < oldLayer.Length; i++)
		{
			if (noFadeList[i] != null)
			{
				oldLayer[i] = noFadeList[i].layer;
			}
		}
	}

	public void Update()
	{
		if (enterDoorState != null)
		{
			enterDoorState.Update();
		}
		if (exitDoorState != null)
		{
			exitDoorState.Update();
		}
	}

	public override void Initialize(InteractiveObject owner, GameObject model)
	{
		base.Initialize(owner, model);
		enterDoorCollider = DetachCollider(enterDoor);
		exitDoorCollider = DetachCollider(exitDoor);
	}

	public override InteractiveObject.StateIdx GetStateForPlayer(GameObject player)
	{
		return InteractiveObject.StateIdx.Enable;
	}

	public override bool CanPlayerUse(GameObject player)
	{
		return true;
	}

	public override bool StartWithPlayer(GameObject player, OnDone callback)
	{
		PlayerBlob playerBlob = new PlayerBlob(this, player, callback);
		return playerBlob.Start();
	}

	protected Collider DetachCollider(GameObject parent)
	{
		MeshCollider componentInChildren = parent.GetComponentInChildren<MeshCollider>();
		if (componentInChildren != null)
		{
			GameObject gameObject = new GameObject("detached_collider");
			gameObject.transform.parent = componentInChildren.gameObject.transform.parent.transform;
			gameObject.transform.position = componentInChildren.transform.position;
			gameObject.transform.rotation = componentInChildren.transform.rotation;
			gameObject.layer = componentInChildren.gameObject.layer;
			MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
			meshCollider.sharedMesh = componentInChildren.sharedMesh;
			meshCollider.isTrigger = componentInChildren.isTrigger;
			meshCollider.smoothSphereCollisions = componentInChildren.smoothSphereCollisions;
			meshCollider.convex = true;
			Object.Destroy(componentInChildren);
			if (meshCollider.isTrigger)
			{
				gameObject.layer = 9;
			}
			else
			{
				gameObject.layer = 20;
			}
			InteractiveObjectForwarder interactiveObjectForwarder = Utils.AddComponent<InteractiveObjectForwarder>(gameObject);
			interactiveObjectForwarder.SetOwner(owner, InteractiveObjectForwarder.Options.TiggerRolloverClick);
			owner.AddCollider(gameObject);
			return meshCollider;
		}
		return null;
	}
}
