public class OnActivatedTrigger : TriggerBase
{
	public bool FireOnFirstActivation;

	private bool isFirstActivation = true;

	private void OnEnable()
	{
		if (FireOnFirstActivation || !isFirstActivation)
		{
			DetermineTriggerTargets();
			OnTriggered(base.gameObject);
		}
		isFirstActivation = false;
	}
}
