public class SHSYesNoDialogWindow : SHSCommonDialogWindow
{
	public SHSYesNoDialogWindow()
		: base("common_bundle|L_mshs_button_yes", "common_bundle|L_mshs_button_no", typeof(SHSDialogOkButton), typeof(SHSDialogCancelButton))
	{
	}
}
