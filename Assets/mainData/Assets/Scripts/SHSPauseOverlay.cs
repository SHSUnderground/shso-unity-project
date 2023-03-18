using System;
using UnityEngine;

public class SHSPauseOverlay : GUISimpleControlWindow
{
	public class PauseOverlayAnim : SHSAnimations
	{
		public static AnimClip GetScanlineAnim(SHSPauseOverlay main)
		{
			Vector2 size = main.scanline.Size;
			return Absolute.OffsetY(Path.Linear(0f - size.y, GUIManager.ScreenRect.height, 5f), main.scanline) ^ Absolute.AnimationAlpha(Path.Linear(0.6f, 1f, 2.5f) | Path.Linear(1f, 0.6f, 2.5f), main.scanline);
		}

		public static AnimClip GetPauseAnim(SHSPauseOverlay main)
		{
			return Absolute.AnimationAlpha(Path.Linear(1f, 0.2f, 0.5f) | Path.Linear(0.2f, 1f, 0.5f), main.pauseSymbol);
		}
	}

	protected const float MAX_HEIGHT = 1200f;

	protected GUIImage scanline;

	protected GUIImage pauseSymbol;

	protected GUIImage pauseBackground;

	protected float sizeScalar;

	private AnimClip scanlineMovement;

	private AnimClip pauseAlpha;

	public SHSPauseOverlay()
	{
		Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		IsVisible = false;
		SetPosition(QuickSizingHint.Centered);
		SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		sizeScalar = GUIManager.ScreenRect.height / 1200f;
		pauseBackground = GUIControl.CreateControlFrameCentered<GUIImage>(Vector2.zero, Vector2.zero);
		pauseBackground.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		pauseBackground.TextureSource = "hq_bundle|mshs_hq_pause_background";
		scanline = GUIControl.CreateControl<GUIImage>(Vector2.zero, Vector2.zero, DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle);
		scanline.SetSize(new Vector2(1f, 206f * sizeScalar), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Absolute);
		scanline.TextureSource = "hq_bundle|mshs_hq_hud_pause_scanline";
		pauseSymbol = GUIControl.CreateControl<GUIImage>(new Vector2(234f, 199f) * sizeScalar, new Vector2(0f, 60f), DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle);
		pauseSymbol.TextureSource = "hq_bundle|hq_pause_mode_pausesymbol";
		Add(pauseBackground);
		Add(scanline);
		Add(pauseSymbol);
	}

	public override void OnShow()
	{
		base.OnShow();
		BeginScanline();
		BeginPauseAlpha();
	}

	public void BeginScanline()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		AnimClip scanlineAnim = PauseOverlayAnim.GetScanlineAnim(this);
		scanlineAnim.OnFinished += (Action)(object)(Action)delegate
		{
			BeginScanline();
		};
		base.AnimationPieceManager.SwapOut(ref scanlineMovement, scanlineAnim);
	}

	public void BeginPauseAlpha()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		AnimClip pauseAnim = PauseOverlayAnim.GetPauseAnim(this);
		pauseAnim.OnFinished += (Action)(object)(Action)delegate
		{
			BeginPauseAlpha();
		};
		base.AnimationPieceManager.SwapOut(ref pauseAlpha, pauseAnim);
	}
}
