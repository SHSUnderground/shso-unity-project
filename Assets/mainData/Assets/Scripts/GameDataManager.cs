using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected enum Status
	{
		Loading,
		Ready,
		Error
	}


	public delegate void GameDataLoadedCallback(GameDataLoadResponse response, object extraData);

	public const string ASSET_LOCAL_URI_PREFIX = "localfile$";

	public const string ASSET_BUNDLE_PREFIX = "bundle$";

	public const string ASSET_FOLDER_NAME = "GameData/";

	public const string ASSET_EXTENSION = ".xml";

	protected string localUriPrefix = string.Empty;

	protected string assetBundlePrefix = string.Empty;

	protected Queue<GameDataLoadRequest> requests;

	protected Queue<CachedGameData> responsesReady;

	protected Dictionary<string, StaticDataDefinition> cachedData;

	protected Dictionary<string, CachedGameData> pendingData;

	protected TransactionMonitor startTransaction;

	protected AssetBundle generalBundle;

	protected Status statusBundle;

	protected int foo;

	public void Awake()
	{
		CspUtils.DebugLog("GameDataManager Awake() called!");
		startTransaction = TransactionMonitor.CreateTransactionMonitor("GameDataManager_startTransaction", OnLoadingComplete, 120f, null);
		startTransaction.AddStep("general");
		statusBundle = Status.Loading;
		generalBundle = null;
		requests = new Queue<GameDataLoadRequest>();
		responsesReady = new Queue<CachedGameData>();
		cachedData = new Dictionary<string, StaticDataDefinition>();
		pendingData = new Dictionary<string, CachedGameData>();
		localUriPrefix = "localfile$GameData/";
		assetBundlePrefix = "bundle$";
	}

	protected void OnGetCharacterData(ShsWebResponse response)
	{
		CspUtils.DebugLog("OnGetCharacterData: " + response.Body);
		string[] responseBody = response.Body.Split('|');
		string[] fileBytesArray = responseBody[0].Split(',');
		string [] filePaths = responseBody[1].Split(',');
		for (int index=0; index < fileBytesArray.Length; index++)
		{
			if (fileBytesArray[index] != "")
			{
				byte[] fileBytes = Convert.FromBase64String(fileBytesArray[index]);
				string filePath = Path.Combine(Application.dataPath, filePaths[index]);
				File.WriteAllBytes(filePath, fileBytes);
			}
		}
	}

	public void Start()
	{
		if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
		{
			startTransaction.CompleteStep("general");
			return;
		}
		startTransaction.AddStepBundle("general", "Data/general");
		AppShell.Instance.WebService.StartRequest("resources$data/json/update_files.py", OnGetCharacterData,null,ShsWebService.ShsWebServiceType.RASP); // Titan
		AppShell.Instance.BundleLoader.FetchAssetBundle("Data/general", OnAssetBundleLoaded, null, false);
	}

	protected void OnAssetBundleLoaded(AssetBundleLoadResponse response, object extraData)
	{
		if (string.IsNullOrEmpty(response.Error))
		{
			generalBundle = response.Bundle;
			if (startTransaction != null)
			{
				startTransaction.CompleteStep("general");
			}
		}
		else
		{
			CspUtils.DebugLog("Failed to load asset bundle <Data/general> with error " + response.Error);
			if (startTransaction != null)
			{
				startTransaction.FailStep("general", response.Error);
			}
		}
	}

	protected void OnLoadingComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		if (exit == TransactionMonitor.ExitCondition.Success)
		{
			statusBundle = Status.Ready;
		}
		else
		{
			statusBundle = Status.Error;
		}
		startTransaction = null;
	}

	public void Update()
	{
		//Discarded unreachable code: IL_022e
		if (startTransaction != null)
		{
			startTransaction.Update();
			return;
		}
		if (statusBundle == Status.Error)
		{
			GameDataLoadResponse gameDataLoadResponse = new GameDataLoadResponse();
			while (requests.Count > 0)
			{
				GameDataLoadRequest gameDataLoadRequest = requests.Dequeue();
				if (gameDataLoadRequest.callback != null)
				{
					gameDataLoadResponse.Path = gameDataLoadRequest.requestUri;
					gameDataLoadResponse.Error = "The requested data at <" + gameDataLoadResponse.Path + "> could not be loaded.";
					gameDataLoadRequest.callback(gameDataLoadResponse, gameDataLoadRequest.extraData);
				}
			}
			return;
		}
		while (requests.Count > 0)
		{
			GameDataLoadRequest gameDataLoadRequest2 = requests.Dequeue();
			ProcessRequest(gameDataLoadRequest2.requestUri, gameDataLoadRequest2.callback, gameDataLoadRequest2.dataDef, gameDataLoadRequest2.extraData);
		}
		CachedGameData cachedGameData;
		GameDataLoadResponse gameDataLoadResponse2;
		while (true)
		{
			if (responsesReady.Count <= 0)
			{
				return;
			}
			cachedGameData = responsesReady.Dequeue();
			if (!pendingData.Remove(cachedGameData.Path))
			{
				CspUtils.DebugLog("The request <" + cachedGameData.Path + "> was not found in the pending list");
			}
			gameDataLoadResponse2 = new GameDataLoadResponse();
			if (cachedGameData.OutstandingCallbacks.Count > 0 && cachedGameData.OutstandingCallbacks.Peek().ExtraData != null && cachedGameData.OutstandingCallbacks.Peek().ExtraData.ToString() == "JSON")
			{
				break;
			}
			if (cachedGameData.TimeLoaded >= 0f && cachedGameData.DataText != null)
			{
				XPathDocument xPathDocument = null;
				if (cachedGameData.needsDataWarehouse)
				{
					try
					{
						xPathDocument = new XPathDocument(new StringReader(cachedGameData.DataText));
						gameDataLoadResponse2.Data = new DataWarehouse(xPathDocument.CreateNavigator());
					}
					catch (Exception)
					{
						CspUtils.DebugLog(cachedGameData.Path);
						CspUtils.DebugLog("ERROR:" + cachedGameData.DataText);
						throw;
					}
				}
				gameDataLoadResponse2.Path = cachedGameData.Path;
				gameDataLoadResponse2.Error = null;
				gameDataLoadResponse2.DataDefinition = cachedGameData.DataDefinition;
				if (cachedGameData.DataDefinition != null)
				{
					bool flag = false;
					IStaticDataDefinition staticDataDefinition = cachedGameData.DataDefinition as IStaticDataDefinition;
					if (staticDataDefinition != null)
					{
						flag = true;
						staticDataDefinition.InitializeFromData(new DataWarehouse(xPathDocument.CreateNavigator()));
					}
					IStaticDataDefinitionTxt staticDataDefinitionTxt = cachedGameData.DataDefinition as IStaticDataDefinitionTxt;
					if (staticDataDefinitionTxt != null)
					{
						flag = true;
						staticDataDefinitionTxt.InitializeFromData(cachedGameData.DataText);
					}
					if (!flag)
					{
						CspUtils.DebugLog(cachedGameData.DataDefinition.GetType().ToString() + " is missing a IStaticDataDefinition or IStaticDataDefinitionTxt inteface!");
					}
					cachedData[cachedGameData.Path] = cachedGameData.DataDefinition;
				}
			}
			else
			{
				gameDataLoadResponse2.Path = cachedGameData.Path;
				gameDataLoadResponse2.Error = "The requested data at <" + gameDataLoadResponse2.Path + "> could not be loaded.";
				gameDataLoadResponse2.Data = null;
				gameDataLoadResponse2.DataDefinition = null;
			}
			while (cachedGameData.OutstandingCallbacks.Count > 0)
			{
				GameDataCallbackWithData gameDataCallbackWithData = cachedGameData.OutstandingCallbacks.Dequeue();
				gameDataCallbackWithData.Callback(gameDataLoadResponse2, gameDataCallbackWithData.ExtraData);
			}
		}
		while (cachedGameData.OutstandingCallbacks.Count > 0)
		{
			GameDataCallbackWithData gameDataCallbackWithData2 = cachedGameData.OutstandingCallbacks.Dequeue();
			gameDataCallbackWithData2.ExtraData = cachedGameData.DataText;
			gameDataCallbackWithData2.Callback(gameDataLoadResponse2, gameDataCallbackWithData2.ExtraData);
		}
	}

	public void LoadGameData(string requestUri, GameDataLoadedCallback callback)
	{
		LoadGameData(requestUri, callback, null, null);
	}

	public void LoadGameData(string requestUri, GameDataLoadedCallback callback, object extraData)
	{
		LoadGameData(requestUri, callback, null, extraData);
	}

	public void LoadGameData(string requestUri, GameDataLoadedCallback callback, StaticDataDefinition dataDef)
	{
		LoadGameData(requestUri, callback, dataDef, null);
	}

	public void LoadGameData(string requestUri, GameDataLoadedCallback callback, StaticDataDefinition dataDef, object extraData)
	{
		CspUtils.DebugLog("requestUri=" + requestUri);
		CspUtils.DebugLog("callback=" + callback);
		CspUtils.DebugLog("dataDef=" + dataDef);
		CspUtils.DebugLog("extraData=" + extraData);
		GameDataLoadRequest gameDataLoadRequest = new GameDataLoadRequest();
		gameDataLoadRequest.requestUri = requestUri;
		gameDataLoadRequest.callback = callback;
		gameDataLoadRequest.dataDef = dataDef;
		gameDataLoadRequest.extraData = extraData;
		requests.Enqueue(gameDataLoadRequest);
	}

	// public UnityEngine.Object CspLoad(AssetBundle bundle, string assetName) {
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

	protected void ProcessRequest(string requestUri, GameDataLoadedCallback callback, StaticDataDefinition dataDef, object extraData)
	{
		bool flag = false;
		string empty = string.Empty;
		string text = string.Empty;
		if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
		{
			flag = false;
			empty = localUriPrefix + requestUri + ".xml";
		}
		else
		{
			flag = true;
			text = Path.GetFileNameWithoutExtension(requestUri);
			empty = assetBundlePrefix + text + ".xml";
		}
		CachedGameData value = null;
		if (pendingData.TryGetValue(empty, out value))
		{
			value.OutstandingCallbacks.Enqueue(new GameDataCallbackWithData(callback, extraData));
			if (value.DataDefinition == null)
			{
				value.DataDefinition = dataDef;
			}
			if (dataDef == null)
			{
				value.needsDataWarehouse = true;
			}
			else if (dataDef is IStaticDataDefinition)
			{
				value.needsDataWarehouse = true;
			}
			return;
		}
		if (dataDef != null)
		{
			StaticDataDefinition value2 = null;
			if (cachedData.TryGetValue(empty, out value2))
			{
				GameDataLoadResponse gameDataLoadResponse = new GameDataLoadResponse();
				gameDataLoadResponse.Path = empty;
				gameDataLoadResponse.Error = null;
				gameDataLoadResponse.Data = null;
				gameDataLoadResponse.DataDefinition = value2;
				callback(gameDataLoadResponse, extraData);
				return;
			}
		}
		if (flag)
		{
			//UnityEngine.Object @object = generalBundle.Load(text);
			UnityEngine.Object @object = CspUtils.CspLoad(generalBundle,text);
			if (@object == null)
			{
				CachedGameData cachedGameData = new CachedGameData();
				cachedGameData.Path = empty;
				cachedGameData.TimeLoaded = -1f;
				cachedGameData.OutstandingCallbacks.Enqueue(new GameDataCallbackWithData(callback, extraData));
				pendingData[empty] = cachedGameData;
				responsesReady.Enqueue(cachedGameData);
				return;
			}
			CachedGameData cachedGameData2 = new CachedGameData();
			cachedGameData2.Path = empty;
			cachedGameData2.DataText = ((TextAsset)@object).text;
			cachedGameData2.TimeLoaded = Time.time;
			cachedGameData2.DataDefinition = dataDef;
			cachedGameData2.OutstandingCallbacks.Enqueue(new GameDataCallbackWithData(callback, extraData));
			if (dataDef == null)
			{
				cachedGameData2.needsDataWarehouse = true;
			}
			else if (dataDef is IStaticDataDefinition)
			{
				cachedGameData2.needsDataWarehouse = true;
			}
			pendingData[empty] = cachedGameData2;
			responsesReady.Enqueue(cachedGameData2);
		}
		else
		{
			CachedGameData cachedGameData3 = new CachedGameData();
			cachedGameData3.Path = empty;
			cachedGameData3.DataDefinition = dataDef;
			cachedGameData3.OutstandingCallbacks.Enqueue(new GameDataCallbackWithData(callback, extraData));
			if (dataDef == null)
			{
				cachedGameData3.needsDataWarehouse = true;
			}
			else if (dataDef is IStaticDataDefinition)
			{
				cachedGameData3.needsDataWarehouse = true;
			}
			pendingData[empty] = cachedGameData3;
			AppShell.Instance.WebService.StartRequest(empty, OnGameDataLoaded, ShsWebService.ShsWebServiceType.Text);
		}
	}

	public void OnGameDataLoaded(ShsWebResponse response)
	{
		CachedGameData value = null;
		if (!pendingData.TryGetValue(response.OriginalUri, out value))
		{
			CspUtils.DebugLog("Unable to find a pending entry for this request <" + response.OriginalUri + ">.  The request cannot be fulfilled!");
			return;
		}
		string body = response.Body;
		if (body != null && body != string.Empty)
		{
			value.DataText = body;
			value.TimeLoaded = Time.time;
		}
		else
		{
			if (body != null)
			{
				CspUtils.DebugLog(response.RequestUri + " : The returned game data was an empty string.  The request failed.");
			}
			else
			{
				CspUtils.DebugLog(response.RequestUri + " : The returned game data was not a TextAsset.  The request failed.");
			}
			value.TimeLoaded = -1f;
		}
		responsesReady.Enqueue(value);
	}

	public void ClearGameDataCache()
	{
		CspUtils.DebugLog("You are clearing the game data cache!  This should only be used in editor-only circumstances where changes have been made to existing assets");
		cachedData.Clear();
	}
}
