using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSDebug
{
	public delegate void DebugLogger(object message);

	private static bool passThrough;

	private static List<int> singleLogStringHistory;

	private static List<SHSLogEntry> logEntries;

	private static List<ILogListener> listenerList;

	public static DebugLogger Log;

	public static DebugLogger LogWarning;

	public static DebugLogger LogSingleWarning;

	public static DebugLogger LogError;

	public static DebugLogger LogSingleError;

	public static bool PassThrough
	{
		get
		{
			return passThrough;
		}
		set
		{
			if (passThrough != value)
			{
				passThrough = value;
				if (value)
				{
					Log = (DebugLogger)Delegate.Combine(Log, new DebugLogger(CspUtils.DebugLog));
				}
				else
				{
					Log = (DebugLogger)Delegate.Remove(Log, new DebugLogger(CspUtils.DebugLog));
				}
			}
		}
	}

	public static List<SHSLogEntry> LogEntries
	{
		get
		{
			return logEntries;
		}
	}

	public static List<ILogListener> ListenerList
	{
		get
		{
			return listenerList;
		}
	}

	static SHSDebug()
	{
		singleLogStringHistory = new List<int>();
		logEntries = new List<SHSLogEntry>();
		listenerList = new List<ILogListener>();
		PassThrough = true;
		Log = (DebugLogger)Delegate.Combine(Log, new DebugLogger(ManagedLog));
		LogWarning = (DebugLogger)Delegate.Combine(LogWarning, new DebugLogger(ManagedLogWarning));
		LogSingleWarning = (DebugLogger)Delegate.Combine(LogSingleWarning, new DebugLogger(ManagedLogSingleWarning));
		LogError = (DebugLogger)Delegate.Combine(LogError, new DebugLogger(ManagedLogError));
		LogSingleError = (DebugLogger)Delegate.Combine(LogSingleError, new DebugLogger(ManagedLogSingleError));
		LogWarning = (DebugLogger)Delegate.Combine(LogWarning, new DebugLogger(CspUtils.DebugLogWarning));
		LogError = (DebugLogger)Delegate.Combine(LogError, new DebugLogger(CspUtils.DebugLogError));
		string empty = string.Empty;
		empty.GetHashCode();
	}

	private static void ManagedLog(object message)
	{
		WriteLog(message, SHSLogEntry.LogEntryType.Info);
	}

	private static void ManagedLogSingleWarning(object message)
	{
		if (!singleLogStringHistory.Contains(message.GetHashCode()))
		{
			WriteLog(message, SHSLogEntry.LogEntryType.Warning);
			singleLogStringHistory.Add(message.GetHashCode());
			LogWarning(message);
		}
	}

	private static void ManagedLogSingleError(object message)
	{
		if (!singleLogStringHistory.Contains(message.GetHashCode()))
		{
			WriteLog(message, SHSLogEntry.LogEntryType.Error);
			singleLogStringHistory.Add(message.GetHashCode());
			LogError(message);
		}
	}

	private static void ManagedLogWarning(object message)
	{
		WriteLog(message, SHSLogEntry.LogEntryType.Warning);
	}

	private static void ManagedLogError(object message)
	{
		WriteLog(message, SHSLogEntry.LogEntryType.Error);
	}

	private static void WriteLog(object message, SHSLogEntry.LogEntryType logEntryType)
	{
		if (message == null)
		{
			message = "[Null]";
		}
		SHSLogEntry sHSLogEntry = new SHSLogEntry(message.ToString(), logEntryType);
		logEntries.Add(sHSLogEntry);
		sendLogListenerMessage(sHSLogEntry);
	}

	private static void sendLogListenerMessage(SHSLogEntry entry)
	{
		foreach (ILogListener listener in listenerList)
		{
			listener.OnLogEntryAdded(entry);
		}
	}

	public static void Break()
	{
		Debug.Break();
	}

	public static void DrawLine(Vector3 start, Vector3 end)
	{
		Debug.DrawLine(start, end);
	}

	public static void DrawRay(Vector3 start, Vector3 dir)
	{
		Debug.DrawRay(start, dir);
	}

	public static void AddListener(ILogListener listener)
	{
		if (!listenerList.Contains(listener))
		{
			listenerList.Add(listener);
		}
	}

	public static void RemoveListener(ILogListener listener)
	{
		if (listenerList.Contains(listener))
		{
			listenerList.Remove(listener);
		}
	}
}
