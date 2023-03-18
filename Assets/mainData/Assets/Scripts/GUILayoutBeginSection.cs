using UnityEngine;

public class GUILayoutBeginSection : GUIChildControl
{
	public enum OrientationEnum
	{
		Horizontal,
		Vertical
	}

	private OrientationEnum orientation;

	private int sectionSize;

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

	public int SectionSize
	{
		get
		{
			return sectionSize;
		}
		set
		{
			sectionSize = value;
		}
	}

	public GUILayoutBeginSection(OrientationEnum Orientation)
		: this(Orientation, int.MaxValue)
	{
	}

	public GUILayoutBeginSection(OrientationEnum Orientation, int SectionSize)
	{
		orientation = Orientation;
		sectionSize = SectionSize;
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		base.Draw(drawFlags);
		if (orientation == OrientationEnum.Horizontal)
		{
			if (sectionSize == int.MaxValue)
			{
				GUILayout.BeginHorizontal();
			}
			else
			{
				GUILayout.BeginHorizontal(GUILayout.Width(sectionSize));
			}
		}
		else if (sectionSize == int.MaxValue)
		{
			GUILayout.BeginVertical();
		}
		else
		{
			GUILayout.BeginVertical(GUILayout.Height(sectionSize));
		}
	}
}
