public class FriendInviteNotificationData : NotificationData
{
	public FriendRequestMessage message;

	public FriendInviteNotificationData(FriendRequestMessage message)
		: base(NotificationType.FriendInvite, NotificationOrientation.Right)
	{
		this.message = message;
	}
}
