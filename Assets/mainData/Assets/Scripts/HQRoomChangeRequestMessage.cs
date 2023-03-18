public class HQRoomChangeRequestMessage : ShsEventMessage
{
	public enum RoomCycleDirection
	{
		Next,
		Previous
	}

	public readonly string roomId;

	public readonly RoomCycleDirection requestedDirection;

	public HQRoomChangeRequestMessage(RoomCycleDirection direction)
		: this(null)
	{
		requestedDirection = direction;
	}

	public HQRoomChangeRequestMessage(string roomId)
	{
		this.roomId = roomId;
	}
}
