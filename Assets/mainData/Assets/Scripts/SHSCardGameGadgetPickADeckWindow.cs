using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSCardGameGadgetPickADeckWindow : SHSCardGameGadgetCenterWindowBase
{
	public class DeckSelectionWindow : SHSSelectionWindow<DeckItem, GUISimpleControlWindow>
	{
		public DeckSelectionWindow(GUISlider slider)
			: base(slider, 588f, new Vector2(294f, 68f), 12)
		{
			SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
			TopOffsetAdjustHeight = 20f;
			BottomOffsetAdjustHeight = 20f;
			slider.FireChanged();
		}
	}

	public class GoButtonWindow : SHSGlowOutlineWindow
	{
		private readonly SHSCardGameGadgetWindow mainWindow;

		public GoButtonWindow(SHSCardGameGadgetWindow mainWindow)
			: base(GetGoButtonGlowPath())
		{
			this.mainWindow = mainWindow;
			SetSize(360f, 122f);
			SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
			Offset = new Vector2(-2f, 241f);
			GUIButton gUIButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(512f, 512f), Vector2.zero);
			gUIButton.StyleInfo = new SHSButtonStyleInfo("persistent_bundle|L_okbutton_big");
			Add(gUIButton);
			gUIButton.Click += goButton_Click;
		}

		private void goButton_Click(GUIControl sender, GUIClickEvent EventData)
		{
			mainWindow.GoToWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.Main);
		}

		private static List<Vector2> GetGoButtonGlowPath()
		{
			return SHSGlowOutlineWindow.GetGlowPath(new Vector2(-161f, -28f), new Vector2(-137f, -39f), new Vector2(-89f, -45f), new Vector2(0f, -48f), new Vector2(89f, -45f), new Vector2(137f, -39f), new Vector2(161f, -28f), new Vector2(160f, 0f), new Vector2(145f, 36f), new Vector2(127f, 42f), new Vector2(0f, 46f), new Vector2(-127f, 42f), new Vector2(-145f, 36f), new Vector2(-160f, 0f));
		}

		public override void OnShow()
		{
			base.OnShow();
			Highlight(true);
		}
	}

	public class DeckItem : SHSSelectionItem<GUISimpleControlWindow>, IComparable<DeckItem>
	{
		public DeckProperties deck;

		private GUIButton button;

		private GUIImage deckIcon;

		public event DeckClickedDelegate OnDeckClicked;

		public DeckItem(DeckProperties deck)
		{
			this.deck = deck;
			item = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(new Vector2(302f, 77f), Vector2.zero);
			itemSize = new Vector2(302f, 77f);
			button = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(302f, 77f), Vector2.zero);
			button.StyleInfo = new SHSButtonStyleInfo("cardgamegadget_bundle|cardlauncher_deckselect");
			item.Add(button);
			button.Click += button_Click;
			deckIcon = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(60f, 60f), new Vector2(-90f, 0f));
			deckIcon.TextureSource = "characters_bundle|inventory_character_" + deck.HeroName + "_normal";
			item.Add(deckIcon);
			GUIDropShadowTextLabel gUIDropShadowTextLabel = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(150f, 60f), new Vector2(20f, 0f));
			gUIDropShadowTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(58, 71, 94), new Vector2(-2f, 1f), TextAnchor.MiddleLeft);
			gUIDropShadowTextLabel.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Ignore;
			gUIDropShadowTextLabel.Text = deck.DeckName;
			item.Add(gUIDropShadowTextLabel);
		}

		private void button_Click(GUIControl sender, GUIClickEvent EventData)
		{
			button.IsSelected = true;
			if (this.OnDeckClicked != null)
			{
				this.OnDeckClicked(deck);
			}
		}

		public int CompareTo(DeckItem other)
		{
			return deck.DeckName.CompareTo(other.deck.DeckName);
		}

		public void Disable()
		{
			button.IsEnabled = false;
			deckIcon.IsEnabled = false;
		}
	}

	public delegate void DeckClickedDelegate(DeckProperties deck);

	private DeckSelectionWindow deckSelect;

	private bool DeckListenerRegistered;

	public SHSCardGameGadgetPickADeckWindow(SHSCardGameGadgetWindow mainWindow)
		: base(mainWindow)
	{
		GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(758f, 440f), new Vector2(0f, 28f));
		gUIImage.TextureSource = "cardgamegadget_bundle|cardlauncher_choosedeck_bg_frame";
		GUISlider gUISlider = GUIControl.CreateControlFrameCentered<GUISlider>(new Vector2(50f, 310f), new Vector2(321f, 17f));
		deckSelect = new DeckSelectionWindow(gUISlider);
		deckSelect.SetSize(590f, 304f);
		deckSelect.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(6f, 27f));
		GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(640f, 289f), new Vector2(6f, 27f));
		gUIImage2.TextureSource = "cardgamegadget_bundle|cardlauncher_choosedeck_bg_fill";
		GoButtonWindow goButtonWindow = new GoButtonWindow(mainWindow);
		goButtonWindow.Click += delegate
		{
			mainWindow.GoToWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.Main);
		};
		GUIButton gUIButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(-224f, 248f));
		gUIButton.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_brawler_airlock_backbutton");
		gUIButton.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton.Click += delegate
		{
			mainWindow.GoToWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.Main);
		};
		GUIButton gUIButton2 = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(224f, 248f));
		gUIButton2.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_brawler_airlock_quitbutton");
		gUIButton2.HitTestType = HitTestTypeEnum.Alpha;
		Add(gUIImage2);
		Add(deckSelect);
		Add(gUIImage);
		Add(gUISlider);
		Add(goButtonWindow);
		Add(gUIButton);
		Add(gUIButton2);
	}

	public override void OnShow()
	{
		base.OnShow();
		if (mainWindow.LaunchManager.DecksLoaded)
		{
			PopulateDeckList();
			return;
		}
		DeckListenerRegistered = true;
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.DeckListLoaded>(OnDecksLoaded);
	}

	public override void OnHide()
	{
		base.OnHide();
		if (DeckListenerRegistered)
		{
			AppShell.Instance.EventMgr.AddListener<CardGameEvent.DeckListLoaded>(OnDecksLoaded);
			DeckListenerRegistered = false;
		}
	}

	public void OnDecksLoaded(CardGameEvent.DeckListLoaded evt)
	{
		AppShell.Instance.EventMgr.RemoveListener<CardGameEvent.DeckListLoaded>(OnDecksLoaded);
		DeckListenerRegistered = false;
		PopulateDeckList();
	}

	public void OnDeckClicked(DeckProperties deckClicked)
	{
		mainWindow.LaunchManager.SelectedDeck = deckClicked;
		AppShell.Instance.Profile.LastDeckID = deckClicked.DeckId;
		AppShell.Instance.Profile.PersistExtendedData();
	}

	public void PopulateDeckList()
	{
		deckSelect.ClearItems();
		foreach (KeyValuePair<string, DeckProperties> deck in CardCollection.DeckList)
		{
			DeckItem deckItem = new DeckItem(deck.Value);
			deckItem.OnDeckClicked += OnDeckClicked;
			deckSelect.AddItem(deckItem);
			if (!deckItem.deck.Legal)
			{
				deckItem.Disable();
			}
		}
		deckSelect.SortItemList();
	}
}
