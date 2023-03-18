using UnityEngine;

[AddComponentMenu("GUI/CurveAnimation")]
public class GUICurveAnimation : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float PositionX;

	public float PositionY;

	public float SizeX;

	public float SizeY;

	public float Alpha;

	public float OffsetX;

	public float OffsetY;

	public float Rotation;

	public bool UsePositionX = true;

	public bool UsePositionY = true;

	public bool UseSizeX = true;

	public bool UseSizeY = true;

	public bool UseAlpha = true;

	public bool UseOffsetX = true;

	public bool UseOffsetY = true;

	public bool UseRotation = true;

	public void SetGUIControl(GUIControl ctrl)
	{
		float num;
		if (UsePositionX)
		{
			num = PositionX;
		}
		else
		{
			Vector2 position = ctrl.Position;
			num = position.x;
		}
		float x = num;
		float num2;
		if (UsePositionY)
		{
			num2 = PositionY;
		}
		else
		{
			Vector2 position2 = ctrl.Position;
			num2 = position2.y;
		}
		float y = num2;
		float num3;
		if (UseSizeX)
		{
			num3 = SizeX;
		}
		else
		{
			Vector2 size = ctrl.Size;
			num3 = size.x;
		}
		float x2 = num3;
		float num4;
		if (UseSizeY)
		{
			num4 = SizeY;
		}
		else
		{
			Vector2 size2 = ctrl.Size;
			num4 = size2.y;
		}
		float y2 = num4;
		float alpha = (!UseAlpha) ? ctrl.Alpha : Alpha;
		float num5;
		if (UseOffsetX)
		{
			num5 = OffsetX;
		}
		else
		{
			Vector2 offset = ctrl.Offset;
			num5 = offset.x;
		}
		float x3 = num5;
		float num6;
		if (UseOffsetY)
		{
			num6 = OffsetY;
		}
		else
		{
			Vector2 offset2 = ctrl.Offset;
			num6 = offset2.y;
		}
		float y3 = num6;
		float rotation = (!UseRotation) ? ctrl.Rotation : Rotation;
		ctrl.SetPosition(new Vector2(x, y), ctrl.Docking, ctrl.Anchor, ctrl.OffsetStyle, new Vector2(x3, y3));
		ctrl.SetSize(new Vector2(x2, y2), ctrl.HorizontalSizeHint, ctrl.VerticalSizeHint);
		ctrl.Alpha = alpha;
		ctrl.Rotation = rotation;
	}
}
