using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : InteractiveObjectController
{
	protected enum DoorState
	{
		Closed,
		Opening,
		Closing
	}

	protected enum PlayerState
	{
		Entering,
		Exiting
	}

	public class PlayerBlob
	{
		protected DoorManager owner;

		protected GameObject player;

		protected OnDone onDone;

		protected bool isLocalPlayer;

		protected bool haveChangedCameras;

		public PlayerBlob(DoorManager owner, GameObject player, OnDone onDone)
		{
			this.owner = owner;
			this.player = player;
			this.onDone = onDone;
			haveChangedCameras = false;
			isLocalPlayer = Utils.IsLocalPlayer(player);
		}

		public virtual bool StartEnter()
		{
			BehaviorManager component = player.GetComponent<BehaviorManager>();
			BehaviorApproach behaviorApproach = component.requestChangeBehavior<BehaviorApproach>(false);
			if (behaviorApproach == null)
			{
				return false;
			}
			behaviorApproach.Initialize(owner.outsideDockPoint.transform.position, owner.outsideDockPoint.transform.rotation, !owner.forcedApproach, ArrivedOutsideDockPoint, ApproachCancelled, 0.2f, 0.3f, false, owner.disableEntryRotation);
			return true;
		}

		public void ArrivedOutsideDockPoint(GameObject player)
		{
			BehaviorManager component = Utils.GetComponent<BehaviorManager>(player);
			component.ChangeDefaultBehavior("BehaviorWait");
			BehaviorDelay behaviorDelay = component.requestChangeBehavior(typeof(BehaviorDelay), false) as BehaviorDelay;
			if (behaviorDelay == null)
			{
				ApproachCancelled(player);
				return;
			}
			if (owner.highlightAnimation != null)
			{
				owner.controller.highlightOnProximity = false;
				owner.controller.highlightOnHover = false;
				owner.controller.GotoBestState();
			}
			TurnFadingOff();
			CharacterMotionController component2 = player.GetComponent<CharacterMotionController>();
			if (component2 != null)
			{
				component2.DisableNetUpdates(true);
			}
			CharacterController component3 = player.GetComponent<CharacterController>();
			foreach (Collider doorCollider in owner.doorColliders)
			{
				Physics.IgnoreCollision(doorCollider, component3, true);
			}
			if (isLocalPlayer && owner.cameraOverride != null && CameraLiteManager.Instance != null)
			{
				haveChangedCameras = true;
				CameraLiteManager.Instance.PushCamera(owner.cameraOverride, owner.cameraBlendToEnter);
			}
			foreach (Collider doorCollider2 in owner.doorColliders)
			{
				Physics.IgnoreCollision(doorCollider2, component3, true);
			}
			PlayEnterVO(player);
			owner.playerState = PlayerState.Entering;
			owner.OpenDoor(0f);
			behaviorDelay.Initialize(owner.enterDelay, (owner.InsidePoint.transform.position - player.transform.position).normalized, StartEnterApproach);
		}

		public void ApproachCancelled(GameObject player)
		{
			if (onDone != null)
			{
				onDone(player, CompletionStateEnum.Canceled);
			}
		}

		public void StartEnterApproach()
		{
			BehaviorManager component = Utils.GetComponent<BehaviorManager>(player);
			BehaviorApproach behaviorApproach = component.forceChangeBehavior(typeof(BehaviorApproachUninterruptable)) as BehaviorApproach;
			behaviorApproach.Initialize(owner.InsidePoint.transform.position, owner.InsidePoint.transform.rotation, false, ArrivedEnterDoor, CancelEnterDoor, 0.15f, 2f, false, true);
		}

		public virtual void StartExit(bool hasChangedCameras)
		{
			haveChangedCameras = hasChangedCameras;
			TurnFadingOff();
			if (isLocalPlayer && owner.cameraOverride != null && CameraLiteManager.Instance != null)
			{
				haveChangedCameras = true;
				CameraLiteManager.Instance.ReplaceCamera(owner.cameraOverride, -1f);
			}
			if (owner.highlightAnimation != null)
			{
				owner.controller.highlightOnProximity = false;
				owner.controller.highlightOnHover = false;
				owner.controller.GotoBestState();
			}
			CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(player);
			component.teleportTo(owner.InsidePoint.transform.position, Quaternion.identity);
			component.setDestination(owner.InsidePoint);
			owner.PlayExitEffects(player);
			CharacterController component2 = Utils.GetComponent<CharacterController>(player);
			foreach (Collider doorCollider in owner.doorColliders)
			{
				if (doorCollider.gameObject.active)
				{
					Physics.IgnoreCollision(doorCollider, component2, true);
				}
			}
			owner.playerState = PlayerState.Exiting;
			owner.OpenDoor(0.75f);
			owner.StartCoroutine(WaitAndMove());
		}

		protected void ArrivedEnterDoor(GameObject player)
		{
			CharacterController component = Utils.GetComponent<CharacterController>(player);
			foreach (Collider doorCollider in owner.doorColliders)
			{
				Physics.IgnoreCollision(doorCollider, component, false);
			}
			owner.StartCoroutine(OnDoorEntered());
		}

		protected void CancelEnterDoor(GameObject player)
		{
			player.transform.position = owner.InsidePoint.transform.position;
			CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(player);
			component.teleportTo(player.transform.position);
			component.setDestination(player.transform.position);
			ArrivedEnterDoor(player);
		}

		protected virtual IEnumerator OnDoorEntered()
		{
			if (player != null)
			{
				yield return new WaitForSeconds(owner.AnimLength);
				if (!(player == null))
				{
					TurnFadingOn();
					owner.controller.highlightOnHover = true;
					owner.controller.highlightOnProximity = true;
					owner.PlayEnterEffects(player);
					DoorManager exit2 = null;
					exit2 = ((!(owner.exit != null)) ? owner : owner.exit);
					exit2.ExitWithPlayer(player, onDone, haveChangedCameras);
					player = null;
					onDone = null;
					owner = null;
				}
			}
			else
			{
				onDone(player, CompletionStateEnum.Unknown);
			}
		}

		protected IEnumerator WaitAndMove()
		{
			if (player != null)
			{
				float delay = owner.exitDelay;
				if (delay <= 0f)
				{
					delay = owner.AnimLength;
				}
				yield return new WaitForSeconds(delay);
				if (!(player == null))
				{
					BehaviorManager behaviorManager = Utils.GetComponent<BehaviorManager>(player);
					behaviorManager.ChangeDefaultBehavior("BehaviorMovement");
					BehaviorApproach approach = behaviorManager.requestChangeBehavior(typeof(BehaviorApproachUninterruptable), false) as BehaviorApproach;
					approach.Initialize(owner.OutsidePoint.transform.position, owner.OutsidePoint.transform.rotation, false, ArrivedExitDoor, CancelExitDoor, 0f, 2f, false, true);
				}
			}
		}

		protected virtual void ArrivedExitDoor(GameObject player)
		{
			CharacterController component = Utils.GetComponent<CharacterController>(player);
			foreach (Collider doorCollider in owner.doorColliders)
			{
				if (doorCollider.gameObject.active)
				{
					Physics.IgnoreCollision(doorCollider, component, false);
				}
			}
			owner.controller.StartCoroutine(WaitForExitDoorClose());
			TurnFadingOn();
			if (haveChangedCameras)
			{
				CameraLiteManager.Instance.PopCamera(owner.cameraBlendToPlayer);
			}
			CharacterMotionController component2 = Utils.GetComponent<CharacterMotionController>(player);
			if (component2 != null)
			{
				component2.DisableNetUpdates(false);
			}
			if (onDone != null)
			{
				onDone(player, CompletionStateEnum.Success);
			}
		}

		protected IEnumerator WaitForExitDoorClose()
		{
			while (owner.doorAnim.isPlaying)
			{
				yield return 0;
			}
			owner.OnExitDoorClosed();
		}

		protected virtual void CancelExitDoor(GameObject player)
		{
			player.transform.position = owner.OutsidePoint.transform.position;
			CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(player);
			component.teleportTo(player.transform.position);
			component.setDestination(player.transform.position);
			ArrivedExitDoor(player);
		}

		protected virtual void PlayEnterVO(GameObject player)
		{
			if (DoorVOCooldown.OkToPlay(player))
			{
				VOManager.Instance.PlayVO("door_entry", player);
				DoorVOCooldown.StartCooldown(player);
			}
		}

		protected void TurnFadingOff()
		{
			if (isLocalPlayer)
			{
				foreach (GameObject noFadeObject in owner.noFadeObjects)
				{
					noFadeObject.layer = 0;
				}
			}
		}

		protected void TurnFadingOn()
		{
			if (isLocalPlayer)
			{
				for (int i = 0; i < owner.oldLayer.Count; i++)
				{
					owner.noFadeObjects[i].layer = owner.oldLayer[i];
				}
			}
		}
	}

	public bool disabled;

	public DoorManager exit;

	public CameraLite cameraOverride;

	public float cameraBlendToEnter = 0.5f;

	public float cameraBlendToPlayer = 1f;

	public float enterDelay = -1f;

	public float exitDelay = -1f;

	public string animName = "Take 001";

	public AnimationClip highlightAnimation;

	public float highlightRadius = 4f;

	public GameObject soundOpen;

	public bool playOpenSoundOnExit = true;

	public GameObject soundClose;

	public bool playCloseSoundOnExit = true;

	public EffectSequence effectOnEnter;

	public EffectSequence effectOnExit;

	public string characterEffectOnEnter;

	public string characterEffectOnExit;

	public bool allowEffectsToStack;

	public string[] effectsToReplace;

	public string[] noFadeList;

	public bool useDetachedSphereCollider;

	public float detachedSphereColliderRadius;

	public bool ignoreBoxColliders;

	public bool disableEntryRotation;

	public bool forcedApproach;

	public float topOffsetTolerance;

	public float bottomOffsetTolerance;

	protected InteractiveObject controller;

	protected GameObject sfxInstance;

	protected List<Collider> doorColliders;

	protected List<GameObject> noFadeObjects;

	protected List<int> oldLayer;

	protected DockPoint insideDockPoint;

	protected DockPoint outsideDockPoint;

	protected Animation doorAnim;

	protected DoorState doorState;

	protected PlayerState playerState;

	protected float timeTillClose;

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
			return doorAnim[animName].clip.length;
		}
	}

	public virtual void OnEnable()
	{
		doorColliders = new List<Collider>();
	}

	public virtual void Start()
	{
		if (disabled)
		{
			Object.Destroy(this);
			return;
		}
		noFadeObjects = new List<GameObject>(noFadeList.Length);
		oldLayer = new List<int>(noFadeList.Length);
		string[] array = noFadeList;
		foreach (string name in array)
		{
			GameObject gameObject = GameObject.Find(name);
			if (gameObject != null)
			{
				noFadeObjects.Add(gameObject);
				oldLayer.Add(gameObject.layer);
			}
		}
		doorAnim = Utils.GetComponent<Animation>(base.gameObject, Utils.SearchChildren);
		GameObject gameObject2 = new GameObject(base.gameObject.name);
		gameObject2.transform.parent = base.transform.parent;
		gameObject2.transform.position = base.transform.position;
		gameObject2.transform.rotation = base.transform.rotation;
		controller = gameObject2.AddComponent<InteractiveObject>();
		controller.hidden = false;
		controller.playerOnly = true;
		controller.highlightOnProximity = true;
		controller.highlightOnHover = true;
		controller.hoverOnlyInProximity = false;
		controller.highlightOnHoverAtAnyDistance = true;
		controller.clickAcceptedForEnable = true;
		controller.maxInteractRange = -1f;
		controller.pemoteLocalPlayerOnly = false;
		controller.checkAngle = true;
		controller.flipAngle = true;
		controller.maxAngle = 75f;
		controller.checkHeight = false;
		controller.interactionType = "door";
		base.transform.parent = gameObject2.transform;
		base.transform.localPosition = Vector3.zero;
		base.transform.localRotation = Quaternion.identity;
		if (highlightAnimation != null)
		{
			GameObject gameObject3 = new GameObject("OnProximity");
			gameObject3.transform.parent = gameObject2.transform;
			gameObject3.transform.position = doorAnim.transform.position;
			gameObject3.transform.rotation = doorAnim.transform.rotation;
			gameObject3.layer = 2;
			SphereCollider sphereCollider = Utils.AddComponent<SphereCollider>(gameObject3);
			sphereCollider.radius = highlightRadius;
			sphereCollider.isTrigger = true;
			GameObject gameObject4 = new GameObject("OnHighlight");
			gameObject4.transform.parent = gameObject2.transform;
			gameObject4.transform.position = doorAnim.transform.position;
			gameObject4.transform.rotation = doorAnim.transform.rotation;
			gameObject4.layer = 2;
			InteractiveObjectAnimation interactiveObjectAnimation = Utils.AddComponent<InteractiveObjectAnimation>(gameObject4);
			interactiveObjectAnimation.clipName = highlightAnimation.name;
		}
		OnInteractiveObjectCreated(controller);
		DockPoint[] components = Utils.GetComponents<DockPoint>(base.gameObject, Utils.SearchChildren);
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
			CspUtils.DebugLog("No inside dock point on " + base.gameObject.name);
		}
		if (outsideDockPoint == null)
		{
			CspUtils.DebugLog("No outside dock point on " + base.gameObject.name);
		}
		if (cameraOverride == null)
		{
			cameraOverride = Utils.GetComponent<CameraLite>(base.gameObject, Utils.SearchChildren);
		}
		if (doorAnim == null)
		{
			CspUtils.DebugLog("No animation component on " + base.gameObject.name);
		}
		else if (highlightAnimation != null)
		{
			doorAnim.AddClip(highlightAnimation, highlightAnimation.name);
		}
		AnimationState animationState = doorAnim[animName];
		if (animationState != null)
		{
			animationState.enabled = false;
			animationState.time = 0f;
			animationState.speed = 1f;
		}
		doorState = DoorState.Closed;
		timeTillClose = 0f;
		if (noFadeObjects.Count > 0)
		{
			Vector3 position = controller.transform.position;
			Quaternion rotation = controller.transform.rotation;
			controller.transform.parent = noFadeObjects[0].transform;
			controller.transform.position = position;
			controller.transform.rotation = rotation;
		}
	}

	public void Update()
	{
		UpdateDoor();
	}

	public virtual void OnInteractiveObjectCreated(InteractiveObject owner)
	{
	}

	public virtual void OnExitDoorClosed()
	{
		controller.highlightOnProximity = true;
		controller.highlightOnHover = true;
		controller.GotoBestState();
	}

	public override void Initialize(InteractiveObject owner, GameObject model)
	{
		base.Initialize(owner, model);
		Collider[] componentsInChildren = base.gameObject.GetComponentsInChildren<Collider>();
		Collider[] array = componentsInChildren;
		foreach (Collider collider in array)
		{
			if (!(collider is MeshCollider) && (ignoreBoxColliders || !(collider is BoxCollider)))
			{
				continue;
			}
			GameObject gameObject = new GameObject("detached_collider");
			gameObject.transform.parent = collider.gameObject.transform.parent.transform;
			gameObject.transform.position = collider.transform.position;
			gameObject.transform.rotation = collider.transform.rotation;
			gameObject.layer = 20;
			if (!useDetachedSphereCollider)
			{
				if (collider is MeshCollider)
				{
					MeshCollider meshCollider = collider as MeshCollider;
					MeshCollider meshCollider2 = gameObject.AddComponent<MeshCollider>();
					meshCollider2.sharedMesh = meshCollider.sharedMesh;
					meshCollider2.isTrigger = false;
					meshCollider2.smoothSphereCollisions = meshCollider.smoothSphereCollisions;
					meshCollider2.convex = true;
					doorColliders.Add(meshCollider2);
				}
				else
				{
					BoxCollider boxCollider = collider as BoxCollider;
					BoxCollider boxCollider2 = gameObject.AddComponent<BoxCollider>();
					boxCollider2.size = boxCollider.size;
					boxCollider2.center = boxCollider.center;
					boxCollider2.isTrigger = false;
					doorColliders.Add(boxCollider2);
				}
				Object.Destroy(collider);
			}
			else
			{
				gameObject.layer = 9;
				SphereCollider sphereCollider = gameObject.AddComponent<SphereCollider>();
				sphereCollider.isTrigger = true;
				sphereCollider.center = Vector3.zero;
				sphereCollider.radius = detachedSphereColliderRadius;
				doorColliders.Add(sphereCollider);
			}
			InteractiveObjectForwarder interactiveObjectForwarder = Utils.AddComponent<InteractiveObjectForwarder>(gameObject);
			if (!useDetachedSphereCollider)
			{
				interactiveObjectForwarder.SetOwner(owner, InteractiveObjectForwarder.Options.TiggerRolloverClick);
			}
			else
			{
				interactiveObjectForwarder.SetOwner(owner, InteractiveObjectForwarder.Options.RolloverClick);
			}
			owner.AddCollider(gameObject);
		}
	}

	public override InteractiveObject.StateIdx GetStateForPlayer(GameObject player)
	{
		return InteractiveObject.StateIdx.Enable;
	}

	public override bool ShouldIgnoreMouseClick(GameObject player)
	{
		if (doorColliders != null)
		{
			foreach (Collider doorCollider in doorColliders)
			{
				Vector3 max = player.collider.bounds.max;
				float y = max.y;
				Vector3 min = doorCollider.bounds.min;
				if (!(y < min.y - bottomOffsetTolerance))
				{
					Vector3 min2 = player.collider.bounds.min;
					float y2 = min2.y;
					Vector3 max2 = doorCollider.bounds.max;
					if (!(y2 > max2.y + topOffsetTolerance))
					{
						continue;
					}
				}
				return true;
			}
		}
		CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(player);
		if (component != null && !component.IsOnGround())
		{
			return true;
		}
		return false;
	}

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		PlayerBlob playerBlob = new PlayerBlob(this, player, onDone);
		return playerBlob.StartEnter();
	}

	public virtual void ExitWithPlayer(GameObject player, OnDone onDone, bool resetCamera)
	{
		PlayerBlob playerBlob = new PlayerBlob(this, player, onDone);
		playerBlob.StartExit(resetCamera);
	}

	protected void OpenDoor(float holdOpenOffset)
	{
		AnimationState animationState = doorAnim[animName];
		bool flag = false;
		switch (doorState)
		{
		case DoorState.Closed:
			animationState.time = 0f;
			flag = true;
			break;
		case DoorState.Closing:
			flag = true;
			break;
		}
		if (flag)
		{
			animationState.speed = 1f;
			animationState.wrapMode = WrapMode.ClampForever;
			doorAnim.Play(animName);
			doorState = DoorState.Opening;
			if (playerState == PlayerState.Entering || playOpenSoundOnExit)
			{
				ShsAudioSource.PlayAutoSound(soundOpen, outsideDockPoint.transform);
			}
		}
		timeTillClose = AnimLength + holdOpenOffset;
	}

	protected void UpdateDoor()
	{
		switch (doorState)
		{
		case DoorState.Opening:
			timeTillClose -= Time.deltaTime;
			if (timeTillClose <= 0f)
			{
				AnimationState animationState2 = doorAnim[animName];
				animationState2.time = animationState2.clip.length;
				animationState2.speed = -1f;
				animationState2.wrapMode = WrapMode.ClampForever;
				doorAnim.Play(animName);
				doorState = DoorState.Closing;
				if (playerState == PlayerState.Entering || playCloseSoundOnExit)
				{
					ShsAudioSource.PlayAutoSound(soundClose, outsideDockPoint.transform);
				}
			}
			break;
		case DoorState.Closing:
		{
			AnimationState animationState = doorAnim[animName];
			if (animationState.time <= 0f)
			{
				animationState.enabled = false;
				doorState = DoorState.Closed;
			}
			break;
		}
		}
	}

	public void PlayEnterEffects(GameObject player)
	{
		PlayEffects(player, characterEffectOnEnter, effectOnEnter);
	}

	public void PlayExitEffects(GameObject player)
	{
		PlayEffects(player, characterEffectOnExit, effectOnExit);
	}

	protected void PlayEffects(GameObject player, string characterEffect, EffectSequence sceneEffect)
	{
		if (!string.IsNullOrEmpty(characterEffect))
		{
			RemoveDuplicateEffect(player, characterEffect);
			player.GetComponent<EffectSequenceList>().TryOneShot(characterEffect);
		}
		if (sceneEffect != null)
		{
			RemoveDuplicateEffect(player, sceneEffect.name);
			EffectSequence.PlayOneShot(sceneEffect, player);
		}
	}

	protected void RemoveDuplicateEffect(GameObject player, string effectName)
	{
		if (!allowEffectsToStack)
		{
			foreach (Transform item in player.transform)
			{
				if (item.name.Contains(effectName))
				{
					Object.Destroy(item.gameObject);
					break;
				}
				string[] array = effectsToReplace;
				foreach (string value in array)
				{
					if (item.name.Contains(value))
					{
						Object.Destroy(item.gameObject);
						return;
					}
				}
			}
		}
	}
}
