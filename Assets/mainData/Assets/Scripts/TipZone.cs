using UnityEngine;

public class TipZone : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string tipStringKey;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == GameController.GetController().LocalPlayer && ShsPlayerPrefs.GetInt(ShsPlayerPrefs.Keys.ProTips, 1) == 1 && Utils.GetComponent<TipZonePlayerTracker>(other.gameObject) == null)
		{
			TipZonePlayerTracker tipZonePlayerTracker = Utils.AddComponent<TipZonePlayerTracker>(other.gameObject);
			tipZonePlayerTracker.tipKey = tipStringKey;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		TipZonePlayerTracker component = Utils.GetComponent<TipZonePlayerTracker>(other.gameObject);
		if (component != null)
		{
			Object.Destroy(component);
		}
	}
}
