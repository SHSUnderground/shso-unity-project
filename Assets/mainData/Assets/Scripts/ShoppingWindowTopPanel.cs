using UnityEngine;

public class ShoppingWindowTopPanel : GUIDialogWindow
{
	private GUIImage _background;

	private GUIStrokeTextLabel _totalGoldLabel;

	private GUIStrokeTextLabel _totalFractalsLabel;

	private GUITextField _searchText;

	private Vector2 _offset = new Vector2(13f, 13f);

	public ShoppingWindowTopPanel()
	{
		AppShell.Instance.EventMgr.AddListener<CurrencyUpdateMessage>(OnCurrencyUpdateMessage);
		_background = new GUIImage();
		_background.SetSize(new Vector2(1050f, 71f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_background.Position = Vector2.zero;
		_background.TextureSource = "shopping_bundle|toppanel";
		_background.Id = "Background";
		Add(_background);
		GUIImage gUIImage = new GUIImage();
		gUIImage.SetSize(new Vector2(119f, 30f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIImage.Position = _offset;
		gUIImage.TextureSource = "shopping_bundle|searchbox";
		Add(gUIImage);
		_searchText = GUIControl.CreateControlTopLeftFrame<GUITextField>(gUIImage.Size, gUIImage.Position);
		_searchText.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, GUILabel.GenColor(81, 82, 81), TextAnchor.MiddleLeft);
		_searchText.Text = string.Empty;
		_searchText.WordWrap = false;
		_searchText.ToolTip = new NamedToolTipInfo("#characterselect_text_search_tt");
		_searchText.Changed += delegate
		{
			ShoppingWindow.instance.filterByText(_searchText.Text);
		};
		Add(_searchText);
		Vector2 size = new Vector2(61f, 61f);
		Vector2 position = gUIImage.Position;
		Vector2 size2 = gUIImage.Size;
		GUIImage gUIImage2 = GUIControl.CreateControlTopLeftFrame<GUIImage>(size, position + new Vector2(size2.x - 20f, -20f));
		gUIImage2.TextureSource = "persistent_bundle|gadget_searchbutton_normal";
		Add(gUIImage2);
		GUIButton gUIButton = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(247f, 49f), _offset + new Vector2(195f, -10f));
		gUIButton.StyleInfo = new SHSButtonStyleInfo("shopping_bundle|buymembershipbutton", SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		gUIButton.Click += delegate
		{
			ShoppingWindow.instance.buyMembership();
		};
		gUIButton.IsVisible = true;
		Add(gUIButton);
		GUIButton gUIButton2 = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(225f, 56f), gUIButton.Position + new Vector2(223f, -4f));
		gUIButton2.StyleInfo = new SHSButtonStyleInfo("shopping_bundle|buygoldbutton", SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		gUIButton2.Click += delegate
		{
			ShoppingWindow.instance.buyGold();
		};
		gUIButton2.IsVisible = true;
		Add(gUIButton2);
		GUIImage gUIImage3 = new GUIImage();
		gUIImage3.SetSize(new Vector2(223f, 40f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIImage3.Position = _offset + new Vector2(689f, -6f);
		gUIImage3.TextureSource = "shopping_bundle|topnav_currencyunderlay";
		Add(gUIImage3);
		GUIImage gUIImage4 = new GUIImage();
		gUIImage4.SetSize(new Vector2(36f, 33f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIImage4.Position = gUIImage3.Position + new Vector2(192f, 4f);
		gUIImage4.TextureSource = "shopping_bundle|gold_icon";
		gUIImage4.Id = "goldIcon";
		Add(gUIImage4);
		_totalGoldLabel = new GUIStrokeTextLabel();
		_totalGoldLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, gUIImage3.Position + new Vector2(120f, 10f), new Vector2(68f, 21f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_totalGoldLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 226, 90), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(2f, 3f), TextAnchor.MiddleRight);
		_totalGoldLabel.BackColorAlpha = 1f;
		_totalGoldLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		_totalGoldLabel.Id = "goldPriceLabel";
		Add(_totalGoldLabel);
		GUIImage gUIImage5 = new GUIImage();
		gUIImage5.SetSize(new Vector2(28f, 32f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIImage5.Position = gUIImage3.Position + new Vector2(3f, 4f);
		gUIImage5.TextureSource = "shopping_bundle|newfractal_small";
		gUIImage5.Id = "shardIcon";
		Add(gUIImage5);
		_totalFractalsLabel = new GUIStrokeTextLabel();
		_totalFractalsLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, gUIImage3.Position + new Vector2(34f, 10f), new Vector2(68f, 21f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_totalFractalsLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 16, GUILabel.GenColor(255, 136, 255), GUILabel.GenColor(0, 0, 0), GUILabel.GenColor(0, 0, 0), new Vector2(2f, 3f), TextAnchor.MiddleLeft);
		_totalFractalsLabel.BackColorAlpha = 1f;
		_totalFractalsLabel.AutoSizeText = GUILabel.AutoSizeTextEnum.ShrinkOnly;
		_totalFractalsLabel.Id = "shardPriceLabel";
		Add(_totalFractalsLabel);
		OnCurrencyUpdateMessage(null);
		SetSize(_background.Size);
	}

	public void resetSearch()
	{
		_searchText.Text = string.Empty;
	}

	private void OnCurrencyUpdateMessage(CurrencyUpdateMessage msg)
	{
		_totalGoldLabel.Text = string.Empty + ShoppingWindow.convertNumber(AppShell.Instance.Profile.Gold);
		_totalFractalsLabel.Text = string.Empty + ShoppingWindow.convertNumber(AppShell.Instance.Profile.Shards);
	}
}
