using UnityEngine;

public class HighScoreEntryWindow : GUIChildWindow
{
	private static readonly Vector2 CONTENT_SIZE = new Vector2(325f, 21f);

	public GUILabel nameLabel;

	public GUILabel scoreLabel;

	public static readonly Color TOP_COLOR = ColorUtil.FromRGB255(143, 224, 254);

	public static readonly Color ENTRY_COLOR = ColorUtil.FromRGB255(100, 200, 220);

	public static readonly Color SELF_COLOR = ColorUtil.FromRGB255(162, 255, 0);

	public HighScoreEntryWindow()
		: this(false)
	{
	}

	public HighScoreEntryWindow(bool isTopScore)
	{
		SetPositionAndSize(Vector2.zero, CONTENT_SIZE);
		int fontSize = (!isTopScore) ? 17 : 20;
		Color color = (!isTopScore) ? ENTRY_COLOR : TOP_COLOR;
		nameLabel = new GUILabel();
		nameLabel.SetPositionAndSize(Vector2.zero, CONTENT_SIZE);
		nameLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, fontSize, color, TextAnchor.MiddleLeft);
		nameLabel.Id = "nameLabel";
		nameLabel.Text = "nameLabel";
		Add(nameLabel);
		scoreLabel = new GUILabel();
		scoreLabel.SetPositionAndSize(Vector2.zero, CONTENT_SIZE);
		scoreLabel.Id = "scoreLabel";
		scoreLabel.Text = "0";
		scoreLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, fontSize, color, TextAnchor.MiddleRight);
		Add(scoreLabel);
	}
}
