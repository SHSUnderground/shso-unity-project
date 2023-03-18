using System.Collections;
using UnityEngine;

public class ScoreWindowHider : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public void OnEnable()
	{
		if (GUIManager.Instance != null)
		{
			SHSBrawlerMainWindow sHSBrawlerMainWindow = (SHSBrawlerMainWindow)GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow"];
			if (sHSBrawlerMainWindow != null)
			{
				sHSBrawlerMainWindow.ShowScoreWindow = false;
			}
			else
			{
				StartCoroutine(RegisterForEvents());
			}
		}
		else
		{
			StartCoroutine(RegisterForEvents());
		}
	}

	private IEnumerator RegisterForEvents()
	{
		while (AppShell.Instance == null || AppShell.Instance.EventMgr == null)
		{
			yield return 0;
		}
		AppShell.Instance.EventMgr.AddListener<BrawlerMainWindowInitializedMessage>(OnBrawlerMainWindowInitialized);
	}

	public void OnDisable()
	{
		if (AppShell.Instance != null && AppShell.Instance.EventMgr != null)
		{
			AppShell.Instance.EventMgr.RemoveListener<BrawlerMainWindowInitializedMessage>(OnBrawlerMainWindowInitialized);
		}
		if (GUIManager.Instance != null)
		{
			SHSBrawlerMainWindow sHSBrawlerMainWindow = (SHSBrawlerMainWindow)GUIManager.Instance["/SHSMainWindow/SHSBrawlerMainWindow"];
			if (sHSBrawlerMainWindow != null)
			{
				sHSBrawlerMainWindow.ShowScoreWindow = true;
			}
		}
	}

	private void OnBrawlerMainWindowInitialized(BrawlerMainWindowInitializedMessage e)
	{
		if (e.Window != null)
		{
			e.Window.ShowScoreWindow = false;
		}
	}
}
