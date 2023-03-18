using System;
using UnityEngine;

public class SHSVOSpeakerWindow : GUIDynamicWindow, IVOSpeakerWindow
{
	protected static readonly Vector2 OFFSET_FROM_BR = new Vector2(-10f, -100f);

	protected static readonly Vector2 FRAME_SIZE = new Vector2(100f, 100f);

	protected static readonly Vector2 PORTRAIT_SIZE = new Vector2(130f, 142f);

	protected static readonly Vector2 PORTRAIT_OFFSET_FROM_MID = new Vector2(-1f, 4f);

	protected static readonly Vector2 PORTRAIT_PADDING = new Vector2(40f, 40f);

	private GUIImage characterPortraitFrame;

	private GUIImage characterPortrait;

	private float animStartX = SHSVOSpeakerWindow.FRAME_SIZE.x - SHSVOSpeakerWindow.OFFSET_FROM_BR.x + SHSVOSpeakerWindow.PORTRAIT_PADDING.x;

	public static IVOSpeakerWindow CreateWindow()
	{
		SHSVOSpeakerWindow sHSVOSpeakerWindow = null;
		IGUIContainer root = GUIManager.Instance.GetRoot(GUIManager.UILayer.Notification);
		if (root != null)
		{
			sHSVOSpeakerWindow = new SHSVOSpeakerWindow();
			root.Add(sHSVOSpeakerWindow);
			sHSVOSpeakerWindow.Show();
		}
		return sHSVOSpeakerWindow;
	}

	public void SetVO(ResolvedVOAction vo)
	{
	}

	public void SetCharacter(string characterID)
	{
		Texture2D texture;
		if (GUIManager.Instance.LoadTexture("characters_bundle|" + characterID + "_targeted_default", out texture))
		{
			this.characterPortrait.Texture = texture;
			this.characterPortrait.Alpha = 1f;
		}
		else
		{
			this.characterPortrait.Alpha = 0f;
		}
	}

	public void SetText(string textID)
	{
	}

	public void AnimateIn()
	{
		base.AnimationPieceManager.Add(AnimClipBuilder.Custom.Function(SHSAnimations.GenericPaths.LinearWithBounce(this.animStartX, 0f, 0.3f), new Action<float>(this.OnAnimPosUpdate)));
	}

	public void AnimateOut()
	{
		base.AnimationPieceManager.Add(AnimClipBuilder.Custom.Function(SHSAnimations.GenericPaths.LinearWithBounce(0f, this.animStartX, 0.3f), new Action<float>(this.OnAnimPosUpdate)) | this.FinishAnimateOut());
	}

	public override bool InitializeResources(bool reload)
	{
		Vector2 size = SHSVOSpeakerWindow.FRAME_SIZE + SHSVOSpeakerWindow.PORTRAIT_PADDING;
		Vector2 oFFSET_FROM_BR = SHSVOSpeakerWindow.OFFSET_FROM_BR;
		oFFSET_FROM_BR.x += this.animStartX;
		this.SetPositionAndSize(GUIControl.DockingAlignmentEnum.BottomRight, GUIControl.AnchorAlignmentEnum.BottomRight, GUIControl.OffsetType.Absolute, oFFSET_FROM_BR, size, GUIControl.AutoSizeTypeEnum.Absolute, GUIControl.AutoSizeTypeEnum.Absolute);
		Vector2 fRAME_SIZE = SHSVOSpeakerWindow.FRAME_SIZE;
		Vector2 zero = Vector2.zero;
		this.characterPortraitFrame = GUIControl.CreateControl<GUIImage>(fRAME_SIZE, zero, GUIControl.DockingAlignmentEnum.Middle, GUIControl.AnchorAlignmentEnum.Middle);
		this.characterPortraitFrame.TextureSource = "common_bundle|mshs_common_hud_character_frame";
		this.Add(this.characterPortraitFrame);
		Vector2 pORTRAIT_SIZE = SHSVOSpeakerWindow.PORTRAIT_SIZE;
		Vector2 pORTRAIT_OFFSET_FROM_MID = SHSVOSpeakerWindow.PORTRAIT_OFFSET_FROM_MID;
		this.characterPortrait = GUIControl.CreateControl<GUIImage>(pORTRAIT_SIZE, pORTRAIT_OFFSET_FROM_MID, GUIControl.DockingAlignmentEnum.Middle, GUIControl.AnchorAlignmentEnum.Middle);
		this.characterPortrait.Alpha = 0f;
		this.Add(this.characterPortrait);
		this.Traits.BlockTestType = GUIControl.BlockTestTypeEnum.Transparent;
		this.Traits.HitTestType = GUIControl.HitTestTypeEnum.Transparent;
		this.SetControlFlag(GUIControl.ControlFlagSetting.HitTestIgnore, true, false);
		this.Traits.LifeSpan = GUIControl.ControlTraits.LifeSpanTraitEnum.DestroyOnHide;
		return base.InitializeResources(reload);
	}

	private void OnAnimPosUpdate(float x)
	{
		Vector2 oFFSET_FROM_BR = SHSVOSpeakerWindow.OFFSET_FROM_BR;
		oFFSET_FROM_BR.x += x;
		base.SetPosition(GUIControl.DockingAlignmentEnum.BottomRight, GUIControl.AnchorAlignmentEnum.BottomRight, GUIControl.OffsetType.Absolute, oFFSET_FROM_BR);
	}

	private AnimClip FinishAnimateOut()
	{
		return new AnimClipFunction(0f, delegate(float x)
		{
			this.Hide();
		});
	}
}
