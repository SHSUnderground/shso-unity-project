using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSGadgetInviteFriendsWindow : SHSGadget.GadgetCenterWindow
{
	public class SelectedFriendsList : GUISimpleControlWindow
	{
		public class SelectedFriendItem : GUISimpleControlWindow
		{
			public Friend StoredFriend;

			private GUILabel label;

			public SelectedFriendItem(SHSGadgetInviteFriendsWindow headWindow)
			{
				label = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(250f, 28f), Vector2.zero);
				label.SetupText(GUIFontManager.SupportedFontEnum.Komica, 21, Color.white, TextAnchor.MiddleLeft);
				label.HitTestType = HitTestTypeEnum.Rect;
				label.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
				Add(label);
				label.Click += delegate
				{
					if (StoredFriend != null)
					{
						headWindow.RemoveFriendClickedOn(StoredFriend);
					}
				};
			}

			public void StoreFriend(Friend StoredFriend)
			{
				this.StoredFriend = StoredFriend;
				bool flag = StoredFriend != null;
				label.Text = ((!flag) ? string.Empty : StoredFriend.Name);
				IsVisible = flag;
			}
		}

		private List<Friend> SelectedFriends = new List<Friend>(10);

		private List<SelectedFriendItem> playerNames = new List<SelectedFriendItem>(10);

		private GUIImage PlayWithAnyone;

		private GUIDropShadowTextLabel txt;

		private GUIDropShadowTextLabel AnyoneTxt;

		public SelectedFriendsList(SHSGadgetInviteFriendsWindow headWindow)
		{
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(599f, 531f), Vector2.zero);
			gUIImage.TextureSource = "persistent_bundle|brawler_gadget_invite_backdrop";
			Add(gUIImage);
			txt = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(285f, 100f), new Vector2(44f, -173f));
			txt.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 24, Color.white, GUILabel.GenColor(0, 21, 105), new Vector2(-2f, 2f), TextAnchor.MiddleCenter);
			Add(txt);
			PlayWithAnyone = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(251f, 253f), new Vector2(35f, -11f));
			PlayWithAnyone.TextureSource = "persistent_bundle|cardlauncher_anyone_graphic";
			Add(PlayWithAnyone);
			AnyoneTxt = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(285f, 100f), new Vector2(46f, 82f));
			AnyoneTxt.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 37, Color.white, GUILabel.GenColor(0, 22, 88), new Vector2(-2f, 2f), TextAnchor.MiddleCenter);
			AnyoneTxt.Text = PLAY_WITH_ANYONE;
			Add(AnyoneTxt);
			for (int i = 0; i < 10; i++)
			{
				SelectedFriendItem selectedFriendItem = new SelectedFriendItem(headWindow);
				selectedFriendItem.SetSize(new Vector2(250f, 28f));
				selectedFriendItem.SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(46f, 145 + 28 * i));
				playerNames.Add(selectedFriendItem);
				Add(selectedFriendItem);
			}
			RefreshFriendsList();
		}

		public void RefreshFriendsList()
		{
			bool flag = false;
			for (int i = 0; i < playerNames.Count; i++)
			{
				if (i < SelectedFriends.Count)
				{
					playerNames[i].StoreFriend(SelectedFriends[i]);
					flag = true;
				}
				else
				{
					playerNames[i].StoreFriend(null);
				}
			}
			PlayWithAnyone.IsVisible = !flag;
			AnyoneTxt.IsVisible = !flag;
			txt.Text = ((!flag) ? RIGHT_TEXT_ANYONE : RIGHT_TEXT_INFO);
		}

		public void AddFriend(Friend friend)
		{
			SelectedFriends.Add(friend);
			RefreshFriendsList();
		}

		public void RemoveFriend(Friend friend)
		{
			SelectedFriends.Remove(friend);
			RefreshFriendsList();
		}

		public string[] GetSelectedFriendsArray()
		{
			List<string> list = new List<string>(SelectedFriends.Count);
			foreach (Friend selectedFriend in SelectedFriends)
			{
				list.Add(selectedFriend.Id.ToString());
			}
			return list.ToArray();
		}
	}

	public class FriendSelection : SHSSelectionWindow<FriendItem, GUISimpleControlWindow>
	{
		public FriendSelection(GUISlider slider)
			: base(slider, 227f, new Vector2(206f, 47f), 10, (GetBackgroundLocation)SHSSelectionWindow<FriendItem, GUISimpleControlWindow>.GetBackgroundLocationOneAcross)
		{
			SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
			TopOffsetAdjustHeight = 18f;
			BottomOffsetAdjustHeight = 13f;
			slider.FireChanged();
		}
	}

	public class FriendItem : SHSSelectionItem<GUISimpleControlWindow>, IComparable<FriendItem>
	{
		public GUILabel nameLabel;

		public GUILabel locationLabel;

		private GUIButton thumbsUp;

		private GUIHotSpotButton hotSpot;

		private GUIImage friendStatus;

		public Friend friend;

		private SHSGadgetInviteFriendsWindow headWindow;

		public bool selected;

		private bool trueFriend;

		public FriendItem(Friend friend, bool trueFriend, SHSGadgetInviteFriendsWindow headWindow)
		{
			this.friend = friend;
			this.headWindow = headWindow;
			this.trueFriend = trueFriend;
			item = new GUISimpleControlWindow();
			itemSize = new Vector2(203f, 46f);
			currentState = SelectionState.Active;
			nameLabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(160f, 46f), new Vector2(14f, 7f));
			nameLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(81, 83, 78), TextAnchor.UpperLeft);
			locationLabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(160f, 46f), new Vector2(14f, 22f));
			locationLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, GUILabel.GenColor(139, 149, 96), TextAnchor.UpperLeft);
			thumbsUp = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(56f, 56f), new Vector2(79f, 0f));
			thumbsUp.StyleInfo = new SHSButtonStyleInfo("persistent_bundle|thumbsup_button");
			friendStatus = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(35f, 35f), new Vector2(-83f, 0f));
			friendStatus.TextureSource = ((!trueFriend) ? "persistent_bundle|mshs_player_indent_icon_met" : "persistent_bundle|mshs_player_indent_icon_friend");
			hotSpot = GUIControl.CreateControlFrameCentered<GUIHotSpotButton>(new Vector2(203f, 46f), new Vector2(0f, 5f));
			item.Add(nameLabel);
			item.Add(locationLabel);
			item.Add(thumbsUp);
			item.Add(friendStatus);
			item.Add(hotSpot);
			hotSpot.Click += hotSpot_Click;
			thumbsUp.Click += hotSpot_Click;
			UpdateInfo();
		}

		public void hotSpot_Click(GUIControl sender, GUIClickEvent EventData)
		{
			if (selected)
			{
				headWindow.RemoveFriendClickedOn(friend);
				return;
			}
			selected = true;
			UpdateInfo();
			headWindow.FriendClickedOn(friend);
		}

		public void UpdateInfo()
		{
			hotSpot.IsVisible = (friend.Online && friend.AvailableForActivity);
			nameLabel.Text = friend.Name;
			locationLabel.Text = friend.Location;
			thumbsUp.IsVisible = selected;
			friendStatus.Alpha = ((!friend.Online || !friend.AvailableForActivity) ? 0.4f : 1f);
			if (friend.Online && friend.AvailableForActivity)
			{
				currentState = SelectionState.Active;
				return;
			}
			thumbsUp.IsVisible = false;
			currentState = SelectionState.Passive;
		}

		public int CompareTo(FriendItem other)
		{
			if (trueFriend == other.trueFriend && friend.Online == other.friend.Online && friend.AvailableForActivity == other.friend.AvailableForActivity)
			{
				return friend.Name.CompareTo(other.friend.Name);
			}
			if (!trueFriend)
			{
				return -1;
			}
			if (friend.Online)
			{
				return -1;
			}
			if (friend.AvailableForActivity)
			{
				return -1;
			}
			return 1;
		}
	}

	public delegate void OkClickDelegate(string[] friendsToInvite);

	public FriendSelection FriendsWindow;

	private SelectedFriendsList SelectedFriendsWindow;

	public static string LEFT_TEXT = "#gadget_friends_list_left_info_text";

	public static string RIGHT_TEXT_INFO = "#gadget_friends_list_right_info_confirm_text";

	public static string RIGHT_TEXT_ANYONE = "#gadget_friends_list_right_info_anyone_text";

	public static string PLAY_WITH_ANYONE = "#gadget_play_with_anyone";

	public event OkClickDelegate OkClick;

	public SHSGadgetInviteFriendsWindow()
	{
		GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(314f, 492f), new Vector2(-238f, 40f));
		gUIImage.TextureSource = "persistent_bundle|leftselectwindow_backframe";
		GUISlider gUISlider = GUIControl.CreateControlFrameCentered<GUISlider>(new Vector2(50f, 352f), new Vector2(-108f, 68f));
		FriendsWindow = new FriendSelection(gUISlider);
		FriendsWindow.SetSize(217f, 383f);
		FriendsWindow.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-232f, 70f));
		FriendsWindow.AddList(getFriendsList());
		GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(314f, 492f), new Vector2(-238f, 40f));
		gUIImage2.TextureSource = "persistent_bundle|leftselectwindow_frontframe";
		SelectedFriendsWindow = new SelectedFriendsList(this);
		SelectedFriendsWindow.SetSize(599f, 531f);
		SelectedFriendsWindow.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(121f, 30f));
		GUIButton gUIButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(168f, 225f));
		gUIButton.StyleInfo = new SHSButtonStyleInfo("persistent_bundle|L_okbutton");
		gUIButton.HitTestSize = new Vector2(0.547f, 0.367f);
		gUIButton.Click += delegate
		{
			if (this.OkClick != null)
			{
				this.OkClick(SelectedFriendsWindow.GetSelectedFriendsArray());
			}
		};
		GUIDropShadowTextLabel gUIDropShadowTextLabel = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(258f, 100f), new Vector2(-236f, -145f));
		gUIDropShadowTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 24, Color.white, GUILabel.GenColor(0, 21, 105), new Vector2(-2f, 2f), TextAnchor.MiddleCenter);
		gUIDropShadowTextLabel.Text = LEFT_TEXT;
		Add(gUIImage);
		Add(FriendsWindow);
		Add(gUIImage2);
		Add(SelectedFriendsWindow);
		Add(gUIButton);
		Add(gUISlider);
		Add(gUIDropShadowTextLabel);
	}

	public void RemoveFriendClickedOn(Friend toRemove)
	{
		SelectedFriendsWindow.RemoveFriend(toRemove);
		FriendItem friendItem = FriendsWindow.items.Find(delegate(FriendItem friend)
		{
			return friend.friend == toRemove;
		});
		if (friendItem != null)
		{
			friendItem.selected = false;
			friendItem.UpdateInfo();
		}
	}

	public void FriendClickedOn(Friend friend)
	{
		SelectedFriendsWindow.AddFriend(friend);
	}

	public void PreSelectFriendsAndTempFriends(params Friend[] tempFriends)
	{
		Friend f;
		for (int i = 0; i < tempFriends.Length; i++)
		{
			f = tempFriends[i];
			FriendItem friendItem = FriendsWindow.items.Find(delegate(FriendItem friend)
			{
				return friend.friend.Id == f.Id;
			});
			if (friendItem == null)
			{
				FriendItem friendItem2 = new FriendItem(f, false, this);
				FriendsWindow.AddItem(friendItem2);
				friendItem2.hotSpot_Click(this, null);
			}
			else
			{
				friendItem.hotSpot_Click(this, null);
			}
		}
		FriendsWindow.SortItemList();
	}

	private List<FriendItem> getFriendsList()
	{
		List<FriendItem> list = new List<FriendItem>();
		UserProfile profile = AppShell.Instance.Profile;
		if (profile == null)
		{
			return list;
		}
		foreach (KeyValuePair<string, Friend> availableFriend in profile.AvailableFriends)
		{
			list.Add(new FriendItem(availableFriend.Value, true, this));
		}
		return list;
	}

	public override void OnShow()
	{
		base.OnShow();
		AppShell.Instance.EventMgr.AddListener<FriendUpdateMessage>(OnFriendUpdate);
		foreach (FriendItem item in FriendsWindow.items)
		{
			item.UpdateInfo();
		}
		FriendsWindow.SortItemList();
	}

	public override void OnHide()
	{
		base.OnHide();
		AppShell.Instance.EventMgr.RemoveListener<FriendUpdateMessage>(OnFriendUpdate);
	}

	private void OnFriendUpdate(FriendUpdateMessage message)
	{
		FriendItem friendItem = FriendsWindow.items.Find(delegate(FriendItem friend)
		{
			return friend.friend.Id == message.FriendID;
		});
		friendItem.UpdateInfo();
		FriendsWindow.SortItemList();
	}
}
