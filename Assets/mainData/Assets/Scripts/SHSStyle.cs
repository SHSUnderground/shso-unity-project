using UnityEngine;

public class SHSStyle
{
	private static SHSStyle noStyle = new SHSStyle();

	public GameObject AudioOver;

	public GameObject AudioOut;

	public GameObject AudioDown;

	public GameObject AudioUp;

	public readonly GUIStyle UnityStyle;

	public static SHSStyle NoStyle
	{
		get
		{
			return noStyle;
		}
	}

	public SHSStyle(GUIStyle style)
	{
		UnityStyle = style;
	}

	public SHSStyle(SHSStyle style)
	{
		if (style.UnityStyle == null)
		{
			CspUtils.DebugLog("Cant clone SHSStyle " + style.ToString() + " Because it has no underlying GUIStyle.");
		}
		else
		{
			UnityStyle = new GUIStyle(style.UnityStyle);
		}
	}

	public SHSStyle()
	{
	}
}
