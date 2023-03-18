using System.Collections.Generic;
using UnityEngine;

public class NineSliceTexture
{
	public enum Side
	{
		Top,
		Left,
		Right,
		Bottom
	}

	public enum SideAlignment
	{
		Left,
		Middle,
		Right,
		Top,
		Bottom
	}

	public enum CompressionType
	{
		Uncompressed,
		Compressed
	}

	private struct BorderPiece
	{
		public int hOffset;

		public int hTotal;

		public int vOffset;

		public int vTotal;

		public static BorderPiece EmptyBorderPiece
		{
			get
			{
				BorderPiece result = default(BorderPiece);
				result.hOffset = 0;
				result.hTotal = 0;
				result.vOffset = 0;
				result.vTotal = 0;
				return result;
			}
		}
	}

	private Dictionary<int, Color[]> masterColorCache;

	private Color[] currentColorEntry;

	private Dictionary<KeyValuePair<int, int>, Texture2D> masterTextureCache;

	private CompressionType compressionType;

	private int leftSideOffset;

	private int topSideOffset;

	private int theTotalWidth;

	private int theTotalHeight;

	private int maxLeftMargin;

	private int maxRightMargin;

	private int maxTopMargin;

	private int maxBottomMargin;

	private bool delayedPixelApply;

	private int attachmentHOffset;

	private int attachmentVOffset;

	private bool verticalFlip;

	private bool horizontalFlip;

	private Texture2D texture;

	private Vector2 topLeftCornerSize = Vector2.zero;

	private Vector2 topRightCornerSize = Vector2.zero;

	private Vector2 bottomRightCornerSize = Vector2.zero;

	private Vector2 bottomLeftCornerSize = Vector2.zero;

	private Vector2 leftBorderSize = Vector2.zero;

	private Vector2 rightBorderSize = Vector2.zero;

	private Vector2 topBorderSize = Vector2.zero;

	private Vector2 bottomBorderSize = Vector2.zero;

	private Vector2 attachmentSize = Vector2.zero;

	public bool DelayedPixelApply
	{
		get
		{
			return delayedPixelApply;
		}
		set
		{
			delayedPixelApply = value;
		}
	}

	public int AttachmentHOffset
	{
		get
		{
			return attachmentHOffset;
		}
		set
		{
			attachmentHOffset = value;
		}
	}

	public int AttachmentVOffset
	{
		get
		{
			return attachmentVOffset;
		}
		set
		{
			attachmentVOffset = value;
		}
	}

	public bool VerticalFlip
	{
		get
		{
			return verticalFlip;
		}
		set
		{
			verticalFlip = value;
		}
	}

	public bool HorizontalFlip
	{
		get
		{
			return horizontalFlip;
		}
		set
		{
			horizontalFlip = value;
		}
	}

	public Texture2D Texture
	{
		get
		{
			return texture;
		}
	}

	public Vector2 TopLeftCornerSize
	{
		get
		{
			return topLeftCornerSize;
		}
	}

	public Vector2 TopRightCornerSize
	{
		get
		{
			return topRightCornerSize;
		}
	}

	public Vector2 BottomRightCornerSize
	{
		get
		{
			return bottomRightCornerSize;
		}
	}

	public Vector2 BottomLeftCornerSize
	{
		get
		{
			return bottomLeftCornerSize;
		}
	}

	public Vector2 LeftBorderSize
	{
		get
		{
			return leftBorderSize;
		}
	}

	public Vector2 RightBorderSize
	{
		get
		{
			return rightBorderSize;
		}
	}

	public Vector2 TopBorderSize
	{
		get
		{
			return topBorderSize;
		}
	}

	public Vector2 BottomBorderSize
	{
		get
		{
			return bottomBorderSize;
		}
	}

	public Vector2 AttachmentSize
	{
		get
		{
			return attachmentSize;
		}
	}

	public NineSliceTexture()
	{
		masterColorCache = new Dictionary<int, Color[]>();
		masterTextureCache = new Dictionary<KeyValuePair<int, int>, Texture2D>();
	}

	public NineSliceTexture(CompressionType compressionType)
	{
		this.compressionType = compressionType;
		masterColorCache = new Dictionary<int, Color[]>();
		masterTextureCache = new Dictionary<KeyValuePair<int, int>, Texture2D>();
	}

