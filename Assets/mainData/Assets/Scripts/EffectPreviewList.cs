using System.Collections.Generic;
using UnityEngine;

public class EffectPreviewList : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	internal class SequenceState
	{
		internal EffectPreviewList owner;

		internal GameObject prefab;

		internal EffectSequence seq;

		internal bool done;

		internal SequenceState(EffectPreviewList owner, GameObject prefab)
		{
			this.owner = owner;
			this.prefab = prefab;
			seq = null;
			done = true;
		}

		internal void Update()
		{
			if (done)
			{
				Reset();
			}
		}

		internal void Reset()
		{
			done = false;
			GameObject gameObject = Object.Instantiate(prefab, owner.transform.position, owner.transform.rotation) as GameObject;
			if (gameObject != null)
			{
				seq = (gameObject.GetComponent(typeof(EffectSequence)) as EffectSequence);
				if (seq != null)
				{
					if (owner.attachedToHqObject)
					{
						seq.transform.parent = owner.modelInstance.transform;
						seq.Initialize(owner.modelInstance, onDone, null);
						seq.transform.localPosition = new Vector3(0f, 0f, 0f);
						seq.transform.localRotation = Quaternion.Euler(270f, 0f, 0f);
					}
					else
					{
						seq.Initialize(owner.modelInstance, onDone, null);
					}
					seq.StartSequence();
				}
				else
				{
					CspUtils.DebugLog("No effect sequence found on <" + prefab.name + ">");
				}
			}
			else
			{
				CspUtils.DebugLog("Unable to instantiate <" + prefab.name + ">");
			}
		}

		internal void onDone(EffectSequence s)
		{
			done = true;
			seq = null;
		}
	}

	public GameObject model;

	public GameObject[] sequences;

	public bool attachedToHqObject;

	protected GameObject modelInstance;

	internal List<SequenceState> seqs;

	private void Start()
	{
		MeshRenderer meshRenderer = GetComponent(typeof(MeshRenderer)) as MeshRenderer;
		if (meshRenderer != null)
		{
			meshRenderer.enabled = false;
		}
		if (model != null)
		{
			modelInstance = (Object.Instantiate(model) as GameObject);
			modelInstance.transform.position = base.gameObject.transform.position;
		}
		seqs = new List<SequenceState>(sequences.Length);
		for (int i = 0; i < sequences.Length; i++)
		{
			if (sequences[i] != null)
			{
				seqs.Add(new SequenceState(this, sequences[i]));
			}
		}
	}

	private void Update()
	{
		foreach (SequenceState seq in seqs)
		{
			seq.Update();
		}
	}
}
