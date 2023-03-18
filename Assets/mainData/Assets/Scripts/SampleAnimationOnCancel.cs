using UnityEngine;

public class SampleAnimationOnCancel : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public void OnEffectSequenceCanceled(EffectSequence canceledSequence)
	{
		Animation componentInChildren = base.transform.root.gameObject.GetComponentInChildren<Animation>();
		if (componentInChildren != null)
		{
			componentInChildren.Stop();
			componentInChildren.Rewind();
			componentInChildren.Sample();
		}
	}
}
