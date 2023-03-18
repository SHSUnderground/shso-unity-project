using UnityEngine;

[AddComponentMenu("ScenarioEvent/Method")]
public class MethodScenarioEvent : ScenarioEventHandlerEnableBase
{
	public string enableMethod = string.Empty;

	public string disableMethod = string.Empty;

	protected override void OnEnableEvent(string eventName)
	{
		base.OnEnableEvent(eventName);
		if (enableMethod != string.Empty)
		{
			SendMessage(enableMethod);
		}
	}

	protected override void OnDisableEvent(string eventName)
	{
		base.OnDisableEvent(eventName);
		if (disableMethod != string.Empty)
		{
			SendMessage(disableMethod);
		}
	}
}
