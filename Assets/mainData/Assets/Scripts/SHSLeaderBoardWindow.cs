using UnityEngine;

public class SHSLeaderBoardWindow : GUIChildWindow
{
	private static readonly Vector2 CONTENT_SIZE = new Vector2(325f, 150f);

	private static readonly uint MAX_USERNAME_DISPLAY_LENGTH = 30u;

	private GUIStrokeTextLabel highScoreTitle;

	private GUILabel[] highScoreNames;

	private GUILabel[] highScores;

	private GUILayoutWindow boardWindow;

	private HighScoreEntryWindow[] highScoreTable;

	private AnimClip fadeAnim;

	public SHSLeaderBoardWindow()
	{
		highScoreTitle = GUIControl.CreateControlTopFrameCentered<GUIStrokeTextLabel>(new Vector2(325f, 31f), new Vector2(0f, 15f));
		highScoreTitle.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 31, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 114, 255), GUILabel.GenColor(0, 0, 0), new Vector2(4f, 3f), TextAnchor.MiddleCenter);
		highScoreTitle.Text = "#ARCADE_HIGH_SCORE_TITLE";
		highScoreTitle.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
		highScoreTitle.Id = "highscoreTitle";
		Add(highScoreTitle);
		boardWindow = new GUILayoutWindow();
		boardWindow.Id = "Leader Board Window";
		boardWindow.SetPositionAndSize(Vector2.zero, CONTENT_SIZE);
		boardWindow.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Inherit;
		boardWindow.Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		Add(boardWindow);
		boardWindow.Add(new GUILayoutBeginSection(GUILayoutBeginSection.OrientationEnum.Vertical));
		boardWindow.Add(new GUILayoutSpace(21));
		highScoreTable = new HighScoreEntryWindow[5];
		for (int i = 0; i < 5; i++)
		{
			HighScoreEntryWindow highScoreEntryWindow = new HighScoreEntryWindow(i == 0);
			highScoreEntryWindow.scoreLabel.Text = string.Format("{0:#,0}", i * 1000);
			highScoreEntryWindow.nameLabel.Text = "Player And  Really Long Name to Test Over 30char Limit" + i;
			highScoreTable[i] = highScoreEntryWindow;
			boardWindow.Add(highScoreEntryWindow);
		}
		boardWindow.Add(new GUILayoutEndSection(GUILayoutEndSection.OrientationEnum.Vertical));
	}

	public void ClearPlayerSlots()
	{
		HighScoreEntryWindow[] array = highScoreTable;
		foreach (HighScoreEntryWindow highScoreEntryWindow in array)
		{
			highScoreEntryWindow.scoreLabel.Text = string.Empty;
			highScoreEntryWindow.nameLabel.Text = string.Empty;
		}
	}

	public void UpdatePlayerSlot(int i, string name, int score)
	{
		if (i >= 0 && i < highScoreTable.Length)
		{
			if (name.Length > MAX_USERNAME_DISPLAY_LENGTH)
			{
				highScoreTable[i].nameLabel.Text = name.Substring(0, 26) + "...";
			}
			else
			{
				highScoreTable[i].nameLabel.Text = name;
			}
			highScoreTable[i].scoreLabel.Text = string.Format("{0:#,0}", score);
			if (name == AppShell.Instance.Profile.PlayerName)
			{
				highScoreTable[i].nameLabel.Color = HighScoreEntryWindow.SELF_COLOR;
				highScoreTable[i].scoreLabel.Color = HighScoreEntryWindow.SELF_COLOR;
			}
			else
			{
				highScoreTable[i].nameLabel.Color = ((i != 0) ? HighScoreEntryWindow.ENTRY_COLOR : HighScoreEntryWindow.TOP_COLOR);
				highScoreTable[i].scoreLabel.Color = ((i != 0) ? HighScoreEntryWindow.ENTRY_COLOR : HighScoreEntryWindow.TOP_COLOR);
			}
		}
	}

	public void FadeIn()
	{
		base.AnimationPieceManager.SwapOut(ref fadeAnim, SHSAnimations.Generic.FadeInVis(boardWindow, 0.2f));
	}

	public void FadeOut()
	{
		base.AnimationPieceManager.SwapOut(ref fadeAnim, SHSAnimations.Generic.FadeOutVis(boardWindow, 0.2f));
	}
}
