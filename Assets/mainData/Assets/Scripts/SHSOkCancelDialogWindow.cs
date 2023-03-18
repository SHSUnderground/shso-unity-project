public class SHSOkCancelDialogWindow : SHSCommonDialogWindow
{
	public SHSOkCancelDialogWindow()
		: base("common_bundle|L_mshs_button_ok", "common_bundle|L_mshs_button_cancel", typeof(SHSDialogOkButton), typeof(SHSDialogCancelButton))
	{
	}
}
