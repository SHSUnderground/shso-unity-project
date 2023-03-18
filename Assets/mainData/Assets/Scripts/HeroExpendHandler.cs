public class HeroExpendHandler : ExpendHandlerBase
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
