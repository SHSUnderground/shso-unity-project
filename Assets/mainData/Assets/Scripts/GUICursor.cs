using UnityEngine;

public class GUICursor
{
	private Texture2D cursorImage;

	public Vector2 clickOffset = Vector2.zero;

	public Texture2D CursorImage
	{
		get
		{
			return cursorImage;
		}
	}

	public GUICursor(string cursorImageLoc, Vector2 clickOffset)
	{
		cursorImage = GUIManager.Instance.LoadTexture(cursorImageLoc);
		this.clickOffset = clickOffset;
	}
}
