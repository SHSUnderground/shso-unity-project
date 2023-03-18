using UnityEngine;

public class SHSFriendInvitationWindow : SHSInvitationShell
{
	public delegate void InitializeFinished(bool succeeded);

	private int requestorID;

	public string requestorSquadName;

	public int RequestorID
	{
		get
		{
			return requestorID;
		}
	}

	public SHSFriendInvitationWindow()
		: base(null)
	{
	}

	public void Initialize(FriendRequestMessage request, InitializeFinished onFinished)
	{
		requestorID = request.FriendID;
		request.FetchSquadName(delegate(string squadName)
		{
			if (squadName != null)
			{
				requestorSquadName = squadName;
				BuildViewWindow();
				onFinished(true);
			}
			else
			{
				onFinished(false);
			}
		});
	}

	public override void BuildViewWindow()
	{
		StringTable stringTable = AppShell.Instance.stringTable;
		base.ViewWindow = new SHSInvitationViewWindow(this);
		base.ViewWindow.IconSetup("hud_bundle|friends01", new Vector2(-70f, -85f), new Vector2(256f, 256f));
		base.ViewWindow.BuildWindow(requestorSquadName, " " + stringTable["#friendinvite_title"]);
		base.ViewWindow.AddAcceptEvent(acceptButton_Click);
		base.ViewWindow.AddDeclineEvent(declineButton_Click);
		base.ViewWindow.Id = "FRIEND WINDOW";
	}

	private void declineButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		base.ViewWindow.DeclineEnable = false;
		HandleInvitationResponse(false);
	}

	private void acceptButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		base.ViewWindow.AcceptEnable = false;
		HandleInvitationResponse(true);
	}

	protected override void HandleInvitationResponse(bool accept)
	{
		if (accept)
		{
			AppShell.Instance.Profile.AvailableFriends.AddFriend(requestorID);
		}
		else
		{
			AppShell.Instance.Profile.AvailableFriends.DeclineFriend(requestorID);
		}
		base.ViewWindow.PerformCloseProcess();
	}
}
