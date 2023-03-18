using System;
using System.Collections.Generic;
using UnityEngine;

public class GUIDeckStatBar : GUISimpleControlWindow
{
	protected const float MIN_TEXT_DISPLAY_WIDTH = 15f;

	protected float maxWidth;

	protected Dictionary<BattleCard.Factor, GUIImage> factorImages;

	protected Dictionary<BattleCard.Factor, GUILabel> factorLabels;

	protected Dictionary<BattleCard.Factor, Color> factorColors;

	protected GUIImage blankSection;

	public int DeckCount = 40;

	public Dictionary<BattleCard.Factor, int> CardCounts
	{
		set
		{
			int deckCount = DeckCount;
			foreach (GUILabel value2 in factorLabels.Values)
			{
				value2.IsVisible = false;
			}
			if (deckCount > 0)
			{
				float num = 0f;
				foreach (BattleCard.Factor key in factorImages.Keys)
				{
					GUIImage gUIImage = factorImages[key];
					if (value.ContainsKey(key) && value[key] > 0)
					{
						float num2 = (float)value[key] / (float)deckCount;
						Vector2 size = Size;
						float num3 = Mathf.Ceil(num2 * size.x);
						gUIImage.SetPositionAndSize(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(num, 0f), new Vector2(num3, base.size.y), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
						gUIImage.IsVisible = true;
						if (factorLabels.ContainsKey(key))
						{
							factorLabels[key].Text = value[key].ToString();
							factorLabels[key].SetPositionAndSize(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(num + num3 / 2f - 10f, -5f), new Vector2(20f, 10f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
						}
						num += num3;
					}
					else
					{
						gUIImage.IsVisible = false;
					}
				}
				if (blankSection != null)
				{
					blankSection.SetPositionAndSize(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(num, 0f), new Vector2(base.size.x - num, base.size.y), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				}
			}
			else if (blankSection != null)
			{
				blankSection.IsVisible = true;
				blankSection.SetPositionAndSize(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(base.size.x, base.size.y), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			}
		}
	}

	public GUIDeckStatBar()
	{
		factorImages = new Dictionary<BattleCard.Factor, GUIImage>();
		factorLabels = new Dictionary<BattleCard.Factor, GUILabel>();
		factorColors = new Dictionary<BattleCard.Factor, Color>();
		Color value = GUILabel.GenColor(0, 0, 0);
		value.a = 0.5f;
		Color value2 = GUILabel.GenColor(255, 255, 255);
		value2.a = 0.6f;
		factorColors[BattleCard.Factor.Strength] = value;
		factorColors[BattleCard.Factor.Speed] = value;
		factorColors[BattleCard.Factor.Elemental] = value2;
		factorColors[BattleCard.Factor.Tech] = value;
		factorColors[BattleCard.Factor.Animal] = value;
		factorColors[BattleCard.Factor.Energy] = value;
		HitTestType = HitTestTypeEnum.Rect;
		MouseOver += OnMouseOver;
		MouseOut += OnMouseOut;
	}

	public override bool InitializeResources(bool reload)
	{
		BattleCard.Factor[] array = (BattleCard.Factor[])Enum.GetValues(typeof(BattleCard.Factor));
		BattleCard.Factor[] array2 = array;
		foreach (BattleCard.Factor factor in array2)
		{
			if (factor != 0 && factor != BattleCard.Factor.Universal)
			{
				GUIImage gUIImage = new GUIImage();
				gUIImage.SetPositionAndSize(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(18f, 4f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				gUIImage.TextureSource = "deckbuilder_bundle|deckbuilder_stat_bar_fill_" + BattleCard.FactorToChar(factor);
				gUIImage.IsVisible = false;
				gUIImage.Id = "FactorSection_" + factor.ToString();
				factorImages[factor] = gUIImage;
				gUIImage.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
				Add(gUIImage);
				GUILabel gUILabel = new GUILabel();
				gUILabel.SetPositionAndSize(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(20f, 5f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
				gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 13, factorColors[factor], TextAnchor.MiddleCenter);
				gUILabel.Bold = true;
				gUILabel.IsVisible = false;
				factorLabels[factor] = gUILabel;
				Add(gUILabel);
			}
		}
		blankSection = new GUIImage();
		blankSection.TextureSource = "deckbuilder_bundle|deckbuilder_stat_bar_fill_blank";
		blankSection.SetPositionAndSize(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(size.x, size.y), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		blankSection.Id = "Blank_Section";
		blankSection.IsVisible = true;
		Add(blankSection);
		return true;
	}

	protected void OnMouseOut(GUIControl sender, GUIMouseEvent EventData)
	{
		if (factorLabels != null)
		{
			foreach (GUILabel value in factorLabels.Values)
			{
				value.IsVisible = false;
			}
		}
	}

	protected void OnMouseOver(GUIControl sender, GUIMouseEvent EventData)
	{
		if (factorImages != null && factorLabels != null)
		{
			foreach (KeyValuePair<BattleCard.Factor, GUIImage> factorImage in factorImages)
			{
				if (factorLabels.ContainsKey(factorImage.Key) && factorImages.ContainsKey(factorImage.Key))
				{
					Vector2 size = factorImages[factorImage.Key].Size;
					if (size.x > 15f)
					{
						factorLabels[factorImage.Key].IsVisible = factorImage.Value.IsVisible;
					}
				}
			}
		}
	}
}
