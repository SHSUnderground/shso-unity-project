using UnityEngine;

public class CardGameInviteNotificationWindow : InviteNotificationWindow
{
	private GUIImage _icon2;

	public CardGameInviteNotificationWindow()
		: base(NotificationData.NotificationType.CardGameInvite, "hud_bundle|cardgame01", new Vector2(256f, 256f), new Vector2(-75f, -85f), AppShell.Instance.stringTable["#cardinvite_title"])
	{
		AppShell.Instance.EventMgr.AddListener<InvitationCardGameCanceledMessage>(OnCardGameInviteCanceledMessage);
	}

	public void OnDestroy()
	{
		CspUtils.DebugLog("desotrying listener");
		AppShell.Instance.EventMgr.RemoveListener<InvitationCardGameCanceledMessage>(OnCardGameInviteCanceledMessage);
	}

	public override void init(NotificationHUD parent, NotificationData data)
	{
		base.init(parent, data);
	}

	public override void OnCancelled(InvitationCancelReason reason)
	{
		base.OnCancelled(reason);
		InviteNotificationData inviteNotificationData = (InviteNotificationData)_data;
		switch (reason)
		{
		case InvitationCancelReason.PlayerCanceled:
			cancelledMessage = "Sorry,  " + inviteNotificationData.getInvitation().inviterName + " has cancelled the card game";
			break;
		case InvitationCancelReason.GameFull:
			cancelledMessage = "Sorry, this card game is full.";
			break;
		case InvitationCancelReason.TimedOut:
			_parent.notificationComplete(this);
			break;
		case InvitationCancelReason.GameIsStarting:
			cancelledMessage = "Sorry, this card game has already started.";
			break;
		}
		inviteValid = false;
	}

	private void OnCardGameInviteCanceledMessage(InvitationCardGameCanceledMessage message)
	{
		CspUtils.DebugLog("OnCardGameInviteCanceledMessage ");
		InviteNotificationData inviteNotificationData = (InviteNotificationData)_data;
		if (inviteNotificationData.getInvitation().invitationId == message.invitation.invitationId)
		{
			CspUtils.DebugLog("Canceled invitation." + message.invitation.invitationId + " reason: " + message.reason);
			OnCancelled(message.reason);
		}
	}

	protected override void HandleInvitationResponse(bool accept)
	{
		InviteNotificationData inviteNotificationData = (InviteNotificationData)_data;
		if (accept)
		{
			if (inviteValid)
			{
				SHSCardGameGadgetWindow sHSCardGameGadgetWindow = new SHSCardGameGadgetWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.Main);
				sHSCardGameGadgetWindow.SetupForCardGameInvitee(inviteNotificationData.getInvitation().invitationId);
				GUIManager.Instance.ShowDynamicWindow(sHSCardGameGadgetWindow, ModalLevelEnum.Default);
			}
			else
			{
				GUIManager.Instance.ShowDialogWithTitle(GUIManager.DialogTypeEnum.OkDialog, "Can't go into card game!", cancelledMessage, null, ModalLevelEnum.Full);
			}
		}
		else
		{
			AppShell.Instance.Matchmaker2.RejectCardGame(inviteNotificationData.getInvitation().invitationId, true);
		}
		_parent.notificationComplete(this);
	}
}
