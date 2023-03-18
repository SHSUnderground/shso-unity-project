using UnityEngine;

public class ObjectPlaceholder : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public delegate void ObjectPlaceholderCallback(bool success);

	public string prefabName;

	public string bundleName;

	public string parentName;

	public bool dropOnMinSpec;

	public bool forceCollisionLayerToDefault;

	protected ObjectPlaceholderCallback callback;

	protected bool objectCreated;

	protected GameObject parent;

	private void Start()
	{
		createObject(null);
	}

	public void createObject(ObjectPlaceholderCallback newCallback)
	{
		if (objectCreated)
		{
			return;
		}
		objectCreated = true;
		callback = newCallback;
		if (dropOnMinSpec && GraphicsOptions.ModelQuality == GraphicsOptions.GraphicsQuality.Low)
		{
			if (callback != null)
			{
				callback(true);
			}
		}
		else
		{
			AppShell.Instance.BundleLoader.FetchAssetBundle(bundleName, OnAssetBundleLoaded);
		}
	}

	// public Object CspLoad(AssetBundle bundle, string assetName) {
	// 	UnityEngine.Object[] objs = null;
	// 	//UnityEngine.Object[] objs = bundle.LoadAll();  
	// 	//UnityEngine.Object request = bundle.Load(assetName, typeof(UnityEngine.Object));
	// 	UnityEngine.Object request = bundle.Load(assetName);
	// 	if (request == null) {
	// 		objs = bundle.LoadAll();
	// 		request = bundle.Load(assetName);
	// 	}

	// 	UnityEngine.Object asset2 = request;
	// 	if (asset2 == null) {	// CSP - LoadAll does work, though. //////
	// 		CspUtils.DebugLog("asset2 == null in CspLoad() for assetName=" + assetName);
	// 		int i=0;
	// 		foreach (UnityEngine.Object obj in objs)	
	// 		{	
	// 			//CspUtils.DebugLog(i + " loaded asset name:" + obj.name);
	// 			i++;
	// 			if (obj.name.ToLower() == assetName.ToLower()) {
	// 					asset2 = obj;
	// 					CspUtils.DebugLog("FOUND asset:" + assetName);
	// 					CspUtils.DebugLog("of type :" + asset2.GetType().ToString());
	// 					return asset2;
	// 			}	
	// 		}		
	// 	}
	// 	//else
	// 	//	CspUtils.DebugLog("ASSET FOUND in CspLoad() for assetName=" + assetName + " bundle=" + bundle.name);
	// 	return asset2;
	// }

	protected void OnAssetBundleLoaded(AssetBundleLoadResponse response, object extraData)
	{
		if (response.Error != null && response.Error != string.Empty)
		{
			CspUtils.DebugLog("Failed to load ObjectPlaceholder asset bundle <" + response.Path + ">.");
			if (callback != null)
			{
				callback(false);
			}
			return;
		}
		GameObject gameObject = response.Bundle.Load(prefabName) as GameObject;
		Object loadObject =  CspUtils.CspLoad(response.Bundle, prefabName);  // CSP
		CspUtils.DebugLog("loadObject prefabName=" + prefabName +  " type=" + loadObject.GetType().ToString()); // CSP
		gameObject = (UnityEngine.GameObject) loadObject;  // CSP

		if (gameObject == null)
		{
			CspUtils.DebugLog("Failed to find prefab " + prefabName + " in ObjectPlaceholder asset bundle <" + response.Path + ">.");
			if (callback != null)
			{
				callback(false);
			}
			return;
		}
		if (forceCollisionLayerToDefault)
		{
			gameObject.layer = 0;
			foreach (Transform item in Utils.WalkTree(gameObject.transform))
			{
				GameObject gameObject2 = item.gameObject;
				if (gameObject2.name.ToLowerInvariant().Contains("collision"))
				{
					gameObject2.layer = 0;
				}
			}
		}
		GameObject gameObject3 = Object.Instantiate(gameObject) as GameObject;
		gameObject3.AddComponent<ObjectFromPlaceholder>();
		if (dropOnMinSpec)
		{
			LodGraphicsQuality lodGraphicsQuality = gameObject3.AddComponent<LodGraphicsQuality>();
			lodGraphicsQuality.cutOff = GraphicsOptions.GraphicsQuality.Low;
		}
		gameObject3.transform.position = base.transform.position;
		gameObject3.transform.rotation = base.transform.rotation;
		gameObject3.transform.localScale = base.transform.localScale;
		if (!string.IsNullOrEmpty(parentName))
		{
			parent = GameObject.Find(parentName);
			if (parent == null)
			{
				parent = new GameObject(parentName);
				parent.AddComponent(typeof(ObjectFromPlaceholder));
			}
			Utils.AttachGameObject(parent, gameObject3);
		}
		if (callback != null)
		{
			callback(true);
		}
	}
}
