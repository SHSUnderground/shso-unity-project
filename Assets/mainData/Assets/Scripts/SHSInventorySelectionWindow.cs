public class SHSInventorySelectionWindow : SHSSelectionWindow<SHSInventorySelectionItem, SHSItemLoadingWindow>
{
	public SHSInventorySelectionWindow(GUISlider slider)
		: base(slider, SelectionWindowType.ThreeAcross)
	{
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		TopOffsetAdjustHeight = 5f;
		BottomOffsetAdjustHeight = 15f;
		slider.FireChanged();
	}
}
