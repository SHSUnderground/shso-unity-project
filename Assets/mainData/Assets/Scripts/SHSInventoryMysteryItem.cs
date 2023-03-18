using UnityEngine;

public class SHSInventoryMysteryItem : SHSInventorySelectionItem
{
	public MysteryBox mysteryBox;

	private PrerequisiteCheckStateEnum _prereqState;

	public override GUIControl.ToolTipInfo ItemHoverHelpInfo
	{
		get
		{
			return new GUIControl.GenericHoverHelpInfo("#MYSTERYBOX_" + mysteryBox.Definition.name + "_NAME", "#MYSTERYBOX_" + mysteryBox.Definition.name + "_DESC", mysteryBox.Definition.shoppingIcon, mysteryBox.Definition.shoppingIconSize);
		}
	}

	public override SHSButtonStyleInfo ButtonStyleInfo
	{
		get
		{
			return new SHSButtonStyleInfo(mysteryBox.Definition.iconBase, SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		}
	}

	public override ShsCollectionItem CollectionItem
	{
		get
		{
			return mysteryBox;
		}
	}

	public override int CollectionItemCount
	{
		get
		{
			return mysteryBox.Quantity;
		}
	}

	public SHSInventoryMysteryItem(MysteryBox mysteryBox, SHSInventoryAnimatedWindow headWindow)
		: base(headWindow)
	{
		this.mysteryBox = mysteryBox;
		SetupWindow();
		InventoryItemIcon.SetSize(new Vector2(90f, 90f));
		InventoryItemIcon.HitTestSize = new Vector2(1f, 1f);
		ItemCount.Text = mysteryBox.Quantity.ToString();
		base.IsEnabled = (!CollectionItem.ShieldAgentOnly || Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ShieldPlayAllow));
		_prereqState = PrerequisiteCheckStateEnum.Usable;
	}

	public void Enable(PrerequisiteCheckResult result)
	{
		if (_prereqState != result.State)
		{
			_prereqState = result.State;
			base.IsEnabled = (result.State == PrerequisiteCheckStateEnum.Usable);
			if (result.State == PrerequisiteCheckStateEnum.Usable)
			{
				InventoryItemIcon.ToolTip = ItemHoverHelpInfo;
			}
			else
			{
				InventoryItemIcon.ToolTip = new GUIControl.InventoryHoverHelpInfo(mysteryBox, result.StateExplanation);
			}
			if (GUIManager.Instance != null && GUIManager.Instance.TooltipManager != null && GUIManager.Instance.TooltipManager.CurrentOverControl == InventoryItemIcon)
			{
				GUIManager.Instance.TooltipManager.RefreshToolTip();
			}
		}
	}

	public override GUIButton GetSetupInventoryDragDropButton()
	{
		return new SHSInventoryMysteryButton(mysteryBox);
	}

	public override int CompareTo(SHSInventorySelectionItem other)
	{
		int num = base.CompareTo(other);
		if (num != 0)
		{
			return num;
		}
		string text = mysteryBox.Definition.name;
		string text2 = ((SHSInventoryMysteryItem)other).mysteryBox.Definition.name;
		if (AppShell.Instance.stringTable != null)
		{
			text = AppShell.Instance.stringTable[text];
			text2 = AppShell.Instance.stringTable[text2];
		}
		return text.CompareTo(text2);
	}

	public override bool CompareTo(string id)
	{
		return string.Empty + mysteryBox.Definition.ownableTypeID == id;
	}

	public override void UpdateItemCount()
	{
		ItemCount.Text = mysteryBox.Quantity.ToString();
	}
}
