using UnityEngine;

public class SquadBattleSmashable : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject hitObjectAttachedEffect;

	public GameObject detachedEffect;

	public GameObject swapObject;

	public GameObject swapObjectEffect;

	protected bool hit;

	private void Start()
	{
		hit = false;
	}

	private void Update()
	{
		if (base.transform.rigidbody.IsSleeping())
		{
			base.transform.rigidbody.WakeUp();
		}
	}

	public void TriggerHit(GameObject other)
	{
		if (hit)
		{
			return;
		}
		hit = true;
		if (hitObjectAttachedEffect != null)
		{
			GameObject gameObject = Object.Instantiate(hitObjectAttachedEffect, base.gameObject.transform.position, base.gameObject.transform.rotation) as GameObject;
			EffectSequence effectSequence = gameObject.GetComponent(typeof(EffectSequence)) as EffectSequence;
			if (effectSequence != null)
			{
				effectSequence.AttachToParent = true;
				effectSequence.Initialize(base.gameObject, OnSequenceDone, OnSequenceEvent);
				effectSequence.StartSequence();
			}
		}
		else
		{
			OnSequenceEvent(null, new EventEffect("swap", 0f));
		}
		if (detachedEffect != null)
		{
			Object.Instantiate(detachedEffect, base.gameObject.transform.position + detachedEffect.transform.position, detachedEffect.transform.rotation);
		}
	}

	protected void OnSequenceEvent(EffectSequence seq, EventEffect effect)
	{
		if (!(effect.EventName == "swap"))
		{
			return;
		}
		if (swapObject != null)
		{
			GameObject parent = Object.Instantiate(swapObject, base.gameObject.transform.position, base.gameObject.transform.rotation) as GameObject;
			if (swapObjectEffect != null)
			{
				GameObject child = Object.Instantiate(swapObjectEffect, swapObjectEffect.transform.position, swapObjectEffect.transform.rotation) as GameObject;
				Utils.AttachGameObject(parent, child);
			}
		}
		Object.Destroy(base.gameObject);
	}

	protected void OnSequenceDone(EffectSequence seq)
	{
		if (base.gameObject != null)
		{
			OnSequenceEvent(seq, new EventEffect("swap", 0f));
		}
	}
}
