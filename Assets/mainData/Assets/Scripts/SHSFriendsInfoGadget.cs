using System.Collections.Generic;
using System.Xml.XPath;
using UnityEngine;

public class SHSFriendsInfoGadget : SHSGadget
{
	private class BaseFriendInfoWindow : GadgetRightWindow
	{
		private GUIButton blockListButton;

		private GUIButton pendingInviteButton;

		private GUIButton sentInviteButton;

		private GUILabel blockListLabel;

		private GUILabel pendingInviteLabel;

		private GUILabel sentInviteLabel;

		public BaseFriendInfoWindow(SHSFriendsInfoGadget baseWindow)
		{
			blockListButton = SetupButton(80f, 90f);
			pendingInviteButton = SetupButton(200f, 90f);
			sentInviteButton = SetupButton(320f, 90f);
			blockListLabel = SetupText(80f, 60f, "To Blocked");
			pendingInviteLabel = SetupText(200f, 60f, "To Pending Invites");
			sentInviteLabel = SetupText(320f, 60f, "To Sent Invites");
			blockListButton.Click += delegate
			{
				baseWindow.GoToBlockedList();
			};
			pendingInviteButton.Click += delegate
			{
				baseWindow.GoToPendingInvites();
			};
			sentInviteButton.Click += delegate
			{
				baseWindow.GoToSentInvites();
			};
			Add(blockListButton);
			Add(blockListLabel);
			Add(pendingInviteButton);
			Add(pendingInviteLabel);
			Add(sentInviteButton);
			Add(sentInviteLabel);
		}

		protected GUIButton SetupButton(float x, float y)
		{
			GUIButton gUIButton = GUIControl.CreateControlCenter<GUIButton>(new Vector2(64f, 64f), new Vector2(x, y));
			gUIButton.StyleInfo = new SHSButtonStyleInfo("persistent_bundle|brawler_gadget_powerON_placeholder", SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
			gUIButton.HitTestType = HitTestTypeEnum.Circular;
			gUIButton.HitTestSize = new Vector2(0.8f, 0.8f);
			return gUIButton;
		}

		protected GUILabel SetupText(float x, float y, string text)
		{
			GUILabel gUILabel = GUIControl.CreateControlCenter<GUILabel>(new Vector2(120f, 64f), new Vector2(x, y));
			gUILabel.Text = text;
			gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 21, Color.black, TextAnchor.MiddleCenter);
			gUILabel.Traits.HitTestType = HitTestTypeEnum.Transparent;
			return gUILabel;
		}
	}

	private class BlockListInfoWindow : BaseFriendInfoWindow
	{
		private int blockedId;

		private SHSFriendsInfoGadget baseWindow;

		private GUIButton removeFromBlockListButton;

		private GUILabel removeFromBlockListLabel;

		public BlockListInfoWindow(SHSFriendsInfoGadget baseWindow)
			: base(baseWindow)
		{
			this.baseWindow = baseWindow;
			removeFromBlockListButton = SetupButton(200f, 200f);
			removeFromBlockListLabel = SetupText(200f, 180f, "Remove From BlockedList");
			removeFromBlockListButton.IsEnabled = false;
			removeFromBlockListButton.Click += RemoveFromBlockedList;
			Add(removeFromBlockListButton);
			Add(removeFromBlockListLabel);
		}

		public void RemoveFromBlockedList(GUIControl sender, GUIClickEvent EventData)
		{
			baseWindow.profile.AvailableFriends.RemoveBlocked(blockedId);
			baseWindow.blockList.RemoveFriend(blockedId);
		}

		public void BlockedSelected(string blockedName, int blockedId)
		{
			this.blockedId = blockedId;
			removeFromBlockListButton.IsEnabled = true;
		}
	}

	private class PendingInvitesInfoWindow : BaseFriendInfoWindow
	{
		private int friendId;

		private SHSFriendsInfoGadget baseWindow;

		private GUIButton acceptFriendInviteButton;

		private GUILabel acceptFriendInviteLabel;

		public PendingInvitesInfoWindow(SHSFriendsInfoGadget baseWindow)
			: base(baseWindow)
		{
			this.baseWindow = baseWindow;
			acceptFriendInviteButton = SetupButton(200f, 200f);
			acceptFriendInviteLabel = SetupText(200f, 180f, "Confirm Friend Request");
			acceptFriendInviteButton.IsEnabled = false;
			acceptFriendInviteButton.Click += AcceptFriend;
			Add(acceptFriendInviteButton);
			Add(acceptFriendInviteLabel);
		}

		public void AcceptFriend(GUIControl sender, GUIClickEvent EventData)
		{
			baseWindow.profile.AvailableFriends.AddFriend(friendId);
			baseWindow.pendingInvites.RemoveFriend(friendId);
		}

