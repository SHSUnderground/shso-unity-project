using UnityEngine;
using System.Collections;
using System;

// Resource Manager - This class did not exist in original SHSO code. 
public class ResMgr : MonoBehaviour {

	//Hashtable bundleToAssets = new Hashtable();
	private static Hashtable bundleHash = new Hashtable();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// load all assets and add to dictionary.
	private static UnityEngine.Object[] loadNewBundle(string bundleName) {

		UnityEngine.Object[] objs = Resources.LoadAll(bundleName);  //NOTE!!! bundleName is the dir name under Resources.
		bundleHash.Add(bundleName, objs);

		return objs;
	}

	// get asset from bundle, if possible. if bundle is not yet in dictionary, load all assets and add to dictionary.
	public static UnityEngine.Object getAssetFromBundle(string bundleName, string assetName) {

		UnityEngine.Object [] objs = (UnityEngine.Object []) bundleHash[bundleName];

		if (objs == null) 
			objs = loadNewBundle(bundleName);
	
		//UnityEngine.Object first = Array.Find(objs, nameMatches(assetName));
		UnityEngine.Object asset = Array.Find(objs, p => p.name == assetName);   // find asset by name in objs array.
		if (asset != null)
			CspUtils.DebugLog("ResMgr found asset: " + assetName + " in bundle "  + bundleName);
		else
			CspUtils.DebugLog("ResMgr NOT FOUND asset: " + assetName + " in bundle "  + bundleName);
		return asset;
	}
}
