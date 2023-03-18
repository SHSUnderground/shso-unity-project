using System.Collections.Generic;
using UnityEngine;

public class SHSServersViewerWindow : GUITabbedDialogWindow.GUITabbedWindow
{
	private Vector2 scrollPos;

	private SHSStyle headerStyle;

	public SHSServersViewerWindow(string name)
		: base(name, null)
	{
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		base.Draw(drawFlags);
		GUILayout.BeginArea(new Rect(30f, 30f, 550f, base.rect.height - 50f));
		scrollPos = GUILayout.BeginScrollView(scrollPos);
		IServerConnection serverConnection = AppShell.Instance.ServerConnection;
		if (serverConnection == null || (serverConnection.State & NetworkManager.ConnectionState.ConnectedToGame) == 0)
		{
			GUILayout.Label("--- No Game Server ---");
			GUILayout.Label("Connection State: " + ((serverConnection == null) ? "null" : serverConnection.State.ToString()));
			GUILayout.EndScrollView();
			GUILayout.EndArea();
			return;
		}
		GUILayout.BeginVertical();
		GUILayout.Space(10f);
		GUILayout.Label("ROOM", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
		GUILayout.Label("Connection State: " + serverConnection.State);
		GUILayout.Label("Id: " + serverConnection.GetGameRoomId());
		GUILayout.Label("Name: " + serverConnection.GetRoomName());
		GUILayout.Label("Users (Current/Max): " + serverConnection.GetGameUserCount() + "/" + serverConnection.GetRoomMaxUsers());
		GUILayout.Label("Notification Server: " + serverConnection.NotificationServerName);
		GUILayout.Label("Game Server: " + serverConnection.GameServerName);
		int gameHostId = serverConnection.GetGameHostId();
		GUILayout.Space(10f);
		GUILayout.Label("USERS", GUIManager.Instance.StyleManager.GetStyle("DebugConsoleTitle").UnityStyle);
		List<NetworkManager.UserInfo> gameAllUsers = serverConnection.GetGameAllUsers();
		Color color = GUI.color;
		foreach (NetworkManager.UserInfo item in gameAllUsers)
		{
			GUILayout.BeginHorizontal(GUILayout.Width(700f));
			GUILayout.Label(item.userId.ToString(), headerStyle.UnityStyle, GUILayout.Width(50f));
			string userName = item.userName;
			if (userName == "<unknown>")
			{
				GUI.color = Color.red;
			}
			GUILayout.Label(userName, headerStyle.UnityStyle, GUILayout.Width(100f));
			GUI.color = color;
			GUILayout.Label((item.userId != gameHostId) ? string.Empty : "HOST", headerStyle.UnityStyle, GUILayout.Width(100f));
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}

	public override void OnShow()
	{
		base.OnShow();
		headerStyle = GUIManager.Instance.StyleManager.GetStyle("DebugConsoleEventHeader");
		scrollPos = Vector2.zero;
	}
}
