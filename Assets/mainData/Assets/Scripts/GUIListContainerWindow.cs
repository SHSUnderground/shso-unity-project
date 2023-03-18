using System;
using System.Collections.Generic;

public class GUIListContainerWindow
{
}
public class GUIListContainerWindow<T> : GUIWindow where T : GUIListItem
{
	public delegate void SelectionChangedDelegate(GUIListItem item, GUIListContainerWindow<T> container);

	public delegate bool FilterDelegate(T evalObject);

	private List<T> itemList;

	private T selectedItem;

	public FilterDelegate Filter;

	public T SelectedItem
	{
		get
		{
			return selectedItem;
		}
		set
		{
			if (selectedItem != null)
			{
				selectedItem.IsSelected = false;
				AppShell.Instance.EventMgr.Fire(this, new ItemDeselectedMessage(selectedItem.ItemId));
			}
			if (value != null)
			{
				AppShell.Instance.EventMgr.Fire(this, new ItemSelectedMessage(value.ItemId));
				value.IsSelected = true;
			}
			selectedItem = value;
		}
	}

	public virtual List<T> ItemList
	{
		get
		{
			if (Filter == null)
			{
				return itemList;
			}
			Predicate<T> match = Filter.Invoke;
			return itemList.FindAll(match);
		}
	}

	public event SelectionChangedDelegate SelectionChanged;

	public GUIListContainerWindow()
	{
		itemList = new List<T>();
	}

	public virtual void AddItem(T item)
	{
		itemList.Add(item);
		item.IsVisible = true;
		item.MouseOver += ItemMouseOver;
		item.MouseOut += ItemMouseOut;
		item.Click += ItemClicked;
	}

	public virtual void RemoveItem(T item)
	{
		itemList.Remove(item);
		item.MouseOver -= ItemMouseOver;
		item.MouseOut -= ItemMouseOut;
		item.Click -= ItemClicked;
	}

	public virtual void RemoveItem(int index)
	{
		if (index > -1 && index < itemList.Count)
		{
			itemList.Remove(itemList[index]);
		}
	}

	public override void Clear()
	{
		foreach (T item in itemList)
		{
			T current = item;
			current.MouseOver -= ItemMouseOver;
			current.MouseOut -= ItemMouseOut;
			current.Click -= ItemClicked;
		}
		itemList.Clear();
	}

	protected virtual void ItemClicked(GUIControl sender, GUIClickEvent eventArgs)
	{
		SelectedItem = (T)sender;
		if (this.SelectionChanged != null)
		{
			this.SelectionChanged((T)sender, this);
		}
	}

	public override void OnHide()
	{
		SelectedItem = (T)null;
		base.OnHide();
	}

	public override bool IsDescendantHandler(IInputHandler handler)
	{
		if (this == handler)
		{
			return true;
		}
		foreach (T item in itemList)
		{
			if (((IInputHandler)item).IsDescendantHandler(handler))
			{
				return true;
			}
		}
		return false;
	}

	protected virtual void ItemMouseOver(GUIControl sender, GUIMouseEvent eventArgs)
	{
		ShsEventMgr eventMgr = AppShell.Instance.EventMgr;
		T val = (T)sender;
		eventMgr.Fire(this, new ItemPreviewMessage(val.ItemId, ItemPreviewMessage.ItemPreviewMessageStateEnum.Activated));
	}

	protected virtual void ItemMouseOut(GUIControl sender, GUIMouseEvent eventArgs)
	{
		ShsEventMgr eventMgr = AppShell.Instance.EventMgr;
		T val = (T)sender;
		eventMgr.Fire(this, new ItemPreviewMessage(val.ItemId, ItemPreviewMessage.ItemPreviewMessageStateEnum.Deactivated));
	}
}
