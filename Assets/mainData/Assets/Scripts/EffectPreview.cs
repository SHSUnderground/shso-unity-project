using UnityEngine;

public class EffectPreview : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject model;

	public GameObject sequence;

	protected GameObject m;

	protected EffectSequence seq;

	protected bool seqDone;

	private void Start()
	{
		MeshRenderer meshRenderer = GetComponent(typeof(MeshRenderer)) as MeshRenderer;
		if (meshRenderer != null)
		{
			meshRenderer.enabled = false;
		}
		if (model != null)
		{
			m = (Object.Instantiate(model, base.transform.position, base.transform.rotation) as GameObject);
		}
		Reset();
	}

	private void Update()
	{
		if (seqDone)
		{
			Reset();
		}
	}

	protected void OnSequenceDone(EffectSequence seq)
	{
		seq = null;
		seqDone = true;
	}

	protected void Reset()
	{
		if (sequence != null)
		{
			seqDone = false;
			GameObject gameObject = Object.Instantiate(sequence, base.transform.position, base.transform.rotation) as GameObject;
			if (gameObject != null)
			{
				seq = (gameObject.GetComponent(typeof(EffectSequence)) as EffectSequence);
				if (seq != null)
				{
					seq.SetParent(m);
					seq.Initialize(null, OnSequenceDone, null);
					seq.StartSequence();
				}
				else
				{
					CspUtils.DebugLog("No effect sequence found on <" + sequence.name + ">");
				}
			}
			else
			{
				CspUtils.DebugLog("Unable to instantiate <" + sequence.name + ">");
			}
		}
		else
		{
			seq = null;
			seqDone = true;
		}
	}
}
