using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSCardGameAirlockWindow : GUIDynamicWindow
{
	private List<GUIImage> cardsToAnimate;

	private int animCardIndex;

	public Matchmaker2.CardGameInvitation invite;

	public SHSCardGameAirlockWindow()
	{
		Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		SetPositionAndSize(QuickSizingHint.ScreenSize);
		cardsToAnimate = new List<GUIImage>();
		GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(Vector2.zero, Vector2.zero);
		gUIImage.SetPositionAndSize(QuickSizingHint.ScreenSize);
		gUIImage.TextureSource = "persistent_bundle|loading_bg_bluecircles";
		Add(gUIImage);
		GUIImage gUIImage2 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(922f, 634f), new Vector2(0f, 4f));
		gUIImage2.TextureSource = "persistent_bundle|gadget_mainwindow_frame";
		Add(gUIImage2);
		GUIImage gUIImage3 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(903f, 556f), new Vector2(-39f, 31f));
		gUIImage3.TextureSource = "cardgamegadget_bundle|card_game_gadget_airlock_centerimage";
		Add(gUIImage3);
		GUISimpleControlWindow gUISimpleControlWindow = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(new Vector2(583f, 147f), new Vector2(0f, -258f));
		Add(gUISimpleControlWindow);
		GUIImage gUIImage4 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(583f, 140f), new Vector2(0f, 7f));
		gUIImage4.TextureSource = "persistent_bundle|gadget_topmodule";
		gUISimpleControlWindow.Add(gUIImage4);
		GUIImage gUIImage5 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(293f, 43f), new Vector2(0f, -3f));
		gUIImage5.TextureSource = "cardgamegadget_bundle|L_card_game_gadget_airlock_title";
		gUISimpleControlWindow.Add(gUIImage5);
		GUILabel gUILabel = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(250f, 100f), new Vector2(170f, 113f));
		gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 26, GUILabel.GenColor(52, 75, 87), TextAnchor.MiddleLeft);
		gUILabel.Text = "#CARDGAME_AIRLOCK_WAITING_TEXT";
		Add(gUILabel);
		GUIImage gUIImage6 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(145f, 186f), new Vector2(-397f, 100f));
		gUIImage6.TextureSource = "cardgamegadget_bundle|card_game_gadget_airlock_card1";
		cardsToAnimate.Add(gUIImage6);
		GUIImage gUIImage7 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(147f, 142f), new Vector2(-405f, 3f));
		gUIImage7.TextureSource = "cardgamegadget_bundle|card_game_gadget_airlock_card2";
		cardsToAnimate.Add(gUIImage7);
		GUIImage gUIImage8 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(153f, 154f), new Vector2(-343f, -84f));
		gUIImage8.TextureSource = "cardgamegadget_bundle|card_game_gadget_airlock_card3";
		cardsToAnimate.Add(gUIImage8);
		GUIImage gUIImage9 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(173f, 199f), new Vector2(-261f, -158f));
		gUIImage9.TextureSource = "cardgamegadget_bundle|card_game_gadget_airlock_card4";
		cardsToAnimate.Add(gUIImage9);
		GUIImage gUIImage10 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(151f, 146f), new Vector2(-95f, -142f));
		gUIImage10.TextureSource = "cardgamegadget_bundle|card_game_gadget_airlock_card5";
		cardsToAnimate.Add(gUIImage10);
		GUIImage gUIImage11 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(204f, 217f), new Vector2(11f, -73f));
		gUIImage11.TextureSource = "cardgamegadget_bundle|card_game_gadget_airlock_card6";
		cardsToAnimate.Add(gUIImage11);
		GUIImage gUIImage12 = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(148f, 217f), new Vector2(73f, 51f));
		gUIImage12.TextureSource = "cardgamegadget_bundle|card_game_gadget_airlock_card7";
		cardsToAnimate.Add(gUIImage12);
		Add(gUIImage9);
		Add(gUIImage6);
		Add(gUIImage7);
		Add(gUIImage8);
		Add(gUIImage12);
		Add(gUIImage11);
		Add(gUIImage10);
		GUIButton gUIButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(-221f, 262f));
		gUIButton.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_brawler_airlock_backbutton");
		gUIButton.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton.Click += delegate
		{
			Close();
		};
		Add(gUIButton);
		GUIButton gUIButton2 = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(512f, 512f), new Vector2(0f, 260f));
		gUIButton2.StyleInfo = new SHSButtonStyleInfo("cardgamegadget_bundle|L_gadget_button_large_play");
		gUIButton2.IsEnabled = false;
		Add(gUIButton2);
		GUIButton gUIButton3 = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(256f, 256f), new Vector2(215f, 258f));
		gUIButton3.StyleInfo = new SHSButtonStyleInfo("brawlergadget_bundle|L_brawler_airlock_quitbutton");
		gUIButton3.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton3.Click += delegate
		{
			Close();
		};
		Add(gUIButton3);
		GUISimpleControlWindow gUISimpleControlWindow2 = GUIControl.CreateControlFrameCentered<GUISimpleControlWindow>(new Vector2(70f, 70f), new Vector2(413f, -234f));
		GUIButton gUIButton4 = new GUIButton();
		gUIButton4.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, new Vector2(0f, -32f), new Vector2(64f, 64f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIButton4.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton4.StyleInfo = new SHSButtonStyleInfo("common_bundle|button_close");
		gUIButton4.Click += delegate
		{
			Close();
		};
		gUISimpleControlWindow2.Add(gUIButton4);
		Add(gUISimpleControlWindow2);
		base.AnimationPieceManager.Add(GenerateCardAnimation(cardsToAnimate[0]));
	}

	public override void OnShow()
	{
		AppShell.Instance.EventMgr.AddListener<InvitationCardGameCanceledMessage>(OnCardGameInviteCanceledMessage);
		base.OnShow();
	}

	public override void OnHide()
	{
		AppShell.Instance.EventMgr.RemoveListener<InvitationCardGameCanceledMessage>(OnCardGameInviteCanceledMessage);
		base.OnHide();
	}

	private void OnCardGameInviteCanceledMessage(InvitationCardGameCanceledMessage message)
	{
		Hide();
	}

	private AnimClip GenerateCardAnimation(GUIImage ctrl)
	{
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		Vector2 size = ctrl.Size;
		float x = size.x;
		Vector2 size2 = ctrl.Size;
		float y = size2.y;
		float time = 0.2f;
		float num = x * (1.15f + UnityEngine.Random.Range(0f, 0.3f));
		float num2 = y * (1.15f + UnityEngine.Random.Range(-0.5f, 0.1f));
		AnimClip animClip = AnimClipBuilder.Absolute.SizeX(AnimClipBuilder.Path.Linear(x, num, time), ctrl) ^ AnimClipBuilder.Absolute.SizeY(AnimClipBuilder.Path.Linear(y, num2, time), ctrl);
		AnimClip animClipShrink = AnimClipBuilder.Absolute.SizeX(AnimClipBuilder.Path.Linear(num, x, time), ctrl) ^ AnimClipBuilder.Absolute.SizeY(AnimClipBuilder.Path.Linear(num2, y, time), ctrl);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			animCardIndex = (animCardIndex + 1) % 7;
			base.AnimationPieceManager.Add(animClipShrink);
			AnimClip animClip2 = GenerateCardAnimation(cardsToAnimate[animCardIndex]);
			if (animCardIndex == 0)
			{
				animClip2 = (AnimClipBuilder.Absolute.Nothing(AnimClipBuilder.Path.Linear(0f, 0f, 0.5f)) | animClip2);
			}
			base.AnimationPieceManager.Add(animClip2);
		};
		return animClip;
	}

	private void Close()
	{
		AppShell.Instance.Matchmaker2.Cancel();
		AppShell.Instance.Matchmaker2.CancelAllInvitations();
		AppShell.Instance.Matchmaker2.LeavePvPQueue(null);
		if (invite != null)
		{
			invite.Cancel(true);
		}
		Hide();
	}
}
