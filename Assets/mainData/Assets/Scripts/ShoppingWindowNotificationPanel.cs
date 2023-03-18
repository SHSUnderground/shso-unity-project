using UnityEngine;

public class ShoppingWindowNotificationPanel : GUISimpleControlWindow
{
	private GUIImageWithEvents _background;

	private GUIStrokeTextLabel _messageTitleLabel;

	private GUILabel _messageLabel;

	private GUISimpleControlWindow _miscWindow;

	public ShoppingWindowNotificationPanel()
	{
		Traits.FullScreenOpaqueBackgroundTrait = ControlTraits.FullScreenOpaqueBackgroundTraitEnum.HasFullScreenOpaqueBackground;
		Traits.EventListenerRegistrationTrait = ControlTraits.EventListenerRegistrationTraitEnum.Ignore;
		_background = new GUIImageWithEvents();
		_background.SetSize(new Vector2(240f, 176f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_background.Position = Vector2.zero;
		_background.TextureSource = "shopping_bundle|upsellpanel";
		Add(_background);
		_background.Click += delegate
		{
		};
		_miscWindow = GUIControl.CreateControlTopLeftFrame<GUISimpleControlWindow>(_background.Size, new Vector2(18f, 15f));
		Add(_miscWindow);
		_miscWindow.IsVisible = true;
		_messageTitleLabel = new GUIStrokeTextLabel();
		_messageTitleLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(18f, 20f), new Vector2(215f, 21f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_messageTitleLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(231, 0, 0), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(2f, 3f), TextAnchor.UpperCenter);
		_messageTitleLabel.BackColorAlpha = 1f;
		_messageTitleLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		_messageTitleLabel.Text = string.Empty;
		Add(_messageTitleLabel);
		_messageLabel = new GUILabel();
		_messageLabel.FontSize = 12;
		_messageLabel.FontFace = GUIFontManager.SupportedFontEnum.Komica;
		_messageLabel.TextColor = GUILabel.GenColor(0, 0, 0);
		_messageLabel.TextAlignment = TextAnchor.UpperCenter;
		_messageLabel.WordWrap = true;
		_messageLabel.Text = string.Empty;
		GUILabel messageLabel = _messageLabel;
		Vector2 position = _messageTitleLabel.Position;
		messageLabel.SetPosition(18f, position.y + 25f);
		_messageLabel.SetSize(200f, 120f);
		Add(_messageLabel);
		SetSize(_background.Size);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		IsVisible = false;
	}

	public void displayMessage(string title, string message)
	{
		Alpha = 1f;
		_miscWindow.RemoveAllControls();
		_messageTitleLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(231, 0, 0), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(2f, 3f), TextAnchor.UpperCenter);
		_messageTitleLabel.Text = title;
		_messageLabel.Text = message;
		IsVisible = true;
	}

	public void displayBuyMembership()
	{
		Alpha = 1f;
		IsVisible = true;
		_miscWindow.RemoveAllControls();
		_miscWindow.IsVisible = true;
		GUIButton gUIButton = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(200f, 64f), new Vector2(5f, 80f));
		gUIButton.StyleInfo = new SHSButtonStyleInfo("shopping_bundle|widepurplebutton", SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		gUIButton.Click += delegate
		{
			ShoppingWindow.instance.buyMembership();
		};
		gUIButton.IsVisible = true;
		_miscWindow.Add(gUIButton);
		_messageTitleLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(98, 34, 164), GUILabel.GenColor(98, 34, 164), new Vector2(2f, 3f), TextAnchor.UpperCenter);
		_messageTitleLabel.Text = "#SHOP_AGENT_ONLY";
		_messageLabel.Text = "#SHOPPING_NAG_SUBSCRIPTION";
		GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
		gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, gUIButton.Position + new Vector2(10f, 6f), new Vector2(175f, 32f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 226, 90), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(2f, 3f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel.BackColorAlpha = 1f;
		gUIStrokeTextLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		gUIStrokeTextLabel.Text = "#SHOP_BUY_MEMBERSHIP";
		_miscWindow.Add(gUIStrokeTextLabel);
	}

	public void displayBuyGold()
	{
		Alpha = 1f;
		IsVisible = true;
		_miscWindow.RemoveAllControls();
		_miscWindow.IsVisible = true;
		GUIButton gUIButton = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(210f, 67f), new Vector2(5f, 50f));
		gUIButton.StyleInfo = new SHSButtonStyleInfo("shopping_bundle|widegoldbutton", SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		gUIButton.Click += delegate
		{
			ShoppingWindow.instance.buyGold();
		};
		gUIButton.IsVisible = true;
		_miscWindow.Add(gUIButton);
		_messageTitleLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(2f, 3f), TextAnchor.UpperCenter);
		_messageTitleLabel.Text = "#NEED_MORE_GOLD";
		_messageLabel.Text = string.Empty;
		GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
		gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, gUIButton.Position + new Vector2(10f, 6f), new Vector2(175f, 32f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 226, 90), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(2f, 3f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel.BackColorAlpha = 1f;
		gUIStrokeTextLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		gUIStrokeTextLabel.Text = "#SHOP_BUY_GOLD";
		_miscWindow.Add(gUIStrokeTextLabel);
	}

	public void displayGetFractals()
	{
		Alpha = 1f;
		IsVisible = true;
		_miscWindow.RemoveAllControls();
		_miscWindow.IsVisible = true;
		_messageLabel.Text = string.Empty;
		_messageTitleLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 136, 255), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(2f, 3f), TextAnchor.UpperCenter);
		_messageTitleLabel.Text = "#NEED_MORE_FRACTALS";
		GUILabel gUILabel = new GUILabel();
		gUILabel.FontSize = 14;
		gUILabel.FontFace = GUIFontManager.SupportedFontEnum.Komica;
		gUILabel.TextColor = GUILabel.GenColor(0, 0, 0);
		gUILabel.TextAlignment = TextAnchor.UpperCenter;
		gUILabel.WordWrap = true;
		gUILabel.Text = "#NEED_MORE_FRACTALS_DESC";
		gUILabel.SetPosition(new Vector2(5f, 30f));
		gUILabel.SetSize(217f, 120f);
		_miscWindow.Add(gUILabel);
		GUIButton gUIButton = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(210f, 67f), new Vector2(5f, 110f));
		gUIButton.StyleInfo = new SHSButtonStyleInfo("shopping_bundle|widegoldbutton", SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		gUIButton.Click += delegate
		{
			ShoppingWindow.instance.buyFractals();
		};
		gUIButton.IsVisible = true;
		_miscWindow.Add(gUIButton);
		GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
		gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, gUIButton.Position + new Vector2(10f, 6f), new Vector2(175f, 32f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 226, 90), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(2f, 3f), TextAnchor.MiddleCenter);
		gUIStrokeTextLabel.BackColorAlpha = 1f;
		gUIStrokeTextLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		gUIStrokeTextLabel.Text = "#SHOP_BUY_FRACTALS";
		_miscWindow.Add(gUIStrokeTextLabel);
	}
}
