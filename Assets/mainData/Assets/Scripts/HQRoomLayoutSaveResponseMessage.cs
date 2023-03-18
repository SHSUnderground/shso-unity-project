public class HQRoomLayoutSaveResponseMessage : ShsEventMessage
{
	public string roomId;

	public HQRoomLayoutSaveResponseMessage(string roomId)
	{
		this.roomId = roomId;
	}
}
