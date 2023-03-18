using UnityEngine;

public class QuestOfTheDayItem : QuestItem
{
	private CardGameLaunchManager LaunchManager;

	private SHSCardGameGadgetQuestSelectionWindow owningWindow;

	private GUIImage background;

	private GUIStrokeTextLabel selectedLabel;

	private GUIStrokeTextLabel unselectedLabel;

	public override CardQuest CardQuest
	{
		get
		{
			return LaunchManager.DailyQuestPart.ParentQuest;
		}
	}

	public QuestOfTheDayItem(SHSCardGameGadgetWindow mainWindow, SHSCardGameGadgetQuestSelectionWindow owningWindow)
		: base("#CGG_BATTLEOFTHEDAY", QuestType.OfTheDay)
	{
		LaunchManager = mainWindow.LaunchManager;
		this.owningWindow = owningWindow;
		GUIHotSpotButton gUIHotSpotButton = GUIControl.CreateControlFrameCentered<GUIHotSpotButton>(new Vector2(212f, 78f), Vector2.zero);
		gUIHotSpotButton.Click += HotSpot_Click;
		gUIHotSpotButton.MouseOver += HotSpot_MouseOver;
		gUIHotSpotButton.MouseOut += HotSpot_MouseOut;
		AddSFXHandlers(gUIHotSpotButton);
		item.Add(gUIHotSpotButton);
		background = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(256f, 256f), new Vector2(15f, 2f));
		background.TextureSource = "cardgamegadget_bundle|button_selected_card_quest_dailybattle_gadget";
		background.Traits.EventHandlingTrait = GUIControl.ControlTraits.EventHandlingEnum.Ignore;
		item.Add(background);
		unselectedLabel = GUIControl.CreateControlFrameCentered<GUIStrokeTextLabel>(new Vector2(121f, 60f), new Vector2(23f, -5f));
		unselectedLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(31, 89, 188), GUILabel.GenColor(31, 89, 188), new Vector2(0f, 0f), TextAnchor.MiddleCenter);
		unselectedLabel.Traits.EventHandlingTrait = GUIControl.ControlTraits.EventHandlingEnum.Ignore;
		unselectedLabel.Text = "#title_daily_quest";
		unselectedLabel.Traits.VisibilityTrait = GUIControl.ControlTraits.VisibilityTraitEnum.Manual;
		unselectedLabel.IsVisible = true;
		item.Add(unselectedLabel);
		selectedLabel = GUIControl.CreateControlFrameCentered<GUIStrokeTextLabel>(new Vector2(121f, 60f), new Vector2(23f, -5f));
		selectedLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(93, 139, 26), GUILabel.GenColor(93, 139, 26), new Vector2(0f, 0f), TextAnchor.MiddleCenter);
		selectedLabel.Traits.EventHandlingTrait = GUIControl.ControlTraits.EventHandlingEnum.Ignore;
		selectedLabel.Text = "#title_daily_quest";
		selectedLabel.Traits.VisibilityTrait = GUIControl.ControlTraits.VisibilityTraitEnum.Manual;
		selectedLabel.IsVisible = false;
		item.Add(selectedLabel);
		item.OnVisible += base.OnBecomeVisible;
	}

	public override void OnSelected()
	{
		background.TextureSource = "cardgamegadget_bundle|button_daily_battle_selected";
		unselectedLabel.IsVisible = false;
		selectedLabel.IsVisible = true;
		base.IsSelected = true;
	}

	public override void OnDeselected()
	{
		background.TextureSource = "cardgamegadget_bundle|button_daily_battle_normal";
		selectedLabel.IsVisible = false;
		unselectedLabel.IsVisible = true;
		base.IsSelected = false;
	}

	private void HotSpot_MouseOver(GUIControl sender, GUIMouseEvent EventData)
	{
		if (!base.IsSelected)
		{
			background.TextureSource = "cardgamegadget_bundle|button_daily_battle_highlight";
		}
	}

	private void HotSpot_MouseOut(GUIControl sender, GUIMouseEvent EventData)
	{
		if (!base.IsSelected)
		{
			background.TextureSource = "cardgamegadget_bundle|button_daily_battle_normal";
		}
	}

	private void HotSpot_Click(GUIControl sender, GUIClickEvent EventData)
	{
		LaunchManager.SelectedQuest = CardQuest;
		if (owningWindow != null)
		{
			owningWindow.SelectQuestItem(this);
		}
	}
}
