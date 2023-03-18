using LitJson;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSMysteryBoxOpeningWindow : GUIDynamicWindow
{
	public delegate void OnHideWindow();

	private const float debugSpeedModifier = 1.75f;

	private const float xDefaultPad = 5f;

	private const float xUncommonPad = 4f;

	private const float xUncommonStarOffset = 16f;

	private const float xRareStarOffset = 14f;

	private readonly GUIImage bgd;

	private MysteryBoxItem largeCardImg;

	private readonly GUIStrokeTextLabel commonLabel;

	private readonly GUIImage commonStar;

	private readonly GUIImage title;

	private readonly GUIStrokeTextLabel uncommonLabel;

	private readonly GUIImage uncommonStar1;

	private readonly GUIImage uncommonStar2;

	private readonly GUIStrokeTextLabel rareLabel;

	private readonly GUIImage rareStar1;

	private readonly GUIImage rareStar2;

	private readonly GUIImage rareStar3;

	private readonly GUIImage rareStar4;

	private readonly GUIImage superRareBackground;

	public readonly GUITBCloseButton closeButton;

	public readonly GUIImage chestImage;

	public readonly GUIButton okButton;

	private OnHideWindow hideHandler;

	private int mysteryBoxID = -1;

	private bool isSuperRare;

	private bool alreadyOpened;

	public SHSMysteryBoxOpeningWindow(int mysteryBoxID, string boxIconBase, OnHideWindow hideHandler)
	{
		this.mysteryBoxID = mysteryBoxID;
		this.hideHandler = hideHandler;
		bgd = new GUIImage();
		bgd.SetPosition(QuickSizingHint.Centered);
		bgd.SetSize(846f, 534f);
		bgd.TextureSource = "shopping_bundle|shopping_boosters_window";
		Add(bgd);
		title = new GUIImage();
		title.SetPosition(QuickSizingHint.Centered);
		title.SetSize(197f, 40f);
		title.Offset = new Vector2(0f, -201f);
		title.TextureSource = "shopping_bundle|L_mystery_box_title";
		Add(title);
		commonLabel = new GUIStrokeTextLabel();
		commonLabel.SetPosition(QuickSizingHint.Centered);
		commonLabel.SetSize(120f, 24f);
		commonLabel.Offset = new Vector2(-55f, -131f);
		commonLabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 20, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 114, 255), GUILabel.GenColor(22, 77, 195), new Vector2(3f, 2f), TextAnchor.MiddleLeft);
		commonLabel.Text = "#CGBPO_COMMON";
		Add(commonLabel);
		uncommonLabel = new GUIStrokeTextLabel();
		uncommonLabel.SetPosition(QuickSizingHint.Centered);
		uncommonLabel.Offset = new Vector2(-52f, 14f);
		uncommonLabel.SetSize(120f, 24f);
		uncommonLabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 20, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 114, 255), GUILabel.GenColor(22, 77, 195), new Vector2(3f, 2f), TextAnchor.MiddleLeft);
		uncommonLabel.Text = "#CGBPO_UNCOMMON";
		Add(uncommonLabel);
		superRareBackground = new GUIImage();
		superRareBackground.SetPositionAndSize(DockingAlignmentEnum.BottomRight, AnchorAlignmentEnum.BottomRight, OffsetType.Absolute, new Vector2(-138f, -124f), new Vector2(145f, 213f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		superRareBackground.TextureSource = "shopping_bundle|shopping_booster_super_frame";
		superRareBackground.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		Add(superRareBackground);
		superRareBackground.IsVisible = false;
		rareLabel = new GUIStrokeTextLabel();
		rareLabel.SetPosition(QuickSizingHint.Centered);
		rareLabel.SetSize(120f, 24f);
		rareLabel.Offset = new Vector2(312f, 14f);
		rareLabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 20, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(0, 114, 255), GUILabel.GenColor(22, 77, 195), new Vector2(3f, 2f), TextAnchor.MiddleLeft);
		rareLabel.Text = "#CGBPO_RARE";
		Add(rareLabel);
		closeButton = new GUITBCloseButton();
		closeButton.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.TopRight, new Vector2(64f, 64f));
		closeButton.Offset = new Vector2(418f, -239f);
		closeButton.Click += delegate
		{
			Hide();
		};
		closeButton.IsEnabled = true;
		Add(closeButton);
		okButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(128f, 128f), new Vector2(0f, 238f));
		okButton.Click += delegate
		{
			Hide();
		};
		okButton.HitTestSize = new Vector2(0.5f, 0.5f);
		okButton.HitTestType = HitTestTypeEnum.Circular;
		okButton.StyleInfo = new SHSButtonStyleInfo("common_bundle|L_mshs_button_ok");
		Add(okButton);
		chestImage = new GUIImage();
		chestImage.SetPosition(QuickSizingHint.Centered);
		chestImage.SetSize(260f, 336f);
		chestImage.Offset = new Vector2(-254f, 23f);
		chestImage.TextureSource = "shopping_bundle|" + boxIconBase;
		Add(chestImage);
		commonStar = new GUIImage();
		commonStar.SetPosition(QuickSizingHint.Centered);
		commonStar.SetSize(16f, 17f);
		commonStar.Offset = new Vector2(-34f, -134f);
		commonStar.TextureSource = "shopping_bundle|shopping_booster_rarity_star";
		commonStar.Id = "commonStar";
		Add(commonStar);
		uncommonStar1 = new GUIImage();
		uncommonStar1.SetPosition(QuickSizingHint.Centered);
		uncommonStar1.SetSize(16f, 17f);
		uncommonStar1.Offset = new Vector2(-10f, 10f);
		uncommonStar1.Id = "uncommonStar1";
		uncommonStar1.TextureSource = "shopping_bundle|shopping_booster_rarity_star";
		Add(uncommonStar1);
		uncommonStar2 = new GUIImage();
		uncommonStar2.SetPosition(QuickSizingHint.Centered);
		uncommonStar2.SetSize(16f, 17f);
		uncommonStar2.Offset = new Vector2(6f, 10f);
		uncommonStar2.Id = "uncommonStar2";
		uncommonStar2.TextureSource = "shopping_bundle|shopping_booster_rarity_star";
		Add(uncommonStar2);
		rareStar1 = new GUIImage();
		rareStar1.SetPosition(QuickSizingHint.Centered);
		rareStar1.SetSize(16f, 17f);
		rareStar1.Offset = new Vector2(309f, 10f);
		rareStar1.Id = "rareStar1";
		rareStar1.TextureSource = "shopping_bundle|shopping_booster_rarity_star";
		Add(rareStar1);
		rareStar2 = new GUIImage();
		rareStar2.SetPosition(QuickSizingHint.Centered);
		rareStar2.SetSize(16f, 17f);
		rareStar2.Offset = new Vector2(323f, 10f);
		rareStar2.Id = "rareStar2";
		rareStar2.TextureSource = "shopping_bundle|shopping_booster_rarity_star";
		Add(rareStar2);
		rareStar3 = new GUIImage();
		rareStar3.SetPosition(QuickSizingHint.Centered);
		rareStar3.SetSize(16f, 17f);
		rareStar3.Offset = new Vector2(337f, 10f);
		rareStar3.Id = "rareStar3";
		rareStar3.TextureSource = "shopping_bundle|shopping_booster_rarity_star";
		Add(rareStar3);
		rareStar4 = new GUIImage();
		rareStar4.SetPosition(QuickSizingHint.Centered);
		rareStar4.SetSize(16f, 17f);
		rareStar4.Offset = new Vector2(351f, 10f);
		rareStar4.Id = "rareStar4";
		rareStar4.TextureSource = "shopping_bundle|shopping_booster_rarity_star";
		Add(rareStar4);
		rareStar4.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		rareStar4.IsVisible = false;
		SetPosition(QuickSizingHint.Centered);
		SetSize(1022f, 644f);
	}

	public override void OnShow()
	{
		base.OnShow();
		closeButton.IsEnabled = true;
		okButton.IsEnabled = true;
		AppShell.Instance.EventMgr.AddListener<MysteryBoxResponseMessage>(OnBoxOpened);
		if (mysteryBoxID != -1)
		{
			MysteryBoxService.OpenMysteryBox(mysteryBoxID, delegate(ShsWebResponse response)
			{
				if (response.Status != 200)
				{
					Hide();
				}
			});
		}
		EnsureStarPositions();
	}

	public override void OnHide()
	{
		base.OnHide();
		AppShell.Instance.EventMgr.RemoveListener<MysteryBoxResponseMessage>(OnBoxOpened);
		if (hideHandler != null)
		{
			hideHandler();
		}
	}

	public void OnDoneOpening()
	{
		okButton.IsEnabled = true;
		closeButton.IsEnabled = true;
		if (isSuperRare)
		{
			superRareBackground.IsVisible = true;
			GUIStrokeTextLabel gUIStrokeTextLabel = rareLabel;
			Vector2 offset = rareLabel.Offset;
			gUIStrokeTextLabel.Offset = new Vector2(306f, offset.y);
			rareLabel.Text = "#CGBPO_SUPERRARE";
			rareStar4.IsVisible = true;
		}
	}

	private void OnBoxOpened(MysteryBoxResponseMessage response)
	{
		CspUtils.DebugLog("OnBoxOpened ");
		if (alreadyOpened)
		{
			return;
		}
		alreadyOpened = true;
		string text = response.payload["rewards"].ToString();
		string[] array = text.Split('|');
		int num = int.Parse(response.payload["silver_awarded"].ToString());
		int num2 = int.Parse(response.payload["gold_awarded"].ToString());
		CspUtils.DebugLog("   got " + num + " silver " + num2 + " gold and ownables " + text);
		float num3 = 0f;
		float num4 = 0.5f;
		int num5 = 0;
		List<MysteryBoxItemData> list = new List<MysteryBoxItemData>();
		Dictionary<OwnableDefinition.Category, bool> dictionary = new Dictionary<OwnableDefinition.Category, bool>();
		string[] array2 = array;
		foreach (string text2 in array2)
		{
			CspUtils.DebugLog("raw data is " + text2);
			MysteryBoxItemDataRaw mysteryBoxItemDataRaw = JsonMapper.ToObject<MysteryBoxItemDataRaw>(text2);
			CspUtils.DebugLog("   " + mysteryBoxItemDataRaw.ownableTypeID + " " + mysteryBoxItemDataRaw.category);
			MysteryBoxItemData mysteryBoxItemData = MysteryBoxItemData.factory(mysteryBoxItemDataRaw.ownableTypeID, mysteryBoxItemDataRaw.quantity, mysteryBoxItemDataRaw.category);
			mysteryBoxItemData.rarity = mysteryBoxItemDataRaw.rarity;
			OwnableDefinition def = OwnableDefinition.getDef(mysteryBoxItemDataRaw.ownableTypeID);
			if (def != null)
			{
				dictionary[def.category] = true;
			}
			list.Add(mysteryBoxItemData);
		}
		foreach (OwnableDefinition.Category key in dictionary.Keys)
		{
			AppShell.Instance.Profile.FetchDataBasedOnCategory(key);
		}
		list.Sort(new MysteryBoxItemDataSort());
		for (num5 = 0; num5 < list.Count; num5++)
		{
			MysteryBoxItemData mysteryBoxItemData = list[num5];
			MysteryBoxItem mysteryBoxItem = new MysteryBoxItem(mysteryBoxItemData, this);
			mysteryBoxItem.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			mysteryBoxItem.IsVisible = true;
			if (num5 <= 5)
			{
				mysteryBoxItem.SetDisplayParameters(-84, 81, -69, 0.4f, 1f, 0.3f, 0.5f);
				num3 = 0.6125f * (float)num5 + num4;
				mysteryBoxItem.DisplayCard(num5, num3, 1.75f, (num5 == 0) ? 1 : 0);
			}
			else if (num5 <= 8)
			{
				mysteryBoxItem.SetDisplayParameters(-65, 117, 100, 0.6f, 1f, 0.25f, 0.5f);
				num3 += ((num5 != 6) ? (0.5f * (float)(num5 - 6)) : 1.5f);
				mysteryBoxItem.DisplayCard(num5 - 6, num3, 1.75f, 0);
			}
			else
			{
				mysteryBoxItem.SetDisplayParameters(301, 102, 102, 0.65f, 0.75f, 0.2f, 0.5f);
				mysteryBoxItem.DisplayCard(0, num3 + 1.5f, 1.75f, mysteryBoxItemData.rarity);
			}
			mysteryBoxItem.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, new Vector2(-254f, 40 - Math.Min(num5, 4) * 2));
			mysteryBoxItem.SetSize(181f, 251f);
			mysteryBoxItem.Rotation = ((num5 < 4) ? (1 * (4 - num5)) : 0);
			Add(mysteryBoxItem);
		}
		chestImage.BringToFront();
	}

	public void rollOffItem()
	{
		chestImage.IsVisible = true;
		if (largeCardImg != null)
		{
			largeCardImg.IsVisible = false;
		}
	}

	public void rollOverItem(MysteryBoxItem sender)
	{
		if (largeCardImg != null)
		{
			Remove(largeCardImg);
			largeCardImg = null;
		}
		largeCardImg = new MysteryBoxItem(sender.itemData, this, true);
		largeCardImg.SetPosition(QuickSizingHint.Centered);
		largeCardImg.SetSize(286f, 370f);
		largeCardImg.Offset = new Vector2(-254f, 23f);
		Add(largeCardImg);
		largeCardImg.IsVisible = true;
		chestImage.IsVisible = false;
	}

	private void EnsureStarPositions()
	{
		float num = 0f;
		float num2 = 0f;
		commonLabel.CalculateTextLayout();
		uncommonLabel.CalculateTextLayout();
		rareLabel.CalculateTextLayout();
		Vector2 position = commonLabel.Position;
		num = position.x + (float)commonLabel.LongestLine + 5f;
		float num3 = num;
		Vector2 position2 = commonStar.Position;
		num2 = num3 - position2.x;
		if (num2 > 0f)
		{
			GUIImage gUIImage = commonStar;
			Vector2 offset = commonStar.Offset;
			float x = offset.x + num2;
			Vector2 offset2 = commonStar.Offset;
			gUIImage.Offset = new Vector2(x, offset2.y);
		}
		Vector2 position3 = uncommonLabel.Position;
		num = position3.x + (float)uncommonLabel.LongestLine + 4f;
		float num4 = num;
		Vector2 position4 = uncommonStar1.Position;
		num2 = num4 - position4.x;
		if (num2 > 0f)
		{
			GUIImage gUIImage2 = uncommonStar1;
			Vector2 offset3 = uncommonStar1.Offset;
			float x2 = offset3.x + num2;
			Vector2 offset4 = uncommonStar1.Offset;
			gUIImage2.Offset = new Vector2(x2, offset4.y);
			GUIImage gUIImage3 = uncommonStar2;
			Vector2 offset5 = uncommonStar1.Offset;
			float x3 = offset5.x + 16f;
			Vector2 offset6 = uncommonStar2.Offset;
			gUIImage3.Offset = new Vector2(x3, offset6.y);
		}
		Vector2 position5 = rareLabel.Position;
		num = position5.x + (float)rareLabel.LongestLine + 5f;
		float num5 = num;
		Vector2 position6 = rareStar1.Position;
		num2 = num5 - position6.x;
		if (num2 > 0f)
		{
			GUIImage gUIImage4 = rareStar1;
			Vector2 offset7 = rareStar1.Offset;
			float x4 = offset7.x + num2;
			Vector2 offset8 = rareStar1.Offset;
			gUIImage4.Offset = new Vector2(x4, offset8.y);
			GUIImage gUIImage5 = rareStar2;
			Vector2 offset9 = rareStar1.Offset;
			float x5 = offset9.x + 14f;
			Vector2 offset10 = rareStar2.Offset;
			gUIImage5.Offset = new Vector2(x5, offset10.y);
			GUIImage gUIImage6 = rareStar3;
			Vector2 offset11 = rareStar2.Offset;
			float x6 = offset11.x + 14f;
			Vector2 offset12 = rareStar3.Offset;
			gUIImage6.Offset = new Vector2(x6, offset12.y);
			GUIImage gUIImage7 = rareStar4;
			Vector2 offset13 = rareStar3.Offset;
			float x7 = offset13.x + 14f;
			Vector2 offset14 = rareStar4.Offset;
			gUIImage7.Offset = new Vector2(x7, offset14.y);
		}
	}
}
