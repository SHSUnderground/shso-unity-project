using UnityEngine;

public class InteractiveCharacterFixup : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private void Start()
	{
		Utils.SetLayerTree(base.gameObject, 2);
		InteractiveObjectForwarder interactiveObjectForwarder = Utils.AddComponent<InteractiveObjectForwarder>(base.gameObject.transform.parent.gameObject);
		interactiveObjectForwarder.owner = Utils.GetComponent<InteractiveObject>(base.gameObject);
		interactiveObjectForwarder.options = InteractiveObjectForwarder.Options.TiggerRolloverClick;
		Transform transform = Utils.FindNodeInChildren(base.transform, "ClickTrigger");
		if (transform != null)
		{
			transform.gameObject.layer = 14;
		}
		Object.Destroy(this);
	}
}
