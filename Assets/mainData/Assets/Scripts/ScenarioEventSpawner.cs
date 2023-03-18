using UnityEngine;

public class ScenarioEventSpawner : ScenarioEventHandlerBase
{
	public ScenarioEventSpawnEntry[] spawns = new ScenarioEventSpawnEntry[1]
	{
		new ScenarioEventSpawnEntry()
	};

	private void Start()
	{
		ScenarioEventSpawnEntry[] array = spawns;
		foreach (ScenarioEventSpawnEntry scenarioEventSpawnEntry in array)
		{
			ScenarioEventManager.Instance.SubscribeScenarioEvent(scenarioEventSpawnEntry.spawnEventName, OnSpawnEvent);
		}
	}

	private void OnDisable()
	{
		if (ScenarioEventManager.Instance != null)
		{
			ScenarioEventSpawnEntry[] array = spawns;
			foreach (ScenarioEventSpawnEntry scenarioEventSpawnEntry in array)
			{
				ScenarioEventManager.Instance.UnsubscribeScenarioEvent(scenarioEventSpawnEntry.spawnEventName, OnSpawnEvent);
			}
		}
	}

	protected void OnSpawnEvent(string eventName)
	{
		ScenarioEventSpawnEntry[] array = spawns;
		foreach (ScenarioEventSpawnEntry scenarioEventSpawnEntry in array)
		{
			if (scenarioEventSpawnEntry.spawnEventName == eventName)
			{
				GameObject child = Object.Instantiate(scenarioEventSpawnEntry.prefab) as GameObject;
				Utils.AttachGameObject(base.gameObject, child);
			}
		}
	}

	public override void DrawTriggerGizmo(string triggerEventName, GameObject triggerObject)
	{
		ScenarioEventSpawnEntry[] array = spawns;
		foreach (ScenarioEventSpawnEntry scenarioEventSpawnEntry in array)
		{
			if (scenarioEventSpawnEntry.spawnEventName == triggerEventName)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawLine(base.transform.position, triggerObject.transform.position);
				Gizmos.DrawSphere(base.transform.position, 0.5f);
			}
		}
	}
}
