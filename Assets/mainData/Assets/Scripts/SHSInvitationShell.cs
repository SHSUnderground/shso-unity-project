public class SHSInvitationShell
{
	private Matchmaker2.Invitation invitation;

	private SHSInvitationViewWindow viewWindow;

	public Matchmaker2.Invitation Invitation
	{
		get
		{
			return invitation;
		}
	}

	public SHSInvitationViewWindow ViewWindow
	{
		get
		{
			return viewWindow;
		}
		set
		{
			viewWindow = value;
		}
	}

	public SHSInvitationShell(Matchmaker2.Invitation invitation)
	{
		this.invitation = invitation;
	}

	public virtual void BuildViewWindow()
	{
	}

	protected virtual void HandleInvitationResponse(bool accept)
	{
	}

	public virtual void OnCancelled(InvitationCancelReason reason)
	{
	}
}
