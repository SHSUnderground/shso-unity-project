using System;
using UnityEngine;

public class AndyTestScenarioEvent : ScenarioEventHandlerEnableBase
{
	public delegate void LoggingFunction(object message);

	public LoggingFunction DebugLog;

	public void ExtraFunction(object message)
	{
		CspUtils.DebugLog("Extra function called");
	}

	public virtual void OnEnable()
	{
		DebugLog = CspUtils.DebugLog;
		DebugLog = (LoggingFunction)Delegate.Combine(DebugLog, new LoggingFunction(ExtraFunction));
	}

	protected override void OnEnableEvent(string eventName)
	{
		DebugLog("On enable event called for the test scenario event!");
	}

	protected override void OnDisableEvent(string eventName)
	{
		DebugLog("On disable event called for the test scenario event!");
	}
}
