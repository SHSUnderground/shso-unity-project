using LitJson;
using SmartFoxClientAPI;
using SmartFoxClientAPI.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
	using UnityEditor;
#endif


public class RTCClient : IDisposable
{
	public delegate void OnConnectDelegate(bool success, string error);

	public delegate void OnMessageDelegate(NetworkMessage msg);

	public delegate void OnUserEnterDelegate(int roomId, int userId);

	public delegate void OnUserLeaveDelegate(int roomId, int userId);

	public delegate void OnDisconnectedDelegate();

	public delegate void OnSessionCreatedDelegate(Hashtable msg);

	private const float LAST_LEVEL_THRESHOLD = 5f;

	protected bool disposed;

	private SmartFoxClient api;

	private int playerId;

	private string sik;

	private string zone;

	private bool isNotification;

	private string ticket;

	private bool connectReported;

	private float keepAlive;

	private float nextPing;

	private bool waitingForPingResponse;

	private bool connectionProblem;

	private TransactionMonitor loginTransaction;

	private RTCServerInfo serverInfo;

	static public int paid = 0;  // CSP

	public bool IsConnected
	{
		get
		{
			return api.IsConnected();
		}
	}

	public int UserId
	{
		get
		{
			return api.myUserId;
		}
	}

	public int PlayerId
	{
		get
		{
			return playerId;
		}
	}

	public string Sik
	{
		get
		{
			return sik;
		}
		set
		{
			sik = value;
		}
	}

	public RTCServerInfo ServerInfo
	{
		get
		{
			return serverInfo;
		}
	}

	public string ServerFullName
	{
		get
		{
			return (serverInfo != null) ? serverInfo.ToString() : null;
		}
	}

	public string ObfuscatedName
	{
		get
		{
			return ServerInfo.HostName + "/" + ServerInfo.Port;
		}
	}

	public event OnConnectDelegate onConnect;

	public event OnMessageDelegate onMessage;

	public event OnUserEnterDelegate onUserEnter;

	public event OnUserLeaveDelegate onUserLeave;

	public event OnDisconnectedDelegate onDisconnect;

	public event OnSessionCreatedDelegate onSessionCreated;
	private string username;
	private string password;

	private string sfKey = null;  // CSP
	bool successCSP;  // CSP
	string errorCSP;  // CSP

	public RTCClient()
	{
		disposed = false;
		loginTransaction = null;
		sik = null;
		ticket = null;
		connectReported = false;
		keepAlive = 0f;
		nextPing = 5f;
		this.onConnect = null;
		this.onMessage = null;
		this.onUserEnter = null;
		this.onUserLeave = null;
		this.onDisconnect = null;
		api = new SmartFoxClient();
		api.runInQueueMode = true;
		api.smartConnect = false;
		api.debug = true;    // CSP false;
		AddEventHandlers();
	}

	protected void OnConnect(bool success, string error)
	{
		//CspUtils.DebugLog("OnConnect: " + connectReported);

		if (!connectReported) 
		{
			connectReported = true;
			if (this.onConnect != null)
			{
				this.onConnect(success, error);
			}
		}
	}

	protected void OnMessage(NetworkMessage msg)
	{
		if (this.onMessage != null)
		{
			this.onMessage(msg);
		}
	}

	private static void OnAdminMessage(string message)
	{
		if (!string.IsNullOrEmpty(message) && !(message == "null"))
		{
			AppShell.Instance.EventMgr.Fire(AppShell.Instance, new UserAdminMessage(message));
		}
	}

	private static void OnSFDebugMessage(string message)
	{
		CspUtils.DebugLog("SF: " + message);
	}

	protected void OnUserEnter(int roomId, int userId)
	{
		if (this.onUserEnter != null)
		{
			this.onUserEnter(roomId, userId);
		}
		GameController controller = GameController.GetController();
		if (controller is SocialSpaceController)
		{
			if (userId != AppShell.Instance.ServerConnection.GetGameUserId() && PetDataManager.getCurrentPet() != 0)
			{
				SetPetMessage setPetMessage = new SetPetMessage(new GoNetId(GoNetId.PLAYER_ID_FLAG, AppShell.Instance.ServerConnection.GetGameUserId()));
				setPetMessage.petID = PetDataManager.getCurrentPet();
				AppShell.Instance.ServerConnection.SendGameMsg(setPetMessage, userId);
			}
			if (userId != AppShell.Instance.ServerConnection.GetGameUserId() && (TitleManager.currentTitleID != -1 || TitleManager.currentMedallionID != -1))
			{
				SetTitleMessage setTitleMessage = new SetTitleMessage(new GoNetId(GoNetId.PLAYER_ID_FLAG, AppShell.Instance.ServerConnection.GetGameUserId()));
				setTitleMessage.titleID = TitleManager.currentTitleID;
				setTitleMessage.medallionID = TitleManager.currentMedallionID;
				AppShell.Instance.ServerConnection.SendGameMsg(setTitleMessage, userId);
			}
			if (userId != AppShell.Instance.ServerConnection.GetGameUserId() && AppShell.Instance.Profile.AvailableCostumes[AppShell.Instance.Profile.SelectedCostume].Level == StatLevelReqsDefinition.MAX_HERO_LEVEL_BADGE2)
			{
				CspUtils.DebugLog("OnUserEnter prestige");
				SpawnPrestigeMessage msg = new SpawnPrestigeMessage(new GoNetId(GoNetId.PLAYER_ID_FLAG, AppShell.Instance.ServerConnection.GetGameUserId()));
				AppShell.Instance.ServerConnection.SendGameMsg(msg, userId);
			}
		}
	}

	protected void OnUserLeave(int roomId, int userId)
	{
		if (this.onUserLeave != null)
		{
			this.onUserLeave(roomId, userId);
		}
	}

	protected void OnDisconnect()
	{
		if (this.onDisconnect != null)
		{
			this.onDisconnect();
		}
	}

	protected void OnSessionCreated(Hashtable msg)
	{
		if (this.onSessionCreated != null)
		{
			this.onSessionCreated(msg);
		}
	}

