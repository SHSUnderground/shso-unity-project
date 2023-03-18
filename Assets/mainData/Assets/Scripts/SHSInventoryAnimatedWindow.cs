using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SHSInventoryAnimatedWindow : SHSHudWindows
{
	public enum InventoryType
	{
		InvalidInventory,
		HQInventory,
		SocialSpaceInventory
	}

	public class InvAnims : SHSAnimations
	{
		public static AnimationPieceGeneratorDelegate GetOpenAnim(List<GUIControl> bounce, List<GUIControl> fade, GUIWindow win)
		{
			return delegate
			{
				Vector2 bACKGROUND_SIZE = BACKGROUND_SIZE;
				AnimClip pieceOne = Absolute.SizeX(GenericPaths.BounceTransitionInX(bACKGROUND_SIZE.x + 22f, 0f), win);
				Vector2 bACKGROUND_SIZE2 = BACKGROUND_SIZE;
				return pieceOne ^ Absolute.SizeY(GenericPaths.BounceTransitionInY(bACKGROUND_SIZE2.y, 0f), win) ^ Generic.AnimationBounceTransitionIn(BACKGROUND_SIZE, 0.3f, bounce.ToArray()) ^ Generic.AnimationFadeTransitionIn(fade.ToArray());
			};
		}

		public static AnimationPieceGeneratorDelegate GetCloseAnim(List<GUIControl> bounce, List<GUIControl> fade, GUIWindow win)
		{
			return delegate
			{
				Vector2 bACKGROUND_SIZE = BACKGROUND_SIZE;
				AnimClip pieceOne = Absolute.SizeX(GenericPaths.BounceTransitionOutX(bACKGROUND_SIZE.x + 22f, 0f), win);
				Vector2 bACKGROUND_SIZE2 = BACKGROUND_SIZE;
				return pieceOne ^ Absolute.SizeY(GenericPaths.BounceTransitionOutY(bACKGROUND_SIZE2.y, 0f), win) ^ Generic.AnimationBounceTransitionOut(BACKGROUND_SIZE, 0.3f, bounce.ToArray()) ^ Generic.AnimationFadeTransitionOut(fade.ToArray());
			};
		}
	}

	public class Sorter : GUISimpleControlWindow
	{
		public class SorterButton : GUIButton
		{
			public enum SorterType
			{
				ItemSorter,
				HeroSorter,
				ExpendableSorter,
				MysteryBoxSorter,
				RecentAcquireSorter
			}

			protected static readonly Vector2 SIZE = new Vector2(102f, 97f) * 0.81f;

			public float currentValue;

			public bool display;

			public string sliderTooltipName;

			public SorterType sorterType;

			public object[] sortBy;

			public SorterButton(string toolTipName, string sliderTooltipName, string imageLocation, Sorter headWindow, SorterType type, params object[] sortBy)
			{
				SetSize(SIZE);
				this.sliderTooltipName = sliderTooltipName;
				sorterType = type;
				this.sortBy = sortBy;
				StyleInfo = new SHSButtonStyleInfo(imageLocation, SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
				display = true;
				base.ToolTip = new NamedToolTipInfo(toolTipName);
				Click += delegate
				{
					headWindow.CenterButton(this);
				};
			}

			public void ModifyParams(float value)
			{
				currentValue = value;
				float num = Mathf.Abs(value);
				Offset = new Vector2(value * 50f, num * 3f);
				if (display)
				{
					SetSize(SIZE * Mathf.Clamp01((2f - num) / 2f));
					IsVisible = true;
				}
				else
				{
					IsVisible = false;
				}
			}
		}

		public class SorterAnim : SHSAnimations
		{
			public static AnimClip GoToAll(List<SorterButton> sorterButtons, float targetLoc)
			{
				AnimClip result = Generic.Blank();
				for (int i = 0; i < sorterButtons.Count; i++)
				{
					float num = ConvertToLoopedLocation(sorterButtons.Count, i, targetLoc);
					sorterButtons[i].display = (Mathf.Abs(num) < 4f);
					result ^= GoTo(sorterButtons[i], num, 0.3f);
				}
				return result;
			}

			private static float ConvertToLoopedLocation(float size, float loc, float center)
			{
				float num = loc - center;
				float num2 = (size - Mathf.Abs(num)) * (float)((!(num > 0f)) ? 1 : (-1));
				return BestNum(num, num2);
			}

			private static float BestNum(float num1, float num2)
			{
				if (Mathf.Abs(num1) < Mathf.Abs(num2))
				{
					return num1;
				}
				return num2;
			}

			public static AnimClip GoTo(SorterButton button, float loc, float time)
			{
				return Custom.Function(GenericPaths.LinearWithBounce(button.currentValue, loc, time), button.ModifyParams);
			}
		}

		private List<SorterButton> sorterButtons = new List<SorterButton>();

		private SHSInventoryAnimatedWindow inventoryWindow;

		public Sorter(SHSInventoryAnimatedWindow inventoryWindow)
		{
			this.inventoryWindow = inventoryWindow;
			Vector2 bACKGROUND_SIZE = BACKGROUND_SIZE;
			SetSize(bACKGROUND_SIZE.x, 67f);
			SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(0f, -65f));
			switch (inventoryWindow.InventoryContext)
			{
			case InventoryType.HQInventory:
				sorterButtons.Add(new SorterButton("#TT_INVENTORY_1", "#TT_inventory_more_heroes", "persistent_bundle|inventory_heroes", this, SorterButton.SorterType.HeroSorter, ItemDefinition.Categories.Heroes));
				sorterButtons.Add(new SorterButton("#TT_INVENTORY_2", "#TT_inventory_more_buildingBlocks", "persistent_bundle|inventory_buildingblocks", this, SorterButton.SorterType.ItemSorter, ItemDefinition.Categories.BuildingBlocks));
				sorterButtons.Add(new SorterButton("#TT_INVENTORY_3", "#TT_inventory_more_seating", "persistent_bundle|inventory_seating", this, SorterButton.SorterType.ItemSorter, ItemDefinition.Categories.Seating));
				sorterButtons.Add(new SorterButton("#TT_INVENTORY_4", "#TT_inventory_more_otherFurniture", "persistent_bundle|inventory_otherfurniture", this, SorterButton.SorterType.ItemSorter, ItemDefinition.Categories.OtherFurniture));
				sorterButtons.Add(new SorterButton("#TT_INVENTORY_5", "#TT_inventory_more_decorations", "persistent_bundle|inventory_decorations", this, SorterButton.SorterType.ItemSorter, ItemDefinition.Categories.Decorations));
				sorterButtons.Add(new SorterButton("#TT_INVENTORY_6", "#TT_inventory_more_food", "persistent_bundle|inventory_food", this, SorterButton.SorterType.ItemSorter, ItemDefinition.Categories.Food));
				sorterButtons.Add(new SorterButton("#TT_INVENTORY_7", "#TT_inventory_more_explosives", "persistent_bundle|inventory_explosives", this, SorterButton.SorterType.ItemSorter, ItemDefinition.Categories.Explosives));
				sorterButtons.Add(new SorterButton("#TT_INVENTORY_8", "#TT_inventory_more_paints", "persistent_bundle|inventory_paints", this, SorterButton.SorterType.ItemSorter, ItemDefinition.Categories.Paints));
				sorterButtons.Add(new SorterButton("#TT_INVENTORY_9", "#TT_inventory_more_myToys", "persistent_bundle|inventory_mytoys", this, SorterButton.SorterType.ItemSorter, ItemDefinition.Categories.MyToys));
				sorterButtons.Add(new SorterButton("#TT_INVENTORY_10", "#TT_inventory_more_heroToys", "persistent_bundle|inventory_herotoys", this, SorterButton.SorterType.ItemSorter, ItemDefinition.Categories.HeroToys));
				break;
			case InventoryType.SocialSpaceInventory:
				sorterButtons.Add(new SorterButton("#TT_GOODY_INVENTORY_STATISTICS", "#TT_GOODY_INVENTORY_MORE_STATISTICS", "goodiesstats_bundle|goody_icon_levelup", this, SorterButton.SorterType.ExpendableSorter, ExpendableDefinition.Categories.Boosts));
				sorterButtons.Add(new SorterButton("#TT_GOODY_INVENTORY_EFFECTS", "#TT_GOODY_INVENTORY_MORE_EFFECTS", "goodieseffects_bundle|goody_icon_effects", this, SorterButton.SorterType.ExpendableSorter, ExpendableDefinition.Categories.WorldEffect));
				sorterButtons.Add(new SorterButton("#TT_GOODY_INVENTORY_MYSTERYBOX", "#TT_GOODY_INVENTORY_MORE_MYSTERYBOX", "goodieseffects_bundle|goody_icon_mysterybox", this, SorterButton.SorterType.MysteryBoxSorter, ExpendableDefinition.Categories.MysteryBox));
				break;
			}
			sorterButtons.Add(new SorterButton("#TT_INVENTORY_RECENT_ACQUIRE", "#TT_INVENTORY_MORE_RECENT_ACQUIRE", "persistent_bundle|inventory_new_session", this, SorterButton.SorterType.RecentAcquireSorter));
			foreach (SorterButton sorterButton in sorterButtons)
			{
				sorterButton.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 0f));
				Add(sorterButton);
			}
			SetSorterLocation(GetDefaultLocation(inventoryWindow.InventoryContext));
			CenterButton(GetSorterButton(inventoryWindow.currentLoc));
			GUIButton gUIButton = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(64f, 64f), new Vector2(-105f, 8f));
			gUIButton.StyleInfo = new SHSButtonStyleInfo("persistent_bundle|button_inventory_leftscroll");
			gUIButton.HitTestType = HitTestTypeEnum.Rect;
			gUIButton.ToolTip = new NamedToolTipInfo("#TT_INVENTORY_11");
			gUIButton.Click += delegate
			{
				SetSorterLocation(inventoryWindow.currentLoc - 1);
				CenterButton(GetSorterButton(inventoryWindow.currentLoc));
			};
			Add(gUIButton);
			GUIButton gUIButton2 = GUIControl.CreateControlFrameCentered<GUIButton>(new Vector2(64f, 64f), new Vector2(104f, 8f));
			gUIButton2.StyleInfo = new SHSButtonStyleInfo("persistent_bundle|button_inventory_rightscroll");
			gUIButton2.HitTestType = HitTestTypeEnum.Rect;
			gUIButton2.ToolTip = new NamedToolTipInfo("#TT_INVENTORY_12");
			gUIButton2.Click += delegate
			{
				SetSorterLocation(inventoryWindow.currentLoc + 1);
				CenterButton(GetSorterButton(inventoryWindow.currentLoc));
			};
			Add(gUIButton2);
		}

		public void CenterButtonOnNewCurrentLoc()
		{
			SetSorterLocation(GetDefaultLocation(inventoryWindow.InventoryContext));
			CenterButton(GetSorterButton(inventoryWindow.currentLoc));
		}

		public void CenterButton(SorterButton target)
		{
			if (target != null)
			{
				SetSorterLocation(sorterButtons.IndexOf(target));
				base.AnimationPieceManager.Add(SorterAnim.GoToAll(sorterButtons, inventoryWindow.currentLoc));
				Sort(target);
				switch (target.sorterType)
				{
				case SorterButton.SorterType.HeroSorter:
					inventoryWindow.HeroSlider.StartArrow.ToolTip = new NamedToolTipInfo(target.sliderTooltipName);
					inventoryWindow.HeroSlider.EndArrow.ToolTip = new NamedToolTipInfo(target.sliderTooltipName);
					break;
				case SorterButton.SorterType.ItemSorter:
					inventoryWindow.ItemSlider.StartArrow.ToolTip = new NamedToolTipInfo(target.sliderTooltipName);
					inventoryWindow.ItemSlider.EndArrow.ToolTip = new NamedToolTipInfo(target.sliderTooltipName);
					break;
				case SorterButton.SorterType.ExpendableSorter:
					inventoryWindow.ExpendableSlider.StartArrow.ToolTip = new NamedToolTipInfo(target.sliderTooltipName);
					inventoryWindow.ExpendableSlider.EndArrow.ToolTip = new NamedToolTipInfo(target.sliderTooltipName);
					break;
				case SorterButton.SorterType.MysteryBoxSorter:
					inventoryWindow.ExpendableSlider.StartArrow.ToolTip = new NamedToolTipInfo(target.sliderTooltipName);
					inventoryWindow.ExpendableSlider.EndArrow.ToolTip = new NamedToolTipInfo(target.sliderTooltipName);
					break;
				case SorterButton.SorterType.RecentAcquireSorter:
					inventoryWindow.RecentAquireSlider.StartArrow.ToolTip = new NamedToolTipInfo(target.sliderTooltipName);
					inventoryWindow.RecentAquireSlider.EndArrow.ToolTip = new NamedToolTipInfo(target.sliderTooltipName);
					break;
				}
			}
		}

		public void Sort(SorterButton target)
		{
			if (target != null)
			{
				switch (target.sorterType)
				{
				case SorterButton.SorterType.HeroSorter:
					inventoryWindow.GoToCharacterWindow();
					break;
				case SorterButton.SorterType.ItemSorter:
					inventoryWindow.GoToInventoryWindow(target.sortBy);
					break;
				case SorterButton.SorterType.ExpendableSorter:
					inventoryWindow.GoToExpendableWindow(target.sortBy);
					break;
				case SorterButton.SorterType.MysteryBoxSorter:
					inventoryWindow.GoToMysteryBoxWindow(target.sortBy);
					break;
				case SorterButton.SorterType.RecentAcquireSorter:
					inventoryWindow.GoToRecentAcquireWindow();
					break;
				}
			}
		}

		public SorterButton GetSorterButton(int index)
		{
			return (index < 0 || index >= sorterButtons.Count) ? null : sorterButtons[index];
		}

		public void SetSorterLocation(int location)
		{
			int num = 0;
			if (sorterButtons != null && sorterButtons.Count > 0)
			{
				num = sorterButtons.Count - 1;
			}
			if (location > num)
			{
				inventoryWindow.currentLoc = 0;
			}
			else if (location < 0)
			{
				inventoryWindow.currentLoc = num;
			}
			else
			{
				inventoryWindow.currentLoc = location;
			}
		}
	}

	private enum PriorityTabType
	{
		HeroTab = 0,
		FoodTab = 5,
		RecentGoodieTab = 2,
		RecentHqTab = 3
	}

	protected static readonly Vector2 SLIDER_SIZE = new Vector2(50f, 325f);

	protected static readonly Vector2 SLIDER_OFFSET = new Vector2(-6f, 54f);

	protected static readonly Vector2 IWINDOW_SIZE = new Vector2(227f, 358f);

	protected static readonly Vector2 IWINDOW_OFFSET = new Vector2(8f, 35f);

	protected static readonly Vector2 BACKGROUND_SIZE = new Vector2(292f, 527f);

	protected static readonly int RECENT_ACQUIRE_MAX = 100;

	private GUISlider ItemSlider;

	private GUISlider HeroSlider;

	private GUISlider ExpendableSlider;

	private GUISlider MysteryBoxSlider;

	private GUISlider RecentAquireSlider;

	private SHSInventorySelectionWindow ItemWindow;

	private SHSInventorySelectionWindow CharacterWindow;

	private SHSInventoryExpendableWindow ExpendableWindow;

	private SHSInventoryMysteryBoxWindow MysteryBoxWindow;

	private SHSInventoryExpendableWindow RecentAquireWindow;

	private Sorter sorter;

	public static SHSInventoryAnimatedWindow instance = null;

	private object[] currentCategories;

	private static int _priorityLocations;

	[CompilerGenerated]
	private InventoryType _003CInventoryContext_003Ek__BackingField;

	public InventoryType InventoryContext
	{
		[CompilerGenerated]
		get
		{
			return _003CInventoryContext_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CInventoryContext_003Ek__BackingField = value;
		}
	}

	private int currentLoc
	{
		get
		{
			return GetCurrentLoc();
		}
		set
		{
			SetCurrentLoc(value);
		}
	}

	public SHSInventoryAnimatedWindow()
	{
		if (SocialSpaceController.Instance != null)
		{
			InventoryContext = InventoryType.SocialSpaceInventory;
		}
		else
		{
			InventoryContext = InventoryType.HQInventory;
		}
		AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "generic_event", "open_backpack", 1, string.Empty);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		SetPosition(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomMiddle, OffsetType.Absolute, new Vector2(138f, -126f));
		SetSize(BACKGROUND_SIZE);
		GUIImage gUIImage = new GUIImage();
		gUIImage.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle);
		gUIImage.SetSize(BACKGROUND_SIZE);
		gUIImage.TextureSource = "persistent_bundle|inventory_back_panel";
		GUIImage gUIImage2 = new GUIImage();
		gUIImage2.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle);
		gUIImage2.SetSize(BACKGROUND_SIZE);
		gUIImage2.TextureSource = "persistent_bundle|inventory_front_panel";
		if (InventoryContext == InventoryType.HQInventory)
		{
			HeroSlider = new GUISlider();
			HeroSlider.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, SLIDER_OFFSET);
			HeroSlider.SetSize(SLIDER_SIZE);
			HeroSlider.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			ItemSlider = new GUISlider();
			ItemSlider.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, SLIDER_OFFSET);
			ItemSlider.IsVisible = false;
			ItemSlider.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			ItemWindow = new SHSInventorySelectionWindow(ItemSlider);
			ItemWindow.AddList(getItemList());
			ItemWindow.SortItemList();
			ItemWindow.SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, IWINDOW_OFFSET);
			ItemWindow.IsVisible = false;
			ItemWindow.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			CharacterWindow = new SHSInventorySelectionWindow(HeroSlider);
			CharacterWindow.AddList(getCharacterList());
			CharacterWindow.SortItemList();
			CharacterWindow.SetSize(IWINDOW_SIZE);
			CharacterWindow.SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, IWINDOW_OFFSET);
			CharacterWindow.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		}
		else if (InventoryContext == InventoryType.SocialSpaceInventory)
		{
			ExpendableSlider = new GUISlider();
			ExpendableSlider.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, SLIDER_OFFSET);
			ExpendableSlider.SetSize(SLIDER_SIZE);
			ExpendableSlider.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			ExpendableWindow = new SHSInventoryExpendableWindow(ExpendableSlider);
			ExpendableWindow.InitializeExpendableList(getExpendableList());
			ExpendableWindow.SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, IWINDOW_OFFSET);
			ExpendableWindow.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			MysteryBoxSlider = new GUISlider();
			MysteryBoxSlider.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, SLIDER_OFFSET);
			MysteryBoxSlider.SetSize(SLIDER_SIZE);
			MysteryBoxSlider.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
			MysteryBoxWindow = new SHSInventoryMysteryBoxWindow(MysteryBoxSlider);
			MysteryBoxWindow.InitializeExpendableList(getMysteryBoxList());
			MysteryBoxWindow.SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, IWINDOW_OFFSET);
			MysteryBoxWindow.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		}
		RecentAquireSlider = new GUISlider();
		RecentAquireSlider.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, SLIDER_OFFSET);
		RecentAquireSlider.SetSize(SLIDER_SIZE);
		RecentAquireSlider.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		RecentAquireWindow = new SHSInventoryExpendableWindow(RecentAquireSlider);
		RecentAquireWindow.InitializeExpendableList(getRecentAcquireList());
		RecentAquireWindow.SetPosition(DockingAlignmentEnum.TopMiddle, AnchorAlignmentEnum.TopMiddle, OffsetType.Absolute, IWINDOW_OFFSET);
		RecentAquireWindow.Traits.VisibilityTrait = ControlTraits.VisibilityTraitEnum.Manual;
		sorter = new Sorter(this);
		GUITBCloseButton gUITBCloseButton = new GUITBCloseButton();
		gUITBCloseButton.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-30f, 26f));
		gUITBCloseButton.Click += delegate
		{
			ToggleClosed();
		};
		Add(gUIImage);
		if (InventoryContext == InventoryType.HQInventory)
		{
			Add(ItemWindow);
			Add(CharacterWindow);
		}
		else if (InventoryContext == InventoryType.SocialSpaceInventory)
		{
			Add(ExpendableWindow);
			Add(MysteryBoxWindow);
		}
		Add(RecentAquireWindow);
		Add(gUIImage2);
		Add(sorter);
		if (InventoryContext == InventoryType.HQInventory)
		{
			Add(HeroSlider);
			Add(ItemSlider);
		}
		else if (InventoryContext == InventoryType.SocialSpaceInventory)
		{
			Add(ExpendableSlider);
		}
		Add(RecentAquireSlider);
		Add(gUITBCloseButton);
		List<GUIControl> list = new List<GUIControl>();
		list.Add(gUIImage);
		list.Add(gUIImage2);
		List<GUIControl> list2 = new List<GUIControl>();
		if (InventoryContext == InventoryType.HQInventory)
		{
			list2.Add(HeroSlider);
			list2.Add(ItemSlider);
			list2.Add(ItemWindow);
			list2.Add(CharacterWindow);
		}
		else if (InventoryContext == InventoryType.SocialSpaceInventory)
		{
			list2.Add(ExpendableSlider);
			list2.Add(ExpendableWindow);
			list2.Add(MysteryBoxSlider);
			list2.Add(MysteryBoxWindow);
		}
		list2.Add(RecentAquireSlider);
		list2.Add(RecentAquireWindow);
		list2.Add(sorter);
		list2.Add(gUITBCloseButton);
		base.AnimationOnOpen = InvAnims.GetOpenAnim(list, list2, this);
		base.AnimationOnClose = InvAnims.GetCloseAnim(list, list2, this);
		instance = this;
	}

	private void OnRequestCenterButtonOnNewCurrentLoc(InventoryRequestCenterButtonOnNewCurrentLoc msg)
	{
		sorter.CenterButtonOnNewCurrentLoc();
	}

	private void OnInventoryFetchComplete(InventoryFetchCompleteMessage msg)
	{
		if (msg.success)
		{
			if (InventoryContext == InventoryType.HQInventory)
			{
				RefreshItems();
			}
			RefreshRecentAcquires();
		}
	}

	private void OnItemsUpdated(InventoryCollectionUpdateMessage message)
	{
		string[] keys = message.keys;
		foreach (string text in keys)
		{
			CspUtils.DebugLog("  " + text + " " + (InventoryContext == InventoryType.SocialSpaceInventory) + " " + MysteryBoxWindow);
			if (InventoryContext == InventoryType.HQInventory)
			{
				UpdateItem(text, ItemWindow);
			}
			else if (InventoryContext == InventoryType.SocialSpaceInventory)
			{
				UpdateItem(text, ExpendableWindow);
				UpdateItem(text, MysteryBoxWindow);
			}
			UpdateItem(text, RecentAquireWindow);
		}
	}

	private void UpdateItem(string id, SHSInventorySelectionWindow window)
	{
		if (window == null)
		{
			return;
		}
		if (window == MysteryBoxWindow)
		{
			CspUtils.DebugLog("UpdateItem called with id " + id);
		}
		SHSInventorySelectionItem sHSInventorySelectionItem = window.Find(delegate(SHSInventorySelectionItem item)
		{
			return item.CompareTo(id);
		});
		if (sHSInventorySelectionItem == null)
		{
			if (window == MysteryBoxWindow)
			{
				CspUtils.DebugLog("found no matching with that id " + id);
			}
			return;
		}
		if (window == MysteryBoxWindow)
		{
			CspUtils.DebugLog("found ite, has quan " + sHSInventorySelectionItem.CollectionItemCount);
		}
		if (sHSInventorySelectionItem.CollectionItemCount <= 0)
		{
			window.RemoveItem(sHSInventorySelectionItem);
		}
		else
		{
			sHSInventorySelectionItem.UpdateItemCount();
		}
	}

	private void OnHeroesUpdated(HeroCollectionUpdateMessage message)
	{
		string[] keys = message.keys;
		foreach (string id in keys)
		{
			if (InventoryContext == InventoryType.HQInventory)
			{
				UpdateHero(id, CharacterWindow);
			}
			UpdateHero(id, RecentAquireWindow);
		}
	}

	private void UpdateHero(string id, SHSInventorySelectionWindow window)
	{
		SHSInventorySelectionItem sHSInventorySelectionItem = window.Find(delegate(SHSInventorySelectionItem item)
		{
			return item.CompareTo(id);
		});
		if (sHSInventorySelectionItem is SHSInventoryHeroItem)
		{
			((SHSInventoryHeroItem)sHSInventorySelectionItem).UpdateHeroPlaced();
		}
	}

	private void OnMysteryBoxFetchComplete(MysteryBoxFetchCompleteMessage msg)
	{
		if (msg.success)
		{
			if (InventoryContext == InventoryType.SocialSpaceInventory)
			{
				RefreshInventoryMysteryBoxes();
			}
			RefreshRecentAcquires();
		}
	}

	private void OnPotionFetchComplete(PotionFetchCompleteMessage msg)
	{
		if (msg.success)
		{
			if (InventoryContext == InventoryType.SocialSpaceInventory)
			{
				RefreshInventoryExpendables();
			}
			RefreshRecentAcquires();
		}
	}

	private void RefreshItems()
	{
		ItemWindow.ClearItems();
		ItemWindow.AddList(getItemList());
		ItemWindow.SortItemList();
		if (currentCategories != null)
		{
			InventorySort(currentCategories);
		}
	}

	private void InventorySort(object[] categories)
	{
		currentCategories = categories;
		foreach (SHSInventoryHQItem item in ItemWindow.items)
		{
			bool active = true;
			foreach (object obj in categories)
			{
				if (!item.InventoryItem.Definition.CategoryList.Contains((ItemDefinition.Categories)(int)obj))
				{
					active = false;
					break;
				}
			}
			item.active = active;
		}
		ItemWindow.RequestARefresh();
	}

	public void GoToInventoryWindow(object[] categories)
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		base.AnimationPieceManager.ClearAll();
		AnimClip animClip = SHSAnimations.Generic.FadeOut(ItemSlider, 0.3f) ^ SHSAnimations.Generic.FadeOut(HeroSlider, 0.3f) ^ SHSAnimations.Generic.FadeOut(RecentAquireSlider, 0.3f) ^ SHSAnimations.Generic.FadeOut(ItemWindow, 0.3f) ^ SHSAnimations.Generic.FadeOut(CharacterWindow, 0.3f) ^ SHSAnimations.Generic.FadeOut(RecentAquireWindow, 0.3f);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			HeroSlider.IsVisible = false;
			CharacterWindow.IsVisible = false;
			RecentAquireSlider.IsVisible = false;
			RecentAquireWindow.IsVisible = false;
			ItemSlider.IsVisible = true;
			ItemWindow.IsVisible = true;
			ItemSlider.SetSize(SLIDER_SIZE);
			ItemWindow.SetSize(IWINDOW_SIZE);
			InventorySort(categories);
		};
		AnimClip pieceTwo = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0f, 1f, 0.3f), ItemSlider) ^ AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0f, 1f, 0.3f), ItemWindow);
		AnimClip toAdd = animClip | pieceTwo;
		base.AnimationPieceManager.Add(toAdd);
	}

	public void GoToCharacterWindow()
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		base.AnimationPieceManager.ClearAll();
		AnimClip animClip = SHSAnimations.Generic.FadeOut(ItemSlider, 0.3f) ^ SHSAnimations.Generic.FadeOut(HeroSlider, 0.3f) ^ SHSAnimations.Generic.FadeOut(RecentAquireSlider, 0.3f) ^ SHSAnimations.Generic.FadeOut(ItemWindow, 0.3f) ^ SHSAnimations.Generic.FadeOut(CharacterWindow, 0.3f) ^ SHSAnimations.Generic.FadeOut(RecentAquireWindow, 0.3f);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			ItemSlider.IsVisible = false;
			ItemWindow.IsVisible = false;
			RecentAquireSlider.IsVisible = false;
			RecentAquireWindow.IsVisible = false;
			HeroSlider.IsVisible = true;
			CharacterWindow.IsVisible = true;
			HeroSlider.SetSize(SLIDER_SIZE);
			CharacterWindow.SetSize(IWINDOW_SIZE);
		};
		AnimClip pieceTwo = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0f, 1f, 0.3f), HeroSlider) ^ AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0f, 1f, 0.3f), CharacterWindow);
		AnimClip toAdd = animClip | pieceTwo;
		base.AnimationPieceManager.Add(toAdd);
	}

	private List<SHSInventorySelectionItem> getItemList()
	{
		List<SHSInventorySelectionItem> list = new List<SHSInventorySelectionItem>();
		UserProfile profile = AppShell.Instance.Profile;
		if (profile == null)
		{
			return list;
		}
		foreach (KeyValuePair<string, Item> availableItem in profile.AvailableItems)
		{
			string id = availableItem.Value.Id;
			Item value = null;
			if (!profile.AvailableItems.TryGetValue(id, out value))
			{
				CspUtils.DebugLog("Item <" + id + "> being added to inventory UI was not found in the inventory collection!");
			}
			else if (value.Definition == null)
			{
				CspUtils.DebugLog("Item definition <" + value.Id + "> is awol. not showing what can't be defined.");
			}
			else
			{
				list.Add(new SHSInventoryHQItem(value, this));
			}
		}
		return list;
	}

	private List<SHSInventorySelectionItem> getCharacterList()
	{
		List<SHSInventorySelectionItem> list = new List<SHSInventorySelectionItem>();
		UserProfile profile = AppShell.Instance.Profile;
		if (profile == null)
		{
			return list;
		}
		foreach (KeyValuePair<string, HeroPersisted> availableCostume in profile.AvailableCostumes)
		{
			string key = availableCostume.Key;
			HeroPersisted value = null;
			if (!profile.AvailableCostumes.TryGetValue(key, out value))
			{
				CspUtils.DebugLog("Hero <" + key + "> being added to hero UI was not found in the hero collection!");
			}
			else
			{
				list.Add(new SHSInventoryHeroItem(value, this));
			}
		}
		return list;
	}

	private void RefreshInventoryMysteryBoxes()
	{
		MysteryBoxWindow.ClearItems();
		MysteryBoxWindow.AddList(getMysteryBoxList());
		MysteryBoxWindow.SortItemList();
		MysteryBoxWindow.RequestARefresh();
	}

	private void RefreshInventoryExpendables()
	{
		ExpendableWindow.ClearItems();
		ExpendableWindow.AddList(getExpendableList());
		ExpendableWindow.SortItemList();
		if (currentCategories != null)
		{
			InventoryExpendableSort(currentCategories);
		}
	}

	private void InventoryExpendableSort(object[] categories)
	{
		currentCategories = categories;
		foreach (SHSInventoryExpendableItem item in ExpendableWindow.items)
		{
			bool active = true;
			foreach (object obj in categories)
			{
				if (!item.expendable.Definition.CategoryList.Contains((ExpendableDefinition.Categories)(int)obj))
				{
					active = false;
					break;
				}
			}
			item.active = active;
		}
		ExpendableWindow.RequestARefresh();
	}

	public void GoToMysteryBoxWindow(object[] categories)
	{
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Expected O, but got Unknown
		base.AnimationPieceManager.ClearAll();
		AnimClip animClip = SHSAnimations.Generic.FadeOut(ExpendableSlider, 0.3f) ^ SHSAnimations.Generic.FadeOut(RecentAquireSlider, 0.3f) ^ SHSAnimations.Generic.FadeOut(ExpendableWindow, 0.3f) ^ SHSAnimations.Generic.FadeOut(MysteryBoxSlider, 0.3f) ^ SHSAnimations.Generic.FadeOut(MysteryBoxWindow, 0.3f) ^ SHSAnimations.Generic.FadeOut(RecentAquireWindow, 0.3f);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			RecentAquireSlider.IsVisible = false;
			RecentAquireWindow.IsVisible = false;
			ExpendableSlider.IsVisible = false;
			ExpendableWindow.IsVisible = false;
			MysteryBoxSlider.IsVisible = true;
			MysteryBoxWindow.IsVisible = true;
			MysteryBoxWindow.SetSize(SLIDER_SIZE);
			MysteryBoxWindow.SetSize(IWINDOW_SIZE);
			InventoryExpendableSort(categories);
		};
		AnimClip pieceTwo = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0f, 1f, 0.3f), MysteryBoxSlider) ^ AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0f, 1f, 0.3f), MysteryBoxWindow);
		AnimClip toAdd = animClip | pieceTwo;
		base.AnimationPieceManager.Add(toAdd);
	}

	public void GoToExpendableWindow(object[] categories)
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		base.AnimationPieceManager.ClearAll();
		AnimClip animClip = SHSAnimations.Generic.FadeOut(ExpendableSlider, 0.3f) ^ SHSAnimations.Generic.FadeOut(RecentAquireSlider, 0.3f) ^ SHSAnimations.Generic.FadeOut(ExpendableWindow, 0.3f) ^ SHSAnimations.Generic.FadeOut(RecentAquireWindow, 0.3f);
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			RecentAquireSlider.IsVisible = false;
			RecentAquireWindow.IsVisible = false;
			ExpendableSlider.IsVisible = true;
			ExpendableWindow.IsVisible = true;
			ExpendableSlider.SetSize(SLIDER_SIZE);
			ExpendableWindow.SetSize(IWINDOW_SIZE);
			MysteryBoxSlider.IsVisible = false;
			MysteryBoxWindow.IsVisible = false;
			InventoryExpendableSort(categories);
		};
		AnimClip pieceTwo = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0f, 1f, 0.3f), ExpendableSlider) ^ AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0f, 1f, 0.3f), ExpendableWindow);
		AnimClip toAdd = animClip | pieceTwo;
		base.AnimationPieceManager.Add(toAdd);
	}

	private List<SHSInventorySelectionItem> getExpendableList()
	{
		List<SHSInventorySelectionItem> list = new List<SHSInventorySelectionItem>();
		UserProfile profile = AppShell.Instance.Profile;
		if (profile == null)
		{
			return list;
		}
		foreach (KeyValuePair<string, Expendable> item in profile.ExpendablesCollection)
		{
			if (IsExpendableValid(item.Key, item.Value))
			{
				list.Add(new SHSInventoryExpendableItem(item.Value, this));
			}
		}
		return list;
	}

	private List<SHSInventorySelectionItem> getMysteryBoxList()
	{
		List<SHSInventorySelectionItem> list = new List<SHSInventorySelectionItem>();
		UserProfile profile = AppShell.Instance.Profile;
		if (profile == null)
		{
			return list;
		}
		foreach (KeyValuePair<string, MysteryBox> item in profile.MysteryBoxesCollection)
		{
			if (item.Value.Quantity > 0)
			{
				list.Add(new SHSInventoryMysteryItem(item.Value, this));
			}
		}
		return list;
	}

	private bool IsExpendableValid(string expendableId, Expendable expendableData)
	{
		if (expendableData == null || expendableData.Definition == null)
		{
			CspUtils.DebugLog("SHSInventoryAnimatedWindow::IsExpendableValid() - Expendable <" + expendableId + "> does not have a definition");
			return false;
		}
		if (expendableData.Definition.CategoryList == null || expendableData.Definition.CategoryList.Count == 0)
		{
			CspUtils.DebugLog("SHSInventoryAnimatedWindow::IsExpendableValid() - Expendable <" + expendableId + "> is not categorized");
			return false;
		}
		return !expendableData.Definition.CategoryList.Contains(ExpendableDefinition.Categories.Internal);
	}

	public void GoToRecentAcquireWindow()
	{
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		base.AnimationPieceManager.ClearAll();
		AnimClip animClip = SHSAnimations.Generic.FadeOut(RecentAquireSlider, 0.3f) ^ SHSAnimations.Generic.FadeOut(RecentAquireWindow, 0.3f);
		if (InventoryContext == InventoryType.HQInventory)
		{
			animClip ^= (SHSAnimations.Generic.FadeOut(ItemSlider, 0.3f) ^ SHSAnimations.Generic.FadeOut(HeroSlider, 0.3f) ^ SHSAnimations.Generic.FadeOut(ItemWindow, 0.3f) ^ SHSAnimations.Generic.FadeOut(CharacterWindow, 0.3f));
		}
		else if (InventoryContext == InventoryType.SocialSpaceInventory)
		{
			animClip ^= (SHSAnimations.Generic.FadeOut(ExpendableSlider, 0.3f) ^ SHSAnimations.Generic.FadeOut(ExpendableWindow, 0.3f));
		}
		animClip.OnFinished += (Action)(object)(Action)delegate
		{
			if (InventoryContext == InventoryType.HQInventory)
			{
				HeroSlider.IsVisible = false;
				CharacterWindow.IsVisible = false;
				ItemSlider.IsVisible = false;
				ItemWindow.IsVisible = false;
			}
			else if (InventoryContext == InventoryType.SocialSpaceInventory)
			{
				ExpendableSlider.IsVisible = false;
				ExpendableWindow.IsVisible = false;
				MysteryBoxSlider.IsVisible = false;
				MysteryBoxWindow.IsVisible = false;
			}
			RecentAquireSlider.IsVisible = true;
			RecentAquireWindow.IsVisible = true;
			RecentAquireSlider.SetSize(SLIDER_SIZE);
			RecentAquireWindow.SetSize(IWINDOW_SIZE);
			RecentAcquireSort();
		};
		AnimClip pieceTwo = AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0f, 1f, 0.3f), RecentAquireSlider) ^ AnimClipBuilder.Absolute.Alpha(AnimClipBuilder.Path.Linear(0f, 1f, 0.3f), RecentAquireWindow);
		AnimClip toAdd = animClip | pieceTwo;
		base.AnimationPieceManager.Add(toAdd);
	}

	private List<SHSInventorySelectionItem> getRecentAcquireList()
	{
		List<SHSInventorySelectionItem> list = new List<SHSInventorySelectionItem>();
		if (AppShell.Instance.InventoryRecorder == null || !AppShell.Instance.InventoryRecorder.IsInitialized)
		{
			return list;
		}
		if (AppShell.Instance.Profile == null)
		{
			return list;
		}
		foreach (InventorySessionRecorder.InventoryRecord inventoryRecord in AppShell.Instance.InventoryRecorder.InventoryRecordList)
		{
			if (list.Count >= RECENT_ACQUIRE_MAX)
			{
				return list;
			}
			SHSInventorySelectionItem sHSInventorySelectionItem = null;
			switch (inventoryRecord.RecordOwnableType)
			{
			case InventorySessionRecorder.InventoryRecord.RecordType.HQOwnable:
			{
				Item value3 = null;
				if (AppShell.Instance.Profile.AvailableItems.TryGetValue(inventoryRecord.RecordOwnableId, out value3))
				{
					sHSInventorySelectionItem = new SHSInventoryHQItem(value3, this);
				}
				else
				{
					CspUtils.DebugLog("SHSInventoryAnimatedWindow::getRecentAcquireList() - Item <" + inventoryRecord.RecordOwnableId + "> not found in profile item collection");
				}
				break;
			}
			case InventorySessionRecorder.InventoryRecord.RecordType.HeroOwnable:
			{
				HeroPersisted value2 = null;
				if (AppShell.Instance.Profile.AvailableCostumes.TryGetValue(inventoryRecord.RecordOwnableId, out value2))
				{
					sHSInventorySelectionItem = new SHSInventoryHeroItem(value2, this);
				}
				else
				{
					CspUtils.DebugLog("SHSInventoryAnimatedWindow::getRecentAcquireList() - Hero <" + inventoryRecord.RecordOwnableId + "> not found in profile hero collection");
				}
				break;
			}
			case InventorySessionRecorder.InventoryRecord.RecordType.ExpendableOwnable:
			{
				Expendable value = null;
				if (AppShell.Instance.Profile.ExpendablesCollection.TryGetValue(inventoryRecord.RecordOwnableId, out value))
				{
					if (IsExpendableValid(inventoryRecord.RecordOwnableId, value))
					{
						sHSInventorySelectionItem = new SHSInventoryExpendableItem(value, this);
					}
				}
				else
				{
					CspUtils.DebugLog("SHSInventoryAnimatedWindow::getRecentAcquireList() - Expendable <" + inventoryRecord.RecordOwnableId + "> not found in profile expendable collection");
				}
				break;
			}
			}
			if (sHSInventorySelectionItem != null)
			{
				list.Add(sHSInventorySelectionItem);
			}
		}
		return list;
	}

	private void RefreshRecentAcquires()
	{
		RecentAquireWindow.ClearItems();
		RecentAquireWindow.InitializeExpendableList(getRecentAcquireList());
		RecentAcquireSort();
	}

	private void RecentAcquireSort()
	{
		foreach (SHSInventorySelectionItem item in RecentAquireWindow.items)
		{
			if (InventoryContext == InventoryType.HQInventory)
			{
				item.active = !(item is SHSInventoryExpendableItem);
			}
			else if (InventoryContext == InventoryType.SocialSpaceInventory)
			{
				item.active = (item is SHSInventoryExpendableItem);
			}
		}
	}

	public override void OnShow()
	{
		InventoryType inventoryContext = InventoryContext;
		if (inventoryContext == InventoryType.HQInventory)
		{
			AppShell.Instance.EventMgr.AddListener<InventoryRequestCenterButtonOnNewCurrentLoc>(OnRequestCenterButtonOnNewCurrentLoc);
		}
		AppShell.Instance.EventMgr.AddListener<HeroCollectionUpdateMessage>(OnHeroesUpdated);
		AppShell.Instance.EventMgr.AddListener<InventoryCollectionUpdateMessage>(OnItemsUpdated);
		AppShell.Instance.EventMgr.AddListener<InventoryFetchCompleteMessage>(OnInventoryFetchComplete);
		AppShell.Instance.EventMgr.AddListener<PotionFetchCompleteMessage>(OnPotionFetchComplete);
		AppShell.Instance.EventMgr.AddListener<MysteryBoxFetchCompleteMessage>(OnMysteryBoxFetchComplete);
		base.OnShow();
	}

	public override void OnHide()
	{
		instance = null;
		InventoryType inventoryContext = InventoryContext;
		if (inventoryContext == InventoryType.HQInventory)
		{
			AppShell.Instance.EventMgr.RemoveListener<InventoryRequestCenterButtonOnNewCurrentLoc>(OnRequestCenterButtonOnNewCurrentLoc);
		}
		AppShell.Instance.EventMgr.RemoveListener<HeroCollectionUpdateMessage>(OnHeroesUpdated);
		AppShell.Instance.EventMgr.RemoveListener<InventoryCollectionUpdateMessage>(OnItemsUpdated);
		AppShell.Instance.EventMgr.RemoveListener<InventoryFetchCompleteMessage>(OnInventoryFetchComplete);
		AppShell.Instance.EventMgr.RemoveListener<PotionFetchCompleteMessage>(OnPotionFetchComplete);
		base.OnHide();
	}

	public override bool CanDrop(DragDropInfo DragDropInfo)
	{
		if (InventoryContext == InventoryType.HQInventory)
		{
			return DragDropInfo.CollectionId == DragDropInfo.CollectionType.Items || DragDropInfo.CollectionId == DragDropInfo.CollectionType.Heroes;
		}
		return false;
	}

	public static bool isCurrentTab(int tabLoc)
	{
		return tabLoc == GetCurrentLoc();
	}

	public static void SetHeroTabAsDefault()
	{
		DirtyPriorityLocation(PriorityTabType.HeroTab);
	}

	public static void SetFoodTabAsDefault()
	{
		DirtyPriorityLocation(PriorityTabType.FoodTab);
	}

	public static void InventoryItemAcquired(InventorySessionRecorder.InventoryRecord.RecordType inventoryType)
	{
		switch (inventoryType)
		{
		case InventorySessionRecorder.InventoryRecord.RecordType.HQOwnable:
		case InventorySessionRecorder.InventoryRecord.RecordType.HeroOwnable:
			DirtyPriorityLocation(PriorityTabType.RecentHqTab);
			break;
		case InventorySessionRecorder.InventoryRecord.RecordType.ExpendableOwnable:
			DirtyPriorityLocation(PriorityTabType.RecentGoodieTab);
			break;
		}
	}

	private static void SetCurrentLoc(int i)
	{
		AppShell.Instance.SharedHashTable["GUIDefaultTabPosition"] = i;
	}

	private static int GetCurrentLoc()
	{
		if (AppShell.Instance != null && AppShell.Instance.SharedHashTable.ContainsKey("GUIDefaultTabPosition"))
		{
			return (int)AppShell.Instance.SharedHashTable["GUIDefaultTabPosition"];
		}
		return 0;
	}

	private static void DirtyPriorityLocation(PriorityTabType tab)
	{
		_priorityLocations |= 1 << (int)(tab & (PriorityTabType)31);
	}

	private static void ClearPriorityLocation(PriorityTabType tab)
	{
		_priorityLocations &= ~(1 << (int)(tab & (PriorityTabType)31));
	}

	private static bool IsPriorityLocationDirty(PriorityTabType tab)
	{
		return (_priorityLocations & (1 << (int)tab)) != 0;
	}

	private static int GetDefaultLocation(InventoryType context)
	{
		if (context == InventoryType.HQInventory)
		{
			if (IsPriorityLocationDirty(PriorityTabType.HeroTab))
			{
				ClearPriorityLocation(PriorityTabType.HeroTab);
				return 0;
			}
			if (IsPriorityLocationDirty(PriorityTabType.FoodTab))
			{
				ClearPriorityLocation(PriorityTabType.FoodTab);
				return 5;
			}
			if (IsPriorityLocationDirty(PriorityTabType.RecentHqTab))
			{
				ClearPriorityLocation(PriorityTabType.RecentHqTab);
				return 10;
			}
		}
		if (context == InventoryType.SocialSpaceInventory && IsPriorityLocationDirty(PriorityTabType.RecentGoodieTab))
		{
			ClearPriorityLocation(PriorityTabType.RecentGoodieTab);
			return 2;
		}
		return GetCurrentLoc();
	}
}
