using System;
using UnityEngine;

public abstract class SHSInventorySelectionItem : SHSSelectionItem<SHSItemLoadingWindow>, IComparable<SHSInventorySelectionItem>
{
	protected GUIButton InventoryItemIcon;

	protected GUILabel ItemCount;

	protected GUIImage NumberOfItems;

	protected string Id;

	protected SHSInventoryAnimatedWindow headWindow;

	public bool ButtonUseable = true;

	public abstract SHSButtonStyleInfo ButtonStyleInfo
	{
		get;
	}

	public abstract GUIControl.ToolTipInfo ItemHoverHelpInfo
	{
		get;
	}

	public abstract ShsCollectionItem CollectionItem
	{
		get;
	}

	public abstract int CollectionItemCount
	{
		get;
	}

	public bool IsEnabled
	{
		get
		{
			return InventoryItemIcon.IsEnabled;
		}
		set
		{
			InventoryItemIcon.IsEnabled = value;
		}
	}

	public SHSInventorySelectionItem(SHSInventoryAnimatedWindow headWindow)
	{
		this.headWindow = headWindow;
	}

	public abstract GUIButton GetSetupInventoryDragDropButton();

	public void SetupWindow()
	{
		item = new SHSItemLoadingWindow();
		item.HitTestType = GUIControl.HitTestTypeEnum.Transparent;
		itemSize = new Vector2(100f, 100f);
		currentState = SelectionState.Active;
		InventoryItemIcon = GetSetupInventoryDragDropButton();
		InventoryItemIcon.SetSize(new Vector2(103f, 103f));
		InventoryItemIcon.SetPosition(GUIControl.DockingAlignmentEnum.Middle, GUIControl.AnchorAlignmentEnum.Middle, GUIControl.OffsetType.Absolute, new Vector2(0f, 0f));
		SHSButtonStyleInfo buttonStyleInfo = ButtonStyleInfo;
		buttonStyleInfo.useHighlightAudio = false;
		InventoryItemIcon.StyleInfo = buttonStyleInfo;
		item.Add(InventoryItemIcon);
		InventoryItemIcon.HitTestType = GUIControl.HitTestTypeEnum.Circular;
		InventoryItemIcon.HitTestSize = new Vector2(0.6f, 0.6f);
		InventoryItemIcon.ToolTip = ItemHoverHelpInfo;
		NumberOfItems = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(26f, 26f), new Vector2(25f, 25f));
		NumberOfItems.TextureSource = "persistent_bundle|inventory_stacked_indicator";
		item.Add(NumberOfItems);
		ItemCount = GUIControl.CreateControlFrameCentered<GUILabel>(new Vector2(26f, 26f), new Vector2(25f, 25f));
		ItemCount.SetupText(GUIFontManager.SupportedFontEnum.Komica, 14, GUILabel.GenColor(26, 39, 62), TextAnchor.MiddleCenter);
		ItemCount.Traits.EventHandlingTrait = GUIControl.ControlTraits.EventHandlingEnum.Ignore;
		item.Add(ItemCount);
		if (CollectionItem.ShieldAgentOnly)
		{
			InventoryItemIcon.EntitlementFlag = Entitlements.EntitlementFlagEnum.ShieldHeroesAllow;
			if (!Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ShieldHeroesAllow))
			{
				InventoryItemIcon.ToolTip = new GUIControl.NamedToolTipInfo("#TT_RenewShieldAgentForHeroAccess");
				GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(40f, 40f), new Vector2(-18f, -18f));
				gUIImage.TextureSource = "persistent_bundle|expired_subscriber_item_icon";
				item.Add(gUIImage);
				ButtonUseable = false;
			}
		}
	}

	public virtual int CompareTo(SHSInventorySelectionItem other)
	{
		if (GetType() != other.GetType())
		{
			if (this is SHSInventoryHQItem)
			{
				return -1;
			}
			if (other is SHSInventoryHQItem)
			{
				return 1;
			}
			return (!(this is SHSInventoryHeroItem)) ? 1 : (-1);
		}
		return 0;
	}

	public virtual bool CompareTo(string id)
	{
		return false;
	}

	public virtual void UpdateItemCount()
	{
	}
}
