using UnityEngine;

public class OwnedQuestItem : QuestItem
{
	public const float CharIconOffsetYNormal = -12f;

	public const float CharIconOffsetYHighlight = -16f;

	public const float HoverAnimTime = 0.18f;

	public static readonly Vector2 BkgSize = new Vector2(212f, 65f);

	public static readonly Vector2 ChestSize = new Vector2(80f, 78f);

	public static readonly Vector2 CharIconSize = new Vector2(70f, 70f);

	private CardQuest _cardQuest;

	private GUIImage ChestBack;

	private GUIImage QuestIcon;

	private GUIImage ChestFront;

	private GUIImage bkg;

	private GUIImage selectedBackground;

	private GUIStrokeTextLabel selectedLabel;

	private GUILabel unselectedLabel;

	private SHSCardGameGadgetQuestSelectionWindow owningWindow;

	private AnimClip currentHoverAnimation;

	public override CardQuest CardQuest
	{
		get
		{
			return _cardQuest;
		}
		protected set
		{
			_cardQuest = value;
		}
	}

	public OwnedQuestItem(CardQuest cardQuest, SHSCardGameGadgetQuestSelectionWindow owningWindow)
		: base(cardQuest.Name, QuestType.Regular)
	{
		CardQuest = cardQuest;
		this.owningWindow = owningWindow;
		GUIHotSpotButton gUIHotSpotButton = GUIControl.CreateControlFrameCentered<GUIHotSpotButton>(new Vector2(212f, 78f), Vector2.zero);
		gUIHotSpotButton.Click += HotSpot_Click;
		gUIHotSpotButton.MouseOut += HotSpot_MouseOut;
		gUIHotSpotButton.MouseOver += HotSpot_MouseOver;
		AddSFXHandlers(gUIHotSpotButton);
		item.Add(gUIHotSpotButton);
		bkg = GUIControl.CreateControlFrameCentered<GUIImage>(BkgSize, Vector2.zero);
		bkg.TextureSource = "cardgamegadget_bundle|cardlauncher_quest_left_container";
		item.Add(bkg);
		selectedBackground = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(256f, 256f), new Vector2(18f, 0f));
		selectedBackground.TextureSource = "cardgamegadget_bundle|button_selected_card_quest_gadget";
		selectedBackground.Traits.EventHandlingTrait = GUIControl.ControlTraits.EventHandlingEnum.Ignore;
		selectedBackground.Traits.VisibilityTrait = GUIControl.ControlTraits.VisibilityTraitEnum.Manual;
		selectedBackground.IsVisible = false;
		item.Add(selectedBackground);
		ChestBack = GUIControl.CreateControlFrameCentered<GUIImage>(ChestSize, new Vector2(-67f, 0f));
		ChestBack.TextureSource = "cardgamegadget_bundle|cardlauncher_quest_chestback";
		item.Add(ChestBack);
		QuestIcon = GUIControl.CreateControlFrameCentered<GUIImage>(CharIconSize, new Vector2(-63f, -12f));
		QuestIcon.TextureSource = "characters_bundle|inventory_character_" + cardQuest.Sponsor + "_normal";
		item.Add(QuestIcon);
		ChestFront = GUIControl.CreateControlFrameCentered<GUIImage>(ChestSize, new Vector2(-67f, 0f));
		ChestFront.TextureSource = "cardgamegadget_bundle|cardlauncher_quest_chestfront";
		item.Add(ChestFront);
		unselectedLabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(121f, 60f), new Vector2(35f, -5f));
		unselectedLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(58, 71, 94), TextAnchor.MiddleCenter);
		unselectedLabel.Traits.EventHandlingTrait = GUIControl.ControlTraits.EventHandlingEnum.Ignore;
		unselectedLabel.Traits.VisibilityTrait = GUIControl.ControlTraits.VisibilityTraitEnum.Manual;
		unselectedLabel.IsVisible = true;
		unselectedLabel.Text = cardQuest.Name;
		item.Add(unselectedLabel);
		selectedLabel = GUIControl.CreateControlFrameCentered<GUIStrokeTextLabel>(new Vector2(121f, 60f), new Vector2(35f, -5f));
		selectedLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(93, 139, 26), GUILabel.GenColor(93, 139, 26), new Vector2(0f, 0f), TextAnchor.MiddleCenter);
		selectedLabel.Traits.EventHandlingTrait = GUIControl.ControlTraits.EventHandlingEnum.Ignore;
		selectedLabel.Text = cardQuest.Name;
		selectedLabel.Traits.VisibilityTrait = GUIControl.ControlTraits.VisibilityTraitEnum.Manual;
		selectedLabel.IsVisible = false;
		item.Add(selectedLabel);
		item.OnVisible += base.OnBecomeVisible;
	}

	public override void OnSelected()
	{
		selectedBackground.IsVisible = true;
		unselectedLabel.IsVisible = false;
		selectedLabel.IsVisible = true;
		base.IsSelected = true;
		bkg.IsVisible = false;
		QuestIcon.TextureSource = "characters_bundle|inventory_character_" + CardQuest.Sponsor + "_highlight";
	}

	public override void OnDeselected()
	{
		selectedBackground.IsVisible = false;
		selectedLabel.IsVisible = false;
		unselectedLabel.IsVisible = true;
		base.IsSelected = false;
		bkg.IsVisible = true;
		QuestIcon.TextureSource = "characters_bundle|inventory_character_" + CardQuest.Sponsor + "_normal";
	}

	private void HotSpot_MouseOver(GUIControl sender, GUIMouseEvent EventData)
	{
		if (!base.IsSelected)
		{
			Vector2 offset = QuestIcon.Offset;
			float time = SHSAnimations.GenericFunctions.FrationalTime(-12f, -16f, offset.y, 0.18f);
			AnimClipManager animationPieceManager = item.AnimationPieceManager;
			//ref AnimClip oldPiece = ref currentHoverAnimation;
			AnimClip pieceOne = SHSAnimations.Generic.ChangeSizeDirect(ChestBack, ChestSize * 1.05f, ChestSize, 0.18f, 0f) ^ SHSAnimations.Generic.ChangeSizeDirect(ChestFront, ChestSize * 1.05f, ChestSize, 0.18f, 0f) ^ SHSAnimations.Generic.ChangeSizeDirect(QuestIcon, CharIconSize * 1.1f, CharIconSize, 0.18f, 0f) ^ SHSAnimations.Generic.ChangeSizeDirect(bkg, BkgSize * 1.05f, BkgSize, 0.18f, 0f);
			Vector2 offset2 = QuestIcon.Offset;
			animationPieceManager.SwapOut(ref currentHoverAnimation, pieceOne ^ AnimClipBuilder.Absolute.OffsetY(AnimClipBuilder.Path.Linear(offset2.y, -16f, time), QuestIcon));
			QuestIcon.TextureSource = "characters_bundle|inventory_character_" + CardQuest.Sponsor + "_highlight";
		}
	}

	private void HotSpot_MouseOut(GUIControl sender, GUIMouseEvent EventData)
	{
		if (!base.IsSelected)
		{
			Vector2 offset = QuestIcon.Offset;
			float time = SHSAnimations.GenericFunctions.FrationalTime(-16f, -12f, offset.y, 0.18f);
			AnimClipManager animationPieceManager = item.AnimationPieceManager;
			//ref AnimClip oldPiece = ref currentHoverAnimation;
			AnimClip pieceOne = SHSAnimations.Generic.ChangeSizeDirect(ChestBack, ChestSize, ChestSize * 1.05f, 0.18f, 0f) ^ SHSAnimations.Generic.ChangeSizeDirect(ChestFront, ChestSize, ChestSize * 1.05f, 0.18f, 0f) ^ SHSAnimations.Generic.ChangeSizeDirect(QuestIcon, CharIconSize, CharIconSize * 1.1f, 0.18f, 0f) ^ SHSAnimations.Generic.ChangeSizeDirect(bkg, BkgSize, BkgSize * 1.05f, 0.18f, 0f);
			Vector2 offset2 = QuestIcon.Offset;
			animationPieceManager.SwapOut(ref currentHoverAnimation, pieceOne ^ AnimClipBuilder.Absolute.OffsetY(AnimClipBuilder.Path.Linear(offset2.y, -12f, time), QuestIcon));
			QuestIcon.TextureSource = "characters_bundle|inventory_character_" + CardQuest.Sponsor + "_normal";
		}
	}

	private void HotSpot_Click(GUIControl sender, GUIClickEvent EventData)
	{
		if (owningWindow != null)
		{
			owningWindow.SelectQuestItem(this);
		}
	}
}
