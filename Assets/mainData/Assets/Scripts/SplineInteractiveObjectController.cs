using UnityEngine;

[AddComponentMenu("Interactive Object/Spline Controller")]
public class SplineInteractiveObjectController : SplineInteractiveObjectBaseController
{
	public string introEffectSequence = string.Empty;

	public bool ignoreCollision;

	public bool snapToGroundOnLand;

	public bool disableGravityInLaunch;

	private float oldGravity = -1f;

	private bool IsPetBasedInteraction(GameObject obj)
	{
		return (obj.GetComponent<CharacterMotionController>().hotSpotType & hotSpotType) == 0 && obj.GetComponent<CharacterGlobals>().activeSidekick != null && (obj.GetComponent<CharacterGlobals>().activeSidekick.motionController.hotSpotType & hotSpotType) != 0;
	}

	protected override void ApproachArrived(GameObject obj)
	{
		base.ApproachArrived(obj);
		if (IsPetBasedInteraction(obj))
		{
			obj.transform.rotation = Quaternion.identity;
		}
		if (disableGravityInLaunch)
		{
			SetGravityEnabled(obj, false);
		}
		PlayIntroEffectSequence(obj);
	}

	protected void PlayIntroEffectSequence(GameObject obj)
	{
		EffectSequence effectSequence = GetIntroEffectSequence(obj);
		if (effectSequence == null && !string.IsNullOrEmpty(introEffectSequence))
		{
			CspUtils.DebugLog("Unable to find Effect Sequence " + introEffectSequence + " on the character");
		}
		else if (effectSequence == null && hotSpotType == HotSpotType.Style.None)
		{
			EffectSequenceList component = obj.GetComponent<EffectSequenceList>();
			if (component != null)
			{
				component.TryOneShot("spline_jump_sequence");
			}
		}
		if (effectSequence != null)
		{
			BehaviorManager behaviorManager = obj.GetComponent(typeof(BehaviorManager)) as BehaviorManager;
			if (!(behaviorManager == null))
			{
				BehaviorEffectSequence behaviorEffectSequence = behaviorManager.requestChangeBehavior(typeof(BehaviorEffectSequence), true) as BehaviorEffectSequence;
				if (behaviorEffectSequence != null)
				{
					behaviorEffectSequence.Initialize(effectSequence, EffectSequenceDone);
				}
				else
				{
					Object.Destroy(effectSequence);
				}
			}
		}
		else
		{
			EffectSequenceDone(obj);
		}
	}

	protected EffectSequence GetIntroEffectSequence(GameObject obj)
	{
		if (!string.IsNullOrEmpty(introEffectSequence))
		{
			EffectSequenceList effectSequenceList = obj.GetComponent(typeof(EffectSequenceList)) as EffectSequenceList;
			EffectSequence sequence;
			if (effectSequenceList != null && ((IsPetBasedInteraction(obj) && effectSequenceList.TryGetEffectSequenceByName("pet flight sequence", out sequence)) || effectSequenceList.TryGetLogicalEffectSequence(introEffectSequence, out sequence) || effectSequenceList.TryGetEffectSequenceByName(introEffectSequence, out sequence)))
			{
				return sequence;
			}
		}
		return null;
	}

	protected void EffectSequenceDone(GameObject obj)
	{
		if (disableGravityInLaunch)
		{
			SetGravityEnabled(obj, true);
		}
		BehaviorManager behaviorManager = obj.GetComponent(typeof(BehaviorManager)) as BehaviorManager;
		if (behaviorManager == null)
		{
			return;
		}
		BehaviorSpline behaviorSpline = behaviorManager.requestChangeBehavior(typeof(BehaviorSpline), true) as BehaviorSpline;
		if (behaviorSpline != null)
		{
			behaviorSpline.Initialize(spline, !IsPetBasedInteraction(obj), SplineDone, ignoreCollision, false);
			behaviorSpline.SnapToGroundOnLand(snapToGroundOnLand);
			if (hotSpotType == HotSpotType.Style.Flying && AppShell.Instance != null && AppShell.Instance.EventMgr != null)
			{
				AppShell.Instance.EventMgr.Fire(obj, new EntityTakeoffMessage(obj, spline));
			}
		}
	}

	protected void SetGravityEnabled(GameObject player, bool enabled)
	{
		CharacterMotionController component = player.GetComponent<CharacterMotionController>();
		if (enabled && oldGravity != -1f)
		{
			component.gravity = oldGravity;
		}
		else if (!enabled)
		{
			oldGravity = component.gravity;
			component.gravity = 0f;
		}
	}
}
