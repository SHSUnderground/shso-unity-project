using System.Xml;
using UnityEngine;

public class SHSBrawlerInvitationWindow : SHSInvitationShell
{
	private bool inviteValid = true;

	private bool acceptPending;

	private string cancelledMessage = string.Empty;

	public SHSBrawlerInvitationWindow(Matchmaker2.Invitation invitation)
		: base(invitation)
	{
		CspUtils.DebugLog("SHSBrawlerInvitationWindow called!");
	}

	public override void BuildViewWindow()
	{
		Matchmaker2.BrawlerInvitation brawlerInvitation = (Matchmaker2.BrawlerInvitation)base.Invitation;
		AssetBundleLoader.BundleGroup bundleDependency = AssetBundleLoader.BundleGroup.MissionsAndEnemies;
		if (brawlerInvitation.missionId == LauncherSequences.FixedMissionKey)
		{
			bundleDependency = AssetBundleLoader.BundleGroup.SpecialMission;
		}
		base.ViewWindow = new SHSInvitationViewWindow(this);
		base.ViewWindow.IconSetup("hud_bundle|brawler01", new Vector2(-70f, -85f), new Vector2(256f, 256f));
		base.ViewWindow.BuildWindow(brawlerInvitation.inviterName, AppShell.Instance.stringTable["#missioninvite_title"], bundleDependency);
		base.ViewWindow.AddAcceptEvent(acceptButton_Click);
		base.ViewWindow.AddDeclineEvent(declineButton_Click);
		base.ViewWindow.Id = "MISSION WINDOW";
	}

	private void declineButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		base.ViewWindow.DeclineEnable = false;
		HandleInvitationResponse(false);
	}

	private void acceptButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		PlayAcceptVO();
		acceptPending = true;
		base.ViewWindow.AcceptEnable = false;
		HandleInvitationResponse(true);
	}

	protected void PlayAcceptVO()
	{
		if (AppShell.Instance.Profile != null)
		{
			VOManager.Instance.PlayVO("mission_multi", VOInputString.FromStrings(AppShell.Instance.Profile.LastSelectedCostume));
		}
	}

	protected void GenerateErrorInvitationDialog()
	{
		SHSErrorNotificationWindow sHSErrorNotificationWindow = new SHSErrorNotificationWindow(SHSErrorNotificationWindow.ErrorIcons.MissionNoGo);
		sHSErrorNotificationWindow.TitleText = "#error_oops_title";
		sHSErrorNotificationWindow.Text = cancelledMessage;
		sHSErrorNotificationWindow.AllowTimeout = false;
		GUIManager.Instance.ShowDynamicWindow(sHSErrorNotificationWindow, "Notification Root", GUIControl.ModalLevelEnum.Full);
	}

	protected override void HandleInvitationResponse(bool accept)
	{
		if (accept)
		{
			if (inviteValid)
			{
				AppShell.Instance.Matchmaker2.AcceptBrawler(base.Invitation.invitationId, OnAccept);
				return;
			}
			GenerateErrorInvitationDialog();
			base.ViewWindow.PerformCloseProcess();
		}
		else
		{
			AppShell.Instance.Matchmaker2.CancelBrawler(base.Invitation.invitationId, true);
			base.ViewWindow.PerformCloseProcess();
		}
	}

	public override void OnCancelled(InvitationCancelReason reason)
	{
		base.OnCancelled(reason);
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
			base.ViewWindow.PerformCloseProcess();
			break;
		case InvitationCancelReason.GameIsStarting:
			cancelledMessage = "#airlock_invitation_bad_1";
			break;
		}
		if (acceptPending)
		{
			if (reason != 0)
			{
				GenerateErrorInvitationDialog();
			}
			base.ViewWindow.PerformCloseProcess();
		}
	}

	public void OnAccept(Matchmaker2.Ticket ticket)
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
			GenerateErrorInvitationDialog();
			base.ViewWindow.PerformCloseProcess();
		}
	}
}
