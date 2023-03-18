using System.Collections;
using UnityEngine;

public class TeleportDoorManager : DoorManager
{
	public class TeleportPlayerBlob : PlayerBlob
	{
		public TeleportPlayerBlob(DoorManager owner, GameObject player, OnDone onDone)
			: base(owner, player, onDone)
		{
		}

		public override void StartExit(bool hasChangedCameras)
		{
			base.StartExit(hasChangedCameras);
			if (!(player != null))
			{
				return;
			}
			BehaviorManager component = player.GetComponent<BehaviorManager>();
			if (component != null)
			{
				BehaviorTeleport behaviorTeleport = component.getBehavior() as BehaviorTeleport;
				if (behaviorTeleport != null)
				{
					component.endBehavior();
				}
			}
			TeleportDoorManager teleportDoorManager = owner as TeleportDoorManager;
			if (teleportDoorManager != null && teleportDoorManager.teleportEffectPrefab != null)
			{
				Object.Instantiate(teleportDoorManager.teleportEffectPrefab, player.transform.position, player.transform.rotation);
			}
		}

		protected override IEnumerator OnDoorEntered()
		{
			if (player != null)
			{
				TeleportDoorManager teleportOwner = owner as TeleportDoorManager;
				if (teleportOwner != null && !string.IsNullOrEmpty(teleportOwner.eventOnApproach))
				{
					ScenarioEventManager.Instance.FireScenarioEvent(teleportOwner.eventOnApproach, false);
				}
				yield return new WaitForSeconds(owner.AnimLength);
				if (player == null)
				{
					yield break;
				}
				owner.PlayEnterEffects(player);
				if (!(teleportOwner != null))
				{
					yield break;
				}
				if (teleportOwner.teleportEffectPrefab != null)
				{
					Object.Instantiate(teleportOwner.teleportEffectPrefab, player.transform.position, player.transform.rotation);
				}
				BehaviorManager behaviorManager = player.GetComponent<BehaviorManager>();
				if (behaviorManager != null)
				{
					BehaviorTeleport hot = behaviorManager.requestChangeBehavior(typeof(BehaviorTeleport), true) as BehaviorTeleport;
					if (hot != null)
					{
						DoorManager exit2 = null;
						exit2 = ((!(owner.exit != null)) ? owner : owner.exit);
						hot.Initialize(TeleportDone, teleportOwner.teleportPurgatory, teleportOwner.teleportStartTime, false, null, exit2.InsidePoint, teleportOwner.teleportTime, false);
					}
				}
			}
			else
			{
				onDone(player, CompletionStateEnum.Unknown);
			}
		}

		protected override void PlayEnterVO(GameObject player)
		{
		}

		public void TeleportDone(GameObject obj)
		{
			TurnFadingOn();
			DoorManager doorManager = null;
			doorManager = ((!(owner.exit != null)) ? owner : owner.exit);
			TeleportDoorManager teleportDoorManager = doorManager as TeleportDoorManager;
			if (teleportDoorManager != null)
			{
				teleportDoorManager.ExitWithPlayer(player, onDone, haveChangedCameras);
			}
			else
			{
				doorManager.ExitWithPlayer(player, onDone, haveChangedCameras);
			}
			player = null;
			onDone = null;
			owner = null;
		}
	}

	public bool enableTeleporter = true;

	public string enableTeleporterEvent = string.Empty;

	public string disableTeleporterEvent = string.Empty;

	public float teleportStartTime;

	public GameObject teleportEffectPrefab;

	public GameObject teleportPurgatory;

	public float interactiveMaxAngle = 180f;

	public float interactiveMaxRange = 3.5f;

	public float teleportTime;

	public string eventOnApproach = string.Empty;

	private bool mSubscribed;

	public override void OnEnable()
	{
		base.OnEnable();
		ToggleEventSubscription(true);
	}

	public void OnDisable()
	{
		ToggleEventSubscription(false);
	}

	public override void Initialize(InteractiveObject owner, GameObject model)
	{
		base.Initialize(owner, model);
		UpdateDetachedColliderStatus();
		ToggleEventSubscription(true);
	}

	public override InteractiveObject.StateIdx GetStateForPlayer(GameObject player)
	{
		return (!enableTeleporter) ? InteractiveObject.StateIdx.Disable : InteractiveObject.StateIdx.Enable;
	}

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		TeleportPlayerBlob teleportPlayerBlob = new TeleportPlayerBlob(this, player, onDone);
		return teleportPlayerBlob.StartEnter();
	}

	public new void ExitWithPlayer(GameObject player, OnDone onDone, bool resetCamera)
	{
		TeleportPlayerBlob teleportPlayerBlob = new TeleportPlayerBlob(this, player, onDone);
		teleportPlayerBlob.StartExit(resetCamera);
	}

	public override void OnInteractiveObjectCreated(InteractiveObject owner)
	{
		owner.highlightOnProximity = false;
		owner.highlightOnHover = false;
		owner.hoverOnlyInProximity = false;
		owner.highlightOnHoverAtAnyDistance = true;
		owner.maxAngle = interactiveMaxAngle;
		owner.maxInteractRange = interactiveMaxRange;
		Transform transform = Utils.FindNodeInChildren(base.transform, "OnEnable");
		if (transform != null)
		{
			Utils.DetachGameObject(transform.gameObject);
			Utils.AttachGameObject(owner.gameObject, transform.gameObject);
		}
	}

	public override void OnExitDoorClosed()
	{
		controller.highlightOnProximity = false;
		controller.highlightOnHover = false;
		controller.GotoBestState();
	}

	public void ToggleEventSubscription(bool subscribe)
	{
		if (ScenarioEventManager.Instance == null || mSubscribed == subscribe)
		{
			return;
		}
		if (subscribe)
		{
			if (enableTeleporterEvent != string.Empty)
			{
				ScenarioEventManager.Instance.SubscribeScenarioEvent(enableTeleporterEvent, EnableTeleporter);
			}
			if (disableTeleporterEvent != string.Empty)
			{
				ScenarioEventManager.Instance.SubscribeScenarioEvent(disableTeleporterEvent, DisableTeleporter);
			}
		}
		else
		{
			if (enableTeleporterEvent != string.Empty)
			{
				ScenarioEventManager.Instance.UnsubscribeScenarioEvent(enableTeleporterEvent, EnableTeleporter);
			}
			if (disableTeleporterEvent != string.Empty)
			{
				ScenarioEventManager.Instance.UnsubscribeScenarioEvent(disableTeleporterEvent, DisableTeleporter);
			}
		}
		mSubscribed = subscribe;
	}

	public void EnableTeleporter(string eventName)
	{
		enableTeleporter = true;
		UpdateDetachedColliderStatus();
		owner.GotoBestState();
	}

	public void DisableTeleporter(string eventName)
	{
		enableTeleporter = false;
		UpdateDetachedColliderStatus();
		owner.GotoBestState();
	}

	public void UpdateDetachedColliderStatus()
	{
		if (useDetachedSphereCollider)
		{
			Transform transform = Utils.FindNodeInChildren(base.transform, "detached_collider");
			if (transform != null)
			{
				transform.gameObject.active = enableTeleporter;
			}
		}
	}
}
