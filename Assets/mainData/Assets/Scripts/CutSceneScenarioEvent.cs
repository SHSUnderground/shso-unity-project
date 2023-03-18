using UnityEngine;

[AddComponentMenu("Cut Scene Clips/Scenario Event")]
public class CutSceneScenarioEvent : CutSceneEvent
{
	public string scenarioEvent = string.Empty;

	public override void StartEvent()
	{
		base.StartEvent();
		if (scenarioEvent == string.Empty)
		{
			LogEventError("Event to fire must be a valid string for the scenario event");
		}
		else
		{
			ScenarioEventManager.Instance.FireScenarioEvent(scenarioEvent, true);
		}
	}
}
