using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SHSCardGameLoadSaveDialog : GUIDialogWindow
{
	public enum DialogMode
	{
		Load,
		Save,
		Gelatinous
	}

	public class DeckSelectionWindow : SHSSelectionWindow<SHSCardGameDeckListItem, GUISimpleControlWindow>
	{
		public DeckSelectionWindow(GUISlider slider)
			: base(slider, 459f, new Vector2(459f, 55f), 12)
		{
			SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
			TopOffsetAdjustHeight = 0f;
			BottomOffsetAdjustHeight = 0f;
			slider.FireChanged();
		}
	}

	private DialogMode currentMode;

	private string deckName;

	private DeckProperties selectedDeck;

	private DeckProperties lastDeck;

	private readonly GUIImage bgImage;

	private readonly GUIImage friendlyIconLoadImage;

	private readonly GUIImage friendlyIconSaveImage;

	private readonly GUIImage titleImage;

	private GUIImage icon;

	private readonly GUITBCloseButton closeButton;

	private readonly GUITextField decknameTextField;

	private readonly GUIImage decknameTextFieldBgdImage;

	private readonly DeckSelectionWindow deckSelectionList;

	private readonly GUISlider slider;

	private readonly GUIButton okButton;

	private readonly GUIButton cancelButton;

	[CompilerGenerated]
	private bool _003CLegalDecksOnly_003Ek__BackingField;

	public DialogMode CurrentMode
	{
		get
		{
			return currentMode;
		}
		set
		{
			currentMode = value;
			updateMode();
		}
	}

	public bool LegalDecksOnly
	{
		[CompilerGenerated]
		get
		{
			return _003CLegalDecksOnly_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CLegalDecksOnly_003Ek__BackingField = value;
		}
	}

	public string DeckName
	{
		get
		{
			return deckName;
		}
	}

	public DeckProperties SelectedDeck
	{
		get
		{
			return selectedDeck;
		}
	}

	public DeckProperties LastDeck
	{
		get
		{
			return lastDeck;
		}
	}

	public SHSCardGameLoadSaveDialog()
	{
		bgImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(648f, 463f), Vector2.zero);
		bgImage.TextureSource = "persistent_bundle|card_game_load_save_background";
		Add(bgImage);
		titleImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(171f, 40f), new Vector2(0f, -174f));
		titleImage.TextureSource = string.Format("persistent_bundle|{0}", (currentMode != 0) ? "L_card_game_save_title" : "L_card_game_load_title");
		Add(titleImage);
		friendlyIconLoadImage = GUIControl.CreateControlTopFrame<GUIImage>(new Vector2(229f, 192f), new Vector2(-300f, 80f));
		friendlyIconLoadImage.TextureSource = "persistent_bundle|notification_icon_loaddeck";
		friendlyIconLoadImage.IsVisible = false;
		Add(friendlyIconLoadImage);
		friendlyIconSaveImage = GUIControl.CreateControlTopFrame<GUIImage>(new Vector2(198f, 201f), new Vector2(-275f, 32f));
		friendlyIconSaveImage.TextureSource = "persistent_bundle|notification_icon_savedeck";
		friendlyIconSaveImage.IsVisible = false;
		Add(friendlyIconSaveImage);
		closeButton = GUIControl.CreateControlFrameCentered<GUITBCloseButton>(new Vector2(50f, 50f), new Vector2(287f, -150f));
		closeButton.Click += delegate
		{
			OnCancel();
		};
		Add(closeButton);
		decknameTextFieldBgdImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(292f, 69f), new Vector2(-111f, 151f));
		decknameTextFieldBgdImage.TextureSource = "persistent_bundle|L_card_game_textentryfield_active";
		Add(decknameTextFieldBgdImage);
		decknameTextField = GUIControl.CreateControlBottomFrame<GUITextField>(new Vector2(260f, 20f), new Vector2(-105f, -131f));
		decknameTextField.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(16, 44, 57), TextAnchor.MiddleLeft);
		decknameTextField.BackgroundColor = new Color(1f, 1f, 1f, 0f);
		decknameTextField.MaxLength = 140;
		decknameTextField.Id = "Deck Name";
		decknameTextField.WordWrap = false;
		decknameTextField.Changed += delegate
		{
			string text = decknameTextField.Text.Trim();
			if (!string.IsNullOrEmpty(text))
			{
				okButton.IsEnabled = true;
			}
			else
			{
				okButton.IsEnabled = false;
			}
			if (currentMode == DialogMode.Save && CardCollection.DeckList.ContainsKey(text))
			{
				DeckProperties deckProperties = CardCollection.DeckList[text];
				if (deckProperties.ReadOnly)
				{
					okButton.IsEnabled = false;
				}
				else
				{
					okButton.IsEnabled = true;
				}
			}
		};
		decknameTextField.Click += delegate
		{
		};
		decknameTextField.ToolTip = new NamedToolTipInfo("#CG_DECK_NAME_FIELD");
		Add(decknameTextField);
		slider = GUIControl.CreateControlFrameCentered<GUISlider>(new Vector2(50f, 210f), new Vector2(248f, -28f));
		slider.UseMouseWheelScroll = true;
		Add(slider);
		deckSelectionList = new DeckSelectionWindow(slider);
		deckSelectionList.SetSize(459f, 215f);
		deckSelectionList.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, -20f));
		deckSelectionList.TopOffsetAdjustHeight = 0f;
		deckSelectionList.BottomOffsetAdjustHeight = 0f;
		slider.FireChanged();
		Add(deckSelectionList);
		okButton = new GUIButton();
		okButton.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(37f, -64f), new Vector2(128f, 128f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		okButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|L_mshs_button_ok");
		okButton.Text = string.Empty;
		okButton.Click += okButton_Click;
		okButton.IsEnabled = false;
		Add(okButton);
		cancelButton = new GUIButton();
		cancelButton.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(140f, -54f), new Vector2(128f, 128f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		cancelButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|L_mshs_button_cancel");
		cancelButton.Text = string.Empty;
		cancelButton.Click += cancelButton_Click;
		Add(cancelButton);
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.DeckListLoaded>(OnDecksLoaded);
		CardCollection.EnumerateDecks();
		SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle);
		SetSize(new Vector2(1000f, 600f));
	}

	private void updateMode()
	{
		if (currentMode == DialogMode.Load)
		{
			decknameTextField.IsVisible = false;
			decknameTextFieldBgdImage.IsVisible = false;
			friendlyIconLoadImage.IsVisible = true;
			friendlyIconSaveImage.IsVisible = false;
		}
		else
		{
			decknameTextField.IsVisible = true;
			decknameTextFieldBgdImage.IsVisible = true;
			friendlyIconLoadImage.IsVisible = false;
			friendlyIconSaveImage.IsVisible = true;
		}
		titleImage.TextureSource = string.Format("persistent_bundle|{0}", (currentMode != 0) ? "L_card_game_save_title" : "L_card_game_load_title");
	}

	public void OnDecksLoaded(CardGameEvent.DeckListLoaded evt)
	{
		AppShell.Instance.EventMgr.RemoveListener<CardGameEvent.DeckListLoaded>(OnDecksLoaded);
		DeckProperties deckProperties = null;
		foreach (KeyValuePair<string, DeckProperties> deck in CardCollection.DeckList)
		{
			if (deck.Value.Legal)
			{
				if (AppShell.Instance.Profile.LastDeckID == 0 || deck.Value.DeckId == AppShell.Instance.Profile.LastDeckID)
				{
					lastDeck = deck.Value;
					break;
				}
				deckProperties = deck.Value;
			}
		}
		if (lastDeck == null)
		{
			lastDeck = deckProperties;
		}
		PopulateDeckList();
	}

	public override void OnShow()
	{
		base.OnShow();
		PopulateDeckList();
		if (decknameTextField == null)
		{
		}
	}

	public void PopulateDeckList()
	{
		deckSelectionList.ClearItems();
		foreach (KeyValuePair<string, DeckProperties> deck in CardCollection.DeckList)
		{
			SHSCardGameDeckListItem sHSCardGameDeckListItem = new SHSCardGameDeckListItem(deck.Value);
			sHSCardGameDeckListItem.OnDeckClicked += OnDeckClicked;
			sHSCardGameDeckListItem.OnDeckDelete += OnDeckDelete;
			deckSelectionList.AddItem(sHSCardGameDeckListItem);
			if (!sHSCardGameDeckListItem.deck.Legal && LegalDecksOnly)
			{
				sHSCardGameDeckListItem.Disable();
			}
			if (currentMode == DialogMode.Save && sHSCardGameDeckListItem.deck.ReadOnly)
			{
				sHSCardGameDeckListItem.Disable();
			}
		}
		deckSelectionList.SortItemList();
	}

	public void OnDeckClicked(DeckProperties deckClicked)
	{
		okButton.IsEnabled = true;
		selectedDeck = deckClicked;
		if (deckClicked.Legal)
		{
			AppShell.Instance.Profile.LastDeckID = deckClicked.DeckId;
		}
		else if (AppShell.Instance.Profile.LastDeckID == deckClicked.DeckId)
		{
			AppShell.Instance.Profile.LastDeckID = 0;
		}
		foreach (SHSCardGameDeckListItem item in deckSelectionList.items)
		{
			if (item.deck.DeckId != deckClicked.DeckId)
			{
				item.Unselect();
			}
		}
		if (currentMode == DialogMode.Save)
		{
			decknameTextField.Text = AppShell.Instance.stringTable[deckClicked.DeckName];
		}
	}

	public void OnDeckDelete(DeckProperties deckToDelete)
	{
		DeckProperties DeckToDelete = selectedDeck;
		if (DeckToDelete != null)
		{
			if (DeckToDelete.ReadOnly)
			{
				string text = string.Format(AppShell.Instance.stringTable["#CARD_STARTER_DECK_DELETE"], AppShell.Instance.stringTable[DeckToDelete.DeckName]);
				GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.OkDialog, text, delegate
				{
				}, ModalLevelEnum.Default);
			}
			else
			{
				GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.OkCancelDialog, "Are you sure you want to delete this deck?", delegate(string id, DialogState state)
				{
					if (state == DialogState.Ok)
					{
						WWWForm wWWForm = new WWWForm();
						wWWForm.AddField("deck_id", DeckToDelete.DeckId);
						AppShell.Instance.WebService.StartRequest("resources$users/decks_delete.py", delegate(ShsWebResponse response)
						{
							if (response.Status == 200)
							{
								CspUtils.DebugLog("Deleted deck " + DeckToDelete.DeckName + " with ID " + DeckToDelete.DeckId);
								AppShell.Instance.EventMgr.AddListener<CardGameEvent.DeckListLoaded>(OnDecksLoaded);
								CardCollection.EnumerateDecks();
								deckSelectionList.ClearItems();
								selectedDeck = null;
								okButton.IsEnabled = false;
								decknameTextField.Text = string.Empty;
								if (AppShell.Instance.Profile.LastDeckID == DeckToDelete.DeckId)
								{
									AppShell.Instance.Profile.LastDeckID = 0;
									AppShell.Instance.Profile.PersistExtendedData();
								}
							}
							else
							{
								CspUtils.DebugLog("Failed to delete deck " + DeckToDelete.DeckName + " with ID " + DeckToDelete.DeckId);
							}
						}, wWWForm.data, ShsWebService.ShsWebServiceType.RASP);
					}
				}, ModalLevelEnum.Default);
			}
		}
	}

	private void okButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		deckName = decknameTextField.Text;
		OnOk();
	}

	private void cancelButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		OnCancel();
	}
}
