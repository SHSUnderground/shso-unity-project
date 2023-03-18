using UnityEngine;

public class TargetDummyXPAwarder : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public int xpToAwardPerHit = 5;

	private void OnAttacked(CharacterGlobals attackingPlayer)
	{
		if (SocialSpaceController.Instance != null && SocialSpaceController.Instance.Controller != null)
		{
			SocialSpaceController.Instance.Controller.GrantXP(attackingPlayer, xpToAwardPerHit);
		}
	}
}
