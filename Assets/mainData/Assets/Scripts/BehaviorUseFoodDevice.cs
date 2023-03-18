using System;
using UnityEngine;

public class BehaviorUseFoodDevice : BehaviorBase
{
	protected enum State
	{
		Initial,
		TurningOn,
		PlayFoodSequence,
		PlayingFoodSequence,
		SpawnFood,
		EatItem,
		EatingItem,
		Done
	}

	private const string FOODSPAWNER = "food_spawner";

	private FoodCreationDevice foodDevice;

	private FoodSequence foodSequence;

	private EffectSequenceList list;

	private GameObject createdFood;

	protected State currentState;

	protected OnBehaviorDone ItemUsed;

	protected bool playingFoodAnimation;

	protected EffectSequence currentSequence;

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

	public void Initialize(HqItem itemToUse, DockPoint dockPoint, OnBehaviorDone usedItemCallback)
	{
		foodDevice = Utils.GetComponent<FoodCreationDevice>(itemToUse);
		int max = foodDevice.FoodSequences.Length;
		int num = UnityEngine.Random.Range(0, max);
		foodSequence = foodDevice.FoodSequences[num];
		currentState = State.Initial;
		list = (owningObject.GetComponent(typeof(EffectSequenceList)) as EffectSequenceList);
		playingFoodAnimation = true;
		ItemUsed = usedItemCallback;
	}

	public override void behaviorBegin()
	{
		base.behaviorBegin();
	}

	public override void behaviorUpdate()
	{
		if (Paused)
		{
			return;
		}
		base.behaviorUpdate();
		switch (currentState)
		{
		case State.TurningOn:
			break;
		case State.EatingItem:
			break;
		case State.Initial:
		{
			GameObject gameObject3 = list.GetEffectSequencePrefabByName("emote_poke_sequence") as GameObject;
			if (gameObject3 != null)
			{
				GameObject gameObject4 = UnityEngine.Object.Instantiate(gameObject3) as GameObject;
				currentSequence = (gameObject4.GetComponent(typeof(EffectSequence)) as EffectSequence);
				currentSequence.Initialize(charGlobals.gameObject, null, OnEffectEvent);
				currentSequence.StartSequence();
			}
			currentState = State.TurningOn;
			break;
		}
		case State.PlayFoodSequence:
			currentSequence = null;
			foodDevice.gameObject.animation.Play(foodSequence.Animation);
			foodDevice.PlayFoodEffectSequence(foodSequence);
			currentState = State.PlayingFoodSequence;
			break;
		case State.PlayingFoodSequence:
			if (HasAnimationCompleted(foodSequence.Animation))
			{
				currentState = State.SpawnFood;
			}
			break;
		case State.SpawnFood:
			createdFood = (UnityEngine.Object.Instantiate(foodSequence.Food) as GameObject);
			if (createdFood != null)
			{
				Transform transform = Utils.FindNodeInChildren(foodDevice.gameObject.transform, "food_spawner");
				Utils.AttachGameObject(transform.gameObject, createdFood);
				Vector3 zero = Vector3.zero;
				if (createdFood.collider != null)
				{
					Vector3 size = createdFood.collider.bounds.size;
					zero.y = size.y / 2f;
				}
				createdFood.transform.localPosition = zero;
			}
			currentState = State.EatItem;
			break;
		case State.EatItem:
		{
			GameObject gameObject = list.GetEffectSequencePrefabByName("emote_eat_sequence") as GameObject;
			if (gameObject != null)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject) as GameObject;
				currentSequence = (gameObject2.GetComponent(typeof(EffectSequence)) as EffectSequence);
				currentSequence.Initialize(charGlobals.gameObject, OnEatDone, OnEffectEvent);
				currentSequence.StartSequence();
			}
			currentState = State.EatingItem;
			break;
		}
		case State.Done:
			if (ItemUsed != null)
			{
				ItemUsed(foodDevice.gameObject);
			}
			break;
		}
	}

	private void OnEffectEvent(EffectSequence seq, EventEffect effect)
	{
		if (effect.EventName == "TurnOn")
		{
			currentState = State.PlayFoodSequence;
		}
		else if (effect.EventName == "EatFood")
		{
			if (createdFood != null)
			{
				DetachFood();
				UnityEngine.Object.Destroy(createdFood);
				createdFood = null;
			}
		}
		else if (effect.EventName == "GrabFood")
		{
			AttachFoodObject();
		}
	}

	private void DetachFood()
	{
		AIControllerHQ component = Utils.GetComponent<AIControllerHQ>(owningObject);
		if (component != null)
		{
			component.DetachObject(createdFood);
		}
	}

	protected void OnEatDone(EffectSequence seq)
	{
		currentState = State.Done;
		currentSequence = null;
	}

	protected void AttachFoodObject()
	{
		AIControllerHQ component = Utils.GetComponent<AIControllerHQ>(owningObject);
		if (createdFood != null && component != null)
		{
			component.AttachObject(createdFood, "fx_Rpalm");
			createdFood.transform.localPosition = Vector3.zero;
			if (createdFood.transform.parent != null)
			{
				createdFood.transform.position = createdFood.transform.position + createdFood.transform.parent.transform.up * 0.28f;
			}
		}
	}

	public override void behaviorEnd()
	{
		if (createdFood != null)
		{
			DetachFood();
			UnityEngine.Object.Destroy(createdFood);
			createdFood = null;
		}
		currentState = State.Done;
	}

	public override void behaviorCancel()
	{
		if (createdFood != null)
		{
			DetachFood();
			UnityEngine.Object.Destroy(createdFood);
			createdFood = null;
		}
		base.behaviorCancel();
		currentState = State.Done;
	}

	public override bool allowInterrupt(Type newBehaviorType)
	{
		return currentState == State.Done;
	}

	private bool HasAnimationCompleted(string animationName)
	{
		return !foodDevice.gameObject.animation.isPlaying || (foodDevice.gameObject.animation.IsPlaying(animationName) && foodDevice.gameObject.animation[animationName].time >= foodDevice.gameObject.animation[animationName].length);
	}
}
