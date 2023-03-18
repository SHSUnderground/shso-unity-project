using UnityEngine;

public class SleighRideController : SplineInteractiveObjectController
{
	protected class Use
	{
		private GameObject player;

		private SleighRideController owner;

		public Use(GameObject player, SleighRideController owner)
		{
			this.player = player;
			this.owner = owner;
		}

		public bool ApproachStartPoint(OnDone onDone)
		{
			BehaviorManager component = player.GetComponent<BehaviorManager>();
			if (component == null)
			{
				return false;
			}
			BehaviorApproach behaviorApproach = component.requestChangeBehavior<BehaviorApproach>(false);
			if (behaviorApproach == null)
			{
				return false;
			}
			behaviorApproach.Initialize(owner.waitPoint.transform.position, owner.waitPoint.transform.rotation, owner.allowApproachCancel, StartPointArrived, owner.ApproachCancel, 0.15f, 2f, true, false);
			if (onDone != null)
			{
				onDone(player, CompletionStateEnum.Success);
			}
			return true;
		}

		protected void StartPointArrived(GameObject obj)
		{
			CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(player);
			if (component != null)
			{
				component.DisableNetUpdates(true);
			}
			BehaviorManager component2 = obj.GetComponent<BehaviorManager>();
			component2.requestChangeBehavior<BehaviorWait>(false);
			EffectSequence sequence = owner.sleighArrivalSequence.GetSequence(obj);
			if (!(sequence != null))
			{
				return;
			}
			GameObject gameObject = Object.Instantiate(sequence.gameObject) as GameObject;
			EffectSequence component3 = gameObject.GetComponent<EffectSequence>();
			if (component3 == null)
			{
				CspUtils.DebugLog("No EffectSequence attached to <" + sequence.gameObject.name + ">");
				return;
			}
			if (owner.sleighEntryOwner == null)
			{
				owner.sleighEntryOwner = new GameObject("sleigh_entry_owner");
				owner.sleighEntryOwner.transform.parent = owner.transform;
				Vector3 pos;
				Quaternion rot;
				owner.spline.GetFirstPoint(out pos, out rot);
				owner.sleighEntryOwner.transform.position = pos;
				owner.sleighEntryOwner.transform.rotation = rot;
			}
			component3.Initialize(owner.sleighEntryOwner, OnArrivalSequenceComplete, null);
			if (!component3.AutoStart)
			{
				component3.StartSequence();
			}
		}

		protected void OnArrivalSequenceComplete(EffectSequence seq)
		{
			Object.Destroy(seq.gameObject);
			owner.ApproachArrived(player);
		}
	}

	public DockPoint waitPoint;

	public EffectSequenceReference sleighArrivalSequence;

	protected GameObject sleighEntryOwner;

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		if (waitPoint == null || sleighArrivalSequence == null)
		{
			return false;
		}
		PlayVO(player);
		return new Use(player, this).ApproachStartPoint(onDone);
	}
}
