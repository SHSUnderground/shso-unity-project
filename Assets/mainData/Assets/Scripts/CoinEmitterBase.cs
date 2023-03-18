using System.Collections;
using UnityEngine;

public class CoinEmitterBase : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected Object prefab;

	protected GameObject sourceObject;

	protected GameObject dispenserObject;

	protected CoinsTriggerAdaptor adapter;

	public IEnumerator InitEmitter(GameObject sourceObject, GameObject dispenserObject, CoinsTriggerAdaptor adapter)
	{
		prefab = SocialSpaceController.GameworldPrefabBundle.Load(adapter.coinPrefabName);
		if (prefab == null)
		{
			CspUtils.DebugLog("coin prefab " + adapter.coinPrefabName + " doesn't exist in: " + SocialSpaceController.GameworldPrefabBundleName);
			yield break;
		}
		this.sourceObject = sourceObject;
		this.dispenserObject = dispenserObject;
		this.adapter = adapter;
		OnEmitterInitialized();
		StartCoroutine(CoEmitCoins());
	}

	protected virtual void OnEmitterInitialized()
	{
	}

	protected IEnumerator CoEmitCoins()
	{
		for (int i = 0; i < adapter.coinAmount; i++)
		{
			EmitCoin(i);
			if (adapter.delay > 0f)
			{
				yield return new WaitForSeconds(adapter.delay);
			}
		}
		yield return new WaitForSeconds(2f);
		Object.Destroy(base.gameObject);
	}

	protected virtual void EmitCoin(int coinIndex)
	{
	}
}
