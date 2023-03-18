using System.Collections;
using UnityEngine;

public class BriefingWindowOpener : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string speakerCharacterId = "spider_man";

	public string briefingText = "#VO_SPIDER_MAN_MISSIONBRIEFING_M_1001_1_DOCOCK001";

	public float verticalOffset = -20f;

	public float postSpawnDelaySeconds = 2f;

	public float windowOpenDuration = 30f;

	private void Start()
	{
		AppShell.Instance.EventMgr.AddListener<LocalPlayerChangedMessage>(OnLocalPlayerChanged);
	}

	private void OnLocalPlayerChanged(LocalPlayerChangedMessage e)
	{
		if (e.localPlayer == null)
		{
			return;
		}
		AppShell.Instance.EventMgr.RemoveListener<LocalPlayerChangedMessage>(OnLocalPlayerChanged);
		SocialSpaceController socialSpaceController = GameController.GetController() as SocialSpaceController;
		if (socialSpaceController != null && AppShell.Instance.Profile != null)
		{
			string key = socialSpaceController.Controller.ZoneName + ":opened_briefing";
			if (!AppShell.Instance.Profile.SessionVars.ContainsKey(key) || !(bool)AppShell.Instance.Profile.SessionVars[key])
			{
				AppShell.Instance.Profile.SessionVars[key] = true;
				ShowBriefingWindow();
			}
		}
		else
		{
			ShowBriefingWindow();
		}
	}

	private void ShowBriefingWindow()
	{
		StartCoroutine(CoShowBriefingWindow());
	}

	private IEnumerator CoShowBriefingWindow()
	{
		yield return new WaitForSeconds(postSpawnDelaySeconds);
		SHSVOBrawlerObjectiveWindow window = SHSVOBrawlerObjectiveWindow.CreateWindow() as SHSVOBrawlerObjectiveWindow;
		window.Offset += new Vector2(0f, verticalOffset);
		window.SetCharacter(speakerCharacterId);
		window.SetText(briefingText);
		window.AnimateIn();
		yield return new WaitForSeconds(windowOpenDuration);
		if (window != null && window.IsVisible)
		{
			window.AnimateOut();
		}
	}
}
