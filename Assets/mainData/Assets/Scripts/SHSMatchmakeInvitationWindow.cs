public class SHSMatchmakeInvitationWindow : SHSInvitationWindow
{
	protected Matchmaker2.Invitation invitation;

	public Matchmaker2.Invitation Invitation
	{
		get
		{
			return invitation;
		}
	}

	public SHSMatchmakeInvitationWindow(Matchmaker2.Invitation invitation)
	{
		this.invitation = invitation;
	}
}
