using System;
using UnityEngine;

public class TroubleBotActivityObject : ActivityObject
{
	public EffectSequence smashedPrefab;

	public Vector3 attachOffset = new Vector3(0f, -0.4f, 0f);

	public bool triggered;

	private GameObject usingPlayer;

	private float pickupDistance = 1.354f;

	private float destroyFailSafeTime = 0.9f;

	private bool destroyFailSafeEnabled;

	private bool destroyed;

	private BehaviorManager usingBehaviorManager;

	public bool Destroyed
	{
		get
		{
			return destroyed;
		}
	}

	private void cancel()
	{
		triggered = false;
	}

	public override void ActionTriggered(ActivityObjectActionNameEnum action)
	{
		if (triggered && action != ActivityObjectActionNameEnum.Collision)
		{
			return;
		}
		triggered = true;
		switch (action)
		{
		case ActivityObjectActionNameEnum.PowerEmote:
		{
			TroubleBotLaserNode.DestroyLaser(base.gameObject);
			DisableGlow();
			ShsAudioSource[] components = Utils.GetComponents<ShsAudioSource>(base.gameObject, Utils.SearchChildren, true);
			foreach (ShsAudioSource obj in components)
			{
				UnityEngine.Object.Destroy(obj);
			}
			if (usingPlayer != null)
			{
				TroubleBotSticky.RemoveFrom(usingPlayer);
			}
			base.ActionTriggered(ActivityObjectActionNameEnum.Click);
			return;
		}
		case ActivityObjectActionNameEnum.Collision:
			destroyed = true;
			if (usingPlayer != null)
			{
				TroubleBotSticky.RemoveFrom(usingPlayer);
			}
			base.ActionTriggered(ActivityObjectActionNameEnum.Click);
			return;
		}
		usingPlayer = GameController.GetController().LocalPlayer;
		if (usingPlayer == null)
		{
			CspUtils.DebugLog("Canceling TroubleBotActivityObject.");
			cancel();
			return;
		}
		usingBehaviorManager = Utils.GetComponent<BehaviorManager>(usingPlayer);
		if (usingBehaviorManager == null)
		{
			CspUtils.DebugLog("Player should always have a BehaviorManager");
			return;
		}
		Quaternion rotation = Quaternion.FromToRotation(usingPlayer.gameObject.transform.rotation.eulerAngles.normalized, (base.gameObject.transform.position - usingPlayer.gameObject.transform.position).normalized);
		Vector3 vector = base.gameObject.transform.position - usingPlayer.transform.position;
		vector = ((!(vector.magnitude < pickupDistance)) ? (base.gameObject.transform.position - vector.normalized * pickupDistance) : usingPlayer.transform.position);
		BehaviorApproach behaviorApproach = usingBehaviorManager.requestChangeBehavior(typeof(BehaviorApproach), true) as BehaviorApproach;
		if (behaviorApproach != null)
		{
			behaviorApproach.Initialize(vector, rotation, true, ApproachArrived, ApproachCancelled, 0.1f, 1f, true, true);
			behaviorApproach.setTarget(base.gameObject);
		}
	}

	private void ApproachCancelled(GameObject player)
	{
		triggered = false;
	}

	protected virtual void ApproachArrived(GameObject player)
	{
		BehaviorTossTroubleBot behaviorTossTroubleBot = usingBehaviorManager.requestChangeBehavior(typeof(BehaviorTossTroubleBot), false) as BehaviorTossTroubleBot;
		if (behaviorTossTroubleBot != null)
		{
			TroubleBotSticky.AttachTo(usingPlayer);
			DisableGlow();
			behaviorTossTroubleBot.Initialize(base.gameObject, OnDoneThrowing);
		}
	}

	protected void OnDoneThrowing(GameObject objectThrown)
	{
		destroyFailSafeEnabled = true;
		usingBehaviorManager.requestChangeBehavior(Type.GetType(usingBehaviorManager.defaultBehaviorType), false);
	}

	public virtual void Update()
	{
		if (destroyed || !destroyFailSafeEnabled)
		{
			return;
		}
		destroyFailSafeTime -= Time.deltaTime;
		if (destroyFailSafeTime < 0f)
		{
			CspUtils.DebugLog("TroubleBotActivityObject destroyFailSafeTime");
			if (usingPlayer != null)
			{
				TroubleBotSticky.RemoveFrom(usingPlayer);
			}
			base.ActionTriggered(ActivityObjectActionNameEnum.Click);
			destroyed = true;
			destroyFailSafeEnabled = false;
		}
	}

	protected void DisableGlow()
	{
		InteractiveObject component = base.gameObject.GetComponent<InteractiveObject>();
		if (component != null)
		{
			component.highlightOnProximity = false;
			component.highlightOnHover = false;
			component.GotoBestState();
		}
	}
}
