using UnityEngine;

public class ChallengeFlightEvent : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string eventOnLaunch;

	public SplineController[] eventSplines;

	protected void OnEntityTakeof(EntityTakeoffMessage msg)
	{
		if (!(AppShell.Instance == null) && AppShell.Instance.EventMgr != null && !string.IsNullOrEmpty(eventOnLaunch) && Utils.IsLocalPlayer(msg.entity) && IsChallengeSpline(msg.spline))
		{
			AppShell.Instance.delayedAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "social_zone", eventOnLaunch, string.Empty, 1f);
		}
	}

	protected bool IsChallengeSpline(SplineController spline)
	{
		if (eventSplines == null)
		{
			return false;
		}
		SplineController[] array = eventSplines;
		foreach (SplineController x in array)
		{
			if (x == spline)
			{
				return true;
			}
		}
		return false;
	}

	private void Start()
	{
		if (AppShell.Instance != null && AppShell.Instance.EventMgr != null)
		{
			AppShell.Instance.EventMgr.AddListener<EntityTakeoffMessage>(OnEntityTakeof);
		}
	}

	private void OnDisable()
	{
		if (AppShell.Instance != null && AppShell.Instance.EventMgr != null)
		{
			AppShell.Instance.EventMgr.RemoveListener<EntityTakeoffMessage>(OnEntityTakeof);
		}
	}
}
