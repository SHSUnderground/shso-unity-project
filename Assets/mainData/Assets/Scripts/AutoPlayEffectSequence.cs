using UnityEngine;

public class AutoPlayEffectSequence : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private void Start()
	{
		EffectSequence component = base.gameObject.GetComponent<EffectSequence>();
		if (component != null)
		{
			component.Initialize(base.gameObject, DestroySequence, null);
			component.StartSequence();
		}
	}

	private void DestroySequence(EffectSequence es)
	{
		Object.Destroy(es.gameObject);
	}
}
