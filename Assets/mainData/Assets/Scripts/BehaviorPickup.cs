using UnityEngine;

public class BehaviorPickup : BehaviorBase
{
	public delegate void OnDone(CharacterGlobals player, GameObject pickedUpObject);

	protected const string kPickupAnimation = "pickup";

	protected OnDone onDone;

	protected GameObject pickedUpObject;

	protected Transform attachNode;

	protected Vector3 localPositionStart;

	protected Vector3 localPositionGoal;

	protected bool pickupCompleted;

	protected bool objectAttached;

	protected GameObject spawnedEffect;

	protected float pickupOffset;

	protected float minAnimLength = 0.1f;

	protected float maxAnimLength = 10f;

	protected float oldAnimSpeed = 1f;

	public bool ObjectAttached
	{
		get
		{
			return objectAttached;
		}
	}

	public Vector3 LocalStartPosition
	{
		get
		{
			return localPositionStart;
		}
	}

	public bool Initialize(GameObject objectToPickUp, OnDone onDone)
	{
		if (objectToPickUp == null)
		{
			CspUtils.DebugLog("Can not pick up null object");
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
			CspUtils.DebugLog("Cannot find attach node in pickup behavior");
			return false;
		}
		pickedUpObject = objectToPickUp;
		this.onDone = onDone;
		minAnimLength = ActionTimesDefinition.Instance.PickupMin;
		maxAnimLength = ActionTimesDefinition.Instance.PickupMax;
		oldAnimSpeed = clampAnimationSpeed("pickup", minAnimLength, maxAnimLength);
		animationComponent.Play("pickup");
		objectAttached = (pickupCompleted = false);
		StartSequence();
		return true;
	}

	public bool Initialize(GameObject objectToPickUp, float pickupOffset, OnDone onDone)
	{
		this.pickupOffset = pickupOffset;
		return Initialize(objectToPickUp, onDone);
	}

	public override void behaviorUpdate()
	{
		if (pickedUpObject == null)
		{
			CspUtils.DebugLog("pickedUpObject went to NULL");
			charGlobals.behaviorManager.endBehavior();
			return;
		}
		if (!objectAttached)
		{
			if (!(elapsedTime > 0f))
			{
				base.behaviorUpdate();
				return;
			}
			objectAttached = true;
			localPositionGoal = Vector3.zero;
			localPositionGoal.y += pickupOffset;
			if (pickedUpObject.renderer != null)
			{
				
				float y = localPositionGoal.y;
				Vector3 extents = pickedUpObject.renderer.bounds.extents;
				localPositionGoal.y = y + extents.y * 0.85f;
			}
			pickedUpObject.transform.parent = attachNode.transform;
			localPositionStart = pickedUpObject.transform.localPosition;
		}
		base.behaviorUpdate();
		float num = elapsedTime * animationComponent["pickup"].speed / animationComponent["pickup"].length;
		float num2 = (num - 0.5f) * 2f;
		if (num2 > 0f)
		{
			pickedUpObject.transform.localPosition = Vector3.Lerp(localPositionStart, localPositionGoal, num2);
		}
		if (num >= 1f)
		{
			pickupCompleted = true;
			charGlobals.behaviorManager.endBehavior();
			if (onDone != null)
			{
				onDone(charGlobals, pickedUpObject);
			}
		}
	}

	public override void behaviorEnd()
	{
		if (spawnedEffect != null)
		{
			Object.Destroy(spawnedEffect);
		}
		restoreAnimationSpeed("pickup", oldAnimSpeed);
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
			effectSequence.TimeScale = animationComponent["pickup"].speed / oldAnimSpeed;
			effectSequence.StartSequence();
		}
	}
}
