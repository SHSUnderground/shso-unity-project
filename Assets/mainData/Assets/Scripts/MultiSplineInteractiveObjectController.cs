using UnityEngine;

[AddComponentMenu("Interactive Object/Multi Spline Controller")]
public class MultiSplineInteractiveObjectController : SplineInteractiveObjectController
{
	public MultiHotspotCoordinator hotspotCoordinator;

	public GameObject ActivateObject;

	protected GameObject user;

	public void Start()
	{
		user = null;
		AppShell.Instance.EventMgr.AddListener<EntityDespawnMessage>(OnCharacterDespawned);
		if (ActivateObject != null)
		{
			Utils.ActivateTree(ActivateObject, false);
		}
	}

	public void OnDisable()
	{
		user = null;
		AppShell.Instance.EventMgr.RemoveListener<EntityDespawnMessage>(OnCharacterDespawned);
	}

	protected override void ApproachArrived(GameObject player)
	{
		if (ActivateObject != null)
		{
			Utils.ActivateTree(ActivateObject, true);
		}
		BehaviorManager component = Utils.GetComponent<BehaviorManager>(player);
		if (component != null)
		{
			BehaviorHotspotWait behaviorHotspotWait = component.requestChangeBehavior<BehaviorHotspotWait>(false);
			if (behaviorHotspotWait != null)
			{
				behaviorHotspotWait.Initialize(hotspotCoordinator, GetWindupDuration(player), OnWaitFinished, OnWaitCancelled);
			}
			else
			{
				OnWaitCancelled(player);
			}
		}
		else
		{
			OnWaitCancelled(player);
		}
	}

	protected float GetWindupDuration(GameObject player)
	{
		EffectSequence introEffectSequence = GetIntroEffectSequence(player);
		if (introEffectSequence != null)
		{
			float eventOffset = introEffectSequence.GetEventOffset("behavior end");
			Object.Destroy(introEffectSequence.gameObject);
			if (eventOffset > 0f)
			{
				return eventOffset;
			}
		}
		return 0f;
	}

	protected void OnWaitFinished(GameObject player)
	{
		CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(player);
		if (component != null)
		{
			component.DisableNetUpdates(true);
		}
		if (ActivateObject != null)
		{
			Utils.ActivateTree(ActivateObject, false);
		}
		Launch(player);
		PlayIntroEffectSequence(player);
	}

	protected void OnWaitCancelled(GameObject player)
	{
		MultiHotspotCoordinatorUser.Detach(player);
		user = null;
		if (ActivateObject != null)
		{
			Utils.ActivateTree(ActivateObject, false);
		}
		CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(player);
		if (component != null)
		{
			component.DisableNetUpdates(false);
		}
	}

	protected override void SplineDone(GameObject obj)
	{
		user = null;
		MultiHotspotCoordinatorUser.Detach(obj);
		base.SplineDone(obj);
	}

	protected void OnCharacterDespawned(EntityDespawnMessage e)
	{
		if (e.go == user)
		{
			MultiHotspotCoordinatorUser.Detach(user);
			user = null;
		}
	}

	public override bool CanPlayerUse(GameObject player)
	{
		if (user != null)
		{
			return false;
		}
		if (hotspotCoordinator != null && !hotspotCoordinator.CanJoin)
		{
			return false;
		}
		BehaviorManager component = Utils.GetComponent<BehaviorManager>(player);
		if (component != null && component.getBehavior() is BehaviorHotspotWait)
		{
			return false;
		}
		return base.CanPlayerUse(player);
	}

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		allowApproachCancel = false;
		if (base.StartWithPlayer(player, onDone))
		{
			user = player;
		}
		return user != null;
	}
}
