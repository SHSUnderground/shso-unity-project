using UnityEngine;

public class EffectSequenceTriggerAdapter : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public EffectSequence effectSequence;

	public GameObject parentObject;

	private void Triggered()
	{
		if (parentObject == null)
		{
			parentObject = base.gameObject;
		}
		EffectSequence effectSequence = Object.Instantiate(this.effectSequence) as EffectSequence;
		effectSequence.Initialize(parentObject, null, null);
		effectSequence.StartSequence();
	}
}
