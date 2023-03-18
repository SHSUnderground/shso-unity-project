using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSFriendSelect : SHSGadget.GadgetLeftWindow
{
	public class FriendSelection : SHSSelectionWindow<FriendItem, GUIControlWindow>
	{
		public FriendSelection(GUISlider slider)
			: base(slider, SelectionWindowType.OneAcross)
		{
		}
	}

	public class FriendItem : SHSSelectionItem<GUIControlWindow>, IComparable<FriendItem>
	{
		public string name;

		public bool online;

		public bool available;

		public string location;

		public int playerId;

		private GUILabel nameLabel;

		private GUILabel locationLabel;

		private SHSFriendSelect headWindow;

		public FriendItem(string name, bool online, bool available, string location, int playerId, SHSFriendSelect headWindow)
		{
			this.headWindow = headWindow;
			this.name = name;
			this.online = online;
			this.location = location;
			this.playerId = playerId;
			item = new GUIControlWindow();
			item.SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
			itemSize = new Vector2(203f, 46f);
			nameLabel = new GUILabel();
			nameLabel.SetSize(203f, 46f);
			nameLabel.SetPosition(30f, 5f);
			nameLabel.WordWrap = false;
			item.Add(nameLabel);
			locationLabel = new GUILabel();
			locationLabel.SetSize(203f, 46f);
			locationLabel.SetPosition(30f, 20f);
			locationLabel.WordWrap = false;
			item.Add(locationLabel);
			GUIHotSpotButton gUIHotSpotButton = new GUIHotSpotButton();
			gUIHotSpotButton.SetSize(itemSize);
			gUIHotSpotButton.SetPosition(0f, 0f);
			gUIHotSpotButton.Click += delegate
			{
				if (headWindow.selectedFriend != null)
				{
					FriendItem selectedFriend = headWindow.selectedFriend;
					headWindow.selectedFriend = null;
					selectedFriend.RefreshStatus();
				}
				headWindow.selectedFriend = this;
				RefreshStatus();
				if (headWindow.onFriendClicked != null)
				{
					headWindow.onFriendClicked(name, playerId);
				}
			};
			item.Add(gUIHotSpotButton);
			RefreshStatus();
		}

		public void RefreshStatus()
		{
			nameLabel.Text = name;
			locationLabel.Text = string.Format(AppShell.Instance.stringTable["#friend_location"], location);
			if (headWindow.selectedFriend == this)
			{
				currentState = SelectionState.Selected;
				nameLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(81, 82, 81), TextAnchor.UpperLeft);
				locationLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, GUILabel.GenColor(81, 82, 81), TextAnchor.UpperLeft);
			}
			else if (online)
			{
				currentState = SelectionState.Active;
				nameLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(81, 82, 81), TextAnchor.UpperLeft);
				locationLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, GUILabel.GenColor(81, 82, 81), TextAnchor.UpperLeft);
			}
			else
			{
				currentState = SelectionState.Passive;
				nameLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(164, 156, 83), TextAnchor.UpperLeft);
				locationLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, GUILabel.GenColor(164, 156, 83), TextAnchor.UpperLeft);
			}
		}

		public int CompareTo(FriendItem other)
		{
			if (online == other.online && available == other.available)
			{
				return name.CompareTo(other.name);
			}
			if (online)
			{
				return -1;
			}
			if (available)
			{
				return -1;
			}
			return 1;
		}
	}

	public delegate void FriendClickedDelegate(string friendName, int friendId);

	private FriendClickedDelegate onFriendClicked;

	protected FriendSelection friendSelection;

	private FriendItem selectedFriend;

	public SHSFriendSelect(FriendClickedDelegate onFriendClicked)
	{
		this.onFriendClicked = onFriendClicked;
		List<GUIControl> list = new List<GUIControl>();
		GUIImage gUIImage = new GUIImage();
		gUIImage.SetSize(314f, 492f);
		gUIImage.SetPosition(0f, 0f);
		gUIImage.TextureSource = "persistent_bundle|brawler_gadget_left_bg";
		GUIImage gUIImage2 = new GUIImage();
		gUIImage2.SetSize(285f, 104f);
		gUIImage2.SetPosition(13f, 14f);
		gUIImage2.TextureSource = "persistent_bundle|brawler_gadget_left_fakefade_top";
		GUIImage gUIImage3 = new GUIImage();
		gUIImage3.SetSize(261f, 67f);
		gUIImage3.SetPosition(39f, 411f);
		gUIImage3.TextureSource = "persistent_bundle|brawler_gadget_left_fakefade_bottom";
		GUISlider gUISlider = new GUISlider();
		gUISlider.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(287f, 275f));
		gUISlider.SetSize(50f, 360f);
		friendSelection = new FriendSelection(gUISlider);
		friendSelection.SetSize(227f, 380f);
		friendSelection.SetPosition(54f, 89f);
		AddAndAddFade(gUIImage, list);
		AddAndAddFade(friendSelection, list);
		AddAndAddFade(gUIImage2, list);
		AddAndAddFade(gUIImage3, list);
		AddAndAddFade(gUISlider, list);
		base.AnimationOnOpen = SHSAnimations.WindowOpenCloseDelegates.FadeIn(0.3f, list.ToArray());
		base.AnimationOnClose = SHSAnimations.WindowOpenCloseDelegates.FadeOut(0.3f, list.ToArray());
	}

	public void AddList(List<FriendItem> friendsList)
	{
		friendSelection.AddList(friendsList);
		friendSelection.SortItemList();
	}

	private void AddAndAddFade(GUIControl ctrl, List<GUIControl> fade)
	{
		Add(ctrl);
		fade.Add(ctrl);
	}

	public void RemoveFriend(int playerId)
	{
		FriendItem friendItem = friendSelection.Find(delegate(FriendItem friendSearchItem)
		{
			return friendSearchItem.playerId == playerId;
		});
		if (friendItem == null)
		{
			CspUtils.DebugLog("trying to remove player " + playerId + " from a list, when not in the list");
			return;
		}
		friendSelection.RemoveItem(friendItem);
		if (selectedFriend == friendItem)
		{
			selectedFriend = null;
		}
	}
}
