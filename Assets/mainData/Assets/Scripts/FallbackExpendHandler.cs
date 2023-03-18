public class FallbackExpendHandler : ExpendHandlerBase
{
	public override void OnExpendStart()
	{
		base.OnExpendStart();
		CspUtils.DebugLog("Fallback expend method on handler for ownable type id:" + OwnableTypeId);
	}

	public override void Update()
	{
		base.Update();
		if (State == ExpendHandlerState.Expending)
		{
			OnExpendComplete();
		}
	}
}
