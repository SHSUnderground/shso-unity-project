using UnityEngine;

public class FirstCraftFoundWindow : SHSCommonDialogWindow
{
	private GUIImage craftIcon;

	public FirstCraftFoundWindow()
		: base(string.Empty, "common_bundle|L_mshs_button_ok", typeof(SHSDialogOkButton))
	{
		craftIcon = GUIControl.CreateControl<GUIImage>(Vector2.zero, Vector2.zero, DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight);
		craftIcon.SetPosition(new Vector2(145f, size.y / 4f));
		craftIcon.SetSize(new Vector2(84f, 84f));
		craftIcon.TextureSource = "shopping_bundle|craft_generic";
		Add(craftIcon);
		text.FrontColor = new Color(0f, 10f / 51f, 14f / 51f, 1f);
		text.BackColor = new Color(0f, 4f / 51f, 14f / 51f, 0.1f);
		type = NotificationType.Common;
		TitleText = "#CRAFT_FIRST_FOUND";
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
		craftIcon = null;
	}
}
