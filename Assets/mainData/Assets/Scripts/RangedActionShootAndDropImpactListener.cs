using UnityEngine;

public class RangedActionShootAndDropImpactListener : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public RangedActionShootAndDrop shootAndDropAction;

	private void OnAttacked(CharacterGlobals attackingPlayer)
	{
		if (shootAndDropAction == null)
		{
			CspUtils.DebugLog("No RangedActionShootAndDrop instance linked to " + base.gameObject.name);
		}
		else
		{
			shootAndDropAction.OnObjectAttacked(attackingPlayer);
		}
	}
}
