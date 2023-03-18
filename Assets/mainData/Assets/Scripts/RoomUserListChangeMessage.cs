public class RoomUserListChangeMessage : ShsEventMessage
{
	public int roomId;

	public RoomUserListChangeMessage(int roomId)
	{
		this.roomId = roomId;
	}
}
