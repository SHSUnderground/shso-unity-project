using System;
using UnityEngine;

//[Extension]
public static class RectExtensions
{
	//[Extension]
	public static Rect Combine(Rect srcRect, Rect tgtRect)
	{
		return new Rect(srcRect.x + tgtRect.x, srcRect.y + tgtRect.y, srcRect.width + tgtRect.width, srcRect.height + tgtRect.height);
	}
}
