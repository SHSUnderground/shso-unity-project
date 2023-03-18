using UnityEngine;

public class ThresholdTrigger : TriggerBase
{
	public int Threshold = 2;

	protected int triggerCount;

	private void Start()
	{
		triggerCount = 0;
	}

	private void GameObjectTriggered(GameObject triggeringObject)
	{
		IncrementTrigger(triggeringObject);
	}

	private void Triggered(Object triggeringObject)
	{
		IncrementTrigger(triggeringObject);
	}

	private void IncrementTrigger(Object triggeringObject)
	{
		triggerCount++;
		if (triggerCount >= Threshold)
		{
			CspUtils.DebugLog("Threshold reached <" + base.gameObject.name + "," + triggerCount + "> firing with triggering object <" + triggeringObject.name + ">.");
			if (OnTriggered(triggeringObject))
			{
				triggerCount = 0;
			}
		}
	}
}
