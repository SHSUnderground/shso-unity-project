using System;
using System.Collections.Generic;
using UnityEngine;

public class LauncherSequences
{
	private delegate void LauncherSequenceDelegate(UILauncherAdaptor launchAdaptor);

	private static Dictionary<LauncherTypeEnum, LauncherSequenceDelegate> launcherSequences;

	private static bool initialized;

	public static string FixedMissionKey;

	static LauncherSequences()
	{
		FixedMissionKey = "m_100X_1_Sabretooth001";
	}

	public static void InitiateLaunchSequence(LauncherTypeEnum launcherType)
	{
		initiateLaunchSequence(launcherType, null);
	}

	public static void InitiateLaunchSequence(UILauncherAdaptor launchAdaptor)
	{
		initiateLaunchSequence(launchAdaptor.LauncherType, launchAdaptor);
	}

	private static void initiateLaunchSequence(LauncherTypeEnum launcherType, UILauncherAdaptor adaptor)
	{
		if (!initialized)
		{
			initialize();
		}
		if (launcherSequences.ContainsKey(launcherType))
		{
			launcherSequences[launcherType](adaptor);
		}
		else
		{
			CspUtils.DebugLog("Can't find launcher sequence to invoke for: " + launcherType.ToString());
		}
	}

	private static void initialize()
	{
		launcherSequences = new Dictionary<LauncherTypeEnum, LauncherSequenceDelegate>();
		launcherSequences[LauncherTypeEnum.CardGame] = cardgameSequence;
		launcherSequences[LauncherTypeEnum.Hq] = hqSequence;
		launcherSequences[LauncherTypeEnum.Missions] = missionSequence;
		launcherSequences[LauncherTypeEnum.WorldMap] = worldmapSequence;
		launcherSequences[LauncherTypeEnum.Subscribe] = subscribeSequence;
		launcherSequences[LauncherTypeEnum.BuyGold] = buyGoldSequence;
		launcherSequences[LauncherTypeEnum.Shopping] = shoppingSequence;
		launcherSequences[LauncherTypeEnum.Arcade] = arcadeSequence;
		initialized = true;
	}

	public static void SequenceCompleted(UILauncherAdaptor adaptor)
	{
		if (adaptor != null)
		{
			adaptor.LaunchSequenceComplete();
		}
	}

	private static bool permissionCheck(Entitlements.EntitlementFlagEnum permitFlag)
	{
		return Singleton<Entitlements>.instance.PermissionCheck(permitFlag);
	}

	public static bool DependencyCheck(AssetBundleLoader.BundleGroup group)
	{
		return DependencyCheck(group, true);
	}

	public static bool DependencyCheck(AssetBundleLoader.BundleGroup group, bool ShowDialogOnDependencyFail)
	{
		bool bundleGroupDependenciesDone = AppShell.Instance.BundleLoader.GetBundleGroupDependenciesDone(group);
		if (!bundleGroupDependenciesDone && ShowDialogOnDependencyFail)
		{
			SHSStagedDownloadWindow sHSStagedDownloadWindow = new SHSStagedDownloadWindow();
			GUIManager.Instance.UIRoots[GUIManager.UILayer.System].Add(sHSStagedDownloadWindow);
			sHSStagedDownloadWindow.Show(GUIControl.ModalLevelEnum.Default);
		}
		string key = string.Format("{0}BundleGroupOverride", group.ToString());
		string @string = PlayerPrefs.GetString(key, "Normal");
		return @string == "ForceLoad" || (@string != "NeverLoad" && bundleGroupDependenciesDone);
	}

	public static bool DependencyCheck(AssetBundleLoader.BundleGroup group, out float percentComplete)
	{
		bool flag = false;
		percentComplete = 0f;
		if (DependencyCheck(group, false))
		{
			percentComplete = 1f;
			flag = true;
		}
		if (!flag)
		{
			percentComplete = AppShell.Instance.BundleLoader.GetBundleGroupDependenciesPercentageDone(group);
		}
		string key = string.Format("{0}BundleGroupOverride", group.ToString());
		string @string = PlayerPrefs.GetString(key, "Normal");
		return @string == "ForceLoad" || (@string != "NeverLoad" && flag);
	}

