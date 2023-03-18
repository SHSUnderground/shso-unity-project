using UnityEngine;

public class SHSExpendableUseConfirmationWindow : SHSCommonDialogWindow
{
	public delegate void OnExpend();

	private OnExpend _expendCallback;

	public SHSExpendableUseConfirmationWindow(string expendableName, OnExpend expendCallback)
		: base("goodiescommon_bundle|notification_icon_consumables", "common_bundle|L_mshs_button_ok", "common_bundle|L_mshs_button_cancel", typeof(SHSDialogOkButton), typeof(SHSDialogCancelButton))
	{
		_expendCallback = expendCallback;
		type = NotificationType.Common;
		Text = "#EXPENDABLE_USE_CONFIRM";
		TitleText = expendableName;
		dialogIcon.SetSize(226f, 181f);
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
}
