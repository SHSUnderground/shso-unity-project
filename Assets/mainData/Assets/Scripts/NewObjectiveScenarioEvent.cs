using UnityEngine;

public class NewObjectiveScenarioEvent : ScenarioEventHandlerEnableBase
{
	public string newObjectiveText = string.Empty;

	public string newObjectiveIcon = string.Empty;

	protected override void OnEnableEvent(string eventName)
	{
		BrawlerController instance = BrawlerController.Instance;
		if (instance != null)
		{
			instance.DisplayNewObjective(newObjectiveText, newObjectiveIcon, !string.IsNullOrEmpty(disableEvent));
		}
	}

	protected override void OnDisableEvent(string eventName)
	{
		base.OnDisableEvent(eventName);
		SHSBrawlerMainWindow sHSBrawlerMainWindow = (SHSBrawlerMainWindow)GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow"];
		sHSBrawlerMainWindow.HideOrders();
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "NewObjectiveIcon.png");
	}
}
