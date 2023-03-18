using System.Collections;
using UnityEngine;

public class LoadAssetBundleTest : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string bundleName = string.Empty;

	public bool load;

	public bool unload;

	public bool transition;

	protected string fullPath;

	protected AssetBundle bundle;

	protected GameObject go;

	public void Start()
	{
	}

	public void Update()
	{
		if (load)
		{
			load = false;
			StartCoroutine(Load(bundleName));
		}
		if (unload)
		{
			unload = false;
			if (bundle != null)
			{
				bundle.Unload(true);
				bundle = null;
			}
		}
		if (transition)
		{
			transition = false;
			Application.LoadLevel(12);
		}
	}

	public IEnumerator Load(string bundleName)
	{
		fullPath = "file://" + Application.dataPath + "/AssetBundles/" + bundleName;
		WWW www = new WWW(fullPath);
		yield return www;
		bundle = www.assetBundle;
		go = (Object.Instantiate(bundle.mainAsset) as GameObject);
	}
}
