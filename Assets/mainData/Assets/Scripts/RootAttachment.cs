using UnityEngine;

public class RootAttachment : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public bool removeThisComponent;

	public bool preserveLocalTransform;

	private void Awake()
	{
		if (!preserveLocalTransform)
		{
			base.gameObject.transform.parent = null;
		}
		else
		{
			Utils.AttachGameObject(null, base.gameObject.transform);
		}
		if (removeThisComponent)
		{
			Object.Destroy(this);
		}
	}
}
