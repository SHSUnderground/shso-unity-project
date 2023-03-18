using UnityEngine;

public class SHSSocialWatcherFractalFindMe : SHSCommonDialogWindow
{
	private GUIImage watcherIcon;

	private GUIImage fractalIcon;

	private GUIImage watcherPortraitFrame;

	public SHSSocialWatcherFractalFindMe(FractalActivitySpawnPoint.FractalType fractalType, int balance)
		: base(string.Empty, string.Empty, "common_bundle|L_mshs_button_ok", "common_bundle|L_mshs_button_cancel", typeof(SHSDialogOkButton), typeof(SHSDialogCancelButton), false)
	{
		float d = 0.65f;
		Vector2 position = new Vector2(895f, 30f) * d;
		Vector2 size = new Vector2(203f, 194f) * d;
		Vector2 position2 = new Vector2(870f, 0f) * d;
		Vector2 size2 = new Vector2(256f, 256f) * d;
		watcherPortraitFrame = GUIControl.CreateControl<GUIImage>(Vector2.zero, Vector2.zero, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		watcherPortraitFrame.TextureSource = "common_bundle|mshs_common_hud_character_frame";
		watcherPortraitFrame.SetPosition(position);
		watcherPortraitFrame.SetSize(size);
		watcherPortraitFrame.IsVisible = true;
		Add(watcherPortraitFrame);
		watcherIcon = GUIControl.CreateControl<GUIImage>(Vector2.zero, Vector2.zero, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		watcherIcon.TextureSource = "characters_bundle|watcher_boss_HUD_default";
		watcherIcon.SetPosition(position2);
		watcherIcon.SetSize(size2);
		watcherIcon.IsVisible = true;
		Add(watcherIcon);
		fractalIcon = GUIControl.CreateControl<GUIImage>(Vector2.zero, Vector2.zero, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		fractalIcon.SetPosition(new Vector2(145f, base.size.y / 4f));
		fractalIcon.SetSize(new Vector2(84f, 84f));
		fractalIcon.TextureSource = "goodieseffects_bundle|goody_effects_fractal_shopping";
		Add(fractalIcon);
		text.FrontColor = new Color(0f, 10f / 51f, 14f / 51f, 1f);
		text.BackColor = new Color(0f, 4f / 51f, 14f / 51f, 0.1f);
		type = NotificationType.Common;
		SHSFractalsActivity sHSFractalsActivity = AppShell.Instance.ActivityManager.GetActivity("fractalsactivity") as SHSFractalsActivity;
		TitleText = string.Format(AppShell.Instance.stringTable["#WATCHER_FRACAL_FIND_ME"], sHSFractalsActivity.MaxFractals);
	}

	public override void OnShow()
	{
		SetupBundleDependencies();
		Vector2 vector = BuildTextBlock();
		BuildBackground(new Vector2(vector.x + 100f, vector.y));
		FinalizeAllUIPositioning();
	}

	protected override void dispose(bool disposing)
	{
		base.dispose(disposing);
		watcherIcon = null;
		watcherPortraitFrame = null;
		fractalIcon = null;
	}
}
