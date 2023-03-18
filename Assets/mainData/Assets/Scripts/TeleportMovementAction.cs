using System.Collections.Generic;
using UnityEngine;

public class TeleportMovementAction : HotspotAction
{
	protected class ActionInstance
	{
		private CharacterGlobals player;

		private OnFinished onFinished;

		private IEnumerator<TeleportMovementActionNode> nodeEnumerator;

		private EffectSequenceReference initialEffect;

		public ActionInstance(CharacterGlobals player, OnFinished onFinished, IEnumerable<TeleportMovementActionNode> nodes, EffectSequenceReference initialEffect)
		{
			this.player = player;
			this.onFinished = onFinished;
			nodeEnumerator = nodes.GetEnumerator();
			this.initialEffect = initialEffect;
		}

		public void Go()
		{
			PreAction();
			PlayInitialSequence();
		}

		public void UseNextNode()
		{
			if (nodeEnumerator != null && player != null && nodeEnumerator.MoveNext())
			{
				nodeEnumerator.Current.Use(player, UseNextNode);
				return;
			}
			PostAction();
			if (onFinished != null)
			{
				onFinished();
			}
		}

		protected void PreAction()
		{
			player.behaviorManager.requestChangeBehavior<BehaviorWait>(false);
			player.motionController.DisableNetUpdates(true);
		}

		protected void PostAction()
		{
			player.behaviorManager.endBehavior();
			player.motionController.DisableNetUpdates(false);
		}

		protected void PlayInitialSequence()
		{
			EffectSequence sequence = initialEffect.GetSequence(player.gameObject);
			if (sequence != null)
			{
				EffectSequence.PlayOneShot(sequence.gameObject, player.gameObject, OnInitialSequenceDone);
			}
			else
			{
				UseNextNode();
			}
		}

		protected void OnInitialSequenceDone(EffectSequence sequence)
		{
			Object.Destroy(sequence.gameObject);
			UseNextNode();
		}
	}

	public EffectSequenceReference initialEffect;

	public TeleportMovementActionNode[] nodes;

	public override void PerformAction(CharacterGlobals player, OnFinished onFinished)
	{
		ActionInstance actionInstance = new ActionInstance(player, onFinished, nodes, initialEffect);
		actionInstance.Go();
	}
}
