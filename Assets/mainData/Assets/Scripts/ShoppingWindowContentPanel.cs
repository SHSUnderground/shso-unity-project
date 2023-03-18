using System.Collections.Generic;
using UnityEngine;

public class ShoppingWindowContentPanel : GUIDialogWindow
{
	private GUIImage _background;

	private GUISimpleControlWindow _scrollPaneMask;

	private GUISimpleControlWindow _scrollPane;

	private GUISlider _slider;

	private int deltY = 247;

	private int maxY;

	private ShoppingWindow parentWindow;

	public CatalogItem firstItem;

	private List<CatalogItemWindow> _windowList = new List<CatalogItemWindow>();

	private List<ShoppingTabButton> _tabs = new List<ShoppingTabButton>();

	private GUISimpleControlWindow _inactiveTabs;

	private GUISimpleControlWindow _activeTab;

	private GUIImage _tabEmptyRight;

	private GUIImage _tabEmptyLeft;

	public ShoppingWindowContentPanel(ShoppingWindow parent)
	{
		parentWindow = parent;
		_inactiveTabs = new GUISimpleControlWindow();
		_inactiveTabs.SetSize(552f, 100f);
		_inactiveTabs.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(23f, 0f));
		_inactiveTabs.IsVisible = true;
		Add(_inactiveTabs);
		_background = new GUIImage();
		_background.SetSize(new Vector2(598f, 535f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_background.Position = new Vector2(0f, 39f);
		_background.TextureSource = "shopping_bundle|centerpanel";
		_background.Id = "Background";
		Add(_background);
		_activeTab = new GUISimpleControlWindow();
		GUISimpleControlWindow activeTab = _activeTab;
		Vector2 size = _background.Size;
		activeTab.SetSize(size.x - 46f, 100f);
		_activeTab.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(23f, 0f));
		_activeTab.IsVisible = true;
		Add(_activeTab);
		_tabEmptyLeft = new GUIImage();
		_tabEmptyLeft.SetSize(new Vector2(11f, 11f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_tabEmptyLeft.Position = new Vector2(0f, 39f);
		_tabEmptyLeft.TextureSource = "shopping_bundle|tab_top_empty";
		_activeTab.Add(_tabEmptyLeft);
		_tabEmptyRight = new GUIImage();
		_tabEmptyRight.SetSize(new Vector2(11f, 11f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_tabEmptyRight.Position = new Vector2(0f, 45f);
		_tabEmptyRight.TextureSource = "shopping_bundle|tab_top_empty";
		_activeTab.Add(_tabEmptyRight);
		SetSize(_background.Size + new Vector2(0f, 37f));
		_scrollPaneMask = new GUISimpleControlWindow();
		_scrollPaneMask.SetSize(598f, 534f);
		_scrollPaneMask.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(25f, 56f));
		Add(_scrollPaneMask);
		_scrollPane = new GUISimpleControlWindow();
		_scrollPane.SetSize(600f, 4000f);
		_scrollPane.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, _scrollPaneMask.Position);
		_scrollPaneMask.Add(_scrollPane);
		_slider = new GUISlider();
		_slider.Changed += slider_Changed;
		_slider.Orientation = GUISlider.SliderOrientationEnum.Vertical;
		_slider.ScrollButtonStyleInfo = new SHSButtonStyleInfo("shopping_bundle|scrollbutton", SHSButtonStyleInfo.SizeCategoryEnum.Small);
		_slider.ScrollButtonSize = new Vector2(96f, 96f);
		_slider.VerticalCapHeight = 15f;
		_slider.UseMouseWheelScroll = true;
		_slider.MouseScrollWheelAmount = 1f;
		_slider.TickValue = 1f;
		_slider.ArrowsEnabled = true;
		_slider.SetSize(40f, 515f);
		_slider.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(540f, 55f));
		Add(_slider);
		slider_Changed(null, null);
		ShoppingTabButton activeButton = addTab("All", string.Empty);
		selectTab(activeButton);
	}

	private ShoppingTabButton addTab(string tabName, string command)
	{
		ShoppingTabButton shoppingTabButton = new ShoppingTabButton(tabName, string.Empty, command);
		int num = 0;
		foreach (ShoppingTabButton tab in _tabs)
		{
			int num2 = num;
			Vector2 size = tab.Size;
			num = num2 + ((int)size.x - 10);
		}
		shoppingTabButton.SetPosition(num, 0f);
		_tabs.Add(shoppingTabButton);
		_inactiveTabs.Add(shoppingTabButton);
		return shoppingTabButton;
	}

	public void selectTab(ShoppingTabButton activeButton)
	{
		//Discarded unreachable code: IL_00f0
		foreach (ShoppingTabButton tab in _tabs)
		{
			tab.deactivate();
			_inactiveTabs.Add(tab);
		}
		_activeTab.Add(activeButton);
		activeButton.activate();
		int num = 552;
		GUIImage tabEmptyLeft = _tabEmptyLeft;
		Vector2 position = activeButton.Position;
		tabEmptyLeft.SetSize(position.x, 11f);
		GUIImage tabEmptyRight = _tabEmptyRight;
		float num2 = num;
		Vector2 position2 = activeButton.Position;
		float num3 = num2 - position2.x;
		Vector2 size = activeButton.Size;
		tabEmptyRight.SetSize(num3 + size.x, 11f);
		GUIImage tabEmptyRight2 = _tabEmptyRight;
		Vector2 position3 = activeButton.Position;
		float x = position3.x;
		Vector2 size2 = activeButton.Size;
		float x2 = x + size2.x;
		Vector2 position4 = _tabEmptyLeft.Position;
		tabEmptyRight2.SetPosition(x2, position4.y);
	}

	public static string flattenStringForSearch(string name)
	{
		name = name.ToLowerInvariant();
		name = name.Replace("_", string.Empty);
		name = name.Replace("'", string.Empty);
		name = name.Replace("-", string.Empty);
		name = name.Replace(" ", string.Empty);
		name = name.Replace(".", string.Empty);
		name = name.Replace("!", string.Empty);
		return name;
	}

	public void filterByIDs(List<int> specificIDs)
	{
		if (specificIDs != null && specificIDs.Count > 0)
		{
			maxY = 0;
			int num = 0;
			foreach (CatalogItemWindow window in _windowList)
			{
				window.IsVisible = false;
				if (specificIDs.Contains(window.item.ownableDef.ownableTypeID))
				{
					window.IsVisible = true;
					if (num % 2 == 1)
					{
						window.SetPosition(new Vector2(250f, num / 2 * deltY));
						maxY += deltY;
					}
					else
					{
						window.SetPosition(new Vector2(0f, num / 2 * deltY));
					}
					num++;
				}
			}
			_scrollPane.SetSize(600f, maxY + deltY);
			_slider.Value = 0f;
			slider_Changed(null, null);
		}
	}

	public void filterByText(string text)
	{
		maxY = 0;
		text = flattenStringForSearch(text);
		int num = 0;
		foreach (CatalogItemWindow window in _windowList)
		{
			window.IsVisible = false;
			if (flattenStringForSearch(AppShell.Instance.stringTable.GetString(window.item.name)).Contains(text) || (window.item.ownableDef.category == OwnableDefinition.Category.Hero && flattenStringForSearch(AppShell.Instance.CharacterDescriptionManager[window.item.ownableDef.name].CharacterFamily).Contains(text)))
			{
				window.IsVisible = true;
				if (num % 2 == 1)
				{
					window.SetPosition(new Vector2(250f, num / 2 * deltY));
					maxY += deltY;
				}
				else
				{
					window.SetPosition(new Vector2(0f, num / 2 * deltY));
				}
				num++;
			}
		}
		_scrollPane.SetSize(600f, maxY + deltY);
		_slider.Value = 0f;
		slider_Changed(null, null);
	}

	private void slider_Changed(GUIControl sender, GUIChangedEvent eventData)
	{
		float num = (float)maxY / (float)deltY;
		_scrollPane.Offset = new Vector2(0f, (0f - _slider.Value) * (num / 100f) * (float)deltY);
	}

	public void clear()
	{
		foreach (CatalogItemWindow window in _windowList)
		{
			window.Dispose();
		}
		_windowList.Clear();
		maxY = 0;
		_scrollPane.RemoveAllControls();
	}

	public void setMainCategory(NewShoppingManager.ShoppingCategory shoppingCategory)
	{
		CspUtils.DebugLog("setMainCategory " + shoppingCategory);
		clear();
		List<CatalogItem> list = NewShoppingManager.getList(shoppingCategory);
		if (shoppingCategory == NewShoppingManager.ShoppingCategory.Hero)
		{
			list.Sort(delegate(CatalogItem p1, CatalogItem p2)
			{
				///// this block added by CSP //////
				if (p1.name == null)
					p1.name = " ";
				if (p2.name == null)
					p2.name = " ";
				///////////////////////////////////	

				if (p1.goldPrice < p2.goldPrice)
				{
					return -1;
				}
				return (p1.goldPrice > p2.goldPrice) ? 1 : p1.name.CompareTo(p2.name);		
				
			});
		}
		else
		{
			list.Sort(delegate(CatalogItem p1, CatalogItem p2)
			{
				return (p1.displayPriority == p2.displayPriority) ? p1.name.CompareTo(p2.name) : p2.displayPriority.CompareTo(p1.displayPriority);
			});
		}
		int num = 0;
		firstItem = null;
		foreach (CatalogItem item in list)
		{
			if (firstItem == null)
			{
				firstItem = item;
			}

			///// this block added by CSP //////
			if (item.ownableDef == null) {
				CspUtils.DebugLog("item.ownableDef is null in setMainCategory");
				continue;
			}
			////////////////////////////////////

			CatalogItemWindow catalogItemWindow = new CatalogItemWindow(this, item);
			catalogItemWindow.IsVisible = true;
			if (num % 2 == 1)
			{
				catalogItemWindow.SetPosition(new Vector2(250f, num / 2 * deltY));
				maxY += deltY;
			}
			else
			{
				catalogItemWindow.SetPosition(new Vector2(0f, num / 2 * deltY));
			}
			_scrollPane.Add(catalogItemWindow);
			_windowList.Add(catalogItemWindow);
			num++;
		}
		_scrollPane.SetSize(600f, maxY + deltY);
		_slider.Value = 0f;
		slider_Changed(null, null);
	}

	public void scrollToItem(int ownableTypeID)
	{
		foreach (CatalogItemWindow window in _windowList)
		{
			if (window.item.ownableTypeID == ownableTypeID)
			{
				firstItem = window.item;
				GUISlider slider = _slider;
				Vector2 position = window.Position;
				slider.Value = position.y / (float)maxY * 100f;
				slider_Changed(null, null);
				break;
			}
		}
	}

	public void selectItem(CatalogItem item)
	{
		parentWindow.selectCatalogItem(item);
	}
}