	~RTCClient()
	{
		Dispose(false);
	}

	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (disposing)
			{
				RemoveEventHandlers();
				api.Disconnect();
				api.Dispose();
			}
			disposed = true;
			api = null;
			loginTransaction = null;
			sik = null;
			ticket = null;
			this.onConnect = null;
			this.onMessage = null;
			this.onUserEnter = null;
			this.onUserLeave = null;
			this.onDisconnect = null;
		}
	}

	public void Update()
	{
		while (api != null && api.NumEventsInEventQueue() > 0)
		{
			api.ProcessSingleEventInEventQueue();
		}
		if (loginTransaction != null)
		{
			loginTransaction.Update();
		}
		if (api != null && api.IsConnected())
		{
			if (keepAlive <= 0f)
			{
				SendXtMessage("basic", "keepAlive", null, "str");
				keepAlive = 25f;
			}
			else
			{
				keepAlive -= Time.deltaTime;
			}
		}
		if (loginTransaction == null && api != null && api.IsConnected() && !isNotification)
		{
			if (nextPing <= 0f)
			{
				PingServer();
				nextPing = 5f;
			}
			else
			{
				nextPing -= Time.deltaTime;
			}
		}
	}

	public void Connect(string server, string zoneName, string userTicket, string username, string password)
	{
		zone = zoneName;
		isNotification = (zoneName == "shs.not" || zoneName == "shs.notification");
		ticket = userTicket;
		loginTransaction = TransactionMonitor.CreateTransactionMonitor("RTCClient_loginTransaction", OnLoginTransactionComplete, 30f, null);
		loginTransaction.AddStep("connect");
		loginTransaction.AddStep("login");
		loginTransaction.AddStep("seated");
		this.username = username;
		this.password = password;
		if (!isNotification)
		{
			loginTransaction.AddStep("time");
		}
		connectReported = false;
		if (!isNotification)
		{
			ServerTime.Instance.Reset();
		}
		serverInfo = RTCServerInfo.Parse(server);
		if (serverInfo == null)
		{
			OnConnect(false, "Unable to parse server info <" + server + ">");
			return;
		}
		IPAddress iPAddress = NameToIpResolve(serverInfo.Name);
		if (iPAddress == null)
		{
			OnConnect(false, "Unable to resolve server name <" + serverInfo.Name + ">");
			return;
		}
		if (Application.webSecurityEnabled)
		{
			
			CspUtils.DebugLog("Requesting security policy from " + iPAddress + ":" + serverInfo.Port);
			if (!Security.PrefetchSocketPolicy(iPAddress.ToString(), serverInfo.Port))
			{
				CspUtils.DebugLog("Failed to prefetch socket policy!  The odds of this login being successful are very small!");
			}
		}
		api.Connect(iPAddress.ToString(), serverInfo.Port);
	}

	public void Disconnect()
	{
		OnConnect(false, "Canceled");
		api.Disconnect();
	}

	// // this method added by CSP, as the code for initiating the finding of a game room could not be found. 
	// public void GetGameRoom(int roomType, bool isHost)
	// {
	// 	ArrayList arrayList = new ArrayList(2);
	// 	arrayList.Add(roomType.ToString());
	// 	arrayList.Add(isHost.ToString());
				
	// 	api.SendXtMessage("escrow", "getgameroom", arrayList, "str");  // need to implement in smartfox script...
	// }

	protected void SendXtMessage(string xtName, string cmd, ICollection paramObj, string type)
	{
		api.SendXtMessage(xtName, cmd, paramObj, type);
	}

	public void SendToAll(NetworkMessage msg)
	{
		string value = NetworkMessage.ToEncodedString(msg);
		SFSObject sFSObject = new SFSObject();
		sFSObject.Put("_d", value);
		api.SendObject(sFSObject, api.activeRoomId);
	}

	public void SendToUser(NetworkMessage msg, int userRTCId)
	{
		ArrayList arrayList = new ArrayList();
		arrayList.Add(userRTCId);
		string value = NetworkMessage.ToEncodedString(msg);
		SFSObject sFSObject = new SFSObject();
		sFSObject.Put("_d", value);
		api.SendObjectToGroup(sFSObject, arrayList, api.activeRoomId);
	}

	public void PingServer()
	{
		if (waitingForPingResponse)
		{
			connectionProblem = true;
			AppShell.Instance.EventMgr.Fire(null, new NetworkConnectionProblem(null, false));
		}
		ArrayList arrayList = new ArrayList(1);
		string timeStr = ServerTime.Instance.GetClientTimeInMilliseconds().ToString();
		arrayList.Add(timeStr);
		//CspUtils.DebugLog("timeStr=" + timeStr);
				
		api.SendXtMessage("basic", "ping", arrayList, "str");
		waitingForPingResponse = true;
	}

	public void SendHeroCreate(string hero, string key, string data)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("hero", hero);
		CspUtils.DebugLog("SendHeroCreate hero=" + hero);
		if (string.IsNullOrEmpty(key))
		{
			CspUtils.DebugLog("SERVER: sending hero_create for hero '" + hero + "' but with null/empty key, unlikely to work!");
			return;
		}
		hashtable.Add("key", key);
		hashtable.Add("blob", data);
		hashtable.Add("shsoID", playerId);  // added by CSP
		SendXtMessage("escrow", "hero_create", hashtable, "xml");
	}

	public void SendHeroSync(int userRTCId, string hero, string key, string data)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("tuid", userRTCId.ToString());
		hashtable.Add("hero", hero);
		hashtable.Add("key", key);
		hashtable.Add("blob", data);
		SendXtMessage("escrow", "hero_sync", hashtable, "xml");
	}

	public int GetUserCount()
	{
		return api.GetActiveRoom().GetUserList().Count;
	}

	public int[] GetAllUserIds()
	{
		Hashtable userList = api.GetActiveRoom().GetUserList();
		if (userList == null || userList.Keys.Count == 0)
		{
			CspUtils.DebugLog("SERVER: RTC returned null room list");
			return new int[0];
		}
		int[] array = new int[userList.Keys.Count];
		userList.Keys.CopyTo(array, 0);
		return array;
	}

	public int GetRoomId()
	{
		return api.GetActiveRoom().GetId();
	}

	public List<NetworkManager.UserInfo> GetAllUsers()
	{
		Hashtable userList = api.GetActiveRoom().GetUserList();
		//////// block added by CSP //////////////////////
		Room r = api.GetActiveRoom();
		CspUtils.DebugLog("GetAllUsers room name=" + r.GetName());
		/////////////////////////////////////////////
		List<NetworkManager.UserInfo> list = new List<NetworkManager.UserInfo>(userList.Keys.Count);
		foreach (int key in userList.Keys)
		{
			NetworkManager.UserInfo item = new NetworkManager.UserInfo(key, GetUserName(key));
			CspUtils.DebugLog("GetAllUsers userkey= " + key + " username=" + GetUserName(key));  // added by CSP for testing
			list.Add(item);
		}
		return list;
	}

	public string GetUserName(int userId)
	{
		PlayerDictionary.Player value;
		CspUtils.DebugLog("GetUserName " + userId);
		AppShell.Instance.PlayerDictionary.TryGetValue(userId, out value);
		if (value != null)
		{
			return value.Name;
		}
		return "<unknown>";
	}

	public object GetUserVariable(int userId, string name)
	{
		if (name == "ShsId" || name == "ShsName" || name == "isSubscriber")
		{
			CspUtils.DebugLog("SERVER: !! Requested UserVariable that should NOT be in UserVariables any more: " + name);
			return null;
		}
		Room activeRoom = api.GetActiveRoom();
		if (activeRoom != null)
		{
			User user = activeRoom.GetUser(userId);
			if (user != null)
			{
				return user.GetVariables()[name];
			}
		}
		return null;
	}

	private User GetUser(int userId)
	{
		Room activeRoom = api.GetActiveRoom();
		if (activeRoom == null)
		{
			CspUtils.DebugLog("active room was null.");
			return null;
		}
		return activeRoom.GetUser(userId);
	}

	public string[] GetUserVariables(int userId)
	{
		Hashtable userVariablesDict = GetUserVariablesDict(userId);
		string[] array = new string[userVariablesDict.Count];
		userVariablesDict.Keys.CopyTo(array, 0);
		return array;
	}

	public Hashtable GetUserVariablesDict(int userId)
	{
		User user = GetUser(userId);
		return (user != null) ? user.GetVariables() : null;
	}

	public void SetUserVariable(string name, int value)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[name] = value;
		api.SetUserVariables(hashtable);
	}

	public void SetUserVariable(string name, bool value)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[name] = value;
		CspUtils.DebugLog("SetUserVariable playerID=" + playerId + " userName=" + api.myUserName + " userid=" + api.myUserId + " name=" + name + " value = " + value);  // CSP
		api.SetUserVariables(hashtable);

		object o =  (GetUserVariable(api.myUserId, name));
		if (o != null)
		{ 	//Hashtable ht = (Hashtable) o;
			//foreach (DictionaryEntry entry in ht)
   				//CspUtils.DebugLog("key: " + entry.Key + " value:" + entry.Value);

			CspUtils.DebugLog("type=" + o.GetType().ToString() + " ToString()=" + o.ToString());
		}
		else
			CspUtils.DebugLog("GetUserVariable() returned null!"); 
	}

	public void SetUserVariable(string name, string value)
	{
		Hashtable hashtable = new Hashtable();
		hashtable[name] = value;
		api.SetUserVariables(hashtable);
	}

	public bool GetRoomIsActive()
	{
		if (api.GetActiveRoom() == null)
		{
			return false;
		}
		return true;
	}

	public string GetRoomName()
	{
		Room activeRoom = api.GetActiveRoom();
		if (activeRoom != null)
		{
			return activeRoom.GetName();
		}
		CspUtils.DebugLog("SERVER: No active room when asked for the room name.");
		return "not_connected";
	}

	public int GetRoomMaxUsers()
	{
		Room activeRoom = api.GetActiveRoom();
		if (activeRoom != null)
		{
			return activeRoom.GetMaxUsers();
		}
		CspUtils.DebugLog("SERVER: No active room when asked for the room name.");
		return 0;
	}

	public object GetRoomVariable(string name)
	{
		Room activeRoom = api.GetActiveRoom();
		if (activeRoom != null)
		{
			Hashtable variables = activeRoom.GetVariables();
			return variables[name];
		}
		return null;
	}

	public void SetRoomVariable(string name, object data)
	{
		RoomVariable value = new RoomVariable(name, data);
		ArrayList arrayList = new ArrayList();
		arrayList.Add(value);
		api.SetRoomVariables(arrayList);
	}

	public void LockRoom()
	{
		Room activeRoom = api.GetActiveRoom();
		if (activeRoom != null)
		{
			ArrayList arrayList = new ArrayList(1);
			arrayList.Add(activeRoom.GetName());
			SendXtMessage("maitred", "closeRoom", arrayList, "str");
			AppShell.Instance.Matchmaker2.LockBrawler(-1);
		}
	}

	public void TakeOwnership(GoNetId goNetId, bool autoTransfer)
	{
		string value = goNetId.ToBase64();
		ArrayList arrayList = new ArrayList(1);
		arrayList.Add(value);
		if (autoTransfer)
		{
			SendXtMessage("escrow", "tO", arrayList, "str");
		}
		else
		{
			SendXtMessage("escrow", "ntO", arrayList, "str");
		}
	}

	public void TransferOwnership(int newOwnerId, GoNetId goNetId)
	{
		string value = goNetId.ToBase64();
		ArrayList arrayList = new ArrayList(2);
		arrayList.Add(newOwnerId);
		arrayList.Add(value);
		SendXtMessage("escrow", "gO", arrayList, "str");
	}

	public void ReleaseOwnership(GoNetId goNetId)
	{
		string value = goNetId.ToBase64();
		ArrayList arrayList = new ArrayList(1);
		arrayList.Add(value);
		SendXtMessage("escrow", "rO", arrayList, "str");
	}

	public void QueryOwnership(GoNetId goNetId)
	{
		string value = goNetId.ToBase64();
		ArrayList arrayList = new ArrayList(1);
		arrayList.Add(value);
		SendXtMessage("escrow", "qO", arrayList, "str");
	}

	public void TakeOwnership(string str, bool autoTransfer)
	{
		ArrayList arrayList = new ArrayList(1);
		arrayList.Add("." + str);
		CspUtils.DebugLog("autoTransfer=" + autoTransfer);
		CspUtils.DebugLog("str=" + str);
		if (autoTransfer)
		{
			SendXtMessage("escrow", "tO", arrayList, "str");
		}
		else
		{
			SendXtMessage("escrow", "ntO", arrayList, "str");
		}
	}

	public void TransferOwnership(int newOwnerId, string str)
	{
		ArrayList arrayList = new ArrayList(2);
		arrayList.Add(newOwnerId);
		arrayList.Add("." + str);
		SendXtMessage("escrow", "gO", arrayList, "str");
	}

	public void ReleaseOwnership(string str)
	{
		ArrayList arrayList = new ArrayList(1);
		arrayList.Add("." + str);
		SendXtMessage("escrow", "rO", arrayList, "str");
	}

	public void QueryOwnership(string str)
	{
		ArrayList arrayList = new ArrayList(1);
		arrayList.Add("." + str);
		SendXtMessage("escrow", "qO", arrayList, "str");
	}

	public void QueryAllOwnership()
	{
		ArrayList paramObj = new ArrayList(0);
		SendXtMessage("escrow", "qO", paramObj, "str");
	}

	public void ResetAllOwnership()
	{
		ArrayList paramObj = new ArrayList(0);
		SendXtMessage("escrow", "cO", paramObj, "str");
	}

	public void ReportEvent(string evtName, Hashtable args)
	{
		SendXtMessage("events", evtName, args, "xml");
	}

	public void ReportGameWorldEvent(string evtName, Hashtable args)
	{
		SendXtMessage("escrow", evtName, args, "xml");
	}

	public void ReportBrawlerEvent(string evtName, Hashtable args)
	{
		CspUtils.DebugLog("ReportBrawlerEvent evtName=" + evtName);
		SendXtMessage("escrow", evtName, args, "xml");
	}

	public void ReportChatEvent(string evtName, Hashtable args)
	{
		CspUtils.DebugLog("ReportChatEvent chat " + evtName);
		CspUtils.DebugLog("ReportChatEvent isNotification= " + isNotification);
		// !!!!!!!!! log output was : ReportChatEvent isNotification= True
		//   so chat extension needs to be in notification server, not game server
		
		//SendXtMessage("escrow", "testit", args, "xml");  // CSP test
		//SendXtMessage("chat", "testit", args, "xml");	 // CSP test
		SendXtMessage("chat", evtName, args, "xml");
	}

	public void GetFirstPlayer(int Player)
	{
		ArrayList arrayList = new ArrayList(1);
		arrayList.Add(Player.ToString());
		SendXtMessage("cardgame", "getFirstPlayer", arrayList, "str");
	}

	public void FakePlayer(int Player)
	{
		ArrayList arrayList = new ArrayList(1);
		arrayList.Add(Player.ToString());
		SendXtMessage("cardgame", "fakePlayer", arrayList, "str");
	}

	public void PickHeroDeck(int Player, string HeroName, string DeckRecipe, int DeckId)
	{
		ArrayList arrayList = new ArrayList(4);
		arrayList.Add(Player.ToString());
		arrayList.Add(HeroName);
		arrayList.Add(DeckRecipe);
		arrayList.Add(DeckId.ToString());
		SendXtMessage("cardgame", "pickHeroDeck", arrayList, "str");
	}

	public void Pass(int Player)
	{
		ArrayList arrayList = new ArrayList(1);
		arrayList.Add(Player.ToString());
		SendXtMessage("cardgame", "pass", arrayList, "str");
	}

	public void TossCoin(int Player)
	{
		ArrayList arrayList = new ArrayList(1);
		arrayList.Add(Player.ToString());
		SendXtMessage("cardgame", "tossCoin", arrayList, "str");
	}

	public void PowerUp(int Player, int Increment)
	{
		ArrayList arrayList = new ArrayList(2);
		arrayList.Add(Player.ToString());
		arrayList.Add(Increment.ToString());
		SendXtMessage("cardgame", "powerUp", arrayList, "str");
	}

	public void ShuffleCards(int Player, int Src)
	{
		ArrayList arrayList = new ArrayList(2);
		arrayList.Add(Player.ToString());
		arrayList.Add(Src.ToString());
		SendXtMessage("cardgame", "shuffleCards", arrayList, "str");
	}

	public void PickRandomCard(int Player, int Src)
	{
		ArrayList arrayList = new ArrayList(2);
		arrayList.Add(Player.ToString());
		arrayList.Add(Src.ToString());
		SendXtMessage("cardgame", "pickRandomCard", arrayList, "str");
	}

	public void TransferCards(int Player, int Src, int Dest, int Start, int Count, bool Individually, int NewFacing)
	{
		ArrayList arrayList = new ArrayList(7);
		arrayList.Add(Player.ToString());
		arrayList.Add(Src.ToString());
		arrayList.Add(Dest.ToString());
		arrayList.Add(Start.ToString());
		arrayList.Add(Count.ToString());
		arrayList.Add(Individually.ToString());
		arrayList.Add(NewFacing.ToString());
		SendXtMessage("cardgame", "transferCards", arrayList, "str");
	}

	public void PeekCards(int Player, int Src, int Start, int Count)
	{
		ArrayList arrayList = new ArrayList(4);
		arrayList.Add(Player.ToString());
		arrayList.Add(Src.ToString());
		arrayList.Add(Start.ToString());
		arrayList.Add(Count.ToString());
		SendXtMessage("cardgame", "peekCards", arrayList, "str");
	}

	public void ReportCheat()
	{
	}

	public void EndGame(int WinnerId)
	{
		ArrayList arrayList = new ArrayList(1);
		arrayList.Add(WinnerId.ToString());
		SendXtMessage("cardgame", "endGame", arrayList, "str");
	}

	public void PickCard(int Player, int Src, int Dest, int CardIndex, string CardId, int NewFacing)
	{
		ArrayList arrayList = new ArrayList(6);
		arrayList.Add(Player.ToString());
		arrayList.Add(Src.ToString());
		arrayList.Add(Dest.ToString());
		arrayList.Add(CardIndex.ToString());
		arrayList.Add(CardId);
		arrayList.Add(NewFacing.ToString());
		SendXtMessage("cardgame", "pickCard", arrayList, "str");
	}

	public void PickEx(int Player, int PickType, int iData, string sData)
	{
		ArrayList arrayList = new ArrayList(4);
		arrayList.Add(Player.ToString());
		arrayList.Add(PickType.ToString());
		arrayList.Add(iData.ToString());
		arrayList.Add(sData);
		SendXtMessage("cardgame", "pickEx", arrayList, "str");
	}

	public void AllowEx(int Player, int PickType)
	{
		ArrayList arrayList = new ArrayList(2);
		arrayList.Add(Player.ToString());
		arrayList.Add(PickType.ToString());
		SendXtMessage("cardgame", "allowEx", arrayList, "str");
	}

	public void SendGameSAMessage(string message, ArrayList args)
	{
		SendXtMessage("cardgamesa", message, args, "str");
	}

	protected void OnLoginTransactionComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		loginTransaction = null;
		switch (exit)
		{
		case TransactionMonitor.ExitCondition.Success:
			OnConnect(true, string.Empty);
			break;
		case TransactionMonitor.ExitCondition.Fail:
		case TransactionMonitor.ExitCondition.TimedOut:
			OnConnect(false, error);
			break;
		}
	}

	// this method added by CSP
	void OnRandomKey(string key) {
		sfKey = key;
		CspUtils.DebugLog("sfKey = " + sfKey);

		this.OnSFConnect2(successCSP, errorCSP);
	}

	// this method added by CSP
	protected void OnSFConnect(bool success, string error) {
		CspUtils.DebugLog("OnSFConnect called");
		if (sfKey == null)  // CSP
                api.GetRandomKey();  // CSP
		successCSP = success;  // CSP
		errorCSP = error;  // CSP
	}

	protected void OnSFConnect2(bool success, string error)  // CSP changed method name from OnSFConnect to OnSFConnect2
	{
		if (success)
		{
			CspUtils.DebugLog("!!!!!!!!!!!!!!!!!!!!!!!!!!!!CONNECTED TO SMARTFOX SERVER!!!!!!!!!");

			if (loginTransaction != null)
			{
				loginTransaction.CompleteStep("connect");
			}
			string pass = (!isNotification) ? ticket : sik;

			CspUtils.DebugLog("!!!!!!!!!!!!!!!!!!!!!!!!!!!!ATTEMPT LOGIN TO ZONE: " + zone + ", username:" + username + ", password:" + password);			

			// Create an MD5 hash of the "secret" key + userName
			System.Security.Cryptography.MD5CryptoServiceProvider md = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] passowrd_hashed_bytes = md.ComputeHash(System.Text.Encoding.UTF8.GetBytes(this.password));
			string password_hashed = "";
			for (int i = 0; i < passowrd_hashed_bytes.Length; i++)
			{
				password_hashed += System.Convert.ToString(passowrd_hashed_bytes[i], 16).PadLeft(2, '0');
			}
            var bytes = System.Text.Encoding.UTF8.GetBytes(sfKey + password_hashed);
			byte[] hBytes = md.ComputeHash(bytes);
			// Convert the encrypted bytes back to a string (base 16)
			string hString = "";
			for (int i = 0; i < hBytes.Length; i++)
			{
				hString += System.Convert.ToString(hBytes[i], 16).PadLeft(2, '0');
			}
        	string md5 = hString.PadLeft(32, '0');
			string importantInfo = this.username + "," + CspUtils.version;
			api.Login(zone, importantInfo, md5); //  added by CSP, kludge to validate client/server version.
		}
		else if (loginTransaction != null)
		{
			CspUtils.DebugLog("!!!!!!!!!!!!!!!!!!!!!!!!!!!!FAILURE CONNECTING TO SMARTFOX SERVER!!!!!!!!!");
			loginTransaction.FailStep("connect", error);
		}
	}

	private void OnSFExtensionResponse(object data, string type)
	{
		//CspUtils.DebugLog("OnSFExtensionResponse type=" + type);

		switch (type)
		{
		case "str":
			HandleStringResponse(data as string);
			break;
		case "xml":
			HandleXMLResponse(data as SFSObject);
			break;
		}
	}

	protected void HandleStringResponse(string response)
	{
		string[] array = response.Split('%');
		CspUtils.DebugLog("HandleStringResponse arg=" + response);
		switch (array[0])
		{
		case "card":
			HandleCardGameStringResponse(array);
			break;
		case "cardsa":   // SA = string array?    CSP
		{
			string[] array3 = new string[array.Length - 2];
			Array.ConstrainedCopy(array, 2, array3, 0, array3.Length);
			AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.ServerMessage(array3));
			break;
		}
		case "own":
			HandleEscrowStringResponse(array);
			break;
		case "own_clear":
			HandleEscrowClear();
			break;
		case "ping":
			CspUtils.DebugLog("!!!!!!!!!! PING REPSONSE RECVD !!!!!!!!!!!");
			waitingForPingResponse = false;
			if (connectionProblem)
			{
				connectionProblem = false;
				AppShell.Instance.EventMgr.Fire(null, new NetworkConnectionProblem(null, true));
			}
			ServerTime.Instance.ProcessPingResult(array[2], array[3]);
			if (loginTransaction != null && !loginTransaction.IsStepCompleted("time"))
			{
				loginTransaction.CompleteStep("time");
			}
			break;
		case "cget":
			if (array[3] == "true")
			{
				CspUtils.DebugLog("Event report <" + array[2] + "> was accepted by the server!");
			}
			else
			{
				CspUtils.DebugLog("Event report <" + array[2] + "> was not accepted by the server!");
			}
			break;
		case "hero_create":
		{
			CspUtils.DebugLog("hero_create msg recvd from server! ");
			CspUtils.DebugLog(array[2]);
			CspUtils.DebugLog(array[3]);
			CspUtils.DebugLog(array[4]);
			
			if (array.Length == 5)
			{
				AppShell.Instance.ServerConnection.Game.OnHeroCreate(array[2], array[3], array[4]);
				break;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SERVER: hero_create received an unexpected number of args {0}", array.Length);
			stringBuilder.AppendLine();
			for (int j = 0; j < array.Length; j++)
			{
				stringBuilder.AppendFormat("  args[{0}] = {1}", j, array[j]);
				stringBuilder.AppendLine();
			}
			CspUtils.DebugLog(stringBuilder.ToString());
			break;
		}
		case "playerVars":
		{
			for (int i = 2; i < array.Length; i++)
			{
				CspUtils.DebugLog(i + " playerVars " + array[i]);
				string[] array2 = array[i].Split('|');
				int heroLevel = int.Parse(array2[4]);
				int squadLevel = int.Parse(array2[5]);
				string name = array2[2];  // DecodeString(array2[2]);   // CSP removed base 64 decoding.
				bool isModerator = bool.Parse(array2[3]);
				
				AppShell.Instance.PlayerDictionary.Update(new PlayerDictionary.Player(int.Parse(array2[0]), int.Parse(array2[1]), name, bool.Parse(array2[3]), isModerator, heroLevel, squadLevel));
			}
			break;
		}
		case "start_showing_effect":
		{
			int num2 = int.Parse(array[2]);
			string text3 = array[3];
			string text4 = array[4];
			CspUtils.DebugLog(string.Format("Start Showing Effect ID {0} for sfs user {1} effectData {2}", text3, num2, text4));
			AppShell.Instance.EventMgr.Fire(this, new StartExpendableEffectMessage(num2, text4, text3));
			break;
		}
		case "stop_showing_effect":
		{
			int num = int.Parse(array[2]);
			string text = array[3];
			string text2 = array[4];
			CspUtils.DebugLog(string.Format("Stop Showing Effect ID {0} for sfs user {1} effectData {2}", text, num, text2));
			AppShell.Instance.EventMgr.Fire(this, new StopExpendableEffectMessage(num, text2, text));
			break;
		}
		}
	}

	private static void HandleEscrowClear()
	{
		CspUtils.DebugLog("Clearing ownership state");
		OwnershipGoNetMessage msg = new OwnershipGoNetMessage(-3, null);
		AppShell.Instance.EventMgr.Fire(null, msg);
		OwnershipStringMessage msg2 = new OwnershipStringMessage(-3, null);
		AppShell.Instance.EventMgr.Fire(null, msg2);
	}

	private static void HandleEscrowStringResponse(string[] args)
	{
		try {
			CspUtils.DebugLog("args[2]=" + args[2]);
		}
		catch (Exception e) {			
		}

		for (int i = 2; i < args.Length; i++)
		{
			bool flag = false;
			int j;
			for (j = 0; j < args[i].Length; j++)
			{
				switch (args[i][j])
				{
				case '+':
				case '-':
				case '=':
					flag = true;
					break;
				}
				if (flag)
				{
					break;
				}
			}
			if (!flag)
			{
				continue;
			}
			int ownerId;
			try
			{
				ownerId = int.Parse(args[i].Substring(0, j));
			}
			catch
			{
				ownerId = -2;
			}
			if (args[i][j] == '-')
			{
				ownerId = -2;
			}
			string[] array = args[i].Substring(j + 1).Split('|');
			List<GoNetId> list = new List<GoNetId>();
			List<string> list2 = new List<string>();
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (text.Length > 0)   // CSP added this extra test for text length > 0
				{
					if (text[0] == '.')
					{
						string item = text.Substring(1);
						list2.Add(item);
					}
					else
					{
						GoNetId item2 = GoNetId.FromBase64(text);
						list.Add(item2);
					}
				}
			}
			if (list.Count > 0)
			{
				OwnershipGoNetMessage msg = new OwnershipGoNetMessage(ownerId, list);
				AppShell.Instance.EventMgr.Fire(null, msg);
			}
			if (list2.Count > 0)
			{
				OwnershipStringMessage msg2 = new OwnershipStringMessage(ownerId, list2);
				AppShell.Instance.EventMgr.Fire(null, msg2);
			}
		}
	}

	protected void HandleCardGameStringResponse(string[] args)
	{
		if (args.Length < 4)
		{
			CspUtils.DebugLog("SERVER: String response from card game did not include messageName");
		}
		else if (args[2] == "ack")
		{
			switch (args[3])
			{
			case "randomIntSelected":
				break;
			case "cardsPeeked":
				break;
			case "cardsPeekedSecretly":
				break;
			case "getFirstPlayer":
			{
				ShsEventMessage msg = new AckGetFirstPlayerMessage(Convert.ToInt32(args[4]));
				AppShell.Instance.EventMgr.Fire(null, msg);
				break;
			}
			case "fakePlayer":
			{
				ShsEventMessage msg = new AckFakePlayerMessage(Convert.ToInt32(args[4]));
				AppShell.Instance.EventMgr.Fire(null, msg);
				break;
			}
			case "pickHeroDeck":
			{
				ShsEventMessage msg = new AckPickHeroDeckMessage(Convert.ToInt32(args[4]), args[5], args[6], Convert.ToInt32(args[7]));
				AppShell.Instance.EventMgr.Fire(null, msg);
				break;
			}
			case "transferCards":
			{
				string[] arrayOfCards = args[8].Split('|');
				ShsEventMessage msg = new AckTransferCardsMessage(Convert.ToInt32(args[4]), Convert.ToInt32(args[5]), Convert.ToInt32(args[6]), Convert.ToInt32(args[7]), arrayOfCards, Convert.ToBoolean(args[9]), Convert.ToInt32(args[10]));
				AppShell.Instance.EventMgr.Fire(null, msg);
				break;
			}
			case "pickCard":
			{
				ShsEventMessage msg = new AckPickCardMessage(Convert.ToInt32(args[4]), Convert.ToInt32(args[5]), Convert.ToInt32(args[6]), Convert.ToInt32(args[7]), args[8], Convert.ToInt32(args[9]));
				AppShell.Instance.EventMgr.Fire(null, msg);
				break;
			}
			case "pickEx":
			{
				ShsEventMessage msg = new AckPickExMessage(Convert.ToInt32(args[4]), Convert.ToInt32(args[5]), Convert.ToInt32(args[6]), args[7]);
				AppShell.Instance.EventMgr.Fire(null, msg);
				break;
			}
			case "allowEx":
			{
				ShsEventMessage msg = new AckAllowExMessage(Convert.ToInt32(args[4]), Convert.ToInt32(args[5]));
				AppShell.Instance.EventMgr.Fire(null, msg);
				break;
			}
			case "tossCoin":
			{
				ShsEventMessage msg = new AckTossCoinMessage(Convert.ToInt32(args[4]));
				AppShell.Instance.EventMgr.Fire(null, msg);
				break;
			}
			case "endGame":
			{
				ShsEventMessage msg = new AckEndGameMessage(Convert.ToInt32(args[4]));
				AppShell.Instance.EventMgr.Fire(null, msg);
				break;
			}
			case "pass":
			{
				ShsEventMessage msg = new AckPassMessage(Convert.ToInt32(args[4]));
				AppShell.Instance.EventMgr.Fire(null, msg);
				break;
			}
			case "shuffleCards":
			{
				ShsEventMessage msg = new AckShuffleCardsMessage(Convert.ToInt32(args[4]), Convert.ToInt32(args[5]));
				AppShell.Instance.EventMgr.Fire(null, msg);
				break;
			}
			default:
				CspUtils.DebugLog("SERVER: Unhandled string response, " + args[3] + ", from SmartFox CardGame extension");
				break;
			}
		}
		else
		{
			CspUtils.DebugLog("SERVER: Received NACK from SmartFox CardGame extension, responding to " + args[3]);
		}
	}

	protected void HandleXMLResponse(SFSObject response)
	{
		if (response == null)
		{
			CspUtils.DebugLog("SERVER: Unrecognized response payload");
			return;
		}
		switch (response.GetString("_cmd"))
		{
		case "logOK":
			if (response.GetString("name") != null)
			{
				if (isNotification) {
					paid = int.Parse(response.GetString("paid"));
				}

				api.myUserName = response.GetString("name");
				api.myUserId = int.Parse(response.GetString("id"));
				int.TryParse(response.GetString("playerId"), out playerId);
				CspUtils.DebugLog("HandleXMLResponse playerID=" + playerId + " userName=" + api.myUserName + " userid=" + api.myUserId);  // CSP
				// playerId = 3870526;    // CSP hard coded playerId for now.
				string @string = response.GetString("sessionToken");
				if (!string.IsNullOrEmpty(@string))
				{
					AppShell.Instance.WebService.SessionKey = @string;
				}
			}
			else
			{
				if (loginTransaction != null)
				{
					loginTransaction.FailStep("login", "Unknown login failure");
				}
				api.Disconnect();
			}
			break;
		case "logKO":
			CspUtils.DebugLog("logKO");
			if (response.GetString("err") == "Invalid client version") {
				AppShell.Instance.PrompInvalidClient();
			}
			if (loginTransaction != null)
			{
				loginTransaction.FailStep("auth", response.GetString("err"));    // "Invalid credentials");
			}
			break;
		case "notification_ready":
			if (loginTransaction != null && !loginTransaction.IsStepCompleted("seated"))
			{
				loginTransaction.CompleteStep("seated");
			}
			break;
		case "notification":
		{
			StringBuilder sb = null;
			Hashtable hashtable = HashtableFromSFObject(response, sb);
			if (hashtable.ContainsKey("trans_id"))
			{
				AppShell.Instance.EventReporter.OnEventResponseMessage(hashtable);
				break;
			}
			object obj = hashtable["message_type"];
			if (obj == null)
			{
				break;
			}
			switch ((string)obj)
			{
			case "set_costume_info_response":
				break;
			case "set_title_info_response":
				break;
			case "shs_session_created":
				OnSessionCreated(hashtable);
				break;
			case "sb_stop_stage":
			{
				BrawlerController brawlerController2 = GameController.GetController() as BrawlerController;
				if (brawlerController2 != null)
				{
					brawlerController2.OnNotificationResult(hashtable);
				}
				break;
			}
			case "brawler_scoring":
			{
				BrawlerController brawlerController = GameController.GetController() as BrawlerController;
				if (brawlerController != null)
				{
					brawlerController.OnNotificationResult(hashtable);
				}
				break;
			}
			case "card_invitation":
				if (ArcadeShellController.Instance != null)
				{
					int invitationId2 = int.Parse((string)hashtable["inviter_id"]);
					AppShell.Instance.Matchmaker2.RejectCardGame(invitationId2, true);
				}
				else
				{
					AppShell.Instance.Matchmaker2.OnCardGameInvitation(hashtable);
				}
				break;
			case "card_game_start":
				AppShell.Instance.Matchmaker2.OnCardGameStart(hashtable);
				break;
			case "card_game_cancel":
				AppShell.Instance.Matchmaker2.OnCardGameCancel(hashtable);
				break;
			case "open_booster_pack_response":
				AppShell.Instance.EventMgr.Fire(this, new BoosterPackResponseMessage(true, hashtable));
				break;
			case "open_mystery_box_response":
				AppShell.Instance.EventMgr.Fire(this, new MysteryBoxResponseMessage(true, hashtable));
				break;
			case "craft_blueprint_response":
				CspUtils.DebugLog("craft_blueprint_response received " + hashtable["success"] + " " + hashtable["blueprint_id"] + " " + hashtable["error_code"]);
				if (bool.Parse(string.Empty + hashtable["success"]))
				{
					int blueprintID = int.Parse(string.Empty + hashtable["blueprint_id"]);
					Blueprint blueprint = BlueprintManager.getBlueprint(blueprintID);
					OwnableDefinition def = OwnableDefinition.getDef(blueprint.outputOwnableTypeID);
					AppShell.Instance.Profile.FetchDataBasedOnCategory(def.category);
					if (def.category != OwnableDefinition.Category.Craft)
					{
						AppShell.Instance.Profile.StartCraftFetch();
					}
					if (def.category == OwnableDefinition.Category.Sidekick)
					{
						AppShell.Instance.EventMgr.Fire(null, new PetPurchasedEvent(blueprint.outputOwnableTypeID));
					}
					else if (def.category == OwnableDefinition.Category.Title)
					{
						AppShell.Instance.EventMgr.Fire(null, new TitlePurchasedEvent(blueprint.outputOwnableTypeID));
					}
					else if (def.category == OwnableDefinition.Category.SidekickUpgrade)
					{
						AppShell.Instance.Profile.SidekickUpgrades.Add(string.Empty + def.ownableTypeID, new GenericCollectionItem(string.Empty + def.ownableTypeID, 1));
						PetDataManager.respawnPet();
					}
					else
					{
						AppShell.Instance.Profile.FetchDataBasedOnCategory(def.category);
					}
				}
				break;
			case "quest_game_complete":
			case "card_pvp_game_complete":
			case "quest_game_rewards":
			case "card_pvp_game_rewards":
			{
				CardGameController cardGameController = GameController.GetController() as CardGameController;
				if (cardGameController != null)
				{
					cardGameController.OnNotificationResult(hashtable);
				}
				break;
			}
			case "brawler_invitation":
				CspUtils.DebugLog("brawler_invitation");
				CspUtils.DebugLog("inviter_id=" + int.Parse((string)hashtable["inviter_id"]));
				if (ArcadeShellController.Instance != null)
				{
					int invitationId = int.Parse((string)hashtable["inviter_id"]);
					AppShell.Instance.Matchmaker2.CancelBrawler(invitationId, true);
				}
				else
				{
					AppShell.Instance.Matchmaker2.OnBrawlerInvitation(hashtable);
				}
				break;
			case "brawler_start":
				CspUtils.DebugLog("Brawler Start Message Received...");
				AppShell.Instance.Matchmaker2.OnBrawlerStart(hashtable);
				break;
			case "brawler_start_lock":
				AppShell.Instance.Matchmaker2.OnBrawlerLock(hashtable);
				break;
			case "presence_update":
			{
				int friendId = (!hashtable.ContainsKey("player_id")) ? (-1) : int.Parse(hashtable["player_id"].ToString());
				string friendName3 = (!hashtable.ContainsKey("player_name")) ? "<unknown>" : DecodeString(hashtable["player_name"].ToString());
				string status = (!hashtable.ContainsKey("status")) ? "unknown" : hashtable["status"].ToString();
				string location = (!hashtable.ContainsKey("location")) ? "{unknown}" : hashtable["location"].ToString();
				bool available = !hashtable.ContainsKey("multiplayer") || hashtable["multiplayer"].ToString() == "0";
				AppShell.Instance.EventMgr.Fire(this, new FriendUpdateMessage(friendId, friendName3, status, location, available));
				break;
			}
			case "prize":
				AppShell.Instance.EventMgr.Fire(this, new PrizeWheelResultsMessage(true, hashtable));
				break;
			case "wheel_state_change":
				CspUtils.DebugLog("wheel_state_change: the wheel should now be at mask " + hashtable["prize_wheel_mask"]);
				break;
			case "consume_potion_response":
			{
				CspUtils.DebugLog("consume potion response message received, success=" + hashtable["success"] + ", error_code=" + hashtable["error_code"] + ", ownable_type_id=" + hashtable["ownable_type_id"] + ", potions_remaining=" + hashtable["potions_remaining"] + ", request_id=" + hashtable["request_id"]);
				int requestId = -1;
				if (hashtable.ContainsKey("request_id"))
				{
					requestId = int.Parse(hashtable["request_id"].ToString());
				}
				string errorCode = string.Empty;
				if (hashtable["error_code"] != null)
				{
					errorCode = hashtable["error_code"].ToString();
				}
				ConsumedPotionMessage msg4 = new ConsumedPotionMessage(bool.Parse(hashtable["success"].ToString()), errorCode, int.Parse(hashtable["ownable_type_id"].ToString()), requestId, int.Parse(hashtable["potions_remaining"].ToString()));
				AppShell.Instance.EventMgr.Fire(this, msg4);
				break;
			}
			case "cancel_effect_response":
				CspUtils.DebugLog("cancel effect response message received, success=" + hashtable["success"] + ", error_code=" + hashtable["error_code"] + ", ownable_type_id=" + hashtable["ownable_type_id"] + ", potions_remaining=" + hashtable["request_id"]);
				break;
			case "catalog_purchase_finished":
			case "catalog_purchase_complete":
				CspUtils.DebugLog("Purchase Complete, success=" + hashtable["success"] + ", error_code=" + hashtable["error_code"] + ", identifier=" + hashtable["guid"]);
				AppShell.Instance.EventMgr.Fire(this, new ShoppingPurchaseCompletedMessage(hashtable["success"], hashtable["error_code"], hashtable["guid"]));
				break;
			case "fractal_balance":
			{
				CspUtils.DebugLog(string.Format("Fractal balance, player_id=" + hashtable["player_id"] + ", fractal_type_id=" + hashtable["fractal_type_id"] + ", balance=" + hashtable["balance"]));
				SHSSocialMainWindow sHSSocialMainWindow3 = GUIManager.Instance["SHSMainWindow/SHSSocialMainWindow"] as SHSSocialMainWindow;
				if (sHSSocialMainWindow3 != null)
				{
					sHSSocialMainWindow3.OnDisplayFractalBalance((FractalActivitySpawnPoint.FractalType)Convert.ToInt32(hashtable["fractal_type_id"]), Convert.ToInt32(hashtable["balance"]));
				}
				break;
			}
			case "fractal_first_bank":
			{
				CspUtils.DebugLog(string.Format("Fractal first bank, player_id" + hashtable["player_id"] + ", fractal_type_id=" + hashtable["fractal_type_id"] + ", amount=" + hashtable["amount"]));
				SHSSocialMainWindow sHSSocialMainWindow = GUIManager.Instance["SHSMainWindow/SHSSocialMainWindow"] as SHSSocialMainWindow;
				if (sHSSocialMainWindow != null)
				{
					sHSSocialMainWindow.OnFirstFractalCollected((FractalActivitySpawnPoint.FractalType)Convert.ToInt32(hashtable["fractal_type_id"]), Convert.ToInt32(hashtable["amount"]));
				}
				break;
			}
			case "world_event_winner":
			{
				CspUtils.DebugLog(string.Format("World Event Winner, player_id" + hashtable["player_id"] + "counter_type" + hashtable["counter_type"] + "reward_id") + hashtable["reward_id"]);
				SHSSocialMainWindow sHSSocialMainWindow2 = GUIManager.Instance["SHSMainWindow/SHSSocialMainWindow"] as SHSSocialMainWindow;
				if (sHSSocialMainWindow2 != null)
				{
					sHSSocialMainWindow2.OnWorldEventWon(hashtable["counter_type"].ToString(), int.Parse(hashtable["reward_id"].ToString()));
				}
				break;
			}
			case "hero_unlocked":
			{
				CspUtils.DebugLog(string.Format("Hero Unlocked, player_id" + hashtable["player_id"] + "hero_id" + hashtable["hero_id"]));
				SHSSocialMainWindow sHSSocialMainWindow5 = GUIManager.Instance["SHSMainWindow/SHSSocialMainWindow"] as SHSSocialMainWindow;
				if (sHSSocialMainWindow5 != null)
				{
					sHSSocialMainWindow5.OnHeroUnlocked(int.Parse(hashtable["hero_id"].ToString()));
				}
				break;
			}
			case "hero_instant_effect_response":
				CspUtils.DebugLog(string.Format("Hero Instant Effect Response received - code = {0}, hero = {1}, ownable_type_id = {2}", hashtable["effect_code"], hashtable["hero_name"], hashtable["ownable_type_id"]));
				AppShell.Instance.ServerConnection.ReportGameWorldEvent("hero_instant_effect_response", hashtable);
				break;
			case "hero_effect_response":
				CspUtils.DebugLog(string.Format("Hero Effect Response received - code = {0}, hero = {1}, ownable_type_id = {2}", hashtable["effect_code"], hashtable["hero_name"], hashtable["ownable_type_id"]));
				AppShell.Instance.ServerConnection.ReportGameWorldEvent("hero_effect_response", hashtable);
				break;
			case "hero_effect_cancel_response":
				CspUtils.DebugLog(string.Format("Hero Cancel Effect - code {0} hero {1} ownable {2}", hashtable["effect_code"], hashtable["hero_name"], hashtable["ownable_type_id"]));
				AppShell.Instance.ServerConnection.ReportGameWorldEvent("hero_effect_cancel_response", hashtable);
				break;
			case "player_effect_response":
				CspUtils.DebugLog(string.Format("Player Effect Response received - code = {0}, ownable_type_id = {1}", hashtable["effect_code"], hashtable["ownable_type_id"]));
				AppShell.Instance.ServerConnection.ReportGameWorldEvent("player_effect_response", hashtable);
				break;
			case "player_effect_cancel_response":
				CspUtils.DebugLog(string.Format("Player Cancel Effect - code {0} ownable {1}", hashtable["effect_code"], hashtable["ownable_type_id"]));
				AppShell.Instance.ServerConnection.ReportGameWorldEvent("player_effect_cancel_response", hashtable);
				AppShell.Instance.EventMgr.Fire(this, new StopExpendableEffectMessage(AppShell.Instance.ServerConnection.GetGameUserId(), hashtable["effect_code"].ToString(), hashtable["ownable_type_id"].ToString()));
				break;
			case "receive_scavenger_part":
				CspUtils.DebugLog(string.Format("receive_scavenger_part - player {0} item {1} total owned {2}", hashtable["player_id"], hashtable["item_id"], hashtable["total_owned"]));
				NotificationHUD.addNotification(new TotalScavengerNotificationData(Convert.ToInt32(hashtable["item_id"]), Convert.ToInt32(hashtable["total_owned"])));
				break;
			case "update_scavenger_info":
			{
				CspUtils.DebugLog(string.Format("Scavenger balance, player_id=" + hashtable["player_id"] + ", hero_name=" + hashtable["hero_name"] + ", data=" + hashtable["data"] + ", item_id=" + hashtable["item_id"]));
				HeroPersisted value3 = null;
				if (AppShell.Instance.Profile.AvailableCostumes.TryGetValue(AppShell.Instance.Profile.SelectedCostume, out value3))
				{
					value3.scavengerInfo = hashtable["data"].ToString();
					SHSSocialMainWindow sHSSocialMainWindow4 = GUIManager.Instance["SHSMainWindow/SHSSocialMainWindow"] as SHSSocialMainWindow;
					if (sHSSocialMainWindow4 != null)
					{
						sHSSocialMainWindow4.OnDisplayScavengerBalance(value3.objectsCollected, Convert.ToInt32(hashtable["item_id"]));
					}
				}
				break;
			}
			case "update_shard_info":
			{
				int num4 = Convert.ToInt32(hashtable["next_collect_time"]);
				CspUtils.DebugLog("update_shard_info " + num4);
				AppShell.Instance.Profile.nextShardTime = num4 + 2;
				AppShell.Instance.Profile.Shards = Convert.ToInt32(hashtable["total_shards"]);
				break;
			}
			case "hero_level_response":
			{
				CspUtils.DebugLog(string.Format("Hero Level Response Received - hero = {0}, level = {1}, maxLevel={2}, code = {3}, scavengerInfo = {4}", hashtable["hero_name"], hashtable["hero_level"], hashtable["hero_max_level"], hashtable["hero_level_code"], hashtable["scavenger_info"]));
				AppShell.Instance.ServerConnection.ReportGameWorldEvent("hero_level_response", hashtable);
				HeroPersisted value = null;
				if (AppShell.Instance.Profile.AvailableCostumes.TryGetValue(AppShell.Instance.Profile.SelectedCostume, out value))
				{
					value.scavengerInfo = hashtable["scavenger_info"].ToString();
					AppShell.Instance.EventMgr.Fire(this, new CharacterResponseReceivedMessage());
				}
				break;
			}
			case "squad_level_response":
				CspUtils.DebugLog(string.Format("Squd Level Response Received - level = {0}, code = {1}", hashtable["squad_level"], hashtable["squad_level_code"]));
				AppShell.Instance.ServerConnection.ReportGameWorldEvent("squad_level_response", hashtable);
				break;
			case "catalog_sell_complete":
				CspUtils.DebugLog("Sell item Complete, success=" + hashtable["success"] + ", error_code=" + hashtable["error_code"] + ", ownable_type_id=" + hashtable["ownable_type_id"] + ", guid=" + hashtable["guid"] + ", quantity_sold=" + hashtable["quantity_sold"] + ", gold_total=" + hashtable["gold_total"] + ", silver_total=" + hashtable["silver_total"] + ", tickets_total=" + hashtable["tickets_total"]);
				break;
			case "receive_admin_private_message":
			{
				string text3 = DecodeString((string)hashtable["message"]);
				CspUtils.DebugLog("Admin Private Message received for player " + hashtable["recipient_player_id"] + ": " + text3);
				AppShell.Instance.EventMgr.Fire(this, new UserWarningMessage(text3));
				break;
			}
			case "receive_admin_global_message":
			{
				string text2 = DecodeString((string)hashtable["message"]);
				CspUtils.DebugLog("Admin Global Message Received:" + text2);
				AppShell.Instance.EventMgr.Fire(this, new UserWarningMessage(text2));
				break;
			}
			case "send_room_message_error":
			{
				string text = DecodeString((string)hashtable["message"]);
				CspUtils.DebugLog("Send Room Message Error Received:" + text);
				AppShell.Instance.EventMgr.Fire(this, new UserRoomErrorMessage(text, (string)hashtable["error_code"], hashtable));
				break;
			}
			case "player_muted":
				CspUtils.DebugLog("Player Muted:" + hashtable["muted_for_seconds"]);
				AppShell.Instance.EventMgr.Fire(this, new UserMutedMessage(DecodeString((string)hashtable["message"]), (string)hashtable["muted_for_seconds"]));
				break;
			case "kick":
				CspUtils.DebugLog("SERVER: Player KICKED.");
				break;
			case "receive_room_message":
			{
				CspUtils.DebugLog("RTCCLient receive_room_message");
				string message = DecodeString(hashtable["message"].ToString());
				CspUtils.DebugLog("message=" + message);
				CspUtils.DebugLog("sender_player_id=" + hashtable["sender_player_id"].ToString());
				AppShell.Instance.EventMgr.Fire(this, new GameWorldOpenChatMessage(message, int.Parse(hashtable["sender_player_id"].ToString())));
				break;
			}
			case "friend_request":
			{
				int num3 = (!hashtable.ContainsKey("user")) ? (-1) : int.Parse(hashtable["user"].ToString());
				string friendName2 = (!hashtable.ContainsKey("user_name")) ? "<unknown>" : DecodeString(hashtable["user_name"].ToString());
				if (num3 >= 0)
				{
					AppShell.Instance.EventMgr.Fire(this, new FriendRequestMessage(num3, friendName2));
				}
				break;
			}
			case "friend_accepted":
			{
				int num2 = (!hashtable.ContainsKey("user")) ? (-1) : int.Parse(hashtable["user"].ToString());
				string friendName = (!hashtable.ContainsKey("user_name")) ? "<unknown>" : DecodeString(hashtable["user_name"].ToString());
				if (num2 >= 0)
				{
					AppShell.Instance.EventMgr.Fire(this, new FriendAcceptedMessage(num2, friendName));
				}
				break;
			}
			case "friend_declined":
			{
				int num5 = (!hashtable.ContainsKey("user")) ? (-1) : int.Parse(hashtable["user"].ToString());
				string friendName4 = (!hashtable.ContainsKey("user_name")) ? "<unknown>" : DecodeString((string)hashtable["user_name"]);
				if (num5 >= 0)
				{
					AppShell.Instance.EventMgr.Fire(this, new FriendDeclinedMessage(num5, friendName4));
				}
				break;
			}
			case "set_xp_multiplier":
				if (AppShell.Instance.Profile == null)
				{
					AppShell.Instance.cachedXPMultiplier = float.Parse(string.Empty + hashtable["xp_multiplier"]);
				}
				else
				{
					AppShell.Instance.Profile.xpMultiplier = float.Parse(string.Empty + hashtable["xp_multiplier"]);
				}
				break;
			case "hero_ping_request":
				CspUtils.DebugLog("got hero_ping_request");
				AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "generic_event", "swap_hero", 1, string.Empty);
				break;
			case "zone_ping_request":
				CspUtils.DebugLog("got zone_ping_request");
				AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "generic_event", "switch_zone", 1, -10000, -10000, OwnableDefinition.simpleZoneName(SocialSpaceControllerImpl.getZoneName()), string.Empty);
				break;
			case "achievement_complete":
			{
				CspUtils.DebugLog("achievement_complete");
				List<OwnableSet> rewards = processEconomyMessage(hashtable, false);
				AchievementCompleteNotificationData achievementCompleteNotificationData = new AchievementCompleteNotificationData(int.Parse(string.Empty + hashtable["achievement_id"]), rewards);
				NotificationHUD.addNotification(achievementCompleteNotificationData);
				AppShell.Instance.EventMgr.Fire(null, new AchievementCompleteMessage(int.Parse(string.Empty + hashtable["achievement_id"])));
				List<NewAchievement> dependentAchievements = AchievementManager.getDependentAchievements(achievementCompleteNotificationData.achievementID);
				if (dependentAchievements != null)
				{
					foreach (NewAchievement item in dependentAchievements)
					{
						AppShell.Instance.EventReporter.ReportAchievementEvent(string.Empty, "preprocess", string.Empty, 0, item.achievementID, -10000, string.Empty, string.Empty);
					}
				}
				break;
			}
			case "achievement_step_data_ex_update":
			{
				AchievementStepDataExJsonData data5 = JsonMapper.ToObject<AchievementStepDataExJsonData>(hashtable["data"].ToString());
				AchievementStepDataEx data6 = new AchievementStepDataEx(data5);
				AchievementManager.updateAchievementData(data6);
				break;
			}
			case "achievement_data_update":
			{
				AchievementDataJsonData data3 = JsonMapper.ToObject<AchievementDataJsonData>(hashtable["data"].ToString());
				AchievementData data4 = new AchievementData(data3);
				AchievementManager.updateAchievementData(data4);
				break;
			}
			case "achievement_step_data_update":
			{
				AchievementStepDataJsonData data = JsonMapper.ToObject<AchievementStepDataJsonData>(hashtable["data"].ToString());
				AchievementStepData data2 = new AchievementStepData(data);
				AchievementManager.updateAchievementData(data2);
				break;
			}
			case "economy_message":
			{
				List<OwnableSet> list = processEconomyMessage(hashtable);
				break;
			}
			case "leveled_up":
			{
				LeveledUpMessage leveledUpMessage = new LeveledUpMessage(hashtable);
				CspUtils.DebugLog(leveledUpMessage.ToString());
				HeroPersisted value2;
				if (AppShell.Instance.Profile != null && AppShell.Instance.Profile.AvailableCostumes.TryGetValue(leveledUpMessage.Hero, out value2))
				{
					int heroCurrentXp = leveledUpMessage.HeroCurrentXp;
					if (heroCurrentXp == -1)
					{
						CspUtils.DebugLog("Using old system to update XP");
						XpToLevelDefinition.Instance.GetXpForLevel(leveledUpMessage.NewLevel);
					}
					value2.Xp = heroCurrentXp;
				}
				AppShell.Instance.EventMgr.Fire(this, leveledUpMessage);
				break;
			}
			case "save_hq_room_layout_response":
			{
				string roomId = (!hashtable.ContainsKey("room")) ? "<unknown>" : ((string)hashtable["room"]);
				AppShell.Instance.EventMgr.Fire(this, new HQRoomLayoutSaveResponseMessage(roomId));
				break;
			}
			case "challenge_confirmation":
				if (hashtable.ContainsKey("success") && bool.Parse(hashtable["success"].ToString()))
				{
					if (!hashtable.ContainsKey("player_id") || !hashtable.ContainsKey("challenge_met_id") || !hashtable.ContainsKey("next_challenge_id") || !hashtable.ContainsKey("silver_reward") || !hashtable.ContainsKey("gold_reward") || !hashtable.ContainsKey("ticket_reward") || !hashtable.ContainsKey("item_reward") || !hashtable.ContainsKey("success"))
					{
						CspUtils.DebugLog("Challenge Confirmation missing critical payload info");
						ChallengeServerMessage msg = new ChallengeServerMessage(-1, -1, ChallengeRewardType.Unknown, string.Empty, "false", "missing info");
						AppShell.Instance.EventMgr.Fire(this, msg);
					}
					else
					{
						int result;
						ChallengeRewardType rewardType = (int.TryParse(hashtable["silver_reward"].ToString(), out result) && result > 0) ? ChallengeRewardType.Silver : ((int.TryParse(hashtable["gold_reward"].ToString(), out result) && result > 0) ? ChallengeRewardType.Gold : ((int.TryParse(hashtable["ticket_reward"].ToString(), out result) && result > 0) ? ChallengeRewardType.Tickets : ((int.TryParse(hashtable["item_reward"].ToString(), out result) && result > 0) ? ChallengeRewardType.Hero : ChallengeRewardType.Unknown)));
						ChallengeServerMessage msg2 = new ChallengeServerMessage(Convert.ToInt32(hashtable["challenge_met_id"]), Convert.ToInt32(hashtable["next_challenge_id"]), rewardType, result.ToString(), "true");
						AppShell.Instance.EventMgr.Fire(this, msg2);
					}
				}
				else
				{
					ChallengeServerMessage msg3 = new ChallengeServerMessage(-1, -1, ChallengeRewardType.Unknown, string.Empty, "false", hashtable["error_code"].ToString());
					AppShell.Instance.EventMgr.Fire(this, msg3);
				}
				break;
			case "gold_balance_update":
			{
				int num = (!hashtable.ContainsKey("balance")) ? (-1) : int.Parse(hashtable["balance"].ToString());
				CspUtils.DebugLog("Got gold_balance_update: " + num);
				LocalPlayerProfile localPlayerProfile = AppShell.Instance.Profile as LocalPlayerProfile;
				if (localPlayerProfile == null)
				{
					CspUtils.DebugLog("Unable to obtain players profile to update currency");
				}
				else
				{
					localPlayerProfile.SyncExternalGoldBalance(num);
				}
				break;
			}
			default:
				CspUtils.DebugLog("possible unhandled message received by RTCClient: " + (string)obj);
				break;
			}
			break;
		}
		}
	}

	protected List<OwnableSet> processEconomyMessage(Hashtable payload, bool doAward = true)
	{
		List<OwnableSet> list = new List<OwnableSet>();
		SHSSocialMainWindow sHSSocialMainWindow = GUIManager.Instance["SHSMainWindow/SHSSocialMainWindow"] as SHSSocialMainWindow;
		if (payload.ContainsKey("xp_added"))
		{
			int num = int.Parse(string.Empty + payload["xp_added"]);
			int num2 = 0;
			if (payload.ContainsKey("xp_added_bonus"))
			{
				num2 = int.Parse(string.Empty + payload["xp_added_bonus"]);
			}
			HeroPersisted value;
			if (AppShell.Instance.Profile.AvailableCostumes.TryGetValue(payload["hero_name"].ToString(), out value) && doAward)
			{
				if (!payload.ContainsKey("xp_client_add") || int.Parse(string.Empty + payload["xp_client_add"]) == 1)
				{
					value.UpdateXp(num + num2);
				}
				//Below uncommented by doggo
				SocialSpaceControllerImpl.ShowXPToast(GameController.GetController().LocalPlayer.GetComponent<CharacterGlobals>(), num, num2);
			}
		}
		if (payload["fractals_added"] != null)
		{
			list.Add(new OwnableSet(303943, int.Parse(string.Empty + payload["fractals_added"])));
			if (sHSSocialMainWindow != null)
			{
				sHSSocialMainWindow.OnDisplayFractalBalance(FractalActivitySpawnPoint.FractalType.Fractal, Convert.ToInt32(payload["total_fractals"]));
			}
			//AppShell.Instance.Profile.Shards = Convert.ToInt32(payload["total_fractals"]);CspUtils.DebugLog("GetAllUsers room name=" + r.GetName());			
			int fractals = Convert.ToInt32(payload["fractals_added"]);
			AppShell.Instance.Profile.Shards += fractals;  // added by CSP
			AppShell.Instance.EventMgr.Fire(null, new CurrencyUpdateMessage());
		}
		if (payload.ContainsKey("gold_added"))
		{
		}
		if (payload.ContainsKey("total_gold"))
		{
			AppShell.Instance.Profile.Gold = Convert.ToInt32(payload["total_gold"]);
			AppShell.Instance.EventMgr.Fire(null, new CurrencyUpdateMessage());
		}
		if (payload.ContainsKey("total_ownables"))
		{
			int num3 = int.Parse(string.Empty + payload["total_ownables"]);
			for (int i = 1; i <= num3; i++)
			{
				OwnableDefinition def = OwnableDefinition.getDef(int.Parse(string.Empty + payload["ownableID" + i]));
				if (def != null)
				{
					if (doAward)
					{
						AppShell.Instance.Profile.FetchDataBasedOnCategory(def.category);
					}
					list.Add(new OwnableSet(int.Parse(string.Empty + payload["ownableID" + i]), int.Parse(string.Empty + payload["ownableQuantity" + i])));
				}
			}
		}
		return list;
	}

	private static Hashtable HashtableFromSFObject(SFSObject response, StringBuilder sb)
	{
		Hashtable hashtable = new Hashtable();
		foreach (object item in response.Keys())
		{
			object obj = response.Get(item);
			if (obj != null)
			{
				if (sb != null)
				{
					sb.Append("  ");
					sb.Append(item);
					sb.Append(" = ");
					sb.Append(obj.ToString());
					sb.AppendLine();
				}
				hashtable[item] = obj;
			}
		}
		return hashtable;
	}

	public static string DecodeString(string s)
	{
		//Discarded unreachable code: IL_0016, IL_0038
		try
		{
			return Encoding.UTF8.GetString(Convert.FromBase64String(s));
		}
		catch (FormatException)
		{
			CspUtils.DebugLog("Failed to convert value from base64 (maybe because its not base 64): " + s);
			return s;
		}
	}

	public static string EncodeString(string s)
	{
		return Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
	}

	protected void OnSFDisconnect()
	{
		CspUtils.DebugLog("SF Disconnect!");
		OnConnect(false, "Aborted due to disconnect");
		OnDisconnect();
	}

	protected void OnSFJoinRoom(Room room)
	{
		CspUtils.DebugLog("&&&&&&&&&&&&&&&&&&&&&&&&&&&  OnSFJoinRoom called!");
		CspUtils.DebugLog("room.GetName()=" + room.GetName());

		if (loginTransaction == null)
		{
			CspUtils.DebugLog("SERVER: OnSFJoinRoom: loginTransaction is null");
		}
		else if (room.GetName() == "lobby")
		{
			loginTransaction.CompleteStep("login");
			CspUtils.DebugLog("isNotification = " + isNotification);
			if (!isNotification)
			{
				ArrayList arrayList = new ArrayList(1);
				arrayList.Add(ticket);  // CSP - room name is in ticket, I think....getSeat creates room and joins? ANSWER: no, room ID is in ticket node <instance>, and should have been set previously.
				SendXtMessage("maitred", "getSeat", arrayList, "str");

			}
		}
		else
		{
			PingServer();
			SendXtMessage("uservars", "user_vars", null, "xml");  // added by CSP to request playerVars earlier.				
			AppShell.Instance.PlayerDictionary.Update(new PlayerDictionary.Player(this.UserId, 0, this.username, true, false, 1, 1));	// added by CSP
			loginTransaction.CompleteStep("seated");
			AppShell.Instance.EventMgr.Fire(null, new RoomUserListChangeMessage(room.GetId()));
		}
	}

	public void OnSFUserEnterRoom(int roomId, User user)
	{
		string userName = GetUserName(user.GetId());
		OnUserEnter(roomId, user.GetId());
		if (loginTransaction == null || loginTransaction.IsStepCompleted("seated"))
		{
			AppShell.Instance.EventMgr.Fire(null, new RoomUserEnterMessage(roomId, user.GetId(), userName));
			AppShell.Instance.EventMgr.Fire(null, new RoomUserListChangeMessage(roomId));
		}
	}

	public void OnSFUserLeaveRoom(int roomId, int userId, string userName)
	{
		if (loginTransaction == null || loginTransaction.IsStepCompleted("seated"))
		{
			AppShell.Instance.EventMgr.Fire(null, new RoomUserLeaveMessage(roomId, userId));
			AppShell.Instance.EventMgr.Fire(null, new RoomUserListChangeMessage(roomId));
		}
		OnUserLeave(roomId, userId);
	}

	public void OnSFObjectReceived(SFSObject obj, User sender)
	{
		string text = obj.Get("_d") as string;
		if (!string.IsNullOrEmpty(text))
		{
			NetworkMessage networkMessage = NetworkMessage.FromEncodedString(text);
			networkMessage.senderRTCId = sender.GetId();
			OnMessage(networkMessage);
		}
		else
		{
			CspUtils.DebugLog("SERVER: Unexpected data object");
		}
	}

	public void OnSFRoomVariableUpdate(Room room, Hashtable changedVars)
	{
		RoomVariableChangeMessage msg = new RoomVariableChangeMessage(room.GetId(), changedVars);
		AppShell.Instance.EventMgr.Fire(null, msg);
	}

	public void OnSFUserVariableUpdate(User user, Hashtable changedVars)
	{
		UserVariableChangeMessage msg = new UserVariableChangeMessage(user.GetId(), changedVars);
		AppShell.Instance.EventMgr.Fire(null, msg);
	}

	protected IPAddress NameToIpResolve(string host)
	{
		IPAddress result = null;
		try
		{
			IPHostEntry hostEntry = Dns.GetHostEntry(host);
			if (hostEntry.AddressList.Length > 0)
			{
				return hostEntry.AddressList[0];
			}
			return result;
		}
		catch
		{
			return null;
		}
	}

	// this method added by CSP
	public void OnLogin(bool success, string name, string error) 
	{ 
		CspUtils.DebugLog("OnLogin() reached!");

		if ( success ) { 
			CspUtils.DebugLog("Login for user \"" + name + "\" successful.");
	  	} 
		else { 
			// Login failed - lets display the error message sent to us 
			CspUtils.DebugLog("Login error: " + error); 

			AppShell.Instance.PrompServerPlayerMaxReached();

			//EditorUtility.DisplayDialog("Login Error", error + " - Please try again later.", "OK", "Cancel");
			//#if UNITY_EDITOR
         	//	UnityEditor.EditorApplication.isPlaying = false;
    		//#else
         	//	Application.Quit();
     		//#endif			
		} 
	} 

	protected void AddEventHandlers()
	{
		SmartFoxClient smartFoxClient = api;
		smartFoxClient.onConnection = (SmartFoxClient.OnConnectionDelegate)Delegate.Combine(smartFoxClient.onConnection, new SmartFoxClient.OnConnectionDelegate(OnSFConnect));
		SmartFoxClient smartFoxClient2 = api;
		smartFoxClient2.onConnectionLost = (SmartFoxClient.OnConnectionLostDelegate)Delegate.Combine(smartFoxClient2.onConnectionLost, new SmartFoxClient.OnConnectionLostDelegate(OnSFDisconnect));
		SmartFoxClient smartFoxClient3 = api;
		smartFoxClient3.onExtensionResponse = (SmartFoxClient.OnExtensionResponseDelegate)Delegate.Combine(smartFoxClient3.onExtensionResponse, new SmartFoxClient.OnExtensionResponseDelegate(OnSFExtensionResponse));
		SmartFoxClient smartFoxClient4 = api;
		smartFoxClient4.onJoinRoom = (SmartFoxClient.OnJoinRoomDelegate)Delegate.Combine(smartFoxClient4.onJoinRoom, new SmartFoxClient.OnJoinRoomDelegate(OnSFJoinRoom));
		SmartFoxClient smartFoxClient5 = api;
		smartFoxClient5.onUserEnterRoom = (SmartFoxClient.OnUserEnterRoomDelegate)Delegate.Combine(smartFoxClient5.onUserEnterRoom, new SmartFoxClient.OnUserEnterRoomDelegate(OnSFUserEnterRoom));
		SmartFoxClient smartFoxClient6 = api;
		smartFoxClient6.onUserLeaveRoom = (SmartFoxClient.OnUserLeaveRoomDelegate)Delegate.Combine(smartFoxClient6.onUserLeaveRoom, new SmartFoxClient.OnUserLeaveRoomDelegate(OnSFUserLeaveRoom));
		SmartFoxClient smartFoxClient7 = api;
		smartFoxClient7.onObjectReceived = (SmartFoxClient.OnObjectReceivedDelegate)Delegate.Combine(smartFoxClient7.onObjectReceived, new SmartFoxClient.OnObjectReceivedDelegate(OnSFObjectReceived));
		SmartFoxClient smartFoxClient8 = api;
		smartFoxClient8.onRoomVariablesUpdate = (SmartFoxClient.OnRoomVariablesUpdateDelegate)Delegate.Combine(smartFoxClient8.onRoomVariablesUpdate, new SmartFoxClient.OnRoomVariablesUpdateDelegate(OnSFRoomVariableUpdate));
		SmartFoxClient smartFoxClient9 = api;
		smartFoxClient9.onUserVariablesUpdate = (SmartFoxClient.OnUserVariablesUpdateDelegate)Delegate.Combine(smartFoxClient9.onUserVariablesUpdate, new SmartFoxClient.OnUserVariablesUpdateDelegate(OnSFUserVariableUpdate));
		SmartFoxClient smartFoxClient10 = api;
		smartFoxClient10.onAdminMessage = (SmartFoxClient.OnAdminMessageDelegate)Delegate.Combine(smartFoxClient10.onAdminMessage, new SmartFoxClient.OnAdminMessageDelegate(OnAdminMessage));
		SmartFoxClient smartFoxClient11 = api;
		smartFoxClient11.onDebugMessage = (SmartFoxClient.OnDebugMessageDelegate)Delegate.Combine(smartFoxClient11.onDebugMessage, new SmartFoxClient.OnDebugMessageDelegate(OnSFDebugMessage));
	
		SmartFoxClient smartFoxClient12 = api; // added by CSP
		smartFoxClient12.onLogin += OnLogin; // added by CSP
		SmartFoxClient smartFoxClient13 = api; // added by CSP
		smartFoxClient13.onRandomKey += OnRandomKey; // added by CSP
	}

	protected void RemoveEventHandlers()
	{
		SmartFoxClient smartFoxClient = api;
		smartFoxClient.onConnection = (SmartFoxClient.OnConnectionDelegate)Delegate.Remove(smartFoxClient.onConnection, new SmartFoxClient.OnConnectionDelegate(OnSFConnect));
		SmartFoxClient smartFoxClient2 = api;
		smartFoxClient2.onConnectionLost = (SmartFoxClient.OnConnectionLostDelegate)Delegate.Remove(smartFoxClient2.onConnectionLost, new SmartFoxClient.OnConnectionLostDelegate(OnSFDisconnect));
		SmartFoxClient smartFoxClient3 = api;
		smartFoxClient3.onExtensionResponse = (SmartFoxClient.OnExtensionResponseDelegate)Delegate.Remove(smartFoxClient3.onExtensionResponse, new SmartFoxClient.OnExtensionResponseDelegate(OnSFExtensionResponse));
		SmartFoxClient smartFoxClient4 = api;
		smartFoxClient4.onJoinRoom = (SmartFoxClient.OnJoinRoomDelegate)Delegate.Remove(smartFoxClient4.onJoinRoom, new SmartFoxClient.OnJoinRoomDelegate(OnSFJoinRoom));
		SmartFoxClient smartFoxClient5 = api;
		smartFoxClient5.onUserEnterRoom = (SmartFoxClient.OnUserEnterRoomDelegate)Delegate.Remove(smartFoxClient5.onUserEnterRoom, new SmartFoxClient.OnUserEnterRoomDelegate(OnSFUserEnterRoom));
		SmartFoxClient smartFoxClient6 = api;
		smartFoxClient6.onUserLeaveRoom = (SmartFoxClient.OnUserLeaveRoomDelegate)Delegate.Remove(smartFoxClient6.onUserLeaveRoom, new SmartFoxClient.OnUserLeaveRoomDelegate(OnSFUserLeaveRoom));
		SmartFoxClient smartFoxClient7 = api;
		smartFoxClient7.onObjectReceived = (SmartFoxClient.OnObjectReceivedDelegate)Delegate.Remove(smartFoxClient7.onObjectReceived, new SmartFoxClient.OnObjectReceivedDelegate(OnSFObjectReceived));
		SmartFoxClient smartFoxClient8 = api;
		smartFoxClient8.onRoomVariablesUpdate = (SmartFoxClient.OnRoomVariablesUpdateDelegate)Delegate.Remove(smartFoxClient8.onRoomVariablesUpdate, new SmartFoxClient.OnRoomVariablesUpdateDelegate(OnSFRoomVariableUpdate));
		SmartFoxClient smartFoxClient9 = api;
		smartFoxClient9.onUserVariablesUpdate = (SmartFoxClient.OnUserVariablesUpdateDelegate)Delegate.Remove(smartFoxClient9.onUserVariablesUpdate, new SmartFoxClient.OnUserVariablesUpdateDelegate(OnSFUserVariableUpdate));
		SmartFoxClient smartFoxClient10 = api;
		smartFoxClient10.onAdminMessage = (SmartFoxClient.OnAdminMessageDelegate)Delegate.Remove(smartFoxClient10.onAdminMessage, new SmartFoxClient.OnAdminMessageDelegate(OnAdminMessage));
		SmartFoxClient smartFoxClient11 = api;
		smartFoxClient11.onDebugMessage = (SmartFoxClient.OnDebugMessageDelegate)Delegate.Remove(smartFoxClient11.onDebugMessage, new SmartFoxClient.OnDebugMessageDelegate(OnSFDebugMessage));
	}
}
