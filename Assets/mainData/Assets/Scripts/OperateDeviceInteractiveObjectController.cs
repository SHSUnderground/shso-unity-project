using System.Collections;
using UnityEngine;

public class OperateDeviceInteractiveObjectController : InteractiveObjectController
{
	public bool requiresApproach = true;

	public string playAnimation = "emote_manipulate_loop";

	public GameObject playSound;

	public float soundDelay;

	public bool stopSoundOnComplete = true;

	public float operateTime = 2f;

	public string TriggerScenarioEvent = string.Empty;

	public GameObject useTarget;

	public float approachDistance = 0.4f;

	public bool useObjectRotation = true;

	public bool singleOperatorOnly;

	protected ShsAudioSource soundInstance;

	private float lastUsedTime;

	protected bool InUse
	{
		get
		{
			return Time.time <= lastUsedTime + operateTime;
		}
		set
		{
			if (value)
			{
				lastUsedTime = Time.time;
			}
			else
			{
				lastUsedTime = -1f * operateTime;
			}
		}
	}

	public override InteractiveObject.StateIdx GetStateForPlayer(GameObject player)
	{
		if (player == null)
		{
			return InteractiveObject.StateIdx.Hidden;
		}
		return InteractiveObject.StateIdx.Enable;
	}

	public override bool CanPlayerUse(GameObject player)
	{
		return true;
	}

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		BehaviorManager behaviorManager = player.GetComponent(typeof(BehaviorManager)) as BehaviorManager;
		if (behaviorManager == null)
		{
			CspUtils.DebugLog("Player should always have a BehaviorManager");
			return false;
		}
		if (requiresApproach)
		{
			BehaviorApproach behaviorApproach = behaviorManager.requestChangeBehavior(typeof(BehaviorApproach), false) as BehaviorApproach;
			if (behaviorApproach == null)
			{
				return false;
			}
			CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(player);
			if (component != null)
			{
				component.DisableNetUpdates(true);
			}
			Vector3 newTargetPosition = base.transform.position;
			Quaternion rotation = base.transform.rotation;
			if (useTarget != null)
			{
				newTargetPosition = useTarget.transform.position;
				rotation = useTarget.transform.rotation;
			}
			else if (!useObjectRotation)
			{
				newTargetPosition = base.transform.position + (player.transform.position - base.transform.position).normalized * approachDistance;
				rotation = Quaternion.LookRotation(base.transform.position - player.transform.position);
			}
			behaviorApproach.Initialize(newTargetPosition, rotation, false, ApproachArrived, ApproachCancel, 0.4f, 2f, true, false);
		}
		else
		{
			ApproachArrived(player);
		}
		if (onDone != null)
		{
			onDone(player, CompletionStateEnum.Success);
		}
		return true;
	}

	protected virtual void ApproachArrived(GameObject obj)
	{
		CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(obj);
		if (component != null)
		{
			component.DisableNetUpdates(false);
		}
		if (singleOperatorOnly && InUse)
		{
			return;
		}
		BehaviorManager behaviorManager = obj.GetComponent(typeof(BehaviorManager)) as BehaviorManager;
		if (!(behaviorManager == null))
		{
			BehaviorLoopSequence behaviorLoopSequence = behaviorManager.requestChangeBehavior(typeof(BehaviorLoopSequence), true) as BehaviorLoopSequence;
			if (behaviorLoopSequence != null)
			{
				behaviorLoopSequence.Initialize(playAnimation, AnimationComplete, AnimationCanceled, operateTime);
				StartCoroutine("StartSound");
				InUse = true;
			}
		}
	}

	protected void ApproachCancel(GameObject obj)
	{
		CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(obj);
		if (component != null)
		{
			component.DisableNetUpdates(false);
		}
	}

	protected virtual void AnimationComplete(GameObject obj)
	{
		NetworkComponent networkComponent = obj.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
		if (networkComponent != null && networkComponent.IsOwner())
		{
			ScenarioEventManager.Instance.FireScenarioEvent(TriggerScenarioEvent, false);
		}
		if (stopSoundOnComplete)
		{
			StopSoundInstance();
		}
		InUse = false;
	}

	protected virtual void AnimationCanceled(GameObject obj)
	{
		StopSoundInstance();
	}

	protected IEnumerator StartSound()
	{
		yield return new WaitForSeconds(soundDelay);
		if (playSound != null)
		{
			soundInstance = ShsAudioSource.PlayAutoSound(playSound, base.transform);
		}
	}

	protected void StopSoundInstance()
	{
		StopCoroutine("StartSound");
		if (soundInstance != null)
		{
			Object.Destroy(soundInstance);
		}
	}
}
