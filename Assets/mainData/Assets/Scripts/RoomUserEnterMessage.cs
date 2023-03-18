public class RoomUserEnterMessage : ShsEventMessage
{
	public int roomId;

	public int userId;

	public string userName;

	public RoomUserEnterMessage(int roomId, int userId, string userName)
	{
		this.roomId = roomId;
		this.userId = userId;
		this.userName = userName;
	}
}
