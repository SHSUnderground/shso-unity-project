using System;
using UnityEngine;

public class FrontEndController : GameController
{
	public override void Awake()
	{
		base.Awake();
		bCallControllerReadyFromStart = false;
		AppShell.Instance.EventMgr.AddListener<ApplicationInitializedMessage>(OnAppShellReady);
	}

	public override void Start()
	{
		base.Start();
		if (AppShell.Instance != null && AppShell.Instance.IsReady)
		{
			string value;
			AppShell.Instance.WebService.UrlParameters.TryGetValue("token", out value);
			if (!string.IsNullOrEmpty(value))
			{
				Application.ExternalCall("HEROUPNS.RedirectToLogin");
			}
			else
			{
				OnAppShellReady(null);
			}
		}
	}

	protected void OnAppShellReady(ApplicationInitializedMessage msg)
	{
		CspUtils.DebugLog("!!!!!!!!!!!!!!!!!!!!!!!!!!OnAppShellReady called!!!!!!!!!!!!!!!!!!!");
		AppShell.Instance.EventMgr.RemoveListener<ApplicationInitializedMessage>(OnAppShellReady);
		PreloadingSplashScreen preloadingSplashScreen = UnityEngine.Object.FindObjectOfType(typeof(PreloadingSplashScreen)) as PreloadingSplashScreen;
		if (preloadingSplashScreen != null)
		{
			preloadingSplashScreen.enabled = false;
		}
		else
		{
			CspUtils.DebugLog("------- There was no Preloading screen!");
		}
		bool urlParametersLoaded = AppShell.Instance.WebService.UrlParametersLoaded;
		bool serverConfigurationLoaded = AppShell.Instance.WebService.ServerConfigurationLoaded;
		bool flag = false;
		string value;
		if (AppShell.Instance.WebService.UrlParameters.TryGetValue("auto", out value))
		{
			flag = value.Equals("true", StringComparison.OrdinalIgnoreCase);
		}
		if (urlParametersLoaded && serverConfigurationLoaded && !flag)
		{
			CspUtils.DebugLog("Front end controller denotes a web based credential login.");
			string value2;
			AppShell.Instance.WebService.UrlParameters.TryGetValue("token", out value2);
			if (!string.IsNullOrEmpty(value2))
			{
				CspUtils.DebugLog("Attempting to login given token credentials " + value2);
				AppShell.Instance.EventMgr.AddListener<LoginCompleteMessage>(OnLoginCompleted);
				AppShell.Instance.ServerConnection.Login(value2);
			}
		}
		else if (flag || !Application.isWebPlayer)
		{
			CspUtils.DebugLog("!!!!!!!!!!!!!!!!!!!!!!!!!!OnAppShellReady first else !!!!!!!!!!!!!!!!!!!");
			if (AppShell.Instance.LaunchTransitionTransaction != null)
			{
				AppShell.Instance.LaunchTransitionTransaction.CompleteStep("initialize");
			}
			if (AppShell.Instance.LaunchLoginTransaction != null)
			{
				AppShell.Instance.LaunchLoginTransaction.CompleteStep("initialize");
			}
			PrepareFrontEndWindowCleanup();
			GUIManager.Instance.Transition("SHSFrontendWindow/SHSLoginWindow", StartTransaction);
			ControllerReady();
		}
		else
		{
			CspUtils.DebugLog("Web build of Unity has no credentials and is not in automation mode. Returning to login");
			Application.ExternalCall("HEROUPNS.RedirectToLogin");
		}
	}

	private void OnLoginCompleted(LoginCompleteMessage message)
	{
		AppShell.Instance.EventMgr.RemoveListener<LoginCompleteMessage>(OnLoginCompleted);
		ControllerReady();
		PrepareFrontEndWindowCleanup();
		if (message.status == LoginCompleteMessage.LoginStatus.LoginSucceeded)
		{
			AppShell.Instance.SharedHashTable["SocialSpaceLevel"] = "Daily_Bugle";
			ControllerType @int = (ControllerType)PlayerPrefs.GetInt("startupscene", 8);
			AppShell.Instance.Transition(@int, true);
		}
		else
		{
			AppShell.Instance.CriticalError(SHSErrorCodes.Code.UnableToConnect, "Login complete with status 'failed': " + message.message);
		}
	}

	private bool PrepareFrontEndWindowCleanup()
	{
		AppShell.Instance.CurrentControllerType = ControllerType.FrontEnd;
		bool alreadyDynamic = false;
		GUIManager.Instance.HookPreloadedWindowToDynamicTable("SHSFrontendWindow", "SHSLoginWindow", ref alreadyDynamic);
		GUIManager.Instance.HookPreloadedWindowToDynamicTable("SHSFrontendWindow", "SHSSystemMainWindow", ref alreadyDynamic);
		GUIManager.Instance.HookPreloadedWindowToDynamicTable("SHSFrontendWindow/SHSSystemMainWindow", "SHSSysOptionsWindow", ref alreadyDynamic);
		return alreadyDynamic;
	}
}
