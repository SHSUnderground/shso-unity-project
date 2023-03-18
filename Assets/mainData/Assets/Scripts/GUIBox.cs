using UnityEngine;

public class GUIBox : GUIChildControl
{
	public override void Draw(DrawModeSetting drawFlags)
	{
		if (Style != SHSStyle.NoStyle)
		{
			GUI.Box(Rect, base.Content, Style.UnityStyle);
		}
		else
		{
			GUI.Box(Rect, base.Content);
		}
	}
}
