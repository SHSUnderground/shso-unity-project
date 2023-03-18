using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("ScenarioEvent/TransferBox")]
public class ScenarioEventTransferBox : ScenarioEventHandlerEnableBase
{
	public float timeDelay;

	public string[] triggeringEvents = new string[1]
	{
		string.Empty
	};

	public string[] triggeringEventsMultiplayer;

	public string sendEvent;

	public bool repeatable;

	protected Dictionary<string, bool> triggeredEvents;

	protected float sendTime;

	protected bool subscribed;

	protected override void Start()
	{
		base.Start();
		sendTime = 0f;
		subscribed = false;
		if (enableEvent == string.Empty)
		{
			subscribeEvents();
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		unsubscribeEvents();
	}

	protected virtual void Update()
	{
		if (sendTime > 0f && Time.time >= sendTime)
		{
			fireScenarioEvent();
			sendTime = 0f;
			if (repeatable)
			{
				triggeredEvents = new Dictionary<string, bool>();
			}
			else
			{
				unsubscribeEvents();
			}
		}
	}

	protected virtual void subscribeEvents()
	{
		if (!subscribed)
		{
			subscribed = true;
			triggeredEvents = new Dictionary<string, bool>();
			string[] array = triggeringEvents;
			foreach (string eventName in array)
			{
				ScenarioEventManager.Instance.SubscribeScenarioEvent(eventName, OnTriggeredEvent);
			}
			string[] array2 = triggeringEventsMultiplayer;
			foreach (string eventName2 in array2)
			{
				ScenarioEventManager.Instance.SubscribeScenarioEvent(eventName2, OnTriggeredEvent);
			}
		}
	}

	protected void unsubscribeEvents()
	{
		if (subscribed && ScenarioEventManager.Instance != null)
		{
			subscribed = false;
			string[] array = triggeringEvents;
			foreach (string eventName in array)
			{
				ScenarioEventManager.Instance.UnsubscribeScenarioEvent(eventName, OnTriggeredEvent);
			}
			string[] array2 = triggeringEventsMultiplayer;
			foreach (string eventName2 in array2)
			{
				ScenarioEventManager.Instance.UnsubscribeScenarioEvent(eventName2, OnTriggeredEvent);
			}
		}
	}

	protected void OnTriggeredEvent(string eventName)
	{
		if (!triggeredEvents.ContainsKey(eventName))
		{
			triggeredEvents.Add(eventName, true);
			int num = triggeringEvents.Length;
			if (PlayerCombatController.GetPlayerCount() > 2)
			{
				num += triggeringEventsMultiplayer.Length;
			}
			if (triggeredEvents.Count == num)
			{
				beginTimer();
			}
		}
	}

	protected void beginTimer()
	{
		sendTime = timeDelay + Time.time;
	}

	protected virtual void fireScenarioEvent()
	{
		ScenarioEventManager.Instance.FireScenarioEvent(sendEvent, true);
	}

	protected override void OnEnableEvent(string eventName)
	{
		subscribeEvents();
	}

	protected override void OnDisableEvent(string eventName)
	{
		unsubscribeEvents();
		sendTime = 0f;
	}

	public override void ManualReset()
	{
		subscribeEvents();
		sendTime = 0f;
	}

	public override void DrawTriggerGizmo(string triggerEventName, GameObject triggerObject)
	{
		string[] array = triggeringEvents;
		foreach (string a in array)
		{
			if (a == triggerEventName)
			{
				Gizmos.color = Color.cyan;
				Gizmos.DrawLine(base.transform.position, triggerObject.transform.position);
				Gizmos.DrawSphere(base.transform.position, 0.5f);
			}
		}
	}

	protected virtual void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "ScenarioEventTransferBoxIcon.png");
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		if (!(sendEvent != string.Empty))
		{
			return;
		}
		GameObject[] array = Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		ScenarioEventHandlerBase scenarioEventHandlerBase = null;
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			scenarioEventHandlerBase = (gameObject.GetComponent(typeof(ScenarioEventHandlerBase)) as ScenarioEventHandlerBase);
			if (scenarioEventHandlerBase != null)
			{
				scenarioEventHandlerBase.DrawTriggerGizmo(sendEvent, base.gameObject);
			}
		}
	}
}
