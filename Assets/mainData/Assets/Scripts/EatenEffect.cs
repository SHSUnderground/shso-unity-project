using UnityEngine;

public class EatenEffect : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject effectSequence;

	public void PlaySequence(GameObject newParent)
	{
		if (effectSequence != null)
		{
			GameObject instance = Object.Instantiate(effectSequence) as GameObject;
			EffectSequence component = Utils.GetComponent<EffectSequence>(instance);
			component.Initialize(newParent, delegate
			{
				Object.Destroy(instance);
			}, null);
		}
	}

	public static void Play(GameObject effectOwner, GameObject parent)
	{
		EatenEffect component = Utils.GetComponent<EatenEffect>(effectOwner);
		if (component != null)
		{
			component.PlaySequence(parent);
		}
	}
}
