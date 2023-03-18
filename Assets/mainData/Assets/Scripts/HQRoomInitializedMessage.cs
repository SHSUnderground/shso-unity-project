public class HQRoomInitializedMessage : ShsEventMessage
{
	public HqRoom2 room;

	public HQRoomInitializedMessage(HqRoom2 room)
	{
		this.room = room;
	}
}
