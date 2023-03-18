using System;
using System.Collections.Generic;
using System.Diagnostics;

public class ScenarioEventRecorder
{
	private class ScenarioEventDebugRecord
	{
		private string _event;

		private DateTime _eventTime;

		private StackTrace _eventStack;

		public string EventName
		{
			get
			{
				return _event;
			}
		}

		public DateTime EventTime
		{
			get
			{
				return _eventTime;
			}
		}

		public StackTrace EventStack
		{
			get
			{
				return _eventStack;
			}
		}

		public ScenarioEventDebugRecord(string eventName)
		{
			_event = eventName;
			_eventTime = DateTime.Now;
			_eventStack = new StackTrace(3, true);
		}
	}

	private Dictionary<string, int> _eventRecord;

	private List<ScenarioEventDebugRecord> _eventDebugRecord;

	private bool _isRecording;

	private bool _isDebugRecording;

	public bool IsRecording
	{
		get
		{
			return _isRecording;
		}
		set
		{
			_isRecording = value;
		}
	}

	public bool IsDebugRecording
	{
		get
		{
			return _isDebugRecording;
		}
		set
		{
			_isDebugRecording = value;
		}
	}

	public ScenarioEventRecorder()
	{
		_isRecording = false;
		_isDebugRecording = false;
		_eventRecord = new Dictionary<string, int>();
		_eventDebugRecord = new List<ScenarioEventDebugRecord>();
	}

	public void Record(string eventName)
	{
		if (!IsRecording)
		{
			return;
		}
		if (_eventRecord.ContainsKey(eventName))
		{
			Dictionary<string, int> eventRecord;
			Dictionary<string, int> dictionary = eventRecord = _eventRecord;
			string key;
			string key2 = key = eventName;
			int num = eventRecord[key];
			dictionary[key2] = num + 1;
		}
		else
		{
			_eventRecord.Add(eventName, 1);
		}
		if (IsDebugRecording)
		{
			ScenarioEventDebugRecord scenarioEventDebugRecord = new ScenarioEventDebugRecord(eventName);
			if (scenarioEventDebugRecord != null)
			{
				_eventDebugRecord.Add(scenarioEventDebugRecord);
			}
		}
	}

	public void DumpRecord(string filePath)
	{
	}

	public void LogRecord()
	{
		string text = "BEGIN SCENARIO EVENT RECORDER RECORD DUMP\n\n";
		string text2 = text;
		text = text2 + "TOTAL SCENARIO EVENTS FIRED: " + GetRecordCount() + "\n\n";
		text += "COUNT BREAKDOWN OF SCENARIO EVENTS FIRED [Name, Count]:\n";
		foreach (KeyValuePair<string, int> item in _eventRecord)
		{
			text2 = text;
			text = text2 + "\t" + item + "\n";
		}
		text += "\n";
		text += "DEBUG BREAKDOWN OF SCENARIO EVENTS FIRED:\n";
		foreach (ScenarioEventDebugRecord item2 in _eventDebugRecord)
		{
			text2 = text;
			text = text2 + "\t[Name]: " + item2.EventName + "\n\t[Time]: " + item2.EventTime + "\n";
			if (item2.EventStack != null)
			{
				text += "\t[Stack]: \n";
				StackFrame[] frames = item2.EventStack.GetFrames();
				foreach (StackFrame stackFrame in frames)
				{
					text2 = text;
					text = text2 + "\t\t" + stackFrame.GetMethod() + " in " + stackFrame.GetFileName() + ": line " + stackFrame.GetFileLineNumber() + "\n";
				}
			}
			text += "\n";
		}
		text += "\nEND SCENARIO EVENT RECORDER RECORD DUMP";
		CspUtils.DebugLog(text);
	}

	public void ClearRecord()
	{
		_eventRecord.Clear();
		_eventDebugRecord.Clear();
	}

	public int GetRecordCount()
	{
		int num = 0;
		foreach (int value in _eventRecord.Values)
		{
			num += value;
		}
		return num;
	}

	public bool IsRecorded(string eventName)
	{
		return _eventRecord.ContainsKey(eventName);
	}
}