	private static void arcadeSequence(UILauncherAdaptor adaptor)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected O, but got Unknown
		if (!CanLaunchArcade())
		{
			SequenceCompleted(adaptor);
		}
		else
		{
			LaunchArcade((Action)(object)(Action)delegate
			{
				SequenceCompleted(adaptor);
			}, adaptor);
		}
	}

	public static bool CanLaunchArcade()
	{
		if (!permissionCheck(Entitlements.EntitlementFlagEnum.ArcadeAllow))
		{
			GUIManager.Instance.ShowErrorDialog(SHSErrorCodes.Code.ArcadeUnavailable, string.Empty);
			return false;
		}
		return true;
	}

	public static void LaunchArcade(Action onFinish, UILauncherAdaptor adaptor)
	{
		//Discarded unreachable code: IL_00c5
		try
		{
			GUIManager.Instance.ShowDialog(typeof(SHSArcadeGadget), string.Empty, "SHSMainWindow", new GUIDialogNotificationSink(delegate(string Id, IGUIDialogWindow window)
			{
				if (adaptor != null)
				{
					window.LinkedObject = adaptor.gameObject;
				}
			}, delegate
			{
			}, delegate
			{
			}, delegate
			{
			}, delegate
			{
				if (onFinish != null)
				{
					onFinish.Invoke();
				}
			}), GUIControl.ModalLevelEnum.Default);
		}
		catch (Exception ex)
		{
			if (onFinish != null)
			{
				onFinish.Invoke();
			}
			throw ex;
		}
	}

	private static void cardgameSequence(UILauncherAdaptor adaptor)
	{
		//Discarded unreachable code: IL_01bf
		if (!permissionCheck(Entitlements.EntitlementFlagEnum.CardGameAllow))
		{
			GUIManager.Instance.ShowErrorDialog(SHSErrorCodes.Code.CardGameUnavailable, string.Empty);
			SequenceCompleted(adaptor);
		}
		else if (!DependencyCheck(AssetBundleLoader.BundleGroup.CardGame))
		{
			SequenceCompleted(adaptor);
		}
		else if (!Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.ParentalCardGameDeny))
		{
			GUIManager.Instance.ShowErrorDialog(SHSErrorCodes.Code.ParentalControlLimit, string.Empty);
			SequenceCompleted(adaptor);
		}
		else
		{
			try
			{
				foreach (IGUIContainer root in GUIManager.Instance.Roots)
				{
					List<SHSCardGameGadgetWindow> controlsOfType = root.GetControlsOfType<SHSCardGameGadgetWindow>(true);
					foreach (SHSCardGameGadgetWindow item in controlsOfType)
					{
						CspUtils.DebugLog("Closing existing gadget " + item.Id);
						item.CloseGadget();
					}
				}
				GUIManager.Instance.ShowDialog(typeof(SHSCardGameGadgetWindow), string.Empty, "SHSMainWindow", new GUIDialogNotificationSink(delegate(string Id, IGUIDialogWindow window)
				{
					if (adaptor != null)
					{
						window.LinkedObject = adaptor.gameObject;
					}
				}, delegate
				{
				}, delegate
				{
				}, delegate
				{
				}, delegate
				{
					SequenceCompleted(adaptor);
				}), GUIControl.ModalLevelEnum.Default);
			}
			catch (Exception ex)
			{
				SequenceCompleted(adaptor);
				throw ex;
			}
		}
	}

	private static void hqSequence(UILauncherAdaptor adaptor)
	{
		if (AppShell.Instance.SharedHashTable[HqController2.HQ_USER_ID_KEY] != null)
		{
			AppShell.Instance.SharedHashTable.Remove(HqController2.HQ_USER_ID_KEY);
		}
		if (!DependencyCheck(AssetBundleLoader.BundleGroup.HQ))
		{
			SequenceCompleted(adaptor);
			return;
		}
		GUIDialogNotificationSink notificationSink = new GUIDialogNotificationSink(delegate(string Id, GUIDialogWindow.DialogState state)
		{
			//Discarded unreachable code: IL_002a
			if (state == GUIDialogWindow.DialogState.Ok)
			{
				try
				{
					SequenceCompleted(adaptor);
					launchHq();
				}
				catch (Exception ex)
				{
					SequenceCompleted(adaptor);
					throw ex;
				}
			}
			else
			{
				SequenceCompleted(adaptor);
			}
		});
		SHSCommonDialogWindow sHSCommonDialogWindow = new SHSCommonDialogWindow("common_bundle|notification_icon_hqgoto", new Vector2(188f, 235f), string.Empty, "common_bundle|L_mshs_button_yes", "common_bundle|L_mshs_button_no", typeof(SHSDialogYesButton), typeof(SHSDialogNoButton), false);
		sHSCommonDialogWindow.TitleText = "#confirm_hqtransition";
		sHSCommonDialogWindow.Text = "#CONFIRM_HQ_VISIT";
		sHSCommonDialogWindow.NotificationSink = notificationSink;
		GUIManager.Instance.ShowDynamicWindow(sHSCommonDialogWindow, GUIControl.ModalLevelEnum.Full);
	}

	private static void launchHq()
	{
		AppShell.Instance.Transition(GameController.ControllerType.HeadQuarters);
	}

	private static void missionSequence(UILauncherAdaptor adaptor)
	{
		//Discarded unreachable code: IL_00dc
		if (!permissionCheck(Entitlements.EntitlementFlagEnum.MissionsPermitSet))
		{
			GUIManager.Instance.ShowErrorDialog(SHSErrorCodes.Code.MissionsUnavailable, string.Empty);
			SequenceCompleted(adaptor);
		}
		else
		{
			try
			{
				GUIManager.Instance.ShowDialog(typeof(SHSBrawlerGadget), string.Empty, "SHSMainWindow", new GUIDialogNotificationSink(delegate(string Id, IGUIDialogWindow window)
				{
					if (adaptor != null)
					{
						window.LinkedObject = adaptor.gameObject;
					}
				}, delegate
				{
				}, delegate
				{
				}, delegate
				{
				}, delegate
				{
					SequenceCompleted(adaptor);
				}), GUIControl.ModalLevelEnum.Default);
			}
			catch (Exception ex)
			{
				SequenceCompleted(adaptor);
				throw ex;
			}
		}
	}

	private static void shoppingSequence(UILauncherAdaptor adaptor)
	{
		if (!DependencyCheck(AssetBundleLoader.BundleGroup.Characters))
		{
			SequenceCompleted(adaptor);
		}
	}

	private static void buyGoldSequence(UILauncherAdaptor adaptor)
	{
		//Discarded unreachable code: IL_00c2
		try
		{
			SHSUpsellWindow windowref = null;
			GUIManager.Instance.ShowDialog(typeof(SHSUpsellGoldWindow), "#subscription_bumper_message", new GUIDialogNotificationSink(delegate(string Id, IGUIDialogWindow window)
			{
				if (adaptor != null)
				{
					window.LinkedObject = adaptor.gameObject;
				}
				windowref = (window as SHSUpsellWindow);
			}, delegate
			{
			}, delegate
			{
			}, delegate
			{
			}, delegate(string Id, GUIDialogWindow.DialogState state)
			{
				if (state == GUIDialogWindow.DialogState.Ok)
				{
					ShsWebService.SafeJavaScriptCall("HEROUPNS.RedirectToStore();");
					SequenceCompleted(adaptor);
				}
				else
				{
					SequenceCompleted(adaptor);
				}
			}), GUIControl.ModalLevelEnum.Default);
		}
		catch (Exception ex)
		{
			SequenceCompleted(adaptor);
			throw ex;
		}
	}

	private static void subscribeSequence(UILauncherAdaptor adaptor)
	{
		//Discarded unreachable code: IL_00c2
		try
		{
			SHSUpsellWindow windowref = null;
			GUIManager.Instance.ShowDialog(typeof(SHSUpsellWindow), "#subscription_bumper_message", new GUIDialogNotificationSink(delegate(string Id, IGUIDialogWindow window)
			{
				if (adaptor != null)
				{
					window.LinkedObject = adaptor.gameObject;
				}
				windowref = (window as SHSUpsellWindow);
			}, delegate
			{
			}, delegate
			{
			}, delegate
			{
			}, delegate(string Id, GUIDialogWindow.DialogState state)
			{
				if (state == GUIDialogWindow.DialogState.Ok)
				{
					ShsWebService.SafeJavaScriptCall("HEROUPNS.RedirectToSubscription('" + windowref.SubscriptionPlan + "');");
					SequenceCompleted(adaptor);
				}
				else
				{
					SequenceCompleted(adaptor);
				}
			}), GUIControl.ModalLevelEnum.Default);
		}
		catch (Exception ex)
		{
			SequenceCompleted(adaptor);
			throw ex;
		}
	}

	private static void worldmapSequence(UILauncherAdaptor adaptor)
	{
		//Discarded unreachable code: IL_00b3
		try
		{
			GUIManager.Instance.ShowDialog(typeof(SHSZoneSelectorGadget), string.Empty, "SHSMainWindow", new GUIDialogNotificationSink(delegate(string Id, IGUIDialogWindow window)
			{
				if (adaptor != null)
				{
					window.LinkedObject = adaptor.gameObject;
				}
			}, delegate
			{
			}, delegate
			{
			}, delegate
			{
			}, delegate
			{
				SequenceCompleted(adaptor);
			}), GUIControl.ModalLevelEnum.Default);
		}
		catch (Exception ex)
		{
			SequenceCompleted(adaptor);
			throw ex;
		}
	}
}
