using System.Collections.Generic;
using UnityEngine;

public class EffectSequenceList : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public class EffectInfo
	{
		public string name = string.Empty;

		public GameObject prefab;

		public EffectInfo(string prefabName, GameObject prefabInst)
		{
			name = prefabName;
			prefab = prefabInst;
		}
	}

	public delegate void EffectsLoadedCallback(EffectSequenceList fxList, object extraData);

	protected Dictionary<string, EffectInfo> LogicalEffects;

	protected string characterFxBundleName;

	protected List<string> sharedFxBundleNames = new List<string>();

	protected string characterPrefabsBundleName;

	protected AssetBundle characterFxBundle;

	protected List<AssetBundle> sharedFxBundles = new List<AssetBundle>();

	protected AssetBundle characterPrefabsBundle;

	protected Queue<WaitingForEffectsCallback> waitingQueue;

	protected int bundlesToLoadCount;

	protected bool initialized;

	public Dictionary<string, UnityEngine.Object> assetCache = new Dictionary<string, UnityEngine.Object>();

	public bool Initialized
	{
		get
		{
			return initialized;
		}
	}

	public bool BundlesLoaded
	{
		get
		{
			return bundlesToLoadCount == 0;
		}
	}

	private void Awake()
	{
		bundlesToLoadCount = 2;
		LogicalEffects = new Dictionary<string, EffectInfo>();
		waitingQueue = new Queue<WaitingForEffectsCallback>();
	}

	public void InitializeFromData(DataWarehouse fxData, AssetBundle characterAssetBundle, string characterAssetBundleName)
	{
		characterFxBundleName = fxData.TryGetString("character_fx", string.Empty);
		sharedFxBundleNames.Add("FX/shared_brawler_fx");
		sharedFxBundleNames.Add("FX/shared_character_fx");
		sharedFxBundleNames.Add("FX/shared_consumables_fx");
		sharedFxBundleNames.Add("FX/shared_gameworld_fx");
		sharedFxBundleNames.Add("FX/shared_general_fx");
		sharedFxBundleNames.Add("FX/shared_gimmick_fx");
		sharedFxBundleNames.Add("FX/shared_pickup_fx");
		characterPrefabsBundleName = characterAssetBundleName;
		characterPrefabsBundle = characterAssetBundle;
		foreach (DataWarehouse item in fxData.GetIterator("logical_effect"))
		{
			string text = item.TryGetString("name", string.Empty);
			string text2 = item.TryGetString("prefab_name", string.Empty);
			if (text == string.Empty || text2 == string.Empty)
			{
				CspUtils.DebugLog("Bad logical effect data found <" + text + ", " + text2 + "> for game object <" + base.gameObject.name + ">.  Some logical effects may not play correctly.");
			}
			else
			{
				LogicalEffects[text] = new EffectInfo(text2, null);
			}
		}
		bundlesToLoadCount = 0;
		if (!string.IsNullOrEmpty(characterFxBundleName))
		{
			bundlesToLoadCount++;
			AppShell.Instance.BundleLoader.FetchAssetBundle(characterFxBundleName, OnAssetBundleLoaded);
		}
		foreach (string sharedFxBundleName in sharedFxBundleNames)
		{
			bundlesToLoadCount++;
			AppShell.Instance.BundleLoader.FetchAssetBundle(sharedFxBundleName, OnAssetBundleLoaded);
		}
		initialized = true;
	}

	public void InitializeFromCopy(EffectSequenceList source)
	{
		bundlesToLoadCount = 0;
		characterFxBundleName = source.characterFxBundleName;
		characterFxBundle = source.characterFxBundle;
		sharedFxBundleNames = source.sharedFxBundleNames;
		sharedFxBundles = source.sharedFxBundles;
		characterPrefabsBundleName = source.characterPrefabsBundleName;
		characterPrefabsBundle = source.characterPrefabsBundle;
		LogicalEffects = source.LogicalEffects;
	}

	public void RequestLoadedCallback(EffectsLoadedCallback cb, object extraData)
	{
		waitingQueue.Enqueue(new WaitingForEffectsCallback(cb, extraData));
		if (bundlesToLoadCount <= 0)
		{
			NotifyAllWaitingCallbacks();
		}
	}

	protected void NotifyAllWaitingCallbacks()
	{
		WaitingForEffectsCallback waitingForEffectsCallback = null;
		while (waitingQueue.Count > 0)
		{
			waitingForEffectsCallback = waitingQueue.Dequeue();
			waitingForEffectsCallback.Callback(this, waitingForEffectsCallback.ExtraData);
		}
	}

	protected void OnAssetBundleLoaded(AssetBundleLoadResponse response, object extraData)
	{
		if (response.Error != null && response.Error != string.Empty)
		{
			CspUtils.DebugLog("Failed to load the FX asset bundle <" + response.Path + ">.  There may be FX missing from this character.");
		}
		if (response.Path == characterFxBundleName)
		{
			characterFxBundle = response.Bundle;
			bundlesToLoadCount--;
		}
		else if (sharedFxBundleNames.Contains(response.Path))
		{
			sharedFxBundles.Add(response.Bundle);
			bundlesToLoadCount--;
		}
		else
		{
			CspUtils.DebugLog("Received a call back from the AssetBundleLoader that we did not expect for <" + response.Path + ">.");
		}
		if (bundlesToLoadCount <= 0)
		{
			NotifyAllWaitingCallbacks();
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

	protected virtual Object GetEffectSequencePrefabByNameInternal(string name)
	{
		Object @object = null;

		//// CSP - original code did no asset caching....so I had to implement my own.//////////
		//// probably need to modify it to purge asset cache ASAP. ////////		
		if(assetCache.TryGetValue(name, out @object))
		{
			CspUtils.DebugLog("GESPBNI found in cache: " + name);
			return @object;
		}
		///////////////////////////////////////////////////////////////////////////////////////

		CspUtils.DebugLog("GESPBNI prefab name=" + name);
		if (@object == null && characterPrefabsBundle != null)
		{
			CspUtils.DebugLog("GESPBNI characterPrefabsBundle=" + characterPrefabsBundle.name);
			//@object = characterPrefabsBundle.Load(name);
			@object = CspUtils.CspLoad(characterPrefabsBundle,name);   // CSP
			if (@object != null) {
				CspUtils.DebugLog("ASSET FOUND in CspLoad() for assetName=" + name + " bundle=" + characterPrefabsBundle.name);
			}		
		}
		if (@object == null && characterFxBundle != null)
		{
			CspUtils.DebugLog("GESPBNI characterFxBundle=" + characterFxBundle.name);
			//@object = characterFxBundle.Load(name);
			@object = CspUtils.CspLoad(characterFxBundle,name);   // CSP
			if (@object != null) {
				CspUtils.DebugLog("ASSET FOUND in CspLoad() for assetName=" + name + " bundle=" + characterFxBundle.name);
			}
		}
		if (@object == null)
		{
			foreach (AssetBundle sharedFxBundle in sharedFxBundles)
			{
				if (sharedFxBundle != null) {
					CspUtils.DebugLog("GESPBNI sharedFxBundle=" + sharedFxBundle.name);
					//@object = sharedFxBundle.Load(name);	
					@object = CspUtils.CspLoad(sharedFxBundle,name);   // CSP
				}				

				if (@object != null) {
					CspUtils.DebugLog("ASSET FOUND in CspLoad() for assetName=" + name + " bundle=" + sharedFxBundle.name);
					assetCache.Add(name, @object);  // added by CSP
					return @object;
				}
				
			}
			//return @object;  
		}
		assetCache.Add(name, @object); // added by CSP
		return @object;
	}

	public Object GetEffectSequencePrefabByName(string name)
	{
		return GetEffectSequencePrefabByNameInternal(name);
	}

	public Object TryGetEffectSequencePrefabByName(string name)
	{
		return GetEffectSequencePrefabByNameInternal(name);
	}

	public bool TryGetEffectSequenceByName(string name, out EffectSequence sequence)
	{
		sequence = null;
		Object @object = TryGetEffectSequencePrefabByName(name);
		if (@object != null)
		{
			GameObject gameObject = Object.Instantiate(@object) as GameObject;
			sequence = (gameObject.GetComponent(typeof(EffectSequence)) as EffectSequence);
			sequence.SetParent(base.gameObject);
			return true;
		}
		return false;
	}

	public GameObject GetGameObjectPrefabByName(string name)
	{
		GameObject result = null;
		if (characterPrefabsBundle != null)
		{
			result = (characterPrefabsBundle.Load(name) as GameObject);
		}
		return result;
	}

	public string GetLogicalEffectSequencePrefabName(string name)
	{
		EffectInfo value;
		if (LogicalEffects.TryGetValue(name, out value))
		{
			return value.name;
		}
		return null;
	}

	public GameObject GetLogicalEffectSequencePrefab(string name)
	{
		GameObject prefab;
		if (TryGetLogicalEffectSequencePrefab(name, out prefab))
		{
			return prefab;
		}
		return null;
	}

	public virtual bool TryGetLogicalEffectSequencePrefab(string name, out GameObject prefab)
	{
		prefab = null;
		EffectInfo value = null;
		if (LogicalEffects.TryGetValue(name, out value))
		{
			if (value != null)
			{
				if (value.prefab == null)
				{
					value.prefab = (GetEffectSequencePrefabByName(value.name) as GameObject);
				}
				prefab = value.prefab;
				if (characterFxBundle == null)
				{
					value.prefab = null;
				}
				return true;
			}
			return false;
		}
		return false;
	}

	public EffectSequence GetLogicalEffectSequence(string name)
	{
		EffectSequence sequence;
		if (TryGetLogicalEffectSequence(name, out sequence))
		{
			return sequence;
		}
		return null;
	}

	public bool TryGetLogicalEffectSequence(string name, out EffectSequence sequence)
	{
		sequence = null;
		GameObject prefab;
		if (TryGetLogicalEffectSequencePrefab(name, out prefab))
		{
			if (prefab == null)
			{
				return false;
			}
			GameObject gameObject = Object.Instantiate(prefab) as GameObject;
			sequence = (gameObject.GetComponent(typeof(EffectSequence)) as EffectSequence);
			sequence.SetParent(base.gameObject);
			return true;
		}
		return false;
	}

	public EffectSequence PlaySequence(string name)
	{
		return PlaySequence(name, base.gameObject, null, null);
	}

	public EffectSequence PlaySequence(string name, GameObject parent)
	{
		return PlaySequence(name, parent, null, null);
	}

	public EffectSequence PlaySequence(string name, EffectSequence.OnSequenceDone onDone)
	{
		return PlaySequence(name, base.gameObject, onDone, null);
	}

	public EffectSequence PlaySequence(string name, GameObject parent, EffectSequence.OnSequenceDone onDone)
	{
		return PlaySequence(name, parent, onDone, null);
	}

	public EffectSequence PlaySequence(string name, GameObject parent, EffectSequence.OnSequenceDone onDone, EffectSequence.OnSequenceEvent onEvent)
	{
		EffectSequence effectSequence = null;
		GameObject gameObject = TryGetEffectSequencePrefabByName(name) as GameObject;
		if (gameObject != null)
		{
			effectSequence = (Object.Instantiate(gameObject) as GameObject).GetComponent<EffectSequence>();
			if (effectSequence != null)
			{
				effectSequence.Initialize(parent, onDone, onEvent);
				if (!effectSequence.AutoStart)
				{
					effectSequence.StartSequence();
				}
			}
		}
		return effectSequence;
	}

	public void OneShot(string name)
	{
		OneShot(name, base.gameObject);
	}

	public void OneShot(string name, GameObject parent)
	{
		if (!TryOneShot(name, parent))
		{
			CspUtils.DebugLog("Effect sequence <" + name + "> was not found");
		}
	}

	public bool TryOneShot(string name)
	{
		return TryOneShot(name, base.gameObject);
	}

	public bool TryOneShot(string name, GameObject parent)
	{
		EffectSequence effect;
		return TryOneShot(name, parent, out effect);
	}

	public bool TryOneShot(string name, GameObject parent, out EffectSequence effect)
	{
		return TryOneShot(name, parent, null, out effect);
	}

	public bool TryOneShot(string name, GameObject parent, CharacterGlobals creator, out EffectSequence effect)
	{
		effect = null;
		GameObject gameObject = TryGetEffectSequencePrefabByName(name) as GameObject;
		if (gameObject != null)
		{
			effect = (Object.Instantiate(gameObject) as GameObject).GetComponent<EffectSequence>();
			if (effect != null)
			{
				if (creator != null)
				{
					effect.SendMessage("AssignCreator", creator, SendMessageOptions.DontRequireReceiver);
				}
				effect.Initialize(parent, DeleteOnDone, null);
				effect.StartSequence();
				return true;
			}
			return false;
		}
		return false;
	}

	private void DeleteOnDone(EffectSequence seq)
	{
		Object.Destroy(seq.gameObject);
	}
}
