public class FallbackPreEffectHandler : ExpendHandlerBase
{
	public override void OnExpendPreEffect()
	{
		base.OnExpendPreEffect();
		CspUtils.DebugLog("Fallback pre-effect handler called. Please reference a suitable handler for ownable type id:" + OwnableTypeId);
	}
}
