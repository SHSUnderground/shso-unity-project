internal class DebugWindowMessage : ShsEventMessage
{
	public enum SizeTypeEnum
	{
		FullScreen,
		HalfScreen,
		Minimal
	}

	public SizeTypeEnum SizeType;

	public DebugWindowMessage()
	{
		SizeType = SizeTypeEnum.FullScreen;
	}

	public DebugWindowMessage(SizeTypeEnum sizetype)
	{
		SizeType = sizetype;
	}
}
