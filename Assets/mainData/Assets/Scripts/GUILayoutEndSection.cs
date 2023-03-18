using UnityEngine;

public class GUILayoutEndSection : GUIChildControl
{
	public enum OrientationEnum
	{
		Horizontal,
		Vertical
	}

	private OrientationEnum orientation;

	public OrientationEnum Orientation
	{
		get
		{
			return orientation;
		}
		set
		{
			orientation = value;
		}
	}

	public GUILayoutEndSection(OrientationEnum orientation)
	{
		this.orientation = orientation;
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		base.Draw(drawFlags);
		if (orientation == OrientationEnum.Horizontal)
		{
			GUILayout.EndHorizontal();
		}
		else
		{
			GUILayout.EndVertical();
		}
	}
}
