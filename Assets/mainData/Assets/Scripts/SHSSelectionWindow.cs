using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class SHSSelectionWindow<T, C> : GUISimpleControlWindow where T : SHSSelectionItem<C> where C : GUIControl
{
	public enum SelectionWindowType
	{
		ThreeAcross,
		OneAcross
	}

	public enum SelectionWindowDirection
	{
		Horizontal,
		Vertical
	}

	private class ItemContainer
	{
		private class FadeGUIImage : GUIImage
		{
			private float FadeAlpha = 1f;

			private float TrueAlpha = 1f;

			private bool UpdateFadeCall;

			public override float Alpha
			{
				get
				{
					return TrueAlpha;
				}
				set
				{
					if (!UpdateFadeCall)
					{
						TrueAlpha = value;
					}
					else
					{
						FadeAlpha = value;
					}
					base.Alpha = TrueAlpha * FadeAlpha;
				}
			}

			public void SetFadeAlpha(float i)
			{
				UpdateFadeCall = true;
				Alpha = i;
				UpdateFadeCall = false;
			}
		}

		private SHSSelectionWindow<T, C> headWindow;

		public SHSSelectionItem<C> heldItem;

		private FadeGUIImage backgroundImage;

		private GetBackgroundLocation getBackgroundLocation;

		public ItemContainer(SHSSelectionWindow<T, C> headWindow, GUISimpleControlWindow window, Vector2 BackIconSize, GetBackgroundLocation getBackgroundLocation)
		{
			this.headWindow = headWindow;
			this.getBackgroundLocation = getBackgroundLocation;
			backgroundImage = new FadeGUIImage();
			this.SetBackgroundImage(SHSSelectionItem<C>.SelectionState.Passive, false);
			backgroundImage.SetSize(BackIconSize);
			window.Add(backgroundImage);
		}

		public void SetupPosition(float x, float y, bool oddInSequence)
		{
			backgroundImage.SetPosition(new Vector2(x, y), DockingAlignmentEnum.None, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
			backgroundImage.IsVisible = true;
			if (heldItem != null)
			{
				heldItem.item.SetPosition(new Vector2(x, y), DockingAlignmentEnum.None, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
				heldItem.OddInSequence = oddInSequence;
				SetBackgroundImage(heldItem.currentState, oddInSequence);
				backgroundImage.SetFadeAlpha(1f);
			}
			else
			{
				this.SetBackgroundImage(SHSSelectionItem<C>.SelectionState.Passive, oddInSequence);
				backgroundImage.SetFadeAlpha(headWindow.EmptyBackgroundAlpha);
			}
		}

		public void DisableBackgroundVisibility()
		{
			backgroundImage.IsVisible = false;
		}

		public void VisibilityCheck(float maxXY, SHSSelectionWindow<T, C> headWindow)
		{
			if (headWindow.DirOf(backgroundImage.Position) + headWindow.DirOf(backgroundImage.Size) * 0.5f > maxXY)
			{
				backgroundImage.IsVisible = false;
			}
		}

		private void SetBackgroundImage(SHSSelectionItem<C>.SelectionState selectionState, bool oddInSequence)
		{
			if (getBackgroundLocation != null)
			{
				backgroundImage.TextureSource = getBackgroundLocation(selectionState, oddInSequence);
			}
		}
	}

	public delegate string GetBackgroundLocation(SHSSelectionItem<C>.SelectionState selectionState, bool oddInSequence);

	public List<T> items;

	private List<ItemContainer> buttons;

	public Vector2 ItemSize = new Vector2(70f, 70f);

	public float TopOffsetAdjustHeight = 14f;

	public float BottomOffsetAdjustHeight = 23f;

	private GUISimpleControlWindow contentWindow;

	private GUISlider slider;

	public bool RequestRefresh;

	private SelectionWindowDirection windowDirection = SelectionWindowDirection.Vertical;

	private float emptyBackgroundAlpha = 0.33f;

	public GUISlider Slider
	{
		get
		{
			return slider;
		}
	}

	public SelectionWindowDirection WindowDirection
	{
		get
		{
			return windowDirection;
		}
		set
		{
			windowDirection = value;
			RequestRefresh = true;
		}
	}

	public float EmptyBackgroundAlpha
	{
		get
		{
			return emptyBackgroundAlpha;
		}
		set
		{
			emptyBackgroundAlpha = value;
			RequestRefresh = true;
		}
	}

	public SHSSelectionWindow(GUISlider slider, float contentWindowWidth, Vector2 ItemSize, int numberOfButtons)
		: this(slider, contentWindowWidth, ItemSize, numberOfButtons, (GetBackgroundLocation)null)
	{
	}

	public SHSSelectionWindow(GUISlider slider, SelectionWindowType type)
		: this(slider, 227f, GetItemSizeOnType(type), GetBackIconSizeOnType(type), GetNumberOfButtonsOnType(type), GetBackgroundLocationOnType(type))
	{
	}

	public SHSSelectionWindow(GUISlider slider, float contentWindowWidth, Vector2 ItemSize, int numberOfButtons, GetBackgroundLocation imageLocation)
		: this(slider, contentWindowWidth, ItemSize, ItemSize, numberOfButtons, imageLocation)
	{
	}

	public SHSSelectionWindow(GUISlider slider, float contentWindowWidth, Vector2 ItemSize, Vector2 BackIconSize, int numberOfButtons, GetBackgroundLocation imageLocation)
	{
		this.ItemSize = ItemSize;
		Traits.UpdateTrait = ControlTraits.UpdateTraitEnum.VisibleAndEnabled;
		items = new List<T>();
		contentWindow = new GUISimpleControlWindow();
		contentWindow.SetSize(contentWindowWidth, contentWindowWidth);
		contentWindow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f));
		Add(contentWindow);
		buttons = new List<ItemContainer>();
		for (int i = 0; i < numberOfButtons; i++)
		{
			ItemContainer item = new ItemContainer(this, contentWindow, BackIconSize, imageLocation);
			buttons.Add(item);
		}
		this.slider = slider;
		slider.Changed += slider_Changed;
		slider.UseMouseWheelScroll = true;
		slider.MouseScrollWheelAmount = 40f;
		slider.TickValue = 40f;
		slider.ArrowsEnabled = true;
		slider_Changed(null, null);
	}

	private static Vector2 GetItemSizeOnType(SelectionWindowType type)
	{
		switch (type)
		{
		case SelectionWindowType.OneAcross:
			return new Vector2(206f, 47f);
		case SelectionWindowType.ThreeAcross:
			return new Vector2(70f, 70f);
		default:
			return new Vector2(70f, 70f);
		}
	}

	private static Vector2 GetBackIconSizeOnType(SelectionWindowType type)
	{
		switch (type)
		{
		case SelectionWindowType.OneAcross:
			return new Vector2(206f, 47f);
		case SelectionWindowType.ThreeAcross:
			return new Vector2(128f, 128f);
		default:
			return new Vector2(70f, 70f);
		}
	}

	private static int GetNumberOfButtonsOnType(SelectionWindowType type)
	{
		switch (type)
		{
		case SelectionWindowType.OneAcross:
			return 8;
		case SelectionWindowType.ThreeAcross:
			return 21;
		default:
			return 21;
		}
	}

	private static GetBackgroundLocation GetBackgroundLocationOnType(SelectionWindowType type)
	{
		switch (type)
		{
		case SelectionWindowType.OneAcross:
			return GetBackgroundLocationOneAcross;
		case SelectionWindowType.ThreeAcross:
			return GetBackgroundLocationThreeAcross;
		default:
			return GetBackgroundLocationThreeAcross;
		}
	}

	public static string GetBackgroundLocationOneAcross(SHSSelectionItem<C>.SelectionState selectionState, bool oddInSequence)
	{
		string str = (!oddInSequence) ? "2" : "1";
		switch (selectionState)
		{
		case SHSSelectionItem<C>.SelectionState.Active:
			return "persistent_bundle|friends_list_module_on" + str;
		case SHSSelectionItem<C>.SelectionState.Passive:
			return "persistent_bundle|friends_list_module_off" + str;
		case SHSSelectionItem<C>.SelectionState.Selected:
			return "persistent_bundle|friends_list_module_selected" + str;
		case SHSSelectionItem<C>.SelectionState.Special:
			return "persistent_bundle|friends_list_module_selected" + str;
		default:
			return string.Empty;
		}
	}

	public static string GetBackgroundLocationThreeAcross(SHSSelectionItem<C>.SelectionState selectionState, bool oddInSequence)
	{
		switch (selectionState)
		{
		case SHSSelectionItem<C>.SelectionState.Active:
			return "persistent_bundle|inventory_iconback";
		case SHSSelectionItem<C>.SelectionState.Passive:
			return "persistent_bundle|inventory_iconback";
		case SHSSelectionItem<C>.SelectionState.Selected:
			return "persistent_bundle|inventory_iconback";
		case SHSSelectionItem<C>.SelectionState.Special:
			return "mysquadgadget_bundle|mysquad_inventoryBG_locked";
		case SHSSelectionItem<C>.SelectionState.Subscription:
			return "persistent_bundle|inventory_item_subscriber_backdrop";
		case SHSSelectionItem<C>.SelectionState.Highlighted:
			return "persistent_bundle|inventory_iconback_highlight";
		default:
			return string.Empty;
		}
	}

	public List<C> GetVisibleItemControls()
	{
		List<C> list = new List<C>();
		List<T> visibleItems = GetVisibleItems();
		foreach (T item in visibleItems)
		{
			list.Add(item.item);
		}
		return list;
	}

	public List<T> GetVisibleItems()
	{
		return GetVisibleItems(true);
	}

	public List<T> GetVisibleItems(bool includePartiallyVisible)
	{
		List<T> list = new List<T>();
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		foreach (T item in items)
		{
			int num4 = (!includePartiallyVisible) ? num2 : (num2 + 1);
			int num5 = (!includePartiallyVisible) ? (num2 + 1) : num2;
			if (item.active)
			{
				if (DirOf(ItemSize) * (float)num4 >= 0f - DirOf(contentWindow.Offset) && DirOf(ItemSize) * (float)num5 <= 0f - DirOf(contentWindow.Offset) + DirOf(Size))
				{
					list.Add(item);
					num3++;
				}
				num++;
				if ((float)(num + 1) * DirAgainst(ItemSize) > DirAgainst(contentWindow.Size))
				{
					num = 0;
					num2++;
				}
			}
		}
		return list;
	}

	public void AddItem(T item)
	{
		items.Add(item);
		item.item.SetSize(item.itemSize);
		contentWindow.Add(item.item);
		RequestRefresh = true;
	}

	public void AddList(List<T> itemList)
	{
		foreach (T item in itemList)
		{
			AddItem(item);
		}
	}

	public void RemoveItem(T item)
	{
		items.Remove(item);
		contentWindow.Remove(item.item);
		RequestRefresh = true;
	}

	public void ClearItems()
	{
		foreach (T item in items)
		{
			contentWindow.Remove(item.item);
		}
		items.Clear();
		RequestRefresh = true;
	}

	public T Find(Predicate<T> predicate)
	{
		return items.Find(predicate);
	}

	public void SortItemList()
	{
		items.Sort();
		RequestRefresh = true;
	}

	public override void OnShow()
	{
		base.OnShow();
		RequestRefresh = true;
	}

	private void slider_Changed(GUIControl sender, GUIChangedEvent eventData)
	{
		if (WindowDirection == SelectionWindowDirection.Vertical)
		{
			contentWindow.Offset = new Vector2(0f, 0f - slider.Value + TopOffsetAdjustHeight);
		}
		else
		{
			contentWindow.Offset = new Vector2(0f - slider.Value + TopOffsetAdjustHeight, 0f);
		}
		RequestRefresh = true;
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		foreach (T item in items)
		{
			if (item != null && item.RequestRefresh)
			{
				RequestRefresh = true;
				item.RequestRefresh = false;
			}
		}
		if (RequestRefresh)
		{
			RequestRefresh = false;
			UpdateDisplay();
		}
	}

	public void RequestARefresh()
	{
		RequestRefresh = true;
	}

	public void UpdateDisplay()
	{
		float num = 0f;
		float num2 = 0f;
		float num3 = 0f;
		int i = 0;
		bool flag = false;
		foreach (T item in items)
		{
			if (item.active)
			{
				flag = !flag;
				if (DirOf(ItemSize) * (num2 + 1f) >= 0f - DirOf(contentWindow.Offset) && i < buttons.Count)
				{
					item.item.IsVisible = true;
					buttons[i].heldItem = item;
					if (WindowDirection == SelectionWindowDirection.Vertical)
					{
						buttons[i].SetupPosition((num + 0.5f) * ItemSize.x, (num2 + 0.5f) * ItemSize.y, flag);
					}
					else
					{
						buttons[i].SetupPosition((num2 + 0.5f) * ItemSize.x, (num + 0.5f) * ItemSize.y, flag);
					}
					i++;
				}
				else
				{
					item.item.IsVisible = false;
				}
				num += 1f;
				if ((num + 1f) * DirAgainst(ItemSize) > DirAgainst(contentWindow.Size))
				{
					num = 0f;
					num2 += 1f;
				}
			}
			else
			{
				item.item.IsVisible = false;
			}
		}
		num3 = num2;
		if (num == 0f)
		{
			num3 -= 1f;
		}
		flag = !flag;
		num3 += 1f;
		float num4 = Mathf.Floor((DirOf(Size) - TopOffsetAdjustHeight - BottomOffsetAdjustHeight) / DirOf(ItemSize));
		if (num3 < num4)
		{
			num3 = num4;
		}
		if (WindowDirection == SelectionWindowDirection.Vertical)
		{
			GUISimpleControlWindow gUISimpleControlWindow = contentWindow;
			Vector2 size = contentWindow.Size;
			gUISimpleControlWindow.SetSize(size.x, (num3 + 1f) * ItemSize.y);
		}
		else
		{
			GUISimpleControlWindow gUISimpleControlWindow2 = contentWindow;
			float width = (num3 + 1f) * ItemSize.x;
			Vector2 size2 = contentWindow.Size;
			gUISimpleControlWindow2.SetSize(width, size2.y);
		}
		slider.Max = Mathf.Max(num3 * DirOf(ItemSize) - DirOf(Size) + TopOffsetAdjustHeight + BottomOffsetAdjustHeight, 0f);
		slider.IsVisible = (slider.Max != slider.Min);
		float num5 = num2;
		if (num == 0f)
		{
			num5 -= 1f;
		}
		for (; i < buttons.Count; i++)
		{
			ItemContainer itemContainer = buttons[i];
			itemContainer.heldItem = null;
			if (WindowDirection == SelectionWindowDirection.Vertical)
			{
				itemContainer.SetupPosition((num + 0.5f) * ItemSize.x, (num2 + 0.5f) * ItemSize.y, flag);
			}
			else
			{
				itemContainer.SetupPosition((num2 + 0.5f) * ItemSize.x, (num + 0.5f) * ItemSize.y, flag);
			}
			if (slider.Max == 0f)
			{
				itemContainer.VisibilityCheck(DirOf(contentWindow.Size), this);
			}
			else if (num2 != num5)
			{
				itemContainer.DisableBackgroundVisibility();
			}
			flag = !flag;
			num += 1f;
			if ((num + 1f) * DirAgainst(ItemSize) > DirAgainst(contentWindow.Size))
			{
				num = 0f;
				num2 += 1f;
			}
		}
	}

	private float DirOf(Vector2 value)
	{
		if (WindowDirection == SelectionWindowDirection.Vertical)
		{
			return value.y;
		}
		return value.x;
	}

	private float DirAgainst(Vector2 value)
	{
		if (WindowDirection == SelectionWindowDirection.Vertical)
		{
			return value.x;
		}
		return value.y;
	}
}
