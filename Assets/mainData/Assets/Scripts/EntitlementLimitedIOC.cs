using System.Collections.Generic;
using UnityEngine;

public class EntitlementLimitedIOC : InteractiveObjectController
{
	public Entitlements.EntitlementFlagEnum[] requiredEntitlements;

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		List<Entitlements.EntitlementFlagEnum> list = new List<Entitlements.EntitlementFlagEnum>();
		Entitlements.EntitlementFlagEnum[] array = requiredEntitlements;
		foreach (Entitlements.EntitlementFlagEnum entitlementFlagEnum in array)
		{
			if (!Singleton<Entitlements>.instance.PermissionCheck(entitlementFlagEnum))
			{
				list.Add(entitlementFlagEnum);
			}
		}
		if (list.Count == 0)
		{
			return OnEntitlementCheckPassed(player, onDone);
		}
		return OnEntitlementCheckFailed(list, player, onDone);
	}

	protected virtual bool OnEntitlementCheckPassed(GameObject player, OnDone onDone)
	{
		return base.StartWithPlayer(player, onDone);
	}

	protected virtual bool OnEntitlementCheckFailed(List<Entitlements.EntitlementFlagEnum> failedEntitlements, GameObject player, OnDone onDone)
	{
		string text = "You currently cannot use this object:";
		foreach (Entitlements.EntitlementFlagEnum failedEntitlement in failedEntitlements)
		{
			text = text + "\n" + GetEntitlementString(failedEntitlement);
		}
		GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.OkDialog, text, (IGUIDialogNotification)null, GUIControl.ModalLevelEnum.Default);
		return false;
	}

	protected string GetEntitlementString(Entitlements.EntitlementFlagEnum entitlement)
	{
		Entitlements.Entitlement value;
		if (Singleton<Entitlements>.instance.EntitlementsSet == null || !Singleton<Entitlements>.instance.EntitlementsSet.TryGetValue(entitlement, out value) || value == null || string.IsNullOrEmpty(value.DenyReason))
		{
			return entitlement.ToString();
		}
		return value.DenyReason;
	}
}
