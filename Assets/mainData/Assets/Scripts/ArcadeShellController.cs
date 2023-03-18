using System;
using System.Collections;
using System.Net;
using System.Text;
using UnityEngine;

public class ArcadeShellController : GameController
{
	protected static ArcadeShellController instance;

	public TextAsset javaScript;

	protected bool _ExitRequested;

	protected string _gameUrl;

	public static ArcadeShellController Instance
	{
		get
		{
			return instance;
		}
	}

	public override void Awake()
	{
		base.Awake();
		bCallControllerReadyFromStart = false;
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			CspUtils.DebugLog("A second ArcadeShell controller is being created.  This may lead to instabilities!");
		}
	}

	public override void Start()
	{
		base.Start();
		StartCoroutine(ArcadeMain());
	}

	public override void OnDisable()
	{
		base.OnDisable();
		instance = null;
	}

	protected IEnumerator ArcadeMain()
	{
		AppShell.Instance.BundleLoader.DownloadingPaused = true;
		while (!AppShell.Instance.IsReady)
		{
			yield return null;
		}
		while (StartTransaction == null)
		{
			yield return null;
		}
		string gameName = (string)AppShell.Instance.SharedHashTable["AracdeGame"];
		AppShell.Instance.SharedHashTable["AracdeGame"] = null;
		AppShell.Instance.Matchmaker2.Arcade(gameName);
		_gameUrl = null;
		string uri = "arcade$details/" + gameName + "?relative=true";
		AppShell.Instance.WebService.StartRequest(uri, OnGameUrl, null, ShsWebService.ShsWebServiceType.Text);
		while (_gameUrl == null)
		{
			yield return null;
		}
		StartTransaction.CompleteStep("get_url");
		yield return null;
		StartTransaction.CompleteStep("init");
		ControllerReady();
		yield return null;
		while (AppShell.Instance.TransitionHandler.CurrentWaitWindow != null && AppShell.Instance.TransitionHandler.CurrentWaitWindow.IsVisible)
		{
			yield return null;
		}
		AppShell.Instance.AudioManager.RequestCrossfade(null);
		if (AppShell.Instance.ForceWindowedMode())
		{
			yield return new WaitForSeconds(1f);
		}
		_ExitRequested = false;
		GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Native);
		SHSInputProxy inputProxy = AppShell.Instance.GetComponent<SHSInputProxy>();
		if (inputProxy != null)
		{
			inputProxy.enabled = false;
		}
		string session = string.Format("s={0}|a={1}", AppShell.Instance.WebService.SessionKey, AppShell.Instance.Profile.UserId.ToString());
		Application.ExternalEval("HEROUPNS.SetArcadeCookie('SHSA','" + session + "', 1);");
		yield return null;
		Application.targetFrameRate = 4;
		yield return null;
		string webservice2 = AppShell.Instance.WebService.BuildFullUri("arcade$", ShsWebService.ShsWebServiceType.Text, null);
		byte[] webserviceEncoded = Encoding.UTF8.GetBytes(webservice2);
		webservice2 = WWW.EscapeURL(Convert.ToBase64String(webserviceEncoded));
		string gameUrl = _gameUrl;
		_gameUrl = gameUrl + "?d=" + webservice2 + "&shs=1&loc=" + WWW.EscapeURL(AppShell.Instance.Locale);
		Application.ExternalEval("HEROUPNS.RunArcade('" + _gameUrl + "', '" + base.name + "', 'ExitArcade');");
		yield return null;
		while (!_ExitRequested)
		{
			yield return null;
		}
		Application.targetFrameRate = -1;
		yield return null;
		GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Normal);
		if (inputProxy != null)
		{
			inputProxy.enabled = true;
		}
		AppShell.Instance.BundleLoader.DownloadingPaused = false;
		if (AppShell.Instance.Profile != null)
		{
			AppShell.Instance.Profile.StartCurrencyFetch();
		}
		AppShell.Instance.Transition(ControllerType.SocialSpace);
	}

	protected void OnGameUrl(ShsWebResponse response)
	{
		HttpStatusCode status = (HttpStatusCode)response.Status;
		HttpStatusCode httpStatusCode = status;
		if (httpStatusCode == HttpStatusCode.OK)
		{
			_gameUrl = response.Body.Trim();
			CspUtils.DebugLog("Received game URL <" + _gameUrl + ">");
			if (AppShell.Instance.Locale != "en-us")
			{
				_gameUrl = _gameUrl.Replace("en-us", AppShell.Instance.Locale.ToLower());
				CspUtils.DebugLog("Localized game URL <" + _gameUrl + ">");
			}
			if (!_gameUrl.StartsWith("/"))
			{
				_gameUrl = "/" + _gameUrl;
			}
			_gameUrl = AppShell.Instance.SharedHashTable["ContentUri"] + _gameUrl;
		}
		else
		{
			CspUtils.DebugLog("Failed to retrieve arcade game url <" + status.ToString() + ">");
			AppShell.Instance.CriticalError(SHSErrorCodes.Code.ArcadeGameUrlFail, "Failed to download game details");
		}
	}

	public void ExitArcade(string result)
	{
		CspUtils.DebugLog("ExitArcade called");
		_ExitRequested = true;
	}

	public void OnGUI()
	{
		if (Application.isEditor && GUI.Button(new Rect(100f, 100f, 100f, 100f), "Exit"))
		{
			ExitArcade("Editor Force Exit");
		}
	}
}
