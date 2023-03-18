using System;
using System.Collections.Generic;
using UnityEngine;

public class GUIListViewWindow<T> : GUIListContainerWindow<T> where T : GUIListItem
{
	public enum ListViewOrientationEnum
	{
		Horizontal,
		Vertical
	}

	private GUISlider slider;

	private Texture2D backgroundTexture;

	private int listWidth;

	private bool autoListSize = true;

	private int listHeight;

	private int listPaddingX;

	private int listPaddingY;

	private int listItemWidth = 64;

	private int listItemHeight = 64;

	private int currentSet;

	private ListViewOrientationEnum orientation = ListViewOrientationEnum.Vertical;

	private bool RecalcFlag;

	private float lastSliderValue;

	private float setOffset;

	public GUISlider Slider
	{
		get
		{
			return slider;
		}
		set
		{
			slider = value;
		}
	}

	public Texture2D BackgroundTexture
	{
		get
		{
			return backgroundTexture;
		}
		set
		{
			backgroundTexture = value;
		}
	}

	public int ListWidth
	{
		get
		{
			return listWidth;
		}
		set
		{
			listWidth = value;
			RecalcFlag = true;
		}
	}

	public bool AutoListSize
	{
		get
		{
			return autoListSize;
		}
		set
		{
			autoListSize = value;
			RecalcFlag = true;
		}
	}

	public int ListHeight
	{
		get
		{
			return listHeight;
		}
		set
		{
			listHeight = value;
			RecalcFlag = true;
		}
	}

	public int ListPaddingX
	{
		get
		{
			return listPaddingX;
		}
		set
		{
			listPaddingX = value;
			RecalcFlag = true;
		}
	}

	public int ListPaddingY
	{
		get
		{
			return listPaddingY;
		}
		set
		{
			listPaddingY = value;
			RecalcFlag = true;
		}
	}

	public int ListItemWidth
	{
		get
		{
			return listItemWidth;
		}
		set
		{
			listItemWidth = value;
			RecalcFlag = true;
		}
	}

	public int ListItemHeight
	{
		get
		{
			return listItemHeight;
		}
		set
		{
			listItemHeight = value;
			RecalcFlag = true;
		}
	}

	public int CurrentSet
	{
		get
		{
			return currentSet;
		}
	}

	public ListViewOrientationEnum Orientation
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

	private float totalHeight
	{
		get
		{
			if (ListWidth == 0)
			{
				return 0f;
			}
			return ItemList.Count / ListWidth * listItemHeight + (ItemList.Count / ListWidth - 1) * listPaddingY + ((ItemList.Count % ListWidth != 0) ? (listItemHeight + listPaddingY) : 0);
		}
	}

	private float totalWidth
	{
		get
		{
			if (ListHeight == 0)
			{
				return 0f;
			}
			return ItemList.Count / ListHeight * listItemWidth + (ItemList.Count / ListHeight - 1) * listPaddingX + ((ItemList.Count % ListHeight != 0) ? (listItemWidth + listPaddingX) : 0);
		}
	}

	public override void DrawPreprocess()
	{
		base.DrawPreprocess();
	}

	public override void OnUpdate()
	{
		if (RecalcFlag)
		{
			CalculateSize();
		}
		if (slider != null)
		{
			int num = Convert.ToInt32(slider.Value);
			if ((float)num != lastSliderValue)
			{
				int num2 = (Orientation != ListViewOrientationEnum.Vertical) ? ListHeight : ListWidth;
				float num3 = (Orientation != ListViewOrientationEnum.Vertical) ? totalWidth : totalHeight;
				int num4 = (Orientation != ListViewOrientationEnum.Vertical) ? listItemWidth : listItemHeight;
				int num5 = (Orientation != ListViewOrientationEnum.Vertical) ? listPaddingX : listPaddingY;
				currentSet = Convert.ToInt32(Math.Floor((float)(num * (ItemList.Count / num2)) / num3));
				setOffset = num - currentSet * (num4 + num5);
				lastSliderValue = num;
			}
		}
		base.OnUpdate();
	}

	public override void Draw(DrawModeSetting DrawParams)
	{
		base.Draw(DrawParams);
		GUI.BeginGroup(ClientRect);
		if (Orientation == ListViewOrientationEnum.Vertical)
		{
			int num = -1;
			for (int i = 0; i <= ListHeight + 2; i++)
			{
				for (int j = 0; j < ListWidth; j++)
				{
					num = i * listWidth + j + currentSet * ListWidth;
					if (num >= ItemList.Count)
					{
						break;
					}
					T val = ItemList[num];
					val.Rect = new Rect(j * listItemWidth + j * listPaddingX, (float)(i * listItemHeight + i * listPaddingY) - setOffset, listItemWidth, listItemHeight);
					T val2 = ItemList[num];
					if (val2.IsVisible)
					{
						T val3 = ItemList[num];
						val3.DrawPreprocess();
						T val4 = ItemList[num];
						val4.Draw(DrawParams);
						T val5 = ItemList[num];
						val5.DrawFinalize();
					}
				}
			}
		}
		else
		{
			int num2 = -1;
			List<T> itemList = ItemList;
			for (int k = 0; k <= ListWidth + 2; k++)
			{
				for (int l = 0; l < ListHeight; l++)
				{
					num2 = k * listHeight + l + currentSet * ListHeight;
					if (num2 >= itemList.Count)
					{
						break;
					}
					T val6 = itemList[num2];
					val6.Rect = new Rect((float)(k * listItemHeight + k * listPaddingY) - setOffset, l * listItemWidth + l * listPaddingX, listItemWidth, listItemHeight);
					T val7 = itemList[num2];
					if (val7.IsVisible)
					{
						T val8 = itemList[num2];
						val8.DrawPreprocess();
						T val9 = itemList[num2];
						val9.Draw(DrawParams);
						T val10 = itemList[num2];
						val10.DrawFinalize();
					}
				}
			}
		}
		GUI.EndGroup();
	}

	public override void DrawFinalize()
	{
		base.DrawFinalize();
	}

	public override void AddItem(T item)
	{
		base.AddItem(item);
		CalculateSize();
	}

	public override void RemoveItem(T item)
	{
		base.RemoveItem(item);
		CalculateSize();
	}

	public override void RemoveItem(int index)
	{
		base.RemoveItem(index);
		CalculateSize();
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
		CalculateSize();
	}

	private void CalculateSize()
	{
		if (AutoListSize)
		{
			Vector2 rectSize = base.RectSize;
			int num = Convert.ToInt32(rectSize.x - base.PaddingLeft - base.PaddingRight);
			Vector2 rectSize2 = base.RectSize;
			int num2 = Convert.ToInt32(rectSize2.y - base.PaddingTop - base.PaddingBottom);
			bool flag = false;
			if (listItemWidth == 0)
			{
				ListWidth = 0;
				flag = true;
			}
			if (listItemHeight == 0)
			{
				ListHeight = 0;
				flag = true;
			}
			if (!flag)
			{
				ListWidth = Math.Max(1, (num + listPaddingX) / (listItemWidth + listPaddingX));
				ListHeight = Math.Max(1, (num2 + listPaddingY) / (listItemHeight + listPaddingY));
			}
		}
		if (slider != null)
		{
			slider.Min = 0f;
			slider.Max = ((Orientation == ListViewOrientationEnum.Vertical) ? ((!(totalHeight < ClientRect.height)) ? (totalHeight - ClientRect.height) : 0f) : ((!(totalWidth < ClientRect.width)) ? (totalWidth - ClientRect.width) : 0f));
		}
		RecalcFlag = false;
	}
}
