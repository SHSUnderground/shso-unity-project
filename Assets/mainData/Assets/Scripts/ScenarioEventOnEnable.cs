using UnityEngine;

[AddComponentMenu("ScenarioEvent/Fire On Enable")]
public class ScenarioEventOnEnable : ScenarioEventHandlerBase
{
	public string scenarioEventName;

	private void OnEnable()
	{
		if (ScenarioEventManager.Instance != null)
		{
			ScenarioEventManager.Instance.FireScenarioEvent(scenarioEventName, true);
		}
	}
}
