using UnityEngine;

public class RoomNetTest : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string userName = "foo";

	public string userPassword = "bar";

	public string serverName = "int-rtc01.shs.tas";

	public string roomName = "test_room";

	public string gameType = "BRAWLER";

	public string spaceName = "Daily_Bugle";

	protected NetworkManager.OnConnectDelegate onConnect;

	public void Connect(NetworkManager.OnConnectDelegate onConnectDelegate)
	{
		if (userName == string.Empty || userPassword == string.Empty)
		{
			onConnectDelegate(false, "Missing user name or password");
			return;
		}
		onConnect = onConnectDelegate;
		AppShell.Instance.EventMgr.AddListener<LoginCompleteMessage>(OnLoginComplete);
		AppShell.Instance.ServerConnection.Login(userName.Trim(), userPassword.Trim());
	}

	protected void OnLoginComplete(LoginCompleteMessage msg)
	{
		AppShell.Instance.EventMgr.RemoveListener<LoginCompleteMessage>(OnLoginComplete);
		if (msg.status == LoginCompleteMessage.LoginStatus.LoginSucceeded)
		{
			AppShell.Instance.ServerConnection.ConnectToGame("shs.all", GetTicket(), onConnect);
			onConnect = null;
		}
		else
		{
			CspUtils.DebugLog("Login failed: " + msg.message);
			onConnect(false, "Login failed");
			onConnect = null;
		}
	}

	protected Matchmaker2.Ticket GetTicket()
	{
		if (string.IsNullOrEmpty(serverName))
		{
			string str = AppShell.Instance.ServerConfig.TryGetString("//environment", "DEV");
			serverName = AppShell.Instance.ServerConfig.TryGetString("//" + str + "/smartfox/server", null);
		}
		if (string.IsNullOrEmpty(roomName))
		{
			roomName = "test-room";
		}
		if (string.IsNullOrEmpty(spaceName))
		{
			spaceName = "Daily_Bugle";
		}
		if (string.IsNullOrEmpty(gameType))
		{
			gameType = "BRAWLER";
		}
		string ticket = "<ticket><server>" + serverName + "</server><instance>" + roomName + "</instance><zone>" + spaceName + "</zone><game>" + gameType + "</game></ticket>";
		return new Matchmaker2.Ticket(Matchmaker2.Ticket.Status.SUCCESS, serverName, ticket, null);
	}
}
