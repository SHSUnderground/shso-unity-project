using UnityEngine;

[AddComponentMenu("ScenarioEvent/Tester")]
public class ScenarioEventTester : ScenarioEventHandlerBase
{
	public string scenarioEventName;

	public bool fire;

	private void Update()
	{
		if (fire)
		{
			fire = false;
			ScenarioEventManager.Instance.FireScenarioEvent(scenarioEventName, true);
		}
	}
}
