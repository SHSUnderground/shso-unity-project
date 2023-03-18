public class CardGameInviteNotificationData : InviteNotificationData
{
	public CardGameInviteNotificationData(Matchmaker2.Invitation invitation)
		: base(NotificationType.CardGameInvite, invitation)
	{
	}
}
