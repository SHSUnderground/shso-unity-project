using UnityEngine;
using System.Collections;
//using UnityEditor;
using System.Xml;
using System.Xml.Serialization;
using System.IO;



public class ABundleTester : MonoBehaviour {

	System.Security.Cryptography.MD5CryptoServiceProvider md = null;
        
	static public ABundleTester instance; //the instance of our class that will do the work

	 void Awake(){ //called when an instance awakes in the game
		instance = this; //set our static reference to our newly initialized instance
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator  doTest() {
		WWW www = null;
		AssetBundle myBundle = null;

		CspUtils.DebugLog("doTest() entered");
		// CSP - LoadAll does work, though. ///////
		//AssetBundles abClone = Instantiate(assetBundleDictionary[bundleName].LanguageBundle);
		//Resources.UnloadUnusedAssets();
		//bool cFlag = Caching.IsVersionCached ("AssetBundles/GUI/non_locale/login_bundle.unity3d", 9);
		//CspUtils.DebugLog("@@@@@@@@@@@@@@@@@@ bundle is cached = " + cFlag);



		//////////////////////////////////////////////////////
		// Object[] aobjs = Resources.LoadAll("common_bundle");  //NOTE!!! common_bundles is the dir name under Resources.

		// int j=0;
		// foreach (UnityEngine.Object obj3 in aobjs)
		// {	
		// 	CspUtils.DebugLog(j + " loaded FROM RESOURCES asset name:" + obj3.name);
		// 	j++;
		// 	//if (obj3.name == bundleAsset) {
		// 	//	CspUtils.DebugLog("asset FOUND!:" + bundleAsset);
		// 	//	break;
		// 	//}

		// }
		// EditorApplication.isPaused = true;
		// yield break;
		///////////////////////////////////////////////////////////



		///////////////////////////////////////////////////////////////////////
		// //www = WWW.LoadFromCacheOrDownload ("file://AssetBundles/GUI/non_locale/common_bundle.unity3d", 5);
		// www = WWW.LoadFromCacheOrDownload ("http://192.168.235.128/winter_soldier_fx.unity3d", 5);
		// yield return www;
		// if (www.error != null)
		// {
		// 	CspUtils.DebugLog (www.error);
		// 	yield break;
		// }


	    // myBundle = www.assetBundle;
		// //var asset = myLoadedAssetBundle.mainAsset;
		// if (myBundle == null)
		// {
		// 	CspUtils.DebugLog("myBundle is null!");
		// 	yield break;
		// }
		// else {
		// 	CspUtils.DebugLog("myBundle is loaded!");

		// 	Object[] allObjs = myBundle.LoadAll();
		// 	BuildPipeline.BuildAssetBundle(myBundle.mainAsset, allObjs, "christest.unity3d", BuildAssetBundleOptions.CollectDependencies | BuildAssetBundleOptions.CompleteAssets, BuildTarget.StandaloneWindows, BuildOptions.UncompressedAssetBundle);
		// 	myBundle.Unload(true);			
		// }
		////////////////////////////////////////////////////////////////////////////////////////////


		AssetBundle bundleFromFile = AssetBundle.CreateFromFile("christest.unity3d");
		//AssetBundle bundleFromFile = AssetBundle.CreateFromFile("nighthawk_fx.unity3d");
		
		if (bundleFromFile == null)
		{
			CspUtils.DebugLog("bundleFromFile is null!");
		}
		else 
		{
			
			UnityEngine.Object testobj = bundleFromFile.Load("VO_UI_she_hulk_red_MissionMulti");
			if (testobj == null) {
				CspUtils.DebugLog("testobj is null!");
			}
			else {
				CspUtils.DebugLog("testobj is loaded!");
			}

			UnityEngine.Object[] testobjs = bundleFromFile.LoadAll();
			CspUtils.DebugLog("testobjs.Length= " + testobjs.Length);

			foreach (UnityEngine.Object obj3 in testobjs) {
				CspUtils.DebugLog("obj3 loaded asset name:" + obj3.name);
			}
		

		}
		////////////////////////////////////////////////////////////////

		//EditorApplication.isPaused = true;
		yield return www;
		

		string bundleAsset =  "arrow_left_normal";  //"DebugDialogSkin";
		UnityEngine.Object obj = myBundle.Load(bundleAsset);
		if (obj != null)
			CspUtils.DebugLog("asset FOUND!:" + bundleAsset);
		else
			CspUtils.DebugLog("asset NOT FOUND!:" + bundleAsset);

		UnityEngine.Object[] objs = myBundle.LoadAll();
		int i=0;
		foreach (UnityEngine.Object obj2 in objs)
		{	
			CspUtils.DebugLog(i + " loaded asset name:" + obj2.name);
			i++;
			if (obj2.name == bundleAsset) {
				CspUtils.DebugLog("asset FOUND!:" + bundleAsset);
				//break;
			}
			//else {
			//	CspUtils.DebugLog("asset NOT FOUND!:" + bundleAsset);	
			//}		

			CspUtils.DebugLog ("type: " + obj2.GetType().ToString());


			if (obj2.GetType().ToString() == "UnityEngine.Texture2D") {				
    			//AssetDatabase.CreateAsset(obj2, "c:/dev/" + obj2.name + ".asset");
			}
			

			//else
			// Resources.UnloadAsset(obj);   // if it's not the object we're looking for, destroy it.
		}

		//EditorApplication.isPaused = true;

		

		///////////////////////////////////////

	}

	static public void DoCoroutine(){
		CspUtils.DebugLog("DoCoroutine() entered");
		instance.StartCoroutine("doTest"); //this will launch the coroutine on our instance
	}
}
