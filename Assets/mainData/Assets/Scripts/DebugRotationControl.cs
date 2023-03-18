using UnityEngine;

public class DebugRotationControl : GUIControlWindow
{
	public override void Draw(DrawModeSetting drawFlags)
	{
		Vector2 rectSize = base.RectSize;
		float x = rectSize.x;
		Vector2 rectSize2 = base.RectSize;
		GUI.Box(new Rect(0f, 0f, x, rectSize2.y), "Rotate me");
		base.Draw(drawFlags);
	}
}
