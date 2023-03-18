using UnityEngine;

public static class ColorUtil
{
	public static Color FromRGB255(int r, int g, int b)
	{
		return new Color((float)r / 255f, (float)g / 255f, (float)b / 255f);
	}

	public static Color FromRGB255(int r, int g, int b, float a)
	{
		return new Color((float)r / 255f, (float)g / 255f, (float)b / 255f, a);
	}
}
