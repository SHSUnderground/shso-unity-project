using UnityEngine;

public class ScenarioEventRefresher : ScenarioEventHandlerEnableBase
{
	public TriggerBase[] triggersToRefresh;

	public ScenarioEventHandlerEnableBase[] eventsToRefresh;

	protected override void OnEnableEvent(string eventName)
	{
		TriggerBase[] array = triggersToRefresh;
		foreach (TriggerBase triggerBase in array)
		{
			triggerBase.ManualReset();
		}
		ScenarioEventHandlerEnableBase[] array2 = eventsToRefresh;
		foreach (ScenarioEventHandlerEnableBase scenarioEventHandlerEnableBase in array2)
		{
			scenarioEventHandlerEnableBase.ManualReset();
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "RefresherEvent.png");
	}
}
