public class InvitationBrawlerCanceledMessage : ShsEventMessage
{
	public Matchmaker2.BrawlerInvitation invitation;

	public InvitationCancelReason reason;

	public InvitationBrawlerCanceledMessage(Matchmaker2.BrawlerInvitation invitation, InvitationCancelReason reason)
	{
		this.invitation = invitation;
		this.reason = reason;
	}
}
