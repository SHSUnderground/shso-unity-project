using UnityEngine;

public class TokenAwarder : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public int coinsToAward;

	private void Start()
	{
		NotificationHUD.addNotification(new TotalSilverNotificationData(AppShell.Instance.Profile.Silver));
		AppShell.Instance.EventReporter.ReportAwardTokens(coinsToAward);
		Object.Destroy(base.gameObject);
	}
}
