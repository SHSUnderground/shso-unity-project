using UnityEngine;

public class DisableTriggerAdaptor : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public bool disableChildren;

	public void Triggered()
	{
		if (disableChildren)
		{
			Utils.ActivateTree(base.gameObject, false);
		}
		else
		{
			base.gameObject.active = false;
		}
	}
}
