using UnityEngine;

public class SHSInventoryHQItem : SHSInventorySelectionItem
{
	public Item InventoryItem;

	private AnimClip HoverAnimation;

	public override GUIControl.ToolTipInfo ItemHoverHelpInfo
	{
		get
		{
			return new GUIControl.InventoryHoverHelpInfo(InventoryItem);
		}
	}

	public override SHSButtonStyleInfo ButtonStyleInfo
	{
		get
		{
			return new SHSButtonStyleInfo("items_bundle|" + InventoryItem.Definition.Icon, SHSButtonStyleInfo.SupportedStatesEnum.Normal);
		}
	}

	public override ShsCollectionItem CollectionItem
	{
		get
		{
			return InventoryItem;
		}
	}

	public override int CollectionItemCount
	{
		get
		{
			return InventoryItem.Quantity;
		}
	}

	public SHSInventoryHQItem(Item InventoryItem, SHSInventoryAnimatedWindow headWindow)
		: base(headWindow)
	{
		this.InventoryItem = InventoryItem;
		SetupWindow();
		InventoryItemIcon.SetSize(new Vector2(59f, 59f));
		InventoryItemIcon.HitTestSize = new Vector2(1f, 1f);
		UpdateItemCount();
		InventoryItemIcon.MouseOver += delegate
		{
			headWindow.AnimationPieceManager.SwapOut(ref HoverAnimation, SHSAnimations.Generic.ChangeSizeDirect(InventoryItemIcon, new Vector2(64f, 64f), new Vector2(59f, 59f), 0.1f, 0f));
		};
		InventoryItemIcon.MouseOut += delegate
		{
			headWindow.AnimationPieceManager.SwapOut(ref HoverAnimation, SHSAnimations.Generic.ChangeSizeDirect(InventoryItemIcon, new Vector2(59f, 59f), new Vector2(64f, 64f), 0.1f, 0f));
		};
		item.ConfigureRequiredContent(new ContentReference(ContentTypeEnum.Bundle, item.Id));
		base.IsEnabled = (!CollectionItem.ShieldAgentOnly || Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ShieldHQAllow));
	}

	public override GUIButton GetSetupInventoryDragDropButton()
	{
		item.Id = InventoryItem.Definition.PlacedObjectAssetBundle;
		string text = "items_bundle|" + InventoryItem.Definition.Icon;
		SHSInventoryHQButton sHSInventoryHQButton = new SHSInventoryHQButton(headWindow, InventoryItem.Id, text, DragDropInfo.CollectionType.Items, "Items", delegate
		{
			return InventoryItem.Placed < InventoryItem.Quantity && ButtonUseable;
		});
		sHSInventoryHQButton.TextureSource = text;
		return sHSInventoryHQButton;
	}

	public override int CompareTo(SHSInventorySelectionItem other)
	{
		int num = base.CompareTo(other);
		if (num != 0)
		{
			return num;
		}
		return InventoryItem.Name.CompareTo(((SHSInventoryHQItem)other).InventoryItem.Name);
	}

	public override bool CompareTo(string id)
	{
		return InventoryItem.Id.ToString() == id;
	}

	public override void UpdateItemCount()
	{
		if (InventoryItem.Placed == 0)
		{
			ItemCount.Text = InventoryItem.Quantity + string.Empty;
		}
		else
		{
			ItemCount.Text = InventoryItem.Quantity - InventoryItem.Placed + "*";
		}
	}
}
