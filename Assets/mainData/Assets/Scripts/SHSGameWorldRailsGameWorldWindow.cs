using System;
using UnityEngine;

public class SHSGameWorldRailsGameWorldWindow : SHSGadget.GadgetCenterWindow
{
	private GUILabel goalText;

	private GUIMovieTexture movieToShow;

	private GUITextArea description;

	private GUIButton playButton;

	private GUIButton stopButton;

	private GUITBOkButton okButton;

	public SHSGameWorldRailsGameWorldWindow()
	{
		base.Skin = GUIManager.Instance.StyleManager.GetSkin("rootSkin");
	}

	public override bool InitializeResources(bool reload)
	{
		goalText = new GUILabel();
		goalText.SetPositionAndSize(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, new Vector2(500f, 40f));
		goalText.OffsetStyle = OffsetType.Absolute;
		goalText.Offset = new Vector2(0f, 120f);
		goalText.FontSize = 30;
		goalText.TextAlignment = TextAnchor.MiddleCenter;
		goalText.TextColor = Color.white;
		goalText.Text = "Goal 1: Spin the wheel for a prize!";
		Add(goalText);
		description = new GUITextArea();
		description.SetPositionAndSize(440f, 200f, 380f, 300f);
		description.FontSize = 20;
		description.TextColor = Color.white;
		description.Text = "\"Welcome to Super Hero City! Now that you have two heroes, you have to pick which hero you want to use.\"" + Environment.NewLine + "\"When you enter a new area, your hero list will appear.\"" + Environment.NewLine + "\"Click on one to continue.\"" + Environment.NewLine;
		Add(description);
		movieToShow = new GUIMovieTexture();
		movieToShow.SetPositionAndSize(100f, 200f, 300f, 250f);
		movieToShow.TextureSource = "tutorial_bundle|howtojumpMovie";
		movieToShow.AutoPlay = true;
		Add(movieToShow);
		playButton = new GUIButton();
		playButton.SetPositionAndSize(120f, 470f, 70f, 40f);
		playButton.Text = "PLAY";
		playButton.Click += play_Click;
		Add(playButton);
		stopButton = new GUIButton();
		stopButton.SetPositionAndSize(190f, 470f, 70f, 40f);
		stopButton.Text = "STOP";
		stopButton.Click += stop_Click;
		Add(stopButton);
		okButton = new GUITBOkButton();
		okButton.SetPosition(450f, 570f, AnchorAlignmentEnum.Middle);
		okButton.Click += okButton_Click;
		Add(okButton);
		return base.InitializeResources(reload);
	}

	private void okButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		Hide();
		parent.Parent.Hide();
	}

	private void stop_Click(GUIControl sender, GUIClickEvent EventData)
	{
		movieToShow.Stop();
	}

	private void play_Click(GUIControl sender, GUIClickEvent EventData)
	{
		movieToShow.Play();
	}
}
