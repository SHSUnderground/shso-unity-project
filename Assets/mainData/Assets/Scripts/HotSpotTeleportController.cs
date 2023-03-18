using System.Collections;
using UnityEngine;

public class HotSpotTeleportController : HotSpotController
{
	protected class PlayerBlob
	{
		protected HotSpotTeleportController owner;

		protected GameObject player;

		protected BehaviorManager behaviorManager;

		protected bool isLocalPlayer;

		protected bool haveChangedCameras;

		public PlayerBlob(HotSpotTeleportController owner, GameObject player)
		{
			this.owner = owner;
			this.player = player;
			haveChangedCameras = false;
			if (Utils.GetComponent<PlayerInputController>(player) != null)
			{
				isLocalPlayer = true;
			}
			else
			{
				isLocalPlayer = false;
			}
		}

		public bool Start()
		{
			behaviorManager = Utils.GetComponent<BehaviorManager>(player);
			if (behaviorManager == null)
			{
				CspUtils.DebugLog("Player should always have a BehaviorManager");
				return false;
			}
			BehaviorApproach behaviorApproach = behaviorManager.requestChangeBehavior(typeof(BehaviorApproach), false) as BehaviorApproach;
			if (behaviorApproach == null)
			{
				return false;
			}
			if (isLocalPlayer)
			{
				GameObject[] noFadeList = owner.noFadeList;
				foreach (GameObject gameObject in noFadeList)
				{
					gameObject.layer = 0;
				}
			}
			CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(player);
			if (component != null)
			{
				component.DisableNetUpdates(true);
			}
			behaviorApproach.Initialize(owner.enterDoorState.InsidePoint.transform.position, owner.enterDoorState.InsidePoint.transform.rotation, false, ArrivedEnterDoor, CancelEnterDoor, 0f, 2f, false, true);
			owner.enterDoorState.Open();
			if (isLocalPlayer && owner.enterCamera != null && owner.cameraMgr != null)
			{
				haveChangedCameras = true;
				owner.cameraMgr.PushCamera(owner.enterCamera, owner.blendToEnter);
			}
			return true;
		}

		protected void ArrivedEnterDoor(GameObject player)
		{
			owner.enterDoorState.Close();
			owner.StartCoroutine(WaitAndTeleport());
		}

		protected void CancelEnterDoor(GameObject player)
		{
			player.transform.position = owner.enterDoorState.InsidePoint.transform.position;
			CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(player);
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
			CharacterMotionController mc = Utils.GetComponent<CharacterMotionController>(player);
			mc.teleportTo(owner.exitDoorState.InsidePoint.transform.position);
			mc.setDestination(owner.exitDoorState.InsidePoint);
			BehaviorApproach approach = behaviorManager.requestChangeBehavior(typeof(BehaviorApproach), false) as BehaviorApproach;
			approach.Initialize(owner.exitDoorState.OutsidePoint.transform.position, owner.exitDoorState.OutsidePoint.transform.rotation, false, ArrivedExitDoor, CancelExitDoor, 0f, 2f, false, true);
			owner.exitDoorState.Open();
		}

		protected void ArrivedExitDoor(GameObject player)
		{
			owner.exitDoorState.Close();
			if (haveChangedCameras)
			{
				owner.cameraMgr.PopCamera(owner.blendToPlayer);
			}
			CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(player);
			if (component != null)
			{
				component.DisableNetUpdates(false);
			}
			if (!isLocalPlayer)
			{
				return;
			}
			for (int i = 0; i < owner.oldLayer.Length; i++)
			{
				if (owner.noFadeList[i] != null)
				{
					owner.noFadeList[i].layer = owner.oldLayer[i];
				}
			}
		}

		protected void CancelExitDoor(GameObject player)
		{
			player.transform.position = owner.exitDoorState.OutsidePoint.transform.position;
			CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(player);
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

	public MovementTypes movementType = MovementTypes.Generic;

	public CameraLiteManager cameraMgr;

	public GameObject enterDoor;

	public GameObject exitDoor;

	public CameraLite enterCamera;

	public CameraLite exitCamera;

	public float blendToEnter = 0.5f;

	public float blendToExit = -1f;

	public float blendToPlayer = 1f;

	public GameObject[] noFadeList;

	protected int[] oldLayer;

	protected DoorObject enterDoorState;

	protected DoorObject exitDoorState;

	public override MovementTypes MovementType
	{
		get
		{
			return movementType;
		}
	}

	public void Start()
	{
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

	public override bool CanPlayerUse(GameObject player)
	{
		return true;
	}

	public override bool StartWithPlayer(GameObject player)
	{
		PlayerBlob playerBlob = new PlayerBlob(this, player);
		return playerBlob.Start();
	}
}
