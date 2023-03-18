using UnityEngine;

public class CoinPickupActivityObject : ActivityObject
{
	public GameObject soundEffectPrefab;

	public override void ActionTriggered(ActivityObjectActionNameEnum action)
	{
		AppShell.Instance.EventReporter.ReportAwardTokens(1);
		if (soundEffectPrefab != null)
		{
			GameObject gameObject = Object.Instantiate(soundEffectPrefab, base.transform.position, Quaternion.identity) as GameObject;
			gameObject.AddComponent(typeof(SuicideOnStop));
		}
		NotificationHUD.addNotification(new TotalSilverNotificationData(AppShell.Instance.Profile.Silver));
		Object.Destroy(base.gameObject);
	}
}
