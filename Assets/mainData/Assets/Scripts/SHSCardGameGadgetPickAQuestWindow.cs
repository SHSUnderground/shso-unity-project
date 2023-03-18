using UnityEngine;

public class SHSCardGameGadgetPickAQuestWindow : SHSCardGameGadgetCenterWindowBase
{
	public delegate void QuestClickedDelegate(CardQuest questPartClicked);

	private SHSScanlineTransitionWindow<CardQuest> questInfoWindow;

	public SHSCardGameGadgetPickAQuestWindow(SHSCardGameGadgetWindow mainWindow)
		: base(mainWindow)
	{
		SHSCardGameGadgetQuestSelectionWindow control = new SHSCardGameGadgetQuestSelectionWindow(mainWindow, OnQuestClicked);
		Add(control);
		questInfoWindow = new SHSScanlineTransitionWindow<CardQuest>(GetQuestInfoWindow);
		questInfoWindow.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(172f, 25f));
		Add(questInfoWindow);
	}

	public override void OnShow()
	{
		base.OnShow();
		questInfoWindow.InitialSetup(mainWindow.LaunchManager.SelectedQuest);
	}

	public void OnQuestClicked(CardQuest questClicked)
	{
		questInfoWindow.BeginTransition(questClicked);
	}

	public SHSCardGameGadgetQuestDescriptionWindow GetQuestInfoWindow(CardQuest questClicked)
	{
		return new SHSCardGameGadgetQuestDescriptionWindow(questClicked, this);
	}

	public void QuestBattleSelected(CardQuestPart.QuestBattle selectedQuestBattle)
	{
		mainWindow.LaunchManager.SelectedBattle = selectedQuestBattle;
		mainWindow.LaunchManager.StartMission();
	}

	public void QuestBattleCancel()
	{
		mainWindow.GoToWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.Main);
	}

	public void QuestBattleQuit()
	{
		mainWindow.GoToWindow(SHSCardGameGadgetWindow.CardGameWindowTypeEnum.ScreenWithBigPlayButtonThatQuoteEverybodyQuoteLoves);
	}
}
