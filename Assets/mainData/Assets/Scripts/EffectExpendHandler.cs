public class EffectExpendHandler : ExpendHandlerBase
{
	public override void Update()
	{
		base.Update();
		if (State == ExpendHandlerState.Expending)
		{
			OnExpendComplete();
		}
	}
}
