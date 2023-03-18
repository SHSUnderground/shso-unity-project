public class BrawlerInviteNotificationData : InviteNotificationData
{
	public BrawlerInviteNotificationData(Matchmaker2.BrawlerInvitation invitation)
		: base(NotificationType.BrawlerInvite, invitation)
	{
		CspUtils.DebugLog("BrawlerInviteNotificationData " + invitation);
	}
}
