using UnityEngine;
using UnityEditor;
using System.Collections;

public class CleanCacheCSP {   // : MonoBehaviour {

	// Use this for initialization
	//void Start () {
	  /////// cache clearing block added by CSP
		// if (Caching.CleanCache ()) 
		// {
		// 	CspUtils.DebugLog("Successfully cleaned the cache.");
		// }
		// else 
		// {
		// 	CspUtils.DebugLog("Cache is being used.");
		// }
		///////////////////////////////////////
	//}
	
	[MenuItem("Assets/GarbageCollect")]
    static void garbageCollect () {		// method added by CSP

		Resources.UnloadUnusedAssets();
		EditorUtility.UnloadUnusedAssetsIgnoreManagedReferences();
		System.GC.Collect();
	}
}
