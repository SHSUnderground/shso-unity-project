using UnityEngine;

public class SHSInventoryExpendableItem : SHSInventorySelectionItem
{
	public Expendable expendable;

	private PrerequisiteCheckStateEnum _prereqState;

	public override GUIControl.ToolTipInfo ItemHoverHelpInfo
	{
		get
		{
			return new GUIControl.InventoryHoverHelpInfo(expendable);
		}
	}

	public override SHSButtonStyleInfo ButtonStyleInfo
	{
		get
		{
			return new SHSButtonStyleInfo(expendable.Definition.InventoryIcon, SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		}
	}

	public override ShsCollectionItem CollectionItem
	{
		get
		{
			return expendable;
		}
	}

	public override int CollectionItemCount
	{
		get
		{
			return expendable.Quantity;
		}
	}

	public SHSInventoryExpendableItem(Expendable expendable, SHSInventoryAnimatedWindow headWindow)
		: base(headWindow)
	{
		this.expendable = expendable;
		SetupWindow();
		InventoryItemIcon.SetSize(new Vector2(90f, 90f));
		InventoryItemIcon.HitTestSize = new Vector2(1f, 1f);
		ItemCount.Text = expendable.Quantity.ToString();
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
				InventoryItemIcon.ToolTip = new GUIControl.InventoryHoverHelpInfo(expendable, result.StateExplanation);
			}
			if (GUIManager.Instance != null && GUIManager.Instance.TooltipManager != null && GUIManager.Instance.TooltipManager.CurrentOverControl == InventoryItemIcon)
			{
				GUIManager.Instance.TooltipManager.RefreshToolTip();
			}
		}
	}

	public override GUIButton GetSetupInventoryDragDropButton()
	{
		return new SHSInventoryExpendableButton(expendable);
	}

	public override int CompareTo(SHSInventorySelectionItem other)
	{
		int num = base.CompareTo(other);
		if (num != 0)
		{
			return num;
		}
		string text = expendable.Definition.Name;
		string text2 = ((SHSInventoryExpendableItem)other).expendable.Definition.Name;
		if (AppShell.Instance.stringTable != null)
		{
			text = AppShell.Instance.stringTable[text];
			text2 = AppShell.Instance.stringTable[text2];
		}
		return text.CompareTo(text2);
	}

	public override bool CompareTo(string id)
	{
		return expendable.Definition.OwnableTypeId == id;
	}

	public override void UpdateItemCount()
	{
		ItemCount.Text = expendable.Quantity.ToString();
	}
}
