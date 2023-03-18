using UnityEngine;

public class ScenarioEventCounter : ScenarioEventHandlerEnableBase
{
	public int countRequirement;

	public string newScenarioEvent;

	public bool repeatable;

	protected int currentFireCount;

	protected bool fired;

	protected override void OnEnableEvent(string eventName)
	{
		if (fired)
		{
			return;
		}
		currentFireCount++;
		if (currentFireCount >= countRequirement)
		{
			ScenarioEventManager.Instance.FireScenarioEvent(newScenarioEvent, false);
			if (!repeatable)
			{
				fired = true;
			}
			else
			{
				currentFireCount = 0;
			}
		}
	}

	public override void ManualReset()
	{
		fired = false;
		currentFireCount = 0;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "CounterEvent.png");
	}
}