	private bool ValidTextureParameters(Texture2D bodyToCheck, Texture2D[] cornersToCheck, Texture2D[] bordersToCheck)
	{
		if (bodyToCheck == null)
		{
			CspUtils.DebugLog("Trying to create Nine Slice Texture with an invalid body texture!");
			return false;
		}
		if (cornersToCheck == null)
		{
			CspUtils.DebugLog("Trying to create Nine Slice Texture with an invalid array of corners!");
			return false;
		}
		if (bordersToCheck == null)
		{
			CspUtils.DebugLog("Trying to create Nine Slice Texture with an invalid array of borders!");
			return false;
		}
		if (cornersToCheck.Length != 4)
		{
			CspUtils.DebugLog("Trying to create a Nine Slice Texture without the proper amount of corner textures. There were only " + cornersToCheck.Length + " passed in!");
			return false;
		}
		if (bordersToCheck.Length != 4)
		{
			CspUtils.DebugLog("Trying to create a Nine Slice Texture without the proper amount of border textures. There were only " + bordersToCheck.Length + " passed in!");
			return false;
		}
		return true;
	}

	private void TextureCopy(Texture2D destination, Texture2D source, int widthOffset, int heightOffset)
	{
		for (int i = 0; i < source.width; i++)
		{
			for (int j = 0; j < source.height; j++)
			{
				int num = (!horizontalFlip) ? (leftSideOffset + widthOffset + i) : (destination.width - 1 - (leftSideOffset + widthOffset + i));
				int num2 = (!verticalFlip) ? (destination.height - 1 - (topSideOffset + heightOffset + j)) : (topSideOffset + heightOffset + j);
				int num3 = num2 * theTotalWidth;
				int num4 = num;
				currentColorEntry[num3 + num4] = source.GetPixel(i, source.height - 1 - j);
			}
		}
	}

	private void BorderPixelBlit(bool horizontal, Texture2D destinationTexture, Texture2D borderTexture, Vector2 dwellingSize, int hOffset, int vOffset)
	{
		Color[] array = new Color[(!horizontal) ? borderTexture.width : borderTexture.height];
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		for (int i = 0; i < array.Length; i++)
		{
			Color color = (!horizontal) ? borderTexture.GetPixel(i, 0) : borderTexture.GetPixel(0, borderTexture.height - 1 - i);
			array[i] = new Color(color.r, color.g, color.b, color.a);
		}
		if (horizontal)
		{
			for (int j = 0; j < (int)dwellingSize.x; j++)
			{
				for (int k = 0; k < array.Length; k++)
				{
					num3 = ((!horizontalFlip) ? (hOffset + j) : (destinationTexture.width - 1 - (hOffset + j)));
					num4 = ((!verticalFlip) ? (destinationTexture.height - 1 - (vOffset + k)) : (vOffset + k));
					num = num4 * theTotalWidth;
					num2 = num3;
					currentColorEntry[num + num2] = array[k];
				}
			}
		}
		else
		{
			for (int l = 0; l < array.Length; l++)
			{
				for (int m = 0; m < (int)dwellingSize.y; m++)
				{
					num3 = ((!horizontalFlip) ? (hOffset + l) : (destinationTexture.width - 1 - (hOffset + l)));
					num4 = ((!verticalFlip) ? (destinationTexture.height - 1 - (vOffset + m)) : (vOffset + m));
					num = num4 * theTotalWidth;
					num2 = num3;
					currentColorEntry[num + num2] = array[l];
				}
			}
		}
		array = null;
	}

