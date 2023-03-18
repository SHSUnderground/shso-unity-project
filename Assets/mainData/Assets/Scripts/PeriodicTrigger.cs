using System.Collections;
using UnityEngine;

public class PeriodicTrigger : TriggerBase
{
	public bool PeriodicallyFire = true;

	public float Period = 60f;

	public float InitialDelay;

	private void Start()
	{
		float waitTime = Period;
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
		StartCoroutine(WaitAndTrigger(Period));
	}
}
