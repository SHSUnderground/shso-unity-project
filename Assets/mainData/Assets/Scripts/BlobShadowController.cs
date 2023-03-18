using UnityEngine;

[RequireComponent(typeof(Projector))]
public class BlobShadowController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string bone = "Pelvis";

	public Vector3 offset = Vector3.zero;

	protected Projector blob;

	protected Transform parentNode;

	private void Start()
	{
		blob = null;
		parentNode = null;
		blob = Utils.GetComponent<Projector>(base.gameObject);
		if (blob == null)
		{
			CspUtils.DebugLog("BlobShadowController did not find its projector");
		}
		else if (bone != string.Empty)
		{
			parentNode = Utils.FindNodeInChildren(base.transform.parent, bone);
			if (parentNode == null)
			{
				CspUtils.DebugLog("BlobShadowController did not find specified bone <" + bone + "> on game object <" + base.transform.parent.gameObject.name + ">");
			}
		}
	}

	private void LateUpdate()
	{
		if (!(parentNode == null))
		{
			base.transform.position = parentNode.transform.position + offset;
		}
	}
}
