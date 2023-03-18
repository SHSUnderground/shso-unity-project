using UnityEngine;

public class EffectSequenceController : GlowableInteractiveController
{
	public EffectSequenceCollection defaultSequences;

	public GameObject sequenceTarget;

	private bool _sequencesEnabled = true;

	private EffectSequenceCollection currentCollection;

	private EffectSequenceCollection nextCollection;

	public bool SequencesEnabled
	{
		get
		{
			return _sequencesEnabled;
		}
		set
		{
			bool sequencesEnabled = _sequencesEnabled;
			_sequencesEnabled = value;
			if (!sequencesEnabled && value && currentCollection == null)
			{
				SetActiveCollection(defaultSequences);
			}
		}
	}

	public GameObject SequenceOwner
	{
		get
		{
			if (sequenceTarget != null)
			{
				return sequenceTarget;
			}
			if (owner != null)
			{
				return owner.GetRoot(InteractiveObject.StateIdx.Model);
			}
			return null;
		}
	}

	public void SetActiveCollection(EffectSequenceCollection sequences)
	{
		if (!(SequenceOwner == null))
		{
			nextCollection = sequences;
			if (currentCollection == null)
			{
				OnCollectionFinished();
			}
		}
	}

	public void ForceSetActiveCollection(EffectSequenceCollection sequences)
	{
		if (currentCollection != null)
		{
			currentCollection.Stop();
			currentCollection = null;
		}
		SetActiveCollection(sequences);
	}

	public EffectSequenceCollection GetActiveCollection()
	{
		return currentCollection;
	}

	public void Start()
	{
		SetActiveCollection(defaultSequences);
	}

	public override void OnRootChanged(InteractiveObject.StateIdx root, GameObject oldRoot, GameObject newRoot)
	{
		if (sequenceTarget == null && currentCollection == null && root == InteractiveObject.StateIdx.Model && oldRoot != newRoot)
		{
			SetActiveCollection(defaultSequences);
		}
		base.OnRootChanged(root, oldRoot, newRoot);
	}

	private bool OnCollectionFinished()
	{
		if (!SequencesEnabled)
		{
			currentCollection = null;
			return false;
		}
		if (nextCollection == null || (nextCollection == currentCollection && !currentCollection.loop))
		{
			nextCollection = defaultSequences;
		}
		if (nextCollection != null && nextCollection != currentCollection)
		{
			currentCollection = nextCollection;
			currentCollection.Play(SequenceOwner, OnCollectionFinished);
			return false;
		}
		return true;
	}
}
