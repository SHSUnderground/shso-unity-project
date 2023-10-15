using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetworkManager : IDisposable, IServerConnection
{
	public static string sf_notification;
	public static string sf_game;


	public class UserInfo
	{
		public int userId = -1;

		public string userName = string.Empty;

		public UserInfo(int id, string name)
		{
			userId = id;
			userName = name;
		}
	}

	[Flags]
	public enum ConnectionState
	{
		Disconnected = 0x0,
		Authenticated = 0x1,
		ConnectedToNotificationServer = 0x2,
		DisconnectingFromGame = 0x4,
		ConnectedToGame = 0x8,
		PendingAdminDisconnect = 0x10
	}
	private string username;
	private string password;

	public delegate void OnConnectDelegate(bool success, string error);

	private const int MAX_NOTIFICATION_CONNECTION_RETRIES = 3;

	private const int MAX_GAMEROOM_CONNECTION_RETRIES = 3;

	private TransactionMonitor loginTransaction;

	private string profileTxt;

	protected bool disposed;

	protected int hostUserId = -1;

	private string sik;

	protected ConnectionState state;

	private OnConnectDelegate onConnect;

	private LoginStatusNotifier loginStatusNotifier;

	private string cachedNotificationServerName = "<unknown>";

	private RTCClient notificationServer;  

	private RTCClient gameServer;

	protected NetGameManager gameNetMgr;

	private int notifRetryCount;

	private int gameRoomRetryCount;

	private string gameRoomName;

	private Matchmaker2.Ticket gameRoomTicket;




	public ConnectionState State
	{
		get
		{
			return state;
		}
	}

	public NetGameManager Game
	{
		get
		{
			return gameNetMgr;
		}
	}

	public string GameServerName
	{
		get
		{
			return (gameServer == null) ? null : gameServer.ServerFullName;
		}
	}

	public string NotificationServerName
	{
		get
		{
			if (notificationServer != null)
			{
				return notificationServer.ServerFullName;
			}
			return cachedNotificationServerName;
		}
	}

	public NetworkManager()
	{
		NetType.Initialize();
		disposed = false;
		hostUserId = -1;
		sik = null;
		state = ConnectionState.Disconnected;
		onConnect = null;
		notificationServer = null;
		gameServer = null;
	}

	~NetworkManager()
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
		if (disposed)
		{
			return;
		}
		if (disposing)
		{
			if (gameServer != null)
			{
				DisconnectFromGame();
			}
			if (notificationServer != null)
			{
				DisconnectFromNotificationServer();
			}
		}
		disposed = true;
		hostUserId = -1;
		sik = null;
		AppShell.Instance.WebService.Logout();
		state = ConnectionState.Disconnected;
		onConnect = null;
		notificationServer = null;
		gameServer = null;
		gameNetMgr = null;
	}

	public void Update()
	{
		if (loginTransaction != null)
		{
			loginTransaction.Update();
		}
		if (notificationServer != null)
		{
			notificationServer.Update();
		}
		if (gameServer != null)
		{
			gameServer.Update();
		}
		if (gameNetMgr != null)
		{
			gameNetMgr.Update();
		}
	}

	private void CreateLoginTransaction()
	{
		loginStatusNotifier = new LoginStatusNotifier();
		loginTransaction = TransactionMonitor.CreateTransactionMonitor("NetworkManager_loginTransaction", OnLoginComplete, 120f, null);
		if (AppShell.Instance.LaunchLoginTransaction != null)
		{
			if (AppShell.Instance.LaunchLoginTransaction.HasStep("initialize"))
			{
				AppShell.Instance.LaunchLoginTransaction.CompleteStep("initialize");
			}
			AppShell.Instance.LaunchLoginTransaction.AddChild(loginTransaction);
		}
		loginTransaction.AddStep("auth", TransactionMonitor.DumpTransactionStatus);
		loginTransaction.AddStep("notificationServer", TransactionMonitor.DumpTransactionStatus);
		loginTransaction.AddStep("arrival", TransactionMonitor.DumpTransactionStatus);
		loginTransaction.AddStep("entitlement_ask", TransactionMonitor.DumpTransactionStatus);
		loginTransaction.AddStep("profile", TransactionMonitor.DumpTransactionStatus);
		loginTransaction.AddStep("counters", TransactionMonitor.DumpTransactionStatus);
		loginTransaction.AddStep("profile_bits", TransactionMonitor.DumpTransactionStatus);
		loginTransaction.AddStep("shopping_catalog", TransactionMonitor.DumpTransactionStatus);
	}

	public void Login(string sikCredential)
	{
		Logout();
		CreateLoginTransaction();
		loginStatusNotifier.Notify(LoginStatusNotifier.LoginStep.ReadToken);
		sik = sikCredential;
		loginTransaction.CompleteStep("auth");
		RequestConnectToNotificationServer();
	}

	public void Login(string username, string password)
	{
		Logout();
		CreateLoginTransaction();
		loginStatusNotifier.Notify(LoginStatusNotifier.LoginStep.RequestToken);
		WWWForm wWWForm = new WWWForm();
		// wWWForm.AddField("password", password);
		wWWForm.AddField("username", username.Trim());
		this.password = password;
		this.username = username;
		//AppShell.Instance.WebService.StartRequest("resources$platform/gettoken/", OnAuthenticationWebResponse, wWWForm.data, null, 10f, 0.5f, ShsWebService.ShsWebServiceType.RASP);
		// CSP had to remove trailing '/' from URI above.
		AppShell.Instance.WebService.StartRequest("resources$platform/gettoken.py", OnAuthenticationWebResponse, wWWForm.data, null, 10f, 0.5f, ShsWebService.ShsWebServiceType.RASP);
	}

	public void Logout()
	{
		if (loginTransaction != null)
		{
			loginTransaction.Fail("Aborted");
			loginTransaction = null;
		}
		if (onConnect != null)
		{
			onConnect(false, "Aborted");
			onConnect = null;
		}
		hostUserId = -1;
		profileTxt = null;
		sik = null;
		AppShell.Instance.WebService.Logout();
		DisconnectFromGame();
		DisconnectFromNotificationServer();
		SetConnectionStatus(ConnectionState.Disconnected);
	}

	public void ConnectToGame(string room, Matchmaker2.Ticket ticket, OnConnectDelegate onConnectFn)
	{
		if ((state & ConnectionState.Authenticated) == 0)
		{
			throw new Exception("Not Authenticated");
		}
		if (ticket == null)
		{
			throw new Exception("Ticket is required");
		}
		if ((state & ConnectionState.ConnectedToGame) != 0)
		{
			throw new Exception("Already connected to a game");
		}
		if (gameNetMgr == null)
		{
			gameNetMgr = new NetGameManager();
			onConnect = onConnectFn;
			AppShell.Instance.PlayerDictionary.Clear();
			gameRoomName = room;
			gameRoomTicket = ticket;
			RetryGameServerConnect();
			return;
		}
		throw new Exception("Only allowed in one game at a time");
	}

	private void RetryGameServerConnect()
	{
		RemoveGameEventHandlers();
		gameServer = new RTCClient();
		AddGameEventHandlers();
//		if (AppShell.Instance.ServerConfig.TryGetBool("//custom_smartfox_server", false))
//		{
			string str = AppShell.Instance.ServerConfig.TryGetString("//environment", "DEV");
			string text = AppShell.Instance.ServerConfig.TryGetString("//" + str + "/smartfox/custom_game", null);
			CspUtils.DebugLog("RetryGameServerConnect text=" + text);  //
			if (text != null)
			{
				CspUtils.DebugLog("SERVER: Overriding smartfox game server: " + text);
				gameRoomTicket.server = text;
			}
//		}
		gameServer.Connect(gameRoomTicket.server, gameRoomName, gameRoomTicket.ticket, username, password);
	}

	public void DisconnectFromGame()
	{
		if (gameNetMgr != null)
		{
			gameNetMgr.Dispose();
			gameNetMgr = null;
		}
		SetConnectionStatus(state | ConnectionState.DisconnectingFromGame);
		if (gameServer != null)
		{
			OnGameServerDisconnect();
			gameServer.Disconnect();
			gameServer.Dispose();
			gameServer = null;
		}
	}

	// this method added by CSP, as the code for initiating the finding of a game room could not be found. 
	// public void GetGameRoom(int roomType, bool isHost) {
	// 	if (gameServer != null)
	// 	{
	// 		gameServer.GetGameRoom(roomType, isHost);
	// 	}
	// }

	public void SendGameMsg(NetworkMessage msg)
	{
		//CspUtils.DebugLog("SendGameMsg NetActionPositionFull msg.GetType()= " + msg.GetType().ToString());
		//if (msg.GetType() == typeof(NetActionMessage)) //CSP 
		//	CspUtils.DebugLog("SendGameMsg NetActionMessage");

		//if (msg.GetType() == typeof(NetActionPositionFull)) //CSP 
		//	CspUtils.DebugLog("SendGameMsg NetActionPositionFull");

		if (gameServer != null)
		{
			gameServer.SendToAll(msg);
		}
	}

	public void SendGameMsg(NetworkMessage msg, int userId)
	{
		if (gameServer != null)
		{
			gameServer.SendToUser(msg, userId);
		}
	}

	public void PingServer()
	{
		if (gameServer != null)
		{
			gameServer.PingServer();
		}
	}

	public void SendHeroCreate(string hero, string key, string data)
	{
		if (gameServer != null)
		{
			gameServer.SendHeroCreate(hero, key, data);
		}
	}

	public void SendHeroSync(int userRTCId, string hero, string key, string data)
	{
		if (gameServer != null)
		{
			gameServer.SendHeroSync(userRTCId, hero, key, data);
		}
	}

	public bool IsGameHost()
	{
		if (gameServer != null)
		{
			return hostUserId == gameServer.UserId;
		}
		return true;
	}

	// method added by CSP
	public RTCClient GetGameServer() {
		return gameServer;
	}

	// method added by CSP
	public RTCClient getNotificationServer()
	{
		return notificationServer;
	}

	public int GetGameHostId()
	{
		return hostUserId;
	}

	public int GetGameUserId()
	{
		if (gameServer != null)
		{
			return gameServer.UserId;
		}
		return -1;
	}

	public int GetGameUserCount()
	{
		if (gameServer != null)
		{
			return gameServer.GetUserCount();
		}
		return 0;
	}

	public int[] GetGameAllUserIds()
	{
		if (gameServer != null)
		{
			return gameServer.GetAllUserIds();
		}
		return new int[0];
	}

	public List<UserInfo> GetGameAllUsers()
	{
		if (gameServer != null)
		{
			return gameServer.GetAllUsers();
		}
		return new List<UserInfo>(0);
	}

	public string GetGameUserName(int userId)
	{
		if (gameServer != null)
		{
			return gameServer.GetUserName(userId);
		}
		return "<not connected>";
	}

	public object GetUserVariable(int userId, string name)
	{
		if (gameServer != null)
		{
			return gameServer.GetUserVariable(userId, name);
		}
		CspUtils.DebugLog("SERVER: User variable requested when not connected to game server!");
		return null;
	}

	public Hashtable GetUserVariablesDict(int userId)
	{
		if (gameServer != null)
		{
			return gameServer.GetUserVariablesDict(userId);
		}
		CspUtils.DebugLog("SERVER: User variables (dict) requested when not connected to game server!");
		return null;
	}

	public string[] GetUserVariables(int userId)
	{
		if (gameServer != null)
		{
			return gameServer.GetUserVariables(userId);
		}
		CspUtils.DebugLog("SERVER: User variables requested when not connected to game server!");
		return null;
	}

	public void SetUserVariable(string name, int value)
	{
		if (gameServer != null)
		{
			gameServer.SetUserVariable(name, value);
		}
		else
		{
			CspUtils.DebugLog("SERVER: User variable set  when not connected to game server!");
		}
	}

	public void SetUserVariable(string name, string value)
	{
		if (gameServer != null)
		{
			gameServer.SetUserVariable(name, value);
		}
		else
		{
			CspUtils.DebugLog("SERVER: User variable set  when not connected to game server!");
		}
	}

	public void SetUserVariable(string name, bool value)
	{
		if (gameServer != null)
		{
			gameServer.SetUserVariable(name, value);
		}
		else
		{
			CspUtils.DebugLog(string.Format("SERVER: User variable <{0}> set  when not connected to game server!", name));
		}
	}

	public bool GetRoomIsActive()
	{
		if (gameServer != null)
		{
			return gameServer.GetRoomIsActive();
		}
		return false;
	}

	public int GetGameRoomId()
	{
		if (gameServer != null)
		{
			return gameServer.GetRoomId();
		}
		CspUtils.DebugLog("SERVER: Room ID requested when not connected to game server!");
		return -1;
	}

	public string GetRoomName()
	{
		if (gameServer != null)
		{
			return gameServer.GetRoomName();
		}
		CspUtils.DebugLog("SERVER: Room Name requested when not connected to game server!");
		return "not_connected";
	}

	public int GetRoomMaxUsers()
	{
		if (gameServer != null)
		{
			return gameServer.GetRoomMaxUsers();
		}
		CspUtils.DebugLog("SERVER: Room max users requested when not connected to game server!");
		return 0;
	}

	public object GetRoomVariable(string name)
	{
		if (gameServer != null)
		{
			return gameServer.GetRoomVariable(name);
		}
		CspUtils.DebugLog("SERVER: Room variable requested when not connected to a game server!");
		return null;
	}

	public void SetRoomVariable(string name, int data)
	{
		SetRoomVariableObject(name, data);
	}

	public void SetRoomVariable(string name, bool data)
	{
		SetRoomVariableObject(name, data);
	}

	public void SetRoomVariable(string name, string data)
	{
		SetRoomVariableObject(name, data);
	}

	protected void SetRoomVariableObject(string name, object data)
	{
		if (gameServer != null)
		{
			gameServer.SetRoomVariable(name, data);
		}
		else
		{
			CspUtils.DebugLog("SERVER: Room variable set when not connected to a game server!");
		}
	}

	public void LockRoom()
	{
		if (gameServer != null)
		{
			gameServer.LockRoom();
		}
	}

	public void TakeOwnership(GoNetId goNetId, bool autoTransfer)
	{
		////////// CSP added this block for testing. ///////////////////////////
		NetGameManager.NetEntity value; 
		if (gameNetMgr.dictNetObjs.TryGetValue(goNetId, out value)) {	
			CspUtils.DebugLog("TakeOwner goNetId= " + goNetId); 
			CspUtils.DebugLog("TakeOwner name= " + value.netComp.gameObject.name);
		}
		/////////////////////////////////////////////////////////////////////

		if (gameServer != null)
		{
			gameServer.TakeOwnership(goNetId, autoTransfer);
		}
		else
		{
			CspUtils.DebugLog("SERVER: Game is NULL");
		}
	}

	public void TransferOwnership(int newOwnerId, GoNetId goNetId)
	{
		////////// CSP added this block for testing. ///////////////////////////
		NetGameManager.NetEntity value; 
		if (gameNetMgr.dictNetObjs.TryGetValue(goNetId, out value)) {	
			CspUtils.DebugLog("TransOwner goNetId= " + goNetId + " newOwnerId=" + newOwnerId); 
			CspUtils.DebugLog("TransOwner name= " + value.netComp.gameObject.name);
		}
		/////////////////////////////////////////////////////////////////////

		if (gameServer != null)
		{
			gameServer.TransferOwnership(newOwnerId, goNetId);
		}
	}

	public void ReleaseOwnership(GoNetId goNetId)
	{
		////////// CSP added this block for testing. ///////////////////////////
		NetGameManager.NetEntity value; 
		if (gameNetMgr.dictNetObjs.TryGetValue(goNetId, out value)) {	
			CspUtils.DebugLog("ReleaseOwner goNetId= " + goNetId); 
			CspUtils.DebugLog("ReleaseOwner name= " + value.netComp.gameObject.name);
		}
		/////////////////////////////////////////////////////////////////////

		if (gameServer != null)
		{
			gameServer.ReleaseOwnership(goNetId);
		}
	}

	public void QueryOwnership(GoNetId goNetId)
	{
		if (gameServer != null)
		{
			gameServer.QueryOwnership(goNetId);
		}
	}

	public void TakeOwnership(string str, bool autoTransfer)
	{
		if (gameServer != null)
		{
			gameServer.TakeOwnership(str, autoTransfer);
		}
		else
		{
			CspUtils.DebugLog("SERVER: Game is NULL");
		}
	}

	public void TransferOwnership(int newOwnerId, string str)
	{
		if (gameServer != null)
		{
			gameServer.TransferOwnership(newOwnerId, str);
		}
	}

	public void ReleaseOwnership(string str)
	{
		if (gameServer != null)
		{
			gameServer.ReleaseOwnership(str);
		}
	}

	public void QueryOwnership(string str)
	{
		if (gameServer != null)
		{
			gameServer.QueryOwnership(str);
		}
	}

	public void QueryAllOwnership()
	{
		if (gameServer != null)
		{
			gameServer.QueryAllOwnership();
		}
	}

	public void ResetAllOwnership()
	{
		if (gameServer != null)
		{
			gameServer.ResetAllOwnership();
		}
	}

	public void SendGameSAMessage(string message, ArrayList args)
	{
		if (gameServer != null)
		{
			gameServer.SendGameSAMessage(message, args);
		}
	}

	public void RequestConnectToNotificationServer()
	{
		CspUtils.DebugLog("Requesting Connection to Smartfox Notification Server...");
		loginStatusNotifier.Notify(LoginStatusNotifier.LoginStep.RequestNotificationServer);
		if (sik == null || sik == "NULL")
		{
			CspUtils.DebugLog("SERVER: Not Authenticated - sik is " + sik);
			throw new Exception("Not Authenticated - sik is " + sik);
		}
		if (notificationServer == null)
		{
			notificationServer = new RTCClient();
			notifRetryCount = 0;
			SendGetNotificationServerRequest();
			return;
		}
		CspUtils.DebugLog("SERVER: Already connected to notification server!");
		throw new Exception("Already connected to notification server!");
	}

	private void SendGetNotificationServerRequest()
	{
		AddNotificationServerEventHandlers();
		if (!AppShell.Instance.ServerConfig.TryGetBool("//custom_smartfox_server", false) || !ConnectToOverrideNotificationServer())
		{
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("ticket", sik);
			wWWForm.AddField("AS_PROTOCOL", AppShell.Instance.ServerConfig.TryGetInt("protocol_version", 0));
			AppShell.Instance.WebService.StartRequest("resources$mm/lobby/selection", OnRequestNotificationServerResponse, wWWForm.data);
		}
	}

	private void RetryNotificationServerRequest()
	{
		RemoveNotificationServerEventHandlers();
		notificationServer = new RTCClient();
		SendGetNotificationServerRequest();
	}

	private bool ConnectToOverrideNotificationServer()
	{
		string str = AppShell.Instance.ServerConfig.TryGetString("//environment", "DEV");
		string text = AppShell.Instance.ServerConfig.TryGetString("//" + str + "/smartfox/custom_notification", null);
		sf_notification = text;  // added by CSP 
		sf_game = AppShell.Instance.ServerConfig.TryGetString("//" + str + "/smartfox/custom_game", null); // added by CSP
		if (text != null)
		{
			CspUtils.DebugLog("SERVER: Overriding notification server: " + text);
			ConnectToNotificationServer(text);
			return true;
		}
		return false;
	}

	private void OnRequestNotificationServerResponse(ShsWebResponse response)
	{
		if (response.Status == 200)
		{
			DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
			dataWarehouse.Parse();
			string @string = dataWarehouse.GetString("//notification");

			///////////// block added by CSP
			CspUtils.DebugLog("@string="+@string);
			string str = AppShell.Instance.ServerConfig.TryGetString("//environment", "DEV");
			sf_notification = AppShell.Instance.ServerConfig.TryGetString("//" + str + "/smartfox/custom_notification", null); 
			sf_game = AppShell.Instance.ServerConfig.TryGetString("//" + str + "/smartfox/custom_game", null); 
			@string = sf_notification; 
			CspUtils.DebugLog("@string2="+@string);
			////////////////////////////////////////

			CspUtils.DebugLog("Connect to notification server " + @string);
			ConnectToNotificationServer(@string);
		}
		else
		{
			CspUtils.DebugLog("SERVER: Failed to get NotificationServer from Resource Server: " + response.Status + " " + response.Body);
			loginTransaction.FailStep("notificationserver", "Login failed - Request for Notification Server Name failed: " + response.Status + " " + response.Body);
		}
	}

	private void ConnectToNotificationServer(string serverName)
	{
		cachedNotificationServerName = serverName;
		loginStatusNotifier.Notify(LoginStatusNotifier.LoginStep.ConnectToNotificationServer);
		string zoneName = AppShell.Instance.ServerConfig.TryGetString("smartfox/notificationZoneName", "shs.notification");
		notificationServer.Sik = sik;
		CspUtils.DebugLog("Sending request to join Notification Server: " + serverName + ", " + zoneName + " zone.");
		notificationServer.Connect(serverName, zoneName, string.Empty, username, password);
	}

	public void DisconnectFromNotificationServer()
	{
		if (notificationServer != null)
		{
			notificationServer.Disconnect();
			notificationServer.Dispose();
			notificationServer = null;
		}
	}

	public void NotifyPendingAdminDisconnect()
	{
		SetConnectionStatus(state | ConnectionState.PendingAdminDisconnect);
	}

	public void ReportEvent(string evtName, Hashtable args)
	{
		if (gameServer != null)
		{
			gameServer.ReportEvent(evtName, args);
		}
	}

	public void ReportGameWorldEvent(string evtName, Hashtable args)
	{
		if (gameServer != null)
		{
			gameServer.ReportGameWorldEvent(evtName, args);
		}
	}

	public void ReportBrawlerEvent(string evtName, Hashtable args)
	{
		if (gameServer != null)
		{
			gameServer.ReportBrawlerEvent(evtName, args);
		}
	}

	public void ReportChatEvent(string evtName, Hashtable args)
	{
		if (notificationServer != null)
		{
			notificationServer.ReportChatEvent(evtName, args);
		}
	}

	protected void OnLoginComplete(TransactionMonitor.ExitCondition exit, string error, object userData)
	{
		if (AppShell.Instance.LaunchLoginTransaction != null)
		{
			AppShell.Instance.LaunchLoginTransaction.RemoveChild(loginTransaction);
		}
		loginTransaction = null;
		switch (exit)
		{
		case TransactionMonitor.ExitCondition.Success:
			SetConnectionStatus(state | ConnectionState.Authenticated);
			SendLoginSuccess(profileTxt);
			profileTxt = null;
			break;
		case TransactionMonitor.ExitCondition.Fail:
			SetConnectionStatus(ConnectionState.Disconnected);
			SendLoginFailed(error);
			break;
		case TransactionMonitor.ExitCondition.TimedOut:
			SetConnectionStatus(ConnectionState.Disconnected);
			SendLoginFailed("Timed Out");
			break;
		}
	}

	private void OnAuthenticationWebResponse(ShsWebResponse response)
	{
		if (response.Status == 200)
		{
			sik = response.Body;
			loginTransaction.CompleteStep("auth");
			RequestConnectToNotificationServer();
			return;
		}
		CspUtils.DebugLog("SERVER: Authentication Failed: " + response.Status + " " + response.Body);
		string text = "Login failed. ";
		if (response.Status == 404)
		{
			text += "Invalid username.";
			loginStatusNotifier.Notify(LoginStatusNotifier.LoginStep.BadUserName);
		}
		else if (response.Status == 403)
		{
			if (response.Body.Contains("{\"error"))
			{
				if (response.Body.Contains("11000"))
				{
					loginStatusNotifier.Notify(LoginStatusNotifier.LoginStep.BadPassword);
				}
				else if (response.Body.Contains("11003"))
				{
					loginStatusNotifier.Notify(LoginStatusNotifier.LoginStep.AccountSuspended);
				}
				else if (response.Body.Contains("11004"))
				{
					loginStatusNotifier.Notify(LoginStatusNotifier.LoginStep.AccountBanned);
				}
				else
				{
					loginStatusNotifier.Notify(LoginStatusNotifier.LoginStep.PlatformError);
				}
				text += response.Body;
			}
			else if (response.Body.Contains("ERR_BAD_PROTOCOL"))
			{
				text += "Version mismatch.";
				loginStatusNotifier.Notify(LoginStatusNotifier.LoginStep.BadProtocol);
			}
			else if (response.Body.Contains("ERR_ACCOUNT_EXPIRED"))
			{
				text += "Account expired.";
				loginStatusNotifier.Notify(LoginStatusNotifier.LoginStep.AccountExpired);
			}
			else if (response.Body.Contains("ERR_BAD_PASSWORD"))
			{
				text += "Incorrect password.";
				loginStatusNotifier.Notify(LoginStatusNotifier.LoginStep.BadPassword);
			}
			else
			{
				text += "Unexpected Error 403.";
				loginStatusNotifier.Notify(LoginStatusNotifier.LoginStep.UnknownError);
			}
		}
		else if (response.TimedOut)
		{
			text += "Timed out.";
			loginStatusNotifier.Notify(LoginStatusNotifier.LoginStep.TimedOut);
		}
		else
		{
			string text2 = text;
			text = text2 + "Unknown error. (Status: " + response.Status + ")";
			loginStatusNotifier.Notify(LoginStatusNotifier.LoginStep.UnknownError);
		}
		if (loginTransaction != null)
		{
			loginTransaction.FailStep("auth", text);
		}
	}

	protected void OnProfileWebResponse(ShsWebResponse response)
	{
		if (response.Status == 200)
		{
			profileTxt = response.Body;
			if (loginTransaction != null)
			{
				loginTransaction.CompleteStep("profile");
				AppShell.Instance.Profile = new LocalPlayerProfile(profileTxt, OnProfileLoaded);
			}
		}
		else
		{
			CspUtils.DebugLog("SERVER: User Profile Error <" + response.Status + ">: " + response.Body);
			if (loginTransaction != null)
			{
				loginTransaction.FailStep("profile", "Login failed. Unable to retrieve player data.");
			}
		}
	}

	private void OnProfileLoaded(UserProfile profile)
	{
		if (loginTransaction != null)
		{
			loginTransaction.CompleteStep("profile_bits");
		}
		LoadOwnableList();
	}

	private void LoadOwnableList()
	{
		AppShell.Instance.WebService.StartRequest("resources$data/json/ownables_list.py" , OnOwnableListLoaded, null, ShsWebService.ShsWebServiceType.RASP);
	}

	private void OnOwnableListLoaded(ShsWebResponse response)
	{
		CspUtils.DebugLog("Got ownables list\n" + response.Body);
		OwnableDumpJson ownableDumpJson = JsonMapper.ToObject<OwnableDumpJson>(response.Body);
		OwnableDefinition.load(ownableDumpJson.ownableslist, ownableDumpJson.keywords, ownableDumpJson.ownablekeywords);
		
		// CSP - temporarily comment this block out - promo related?
		//OwnableDefinition def = OwnableDefinition.getDef(615937);		
		//if (def.metadata != null && def.metadata.Length > 1)
		//{
		//	AppShell.Instance.WebService.StartRequest(def.metadata, onUpsellImageLoaded, ShsWebService.ShsWebServiceType.Texture);
		//}
		OwnableDefinition.convertScavengerInfo();
		AppShell.Instance.AchievementManager.begin();
		LoadShoppingCatalog();
	}

	private void onUpsellImageLoaded(ShsWebResponse response)
	{
		CspUtils.DebugLog("onUpsellImageLoaded " + response.RequestUri + " status = " + response.Status + " " + response.Texture);
		AppShell.Instance.promoImage = response.Texture;
	}

	public void LoadMasterAchievementData()
	{
		AppShell.Instance.WebService.StartRequest("resources$data/json/achievements_master.py", OnAchievementsLoaded, null, ShsWebService.ShsWebServiceType.RASP);
	}

	private void OnAchievementsLoaded(ShsWebResponse response)
	{
		CspUtils.DebugLog("OnAchievementsLoaded \n");
		if (response.Status != 200)
		{
			CspUtils.DebugLog("error when requesting root achievement data: " + response.Body);
			AppShell.Instance.AchievementManager.masterLoadFailed();
		}
		else
		{
			AppShell.Instance.AchievementManager.parseAchievements(response.Body);
		}
	}

	public void LoadAchievementData(int targetPlayerID)
	{
		AppShell.Instance.WebService.StartRequest("resources$data/json/achievements.py", OnAchievementDataLoaded, null, ShsWebService.ShsWebServiceType.RASP);
		// AppShell.Instance.WebService.StartRequest("resources$data/json/achievements-player", OnAchievementDataLoaded, null, ShsWebService.ShsWebServiceType.RASP);
	}

	private void OnAchievementDataLoaded(ShsWebResponse response)
	{
		CspUtils.DebugLog("OnAchievementDataLoaded\n");
		PlayerAchievementData data = AchievementManager.parseAchievementData(response.Body);
		AppShell.Instance.EventMgr.Fire(this, new AchievementDataLoadedMessage(data));
	}

	public void LoadBlueprints()
	{
		AppShell.Instance.WebService.StartRequest("resources$data/json/blueprints.py", OnBlueprintsLoaded, null, ShsWebService.ShsWebServiceType.RASP);
	}

	private void OnBlueprintsLoaded(ShsWebResponse response)
	{
		CspUtils.DebugLog("Got blueprints from web \n" + response.Body);
		BlueprintManager.parseBlueprints(response.Body);
	}

	private void LoadShoppingCatalog()
	{
		//Replaced AppShell.Instance.Profile.UserId with 1111111
		AppShell.Instance.WebService.StartRequest("resources$data/json/shopping_catalog.py", OnShoppingWebResponse, null, ShsWebService.ShsWebServiceType.RASP);
	}

	private void OnShoppingWebResponse(ShsWebResponse response)
	{
		CspUtils.DebugLog("GOT CATALOG! (contents excluded for size)\n");
		if (response.Status != 200)
		{
			if (loginTransaction != null)
			{
				loginTransaction.FailStep("shopping_catalog", "Shopping data not available or badly formed. Status: " + response.Status);
			}
			return;
		}
		Dictionary<string, List<NewShoppingManager.NewCatalogJson>> dictionary = JsonMapper.ToObject<Dictionary<string, List<NewShoppingManager.NewCatalogJson>>>(response.Body);
		AppShell.Instance.NewShoppingManager = new NewShoppingManager(dictionary["shopping-catalog"]);
		if (loginTransaction != null)
		{
			loginTransaction.CompleteStep("shopping_catalog");
		}
		LoadBlueprints();
	}

	private void OnCounterResponse(bool success, string error)
	{
		if (success)
		{
			loginTransaction.CompleteStep("counters");
		}
		else
		{
			loginTransaction.FailStep("counters", error);
		}
	}

	protected void OnGameServerConnected(bool success, string error)
	{
		if (success)
		{
			UpdateHostId();
			SetConnectionStatus(state | ConnectionState.ConnectedToGame);
			if (onConnect != null)
			{
				onConnect(true, string.Empty);
				onConnect = null;
			}
			return;
		}
		CspUtils.DebugLog(string.Format("SERVER: SmartFox game server connection failed <{0}> (attempt {1})", error, gameRoomRetryCount));
		if (gameRoomRetryCount < 3)
		{
			gameRoomRetryCount++;
			RetryGameServerConnect();
			return;
		}
		SetConnectionStatus(state & ~(ConnectionState.DisconnectingFromGame | ConnectionState.ConnectedToGame | ConnectionState.PendingAdminDisconnect));
		if (onConnect != null)
		{
			onConnect(false, error);
			onConnect = null;
		}
	}

	private void OnNotificationServerConnected(bool success, string error)
	{
		if (success)
		{
			SetConnectionStatus(state | ConnectionState.ConnectedToNotificationServer);
			loginTransaction.CompleteStep("notificationServer");
			loginStatusNotifier.Notify(LoginStatusNotifier.LoginStep.RequestEntitlements);
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("AS_SESSION_KEY", AppShell.Instance.WebService.SessionKey);
			wWWForm.AddField("AS_PROTOCOL", AppShell.Instance.ServerConfig.TryGetInt("protocol_version", 0));
			AppShell.Instance.WebService.StartRequest("resources$mm/lobby/arrive", ProcessArrivalResponse, wWWForm.data, ShsWebService.ShsWebServiceType.RASP);

		}
		else
		{
			CspUtils.DebugLog(string.Format("SERVER: SmartFox notification server connection failed <{0}> (attempt {1})", error, notifRetryCount));
			if (notifRetryCount < 3)
			{
				notifRetryCount++;
				RetryNotificationServerRequest();
			}
			else
			{
				CspUtils.DebugLog("SERVER: Smartfox notification server connection failed for the last time.  Giving up...");
				SetConnectionStatus(state & ~ConnectionState.ConnectedToNotificationServer);
				loginTransaction.FailStep("notificationServer", error);
			}
		}
	}

	private void ProcessArrivalResponse(ShsWebResponse response)
	{
		HttpStatusCode status = (HttpStatusCode)response.Status;
		HttpStatusCode httpStatusCode = status;
		if (httpStatusCode == HttpStatusCode.OK)
		{
			switch (response.Body)
			{
			case "OK":
				loginTransaction.CompleteStep("arrival");
				RequestUserProfile();
				break;
			case "ERR_BAD_PROTOCOL":
				loginStatusNotifier.Notify(LoginStatusNotifier.LoginStep.BadProtocol);
				loginTransaction.FailStep("arrival", "Wrong protocol");
				break;
			case "ERR_NO_SESSION":
				loginStatusNotifier.Notify(LoginStatusNotifier.LoginStep.BadSession);
				loginTransaction.FailStep("arrival", "Bad or Missing Session");
				break;
			case "ERR_GAME_FULL":
				loginStatusNotifier.Notify(LoginStatusNotifier.LoginStep.GameFull);
				loginTransaction.FailStep("arrival", "Arrival response says that the game is full");
				break;
			default:
				loginStatusNotifier.Notify(LoginStatusNotifier.LoginStep.UnknownError);
				loginTransaction.FailStep("arrival", "Unknown response from arrival service: " + response.Body);
				CspUtils.DebugLog("unknown response from arrival: " + response.Body);
				break;
			}
		}
		else
		{
			loginTransaction.FailStep("arrival", "Error returned from arrival service: " + response.Status + ": " + response.Body);
			CspUtils.DebugLog("SERVER: arrival: " + response.Status + ": " + response.Body);
		}
	}

	protected void OnGameServerDisconnect()
	{
		RemoveGameEventHandlers();
		AppShell.Instance.EventMgr.Fire(this, new GameServerDisconnectMessage(state));
		if ((state & ConnectionState.DisconnectingFromGame) == 0 && (state & ConnectionState.PendingAdminDisconnect) == 0 && (state & ConnectionState.ConnectedToNotificationServer) != 0)
		{
			AppShell.Instance.CriticalError(SHSErrorCodes.Code.GameServerDisconnectedError, "[" + gameServer.ObfuscatedName + "]");
		}
		SetConnectionStatus(state & ~(ConnectionState.DisconnectingFromGame | ConnectionState.ConnectedToGame));
	}

	private void OnNotificationServerDisconnect()
	{
		CspUtils.DebugLog("OnNotificationServerDisconnect");
		RemoveNotificationServerEventHandlers();
		AppShell.Instance.EventMgr.Fire(this, new NotificationServerDisconnectMessage(state));
		if ((state & ConnectionState.DisconnectingFromGame) == 0 && (state & ConnectionState.PendingAdminDisconnect) == 0)
		{
			AppShell.Instance.CriticalError(SHSErrorCodes.Code.NotificationServerDisconnectedError, "[" + notificationServer.ObfuscatedName + "]");
		}
		else
		{
			Logout();
		}
		SetConnectionStatus(state & ~ConnectionState.ConnectedToNotificationServer);
	}

	private void OnNotificationServerSessionCreated(Hashtable msg)
	{
		CspUtils.DebugLog("OnNotificationServerSessionCreated - Requesting User Profile");
		RequestUserProfile();
	}

	private string GetPlayerId()
	{
		return notificationServer.PlayerId.ToString();
	}

	private void RequestUserProfile()
	{
		if (loginTransaction != null)
		{
			if (!loginTransaction.IsStepCompleted("arrival"))
			{
				loginTransaction.CompleteStep("arrival");
			}
			if (!loginTransaction.IsStepCompleted("entitlement_ask"))
			{
				loginTransaction.CompleteStep("entitlement_ask");
				string playerId = GetPlayerId();
				loginStatusNotifier.Notify(LoginStatusNotifier.LoginStep.RequestUserProfile);
				//AppShell.Instance.WebService.StartRequest("resources$users/" + playerId + "-", OnProfileWebResponse);
				WWWForm formData = new WWWForm();  // CSP
				// formData.AddField("user", playerId);	// CSP
				AppShell.Instance.WebService.StartRequest("resources$users/user.py", OnProfileWebResponse, formData.data);	  // CSP
				AppShell.Instance.CounterManager.LoadAllPersisted(playerId, OnCounterResponse);
			}
		}
	}

	protected void OnGameMessage(NetworkMessage msg)
	{
		gameNetMgr.ProcessMessage(msg);
	}

	protected void OnUserEnter(int roomId, int userId)
	{
		UpdateHostId();
		gameNetMgr.OnUserEnter(userId);
	}

	protected void OnUserLeave(int roomId, int userId)
	{
		UpdateHostId();
		gameNetMgr.OnUserLeave(userId);
	}

	protected void AddGameEventHandlers()
	{
		gameServer.onConnect += OnGameServerConnected;
		gameServer.onDisconnect += OnGameServerDisconnect;
		gameServer.onMessage += OnGameMessage;
		gameServer.onUserEnter += OnUserEnter;
		gameServer.onUserLeave += OnUserLeave;
	}

	protected void RemoveGameEventHandlers()
	{
		if (gameServer != null)
		{
			gameServer.onConnect -= OnGameServerConnected;
			gameServer.onDisconnect -= OnGameServerDisconnect;
			gameServer.onMessage -= OnGameMessage;
			gameServer.onUserEnter -= OnUserEnter;
			gameServer.onUserLeave -= OnUserLeave;
		}
	}

	private void AddNotificationServerEventHandlers()
	{
		notificationServer.onConnect += OnNotificationServerConnected;
		notificationServer.onDisconnect += OnNotificationServerDisconnect;
		notificationServer.onSessionCreated += OnNotificationServerSessionCreated;
	}

	private void RemoveNotificationServerEventHandlers()
	{
		notificationServer.onConnect -= OnNotificationServerConnected;
		notificationServer.onDisconnect -= OnNotificationServerDisconnect;
		notificationServer.onSessionCreated -= OnNotificationServerSessionCreated;
	}

	protected void UpdateHostId()
	{
		hostUserId = int.MaxValue;
		int[] allUserIds = gameServer.GetAllUserIds();
		foreach (int num in allUserIds)
		{
			if (num < hostUserId)  // CSP - first one in a game room is assigned a lower userID, and is therefore the host.
			{
				hostUserId = num;
			}
		}
	}

	protected void SetConnectionStatus(ConnectionState newState)
	{
		if (state != newState)
		{
			NetworkConnectionStatusChange msg = new NetworkConnectionStatusChange(newState, state);
			state = newState;
			AppShell.Instance.EventMgr.Fire(this, msg);
		}
	}

	private static void SendLoginSuccess(string profile)
	{
		LoginCompleteMessage msg = new LoginCompleteMessage(LoginCompleteMessage.LoginStatus.LoginSucceeded, string.Empty, profile);
		AppShell.Instance.EventMgr.Fire(null, msg);
	}

	private void SendLoginFailed(string error)
	{
		state = ConnectionState.Disconnected;
		string text = error;
		error = text + " [" + loginStatusNotifier.LastStep + "]";
		LoginCompleteMessage msg = new LoginCompleteMessage(LoginCompleteMessage.LoginStatus.LoginFailed, error, string.Empty);
		AppShell.Instance.EventMgr.Fire(null, msg);
	}
}
