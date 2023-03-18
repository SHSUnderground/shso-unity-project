using System.Collections;
using UnityEngine;

[AddComponentMenu("Brawler/Health Threshold Event Dispatcher")]
public class HealthEventDispatcher : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string targetName;

	public float thresholdPercent = 50f;

	public string eventNameOnBelowThreshold;

	public string eventNameOnAboveThreshold;

	public bool hostOnly = true;

	public bool dispatchSingleEvent = true;

	private GameObject target;

	private bool dispatched;

	private void Start()
	{
		StartCoroutine(AcquireTarget());
	}

	private void OnDestroy()
	{
		if (target != null)
		{
			UnregisterHealthListener(target);
		}
	}

	private IEnumerator AcquireTarget()
	{
		if (!string.IsNullOrEmpty(targetName))
		{
			target = GameObject.Find(targetName);
			while (target == null)
			{
				yield return new WaitForSeconds(1f);
				target = GameObject.Find(targetName);
			}
		}
		else
		{
			target = base.gameObject;
		}
		if (target != null)
		{
			RegisterHealthListener(target);
		}
	}

	private void RegisterHealthListener(GameObject toWatch)
	{
		AppShell.Instance.EventMgr.AddListener<CharacterStat.StatChangeEvent>(toWatch, OnStatChangeEvent);
	}

	private void UnregisterHealthListener(GameObject watched)
	{
		AppShell.Instance.EventMgr.RemoveListener<CharacterStat.StatChangeEvent>(watched, OnStatChangeEvent);
	}

	private void OnStatChangeEvent(CharacterStat.StatChangeEvent e)
	{
		if (e.StatType == CharacterStats.StatType.Health)
		{
			float num = e.MaxValue * (thresholdPercent / 100f);
			if (!string.IsNullOrEmpty(eventNameOnBelowThreshold) && e.NewValue <= num && e.OldValue > num)
			{
				FireEvent(eventNameOnBelowThreshold);
			}
			else if (!string.IsNullOrEmpty(eventNameOnAboveThreshold) && e.NewValue > num && e.OldValue <= num)
			{
				FireEvent(eventNameOnAboveThreshold);
			}
		}
	}

	private void FireEvent(string eventName)
	{
		if (!dispatchSingleEvent || !dispatched)
		{
			dispatched = true;
			if (!hostOnly || AppShell.Instance.ServerConnection.IsGameHost())
			{
				ScenarioEventManager.Instance.FireScenarioEvent(eventName, !hostOnly);
			}
		}
	}
}
