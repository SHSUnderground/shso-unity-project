using System;
using UnityEngine;

public class SHSGameWorldRailsShoppingWindow : SHSGadget.GadgetCenterWindow
{
	private GUILabel goalText;

	private GUIMovieTexture movieToShow;

	private GUITextArea description;

	private GUIButton playButton;

	private GUIButton stopButton;

	private GUITBOkButton okButton;

	public SHSGameWorldRailsShoppingWindow()
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
		goalText.Text = "Goal 1: Buy another hero for your collection!";
		Add(goalText);
		description = new GUITextArea();
		description.SetPositionAndSize(440f, 200f, 380f, 300f);
		description.FontSize = 20;
		description.TextColor = Color.white;
		description.Text = "\"Now that you have some silver, let's go get another hero for your collection.\"" + Environment.NewLine + "\"Stores look like this, and let you buy all kinds of fun things.\"" + Environment.NewLine + "\"Go click on the store, and then buy another hero for your collection.\"" + Environment.NewLine;
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
