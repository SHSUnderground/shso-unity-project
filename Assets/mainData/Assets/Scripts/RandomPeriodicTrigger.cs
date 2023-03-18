using System.Collections;
using UnityEngine;

public class RandomPeriodicTrigger : TriggerBase
{
	public bool PeriodicallyFire = true;

	public float PeriodMax = 60f;

	public float PeriodMin = 40f;

	public float InitialDelay;

	private void Start()
	{
		float waitTime = GetRandomPeriod();
		if (InitialDelay > 0f)
		{
			waitTime = InitialDelay;
		}
		StartCoroutine(WaitAndTrigger(waitTime));
	}

	protected IEnumerator WaitAndTrigger(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		OnTriggered(base.gameObject);
		StartCoroutine(WaitAndTrigger(GetRandomPeriod()));
	}

	private float GetRandomPeriod()
	{
		return Random.Range(PeriodMin, PeriodMax);
	}
}
