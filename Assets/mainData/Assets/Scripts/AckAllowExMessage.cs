public class AckAllowExMessage : ShsEventMessage
{
	public readonly int PlayerId;

	public readonly int PickType;

	public AckAllowExMessage(int _PlayerId, int _AllowType)
	{
		PlayerId = _PlayerId;
		PickType = _AllowType;
	}
}
