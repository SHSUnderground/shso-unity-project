using System.Collections;
using UnityEngine;

[AddComponentMenu("Interactive Object/Effect Sequence")]
public class InteractiveObjectEffectSequence : MonoBehaviour, IInteractiveObjectChild
{
	public EffectSequence effect;

	public FRange delay;

	public bool loop = true;

	protected InteractiveObject owner;

	protected GameObject model;

	protected bool inUse;

	protected void Triggered()
	{
		if (!inUse && effect != null && model != null)
		{
			StartCoroutine(PlayEffectSequence());
		}
	}

	protected IEnumerator PlayEffectSequence()
	{
		yield return new WaitForSeconds(delay.RandomValue);
		inUse = true;
		GameObject inst = Object.Instantiate(effect.gameObject, model.transform.position, model.transform.rotation) as GameObject;
		inst.transform.parent = model.transform;
		EffectSequence effectInst = Utils.GetComponent<EffectSequence>(inst);
		effectInst.Initialize(model, OnEffectFinished, null);
		effectInst.StartSequence();
	}

	private void OnEffectFinished(EffectSequence seq)
	{
		inUse = false;
		if (loop && base.gameObject.active)
		{
			StartCoroutine(PlayEffectSequence());
		}
	}

	public void Initialize(InteractiveObject owner, GameObject model)
	{
		this.owner = owner;
		this.model = model;
		if (model == null && owner != null)
		{
			Animation component = Utils.GetComponent<Animation>(owner, Utils.SearchChildren);
			if (component == null)
			{
				CspUtils.DebugLog("Tried to attach an InteractiveObjectEffectSequence without a child object named \"Model\" or a child with an animation component.");
			}
			else
			{
				this.model = component.gameObject;
			}
		}
	}

	public float GetLength()
	{
		return -1f;
	}
}
