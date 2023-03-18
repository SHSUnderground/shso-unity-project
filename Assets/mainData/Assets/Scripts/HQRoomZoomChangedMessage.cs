public class HQRoomZoomChangedMessage : ShsEventMessage
{
	public readonly int ZoomLevel;

	public HQRoomZoomChangedMessage(int ZoomLevel)
	{
		this.ZoomLevel = ZoomLevel;
	}
}
