public class ScenarioEventObjective : ObjectiveBase
{
	public string objectiveScenarioEvent = "ObjectiveComplete";

	protected bool fired;

	public override bool IsMet()
	{
		return fired;
	}

	protected virtual void OnEnable()
	{
		ScenarioEventManager.Instance.SubscribeScenarioEvent(objectiveScenarioEvent, OnObjectiveEvent);
		fired = false;
	}

	protected virtual void OnDisable()
	{
		if (ScenarioEventManager.Instance != null)
		{
			ScenarioEventManager.Instance.UnsubscribeScenarioEvent(objectiveScenarioEvent, OnObjectiveEvent);
		}
	}

	protected void OnObjectiveEvent(string eventName)
	{
		fired = true;
	}

	public override void Reset()
	{
		fired = false;
	}
}
