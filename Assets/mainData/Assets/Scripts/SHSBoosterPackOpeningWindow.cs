using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSBoosterPackOpeningWindow : GUIDynamicWindow
{
	public delegate void OnHideWindow();

	private const float debugSpeedModifier = 1.75f;

	private const float xDefaultPad = 5f;

	private const float xUncommonPad = 4f;

	private const float xUncommonStarOffset = 16f;

	private const float xRareStarOffset = 14f;

	private readonly GUIImage bgd;

	private readonly GUIImage largeCardImg;

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

	private List<SHSBoosterPackCardWindow> cardAnimationList;

	public readonly GUITBCloseButton closeButton;

	public readonly GUIImage boosterImg;

	public readonly GUIButton okButton;

	private OnHideWindow hideHandler;

	private int boosterPackId = -1;

	private bool isSuperRare;

	private CardZoomComponent zoomComponent;

	private static readonly Vector2 VIEW_CARD_SIZE = new Vector2(218f, 320f);

	private static readonly Vector2 ZOOM_CARD_SIZE = new Vector2(366f, 512f);

	private static readonly Vector2 VIEW_CARD_OFFSET = new Vector2(-252f, 0f);

	private static readonly Vector2 ZOOM_CARD_OFFSET = new Vector2(-326f, 0f);

	public SHSBoosterPackOpeningWindow(int boosterPackId, string boosterPackTextureName, OnHideWindow hideHandler)
	{
		this.boosterPackId = boosterPackId;
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
		title.TextureSource = "shopping_bundle|L_shopping_boosters_title";
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
		closeButton.IsEnabled = false;
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
		boosterImg = new GUIImage();
		boosterImg.SetPosition(QuickSizingHint.Centered);
		boosterImg.SetSize(255f, 309f);
		boosterImg.Offset = new Vector2(-254f, 43f);
		boosterImg.TextureSource = boosterPackTextureName;
		Add(boosterImg);
		largeCardImg = new GUIImage();
		largeCardImg.SetPosition(QuickSizingHint.Centered);
		largeCardImg.SetSize(218f, 320f);
		largeCardImg.Offset = new Vector2(-252f, 20f);
		largeCardImg.IsVisible = false;
		Add(largeCardImg, DrawOrder.DrawLast, DrawPhaseHintEnum.PostDraw);
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
		zoomComponent = new CardZoomComponent(base.AnimationPieceManager, largeCardImg, new CardZoomComponent.ZoomData(VIEW_CARD_SIZE, VIEW_CARD_OFFSET, ZOOM_CARD_SIZE, ZOOM_CARD_OFFSET, 0.5f, 0.5f));
		SetPosition(QuickSizingHint.Centered);
		SetSize(1022f, 644f);
	}

	public override void OnShow()
	{
		base.OnShow();
		closeButton.IsEnabled = false;
		okButton.IsEnabled = false;
		AppShell.Instance.EventMgr.AddListener<BoosterPackResponseMessage>(OnBoosterOpened);
		if (boosterPackId != -1)
		{
			BoosterPackService.OpenBoosterPack(boosterPackId, delegate(ShsWebResponse response)
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
		AppShell.Instance.EventMgr.RemoveListener<BoosterPackResponseMessage>(OnBoosterOpened);
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

	private void OnBoosterOpened(BoosterPackResponseMessage response)
	{
		CspUtils.DebugLog("OnBoosterOpened");
		DeckBuilderController instance = DeckBuilderController.Instance;
		if (instance != null)
		{
			instance.FetchCardCollection();
		}
		if (cardAnimationList != null)
		{
			cardAnimationList.ForEach(Remove);
			cardAnimationList.Clear();
		}
		string recipe = (string)response.payload["cards_awarded"];
		Dictionary<string, int> dictionary = CardManager.ParseRecipe(recipe);
		cardAnimationList = new List<SHSBoosterPackCardWindow>();
		int num = 0;
		float num2 = 0f;
		float num3 = 0.5f;
		foreach (string key in dictionary.Keys)
		{
			Texture2D faceSource = CardManager.LoadCardTexture(key);
			SHSBoosterPackCardWindow cardWindow;
			if (num <= 5)
			{
				cardWindow = new SHSBoosterPackCommonCardWindow(faceSource, "shopping_bundle|card_back");
				cardWindow.SetDisplayParameters(-84, 81, -69, 0.35f, 1f, 0.3f, 0.5f);
				num2 = 0.6125f * (float)num + num3;
				cardWindow.DisplayCard(num, num2, 1.75f);
			}
			else if (num <= 8)
			{
				cardWindow = new SHSBoosterPackUncommonCardWindow(faceSource, "shopping_bundle|card_back");
				cardWindow.SetDisplayParameters(-65, 117, 100, 0.58f, 1f, 0.25f, 0.5f);
				num2 += ((num != 6) ? (0.5f * (float)(num - 6)) : 1.5f);
				cardWindow.DisplayCard(num - 6, num2, 1.75f);
			}
			else
			{
				cardWindow = new SHSBoosterPackRareCardWindow(faceSource, "shopping_bundle|card_back", response.IncludesSuperRare);
				cardWindow.SetDisplayParameters(301, 102, 102, 0.6f, 0.75f, 0.2f, 0.5f);
				cardWindow.DisplayCard(0, num2 + 1.5f, 1.75f);
			}
			cardWindow.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, new Vector2(-254f, 40 - Math.Min(num, 4) * 2));
			cardWindow.SetSize(181f, 251f);
			cardWindow.Rotation = ((num < 4) ? (1 * (4 - num)) : 0);
			Add(cardWindow);
			cardAnimationList.Add(cardWindow);
			num++;
			SHSBoosterPackCardWindow sHSBoosterPackCardWindow = cardWindow;
			sHSBoosterPackCardWindow.OnAnimFinished = (SHSBoosterPackCardWindow.AnimFinished)Delegate.Combine(sHSBoosterPackCardWindow.OnAnimFinished, (SHSBoosterPackCardWindow.AnimFinished)delegate
			{
				cardWindow.MouseOver += cardWindow_MouseOver;
				cardWindow.MouseOut += cardWindow_MouseOut;
			});
		}
		isSuperRare = response.IncludesSuperRare;
		boosterImg.BringToFront();
	}

	private void cardWindow_MouseOut(GUIControl sender, GUIMouseEvent EventData)
	{
		zoomComponent.IsMouseOver = false;
		if (!zoomComponent.IsZooming)
		{
			boosterImg.IsVisible = true;
			largeCardImg.IsVisible = false;
		}
	}

	private void cardWindow_MouseOver(GUIControl sender, GUIMouseEvent EventData)
	{
		zoomComponent.IsMouseOver = true;
		largeCardImg.Texture = ((SHSBoosterPackCardWindow)sender).CardFace.Texture;
		boosterImg.IsVisible = false;
		largeCardImg.IsVisible = true;
	}

	public override void Update()
	{
		zoomComponent.Update();
		base.Update();
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
