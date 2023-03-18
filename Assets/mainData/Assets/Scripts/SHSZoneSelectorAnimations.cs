using System;
using UnityEngine;

public class SHSZoneSelectorAnimations : SHSAnimations
{
	public static AnimClip CenterSlider(SHSSlideZoneItem SelectedItem, float targetLocation, float currentLocation, SHSZoneChooserSlider slider, Action<float> fun)
	{
		float num = Mathf.Abs(targetLocation - currentLocation);
		float a = 0.3f * num;
		a = Mathf.Max(a, 0.2f);
		a = Mathf.Min(a, 0.75f);
		return Custom.Function(GenericPaths.LinearWithSingleWiggle(currentLocation, targetLocation, a), fun);
	}
}