	private Texture2D CreateBaseTexture(Vector2 dwellingSize, Texture2D body, Texture2D[] corners, Texture2D[] borders, int totalWidth, int totalHeight)
	{
		Texture2D texture2D = null;
		if (!ValidTextureParameters(body, corners, borders))
		{
			return null;
		}
		texture2D = getCachedTexture(totalWidth, totalHeight);
		TextureCopy(texture2D, corners[0], 0, 0);
		BorderPixelBlit(true, texture2D, borders[0], dwellingSize, leftSideOffset + corners[0].width, topSideOffset);
		TextureCopy(texture2D, corners[1], maxLeftMargin + (int)dwellingSize.x, 0);
		BorderPixelBlit(false, texture2D, borders[3], dwellingSize, leftSideOffset, topSideOffset + corners[0].height);
		Color pixel = body.GetPixel(0, 0);
		float r = pixel.r;
		Color pixel2 = body.GetPixel(0, 0);
		float g = pixel2.g;
		Color pixel3 = body.GetPixel(0, 0);
		float b = pixel3.b;
		Color pixel4 = body.GetPixel(0, 0);
		Color color = new Color(r, g, b, pixel4.a);
		for (int i = 0; i < (int)dwellingSize.x; i++)
		{
			for (int j = 0; j < (int)dwellingSize.y; j++)
			{
				int num = (!horizontalFlip) ? (leftSideOffset + borders[3].width + i) : (texture2D.width - 1 - (leftSideOffset + borders[3].width + i));
				int num2 = (!verticalFlip) ? (texture2D.height - 1 - (topSideOffset + corners[0].height + j)) : (topSideOffset + corners[0].height + j);
				int num3 = num2 * theTotalWidth;
				int num4 = num;
				currentColorEntry[num3 + num4] = color;
			}
		}
		BorderPixelBlit(false, texture2D, borders[1], dwellingSize, leftSideOffset + maxLeftMargin + (int)dwellingSize.x, topSideOffset + maxTopMargin);
		TextureCopy(texture2D, corners[3], 0, maxTopMargin + (int)dwellingSize.y);
		BorderPixelBlit(true, texture2D, borders[2], dwellingSize, leftSideOffset + corners[3].width, topSideOffset + maxTopMargin + (int)dwellingSize.y);
		TextureCopy(texture2D, corners[2], maxLeftMargin + (int)dwellingSize.x, maxTopMargin + (int)dwellingSize.y);
		return texture2D;
	}

	private Color[] getCachedColorMap(int height, int width)
	{
		if (!masterColorCache.ContainsKey(theTotalHeight * theTotalWidth))
		{
			masterColorCache[theTotalHeight * theTotalWidth] = new Color[theTotalHeight * theTotalWidth];
		}
		return masterColorCache[theTotalWidth * theTotalHeight];
	}

	private Texture2D getCachedTexture(int width, int height)
	{
		if (!masterTextureCache.ContainsKey(new KeyValuePair<int, int>(width, height)))
		{
			masterTextureCache[new KeyValuePair<int, int>(width, height)] = new Texture2D(width, height);
		}
		return masterTextureCache[new KeyValuePair<int, int>(width, height)];
	}

	public void CreateNineSliceTexture(Vector2 dwellingSize, Texture2D body, Texture2D[] corners, Texture2D[] borders)
	{
		GetAllMarginData(corners, borders);
		GatherAllTextureMetricInfo(corners, borders, null);
		theTotalWidth = maxLeftMargin + maxRightMargin + (int)dwellingSize.x;
		theTotalHeight = maxTopMargin + maxBottomMargin + (int)dwellingSize.y;
		currentColorEntry = getCachedColorMap(theTotalHeight, theTotalWidth);
		texture = CreateBaseTexture(dwellingSize, body, corners, borders, theTotalWidth, theTotalHeight);
		if (!delayedPixelApply)
		{
			texture.SetPixels(currentColorEntry);
			texture.Apply();
			if (compressionType == CompressionType.Compressed)
			{
				texture.Compress(true);
			}
		}
	}

	private void GetAllMarginData(Texture2D[] corners, Texture2D[] borders)
	{
		if (maxTopMargin < borders[0].height)
		{
			maxTopMargin = borders[0].height;
		}
		if (maxTopMargin < corners[1].height)
		{
			maxTopMargin = corners[1].height;
		}
		if (maxBottomMargin < borders[2].height)
		{
			maxBottomMargin = borders[2].height;
		}
		if (maxBottomMargin < corners[3].height)
		{
			maxBottomMargin = corners[3].height;
		}
		if (maxRightMargin < borders[1].width)
		{
			maxRightMargin = borders[1].width;
		}
		if (maxRightMargin < corners[2].width)
		{
			maxRightMargin = corners[2].width;
		}
		if (maxLeftMargin < borders[3].width)
		{
			maxLeftMargin = borders[3].width;
		}
		if (maxLeftMargin < corners[3].width)
		{
			maxLeftMargin = corners[3].width;
		}
	}