		public void FriendSelected(string friendName, int friendId)
		{
			this.friendId = friendId;
			acceptFriendInviteButton.IsEnabled = true;
		}
	}

	private class SentInvitesInfoWindow : BaseFriendInfoWindow
	{
		public SentInvitesInfoWindow(SHSFriendsInfoGadget baseWindow)
			: base(baseWindow)
		{
		}
	}

	private class TextTopWindow : GadgetTopWindow
	{
		public GUILabel text;

		public TextTopWindow()
		{
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(592f, 141f), new Vector2(0f, 10f));
			gUIImage.TextureSource = "persistent_bundle|gadget_topmodule";
			Add(gUIImage);
			text = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(312f, 46f), new Vector2(0f, 0f));
			text.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 21, Color.black, TextAnchor.MiddleCenter);
			Add(text);
		}
	}

	private SHSFriendSelect blockList;

	private SHSFriendSelect pendingInvites;

	private SHSFriendSelect sentInvites;

	private BlockListInfoWindow blockListInfoWindow;

	private PendingInvitesInfoWindow pendingInvitesInfoWindow;

	private SentInvitesInfoWindow sentInvitesInfoWindow;

	private TextTopWindow textTopWindow;

	private UserProfile profile;

	public SHSFriendsInfoGadget()
	{
		CspUtils.DebugLog("SHSFriendsInfoGadget called!");
		profile = AppShell.Instance.Profile;
		textTopWindow = new TextTopWindow();
		blockList = new SHSFriendSelect(BlockedSelected);
		pendingInvites = new SHSFriendSelect(FriendSelected);
		sentInvites = new SHSFriendSelect(null);
		blockListInfoWindow = new BlockListInfoWindow(this);
		pendingInvitesInfoWindow = new PendingInvitesInfoWindow(this);
		sentInvitesInfoWindow = new SentInvitesInfoWindow(this);
		AppShell.Instance.WebService.StartRequest("resources$users/" + profile.UserId + "/friends.py", OnFriendsFetchResponse);
		SetupOpeningWindow(BackgroundType.TwoPanel, pendingInvites, pendingInvitesInfoWindow);
		textTopWindow.text.Text = "#pending_invites";
		SetupOpeningTopWindow(textTopWindow);
	}

	public void BlockedSelected(string blockedName, int blockedId)
	{
		blockListInfoWindow.BlockedSelected(blockedName, blockedId);
	}

	public void FriendSelected(string blockedName, int blockedId)
	{
		pendingInvitesInfoWindow.FriendSelected(blockedName, blockedId);
	}

	public void GoToBlockedList()
	{
		SetLeftWindow(blockList);
		SetRightWindow(blockListInfoWindow);
		textTopWindow.text.Text = "#blocked_list";
	}

	public void GoToPendingInvites()
	{
		SetLeftWindow(pendingInvites);
		SetRightWindow(pendingInvitesInfoWindow);
		textTopWindow.text.Text = "#pending_invites";
	}

	public void GoToSentInvites()
	{
		SetLeftWindow(sentInvites);
		SetRightWindow(sentInvitesInfoWindow);
		textTopWindow.text.Text = "#sent_invites";
	}

	protected void OnFriendsFetchResponse(ShsWebResponse response)
	{
		if (response.Status != 200)
		{
			CspUtils.DebugLog("friendsFriend request failed for <" + response.RequestUri + "> with status " + response.Status);
			return;
		}
		//CspUtils.DebugLog("OnFriendsFetchResponse response.Body=" + response.Body);
		DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
		dataWarehouse.Parse();
		LoadFriends(dataWarehouse.GetData("//ignores"), "ignore", blockList);
		LoadFriends(dataWarehouse.GetData("//b"), "f", pendingInvites);
		LoadFriends(dataWarehouse.GetData("//a"), "f", sentInvites);
	}

	public void LoadFriends(DataWarehouse data, string elementName, SHSFriendSelect windowToAddTo)
	{
		List<SHSFriendSelect.FriendItem> list = new List<SHSFriendSelect.FriendItem>();
		XPathNodeIterator values = data.GetValues(elementName);
		foreach (XPathNavigator item2 in Utils.Enumerate(values))
		{
			DataWarehouse dataWarehouse = new DataWarehouse(item2);
			SHSFriendSelect.FriendItem item = new SHSFriendSelect.FriendItem(dataWarehouse.TryGetString("name", "<DataCorrupt>"), false, false, string.Empty, dataWarehouse.TryGetInt("id", -1), windowToAddTo);
			list.Add(item);
		}
		windowToAddTo.AddList(list);
	}
}
