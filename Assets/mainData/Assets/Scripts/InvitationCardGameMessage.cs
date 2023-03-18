public class InvitationCardGameMessage : ShsEventMessage
{
	public Matchmaker2.CardGameInvitation invitation;

	public InvitationCardGameMessage(Matchmaker2.CardGameInvitation invitation)
	{
		this.invitation = invitation;
	}
}
