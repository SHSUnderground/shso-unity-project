using UnityEngine;

public class CharacterMaterialController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public virtual bool Connect()
	{
		return DisconnectOtherControllers();
	}

	public virtual bool Disconnect()
	{
		return true;
	}

	protected bool DisconnectOtherControllers()
	{
		CharacterMaterialController[] components = Utils.GetComponents<CharacterMaterialController>(GetCharacterRoot(), Utils.SearchChildren);
		foreach (CharacterMaterialController characterMaterialController in components)
		{
			if (!(characterMaterialController == this) && !characterMaterialController.Disconnect())
			{
				return false;
			}
		}
		return true;
	}

	protected GameObject GetCharacterRoot()
	{
		CharacterGlobals component = Utils.GetComponent<CharacterGlobals>(base.gameObject, Utils.SearchParents);
		if (component != null)
		{
			return component.gameObject;
		}
		return base.transform.root.gameObject;
	}
}
