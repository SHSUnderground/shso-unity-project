using UnityEngine;

[AddComponentMenu("ScenarioEvent/WakeEnemies")]
public class ScenarioEventWakeEnemies : ScenarioEventHandlerEnableBase
{
	protected override void OnEnableEvent(string eventName)
	{
		AppShell.Instance.EventMgr.Fire(this, new BrawlerWakeFromSleepMessage());
	}
}
