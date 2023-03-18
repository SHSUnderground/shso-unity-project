using System.Collections;
using UnityEngine;

public class RangedActionShootAndDrop : HotspotAction
{
	private const float gravity = 10f;

	public GameObject objectToShoot;

	public GameObject objectToDrop;

	public float distanceToDrop;

	public EffectSequence effectOnFinish;

	public float resetDelay = 3f;

	private OnFinished onFinished;

	public override void PerformAction(CharacterGlobals player, OnFinished onFinished)
	{
		this.onFinished = onFinished;
		if (objectToShoot == null || objectToDrop == null)
		{
			CspUtils.DebugLog(base.gameObject.name + " is not configured correctly: ensure that objectToShoot and objectToDrop are set in the inspector");
			Finish();
			return;
		}
		CombatController.AttackData attackData = player.combatController.getAttackData(1);
		if (attackData == null)
		{
			CspUtils.DebugLog(base.gameObject.name + " could not fetch " + player.gameObject.name + "'s L1");
			Finish();
			return;
		}
		BehaviorAttackOnce behaviorAttackOnce = player.behaviorManager.requestChangeBehavior<BehaviorAttackOnce>(false);
		if (behaviorAttackOnce != null)
		{
			behaviorAttackOnce.Initialize(objectToShoot, attackData, false, false, player.combatController.EmoteBroadcastRadius);
			StartCoroutine(CheckAttackCancel(player));
		}
		else
		{
			CspUtils.DebugLog("Error creating attack behavior for " + player.gameObject);
			Finish();
		}
	}

	private IEnumerator CheckAttackCancel(CharacterGlobals player)
	{
		while (player.behaviorManager.getBehavior() is BehaviorAttackOnce)
		{
			yield return 0;
		}
		yield return new WaitForSeconds(2f);
		Finish();
	}

	public void OnObjectAttacked(CharacterGlobals player)
	{
		StopAllCoroutines();
		StartCoroutine(DropObject());
	}

	private IEnumerator DropObject()
	{
		Animation animComp = Utils.GetComponent<Animation>(objectToDrop, Utils.SearchParents);
		if (animComp != null)
		{
			animComp.Stop();
		}
		Vector3 position = objectToDrop.transform.position;
		float originalHeight = position.y;
		float currentHeight = originalHeight;
		float velocity = -10f * Time.deltaTime;
		while (originalHeight - currentHeight < distanceToDrop)
		{
			velocity += 10f * Time.deltaTime;
			currentHeight -= velocity * Time.deltaTime;
			SetObjectHeight(currentHeight);
			yield return 0;
		}
		PlayFinishEffect();
		yield return new WaitForSeconds(resetDelay);
		SetObjectHeight(originalHeight);
		if (animComp != null)
		{
			animComp.Play();
		}
		Finish();
		onFinished = null;
	}

	private void SetObjectHeight(float height)
	{
		Vector3 position = objectToDrop.transform.position;
		position.y = height;
		objectToDrop.transform.position = position;
	}

	private void PlayFinishEffect()
	{
		if (effectOnFinish != null)
		{
			GameObject g = Object.Instantiate(effectOnFinish.gameObject, Vector3.zero, Quaternion.identity) as GameObject;
			EffectSequence component = Utils.GetComponent<EffectSequence>(g);
			component.Initialize(objectToDrop, AutoKillSequence, null);
			component.StartSequence();
		}
	}

	private void AutoKillSequence(EffectSequence sequence)
	{
		Object.Destroy(sequence.gameObject);
	}

	private void Finish()
	{
		if (onFinished != null)
		{
			onFinished();
		}
	}
}
