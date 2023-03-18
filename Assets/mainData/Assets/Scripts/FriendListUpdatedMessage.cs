public class FriendListUpdatedMessage : ShsEventMessage
{
	public enum Type
	{
		Reload,
		Update,
		Added,
		Removed,
		Decline
	}

	public Type UpdateType;

	public FriendMessage MessageThatCausedUpdate;

	public FriendListUpdatedMessage(Type UpdateType)
	{
		this.UpdateType = UpdateType;
	}

	public FriendListUpdatedMessage(Type UpdateType, FriendMessage MessageThatCausedUpdate)
		: this(UpdateType)
	{
		this.MessageThatCausedUpdate = MessageThatCausedUpdate;
	}
}
