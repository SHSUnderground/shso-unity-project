using UnityEngine;

public class SHSNewsMainWindow : SHSGadget.GadgetCenterWindow
{
	private GUIImage news1;

	private GUIImage news2;

	private GUIImage news3;

	private GUIImage news4;

	private GUIButton BuyHeroButton;

	private GUIButton BuyMissionButton;

	private GUIImage buyHeroGlow;

	private GUIImage buyMissionGlow;

	private SHSNewsGadget newsGadget;

	private AnimClip heroGlowFadeClip;

	private AnimClip missionGlowFadeClip;

	private SHSNewsRewardTableWindow rewardTable;

	public SHSNewsMainWindow(SHSNewsGadget gadget)
	{
		news1 = GUIControl.CreateControlBottomFrameCentered<GUIImage>(new Vector2(396f, 323f), new Vector2(-195f, -359f));
		news1.TextureSource = FeatureImageLoader.FeatureImageInfoDictionary[FeatureImageLoader.FeatureImageEnum.Hero].TexturePath;
		Add(news1);
		news2 = GUIControl.CreateControlBottomFrameCentered<GUIImage>(new Vector2(396f, 163f), new Vector2(197f, -436f));
		news2.TextureSource = FeatureImageLoader.FeatureImageInfoDictionary[FeatureImageLoader.FeatureImageEnum.Upsell].TexturePath;
		Add(news2);
		news3 = GUIControl.CreateControlBottomFrameCentered<GUIImage>(new Vector2(396f, 163f), new Vector2(-193f, -141f));
		news3.TextureSource = FeatureImageLoader.FeatureImageInfoDictionary[FeatureImageLoader.FeatureImageEnum.Mission].TexturePath;
		Add(news3);
		if (FeatureImageLoader.FeatureImageInfoDictionary.ContainsKey(FeatureImageLoader.FeatureImageEnum.Game))
		{
			news4 = GUIControl.CreateControlBottomFrameCentered<GUIImage>(new Vector2(396f, 323f), new Vector2(201f, -223f));
			news4.TextureSource = FeatureImageLoader.FeatureImageInfoDictionary[FeatureImageLoader.FeatureImageEnum.Game].TexturePath;
			Add(news4);
		}
		else
		{
			rewardTable = GUIControl.CreateControlBottomFrameCentered<SHSNewsRewardTableWindow>(SHSNewsRewardTableWindow.WindowSize, new Vector2(195f, -226f));
			Add(rewardTable);
		}
		GUIButton gUIButton = new GUIButton();
		gUIButton.StyleInfo = new SHSButtonStyleInfo("gameworld_bundle|L_mshs_newspaper_button_play");
		gUIButton.SetPositionAndSize(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, -50f), new Vector2(256f, 256f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIButton.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton.Click += delegate
		{
			gadget.CloseGadget();
		};
		Add(gUIButton);
		BuyHeroButton = GUIControl.CreateControlBottomFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(-255f, -272f));
		BuyHeroButton.StyleInfo = new SHSButtonStyleInfo("gameworld_bundle|L_mshs_newspaper_hero_buy_button");
		BuyHeroButton.HitTestType = HitTestTypeEnum.Alpha;
		BuyHeroButton.Click += BuyHeroButton_Click;
		BuyHeroButton.MouseOver += BuyHeroButton_MouseOver;
		BuyHeroButton.MouseOut += BuyHeroButton_MouseOut;
		Add(BuyHeroButton);
		BuyHeroButton.IsVisible = false;
		buyHeroGlow = GUIControl.CreateControlBottomFrameCentered<GUIImage>(new Vector2(435f, 361f), new Vector2(-195f, -360f));
		buyHeroGlow.TextureSource = "gameworld_bundle|mshs_newspaper_hero_buy_button_glow";
		Add(buyHeroGlow, news1);
		buyHeroGlow.Alpha = 0f;
		BuyMissionButton = GUIControl.CreateControlBottomFrameCentered<GUIButton>(new Vector2(396f, 163f), new Vector2(-196f, -146f));
		BuyMissionButton.StyleInfo = new SHSButtonStyleInfo("gameworld_bundle|mshs_newspaper_mission_buy_button");
		BuyMissionButton.HitTestType = HitTestTypeEnum.Alpha;
		BuyMissionButton.Click += BuyMissionButton_Click;
		BuyMissionButton.MouseOver += BuyMissionButton_MouseOver;
		BuyMissionButton.MouseOut += BuyMissionButton_MouseOut;
		Add(BuyMissionButton, news3);
		BuyMissionButton.IsVisible = false;
		buyMissionGlow = GUIControl.CreateControlBottomFrameCentered<GUIImage>(new Vector2(433f, 207f), new Vector2(-195f, -151f));
		buyMissionGlow.TextureSource = "gameworld_bundle|mshs_newspaper_mission_buy_button_glow";
		Add(buyMissionGlow, BuyMissionButton);
		buyMissionGlow.Alpha = 0f;
		newsGadget = gadget;
		FeatureImageLoader.FeatureImageInfoDictionary[FeatureImageLoader.FeatureImageEnum.Hero].SetBuyButton(BuyHeroButton);
		FeatureImageLoader.FeatureImageInfoDictionary[FeatureImageLoader.FeatureImageEnum.Mission].SetBuyButton(BuyMissionButton);
	}

	public void OnOpenAnimationComplete()
	{
		if (rewardTable != null)
		{
			rewardTable.StartRewardDayAnimations();
		}
	}

	private void BuyMissionButton_MouseOut(GUIControl sender, GUIMouseEvent EventData)
	{
		base.AnimationPieceManager.SwapOut(ref missionGlowFadeClip, SHSAnimations.Generic.FadeOut(buyMissionGlow, 0.2f));
	}

	private void BuyMissionButton_MouseOver(GUIControl sender, GUIMouseEvent EventData)
	{
		base.AnimationPieceManager.SwapOut(ref missionGlowFadeClip, SHSAnimations.Generic.FadeIn(buyMissionGlow, 0.2f));
	}

	private void BuyHeroButton_MouseOut(GUIControl sender, GUIMouseEvent EventData)
	{
		base.AnimationPieceManager.SwapOut(ref heroGlowFadeClip, SHSAnimations.Generic.FadeOut(buyHeroGlow, 0.2f));
	}

	private void BuyHeroButton_MouseOver(GUIControl sender, GUIMouseEvent EventData)
	{
		base.AnimationPieceManager.SwapOut(ref heroGlowFadeClip, SHSAnimations.Generic.FadeIn(buyHeroGlow, 0.2f));
	}

	private void BuyHeroButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		ExitToShop(NewShoppingManager.ShoppingCategory.Hero);
	}

	private void BuyMissionButton_Click(GUIControl sender, GUIClickEvent EventData)
	{
		ExitToShop(NewShoppingManager.ShoppingCategory.Mission);
	}

	private void ExitToShop(NewShoppingManager.ShoppingCategory category)
	{
		newsGadget.CloseGadget();
		ShoppingWindow shoppingWindow = new ShoppingWindow(category);
		shoppingWindow.launch();
	}
}
