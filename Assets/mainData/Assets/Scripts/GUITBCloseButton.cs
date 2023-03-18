using UnityEngine;

public class GUITBCloseButton : GUIButton
{
	public GUITBCloseButton()
	{
		StyleInfo = new SHSButtonStyleInfo("common_bundle|button_close");
		SetSize(44f, 44f);
		HitTestType = HitTestTypeEnum.Circular;
		hitTestSize = new Vector2(0.8f, 0.8f);
		base.ToolTip = new NamedToolTipInfo(SHSTooltip.GetCommonToolTipText(SHSTooltip.CommonToolTipText.Close));
	}
}
