using UnityEngine;

public class GUILayoutSpace : GUIChildControl
{
	private int space;

	public int Space
	{
		get
		{
			return space;
		}
		set
		{
			space = value;
		}
	}

	public GUILayoutSpace(int Space)
	{
		space = Space;
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		base.Draw(drawFlags);
		GUILayout.Space(space);
	}
}
