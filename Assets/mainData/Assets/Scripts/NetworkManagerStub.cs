using System.Collections;
using System.Collections.Generic;

public class NetworkManagerStub : IServerConnection
{
	protected NetGameManager gameNetMgr;

	protected Hashtable roomVars;

	protected Hashtable userVars;


	public NetworkManager.ConnectionState State
	{
		get
		{
			return NetworkManager.ConnectionState.Authenticated | NetworkManager.ConnectionState.ConnectedToGame;
		}
	}

	public NetGameManager Game
	{
		get
		{
			return gameNetMgr;
		}
	}

	public string NotificationServerName
	{
		get
		{
			return "stub.notif:9339";
		}
	}

	public string GameServerName
	{
		get
		{
			return "stub.server:9339";
		}
	}

	public NetworkManagerStub()
	{
		gameNetMgr = new NetGameManager();
		roomVars = new Hashtable();
		userVars = new Hashtable();
	}

	public void Update()
	{
	}

	public void Login(string tokenCredentials)
	{
		LoginCompleteMessage msg = new LoginCompleteMessage(LoginCompleteMessage.LoginStatus.LoginFailed, "Single Player", string.Empty);
		AppShell.Instance.EventMgr.Fire(null, msg);
	}

	public void Login(string username, string password)
	{
		LoginCompleteMessage msg = new LoginCompleteMessage(LoginCompleteMessage.LoginStatus.LoginFailed, "Single Player", string.Empty);
		AppShell.Instance.EventMgr.Fire(null, msg);
	}

	public void Logout()
	{
	}

	public void ConnectToGame(string room, Matchmaker2.Ticket ticket, NetworkManager.OnConnectDelegate onConnect)
	{
		if (gameNetMgr == null)
		{
			gameNetMgr = new NetGameManager();
		}
		onConnect(true, string.Empty);
	}

	public void DisconnectFromGame()
	{
		gameNetMgr.Dispose();
		gameNetMgr = null;
	}

	public void SendGameMsg(NetworkMessage msg)
	{
	}

	public void SendGameMsg(NetworkMessage msg, int userId)
	{
	}

	public void PingServer()
	{
	}

	public void LoadBlueprints()
	{
	}

	public void LoadMasterAchievementData()
	{
	}

	public void LoadAchievementData(int targetPlayerID)
	{
	}

	public void SendHeroCreate(string hero, string key, string data)
	{
	}

	public void SendHeroSync(int userRTCId, string hero, string key, string data)
	{
	}

	public bool IsGameHost()
	{
		return true;
	}

	// method added by CSP
	public RTCClient getNotificationServer()
	{
		return null;
	}

	// method added by CSP
	public RTCClient GetGameServer() 
	{
		return null;
	}

	public int GetGameHostId()
	{
		return 1;
	}

	public int GetGameUserId()
	{
		return 1;
	}

	public bool GetRoomIsActive()
	{
		return true;
	}

	// method added by CSP
	// public void GetGameRoom(int roomType, bool isHost)
	// {
	
	// }

	public int GetGameRoomId()
	{
		return 1;
	}

	public int GetGameUserCount()
	{
		return 1;
	}

	public int[] GetGameAllUserIds()
	{
		return new int[1]
		{
			1
		};
	}

	public List<NetworkManager.UserInfo> GetGameAllUsers()
	{
		List<NetworkManager.UserInfo> list = new List<NetworkManager.UserInfo>(1);
		list.Add(new NetworkManager.UserInfo(1, "stub.user"));
		return list;
	}

	public string GetGameUserName(int userId)
	{
		return "stub.user";
	}

	public object GetUserVariable(int userId, string name)
	{
		Hashtable hashtable = userVars[userId] as Hashtable;
		if (hashtable != null)
		{
			return hashtable[name];
		}
		return null;
	}

	public Hashtable GetUserVariablesDict(int userId)
	{
		return userVars[userId] as Hashtable;
	}

	public string[] GetUserVariables(int userId)
	{
		Hashtable userVariablesDict = GetUserVariablesDict(userId);
		if (userVariablesDict != null)
		{
			string[] array = new string[userVariablesDict.Count];
			userVariablesDict.Keys.CopyTo(array, 0);
			return array;
		}
		return null;
	}

	public void SetUserVariable(string name, int value)
	{
		SetUserVariableObject(name, value);
	}

