using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Brawler/ObjectWakeRegion")]
public class ObjectWakeRegion : ObjectRegionBase
{
	public float radius = 5f;

	public float frequency = 0.5f;

	public int frequencyWakes = 1;

	public int maximumWakes = 2;

	public CharacterSpawn[] wakeObjects;

	public string awakeDeathEvent = string.Empty;

	public int awakeDeathCountEventTrigger = 1;

	public bool disableOnDeathEvent;

	protected float wakeTime;

	protected float wakeCount;

	protected List<GameObject> awokenObjects;

	protected int awokenDeaths;

	protected void WakeObjects()
	{
		if (wakeCount >= (float)maximumWakes)
		{
			return;
		}
		List<GameObject> list = new List<GameObject>();
		Collider[] array = Physics.OverlapSphere(base.transform.position, radius);
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			Transform transform = collider.transform;
			while (transform != null)
			{
				if (!list.Contains(transform.gameObject) && !awokenObjects.Contains(transform.gameObject))
				{
					CharacterSpawn[] array3 = wakeObjects;
					foreach (CharacterSpawn characterSpawn in array3)
					{
						if (characterSpawn.CharacterName == transform.gameObject.name && checkConstraints(transform.position, false))
						{
							list.Add(transform.gameObject);
						}
					}
				}
				transform = transform.parent;
			}
		}
		int num = 0;
		while (num < frequencyWakes && list.Count > 0 && wakeCount + (float)num < (float)maximumWakes)
		{
			GameObject gameObject = list[0];
			list.RemoveAt(0);
			if (gameObject != null)
			{
				AIControllerBrawler component = gameObject.GetComponent<AIControllerBrawler>();
				if (component != null && component.IsSleeping() && component.CanWakeUpOnEvent)
				{
					awokenObjects.Add(gameObject);
					component.WakeUpOnEvent();
					num++;
				}
			}
		}
		wakeCount += num;
	}

	protected override void OnEnableEvent(string eventName)
	{
		base.OnEnableEvent(eventName);
		wakeTime = Time.time;
		wakeCount = 0f;
		if (awokenObjects == null)
		{
			awokenObjects = new List<GameObject>();
		}
		else
		{
			awokenObjects.Clear();
		}
		RegisterForEvents();
	}

	protected override void OnDisableEvent(string eventName)
	{
		base.OnDisableEvent(eventName);
		Disable();
	}

	private void Disable()
	{
		wakeTime = 0f;
		UnregisterForEvents();
	}

	private void RegisterForEvents()
	{
		if (!(AppShell.Instance == null) && AppShell.Instance.EventMgr != null && !string.IsNullOrEmpty(awakeDeathEvent))
		{
			AppShell.Instance.EventMgr.AddListener<CombatCharacterKilledMessage>(OnCombatCharacterKilled);
		}
	}

	private void UnregisterForEvents()
	{
		if (!(AppShell.Instance == null) && AppShell.Instance.EventMgr != null && !string.IsNullOrEmpty(awakeDeathEvent))
		{
			AppShell.Instance.EventMgr.RemoveListener<CombatCharacterKilledMessage>(OnCombatCharacterKilled);
		}
	}

	private void OnCombatCharacterKilled(CombatCharacterKilledMessage msg)
	{
		if (msg == null || awokenObjects == null || !awokenObjects.Contains(msg.Character))
		{
			return;
		}
		awokenObjects.Remove(msg.Character);
		awokenDeaths++;
		if (awokenDeaths >= awakeDeathCountEventTrigger && !string.IsNullOrEmpty(awakeDeathEvent) && ScenarioEventManager.Instance != null)
		{
			CspUtils.DebugLog("firing awake death event " + awakeDeathEvent + " on game object " + base.gameObject.name);
			ScenarioEventManager.Instance.FireScenarioEvent(awakeDeathEvent, true);
			awokenDeaths = 0;
			if (disableOnDeathEvent)
			{
				Disable();
			}
		}
	}

	private void Update()
	{
		if (wakeTime > 0f && Time.time >= wakeTime)
		{
			WakeObjects();
			if (frequency > 0f)
			{
				wakeTime = Time.time + frequency;
			}
			else
			{
				wakeTime = 0f;
			}
		}
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(base.transform.position, radius);
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "ObjectWakeIcon.png");
	}
}
