using UnityEngine;

public class PigeonPassOutAdapter : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public void Triggered()
	{
		AIControllerPigeon component = Utils.GetComponent<AIControllerPigeon>(base.gameObject, Utils.SearchParents);
		if (component != null)
		{
			CspUtils.DebugLog("pigeon check " + component.Idling);
			if (component.Idling)
			{
				CspUtils.DebugLog("ko_pigeon trigger");
				AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "generic_event", "ko_pigeon", 1, -10000, -10000, string.Empty, string.Empty);
			}
			component.StartCoroutine(component.PassOut());
		}
	}
}
