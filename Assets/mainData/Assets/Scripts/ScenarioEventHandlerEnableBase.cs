using UnityEngine;

public class ScenarioEventHandlerEnableBase : ScenarioEventHandlerBase
{
	public string enableEvent = string.Empty;

	public string disableEvent = string.Empty;

	protected virtual void Start()
	{
		if (enableEvent != string.Empty)
		{
			ScenarioEventManager.Instance.SubscribeScenarioEvent(enableEvent, OnEnableEvent);
		}
		if (disableEvent != string.Empty)
		{
			ScenarioEventManager.Instance.SubscribeScenarioEvent(disableEvent, OnDisableEvent);
		}
	}

	protected virtual void OnDisable()
	{
		if (ScenarioEventManager.Instance != null)
		{
			if (enableEvent != string.Empty)
			{
				ScenarioEventManager.Instance.UnsubscribeScenarioEvent(enableEvent, OnEnableEvent);
			}
			if (disableEvent != string.Empty)
			{
				ScenarioEventManager.Instance.UnsubscribeScenarioEvent(disableEvent, OnDisableEvent);
			}
		}
	}

	protected virtual void OnEnableEvent(string eventName)
	{
	}

	protected virtual void OnDisableEvent(string eventName)
	{
	}

	public virtual void ManualReset()
	{
	}

	public override void DrawTriggerGizmo(string triggerEventName, GameObject triggerObject)
	{
		if (triggerEventName == enableEvent)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(base.transform.position, triggerObject.transform.position);
			Gizmos.DrawSphere(base.transform.position, 0.5f);
		}
		if (triggerEventName == disableEvent)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(base.transform.position, triggerObject.transform.position);
			Gizmos.DrawSphere(base.transform.position, 0.5f);
		}
	}
}
