using UnityEngine;

[AddComponentMenu("ScenarioEvent/PeriodicEvent")]
public class ScenarioEventPeriodicEvent : ScenarioEventHandlerEnableBase
{
	public string periodicEvent;

	public float period = 1f;

	public bool startPeriod;

	public bool hostOnly = true;

	protected float periodStart;

	protected override void Start()
	{
		base.Start();
		if (startPeriod)
		{
			ResetPeriod();
		}
		else
		{
			StopPeriod();
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		StopPeriod();
	}

	protected override void OnEnableEvent(string eventName)
	{
		base.OnEnableEvent(eventName);
		if (eventName != periodicEvent && IsHost())
		{
			ScenarioEventManager.Instance.FireScenarioEvent(periodicEvent, !hostOnly);
		}
		ResetPeriod();
	}

	protected override void OnDisableEvent(string eventName)
	{
		base.OnDisableEvent(eventName);
		StopPeriod();
	}

	protected virtual void Update()
	{
		if (InPeriod() && IsPeriodEnd())
		{
			if (IsHost())
			{
				ScenarioEventManager.Instance.FireScenarioEvent(periodicEvent, !hostOnly);
			}
			ResetPeriod();
		}
	}

	protected void ResetPeriod()
	{
		periodStart = Time.time;
	}

	protected void StopPeriod()
	{
		periodStart = -1f;
	}

	protected bool InPeriod()
	{
		return periodStart >= 0f;
	}

	protected bool IsPeriodEnd()
	{
		return Time.time - periodStart >= period;
	}

	protected bool IsHost()
	{
		return !hostOnly || AppShell.Instance.ServerConnection.IsGameHost();
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "ScenarioEventPeriodicEvent.png");
	}
}
