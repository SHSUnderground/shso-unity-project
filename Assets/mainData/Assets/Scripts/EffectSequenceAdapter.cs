using UnityEngine;

public class EffectSequenceAdapter : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public EffectSequence effectSequence;

	public bool startEnabled = true;

	private EffectSequence effectInstance;

	public void OnEnable()
	{
		if (startEnabled && effectSequence != null)
		{
			if (effectInstance != null)
			{
				effectInstance.Cancel();
			}
			effectInstance = (Object.Instantiate(effectSequence) as EffectSequence);
			effectInstance.Initialize(effectSequence.transform.parent.gameObject, null, null);
			effectInstance.StartSequence();
		}
		else if (!startEnabled)
		{
			startEnabled = true;
		}
	}

	public void OnDisable()
	{
		if (effectInstance != null)
		{
			effectInstance.Cancel();
			effectInstance = null;
		}
	}
}