	public void SetUserVariable(string name, string value)
	{
		SetUserVariableObject(name, value);
	}

	public void SetUserVariable(string name, bool value)
	{
		SetUserVariableObject(name, value);
	}

	protected void SetUserVariableObject(string name, object value)
	{
		Hashtable hashtable = userVars[1] as Hashtable;
		if (hashtable == null)
		{
			hashtable = new Hashtable();
			userVars[1] = hashtable;
		}
		hashtable[name] = value;
	}

	public string GetRoomName()
	{
		return "stub.room";
	}

	public int GetRoomMaxUsers()
	{
		return 1;
	}

	public object GetRoomVariable(string name)
	{
		return roomVars[name];
	}

	public void SetRoomVariable(string name, int data)
	{
		roomVars[name] = data;
	}

	public void SetRoomVariable(string name, string data)
	{
		roomVars[name] = data;
	}

	public void SetRoomVariable(string name, bool data)
	{
		roomVars[name] = data;
	}

	public void LockRoom()
	{
	}

	public void TakeOwnership(GoNetId goNetId, bool autoTransfer)
	{
		List<GoNetId> list = new List<GoNetId>();
		list.Add(goNetId);
		OwnershipGoNetMessage msg = new OwnershipGoNetMessage(1, list);
		AppShell.Instance.EventMgr.Fire(null, msg);
	}

	public void TransferOwnership(int newOwnerId, GoNetId goNetId)
	{
		List<GoNetId> list = new List<GoNetId>();
		list.Add(goNetId);
		OwnershipGoNetMessage msg = new OwnershipGoNetMessage(newOwnerId, list);
		AppShell.Instance.EventMgr.Fire(null, msg);
	}

	public void ReleaseOwnership(GoNetId goNetId)
	{
		List<GoNetId> list = new List<GoNetId>();
		list.Add(goNetId);
		OwnershipGoNetMessage msg = new OwnershipGoNetMessage(-2, list);
		AppShell.Instance.EventMgr.Fire(null, msg);
	}

	public void QueryOwnership(GoNetId goNetId)
	{
		List<GoNetId> list = new List<GoNetId>();
		list.Add(goNetId);
		OwnershipGoNetMessage msg = new OwnershipGoNetMessage(1, list);
		AppShell.Instance.EventMgr.Fire(null, msg);
	}

	public void TakeOwnership(string str, bool autoTransfer)
	{
		List<string> list = new List<string>();
		list.Add(str);
		OwnershipStringMessage msg = new OwnershipStringMessage(1, list);
		AppShell.Instance.EventMgr.Fire(null, msg);
	}

	public void TransferOwnership(int newOwnerId, string str)
	{
		List<string> list = new List<string>();
		list.Add(str);
		OwnershipStringMessage msg = new OwnershipStringMessage(newOwnerId, list);
		AppShell.Instance.EventMgr.Fire(null, msg);
	}

	public void ReleaseOwnership(string str)
	{
		List<string> list = new List<string>();
		list.Add(str);
		OwnershipStringMessage msg = new OwnershipStringMessage(-2, list);
		AppShell.Instance.EventMgr.Fire(null, msg);
	}

	public void QueryOwnership(string str)
	{
		List<string> list = new List<string>();
		list.Add(str);
		OwnershipStringMessage msg = new OwnershipStringMessage(1, list);
		AppShell.Instance.EventMgr.Fire(null, msg);
	}

	public void QueryAllOwnership()
	{
	}

	public void ResetAllOwnership()
	{
		OwnershipGoNetMessage msg = new OwnershipGoNetMessage(-3, null);
		AppShell.Instance.EventMgr.Fire(null, msg);
		OwnershipStringMessage msg2 = new OwnershipStringMessage(-3, null);
		AppShell.Instance.EventMgr.Fire(null, msg2);
	}

	public void SendGameSAMessage(string message, ArrayList args)
	{
	}

	public void ReportEvent(string evtName, Hashtable args)
	{
	}

	public void ReportGameWorldEvent(string evtName, Hashtable args)
	{
	}

	public void ReportBrawlerEvent(string evtName, Hashtable args)
	{
	}

	public void ReportChatEvent(string evtName, Hashtable args)
	{
	}

	public void RequestConnectToNotificationServer()
	{
	}

	public void DisconnectFromNotificationServer()
	{
	}

	public void NotifyPendingAdminDisconnect()
	{
	}
}
