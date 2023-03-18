using UnityEngine;

public class GUILayoutFlexibleSpace : GUIChildControl
{
	public override void Draw(DrawModeSetting drawFlags)
	{
		base.Draw(drawFlags);
		GUILayout.FlexibleSpace();
	}
}
