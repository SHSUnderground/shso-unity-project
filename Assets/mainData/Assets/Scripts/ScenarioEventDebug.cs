using UnityEngine;

[AddComponentMenu("ScenarioEvent/Debug")]
public class ScenarioEventDebug : ScenarioEventHandlerEnableBase
{
	protected override void OnEnableEvent(string eventName)
	{
		CspUtils.DebugLog("Received Scenario Event <" + eventName + "> as 'enable' event.");
	}

	protected override void OnDisableEvent(string eventName)
	{
		CspUtils.DebugLog("Received Scenario Event <" + eventName + "> as 'disable' event.");
	}
}
