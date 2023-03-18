using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCBotControlPanel : GlowableInteractiveController
{
	public class Use : BaseUse
	{
		private RCBotControlPanel owner;

		private bool pokeTimeoutActive;

		private bool delayTimeoutActive;

		private bool approachTimeoutActive;

		public Use(GameObject player, RCBotControlPanel owner, OnDone onDone)
			: base(player, owner, onDone)
		{
			this.owner = owner;
		}

		public override bool Start()
		{
			if (base.Start())
			{
				Approach(owner.startLocation.transform);
				return true;
			}
			return false;
		}

		protected override void OnApproachArrived(GameObject arrivedPlayer)
		{
			if (base.Player == null)
			{
				Done();
				return;
			}
			owner.StartCoroutine(PokeTimeout());
			BehaviorEmote behaviorEmote = ChangeBehavior<BehaviorEmote>(false);
			if (behaviorEmote != null)
			{
				behaviorEmote.Initialize(EmotesDefinition.Instance.GetEmoteByCommand("poke").id, true, false, OnPokeFinished);
				owner.StartCoroutine(OnPoke());
			}
		}

		private IEnumerator OnPoke()
		{
			yield return new WaitForSeconds(0.6f);
			if (owner.sequenceOnPoke != null)
			{
				GameObject sequenceParent = (!(owner.sequenceOnPokeParent != null)) ? owner.gameObject : owner.sequenceOnPokeParent;
				EffectSequence.PlayOneShot(owner.sequenceOnPoke, sequenceParent);
			}
		}

		private void OnPokeFinished(GameObject delayedPlayer)
		{
			pokeTimeoutActive = false;
			if (owner.activatedSequence != null)
			{
				GameObject parent = (!(owner.activatedSequenceParent != null)) ? owner.gameObject : owner.activatedSequenceParent;
				EffectSequence.PlayOneShot(owner.activatedSequence, parent);
			}
			owner.StartCoroutine(DelayTimeout());
			BehaviorDelay behaviorDelay = ChangeBehavior<BehaviorDelay>(false);
			if (behaviorDelay == null)
			{
				CspUtils.DebugLog("Could not switch to delay behavior.");
				Done();
			}
			else
			{
				behaviorDelay.Initialize(owner.delay, base.Player.transform.forward, OnDelayFinished);
			}
		}

		private void OnDelayFinished()
		{
			delayTimeoutActive = false;
			base.Player.behaviorManager.endBehavior();
			owner.StartCoroutine(ApproachTimeout());
			IEnumerable<Transform> approachWaypoints = owner.approachWaypoints;
			BehaviorApproach.SetupApproachChain(base.Player, false, OnArrivedAtDoor, OnDoorApproachCancelled, approachWaypoints.GetEnumerator());
		}

		private void OnArrivedAtDoor(GameObject arrivedPlayer)
		{
			approachTimeoutActive = false;
			base.Player.motionController.DisableNetUpdates(false);
			owner.rcBotDoor.forcedApproach = true;
			owner.rcBotDoor.StartWithPlayer(base.Player.gameObject, null);
			Done();
		}

		private void OnDoorApproachCancelled(GameObject cancelledPlayer)
		{
			Done();
		}

		private IEnumerator PokeTimeout()
		{
			pokeTimeoutActive = true;
			yield return new WaitForSeconds(5f);
			if (pokeTimeoutActive)
			{
				Done();
			}
		}

		private IEnumerator DelayTimeout()
		{
			delayTimeoutActive = true;
			yield return new WaitForSeconds(owner.delay + 3f);
			if (delayTimeoutActive)
			{
				Done();
			}
		}

		private IEnumerator ApproachTimeout()
		{
			approachTimeoutActive = true;
			yield return new WaitForSeconds(5f);
			if (approachTimeoutActive)
			{
				Done();
			}
		}
	}

	public DoorManager rcBotDoor;

	public DockPoint startLocation;

	public Transform[] approachWaypoints;

	public float delay;

	public EffectSequence activatedSequence;

	public GameObject activatedSequenceParent;

	public EffectSequence sequenceOnPoke;

	public GameObject sequenceOnPokeParent;

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		return new Use(player, this, onDone).Start();
	}
}
