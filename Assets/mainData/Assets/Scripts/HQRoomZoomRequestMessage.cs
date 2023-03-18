public class HQRoomZoomRequestMessage : ShsEventMessage
{
	public readonly bool up;

	public HQRoomZoomRequestMessage(bool up)
	{
		this.up = up;
	}
}
