using System;
using UnityEngine;

public class SHSInventoryHeroButton : GUIButton
{
	private SHSInventoryAnimatedWindow headWindow;

	private string ItemId;

	private string IconSource;

	private DragDropInfo.CollectionType CollectionId;

	private string CollectionResetString;

	private Func<bool> DragPermitted;

	public override bool CanDrag
	{
		get
		{
			DragDropInfo dragDropInfo = new DragDropInfo(this);
			dragDropInfo.ItemId = ItemId;
			dragDropInfo.CollectionId = CollectionId;
			bool flag = true;
			if (HqController2.Instance != null && !HqController2.Instance.CanDrag(dragDropInfo))
			{
				flag = false;
			}
			bool flag2 = true;
			if (DragPermitted != null)
			{
				flag2 = DragPermitted.Invoke();
			}
			return canDrag && Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.InventoryDragDropMode) && !headWindow.AnimationInProgress && flag2 && flag;
		}
	}

	public SHSInventoryHeroButton(SHSInventoryAnimatedWindow headWindow, string ItemId, string IconSource, DragDropInfo.CollectionType CollectionId, string CollectionResetString, Func<bool> DragPermitted)
	{
		this.headWindow = headWindow;
		this.ItemId = ItemId;
		this.IconSource = IconSource;
		this.CollectionId = CollectionId;
		this.CollectionResetString = CollectionResetString;
		this.DragPermitted = DragPermitted;
		canDrag = true;
		Click += delegate
		{
		};
	}

	public override void SetDragInfo(DragDropInfo DragDropInfo)
	{
		base.SetDragInfo(DragDropInfo);
		DragDropInfo.ItemId = ItemId;
		DragDropInfo.IconSource = IconSource;
		DragDropInfo.IconSize = new Vector2(103f, 103f);
		DragDropInfo.CollectionId = CollectionId;
	}

	public override void OnDragBegin(DragDropInfo DragBeginInfo)
	{
		base.OnDragBegin(DragBeginInfo);
		string[] keys = new string[1]
		{
			ItemId
		};
		AppShell.Instance.EventMgr.Fire(this, new CollectionResetMessage(keys, CollectionResetMessage.ActionType.Remove, CollectionResetString));
	}
}
