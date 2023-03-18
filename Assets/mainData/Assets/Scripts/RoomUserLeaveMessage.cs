public class RoomUserLeaveMessage : ShsEventMessage
{
	public int roomId;

	public int userId;

	public RoomUserLeaveMessage(int roomId, int userId)
	{
		this.roomId = roomId;
		this.userId = userId;
	}
}
