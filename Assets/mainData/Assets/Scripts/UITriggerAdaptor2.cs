using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class UITriggerAdaptor2 : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum LocalBundleGroup
	{
		TutorialUIBugle,
		PrizeWheel,
		Characters,
		MissionsAndEnemies,
		NonBugleGameWorlds,
		HQ,
		CardGame,
		All,
		Any
	}

	public string entitlementFlag;

	public bool RequiresConfirmation;

	public LocalBundleGroup requiredBundleGroup = LocalBundleGroup.Any;

	public string ConfirmationText = string.Empty;

	protected bool DialogInUse;

	protected AssetBundleLoader.BundleGroup BundleGroupFromLocal(LocalBundleGroup localGroup)
	{
		switch (localGroup)
		{
		case LocalBundleGroup.TutorialUIBugle:
			return AssetBundleLoader.BundleGroup.TutorialUIBugle;
		case LocalBundleGroup.PrizeWheel:
			return AssetBundleLoader.BundleGroup.PrizeWheel;
		case LocalBundleGroup.Characters:
			return AssetBundleLoader.BundleGroup.Characters;
		case LocalBundleGroup.NonBugleGameWorlds:
			return AssetBundleLoader.BundleGroup.NonBugleGameWorlds;
		case LocalBundleGroup.HQ:
			return AssetBundleLoader.BundleGroup.HQ;
		case LocalBundleGroup.CardGame:
			return AssetBundleLoader.BundleGroup.CardGame;
		case LocalBundleGroup.All:
			return AssetBundleLoader.BundleGroup.All;
		case LocalBundleGroup.Any:
			return AssetBundleLoader.BundleGroup.Any;
		default:
			CspUtils.DebugLog("Unkown local bundle group <" + localGroup.ToString() + ">");
			return AssetBundleLoader.BundleGroup.Any;
		}
	}

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
		if (requiredBundleGroup != LocalBundleGroup.Any)
		{
			AssetBundleLoader.BundleGroup bundleGroup = BundleGroupFromLocal(requiredBundleGroup);
			CspUtils.DebugLog("This item requires <" + bundleGroup.ToString() + "> to be downloaded.");
			if (!AppShell.Instance.BundleLoader.GetBundleGroupDependenciesDone(bundleGroup))
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
