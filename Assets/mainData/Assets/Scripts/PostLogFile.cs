using System.Collections.Generic;
using System.Text;
using System.Xml.XPath;
using UnityEngine;

public class PostLogFile
{
	private static bool _isDone;

	public static bool IsDone
	{
		get
		{
			return _isDone;
		}
		set
		{
			_isDone = value;
		}
	}

	public static void PostToServer()
	{
		if (AppShell.Instance == null || AppShell.Instance.ServerConfig == null || !AppShell.Instance.ServerConfig.TryGetBool("//log_report", false))
		{
			return;
		}
		List<string> list = new List<string>();
		foreach (XPathNavigator item in Utils.Enumerate(AppShell.Instance.ServerConfig.GetValues("logreport/server")))
		{
			list.Add("http://" + item.Value + "/report/logs/");
		}
		if (list.Count <= 0)
		{
			return;
		}
		int @int = AppShell.Instance.ServerConfig.GetInt("//log_report", "maxLines");
		long num = -1L;
		if (AppShell.Instance.Profile != null)
		{
			num = AppShell.Instance.Profile.UserId;
		}
		if (num == -1 && AppShell.Instance.ServerConnection != null)
		{
			PlayerDictionary.Player value;
			AppShell.Instance.PlayerDictionary.TryGetValue(AppShell.Instance.ServerConnection.GetGameUserId(), out value);
			if (value != null)
			{
				num = value.PlayerId;
			}
		}
		string text = AppShell.Instance.ServerConnection.NotificationServerName;
		if (string.IsNullOrEmpty(text))
		{
			text = "<unknown>";
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(string.Format("UserId: {0}", num));
		stringBuilder.AppendLine(string.Format("Notif: {0}", text));
		stringBuilder.AppendLine(string.Format("Unity: {0} version {1}", Application.platform.ToString(), Application.unityVersion));
		stringBuilder.AppendLine(string.Format("Time Running: {0}", Time.realtimeSinceStartup.ToString()));
		stringBuilder.AppendLine(string.Format("OS: {0}", SystemInfo.operatingSystem));
		stringBuilder.AppendLine(string.Format("CPU: {0} Count: {1}", SystemInfo.processorType, SystemInfo.processorCount));
		stringBuilder.AppendLine(string.Format("RAM: {0}", SystemInfo.systemMemorySize));
		stringBuilder.AppendLine(string.Format("GPU: {0} VRAM: {1} Fill: {2}", SystemInfo.graphicsDeviceName, SystemInfo.graphicsMemorySize, SystemInfo.graphicsPixelFillrate));
		List<SHSLogEntry> logEntries = SHSDebug.LogEntries;
		for (int i = Mathf.Max(logEntries.Count - @int, 0); i < logEntries.Count; i++)
		{
			stringBuilder.AppendLine(string.Format("[{0}] {1}: {2}", logEntries[i].TimeStamp.ToString("HH:mm:ss"), logEntries[i].EntryType.ToString(), logEntries[i].Message));
		}
		_isDone = false;
		string text2 = list[Random.Range(0, list.Count)];
		byte[] bytes = new UTF8Encoding(true).GetBytes(stringBuilder.ToString().ToCharArray());
		CspUtils.DebugLog("Posting log to <" + text2 + ">");
		WWW www = new WWW(text2, bytes);
		PostLogFilePoll postLogFilePoll = AppShell.Instance.gameObject.AddComponent<PostLogFilePoll>();
		postLogFilePoll.www = www;
	}
}
