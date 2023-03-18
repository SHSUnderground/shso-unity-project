using System.Collections.Generic;
using UnityEngine;

public class MultiHotspotCoordinator : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float TotalCountDownTime = 6f;

	public float TimeLockedIn = 6f;

	public float JoinEndtime = 5f;

	public ScenarioEventTime[] ScenarioEvents;

	public ShsAudioSource tickSFX;

	public ShsAudioSource lockedInTickSFX;

	private float localLaunchTime = -1f;

	protected List<GameObject> users = new List<GameObject>();

	private float lastDisplayTime = -1f;

	public float LaunchTime
	{
		get
		{
			return localLaunchTime;
		}
		set
		{
			localLaunchTime = value;
			DispatchLaunchTime(localLaunchTime);
			ScenarioEventTime[] scenarioEvents = ScenarioEvents;
			foreach (ScenarioEventTime scenarioEventTime in scenarioEvents)
			{
				scenarioEventTime.Fired = false;
			}
		}
	}

	public float DisplayTime
	{
		get
		{
			if (localLaunchTime > -1f)
			{
				return localLaunchTime - Time.time;
			}
			return TotalCountDownTime;
		}
	}

	public bool CanJoin
	{
		get
		{
			return localLaunchTime < 0f || localLaunchTime - JoinEndtime > Time.time;
		}
	}

	public void Start()
	{
		AppShell.Instance.EventMgr.AddListener<EntityDespawnMessage>(OnCharacterDespawned);
	}

	public void OnDisable()
	{
		AppShell.Instance.EventMgr.RemoveListener<EntityDespawnMessage>(OnCharacterDespawned);
	}

	public void AddUser(GameObject obj)
	{
		users.Add(obj);
	}

	public void RemoveUser(GameObject obj)
	{
		if (users.Contains(obj))
		{
			users.Remove(obj);
			if (users.Count == 0)
			{
				LaunchTime = -1f;
			}
		}
	}

	public void ReceiveLaunchTime(double serverTime)
	{
		if (localLaunchTime == -1f)
		{
			double num = serverTime - ServerTime.time;
			localLaunchTime = Time.time + (float)num;
		}
	}

	protected void DispatchLaunchTime(float localTime)
	{
		if (localTime > 0f)
		{
			NetworkComponent component = Utils.GetComponent<NetworkComponent>(base.gameObject);
			if (component != null)
			{
				float num = localTime - Time.time;
				double serverLaunchTime = ServerTime.time + (double)num;
				component.QueueNetActionIgnoringOwnership(new NetActionMultiHotspotSync(serverLaunchTime));
			}
		}
	}

	protected void Update()
	{
		if (!(localLaunchTime > -1f))
		{
			return;
		}
		ScenarioEventTime[] scenarioEvents = ScenarioEvents;
		foreach (ScenarioEventTime scenarioEventTime in scenarioEvents)
		{
			if (!scenarioEventTime.Fired && DisplayTime <= scenarioEventTime.Time)
			{
				ScenarioEventManager.Instance.FireScenarioEvent(scenarioEventTime.ScenarioEvent, false);
				scenarioEventTime.Fired = true;
			}
		}
		if (DisplayTime > 0f && Mathf.Floor(lastDisplayTime) != Mathf.Floor(DisplayTime))
		{
			if (DisplayTime < JoinEndtime && lockedInTickSFX != null && ((int)DisplayTime & 1) != 0)
			{
				ShsAudioSource.PlayAutoSound(lockedInTickSFX.gameObject, base.transform);
			}
			if (tickSFX != null)
			{
				ShsAudioSource.PlayAutoSound(tickSFX.gameObject, base.transform);
			}
		}
		lastDisplayTime = DisplayTime;
	}

	private void OnCharacterDespawned(EntityDespawnMessage e)
	{
		RemoveUser(e.go);
	}
}
