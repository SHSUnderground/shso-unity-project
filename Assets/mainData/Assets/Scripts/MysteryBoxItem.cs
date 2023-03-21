using System;
using UnityEngine;

public class MysteryBoxItem : GUISubScalingWindow
{
	public delegate void AnimFinished();

	public MysteryBoxItemData itemData;

	private GUIImage background;

	private GUIImage item;

	public AnimFinished OnAnimFinished;

	private SHSMysteryBoxOpeningWindow boxWindow;

	public bool displaySummaryText;

	protected static readonly Vector2 SLIDE_ITEM_SIZE = new Vector2(260f, 336f);

	protected int xStartOffset = -84;

	protected int xIncrement = 81;

	protected int yStartOffset = -69;

	protected float targetSize = 0.35f;

	protected float animationSpeed = 0.75f;

	protected float flipTriggerTime = 0.3f;

	protected float flipDuration = 0.5f;

	public MysteryBoxItem(MysteryBoxItemData itemData, SHSMysteryBoxOpeningWindow boxWindow, bool enableSummary = false)
		: base(SLIDE_ITEM_SIZE)
	{
		this.boxWindow = boxWindow;
		this.itemData = itemData;
		displaySummaryText = enableSummary;
		if (itemData != null)
		{
			initFromData(itemData);
		}
	}

	protected void OnHeroXpLevelResponse(ShsWebResponse response)
	{
		if (response.Status == 200)
		{
			try
			{
				CspUtils.DebugLog("OnHeroXpLevelResponse " + response.Body);
				AppShell.Instance.Profile.AvailableCostumes.UpdateItemsFromData(response.Body);
			}
			catch (Exception arg)
			{
				CspUtils.DebugLog("Exception occurred while processing the HeroXPLevelUpResponse: <" + arg + ">.");
			}
		}
		else
		{
			CspUtils.DebugLog("Unable to retrieve updated Hero XP/Level info.  Proceeding with existing info.");
		}
	}

