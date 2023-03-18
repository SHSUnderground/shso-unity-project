using System.Collections;
using System.Collections.Generic;

public interface IServerConnection
{
	RTCClient getNotificationServer();  // CSP 

	NetworkManager.ConnectionState State
	{
		get;
	}

	NetGameManager Game
	{
		get;
	}

	string GameServerName
	{
		get;
	}

	string NotificationServerName
	{
		get;
	}


	void Update();

	void Login(string username, string password);

	void Login(string credentials);

	void Logout();

	void ConnectToGame(string room, Matchmaker2.Ticket ticket, NetworkManager.OnConnectDelegate onConnect);

	void DisconnectFromGame();

	void SendGameMsg(NetworkMessage msg);

	void SendGameMsg(NetworkMessage msg, int userId);

	void PingServer();

	void SendHeroCreate(string hero, string key, string data);

	void SendHeroSync(int userRTCId, string hero, string key, string data);

	bool IsGameHost();

	
	// method added by CSP
	RTCClient GetGameServer();

	int GetGameHostId();

	int GetGameUserId();

	int GetGameUserCount();

	int GetGameRoomId();

	//void GetGameRoom(int roomType, bool isHost);  // added by CSP

	int[] GetGameAllUserIds();

	List<NetworkManager.UserInfo> GetGameAllUsers();

	object GetUserVariable(int userId, string name);

	Hashtable GetUserVariablesDict(int userId);

	string[] GetUserVariables(int userId);

	void SetUserVariable(string name, int value);

	void SetUserVariable(string name, string value);

	void SetUserVariable(string name, bool value);

	bool GetRoomIsActive();

	string GetRoomName();

	int GetRoomMaxUsers();

	object GetRoomVariable(string name);

	void SetRoomVariable(string name, int value);

	void SetRoomVariable(string name, string value);

	void SetRoomVariable(string name, bool value);

	void LockRoom();

	void TakeOwnership(GoNetId goNetId, bool autoTransfer);

	void TakeOwnership(string str, bool autoTransfer);

	void TransferOwnership(int newOwnerId, GoNetId goNetId);

	void TransferOwnership(int newOwnerId, string str);

	void ReleaseOwnership(GoNetId goNetId);

	void ReleaseOwnership(string str);

	void QueryOwnership(GoNetId goNetId);

	void QueryOwnership(string str);

	void QueryAllOwnership();

	void ResetAllOwnership();

	void SendGameSAMessage(string message, ArrayList args);

	void ReportEvent(string evtName, Hashtable args);

	void ReportGameWorldEvent(string evtName, Hashtable args);

	void ReportBrawlerEvent(string evtName, Hashtable args);

	void ReportChatEvent(string evtName, Hashtable args);

	void RequestConnectToNotificationServer();

	void DisconnectFromNotificationServer();

	void NotifyPendingAdminDisconnect();

	void LoadBlueprints();

	void LoadMasterAchievementData();

	void LoadAchievementData(int playerID);
}
