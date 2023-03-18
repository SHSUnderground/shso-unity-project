using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class UITriggerAdaptor : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string entitlementFlag;

	public bool RequiresConfirmation;

	public bool RequiresGlobalContent;

	public string ConfirmationText = string.Empty;

	protected bool DialogInUse;

	public virtual void Triggered()
	{
		if (DialogInUse)
		{
			CspUtils.DebugLog("Attempting to use a dialog already in use.");
			return;
		}
		if (!string.IsNullOrEmpty(entitlementFlag))
		{
			CspUtils.DebugLog("UITrigger to check entitlement flag " + entitlementFlag);
			string[] names = Enum.GetNames(typeof(Entitlements.EntitlementFlagEnum));
			if (!new List<string>(names).Contains(entitlementFlag))
			{
				CspUtils.DebugLog("Unknown entitlementflag: " + entitlementFlag);
				return;
			}
			if (!Singleton<Entitlements>.instance.PermissionCheck((Entitlements.EntitlementFlagEnum)(int)Enum.Parse(typeof(Entitlements.EntitlementFlagEnum), entitlementFlag)))
			{
				return;
			}
		}
		if (RequiresGlobalContent)
		{
			CspUtils.DebugLog("This item requires global content to be downloaded.");
			if (!AppShell.Instance.GlobalContentLoaded)
			{
				if (!SHSStagedDownloadWindow.DownloadStatusCurrentlyShowing)
				{
					CspUtils.DebugLog("Forcing the download status gadget to display.");
					Assembly executingAssembly = Assembly.GetExecutingAssembly();
					Type type = executingAssembly.GetType("SHSStagedDownloadWindow");
					GUIWindow gUIWindow = (GUIWindow)Activator.CreateInstance(type);
					GUIManager.Instance.UIRoots[GUIManager.UILayer.System].Add(gUIWindow);
					gUIWindow.Show(GUIControl.ModalLevelEnum.Default);
				}
				return;
			}
		}
		DialogInUse = true;
		if (RequiresConfirmation)
		{
			GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.YesNoDialog, ConfirmationText, delegate(string sId, GUIDialogWindow.DialogState state)
			{
				if (state == GUIDialogWindow.DialogState.Ok)
				{
					OnConfirmed();
				}
				else
				{
					DialogInUse = false;
				}
			}, GUIControl.ModalLevelEnum.Default);
		}
		else
		{
			OnConfirmed();
		}
	}

	protected virtual void Update()
	{
	}

	protected virtual void OnConfirmed()
	{
	}
}