	private void GatherAllTextureMetricInfo(Texture2D[] corners, Texture2D[] borders, Texture2D attachmentTexture)
	{
		topLeftCornerSize = new Vector2(corners[0].width, corners[0].height);
		topRightCornerSize = new Vector2(corners[1].width, corners[1].height);
		bottomRightCornerSize = new Vector2(corners[2].width, corners[2].height);
		bottomLeftCornerSize = new Vector2(corners[3].width, corners[3].height);
		topBorderSize = new Vector2(borders[0].width, borders[0].height);
		rightBorderSize = new Vector2(borders[1].width, borders[1].height);
		bottomBorderSize = new Vector2(borders[2].width, borders[2].height);
		leftBorderSize = new Vector2(borders[3].width, borders[3].height);
		if (attachmentTexture != null)
		{
			attachmentSize = new Vector2(attachmentTexture.width, attachmentTexture.height);
		}
	}

	public void CreateNineSliceTexture(Vector2 dwellingSize, Texture2D body, Texture2D[] corners, Texture2D[] borders, Texture2D attachmentPiece, Side side, SideAlignment sideAlignment)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = 0;
		int num7 = 0;
		BorderPiece borderPiece = default(BorderPiece);
		GetAllMarginData(corners, borders);
		GatherAllTextureMetricInfo(corners, borders, attachmentPiece);
		theTotalWidth = maxLeftMargin + maxRightMargin + (int)dwellingSize.x;
		theTotalHeight = maxTopMargin + maxBottomMargin + (int)dwellingSize.y;
		switch (side)
		{
		case Side.Bottom:
			theTotalHeight += attachmentPiece.height;
			break;
		case Side.Right:
			theTotalWidth += attachmentPiece.width;
			break;
		case Side.Left:
			theTotalWidth += attachmentPiece.width;
			leftSideOffset = attachmentPiece.width;
			break;
		case Side.Top:
			theTotalHeight += attachmentPiece.height;
			topSideOffset = attachmentPiece.height;
			break;
		}
		if (side == Side.Top || side == Side.Bottom)
		{
			num = (corners[0].width * 2 + (int)dwellingSize.x) / 2 - attachmentPiece.width / 2;
			if (dwellingSize.x < (float)attachmentPiece.width)
			{
				theTotalWidth += attachmentPiece.width - (int)dwellingSize.x;
				dwellingSize.x = attachmentPiece.width;
			}
		}
		else
		{
			num = (corners[0].height * 2 + (int)dwellingSize.y) / 2 - attachmentPiece.height / 2;
			if (dwellingSize.y < (float)attachmentPiece.height)
			{
				theTotalHeight += attachmentPiece.height - (int)dwellingSize.y;
				dwellingSize.y = attachmentPiece.height;
			}
		}
		currentColorEntry = getCachedColorMap(theTotalHeight, theTotalWidth);
		texture = CreateBaseTexture(dwellingSize, body, corners, borders, theTotalWidth, theTotalHeight);
		num4 = ((side == Side.Bottom) ? (corners[0].height + corners[3].height + (int)dwellingSize.y) : 0);
		num5 = ((side == Side.Right) ? (corners[0].width + corners[1].width + (int)dwellingSize.x) : 0);
		for (int i = 0; i < theTotalWidth; i++)
		{
			for (int j = 0; j < attachmentPiece.height; j++)
			{
				num7 = ((!verticalFlip) ? (texture.height - 1 - (num4 + j)) : (num4 + j));
				num6 = ((!horizontalFlip) ? (num5 + i) : (texture.width - 1 - (num5 + i)));
				num2 = num7 * theTotalWidth;
				num3 = num6;
				currentColorEntry[num2 + num3] = new Color(0f, 0f, 0f, 0f);
			}
		}
		switch (side)
		{
		case Side.Top:
		case Side.Bottom:
			switch (sideAlignment)
			{
			case SideAlignment.Middle:
				borderPiece.hOffset = num + attachmentHOffset;
				borderPiece.hTotal = attachmentPiece.width;
				borderPiece.vOffset = ((side != Side.Bottom) ? (0 + attachmentVOffset) : (corners[0].height + corners[3].height + (int)dwellingSize.y + attachmentVOffset));
				borderPiece.vTotal = attachmentPiece.height;
				break;
			case SideAlignment.Left:
				borderPiece.hOffset = 0 + attachmentHOffset;
				borderPiece.hTotal = attachmentPiece.width;
				borderPiece.vOffset = ((side != Side.Bottom) ? (0 + attachmentVOffset) : (corners[0].height + corners[3].height + (int)dwellingSize.y + attachmentVOffset));
				borderPiece.vTotal = attachmentPiece.height;
				break;
			case SideAlignment.Right:
				borderPiece.hOffset = theTotalWidth - attachmentPiece.width + attachmentHOffset;
				borderPiece.hTotal = attachmentPiece.width;
				borderPiece.vOffset = ((side != Side.Bottom) ? (0 + attachmentVOffset) : (corners[0].height + corners[3].height + (int)dwellingSize.y + attachmentVOffset));
				borderPiece.vTotal = attachmentPiece.height;
				break;
			}
			break;
		case Side.Left:
		case Side.Right:
			switch (sideAlignment)
			{
			case SideAlignment.Middle:
				borderPiece.hOffset = ((side != Side.Right) ? (0 + attachmentHOffset) : (corners[0].width + corners[1].width + (int)dwellingSize.x + attachmentHOffset));
				borderPiece.hTotal = attachmentPiece.width;
				borderPiece.vOffset = num + attachmentVOffset;
				borderPiece.vTotal = attachmentPiece.height;
				break;
			case SideAlignment.Top:
				borderPiece.hOffset = ((side != Side.Right) ? (0 + attachmentHOffset) : (corners[0].width + corners[1].width + (int)dwellingSize.x + attachmentHOffset));
				borderPiece.hTotal = attachmentPiece.width;
				borderPiece.vOffset = 0 + attachmentVOffset;
				borderPiece.vTotal = attachmentPiece.height;
				break;
			case SideAlignment.Bottom:
				borderPiece.hOffset = ((side != Side.Right) ? (0 + attachmentHOffset) : (corners[0].width + corners[1].width + (int)dwellingSize.x + attachmentHOffset));
				borderPiece.hTotal = attachmentPiece.width;
				borderPiece.vOffset = 0 + attachmentVOffset;
				borderPiece.vTotal = (theTotalHeight - attachmentPiece.height) / 2;
				break;
			}
			break;
		}
		for (int k = 0; k < borderPiece.hTotal; k++)
		{
			for (int l = 0; l < borderPiece.vTotal; l++)
			{
				num7 = ((!verticalFlip) ? (texture.height - 1 - (borderPiece.vOffset + l)) : (borderPiece.vOffset + l));
				num6 = ((!horizontalFlip) ? (borderPiece.hOffset + k) : (texture.width - 1 - (borderPiece.hOffset + k)));
				num2 = num7 * theTotalWidth;
				num3 = num6;
				currentColorEntry[num2 + num3] = attachmentPiece.GetPixel(k, attachmentPiece.height - 1 - l);
			}
		}
		if (texture != null && !delayedPixelApply)
		{
			texture.SetPixels(currentColorEntry);
			texture.Apply();
			if (compressionType == CompressionType.Compressed)
			{
				texture.Compress(true);
			}
		}
	}

	public void CompositeTexture(Texture2D compositeTexture, int xOffset, int yOffset)
	{
		Color colorMask = new Color(-1f, -1f, -1f, -1f);
		CompositeTexture(compositeTexture, colorMask, xOffset, yOffset);
	}

	public void CompositeTexture(Texture2D compositeTexture, Color colorMask, int xOffset, int yOffset)
	{
		int num = 0;
		int num2 = 0;
		int width = compositeTexture.width;
		int height = compositeTexture.height;
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				num2 = xOffset + i;
				num = (texture.height - 1 - (yOffset + j)) * texture.width;
				Color pixel = compositeTexture.GetPixel(i, height - 1 - j);
				int num3 = (int)(pixel.r * 255f);
				int num4 = (int)(pixel.g * 255f);
				int num5 = (int)(pixel.b * 255f);
				int num6 = (int)(pixel.a * 255f);
				int num7 = (int)(colorMask.r * 255f);
				int num8 = (int)(colorMask.g * 255f);
				int num9 = (int)(colorMask.b * 255f);
				int num10 = (int)(colorMask.a * 255f);
				if (num3 != num7 && num4 != num8 && num5 != num9 && num10 != num6)
				{
					currentColorEntry[num + num2] = pixel;
				}
			}
		}
	}

	public void ManualPixelApply()
	{
		if (delayedPixelApply)
		{
			texture.SetPixels(currentColorEntry);
			texture.Apply();
			if (compressionType == CompressionType.Compressed)
			{
				texture.Compress(true);
			}
		}
		else
		{
			CspUtils.DebugLog("Trying to perfom a manual pixel apply operation without setting the 'DelayedPixelApply' flag.");
		}
	}

	public void ReleaseNineSliceTexture()
	{
		texture = null;
	}
}
