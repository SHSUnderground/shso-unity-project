using UnityEngine;

public class FoodCreationDevice : HqFixedItem
{
	public FoodSequence[] FoodSequences;

	public float foodValue = 45f;

	private EffectSequence currentSequence;

	public override bool Paused
	{
		get
		{
			return base.Paused;
		}
		set
		{
			base.Paused = value;
			if (currentSequence != null)
			{
				currentSequence.Paused = value;
			}
		}
	}

	public override float HungerValue
	{
		get
		{
			return foodValue;
		}
	}

	public override bool Use(AIControllerHQ ai, DockPoint dockPoint, BehaviorBase.OnBehaviorDone doneCallback)
	{
		BehaviorUseFoodDevice behaviorUseFoodDevice = ai.ChangeBehavior<BehaviorUseFoodDevice>(false);
		if (behaviorUseFoodDevice != null)
		{
			behaviorUseFoodDevice.Initialize(this, dockPoint, doneCallback);
			return true;
		}
		return false;
	}

	public void PlayFoodEffectSequence(FoodSequence sequence)
	{
		if (sequence.effectSequencePrefab != null)
		{
			currentSequence = (Object.Instantiate(sequence.effectSequencePrefab.gameObject) as GameObject).GetComponent<EffectSequence>();
			currentSequence.Initialize(base.gameObject, DeleteSequenceOnDone, null);
			currentSequence.StartSequence();
		}
	}

	private void DeleteSequenceOnDone(EffectSequence seq)
	{
		if (seq == currentSequence)
		{
			Object.Destroy(currentSequence);
			currentSequence = null;
		}
	}
}
