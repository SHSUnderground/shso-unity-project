using UnityEngine;

public class SHSSocialMayhemModeConfirmation : SHSCommonDialogWindow
{
	public delegate void OnExpend();

	private OnExpend _expendCallback;

	protected GUIImage villainIcon;

	protected GUIImage villainPortraitFrame;

	public SHSSocialMayhemModeConfirmation(string iconPath, string titleText, Color titleTextColor, OnExpend expendCallback)
		: base(string.Empty, string.Empty, "common_bundle|L_mshs_button_ok", "common_bundle|L_mshs_button_cancel", typeof(SHSDialogOkButton), typeof(SHSDialogCancelButton), false)
	{
		_expendCallback = expendCallback;
		if (iconPath != string.Empty)
		{
			float d = 0.65f;
			Vector2 position = new Vector2(885f, 23f) * d;
			Vector2 size = new Vector2(203f, 194f) * d;
			Vector2 position2 = new Vector2(860f, -7f) * d;
			Vector2 size2 = new Vector2(256f, 256f) * d;
			villainPortraitFrame = GUIControl.CreateControl<GUIImage>(Vector2.zero, Vector2.zero, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
			villainPortraitFrame.TextureSource = "common_bundle|mshs_common_hud_character_frame";
			villainPortraitFrame.SetPosition(position);
			villainPortraitFrame.SetSize(size);
			villainPortraitFrame.IsVisible = true;
			Add(villainPortraitFrame);
			villainIcon = GUIControl.CreateControl<GUIImage>(Vector2.zero, Vector2.zero, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
			villainIcon.TextureSource = "characters_bundle|" + iconPath;
			villainIcon.SetPosition(position2);
			villainIcon.SetSize(size2);
			villainIcon.IsVisible = true;
			Add(villainIcon);
		}
		text.FrontColor = titleTextColor;
		text.BackColor = new Color(0f, 4f / 51f, 14f / 51f, 0.1f);
		type = NotificationType.Common;
		Text = "#CONFIRM_MISSION_LAUNCH";
		TitleText = titleText;
	}

	public override void OnShow()
	{
		SetupBundleDependencies();
		Vector2 vector = BuildTextBlock();
		BuildBackground(new Vector2(vector.x + 100f, vector.y));
		FinalizeAllUIPositioning();
	}

	public override void OnOk()
	{
		base.OnOk();
		if (_expendCallback != null)
		{
			_expendCallback();
		}
	}

	protected override void dispose(bool disposing)
	{
		base.dispose(disposing);
		villainIcon = null;
	}
}
