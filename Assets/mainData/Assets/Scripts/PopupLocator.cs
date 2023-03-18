using UnityEngine;

public class PopupLocator : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private void OnEnable()
	{
		CombatController.addPopupLocator(this);
	}

	private void OnDisable()
	{
		CombatController.removePopupLocator(this);
	}
}
