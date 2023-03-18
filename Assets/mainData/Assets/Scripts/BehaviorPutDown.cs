using UnityEngine;

public class BehaviorPutDown : BehaviorBase
{
	public delegate void OnDone(CharacterGlobals player, GameObject putDownObject);

	protected OnDone onDone;

	protected GameObject putDownObject;

	protected Transform attachNode;

	protected Vector3 localPositionStart;

	protected Vector3 localPositionGoal;

	protected bool putDownCompleted;

	protected bool objectAttached;

	protected GameObject spawnedEffect;

	public bool Initialize(GameObject objectToPutDown, Vector3 finalLocalPosition, OnDone onDone)
	{
		if (objectToPutDown == null)
		{
			CspUtils.DebugLog("Could not put down null object");
			return false;
		}
		if (animationComponent["pickup"] == null)
		{
			CspUtils.DebugLog(owningObject.name + " does not have a pickup animation");
			return false;
		}
		attachNode = Utils.FindNodeInChildren(owningObject.transform, charGlobals.characterController.pickupBone);
		if (attachNode == null)
		{
			CspUtils.DebugLog("Cannot find attach node in putdown behavior");
			return false;
		}
		putDownObject = objectToPutDown;
		localPositionGoal = finalLocalPosition;
		this.onDone = onDone;
		animationComponent.Play("pickup");
		animationComponent["pickup"].time = animationComponent["pickup"].length;
		animationComponent["pickup"].speed = -1f;
		objectAttached = (putDownCompleted = false);
		StartSequence();
		return true;
	}

	public override void behaviorUpdate()
	{
		if (!objectAttached)
		{
			if (!(elapsedTime > 0f))
			{
				base.behaviorUpdate();
				return;
			}
			objectAttached = true;
			putDownObject.transform.parent = attachNode.transform;
			localPositionStart = putDownObject.transform.localPosition;
		}
		base.behaviorUpdate();
		float num = elapsedTime / animationComponent["pickup"].length;
		float num2 = num * 2f;
		if (num2 <= 1f)
		{
			putDownObject.transform.localPosition = Vector3.Lerp(localPositionStart, localPositionGoal, num2);
		}
		if (num >= 1f)
		{
			putDownCompleted = true;
			animationComponent["pickup"].time = 0f;
			animationComponent["pickup"].speed = 1f;
			animationComponent.Stop("pickup");
			charGlobals.behaviorManager.endBehavior();
			if (onDone != null)
			{
				onDone(charGlobals, putDownObject);
			}
		}
	}

	public override void behaviorCancel()
	{
		if (!putDownCompleted)
		{
			onDone(charGlobals, putDownObject);
			putDownCompleted = true;
		}
		base.behaviorCancel();
	}

	public override void behaviorEnd()
	{
		if (!putDownCompleted)
		{
			onDone(charGlobals, putDownObject);
			putDownCompleted = true;
		}
		if (spawnedEffect != null)
		{
			Object.Destroy(spawnedEffect);
		}
		base.behaviorEnd();
	}

	public override bool allowUserInput()
	{
		return false;
	}

	public override bool useMotionController()
	{
		return false;
	}

	protected void StartSequence()
	{
		EffectSequence effectSequence = null;
		GameObject gameObject = charGlobals.effectsList.TryGetEffectSequencePrefabByName("pickup_sequence") as GameObject;
		if (gameObject != null)
		{
			spawnedEffect = (Object.Instantiate(gameObject) as GameObject);
			effectSequence = (spawnedEffect.GetComponent(typeof(EffectSequence)) as EffectSequence);
			effectSequence.SetParent(charGlobals.gameObject);
		}
		if (effectSequence != null)
		{
			effectSequence.AssignCreator(charGlobals);
			effectSequence.Initialize(owningObject, null, null);
			effectSequence.TimeScale = animationComponent["pickup"].speed;
			effectSequence.StartSequence();
		}
	}
}
