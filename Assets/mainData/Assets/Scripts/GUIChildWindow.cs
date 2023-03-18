public class GUIChildWindow : GUIWindow
{
	public GUIChildWindow()
	{
		Traits = ControlTraits.ControlDefault;
		Traits.BlockTestType = BlockTestTypeEnum.Transparent;
	}
}
