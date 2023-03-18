public class GUITBSlider : GUISlider
{
	public GUITBSlider(SliderOrientationEnum orientation)
	{
		base.Orientation = orientation;
		IsVisible = true;
		IsEnabled = true;
		base.Value = 0f;
		base.Min = 0f;
		base.Max = 100f;
	}
}
