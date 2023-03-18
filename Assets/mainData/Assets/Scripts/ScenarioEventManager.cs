using System.Collections.Generic;

public class ScenarioEventManager
{
	public delegate void ScenarioEventCallback(string eventName);

	protected static ScenarioEventManager instance;

	protected Dictionary<string, List<ScenarioEventCallback>> eventCallbacks;

	protected ScenarioEventRecorder eventRecorder;

	public static ScenarioEventManager Instance
	{
		get
		{
			return instance;
		}
	}

	public ScenarioEventManager()
	{
		eventCallbacks = new Dictionary<string, List<ScenarioEventCallback>>();
		eventRecorder = new ScenarioEventRecorder();
		instance = this;
	}

	public void Destroy()
	{
		if (eventRecorder != null)
		{
			eventRecorder.ClearRecord();
			eventRecorder = null;
		}
		instance = null;
	}

	public void FireScenarioEvent(string eventName, bool remote)
	{
		if (!remote && AppShell.Instance.ServerConnection != null)
		{
			ScenarioEventMessage msg = new ScenarioEventMessage(eventName);
			AppShell.Instance.ServerConnection.SendGameMsg(msg);
		}
		if (eventCallbacks.ContainsKey(eventName))
		{
			List<ScenarioEventCallback> list = new List<ScenarioEventCallback>(eventCallbacks[eventName]);
			foreach (ScenarioEventCallback item in list)
			{
				item(eventName);
			}
		}
		if (eventRecorder != null)
		{
			eventRecorder.Record(eventName);
		}
	}

	public void SubscribeScenarioEvent(string eventName, ScenarioEventCallback callback)
	{
		if (!eventCallbacks.ContainsKey(eventName))
		{
			List<ScenarioEventCallback> value = new List<ScenarioEventCallback>();
			eventCallbacks.Add(eventName, value);
		}
		if (eventCallbacks[eventName].Contains(callback))
		{
			CspUtils.DebugLog("SubscribeScenarioEvent called with a duplicate event and callback.  Event: " + eventName);
		}
		else
		{
			eventCallbacks[eventName].Add(callback);
		}
	}

	public void UnsubscribeScenarioEvent(string eventName, ScenarioEventCallback callback)
	{
		if (!eventCallbacks.ContainsKey(eventName))
		{
			CspUtils.DebugLog("UnsubscribeScenarioEvent called with nonexistent event : " + eventName);
		}
		else if (!eventCallbacks[eventName].Remove(callback))
		{
			CspUtils.DebugLog("UnsubscribeScenarioEvent called with no matching callback for event : " + eventName);
		}
	}

	public void RecordEvents(bool record)
	{
		if (eventRecorder != null)
		{
			eventRecorder.IsRecording = record;
		}
	}

	public void DebugRecordEvents(bool record)
	{
		if (eventRecorder != null)
		{
			eventRecorder.IsDebugRecording = record;
		}
	}

	public void DumpEventRecord(string filePath)
	{
		if (eventRecorder != null)
		{
			eventRecorder.DumpRecord(filePath);
		}
	}

	public void LogEventRecord()
	{
		if (eventRecorder != null)
		{
			eventRecorder.LogRecord();
		}
	}

	public void ClearEventRecord()
	{
		if (eventRecorder != null)
		{
			eventRecorder.ClearRecord();
		}
	}

	public bool IsEventRecorded(string eventName)
	{
		return eventRecorder != null && eventRecorder.IsRecorded(eventName);
	}
}
