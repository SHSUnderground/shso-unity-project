using System;
using UnityEngine;

public class SHSCardGameDeckListItem : SHSSelectionItem<GUISimpleControlWindow>, IComparable<SHSCardGameDeckListItem>
{
	public DeckProperties deck;

	private GUIButton button;

	private GUIImage deckIcon;

	private GUIDropShadowTextLabel DeckName;

	private GUIStrokeTextLabel SelectedDeckName;

	private GUIButton deleteButton;

	public event SHSCardGameDeckListItemDelegate OnDeckClicked;

	public event SHSCardGameDeckListDeleteDelegate OnDeckDelete;

	public SHSCardGameDeckListItem(DeckProperties deck)
	{
		this.deck = deck;
		item = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(new Vector2(459f, 55f), Vector2.zero);
		itemSize = new Vector2(459f, 65f);
		button = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(459f, 55f), Vector2.zero);
		button.StyleInfo = new SHSButtonStyleInfo("persistent_bundle|card_game_deck_name_module");
		item.Add(button);
		button.Click += button_Click;
		DeckName = GUIControl.CreateControlFrameCentered<GUIDropShadowTextLabel>(new Vector2(300f, 55f), new Vector2(-4f, 2f));
		DeckName.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(58, 71, 94), new Vector2(-2f, 1f), TextAnchor.MiddleLeft);
		DeckName.Traits.EventHandlingTrait = GUIControl.ControlTraits.EventHandlingEnum.Ignore;
		DeckName.Text = AppShell.Instance.stringTable[deck.DeckName];
		item.Add(DeckName);
		SelectedDeckName = GUIControl.CreateControlFrameCentered<GUIStrokeTextLabel>(new Vector2(300f, 55f), new Vector2(-4f, 2f));
		SelectedDeckName.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(173, 95, 16), GUILabel.GenColor(119, 64, 8), new Vector2(2f, 2f), TextAnchor.MiddleLeft);
		SelectedDeckName.Traits.EventHandlingTrait = GUIControl.ControlTraits.EventHandlingEnum.Ignore;
		SelectedDeckName.Text = AppShell.Instance.stringTable[deck.DeckName];
		SelectedDeckName.IsVisible = false;
		item.Add(SelectedDeckName);
		deleteButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(115f, 115f), new Vector2(167f, 1f));
		deleteButton.StyleInfo = new SHSButtonStyleInfo("persistent_bundle|L_card_game_delete_button");
		deleteButton.IsVisible = false;
		item.Add(deleteButton);
		deleteButton.Click += deleteButton_Click;
	}

	private void button_Click(GUIControl sender, GUIClickEvent EventData)
	{
		button.IsSelected = true;
		deleteButton.IsVisible = true;
		DeckName.IsVisible = false;
		SelectedDeckName.IsVisible = true;
		if (this.OnDeckClicked != null)
		{
			this.OnDeckClicked(deck);
		}
	}

	private void deleteButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		if (this.OnDeckDelete != null)
		{
			this.OnDeckDelete(deck);
		}
	}

	public void Unselect()
	{
		DeckName.IsVisible = true;
		SelectedDeckName.IsVisible = false;
		deleteButton.IsVisible = false;
	}

	public int CompareTo(SHSCardGameDeckListItem other)
	{
		return deck.DeckName.CompareTo(other.deck.DeckName);
	}

	public void Disable()
	{
		button.IsEnabled = false;
	}
}
