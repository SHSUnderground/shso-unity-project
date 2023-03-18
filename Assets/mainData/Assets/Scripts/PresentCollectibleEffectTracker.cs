using UnityEngine;

public class PresentCollectibleEffectTracker : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private EffectSequenceCollection effects;

	public static PresentCollectibleEffectTracker GetInstance(GameObject obj)
	{
		if (obj != null)
		{
			PresentCollectibleEffectTracker presentCollectibleEffectTracker = obj.GetComponent<PresentCollectibleEffectTracker>();
			if (presentCollectibleEffectTracker == null)
			{
				presentCollectibleEffectTracker = obj.AddComponent<PresentCollectibleEffectTracker>();
			}
			return presentCollectibleEffectTracker;
		}
		return null;
	}

	public void PlayEffects(EffectSequenceCollection newEffects, GameObject player)
	{
		if (effects != null)
		{
			effects.Stop();
		}
		effects = newEffects;
		effects.Play(player, null);
	}
}
