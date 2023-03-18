using UnityEngine;

public class LauncherDataProvider : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string lookupKey;

	public string assetBundle;

	public string modelName;

	public LauncherDataProviderTypeEnum providerType;

	private GameObject model;

	private void Start()
	{
		if (assetBundle != null && assetBundle != string.Empty && modelName != null && modelName != string.Empty)
		{
			AppShell.Instance.BundleLoader.FetchAssetBundle(assetBundle, delegate(AssetBundleLoadResponse response, object extraData)
			{
				if (response.Error != null)
				{
					CspUtils.DebugLog("Error loading bundle: " + assetBundle);
				}
				else
				{
					Object original = response.Bundle.Load(modelName);
					model = (Object.Instantiate(original, base.transform.position, base.transform.rotation) as GameObject);
					model.transform.parent = base.transform;
					model.animation.Play("movement_idle");
					model.animation.wrapMode = WrapMode.Loop;
				}
			});
		}
	}
}
