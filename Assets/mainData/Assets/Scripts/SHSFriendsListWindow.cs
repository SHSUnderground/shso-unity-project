using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SHSFriendsListWindow : SHSHudWindows
{
	public class FriendSearch : GUISimpleControlWindow
	{
		private GUIButton AddFriend;

		private GUILabel _totalFriendsLabel;

		public FriendSearch(SHSFriendsListWindow parentWindow)
		{
			SetSize(2000f, 2000f);
			SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
			Offset = new Vector2(0f, 172f);
			Traits.EventListenerRegistrationTrait = ControlTraits.EventListenerRegistrationTraitEnum.Register;
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(81f, 81f), new Vector2(88f, -19f));
			gUIImage.TextureSource = "persistent_bundle|gadget_searchbutton_normal";
			GUITextField search = GUIControl.CreateControlFrameCentered<GUITextField>(new Vector2(178f, 55f), new Vector2(-9f, -25f));
			search.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, GUILabel.GenColor(81, 82, 81), TextAnchor.MiddleLeft);
			search.WordWrap = false;
			AddFriend = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(128f, 128f), new Vector2(-66f, 18f));
			AddFriend.HitTestType = HitTestTypeEnum.Alpha;
			AddFriend.StyleInfo = new SHSButtonStyleInfo("persistent_bundle|friends_list_add_button");
			AddFriend.ToolTip = new NamedToolTipInfo("#friendlist_addFriend", new Vector2(30f, 10f));
			GUIHotSpotButton searchTooltip = GUIControl.CreateControlFrameCentered<GUIHotSpotButton>(new Vector2(233f, 60f), new Vector2(15f, -25f));
			parentWindow.AnimationOpenFinished += delegate
			{
				searchTooltip.ToolTip = new NamedToolTipInfo("#friendlist_SearchFriend", new Vector2(-20f, 0f));
			};
			Add(search);
			Add(gUIImage);
			Add(AddFriend);
			Add(searchTooltip);
			AddFriend.Click += delegate
			{
				if (!string.IsNullOrEmpty(search.Text))
				{
					if (AppShell.Instance.Profile.AvailableFriends.IsFriendInviteCooldownReady())
					{
						AppShell.Instance.Profile.AvailableFriends.AddFriend(search.Text);
						search.Text = string.Empty;
					}
					else
					{
						AppShell.Instance.Profile.AvailableFriends.FireFriendCooldownIncompleteDialog();
					}
				}
			};
			_totalFriendsLabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(120f, 20f), new Vector2(35f, 14f));
			_totalFriendsLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(0, 0, 0), TextAnchor.MiddleLeft);
			_totalFriendsLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
			Add(_totalFriendsLabel);
			_totalFriendsLabel.IsVisible = true;
			OnFriendListUpdatedMessage(null);
		}

		public override void OnHide()
		{
			AppShell.Instance.EventMgr.RemoveListener<FriendListUpdatedMessage>(OnFriendListUpdatedMessage);
		}

		public override void OnShow()
		{
			OnFriendListUpdatedMessage(null);
			AppShell.Instance.EventMgr.AddListener<FriendListUpdatedMessage>(OnFriendListUpdatedMessage);
		}

		private void OnFriendListUpdatedMessage(FriendListUpdatedMessage msg)
		{
			int num = 100;
			if (Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ShieldPlayAllow))
			{
				num *= 2;
			}
			_totalFriendsLabel.Text = AppShell.Instance.Profile.AvailableFriends.Count + "/" + num + " " + AppShell.Instance.stringTable.GetString("#TT_FRIENDSLIST_9");
		}

		public override void ConfigureKeyBanks()
		{
			base.ConfigureKeyBanks();
			keyBanks[KeyInputState.Active].AddKey(new KeyCodeEntry(KeyCode.Return, false, false, false), OnEnter);
		}

		public void OnEnter(SHSKeyCode code)
		{
			AddFriend.FireMouseClick(new GUIClickEvent());
		}
	}

	public class FriendSelectedOptions : BaseSelectedOptions
	{
		public FriendSelectedOptions(FriendItem item)
			: base(item, new Vector2(261f, 195f), "friends_list_friendbuttons_expand")
		{
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_007a: Expected O, but got Unknown
			//IL_009d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Expected O, but got Unknown
			//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d4: Expected O, but got Unknown
			//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
			//IL_0101: Expected O, but got Unknown
			//IL_0124: Unknown result type (might be due to invalid IL or missing references)
			//IL_012e: Expected O, but got Unknown
			//IL_0152: Unknown result type (might be due to invalid IL or missing references)
			//IL_015c: Expected O, but got Unknown
			AddInSelectedFriend(item.playerInfo, item.OddInSequence);
			GUIButton gUIButton = AddButton(new Vector2(-46f, -7f), "brawler", "#TT_FRIENDSLIST_3", (Action)(object)(Action)delegate
			{
				InvitationDelegates.InviteToMission(item.playerInfo);
			});
			GUIButton gUIButton2 = AddButton(new Vector2(0f, -7f), "hq", "#TT_FRIENDSLIST_4", (Action)(object)(Action)delegate
			{
				InvitationDelegates.InviteToCardGame(item.playerInfo);
			});
			GUIButton gUIButton3 = AddButton(new Vector2(66f, -7f), "chat", "#TT_FRIENDSLIST_5", (Action)(object)(Action)delegate
			{
				InvitationDelegates.ChatWithPlayer(item.playerInfo);
			});
			GUIButton gUIButton4 = AddButton(new Vector2(-46f, 44f), "cardgame", "#TT_FRIENDSLIST_6", (Action)(object)(Action)delegate
			{
				InvitationDelegates.InviteToCardGame(item.playerInfo);
			});
			GUIButton gUIButton5 = AddButton(new Vector2(46f, -7f), "socialspace", "#TT_FRIENDSLIST_7", (Action)(object)(Action)delegate
			{
				InvitationDelegates.GoToPlayer(item.playerInfo);
			});
			GUIButton gUIButton6 = AddButton(new Vector2(46f, 44f), "drop", "#TT_FRIENDSLIST_8", (Action)(object)(Action)delegate
			{
				InvitationDelegates.DropFriend(item);
			});
			gUIButton.IsEnabled = item.playerInfo.AvailableForActivity;
			gUIButton.ConfigureRequiredContent(new ContentReference(ContentTypeEnum.PriorityGroup, AssetBundleLoader.BundleGroup.SpecialMission));
			if (!item.playerInfo.Online)
			{
				gUIButton.ToolTip = new NamedToolTipInfo("#TT_FRIEND_OFFLINE", new Vector2(-20f, 0f));
			}
			gUIButton2.IsEnabled = false;
			gUIButton2.ConfigureRequiredContent(new ContentReference(ContentTypeEnum.PriorityGroup, AssetBundleLoader.BundleGroup.HQ));
			gUIButton2.ToolTip = new NamedToolTipInfo("#TT_FEATURE_OFF", new Vector2(-40f, 0f));
			gUIButton2.IsVisible = false;
			gUIButton3.IsEnabled = false;
			gUIButton3.ToolTip = new NamedToolTipInfo("#TT_FEATURE_OFF", new Vector2(-40f, 0f));
			gUIButton3.IsVisible = false;
			bool flag = Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.CardGameAllow);
			gUIButton4.ToolTip.Offset = Vector2.zero;
			gUIButton4.IsEnabled = (flag && item.playerInfo.AvailableForActivity);
			if (flag)
			{
				gUIButton4.ConfigureRequiredContent(new ContentReference(ContentTypeEnum.PriorityGroup, AssetBundleLoader.BundleGroup.CardGame));
			}
			if (!flag)
			{
				gUIButton4.ToolTip = new NamedToolTipInfo("#TT_FEATURE_OFF", new Vector2(0f, 0f));
			}
			else if (!item.playerInfo.Online)
			{
				gUIButton4.ToolTip = new NamedToolTipInfo("#TT_FRIEND_OFFLINE", new Vector2(0f, 0f));
			}
			else if (!item.playerInfo.AvailableForActivity)
			{
				gUIButton4.ToolTip = new NamedToolTipInfo("#TT_FRIEND_NOTAVAILABLE", new Vector2(0f, 0f));
			}
			gUIButton5.IsEnabled = (item.playerInfo.Online && item.playerInfo.LocationType == Friend.LocationTypeEnum.GameWorld);
			if (!item.playerInfo.Online)
			{
				gUIButton5.ToolTip = new NamedToolTipInfo("#TT_FRIEND_OFFLINE", new Vector2(-20f, 0f));
			}
			else if (item.playerInfo.LocationType != Friend.LocationTypeEnum.GameWorld)
			{
				gUIButton5.ToolTip = new NamedToolTipInfo("#TT_FRIEND_NOT_IN_GAMEWORLD");
			}
			gUIButton5.ConfigureRequiredContent(new ContentReference(ContentTypeEnum.PriorityGroup, AssetBundleLoader.BundleGroup.NonBugleGameWorlds));
			gUIButton6.IsEnabled = true;
			if (item.playerInfo.LocationType == Friend.LocationTypeEnum.Tutorial)
			{
				gUIButton.IsEnabled = false;
				gUIButton2.IsEnabled = false;
				gUIButton3.IsEnabled = false;
				gUIButton4.IsEnabled = false;
				gUIButton5.IsEnabled = false;
			}
		}

		public GUIButton AddButton(Vector2 offset, string path, string tooltip, Action onClick)
		{
			return AddButton(new Vector2(66f, 66f), offset, "persistent_bundle|friendslist_" + path, tooltip, onClick);
		}
	}

	public class PendingSelectedOptions : BaseSelectedOptions
	{
		public PendingSelectedOptions(PendingInviteItem item)
			: base(item, new Vector2(260f, 140f), "friends_list_invitebuttons_expand")
		{
			//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00aa: Expected O, but got Unknown
			//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00d6: Expected O, but got Unknown
			AddInSelectedNonFriend(item.playerInfo, item.OddInSequence);
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(114f, 32f), new Vector2(-40f, 20f));
			gUIImage.TextureSource = "persistent_bundle|L_friends_list_pendingfriend_text";
			Add(gUIImage);
			GUIButton gUIButton = AddButton(new Vector2(39f, 18f), "up", "#TT_FRIENDSLIST_1", (Action)(object)(Action)delegate
			{
				InvitationDelegates.AcceptFriendRequest(item);
			});
			GUIButton gUIButton2 = AddButton(new Vector2(86f, 30f), "down", "#TT_FRIENDSLIST_2", (Action)(object)(Action)delegate
			{
				InvitationDelegates.RejectFriendRequest(item);
			});
			gUIButton.ToolTip.Offset = new Vector2(-30f, -10f);
			gUIButton2.ToolTip.Offset = new Vector2(-30f, -10f);
		}

		public GUIButton AddButton(Vector2 offset, string path, string tooltip, Action onClick)
		{
			GUIButton gUIButton = AddButton(new Vector2(64f, 64f), offset, "common_bundle|thumbs_" + path, tooltip, onClick);
			gUIButton.HitTestSize = new Vector2(0.7f, 0.7f);
			return gUIButton;
		}
	}

	public class BlockedSelectedOptions : BaseSelectedOptions
	{
		public BlockedSelectedOptions(BlockedItem item)
			: base(item, new Vector2(260f, 140f), "friends_list_invitebuttons_expand")
		{
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Expected O, but got Unknown
			//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c5: Expected O, but got Unknown
			AddInSelectedNonFriend(item.playerInfo, item.OddInSequence);
			GUIButton gUIButton = AddButton(new Vector2(64f, 64f), new Vector2(-40f, 22f), "persistent_bundle|mshs_friendActionIcon_report", "#TT_FRIENDSLIST_15", (Action)(object)(Action)delegate
			{
				InvitationDelegates.ReportPlayer(item.playerInfo);
			});
			gUIButton.IsEnabled = false;
			GUIButton gUIButton2 = AddButton(new Vector2(128f, 128f), new Vector2(50f, 20f), "persistent_bundle|friends_list_unblock", "#TT_FRIENDSLIST_16", (Action)(object)(Action)delegate
			{
				InvitationDelegates.UnBlock(item);
			});
			gUIButton2.ToolTip.Offset = new Vector2(-30f, 20f);
		}
	}

	public class BaseSelectedOptions : GUIDynamicWindow
	{
		public BaseSelectedOptions(FriendsListItem item, Vector2 size, string textureSource)
		{
			Rect screenRect = item.item.ScreenRect;
			SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(screenRect.x - 28f, screenRect.y - 24f));
			SetSize(size);
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(size, new Vector2(0f, 0f));
			gUIImage.TextureSource = "persistent_bundle|" + textureSource;
			Add(gUIImage);
		}

		public void AddInSelectedFriend(Friend playerInfo, bool OddInSequence)
		{
			AddInSelectedNonFriend(playerInfo, OddInSequence);
			GUILabel gUILabel = GUIControl.CreateControlTopFrameCentered<GUILabel>(new Vector2(160f, 46f), new Vector2(0f, 66f));
			gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, GUILabel.GenColor(117, 128, 79), TextAnchor.UpperLeft);
			gUILabel.WordWrap = false;
			gUILabel.Text = AppShell.Instance.stringTable["#friend_location"] + playerInfo.Location;
			Add(gUILabel);
		}

		public void AddInSelectedNonFriend(Friend playerInfo, bool OddInSequence)
		{
			GUIImage gUIImage = GUIControl.CreateControlTopFrameCentered<GUIImage>(new Vector2(217f, 57f), new Vector2(0f, 42f));
			gUIImage.TextureSource = "persistent_bundle|friends_list_module_selected" + (OddInSequence ? 1 : 2);
			GUILabel gUILabel = GUIControl.CreateControlTopFrameCentered<GUILabel>(new Vector2(160f, 46f), new Vector2(0f, 51f));
			gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(81, 83, 78), TextAnchor.UpperLeft);
			gUILabel.WordWrap = false;
			gUILabel.Text = playerInfo.Name;
			Add(gUIImage);
			Add(gUILabel);
			GUITBCloseButton gUITBCloseButton = GUIControl.CreateControlFrameCentered<GUITBCloseButton>(new Vector2(44f, 44f), new Vector2(size.x * 0.5f - 35f, size.y * -0.5f + 22f));
			gUITBCloseButton.Click += delegate
			{
				IsVisible = false;
			};
			Add(gUITBCloseButton);
		}

		public GUIButton AddButton(Vector2 size, Vector2 offset, string path, string tooltip, Action onClick)
		{
			GUIButton gUIButton = GUIControl.CreateControlFrameCentered<GUIButton>(size, offset);
			gUIButton.HitTestType = HitTestTypeEnum.Circular;
			gUIButton.StyleInfo = new SHSButtonStyleInfo(path);
			gUIButton.ToolTip = new NamedToolTipInfo(tooltip, new Vector2(-20f, 0f));
			gUIButton.Click += delegate
			{
				onClick.Invoke();
				IsVisible = false;
			};
			Add(gUIButton);
			return gUIButton;
		}
	}

	public class InvitationDelegates
	{
		public static void DropFriend(FriendItem playerItem)
		{
			SHSErrorNotificationWindow.ErrorIconInfo errorIconInfo = SHSErrorNotificationWindow.GetErrorIconInfo(SHSErrorNotificationWindow.ErrorIcons.TooManyFriends);
			GUIDialogNotificationSink notificationSink = new GUIDialogNotificationSink(delegate(string Id, GUIDialogWindow.DialogState state)
			{
				if (state == GUIDialogWindow.DialogState.Ok)
				{
					AppShell.Instance.Profile.AvailableFriends.RemoveFriend(playerItem.playerInfo.Id);
					playerItem.ParentWindow.FriendsWindow.RemoveItem(playerItem);
				}
			});
			SHSCommonDialogWindow sHSCommonDialogWindow = new SHSCommonDialogWindow(errorIconInfo.IconPath, errorIconInfo.IconSize, "common_bundle|button_close", "common_bundle|L_mshs_button_yes", "common_bundle|L_mshs_button_no", typeof(SHSDialogYesButton), typeof(SHSDialogNoButton), true);
			sHSCommonDialogWindow.TitleText = "#removefriend_title";
			sHSCommonDialogWindow.Text = "#removefriend_text";
			sHSCommonDialogWindow.NotificationSink = notificationSink;
			GUIManager.Instance.ShowDynamicWindow(sHSCommonDialogWindow, ModalLevelEnum.Full);
		}

		public static void InviteToCardGame(Friend playerInfo)
		{
			if (!Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.CardGameAllow))
			{
				GUIManager.Instance.ShowDialog(typeof(SHSCardGameUnavailableDialogWindow), "#cardgame_not_available_text", new GUIDialogNotificationSink(null), ModalLevelEnum.Default);
			}
			else if (!Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ParentalCardGameDeny))
			{
				GUIManager.Instance.ShowErrorDialog(SHSErrorCodes.Code.ParentalControlLimit, string.Empty);
			}
			else if (playerInfo.Id != -1)
			{
				if (AppShell.Instance.Profile.AvailableFriends.IsPlayerInBlockedList(playerInfo.Id))
				{
					UnblockPlayerQuery();
					return;
				}
				SHSCardGameGadgetWindow sHSCardGameGadgetWindow = new SHSCardGameGadgetWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.Main);
				sHSCardGameGadgetWindow.SetupForCardGameInviter(playerInfo.Id, playerInfo.Name);
				GUIManager.Instance.ShowDynamicWindow(sHSCardGameGadgetWindow, ModalLevelEnum.Default);
			}
		}

		public static void InviteToMission(Friend playerInfo)
		{
			if (!Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ParentalMissionsDeny))
			{
				GUIManager.Instance.ShowErrorDialog(SHSErrorCodes.Code.ParentalControlLimit, string.Empty);
				return;
			}
			SHSBrawlerGadget sHSBrawlerGadget = new SHSBrawlerGadget();
			sHSBrawlerGadget.ConfigureOnFriendsInvite(playerInfo);
			GUIManager.Instance.ShowDynamicWindow(sHSBrawlerGadget, ModalLevelEnum.Default);
		}

		public static void GoToPlayer(Friend playerInfo)
		{
			GUIDialogNotificationSink notificationSink = new GUIDialogNotificationSink(delegate(string Id, GUIDialogWindow.DialogState state)
			{
				if (state == GUIDialogWindow.DialogState.Ok)
				{
					AppShell.Instance.Profile.AvailableFriends.GoToFriend(playerInfo.Id);
				}
			});
			SHSCommonDialogWindow sHSCommonDialogWindow = new SHSCommonDialogWindow("common_bundle|notification_icon_friendnotavailable", new Vector2(215f, 210f), new Vector2(0f, 0f), string.Empty, "common_bundle|L_mshs_button_yes", "common_bundle|L_mshs_button_no", typeof(SHSDialogYesButton), typeof(SHSDialogNoButton), false);
			sHSCommonDialogWindow.TitleText = "#friendlist_GoToPlayer";
			sHSCommonDialogWindow.Text = "#friendlist_GoToPlayer";
			sHSCommonDialogWindow.NotificationSink = notificationSink;
			GUIManager.Instance.ShowDynamicWindow(sHSCommonDialogWindow, ModalLevelEnum.Full);
		}

		public static void ChatWithPlayer(Friend playerInfo)
		{
		}

		public static void VisitFriendHQ(Friend playerInfo)
		{
			AppShell.Instance.Matchmaker2.VisitHQ(playerInfo.Id, delegate(Matchmaker2.Ticket ticket)
			{
				if (ticket.status == Matchmaker2.Ticket.Status.SUCCESS)
				{
					AppShell.Instance.SharedHashTable[HqController2.HQ_USER_ID_KEY] = playerInfo.Id;
					AppShell.Instance.Transition(GameController.ControllerType.HeadQuarters);
				}
				else
				{
					CspUtils.DebugLog("Visit friends HQ failed with status: " + ticket.status.ToString() + " This could be caused by serveral things, but most likely a 1 way friendship will cause this consistantly (which is impossible to achieve except by the switchboard)");
				}
			});
		}

		public static void UnBlock(BlockedItem playerItem)
		{
			SHSErrorNotificationWindow.ErrorIconInfo errorIconInfo = SHSErrorNotificationWindow.GetErrorIconInfo(SHSErrorNotificationWindow.ErrorIcons.FriendBlocking);
			GUIDialogNotificationSink notificationSink = new GUIDialogNotificationSink(delegate(string windowId, GUIDialogWindow.DialogState windowState)
			{
				if (windowState == GUIDialogWindow.DialogState.Ok)
				{
					AppShell.Instance.Profile.AvailableFriends.RemoveBlocked(playerItem.playerInfo.Id);
					playerItem.ParentWindow.BlockedWindow.RemoveItem(playerItem);
				}
			});
			SHSCommonDialogWindow sHSCommonDialogWindow = new SHSCommonDialogWindow(errorIconInfo.IconPath, errorIconInfo.IconSize, "common_bundle|button_close", "common_bundle|L_mshs_button_yes", "common_bundle|L_mshs_button_no", typeof(SHSDialogYesButton), typeof(SHSDialogNoButton), true);
			sHSCommonDialogWindow.TitleText = "#unblockconfirm_title";
			sHSCommonDialogWindow.Text = "#unblockconfirm_text";
			sHSCommonDialogWindow.NotificationSink = notificationSink;
			GUIManager.Instance.ShowDynamicWindow(sHSCommonDialogWindow, ModalLevelEnum.Full);
		}

		public static void ReportPlayer(Friend playerInfo)
		{
		}

		public static void AcceptFriendRequest(PendingInviteItem playerItem)
		{
			AppShell.Instance.Profile.AvailableFriends.AddFriend(playerItem.playerInfo.Id);
			playerItem.ParentWindow.PendingInviteWindow.RemoveItem(playerItem);
		}

		public static void RejectFriendRequest(PendingInviteItem playerItem)
		{
			AppShell.Instance.Profile.AvailableFriends.DeclineFriend(playerItem.playerInfo.Id);
			AppShell.Instance.EventMgr.Fire(null, new FriendDeclinedMessage(playerItem.playerInfo.Id, playerItem.playerInfo.Name));
			playerItem.ParentWindow.PendingInviteWindow.RemoveItem(playerItem);
		}
	}

	public class FriendsList : SHSSelectionWindow<FriendsListItem, GUISimpleControlWindow>
	{
		public FriendsList(GUISlider slider)
			: base(slider, SelectionWindowType.OneAcross)
		{
			SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
			TopOffsetAdjustHeight = 5f;
			BottomOffsetAdjustHeight = 3f;
			slider.FireChanged();
			SetSize(new Vector2(217f, 290f));
			SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
			Offset = new Vector2(9f, -19f);
		}
	}

	public class FriendItem : FriendsListItem
	{
		public FriendItem(Friend playerInfo, SHSFriendsListWindow ParentWindow)
			: base(playerInfo, ParentWindow)
		{
		}

		public override void UpdateInfo()
		{
			nameLabel.Text = playerInfo.Name;
			locationLabel.Text = AppShell.Instance.stringTable["#friend_location"] + playerInfo.Location;
			if (playerInfo.Online && playerInfo.AvailableForActivity)
			{
				currentState = SelectionState.Active;
			}
			else
			{
				currentState = SelectionState.Passive;
			}
		}

		public override BaseSelectedOptions GetSelectionOption()
		{
			return new FriendSelectedOptions(this);
		}
	}

	public class PendingInviteItem : FriendsListItem
	{
		public PendingInviteItem(Friend playerInfo, SHSFriendsListWindow ParentWindow)
			: base(playerInfo, ParentWindow)
		{
		}

		public override void UpdateInfo()
		{
			nameLabel.Text = playerInfo.Name;
			nameLabel.Text = playerInfo.Name;
			currentState = SelectionState.Active;
			locationLabel.Text = string.Empty;
		}

		public override BaseSelectedOptions GetSelectionOption()
		{
			return new PendingSelectedOptions(this);
		}
	}

	public class BlockedItem : FriendsListItem
	{
		public BlockedItem(Friend playerInfo, SHSFriendsListWindow ParentWindow)
			: base(playerInfo, ParentWindow)
		{
		}

		public override void UpdateInfo()
		{
			nameLabel.Text = playerInfo.Name;
			nameLabel.Text = playerInfo.Name;
			currentState = SelectionState.Active;
			locationLabel.Text = string.Empty;
		}

		public override BaseSelectedOptions GetSelectionOption()
		{
			return new BlockedSelectedOptions(this);
		}
	}

	public abstract class FriendsListItem : SHSSelectionItem<GUISimpleControlWindow>, IComparable<FriendsListItem>
	{
		public GUILabel nameLabel;

		public GUILabel locationLabel;

		private GUIHotSpotButton hotSpot;

		public Friend playerInfo;

		public SHSFriendsListWindow ParentWindow;

		public FriendsListItem(Friend playerInfo, SHSFriendsListWindow ParentWindow)
		{
			this.ParentWindow = ParentWindow;
			this.playerInfo = playerInfo;
			item = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(new Vector2(203f, 46f), Vector2.zero);
			itemSize = new Vector2(203f, 46f);
			currentState = SelectionState.Active;
			nameLabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(160f, 46f), new Vector2(0f, 7f));
			nameLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(81, 83, 78), TextAnchor.UpperLeft);
			nameLabel.WordWrap = false;
			locationLabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(160f, 46f), new Vector2(0f, 22f));
			locationLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, GUILabel.GenColor(117, 128, 79), TextAnchor.UpperLeft);
			locationLabel.WordWrap = false;
			hotSpot = GUIControl.CreateControlFrameCentered<GUIHotSpotButton>(new Vector2(203f, 46f), new Vector2(0f, 0f));
			hotSpot.ToolTip = new NamedToolTipInfo("#TT_FRIEND_LIST_ITEM");
			item.Add(nameLabel);
			item.Add(locationLabel);
			item.Add(hotSpot);
			hotSpot.Click += delegate
			{
				HotSpotClick();
			};
			UpdateInfo();
		}

		public abstract void UpdateInfo();

		public abstract BaseSelectedOptions GetSelectionOption();

		public void HotSpotClick()
		{
			if (ParentWindow.SelectedOptionOpen && ParentWindow.OpenedOptions != null)
			{
				ParentWindow.OpenedOptions.IsVisible = false;
			}
			ParentWindow.SelectedOptionOpen = true;
			BaseSelectedOptions selectionOption = GetSelectionOption();
			ParentWindow.OpenedOptions = selectionOption;
			selectionOption.OnHidden += delegate
			{
				ParentWindow.SelectedOptionOpen = false;
				ParentWindow.OpenedOptions = null;
			};
			GUIManager.Instance.ShowDynamicWindow(selectionOption, "SHSMainWindow", DrawOrder.DrawFirst, DrawPhaseHintEnum.PostDraw, ModalLevelEnum.None);
		}

		public int CompareTo(FriendsListItem other)
		{
			if (playerInfo.Online && playerInfo.AvailableForActivity && (!other.playerInfo.Online || !other.playerInfo.AvailableForActivity))
			{
				return -1;
			}
			if (other.playerInfo.Online && other.playerInfo.AvailableForActivity && (!playerInfo.Online || !playerInfo.AvailableForActivity))
			{
				return 1;
			}
			return playerInfo.Name.CompareTo(other.playerInfo.Name);
		}
	}

	public class OpenAndCloseAnimations : SHSAnimations
	{
		public static AnimClip Open(GUIControl win, GUIControl bkg, List<GUIControl> toFade)
		{
			return Absolute.SizeX(GenericPaths.BounceTransitionInX(400f, 0f), win) ^ Absolute.SizeY(GenericPaths.BounceTransitionInY(496f, 0f), win) ^ Generic.AnimationBounceTransitionIn(new Vector2(273f, 496f), 0f, bkg) ^ Generic.AnimationFadeTransitionIn(toFade.ToArray());
		}

		public static AnimClip Close(SHSFriendsListWindow win, GUIControl bkg, List<GUIControl> toFade)
		{
			if (win.OpenedOptions != null)
			{
				win.OpenedOptions.IsVisible = false;
			}
			return Absolute.SizeX(GenericPaths.BounceTransitionOutX(400f, 0f), win) ^ Absolute.SizeY(GenericPaths.BounceTransitionOutY(496f, 0f), win) ^ Generic.AnimationBounceTransitionOut(new Vector2(273f, 496f), 0f, bkg) ^ Generic.AnimationFadeTransitionOut(toFade.ToArray());
		}
	}

	public enum TabMode
	{
		Friends,
		PendingInvite,
		Blocked
	}

	public class FadeInOut
	{
		private GUIWindow window;

		private List<GUIControl> toFade = new List<GUIControl>();

		private List<GUIControl> toFadeVis = new List<GUIControl>();

		private List<GUIControl> toAntiFadeVis = new List<GUIControl>();

		private AnimClip fadeAnim;

		private bool fadeState;

		public FadeInOut(GUIWindow window)
		{
			this.window = window;
		}

		public void RegisterFade(GUIControl ctrl)
		{
			toFade.Add(ctrl);
		}

		public void RegisterFadeVis(GUIControl ctrl)
		{
			toFadeVis.Add(ctrl);
		}

		public void RegisterAntiFadeVis(GUIControl ctrl)
		{
			toAntiFadeVis.Add(ctrl);
		}

		public void SetState(bool on)
		{
			fadeState = on;
			toFadeVis.ForEach(delegate(GUIControl ctrl)
			{
				ctrl.IsVisible = on;
				ctrl.Alpha = (on ? 1 : 0);
			});
			toAntiFadeVis.ForEach(delegate(GUIControl ctrl)
			{
				ctrl.IsVisible = !on;
				ctrl.Alpha = ((!on) ? 1 : 0);
			});
		}

		public void FadeIn()
		{
			if (!fadeState)
			{
				fadeState = true;
				window.AnimationPieceManager.SwapOut(ref fadeAnim, SHSAnimations.Generic.FadeIn(toFade, 0.2f) ^ SHSAnimations.Generic.FadeInVis(toFadeVis, 0.2f) ^ SHSAnimations.Generic.FadeOutVis(toAntiFadeVis, 0.2f));
			}
		}

		public void FadeOut()
		{
			if (fadeState)
			{
				fadeState = false;
				window.AnimationPieceManager.SwapOut(ref fadeAnim, SHSAnimations.Generic.FadeOut(toFade, 0.2f) ^ SHSAnimations.Generic.FadeOutVis(toFadeVis, 0.2f) ^ SHSAnimations.Generic.FadeInVis(toAntiFadeVis, 0.2f));
			}
		}
	}

	private const string friendListOpenModeKey = "FriendListOpenMode";

	private GUIImage bkg;

	private FadeInOut FadeFriendsWindow;

	private FadeInOut FadePendingInviteWindow;

	private FadeInOut FadeBlockedWindow;

	private FriendsList FriendsWindow;

	private FriendsList PendingInviteWindow;

	private FriendsList BlockedWindow;

	public bool SelectedOptionOpen;

	public BaseSelectedOptions OpenedOptions;

	private TabMode tabMode;

	public static string FriendListOpenModeKey
	{
		get
		{
			return "FriendListOpenMode";
		}
	}

	public SHSFriendsListWindow()
	{
		CspUtils.DebugLog("SHSFriendsListWindow called.");
		AppShell.Instance.Profile.AvailableFriends.ReloadFriendList();  // added by CSP

		SetSize(new Vector2(400f, 496f));
		SetPosition(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(135f, -140f));
		FadeFriendsWindow = new FadeInOut(this);
		FadePendingInviteWindow = new FadeInOut(this);
		FadeBlockedWindow = new FadeInOut(this);
		bkg = GUIControl.CreateControlBottomFrame<GUIImage>(new Vector2(273f, 496f), new Vector2(0f, 0f));
		bkg.TextureSource = "persistent_bundle|friends_list_container_back";
		Add(bkg);
		CreateAndAddButtonTab(FadeFriendsWindow, 1, -74f, new Vector2(-60f, 20f), "#friendsList_MyFriends", false);
		CreateAndAddButtonTab(FadePendingInviteWindow, 2, -3f, new Vector2(-120f, 20f), "#friendsList_InvitationsFromOthers", false);
		CreateAndAddButtonTab(FadeBlockedWindow, 3, 70f, new Vector2(60f, 20f), "#friendsList_BlockedSquads", true);
		GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(252f, 300f), new Vector2(7f, -16f));
		gUIImage.TextureSource = "persistent_bundle|friends_list_container_panel_back";
		Add(gUIImage);
		GUISlider gUISlider = CreateAndAddSlider(FadeFriendsWindow, "#friendsList_MoreFriends");
		GUISlider gUISlider2 = CreateAndAddSlider(FadePendingInviteWindow, "#friendsList_MoreInvitations");
		GUISlider gUISlider3 = CreateAndAddSlider(FadeBlockedWindow, "#friendsList_MoreBlockedSquads");
		FriendsWindow = CreateAndAddFriendsList(FadeFriendsWindow, gUISlider);
		PendingInviteWindow = CreateAndAddFriendsList(FadePendingInviteWindow, gUISlider2);
		BlockedWindow = CreateAndAddFriendsList(FadeBlockedWindow, gUISlider3);
		GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(252f, 300f), new Vector2(7f, -12f));
		gUIImage2.TextureSource = "persistent_bundle|friends_list_container_panel_frame";
		Add(gUIImage2);
		CreateAndAddImageTab(FadeFriendsWindow, 1, "#friendsList_MyFriends");
		CreateAndAddImageTab(FadePendingInviteWindow, 2, "#friendsList_InvitationsFromOthers");
		CreateAndAddImageTab(FadeBlockedWindow, 3, "#friendsList_BlockedSquads");
		ControlToFront(gUISlider);
		ControlToFront(gUISlider2);
		ControlToFront(gUISlider3);
		FullReload();
		GUITBCloseButton gUITBCloseButton = GUIControl.CreateControlFrameCentered<GUITBCloseButton>(new Vector2(44f, 44f), new Vector2(117f, -228f));
		gUITBCloseButton.Click += delegate
		{
			ToggleClosed();
		};
		Add(gUITBCloseButton);
		Add(new FriendSearch(this));
		SetupOpeningAndClosingAnimations();
	}

	public static void ShowYesNoDialog(string text, Action del)
	{
		GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.YesNoDialog, text, delegate(string Id, GUIDialogWindow.DialogState state)
		{
			if (state == GUIDialogWindow.DialogState.Ok && del != null)
			{
				del.Invoke();
			}
		}, ModalLevelEnum.Default);
	}

	public static void ShowOkDialog(string text)
	{
		GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.OkDialog, text, delegate
		{
		}, ModalLevelEnum.Default);
	}

	public void SetupOpeningAndClosingAnimations()
	{
		List<GUIControl> toFade = ControlList.ConvertAll(delegate(IGUIControl ictrl)
		{
			return ictrl as GUIControl;
		});
		toFade.Remove(bkg);
		base.AnimationOnOpen = delegate
		{
			return OpenAndCloseAnimations.Open(this, bkg, toFade);
		};
		base.AnimationOnClose = delegate
		{
			return OpenAndCloseAnimations.Close(this, bkg, toFade);
		};
	}

	public GUISlider CreateAndAddSlider(FadeInOut toAdd, string tooltip)
	{
		GUISlider gUISlider = GUIControl.CreateControlFrameCentered<GUISlider>(new Vector2(50f, 320f), new Vector2(124f, -27f));
		gUISlider.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		gUISlider.IsVisible = false;
		gUISlider.ArrowsEnabled = true;
		gUISlider.StartArrow.ToolTip = new NamedToolTipInfo(tooltip);
		gUISlider.EndArrow.ToolTip = new NamedToolTipInfo(tooltip);
		toAdd.RegisterFade(gUISlider);
		Add(gUISlider);
		return gUISlider;
	}

	public FriendsList CreateAndAddFriendsList(FadeInOut toAdd, GUISlider slider)
	{
		FriendsList friendsList = new FriendsList(slider);
		friendsList.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		friendsList.IsVisible = false;
		toAdd.RegisterFadeVis(friendsList);
		Add(friendsList);
		return friendsList;
	}

	public GUIButton CreateAndAddButtonTab(FadeInOut toAdd, int path, float offsetX, Vector2 tooltipOffset, string tooltip, bool FlipTooltip)
	{
		GUIHotSpotButton gUIHotSpotButton = GUIControl.CreateControlFrameCentered<GUIHotSpotButton>(new Vector2(128f, 128f), new Vector2(offsetX, -184f));
		gUIHotSpotButton.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		gUIHotSpotButton.ToolTip = new NamedToolTipInfo(tooltip, tooltipOffset);
		gUIHotSpotButton.IsVisible = false;
		gUIHotSpotButton.HitTestSize = new Vector2(0.547f, 0.5f);
		toAdd.RegisterFadeVis(gUIHotSpotButton);
		Add(gUIHotSpotButton);
		GUIButton gUIButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(128f, 128f), new Vector2(offsetX, -184f));
		gUIButton.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		gUIButton.ToolTip = new NamedToolTipInfo(tooltip, tooltipOffset);
		gUIButton.IsVisible = false;
		gUIButton.HitTestSize = new Vector2(0.547f, 0.5f);
		gUIButton.StyleInfo = new SHSButtonStyleInfo("persistent_bundle|friends_list_container_tab" + path + "_inactive");
		gUIButton.Click += delegate
		{
			if (SelectedOptionOpen && OpenedOptions != null)
			{
				OpenedOptions.IsVisible = false;
				SelectedOptionOpen = false;
				OpenedOptions = null;
			}
			toAdd.FadeIn();
			if (toAdd != FadeFriendsWindow)
			{
				FadeFriendsWindow.FadeOut();
			}
			if (toAdd != FadePendingInviteWindow)
			{
				FadePendingInviteWindow.FadeOut();
			}
			if (toAdd != FadeBlockedWindow)
			{
				FadeBlockedWindow.FadeOut();
			}
		};
		toAdd.RegisterAntiFadeVis(gUIButton);
		Add(gUIButton);
		if (FlipTooltip)
		{
			gUIHotSpotButton.ToolTip.VerticalAlignmentOverride = SHSTooltip.ToolTipVerticalAlignment.Top;
			gUIHotSpotButton.ToolTip.HorizontalAlignmentOverride = SHSTooltip.ToolTipHorizontalAlignment.Right;
			gUIButton.ToolTip.VerticalAlignmentOverride = SHSTooltip.ToolTipVerticalAlignment.Top;
			gUIButton.ToolTip.HorizontalAlignmentOverride = SHSTooltip.ToolTipHorizontalAlignment.Right;
		}
		return gUIButton;
	}

	public GUIImage CreateAndAddImageTab(FadeInOut toAdd, int path, string tooltip)
	{
		GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(253f, 68f), new Vector2(6f, -196f));
		gUIImage.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		gUIImage.IsVisible = false;
		gUIImage.TextureSource = "persistent_bundle|friends_list_container_tab" + path + "_active";
		toAdd.RegisterFadeVis(gUIImage);
		Add(gUIImage);
		return gUIImage;
	}

	public void FullReload()
	{
		FriendsWindow.ClearItems();
		PendingInviteWindow.ClearItems();
		BlockedWindow.ClearItems();
		FriendsWindow.AddList(getFriendsListItems());
		PendingInviteWindow.AddList(getPendingListItems());
		BlockedWindow.AddList(getBlockedListItems());
		FriendsWindow.SortItemList();
		PendingInviteWindow.SortItemList();
		BlockedWindow.SortItemList();
	}

	public List<FriendsListItem> getFriendsListItems()
	{
		List<FriendsListItem> list = new List<FriendsListItem>();
		UserProfile profile = AppShell.Instance.Profile;
		if (profile == null)
		{
			return list;
		}
		foreach (KeyValuePair<string, Friend> availableFriend in profile.AvailableFriends)
		{
			list.Add(new FriendItem(availableFriend.Value, this));
		}
		return list;
	}

	public List<FriendsListItem> getPendingListItems()
	{
		List<FriendsListItem> list = new List<FriendsListItem>();
		UserProfile profile = AppShell.Instance.Profile;
		if (profile == null)
		{
			return list;
		}
		foreach (Friend item in profile.AvailableFriends.AvailablePending)
		{
			list.Add(new PendingInviteItem(item, this));
		}
		return list;
	}

	public List<FriendsListItem> getBlockedListItems()
	{
		List<FriendsListItem> list = new List<FriendsListItem>();
		UserProfile profile = AppShell.Instance.Profile;
		if (profile == null)
		{
			return list;
		}
		foreach (KeyValuePair<string, Friend> item in profile.AvailableFriends.AvailableBlocked)
		{
			list.Add(new BlockedItem(item.Value, this));
		}
		return list;
	}

	public override void OnShow()
	{
		base.OnShow();
		AppShell.Instance.EventMgr.AddListener<FriendUpdateMessage>(OnFriendUpdate);
		AppShell.Instance.EventMgr.AddListener<FriendRequestMessage>(OnFriendRequest);
		AppShell.Instance.EventMgr.AddListener<FriendAcceptedMessage>(OnFriendAccepted);
		AppShell.Instance.EventMgr.AddListener<FriendDeclinedMessage>(OnFriendDeclined);
		AppShell.Instance.EventMgr.AddListener<FriendListUpdatedMessage>(OnFriendListUpdate);
		Hashtable sharedHashTable = AppShell.Instance.SharedHashTable;
		if (sharedHashTable.ContainsKey(FriendListOpenModeKey))
		{
			tabMode = (TabMode)(int)sharedHashTable[FriendListOpenModeKey];
			sharedHashTable.Remove(FriendListOpenModeKey);
		}
		else
		{
			tabMode = TabMode.Friends;
		}
		FadeFriendsWindow.SetState(tabMode == TabMode.Friends);
		FadePendingInviteWindow.SetState(tabMode == TabMode.PendingInvite);
		FadeBlockedWindow.SetState(tabMode == TabMode.Blocked);
	}

	public override void OnHide()
	{
		base.OnHide();
		AppShell.Instance.EventMgr.RemoveListener<FriendUpdateMessage>(OnFriendUpdate);
		AppShell.Instance.EventMgr.RemoveListener<FriendRequestMessage>(OnFriendRequest);
		AppShell.Instance.EventMgr.RemoveListener<FriendAcceptedMessage>(OnFriendAccepted);
		AppShell.Instance.EventMgr.RemoveListener<FriendDeclinedMessage>(OnFriendDeclined);
		AppShell.Instance.EventMgr.RemoveListener<FriendListUpdatedMessage>(OnFriendListUpdate);
		if (OpenedOptions != null)
		{
			OpenedOptions.IsVisible = false;
		}
	}

	public void OnFriendListUpdate(FriendListUpdatedMessage msg)
	{
		FullReload();
	}

	public void OnFriendRequest(FriendRequestMessage msg)
	{
		PendingInviteWindow.AddItem(new PendingInviteItem(new Friend(msg.FriendName, msg.FriendID, string.Empty, true, true), this));
		PendingInviteWindow.SortItemList();
	}

	public void OnFriendAccepted(FriendAcceptedMessage msg)
	{
		FriendsListItem friendsListItem = PendingInviteWindow.items.Find(delegate(FriendsListItem item)
		{
			return item.playerInfo.Id == msg.FriendID;
		});
		if (friendsListItem != null)
		{
			PendingInviteWindow.RemoveItem(friendsListItem);
		}
	}

	public void OnFriendDeclined(FriendDeclinedMessage msg)
	{
		FriendsListItem friendsListItem = PendingInviteWindow.items.Find(delegate(FriendsListItem item)
		{
			return item.playerInfo.Id == msg.FriendID;
		});
		if (friendsListItem != null)
		{
			PendingInviteWindow.RemoveItem(friendsListItem);
		}
	}

	private void OnFriendUpdate(FriendUpdateMessage msg)
	{
		FriendsListItem friendsListItem = FriendsWindow.items.Find(delegate(FriendsListItem item)
		{
			return item.playerInfo.Id == msg.FriendID;
		});
		if (friendsListItem == null)
		{
			CspUtils.DebugLog("FriendInfo Update for friend: " + msg.FriendID + ", but it's not in the friend info list.");
			return;
		}
		friendsListItem.UpdateInfo();
		FriendsWindow.SortItemList();
	}

	public static void UnblockPlayerQuery()
	{
		GUIDialogNotificationSink notificationSink = new GUIDialogNotificationSink(delegate(string Id, GUIDialogWindow.DialogState state)
		{
			if (state == GUIDialogWindow.DialogState.Ok)
			{
				AppShell.Instance.SharedHashTable[FriendListOpenModeKey] = TabMode.Blocked;
				AppShell.Instance.EventMgr.Fire(null, new EnsureButtonVisibleMessage(SHSHudWheels.ButtonType.Friends));
			}
		});
		SHSErrorNotificationWindow.ErrorIconInfo errorIconInfo = SHSErrorNotificationWindow.GetErrorIconInfo(SHSErrorNotificationWindow.ErrorIcons.FriendBlocking);
		SHSCommonDialogWindow sHSCommonDialogWindow = new SHSCommonDialogWindow(errorIconInfo.IconPath, errorIconInfo.IconSize, "common_bundle|button_close", "common_bundle|L_mshs_button_yes", "common_bundle|L_mshs_button_no", typeof(SHSDialogYesButton), typeof(SHSDialogNoButton), true);
		sHSCommonDialogWindow.TitleText = "#error_oops";
		sHSCommonDialogWindow.Text = "#invite_block_msg";
		sHSCommonDialogWindow.NotificationSink = notificationSink;
		GUIManager.Instance.ShowDynamicWindow(sHSCommonDialogWindow, ModalLevelEnum.Full);
	}
}
