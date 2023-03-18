public class ScenarioEventIndicatorSuppressor : ScenarioEventHandlerEnableBase
{
	public bool indicatorSuppressed;

	public void Suppress()
	{
		indicatorSuppressed = true;
		if (BrawlerController.Instance != null)
		{
			BrawlerController.Instance.RemoveIndicatorEnemy(base.gameObject);
		}
	}

	public void Attract()
	{
		indicatorSuppressed = false;
		if (BrawlerController.Instance != null)
		{
			BrawlerController.Instance.AddIndicatorEnemy(base.gameObject);
		}
	}

	protected override void OnEnableEvent(string suppressEvent)
	{
		base.OnEnableEvent(suppressEvent);
		Suppress();
	}

	protected override void OnDisableEvent(string attractEvent)
	{
		base.OnDisableEvent(attractEvent);
		Attract();
	}
}
