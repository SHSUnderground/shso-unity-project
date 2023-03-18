using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSBrawlerCharacterCombinationWindow : GUIChildWindow
{
	private class SHSBrawlerCharacterCombinationBannerWindow : GUIChildWindow
	{
		private static readonly Vector2 DEFAULT_WINDOW_SIZE = new Vector2(449f, 103f);

		private static readonly Vector2 DEFAULT_BANNER_SIZE = new Vector2(1f, 103f);

		private GUIImage _ccBannerMiddle;

		public override bool InitializeResources(bool reload)
		{
			SetSize(DEFAULT_WINDOW_SIZE);
			SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle);
			GUIImage gUIImage = GUIControl.CreateControl<GUIImage>(new Vector2(209f, 103f), new Vector2(15f, 20f), DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft);
			gUIImage.TextureSource = "brawler_bundle|mission_hud_teamup_text_panel_left";
			gUIImage.Id = "CCBannerLeft";
			Add(gUIImage);
			_ccBannerMiddle = GUIControl.CreateControlBottomFrame<GUIImage>(DEFAULT_BANNER_SIZE, new Vector2(0f, 20f));
			_ccBannerMiddle.TextureSource = "brawler_bundle|mission_hud_teamup_text_panel_middle";
			_ccBannerMiddle.Id = "CCBannerMiddle";
			Add(_ccBannerMiddle);
			GUIImage gUIImage2 = GUIControl.CreateControl<GUIImage>(new Vector2(209f, 103f), new Vector2(-15f, 20f), DockingAlignmentEnum.BottomRight, AnchorAlignmentEnum.BottomRight);
			gUIImage2.TextureSource = "brawler_bundle|mission_hud_teamup_text_panel_right";
			gUIImage2.Id = "CCBannerRight";
			Add(gUIImage2);
			GUIImage gUIImage3 = GUIControl.CreateControl<GUIImage>(new Vector2(143f, 64f), new Vector2(-4f, -8f), DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft);
			gUIImage3.TextureSource = "brawler_bundle|L_mission_hud_teamup_text";
			gUIImage3.Id = "CCBannerTitle";
			Add(gUIImage3);
			return base.InitializeResources(reload);
		}

		public void ExpandBanner(float dSize, float dTime)
		{
			Vector2 size = Size;
			float x = size.x;
			Vector2 size2 = Size;
			AnimClip pieceOne = AnimClipBuilder.Absolute.SizeX(AnimClipBuilder.Path.Linear(x, size2.x + dSize, dTime), this);
			Vector2 size3 = _ccBannerMiddle.Size;
			float x2 = size3.x;
			Vector2 size4 = _ccBannerMiddle.Size;
			AnimClip toAdd = pieceOne ^ AnimClipBuilder.Absolute.SizeX(AnimClipBuilder.Path.Linear(x2, size4.x + dSize, dTime), _ccBannerMiddle);
			base.AnimationPieceManager.Add(toAdd);
		}
	}

	private static readonly int MAX_LABEL_WIDTH = 280;

	private static readonly int MAX_CHARACTERS = 4;

	private static readonly Vector2 DEFAULT_LABEL_SIZE = new Vector2(124f, 103f);

	private static readonly Vector2 DEFAULT_FRAME_SIZE = new Vector2(94f, 88f);

	private static readonly Vector2 DEFAULT_HEAD_SIZE = new Vector2(95f, 95f);

	private static readonly Vector2 DEFAULT_SWIPE_SIZE = new Vector2(72f, 72f);

	private static readonly Vector2 DEFAULT_SWIPE_COVER_SIZE = new Vector2(0f, 103f);

	private static readonly Vector2 FRAME_OFFSET = new Vector2(404f, 172f);

	private static readonly float FRAME_STRIDE = 80f;

	private static readonly int SWIPE_COUNT = 2;

	private static readonly float RAY_FADE_TIME = 0.5f;

	private static readonly float CHARACTER_APPEAR_TIME = 0.5f;

	private static readonly float CHARACTER_INITIAL_APPEAR_TIME = 0.75f;

	private static readonly float CHARACTER_SHRINK_TIME = 0.25f;

	private static readonly float CHARACTER_BOUNCE_TIME = 0.125f;

	private static readonly float CHARACTER_SCALE = 1.2f;

	private static readonly float SWIPE_TIME = 1f;

	private static readonly float BANNER_FADE_IN_TIME = 0.25f;

	private static readonly float BANNER_EXPAND_TIME = 0.5f;

	private GUIImage[] _ccFrames;

	private GUIImage[] _ccHeads;

	private GUIImage _ccRays;

	private GUILabel _ccTitle;

	private GUILabel _ccDescription;

	private GUIImage[] _ccSwipes;

	private GUIImage _ccSwipeCover;

	private SHSBrawlerCharacterCombinationBannerWindow _ccBanner;

	private AnimClip _rayFadeAnimation;

	private bool _ccVisible;

	public SHSBrawlerCharacterCombinationWindow()
	{
		_ccFrames = new GUIImage[MAX_CHARACTERS];
		_ccHeads = new GUIImage[MAX_CHARACTERS];
		Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		Traits.HitTestType = HitTestTypeEnum.Transparent;
	}

	public override bool InitializeResources(bool reload)
	{
		SetSize(new Vector2(900f, 300f));
		SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle);
		_ccRays = GUIControl.CreateControlBottomFrame<GUIImage>(new Vector2(438f, 436f), new Vector2(0f, 190f));
		_ccRays.TextureSource = "brawler_bundle|stagecomplete_rays";
		_ccRays.Id = "CCRayFade";
		Add(_ccRays);
		GUIImage gUIImage = GUIControl.CreateControlBottomFrame<GUIImage>(new Vector2(278f, 269f), new Vector2(0f, 91f));
		gUIImage.TextureSource = "brawler_bundle|mission_hud_teamup_background";
		gUIImage.Id = "CCBackground";
		Add(gUIImage);
		for (int i = 0; i < MAX_CHARACTERS; i++)
		{
			_ccFrames[i] = GUIControl.CreateControlAbsolute<GUIImage>(DEFAULT_FRAME_SIZE, FRAME_OFFSET);
			_ccFrames[i].TextureSource = "brawler_bundle|mission_hud_teamup_portrait_frame";
			_ccFrames[i].Id = "CCFrame_0" + i;
			_ccFrames[i].Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			_ccFrames[i].IsVisible = false;
			Add(_ccFrames[i]);
		}
		_ccBanner = new SHSBrawlerCharacterCombinationBannerWindow();
		_ccBanner.Id = "CCBannerWindow";
		Add(_ccBanner);
		_ccTitle = GUIControl.CreateControlBottomFrame<GUILabel>(DEFAULT_LABEL_SIZE, new Vector2(0f, 7f));
		_ccTitle.SetupText(GUIFontManager.SupportedFontEnum.Komica, 18, GUILabel.GenColor(23, 58, 75), TextAnchor.MiddleCenter);
		_ccTitle.Bold = true;
		_ccTitle.WordWrap = false;
		_ccTitle.Overflow = false;
		_ccTitle.Id = "CCTitle";
		Add(_ccTitle);
		_ccDescription = GUIControl.CreateControlBottomFrame<GUILabel>(DEFAULT_LABEL_SIZE, new Vector2(0f, 22f));
		_ccDescription.SetupText(GUIFontManager.SupportedFontEnum.Komica, 15, GUILabel.GenColor(143, 41, 1), TextAnchor.MiddleCenter);
		_ccDescription.Bold = false;
		_ccDescription.WordWrap = false;
		_ccDescription.Overflow = false;
		_ccDescription.Id = "CCDescription";
		Add(_ccDescription);
		_ccSwipes = new GUIImage[SWIPE_COUNT];
		for (int j = 0; j < SWIPE_COUNT; j++)
		{
			_ccSwipes[j] = GUIControl.CreateControlBottomFrameCentered<GUIImage>(DEFAULT_SWIPE_SIZE, new Vector2(0f, -36f));
			_ccSwipes[j].TextureSource = "brawler_bundle|mission_hud_teamup_small_highlight";
			_ccSwipes[j].Id = "CCSwipe_0" + j;
			_ccSwipes[j].Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			_ccSwipes[j].IsVisible = false;
			Add(_ccSwipes[j]);
		}
		_ccSwipeCover = GUIControl.CreateControl<GUIImage>(DEFAULT_SWIPE_COVER_SIZE, new Vector2(0f, 20f), DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft);
		_ccSwipeCover.TextureSource = "brawler_bundle|mission_hud_teamup_text_panel_middle_coverup";
		_ccSwipeCover.Id = "CCSwipeCover";
		_ccSwipeCover.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		_ccSwipeCover.IsVisible = true;
		Add(_ccSwipeCover);
		for (int k = 0; k < MAX_CHARACTERS; k++)
		{
			GUIImage[] ccHeads = _ccHeads;
			int num = k;
			Vector2 dEFAULT_HEAD_SIZE = DEFAULT_HEAD_SIZE;
			Vector2 fRAME_OFFSET = FRAME_OFFSET;
			float x = fRAME_OFFSET.x;
			Vector2 fRAME_OFFSET2 = FRAME_OFFSET;
			ccHeads[num] = GUIControl.CreateControlAbsolute<GUIImage>(dEFAULT_HEAD_SIZE, new Vector2(x, fRAME_OFFSET2.y - 5f));
			_ccHeads[k].Id = "CCHead_0" + k;
			_ccHeads[k].Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			_ccHeads[k].IsVisible = false;
			Add(_ccHeads[k]);
		}
		return base.InitializeResources(reload);
	}

	public override void OnShow()
	{
		base.OnShow();
		DoRayOpacityAnimation(true);
		PlayCharacterComboSound();
	}

	public override void OnHide()
	{
		base.OnHide();
		_ccVisible = false;
	}

	public void SetCombination(List<string> characters, string title, string description)
	{
		base.AnimationPieceManager.ClearAll();
		DoRayOpacityAnimation(true);
		if (_ccVisible)
		{
			DoSwipeAnimation(characters, title, description);
		}
		else
		{
			DoBannerAnimation(characters, title, description);
		}
		_ccVisible = true;
	}

	public void SetCharacters(List<string> characters, bool doAnimation)
	{
		if (characters == null || _ccFrames == null || _ccHeads == null)
		{
			return;
		}
		Vector2 fRAME_OFFSET = FRAME_OFFSET;
		float num = fRAME_OFFSET.x - FRAME_STRIDE / 2f * (float)(characters.Count - 1);
		float num2 = CHARACTER_INITIAL_APPEAR_TIME;
		for (int i = 0; i < MAX_CHARACTERS; i++)
		{
			GUIImage gUIImage = _ccFrames[i];
			GUIImage gUIImage2 = _ccHeads[i];
			gUIImage.IsVisible = false;
			gUIImage2.IsVisible = false;
			if (i < characters.Count)
			{
				float x = num + (float)i * FRAME_STRIDE;
				Vector2 offset = gUIImage.Offset;
				gUIImage.Offset = new Vector2(x, offset.y);
				float x2 = num + (float)i * FRAME_STRIDE;
				Vector2 offset2 = gUIImage2.Offset;
				gUIImage2.Offset = new Vector2(x2, offset2.y);
				gUIImage2.TextureSource = "characters_bundle|token_" + characters[i];
				if (doAnimation)
				{
					DoCharacterAnimation(gUIImage, DEFAULT_FRAME_SIZE, num2);
					DoCharacterAnimation(gUIImage2, DEFAULT_HEAD_SIZE, num2);
				}
				else
				{
					gUIImage.IsVisible = true;
					gUIImage2.IsVisible = true;
					gUIImage.SetSize(DEFAULT_FRAME_SIZE);
					gUIImage2.SetSize(DEFAULT_HEAD_SIZE);
				}
				num2 += CHARACTER_APPEAR_TIME;
			}
		}
	}

	public float GetBannerLabelWidth()
	{
		float result;
		if (_ccTitle != null)
		{
			Vector2 size = _ccTitle.Size;
			result = size.x;
		}
		else
		{
			result = 0f;
		}
		return result;
	}

	private void DoRayOpacityAnimation(bool fadeOut)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		float num;
		float finish;
		if (fadeOut)
		{
			num = 1f;
			finish = 0.6f;
		}
		else
		{
			num = 0.6f;
			finish = 1f;
		}
		_ccRays.Alpha = num;
		_rayFadeAnimation = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(num, finish, RAY_FADE_TIME), _ccRays);
		_rayFadeAnimation.OnFinished += (Action)(object)(Action)delegate
		{
			DoRayOpacityAnimation(!fadeOut);
		};
		base.AnimationPieceManager.SwapOut(ref _rayFadeAnimation, _rayFadeAnimation);
	}

	private void DoCharacterAnimation(GUIImage image, Vector2 defSize, float waitTime)
	{
		Vector2 vector = defSize * CHARACTER_SCALE;
		image.Size = vector;
		AnimClip toAdd = SHSAnimations.Generic.Wait(waitTime) | SHSAnimations.Generic.ChangeVisibility(true, image) | SHSAnimations.Generic.ChangeSizeDirect(image, defSize, vector, CHARACTER_SHRINK_TIME, 0f) | AnimClipBuilder.Custom.Function(1f + AnimClipBuilder.Path.Sin(0f, 0.5f, CHARACTER_BOUNCE_TIME) * AnimClipBuilder.Path.Linear(0.1f, 0.05f, CHARACTER_BOUNCE_TIME), delegate(float value)
		{
			image.Size = defSize * value;
		});
		base.AnimationPieceManager.Add(toAdd);
	}

	private void DoSwipeAnimation(List<string> characters, string title, string description)
	{
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Expected O, but got Unknown
		Vector2 size = _ccSwipes[0].Size;
		float num = size.x / 2f + GetBannerLabelWidth() / 2f + 28f;
		float num2 = 2f * num;
		for (int i = 0; i < SWIPE_COUNT; i++)
		{
			_ccSwipes[i].Offset = new Vector3(0f - num, -36f, 0f);
			_ccSwipes[i].Size = new Vector2(0f, 0f);
			_ccSwipes[i].Rotation = 0f;
			_ccSwipes[i].Alpha = 0.5f;
			_ccSwipes[i].IsVisible = true;
		}
		_ccSwipeCover.IsVisible = false;
		GUIImage ccSwipeCover = _ccSwipeCover;
		Vector2 position = _ccTitle.Position;
		ccSwipeCover.Offset = new Vector2(position.x, 20f);
		AnimClip animClip = AnimClipBuilder.Absolute.OffsetX(AnimClipBuilder.Path.Linear(0f - num, num, SWIPE_TIME), _ccSwipes) ^ AnimClipBuilder.Absolute.Rotation(AnimClipBuilder.Path.Linear(0f, 360f, SWIPE_TIME), _ccSwipes[0]) ^ AnimClipBuilder.Absolute.Rotation(AnimClipBuilder.Path.Linear(360f, 0f, SWIPE_TIME), _ccSwipes[1]);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			OnSwipeEnd();
			DoBannerAnimation(characters, title, description);
		};
		base.AnimationPieceManager.Add(animClip);
		Vector2 dEFAULT_SWIPE_SIZE = DEFAULT_SWIPE_SIZE;
		float start = dEFAULT_SWIPE_SIZE.x / 2f;
		Vector2 dEFAULT_SWIPE_SIZE2 = DEFAULT_SWIPE_SIZE;
		AnimClip pieceOne = AnimClipBuilder.Absolute.SizeXY(AnimClipBuilder.Path.Linear(start, dEFAULT_SWIPE_SIZE2.x, SWIPE_TIME / 2f), _ccSwipes) ^ AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0.5f, 1f, SWIPE_TIME / 2f), _ccSwipes);
		Vector2 dEFAULT_SWIPE_SIZE3 = DEFAULT_SWIPE_SIZE;
		float x = dEFAULT_SWIPE_SIZE3.x;
		Vector2 dEFAULT_SWIPE_SIZE4 = DEFAULT_SWIPE_SIZE;
		animClip = (pieceOne | (AnimClipBuilder.Absolute.SizeXY(AnimClipBuilder.Path.Linear(x, dEFAULT_SWIPE_SIZE4.x / 2f, SWIPE_TIME / 2f), _ccSwipes) ^ AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(1f, 0f, SWIPE_TIME / 2f), _ccSwipes)));
		base.AnimationPieceManager.Add(animClip);
		float num3 = GetBannerLabelWidth() + 2f;
		float num4 = SWIPE_TIME * (num2 - num3) / (2f * num2);
		float time = SWIPE_TIME - 2f * num4;
		animClip = (SHSAnimations.Generic.Wait(num4) | (SHSAnimations.Generic.ChangeVisibility(true, _ccSwipeCover) ^ AnimClipBuilder.Absolute.SizeX(AnimClipBuilder.Path.Linear(0f, num3, time), _ccSwipeCover)));
		base.AnimationPieceManager.Add(animClip);
	}

	private void DoBannerAnimation(List<string> characters, string title, string description)
	{
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Expected O, but got Unknown
		_ccTitle.Text = title;
		_ccTitle.Alpha = 0f;
		_ccDescription.Text = description;
		_ccDescription.Alpha = 0f;
		_ccTitle.ClearKerning();
		_ccTitle.CalculateTextLayout();
		_ccDescription.ClearKerning();
		_ccDescription.CalculateTextLayout();
		Vector2 dEFAULT_LABEL_SIZE = DEFAULT_LABEL_SIZE;
		int min = (int)dEFAULT_LABEL_SIZE.x;
		int width = Mathf.Clamp(Mathf.Max(_ccTitle.LongestLine, _ccDescription.LongestLine), min, MAX_LABEL_WIDTH);
		Vector2 dEFAULT_LABEL_SIZE2 = DEFAULT_LABEL_SIZE;
		float num = dEFAULT_LABEL_SIZE2.x + (float)width - (float)min - GetBannerLabelWidth();
		float num2 = Mathf.Abs(num) * (BANNER_EXPAND_TIME / (float)(MAX_LABEL_WIDTH - min));
		if (num2 > 0f)
		{
			_ccBanner.ExpandBanner(num, num2);
		}
		_ccSwipeCover.Size = DEFAULT_SWIPE_COVER_SIZE;
		_ccSwipeCover.IsVisible = false;
		AnimClip toAdd = SHSAnimations.Generic.Wait(num2) | AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0f, 1f, BANNER_FADE_IN_TIME), _ccTitle, _ccDescription);
		base.AnimationPieceManager.Add(toAdd);
		toAdd = SHSAnimations.Generic.Wait(num2);
		toAdd.OnFinished += (Action)(object)(Action)delegate
		{
			GUILabel ccTitle = _ccTitle;
			Vector2 dEFAULT_LABEL_SIZE3 = DEFAULT_LABEL_SIZE;
			float x = dEFAULT_LABEL_SIZE3.x + (float)width - (float)min;
			Vector2 dEFAULT_LABEL_SIZE4 = DEFAULT_LABEL_SIZE;
			ccTitle.Size = new Vector2(x, dEFAULT_LABEL_SIZE4.y);
			GUILabel ccDescription = _ccDescription;
			Vector2 dEFAULT_LABEL_SIZE5 = DEFAULT_LABEL_SIZE;
			float x2 = dEFAULT_LABEL_SIZE5.x + (float)width - (float)min;
			Vector2 dEFAULT_LABEL_SIZE6 = DEFAULT_LABEL_SIZE;
			ccDescription.Size = new Vector2(x2, dEFAULT_LABEL_SIZE6.y);
			SetCharacters(characters, true);
		};
		base.AnimationPieceManager.Add(toAdd);
	}

	private void OnSwipeEnd()
	{
		for (int i = 0; i < SWIPE_COUNT; i++)
		{
			_ccSwipes[i].IsVisible = false;
		}
	}

	private void PlayCharacterComboSound()
	{
		ShsAudioSource.PlayAutoSound(ShsAudioSourceList.GetList("MissionGlobal").GetSource("char_combo_bonus"));
	}
}
