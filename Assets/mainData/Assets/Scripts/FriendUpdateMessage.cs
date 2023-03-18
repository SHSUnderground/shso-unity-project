public class FriendUpdateMessage : FriendMessage
{
	public string Status;

	public string Location;

	public bool Available;

	public FriendUpdateMessage(int friendId, string friendName, string status, string location, bool available)
		: base(friendId, friendName)
	{
		Status = status;
		Location = location;
		Available = available;
	}
}
