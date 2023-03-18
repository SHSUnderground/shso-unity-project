using UnityEngine;

public class SHSCardGameInvitationWindow : SHSInvitationShell
{
	private bool inviteValid = true;

	private string cancelledMessage = string.Empty;

	public SHSCardGameInvitationWindow(Matchmaker2.Invitation invitation)
		: base(invitation)
	{
	}

	public override void BuildViewWindow()
	{
		Matchmaker2.CardGameInvitation cardGameInvitation = (Matchmaker2.CardGameInvitation)base.Invitation;
		base.ViewWindow = new SHSInvitationViewWindow(this);
		base.ViewWindow.IconSetup("hud_bundle|cardgame01", new Vector2(-75f, -85f), new Vector2(256f, 256f));
		base.ViewWindow.BuildWindow(cardGameInvitation.inviterName, AppShell.Instance.stringTable["#cardinvite_title"], AssetBundleLoader.BundleGroup.CardGame);
		base.ViewWindow.AddAcceptEvent(acceptButton_Click);
		base.ViewWindow.AddDeclineEvent(declineButton_Click);
		base.ViewWindow.Id = "CARD GAME WINDOW";
	}

	private void declineButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		HandleInvitationResponse(false);
	}

	private void acceptButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		HandleInvitationResponse(true);
	}

	protected override void HandleInvitationResponse(bool accept)
	{
		if (accept)
		{
			if (inviteValid)
			{
				SHSCardGameGadgetWindow sHSCardGameGadgetWindow = new SHSCardGameGadgetWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.Main);
				sHSCardGameGadgetWindow.SetupForCardGameInvitee(base.Invitation.invitationId);
				GUIManager.Instance.ShowDynamicWindow(sHSCardGameGadgetWindow, GUIControl.ModalLevelEnum.Default);
			}
			else
			{
				GUIManager.Instance.ShowDialogWithTitle(GUIManager.DialogTypeEnum.OkDialog, "Can't go into card game!", cancelledMessage, null, GUIControl.ModalLevelEnum.Full);
			}
		}
		else
		{
			AppShell.Instance.Matchmaker2.RejectCardGame(base.Invitation.invitationId, true);
		}
		base.ViewWindow.PerformCloseProcess();
	}

	public override void OnCancelled(InvitationCancelReason reason)
	{
		base.OnCancelled(reason);
		switch (reason)
		{
		case InvitationCancelReason.PlayerCanceled:
			cancelledMessage = "Sorry,  " + base.Invitation.inviterName + " has cancelled the card game";
			break;
		case InvitationCancelReason.GameFull:
			cancelledMessage = "Sorry, this card game is full.";
			break;
		case InvitationCancelReason.TimedOut:
			base.ViewWindow.PerformCloseProcess();
			break;
		case InvitationCancelReason.GameIsStarting:
			cancelledMessage = "Sorry, this card game has already started.";
			break;
		}
		inviteValid = false;
	}
}
