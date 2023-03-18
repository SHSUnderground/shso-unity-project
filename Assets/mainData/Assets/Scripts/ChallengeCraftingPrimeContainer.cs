using UnityEngine;

public class ChallengeCraftingPrimeContainer : GUISimpleControlWindow
{
	private GUILabel _quantityLabel;

	private GUIButton _unreadyButton;

	private GUIImage _craftImage;

	private GUIButton _selected;

	private GUIButton _readyButton;

	private int _quantityNeeded = 1;

	private OwnableDefinition _def;

	private ChallengeCraftingWindow _parent;

	public ChallengeCraftingPrimeContainer(ChallengeCraftingWindow parent)
	{
		_parent = parent;
		SetSize(new Vector2(80f, 84f));
		_unreadyButton = new GUIButton();
		_unreadyButton.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(74f, 78f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_unreadyButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|container_crafting_items_prime", SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		_unreadyButton.Click += clickedButton;
		_unreadyButton.IsVisible = false;
		Add(_unreadyButton);
		_selected = new GUIButton();
		_selected.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(74f, 78f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_selected.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|container_crafting_items_prime_selected", SHSButtonStyleInfo.SupportedStatesEnum.Normal);
		_selected.IsVisible = false;
		_selected.Click += clickedButton;
		Add(_selected);
		_readyButton = new GUIButton();
		_readyButton.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(74f, 78f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_readyButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|container_crafting_items_prime_ready", SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		_readyButton.Click += clickedButton;
		_readyButton.IsVisible = false;
		Add(_readyButton);
		_craftImage = new GUIImage();
		_craftImage.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(64f, 68f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_craftImage.IsVisible = true;
		Add(_craftImage);
		_quantityLabel = new GUILabel();
		_quantityLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(138, 211, 14), TextAnchor.UpperRight);
		_quantityLabel.SetPositionAndSize(DockingAlignmentEnum.BottomRight, AnchorAlignmentEnum.BottomRight, OffsetType.Absolute, new Vector2(-13f, 0f), new Vector2(80f, 30f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_quantityLabel.Text = "1199/99";
		_quantityLabel.IsVisible = false;
		Add(_quantityLabel);
	}

	public void setup(OwnableDefinition def, int quantityNeeded, int currentQuantity)
	{
		_quantityNeeded = quantityNeeded;
		_def = def;
		_craftImage.TextureSource = def.iconBase;
		_craftImage.IsVisible = true;
		Vector2 offset = new Vector2(50f, 0f);
		_unreadyButton.ToolTip = new NamedToolTipInfo(def.name, offset);
		_readyButton.ToolTip = new NamedToolTipInfo(def.name, offset);
		_selected.ToolTip = new NamedToolTipInfo(def.name, offset);
		setQuantity(currentQuantity);
		setState(false, false);
		isVisible = true;
	}

	private void clickedButton(GUIControl sender, GUIClickEvent EventData)
	{
		_parent.selectPiece(_def.ownableTypeID);
	}

	public void setState(bool isReady, bool isSelected)
	{
		_unreadyButton.IsVisible = false;
		_selected.IsVisible = false;
		_readyButton.IsVisible = false;
		if (isSelected)
		{
			_selected.IsVisible = true;
		}
		else if (isReady)
		{
			_readyButton.IsVisible = true;
		}
		else
		{
			_unreadyButton.IsVisible = true;
		}
		if (isReady)
		{
			_quantityLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(138, 211, 14), TextAnchor.UpperRight);
		}
		else
		{
			_quantityLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(104, 160, 9), TextAnchor.UpperRight);
		}
	}

	public void setQuantity(int quantity)
	{
		_quantityLabel.Text = quantity + "/" + _quantityNeeded;
		_quantityLabel.IsVisible = true;
	}
}
