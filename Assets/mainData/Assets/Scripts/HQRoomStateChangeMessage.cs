public class HQRoomStateChangeMessage : ShsEventMessage
{
	public string roomName;

	public HqRoom2.AccessState state;

	public HQRoomStateChangeMessage(string roomName, HqRoom2.AccessState state)
	{
		this.roomName = roomName;
		this.state = state;
	}
}