	public void initFromData(MysteryBoxItemData itemData)
	{
		this.itemData = itemData;
		Id = "SlideViewItem_" + itemData.name;
		SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, -70f));
		Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		Traits.VisibleAncestryTrait = ControlTraits.VisibilityAncestryTraitEnum.DetachedVisibility;
		if (itemData.displayInBox)
		{
			background = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(260f, 336f), new Vector2(0f, 0f));
			background.TextureSource = "shopping_bundle|shopping_slideview_ItemBox";
			AddItem(background);
		}
		else if (itemData is PotionMysteryBoxItemData)
		{
			background = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(260f, 336f), new Vector2(0f, 0f));
			background.TextureSource = "shopping_bundle|shopping_Goodies_box";
			AddItem(background);
		}
		OwnableDefinition def = OwnableDefinition.getDef(this.itemData.ownableTypeID);
		item = GUIControl.CreateControlFrameCentered<GUIImage>(itemData.itemPictureSize, Vector2.zero);
		item.TextureSource = itemData.itemTextureSource;
		AddItem(item);
		if (itemData is CardQuestMysteryBoxItemData)
		{
			CardQuestPart questPart = AppShell.Instance.CardQuestManager.GetQuestPart(itemData.ownableTypeID);
			if (questPart != null)
			{
				GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(200f, 218f), new Vector2(54f, 29f));
				gUIImage.TextureSource = "characters_bundle|" + questPart.Sponsor + "_HUD_default";
				AddItem(gUIImage);
			}
			else
			{
				CspUtils.DebugLog("Quest Part:" + itemData.ownableTypeID + " is not listed in the card quest collection.");
			}
		}
		else if (itemData is CardMysteryBoxItemData)
		{
			if ((itemData as CardMysteryBoxItemData).itemTexture != null)
			{
				GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(SLIDE_ITEM_SIZE, new Vector2(0f, 0f));
				gUIImage2.Texture = (itemData as CardMysteryBoxItemData).itemTexture;
				AddItem(gUIImage2);
			}
			else
			{
				CspUtils.DebugLog("Card:" + itemData.ownableTypeID + " did not have a proper texture.");
			}
		}
		else if (itemData is BadgeMysteryBoxItemData)
		{
			OwnableDefinition def2 = OwnableDefinition.getDef(itemData.ownableTypeID);
			if (def2 != null)
			{
				int id = int.Parse(def2.metadata);
				def2 = OwnableDefinition.getDef(id);
				if (def2 != null)
				{
					GUIImage gUIImage3 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(103f, 103f), new Vector2(0f, 0f));
					gUIImage3.TextureSource = "characters_bundle|token_" + def2.name + string.Empty;
					gUIImage3.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
					AddItem(gUIImage3);
					AppShell.Instance.WebService.StartRequest("resources$users/heroes.py", OnHeroXpLevelResponse);
				}
			}
			else
			{
				CspUtils.DebugLog("Badge:" + itemData.ownableTypeID + " not found in ownable list");
			}
		}
		else if (itemData is SidekickUpgradeMysteryBoxItemData)
		{
			OwnableDefinition def3 = OwnableDefinition.getDef(itemData.ownableTypeID);
			if (def3 != null)
			{
				int id2 = int.Parse(def3.metadata);
				def3 = OwnableDefinition.getDef(id2);
				if (def3 != null)
				{
					GUIImage gUIImage4 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(103f, 103f), new Vector2(0f, 0f));
					gUIImage4.TextureSource = def3.shoppingIcon;
					gUIImage4.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
					AddItem(gUIImage4);
				}
			}
			else
			{
				CspUtils.DebugLog("Sidekick Upgrade:" + itemData.ownableTypeID + " not found in ownable list");
			}
		}
		if (displaySummaryText && !(itemData is CardMysteryBoxItemData))
		{
			GUIImage gUIImage5 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(200f, 150f), new Vector2(0f, 157f));
			gUIImage5.TextureSource = "shopping_bundle|mysterybox_desc";
			gUIImage5.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(14f, 142f), new Vector2(1f, 0.28f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
			Add(gUIImage5);
			GUILabel gUILabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(220f, 120f), new Vector2(14f, 147f));
			gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 24, GUILabel.GenColor(8, 40, 180), TextAnchor.MiddleCenter);
			string empty = string.Empty;
			empty = (gUILabel.Text = ((!((itemData.name + "   ").Substring(0, 1) == "#")) ? (empty + itemData.name) : (empty + AppShell.Instance.stringTable.GetString(itemData.name))));
			Add(gUILabel);
		}
		if (itemData.quantity > 1)
		{
			GUIImage gUIImage6 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(102f, 90f), new Vector2(72f, 57f));
			gUIImage6.TextureSource = "shopping_bundle|shopping_item_quantity_container";
			AddItem(gUIImage6);
			GUILabel gUILabel2 = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(60f, 35f), new Vector2(75f, 62f));
			gUILabel2.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 33, GUILabel.GenColor(105, 152, 39), TextAnchor.MiddleCenter);
			gUILabel2.Text = itemData.quantity.ToString();
			gUILabel2.Rotation = -5f;
			AddLabel(gUILabel2);
		}
		GUIHotSpotButton gUIHotSpotButton = GUIControl.CreateControlBottomFrame<GUIHotSpotButton>(new Vector2(234f, 314f), new Vector2(0f, 0f));
		gUIHotSpotButton.MouseOver += OnMouseOver;
		gUIHotSpotButton.MouseOut += OnMouseOut;
		AddItem(gUIHotSpotButton);
	}

	private void OnMouseOver(GUIControl sender, GUIMouseEvent EventData)
	{
		boxWindow.rollOverItem(this);
	}

	private void OnMouseOut(GUIControl sender, GUIMouseEvent EventData)
	{
		boxWindow.rollOffItem();
	}

	public void SetDisplayParameters(int xstartOffset, int xincrement, int ystartoffset, float targetsize, float animationspeed, float fliptriggertime, float flipduration)
	{
		xStartOffset = xstartOffset;
		xIncrement = xincrement;
		yStartOffset = ystartoffset;
		targetSize = targetsize;
		animationSpeed = animationspeed;
		flipTriggerTime = fliptriggertime;
		flipDuration = flipduration;
	}

	public void DisplayCard(int cardIndex, float initialDelay, float modifier, int soundToPlay)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		AnimClip animClip = SHSAnimations.Generic.Wait(initialDelay);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			GUIControl gUIControl = this;
			gUIControl.BringToFront();
			((SHSMysteryBoxOpeningWindow)parent).chestImage.BringToFront();
			float time = animationSpeed * modifier;
			Vector2[] obj = new Vector2[4]
			{
				gUIControl.Offset,
				default(Vector2),
				default(Vector2),
				default(Vector2)
			};
			//ref Vector2 reference = ref obj[1];
			Vector2 offset = gUIControl.Offset;
			float x = offset.x;
			Vector2 offset2 = gUIControl.Offset;
			obj[1] = new Vector2(x, offset2.y - 340f);
			//ref Vector2 reference2 = ref obj[2];
			float x2 = xStartOffset + xIncrement * cardIndex;
			Vector2 offset3 = gUIControl.Offset;
			obj[2] = new Vector2(x2, offset3.y - 200f);
			obj[3] = new Vector2(xStartOffset + xIncrement * cardIndex, yStartOffset);
			AnimClip pieceOne = SHSAnimations.Spline.BSplineCurveOffsetXY(time, gUIControl, obj);
			Vector2 size = gUIControl.Size;
			float x3 = size.x;
			Vector2 size2 = gUIControl.Size;
			AnimClip pieceOne2 = pieceOne ^ AnimClipBuilder.Absolute.SizeX(SHSAnimations.GenericPaths.LinearWithWiggle(x3, size2.x * targetSize, animationSpeed * modifier, 10f, 0.25f), gUIControl);
			Vector2 size3 = gUIControl.Size;
			float y = size3.y;
			Vector2 size4 = gUIControl.Size;
			AnimClip animClip2 = pieceOne2 ^ AnimClipBuilder.Absolute.SizeY(SHSAnimations.GenericPaths.LinearWithWiggle(y, size4.y * targetSize, animationSpeed * modifier, 10f, 0.25f), gUIControl) ^ AnimClipBuilder.Delta.Rotation(AnimClipBuilder.Path.Linear(gUIControl.Rotation, 0f, animationSpeed * modifier), gUIControl);
			if (soundToPlay == 1)
			{
				animClip2 = (AnimClipBuilder.Custom.Function(AnimPath.Constant(0f, 0f), delegate
				{
					ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("boosterpack_open"));
				}) ^ animClip2);
			}
			else if (soundToPlay == 20)
			{
				ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("boosterpack_rare"));
			}
			else if (soundToPlay == 10)
			{
				ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("boosterpack_superrare"));
			}
			base.AnimationPieceManager.Add(animClip2);
			if (OnAnimFinished != null)
			{
				OnAnimFinished();
			}
		};
		base.AnimationPieceManager.Add(animClip);
	}
}
