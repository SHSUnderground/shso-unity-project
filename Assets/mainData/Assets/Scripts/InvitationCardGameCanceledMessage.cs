public class InvitationCardGameCanceledMessage : ShsEventMessage
{
	public Matchmaker2.CardGameInvitation invitation;

	public InvitationCancelReason reason;

	public InvitationCardGameCanceledMessage(Matchmaker2.CardGameInvitation invitation, InvitationCancelReason reason)
	{
		this.invitation = invitation;
		this.reason = reason;
	}
}
