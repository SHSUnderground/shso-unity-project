using System.Xml;
using UnityEngine;

public class BrawlerInviteNotificationWindow : InviteNotificationWindow
{
	private GUIImage _icon2;

	public BrawlerInviteNotificationWindow()
		: base(NotificationData.NotificationType.BrawlerInvite, "hud_bundle|brawler01", new Vector2(256f, 256f), new Vector2(-70f, -85f), AppShell.Instance.stringTable["#missioninvite_title"])
	{
		CspUtils.DebugLog("BrawlerInviteNotificationWindow called!");
		AppShell.Instance.EventMgr.AddListener<InvitationBrawlerCanceledMessage>(OnBrawlerInviteCanceledMessage);
	}

	public void OnDestroy()
	{
		CspUtils.DebugLog("desotrying listener");
		AppShell.Instance.EventMgr.RemoveListener<InvitationBrawlerCanceledMessage>(OnBrawlerInviteCanceledMessage);
	}

	public override void init(NotificationHUD parent, NotificationData data)
	{
		CspUtils.DebugLog("init " + getData());
		BrawlerInviteNotificationData brawlerInviteNotificationData = data as BrawlerInviteNotificationData;
		CspUtils.DebugLog("actual  " + brawlerInviteNotificationData);
		Matchmaker2.BrawlerInvitation brawlerInvitation = brawlerInviteNotificationData.getInvitation() as Matchmaker2.BrawlerInvitation;
		CspUtils.DebugLog("BrawlerInviteNotificationWindow got mission " + brawlerInvitation.missionId);
		OwnableDefinition missionDef = OwnableDefinition.getMissionDef(brawlerInvitation.missionId);
		_inviteMessage = AppShell.Instance.stringTable.GetString("#missioninvite_title") + " \n  ";
		_inviteMessage += AppShell.Instance.stringTable.GetString(missionDef.shoppingName);
		base.init(parent, data);
	}

	protected override void HandleInvitationResponse(bool accept)
	{
		CspUtils.DebugLog("HandleInvitationResponse");
		InviteNotificationData inviteNotificationData = (InviteNotificationData)_data;
		if (accept)
		{
			if (inviteValid)
			{
				AppShell.Instance.Matchmaker2.AcceptBrawler(inviteNotificationData.getInvitation().invitationId, OnAccept);
				return;
			}
			GenerateErrorInvitationDialog(cancelledMessage);
			_parent.notificationComplete(this);
		}
		else
		{
			AppShell.Instance.Matchmaker2.CancelBrawler(inviteNotificationData.getInvitation().invitationId, true);
			_parent.notificationComplete(this);
		}
	}

	private void OnBrawlerInviteCanceledMessage(InvitationBrawlerCanceledMessage message)
	{
		CspUtils.DebugLog("OnBrawlerInviteCanceledMessage ");
		InviteNotificationData inviteNotificationData = (InviteNotificationData)_data;
		if (inviteNotificationData.getInvitation().invitationId == message.invitation.invitationId)
		{
			CspUtils.DebugLog("Canceled invitation." + message.invitation.invitationId + " reason: " + message.reason);
			OnCancelled(message.reason);
		}
	}

	public override void OnCancelled(InvitationCancelReason reason)
	{
		inviteValid = false;
		switch (reason)
		{
		case InvitationCancelReason.PlayerCanceled:
			inviteValid = true;
			cancelledMessage = string.Empty;
			break;
		case InvitationCancelReason.GameFull:
			cancelledMessage = "#airlock_invitation_bad_1";
			break;
		case InvitationCancelReason.TimedOut:
			_parent.notificationComplete(this);
			break;
		case InvitationCancelReason.GameIsStarting:
			cancelledMessage = "#airlock_invitation_bad_1";
			break;
		}
		if (acceptPending)
		{
			if (reason != 0)
			{
				GenerateErrorInvitationDialog(cancelledMessage);
			}
			_parent.notificationComplete(this);
		}
	}

	public override void OnAccept(Matchmaker2.Ticket ticket)
	{
		CspUtils.DebugLog(ticket + " On Accept Brawler action invoked. Ticket result:");
		if (ticket.status == Matchmaker2.Ticket.Status.SUCCESS)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(ticket.ticket);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("//mission");
			if (xmlNode == null)
			{
				CspUtils.DebugLog("Brawler ticket does not contain the mission: " + ticket.ticket);
				return;
			}
			AppShell.Instance.QueueLocationInfo();
			AppShell.Instance.SharedHashTable["BrawlerTicket"] = ticket;
			ActiveMission activeMission = new ActiveMission(xmlNode.InnerText);
			activeMission.BecomeActiveMission();
			AppShell.Instance.Transition(GameController.ControllerType.Brawler);
		}
		else
		{
			CspUtils.DebugLog(ticket.status);
			cancelledMessage = "#airlock_invitation_bad_1";
			GenerateErrorInvitationDialog(cancelledMessage);
			_parent.notificationComplete(this);
		}
	}
}
