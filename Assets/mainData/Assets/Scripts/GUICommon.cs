using System;
using UnityEngine;

internal class GUICommon
{
	public enum IntersectResult
	{
		Inside,
		Outside,
		Intersects
	}

	public static Rect Center(Vector2 point, Rect rect)
	{
		return new Rect(point.x - rect.width / 2f, point.y - rect.height / 2f, rect.width, rect.height);
	}

	public static Rect Center(Vector2 point, Vector2 size)
	{
		return new Rect(point.x - size.x / 2f, point.y - size.y / 2f, size.x, size.y);
	}

	public static Vector2 CenterPoint(Rect rect)
	{
		return new Vector2(rect.width / 2f + rect.x, rect.height / 2f + rect.y);
	}

	public static Vector2 normalize(Vector2 vector)
	{
		return new Vector2(vector.x / vector.magnitude, vector.y / vector.magnitude);
	}

	public static Vector2 WorldToUIPoint(Camera camera, Vector3 position)
	{
		Vector3 vector = camera.WorldToScreenPoint(position);
		return new Vector2(vector.x, GUIManager.ScreenRect.height - vector.y);
	}

	public static bool Intersects(Rect outer, Rect inner)
	{
		return !(inner.xMin > outer.xMax) && !(inner.xMax < outer.xMin) && !(inner.yMin > outer.yMax) && !(inner.yMax < outer.yMin);
	}

	public static bool Contains(Rect outer, Rect inner)
	{
		return inner.xMin >= outer.xMin && inner.xMax < outer.xMax && inner.yMin > outer.yMin && inner.yMax < outer.yMax;
	}

	public static BitTexture MaskFromTexture(Texture2D texture, float scale, float opacityThreshold)
	{
		//Discarded unreachable code: IL_00b2, IL_01c9
		if (texture == null)
		{
			return null;
		}
		int num = Convert.ToInt32(Math.Ceiling((float)texture.height * scale));
		int num2 = Convert.ToInt32(Math.Ceiling((float)texture.width * scale));
		if (num == 0 || num2 == 0)
		{
			CspUtils.DebugLog("Can't make a mask from a texture that has no pixels.");
			return null;
		}
		BitTexture bitTexture = new BitTexture(num2, num);
		try
		{
			Color pixel = texture.GetPixel(0, 0);
		}
		catch (UnityException ex)
		{
			if (!ex.Message.Contains("not readable"))
			{
				throw ex;
			}
			CspUtils.DebugLog("Cant create mask because: " + texture.name.ToString() + " is not readable.");
			return null;
		}
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				Color pixel2 = texture.GetPixel(Convert.ToInt32((float)j * (1f / scale)), Convert.ToInt32((float)(num - i) * (1f / scale)));
				try
				{
					bitTexture.Bits[i * bitTexture.Width + j] = ((!(pixel2.a < opacityThreshold)) ? true : false);
				}
				catch (Exception ex2)
				{
					CspUtils.DebugLog("ROW: " + i + " ROWS:" + num + " COL:" + j + " COLS:" + num2);
					CspUtils.DebugLog("BITS LENGTH: " + bitTexture.Bits.Length);
					CspUtils.DebugLog("COMPUTED BITS: " + i * num + j);
					throw ex2;
				}
			}
		}
		return bitTexture;
	}

	public static Texture2D TextureFromMask(BitTexture Mask)
	{
		if (Mask == null)
		{
			return null;
		}
		if (Mask.Width == 0 || Mask.Height == 0)
		{
			CspUtils.DebugLog("Texture from mask: " + Mask.ToString() + " cant be 0 size.");
			return null;
		}
		Texture2D texture2D = new Texture2D(Mask.Width, Mask.Height);
		for (int i = 0; i < Mask.Bits.Length; i++)
		{
			int x = i % texture2D.width;
			int y = i / texture2D.width;
			texture2D.SetPixel(x, y, (!Mask.Bits[i]) ? new Color(0f, 0f, 0f, 1f) : new Color(1f, 0f, 1f, 1f));
		}
		texture2D.Apply();
		return texture2D;
	}
}
