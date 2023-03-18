public class InvitationBrawlerMessage : ShsEventMessage
{
	public Matchmaker2.BrawlerInvitation invitation;

	public InvitationBrawlerMessage(Matchmaker2.BrawlerInvitation invitation)
	{
		this.invitation = invitation;
	}
}
