public class InviteNotificationData : NotificationData
{
	protected Matchmaker2.Invitation invitation;

	public InviteNotificationData(NotificationType notificationType, Matchmaker2.Invitation invitation)
		: base(notificationType, NotificationOrientation.Right)
	{
		CspUtils.DebugLog("InviteNotificationData " + invitation);
		this.invitation = invitation;
	}

	public Matchmaker2.Invitation getInvitation()
	{
		return invitation;
	}
}
