using UnityEngine;

public class HQEffectSequenceAdapter : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public EffectSequence effectSequence;

	private EffectSequence effectInstance;

	public Animation animationToBindTo;

	public bool reversedAnimation;

	public void OnActivated()
	{
		if (effectSequence != null)
		{
			if (effectInstance != null)
			{
				effectInstance.Cancel();
			}
			effectInstance = (Object.Instantiate(effectSequence) as EffectSequence);
			effectInstance.Initialize(base.gameObject, null, null);
			effectInstance.TimeOffset = GetSequenceOffset();
			effectInstance.StartSequence();
		}
	}

	public void OnDeactivated()
	{
		if (effectInstance != null)
		{
			effectInstance.Cancel();
			effectInstance = null;
		}
	}

	private float GetSequenceOffset()
	{
		if (animationToBindTo == null)
		{
			return 0f;
		}
		float time = animationToBindTo[animationToBindTo.clip.name].time;
		if (reversedAnimation)
		{
			return animationToBindTo.clip.length - time;
		}
		return time;
	}
}
