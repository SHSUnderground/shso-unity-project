using UnityEngine;

public class ScenarioEventScore : ScenarioEventHandlerEnableBase
{
	public int scoreForEvent;

	protected override void OnEnableEvent(string eventName)
	{
		AppShell.Instance.EventMgr.Fire(this, new ScenarioEventScoreMessage(scoreForEvent));
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "ScenarioEventScoreIcon.png");
	}
}
