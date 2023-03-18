using System;
using UnityEngine;

public class ChallengeZone : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string challengeEvent;

	protected bool localPlayerInZone;

	private string lastEventSent = string.Empty;

	private DateTime lastEventSentTime = DateTime.MinValue;

	public bool LocalPlayerInZone
	{
		get
		{
			return localPlayerInZone;
		}
	}

	public virtual bool IsZoneChallengeMet()
	{
		return localPlayerInZone;
	}

	protected virtual void OnChallengeZoneEnter(GameObject go)
	{
		if (GameController.GetController() != null && GameController.GetController().LocalPlayer == go)
		{
			localPlayerInZone = true;
		}
		if (IsZoneChallengeMet())
		{
			FireChallengeZoneEvent();
		}
	}

	protected virtual void OnChallengeZoneExit(GameObject go)
	{
		if (GameController.GetController() != null && GameController.GetController().LocalPlayer == go)
		{
			localPlayerInZone = false;
		}
	}

	protected void FireChallengeZoneEvent()
	{
		if (AppShell.Instance != null && AppShell.Instance.EventMgr != null && !string.IsNullOrEmpty(challengeEvent) && (!(lastEventSent == challengeEvent) || (DateTime.Now - lastEventSentTime).Seconds >= 10))
		{
			lastEventSent = challengeEvent;
			lastEventSentTime = DateTime.Now;
			if (AppShell.Instance != null && AppShell.Instance.EventReporter != null && AppShell.Instance.Profile != null)
			{
				AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "social_zone", challengeEvent, 1, string.Empty);
			}
			else
			{
				CspUtils.DebugLog("Firing Challenge Zone Event: " + challengeEvent);
			}
		}
	}

	protected virtual void OnDisable()
	{
		localPlayerInZone = false;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other != null)
		{
			OnChallengeZoneEnter(other.gameObject);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other != null)
		{
			OnChallengeZoneExit(other.gameObject);
		}
	}
}
