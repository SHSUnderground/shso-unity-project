public class HQRoomChangedMessage : ShsEventMessage
{
	public readonly string roomId;

	public HQRoomChangedMessage(string roomId)
	{
		this.roomId = roomId;
	}
}
