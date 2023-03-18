using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SHSCardGameGadgetQuestSelectionWindow : GUISimpleControlWindow
{
	private QuestSelectionListWindow questSelection;

	private SHSCardGameGadgetPickAQuestWindow.QuestClickedDelegate OnQuestClicked;

	private SHSCardGameGadgetWindow mainWindow;

	[CompilerGenerated]
	private QuestItem _003CSelectedQuestItem_003Ek__BackingField;

	public QuestItem SelectedQuestItem
	{
		[CompilerGenerated]
		get
		{
			return _003CSelectedQuestItem_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CSelectedQuestItem_003Ek__BackingField = value;
		}
	}

	public SHSCardGameGadgetQuestSelectionWindow(SHSCardGameGadgetWindow mainWindow, SHSCardGameGadgetPickAQuestWindow.QuestClickedDelegate OnQuestClicked)
	{
		this.mainWindow = mainWindow;
		this.OnQuestClicked = OnQuestClicked;
		SetSize(new Vector2(314f, 492f));
		SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-242f, 36f));
		GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(314f, 496f), Vector2.zero);
		gUIImage.TextureSource = "cardgamegadget_bundle|cardlauncher_QuestListtwindow_backframe";
		GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(314f, 496f), Vector2.zero);
		gUIImage2.TextureSource = "cardgamegadget_bundle|cardlauncher_QuestListtwindow_frontframe";
		GUISlider gUISlider = GUIControl.CreateControlFrameCentered<GUISlider>(new Vector2(50f, 300f), new Vector2(131f, -25f));
		questSelection = new QuestSelectionListWindow(gUISlider);
		questSelection.SetSize(227f, 320f);
		questSelection.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(2f, -17f));
		GUIImage gUIImage3 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(81f, 81f), new Vector2(102f, -199f));
		gUIImage3.TextureSource = "persistent_bundle|gadget_searchbutton_normal";
		GUITextField search = GUIControl.CreateControlFrameCentered<GUITextField>(new Vector2(178f, 55f), new Vector2(-20f, -198f));
		search.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, GUILabel.GenColor(81, 82, 81), TextAnchor.MiddleLeft);
		search.Rotation = -3f;
		search.WordWrap = false;
		search.Changed += delegate
		{
			questSelection.Sort(search.Text);
		};
		GUIStrokeTextButton gUIStrokeTextButton = new GUIStrokeTextButton("cardgamegadget_bundle|L_cardlauncher_button_buycards_fromquests", new Vector2(256f, 256f), new Vector2(0f, 0f), "#CGG_BUYMOREQUESTS", new Vector2(130f, 100f), new Vector2(47f, 10f));
		gUIStrokeTextButton.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(9f, 189f), new Vector2(240f, 126f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextButton.ButtonLabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 27, ColorUtil.FromRGB255(251, 255, 213), ColorUtil.FromRGB255(97, 124, 16), ColorUtil.FromRGB255(71, 118, 5), new Vector2(-3f, 5f), TextAnchor.MiddleLeft);
		gUIStrokeTextButton.ButtonLabel.Rotation = 0f;
		gUIStrokeTextButton.Click += delegate
		{
			ShoppingWindow shoppingWindow = new ShoppingWindow(84537);
			shoppingWindow.launch();
		};
		gUIStrokeTextButton.Id = "buycardsButton";
		Add(gUIImage);
		Add(questSelection);
		Add(gUIImage2);
		Add(gUIImage3);
		Add(search);
		Add(gUIStrokeTextButton);
		Add(gUISlider);
	}

	public void SelectQuestItem(QuestItem questItem)
	{
		QuestItem selectedQuestItem = SelectedQuestItem;
		SelectedQuestItem = questItem;
		if (selectedQuestItem != null)
		{
			selectedQuestItem.OnDeselected();
		}
		if (questItem != null)
		{
			questItem.OnSelected();
		}
		if (OnQuestClicked != null)
		{
			CardQuest cardQuest = questItem.CardQuest;
			if (cardQuest != null)
			{
				OnQuestClicked(cardQuest);
			}
		}
		questSelection.RequestARefresh();
	}

	public override void OnShow()
	{
		PopulateQuests();
		base.OnShow();
		AppShell.Instance.EventMgr.AddListener<CardQuestFetchCompleteMessage>(OnCardQuestFetchComplete);
	}

	public override void OnHide()
	{
		AppShell.Instance.EventMgr.RemoveListener<CardQuestFetchCompleteMessage>(OnCardQuestFetchComplete);
		base.OnHide();
	}

	private void OnCardQuestFetchComplete(CardQuestFetchCompleteMessage message)
	{
		PopulateQuests();
	}

	private void PopulateQuests()
	{
		questSelection.ClearItems();
		QuestItem questItem = null;
		if (mainWindow.LaunchManager.DailyQuestPart != null)
		{
			QuestOfTheDayItem questOfTheDayItem = new QuestOfTheDayItem(mainWindow, this);
			questSelection.AddItem(questOfTheDayItem);
			questItem = questOfTheDayItem;
		}
		CardQuestManager cardQuestManager = AppShell.Instance.CardQuestManager;
		if (!mainWindow.LaunchManager.ShowOnlyDailyQuest)
		{
			List<CardQuest> list = new List<CardQuest>();
			foreach (AvailableQuest value in AppShell.Instance.Profile.AvailableQuests.Values)
			{
				CardQuest questByPartId = cardQuestManager.GetQuestByPartId(value.QuestId);
				if (questByPartId != null && !list.Contains(questByPartId))
				{
					list.Add(questByPartId);
				}
			}
			foreach (CardQuest item in list)
			{
				OwnedQuestItem ownedQuestItem = new OwnedQuestItem(item, this);
				questSelection.AddItem(ownedQuestItem);
				if (!mainWindow.LaunchManager.SelectDailyQuest && item == mainWindow.LaunchManager.SelectedQuest)
				{
					questItem = ownedQuestItem;
				}
			}
			questSelection.SortItemList();
		}
		if (questItem != null)
		{
			SelectQuestItem(questItem);
		}
	}
}
