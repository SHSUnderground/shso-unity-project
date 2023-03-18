using UnityEngine;

[AddComponentMenu("Lab/Character/Target Pusher")]
[RequireComponent(typeof(CharacterTargetSource))]
public class CharacterTargetPusher : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public CharacterTargetSource[] Targets = new CharacterTargetSource[0];

	public bool PushOnEnable = true;

	public bool PushOnDisable;

	private CharacterTargetSource _sourceCache;

	private CharacterTargetSource _source
	{
		get
		{
			_sourceCache = (_sourceCache ?? Utils.GetComponent<CharacterTargetSource>(this));
			return _sourceCache;
		}
	}

	private void PushToTargets()
	{
		if (_source == null)
		{
			return;
		}
		CharacterTargetSource[] targets = Targets;
		foreach (CharacterTargetSource characterTargetSource in targets)
		{
			if (!(characterTargetSource == null))
			{
				characterTargetSource.Target = _source.Target;
				characterTargetSource.enabled = true;
			}
		}
	}

	private void OnEnable()
	{
		if (PushOnEnable)
		{
			PushToTargets();
		}
	}

	private void OnDisable()
	{
		if (PushOnDisable)
		{
			PushToTargets();
		}
	}
}
