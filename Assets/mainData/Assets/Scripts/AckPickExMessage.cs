public class AckPickExMessage : ShsEventMessage
{
	public readonly int PlayerId;

	public readonly int PickType;

	public readonly int iData;

	public readonly string sData;

	public AckPickExMessage(int _PlayerId, int _PickType, int _iData, string _sData)
	{
		PlayerId = _PlayerId;
		PickType = _PickType;
		iData = _iData;
		sData = _sData;
	}
}
