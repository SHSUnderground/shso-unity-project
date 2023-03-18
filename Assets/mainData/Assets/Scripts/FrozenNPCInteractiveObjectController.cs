using System.Collections;
using UnityEngine;

public class FrozenNPCInteractiveObjectController : EffectSequenceController
{
	public class Use : BaseUse
	{
		private FrozenNPCInteractiveObjectController owner;

		public Use(GameObject player, GlowableInteractiveController owner, OnDone onDone)
			: base(player, owner, onDone)
		{
			this.owner = (owner as FrozenNPCInteractiveObjectController);
		}

		public override bool Start()
		{
			if (base.Start())
			{
				Approach(owner.approachTarget ?? owner.transform);
				return true;
			}
			return false;
		}

		protected override void OnApproachArrived(GameObject player)
		{
			BehaviorEmote behaviorEmote = base.Player.behaviorManager.requestChangeBehavior<BehaviorEmote>(false);
			if (behaviorEmote != null && behaviorEmote.Initialize(EmotesDefinition.Instance.GetEmoteByCommand("poke").id, true, false))
			{
				owner.StartCoroutine(WaitForPoke());
			}
			else
			{
				OnPoked();
			}
		}

		protected IEnumerator WaitForPoke()
		{
			yield return new WaitForSeconds(0.8f);
			OnPoked();
		}

		protected void OnPoked()
		{
			DropLoot();
			PlayClickSequence();
			if (owner.countTowardsCheerChallenge && base.IsLocal)
			{
				AppShell.Instance.delayedAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "generic_event", "npc_impress", OwnableDefinition.simpleZoneName(SocialSpaceControllerImpl.getZoneName()), 3f);
			}
		}

		protected void DropLoot()
		{
			if (base.IsLocal)
			{
				ActivitySpawnPoint componentInChildren = owner.GetComponentInChildren<ActivitySpawnPoint>();
				if (componentInChildren != null)
				{
					componentInChildren.SpawnActivityObject();
				}
			}
		}

		protected void PlayClickSequence()
		{
			owner.SequencesEnabled = false;
			owner.GetActiveCollection().Stop();
			EffectSequence effectSequence = InstantiateClickSequence();
			effectSequence.Initialize(owner.SequenceOwner, OnClickSequenceFinished, null);
			if (!effectSequence.AutoStart)
			{
				effectSequence.StartSequence();
			}
		}

		protected EffectSequence InstantiateClickSequence()
		{
			EffectSequence sequence = owner.clickSequence.GetSequence(owner.SequenceOwner);
			GameObject gameObject = Object.Instantiate(sequence.gameObject) as GameObject;
			return gameObject.GetComponent<EffectSequence>();
		}

		protected void OnClickSequenceFinished(EffectSequence seq)
		{
			Object.Destroy(seq.gameObject);
			owner.SequencesEnabled = true;
			owner.ForceSetActiveCollection(owner.defaultSequences);
			Done();
		}
	}

	public Transform approachTarget;

	public EffectSequenceReference clickSequence;

	public bool countTowardsCheerChallenge = true;

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		Use use = new Use(player, this, onDone);
		return use.Start();
	}
}
