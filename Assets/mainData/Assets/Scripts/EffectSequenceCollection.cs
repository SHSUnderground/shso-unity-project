using System;
using UnityEngine;

[Serializable]
public class EffectSequenceCollection
{
	public enum SelectionType
	{
		Random,
		RandomNoRepeat,
		Sequential
	}

	public delegate bool OnFinished();

	public SelectionType sequenceSelection;

	public bool loop = true;

	public EffectSequenceReference[] sequences;

	protected GameObject sequenceOwner;

	protected OnFinished onFinished;

	protected EffectSequence sequenceInstance;

	private int lastSelectedSequence = -1;

	public void Play(GameObject sequenceOwner, OnFinished onFinished)
	{
		this.sequenceOwner = sequenceOwner;
		this.onFinished = onFinished;
		EffectSequence nextSequence = GetNextSequence(sequenceOwner);
		if (nextSequence != null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(nextSequence.gameObject) as GameObject;
			sequenceInstance = gameObject.GetComponentInChildren<EffectSequence>();
			if (sequenceInstance != null)
			{
				sequenceInstance.Initialize(sequenceOwner, OnSequenceFinished, null);
				if (!sequenceInstance.AutoStart)
				{
					sequenceInstance.StartSequence();
				}
			}
			else
			{
				UnityEngine.Object.Destroy(gameObject);
			}
		}
		else if (onFinished != null)
		{
			onFinished();
		}
	}

	public void Stop()
	{
		if (sequenceInstance != null)
		{
			UnityEngine.Object.Destroy(sequenceInstance.gameObject);
		}
	}

	protected EffectSequence GetNextSequence(GameObject sequenceOwner)
	{
		int num = -1;
		if (sequences == null || sequences.Length == 0)
		{
			return null;
		}
		if (sequences.Length == 1)
		{
			num = 0;
		}
		else if (sequenceSelection == SelectionType.Random)
		{
			num = UnityEngine.Random.Range(0, sequences.Length);
		}
		else if (sequenceSelection == SelectionType.RandomNoRepeat)
		{
			num = UnityEngine.Random.Range(0, sequences.Length - 1);
			if (num >= lastSelectedSequence)
			{
				num++;
			}
		}
		else if (sequenceSelection == SelectionType.Sequential)
		{
			num = lastSelectedSequence + 1;
			num = ((num != sequences.Length) ? num : 0);
		}
		if (num != -1)
		{
			lastSelectedSequence = num;
			return sequences[num].GetSequence(sequenceOwner);
		}
		return null;
	}

	private void OnSequenceFinished(EffectSequence sequence)
	{
		UnityEngine.Object.Destroy(sequence.gameObject);
		bool flag = loop;
		if (onFinished != null)
		{
			flag &= onFinished();
		}
		if (flag)
		{
			Play(sequenceOwner, onFinished);
		}
	}
}
