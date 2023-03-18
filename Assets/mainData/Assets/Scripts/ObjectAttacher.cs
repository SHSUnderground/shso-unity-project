using UnityEngine;

public class ObjectAttacher : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public bool inheritPosition = true;

	public bool inheritRotation;

	public bool inheritScale;

	public bool relativePosition = true;

	public bool findChildInParent = true;

	public string childName;

	protected GameObject parentObject;

	protected Vector3 positionOffset;

	private void Start()
	{
		if (!string.IsNullOrEmpty(childName))
		{
			Transform parent = base.transform;
			if (findChildInParent && base.transform.parent != null)
			{
				parent = base.transform.parent;
			}
			Transform transform = Utils.FindNodeInChildren(parent, childName, true);
			if (transform != null)
			{
				AttachTo(transform.gameObject);
			}
		}
	}

	private void LateUpdate()
	{
		if (parentObject != null)
		{
			if (inheritPosition)
			{
				base.transform.position = parentObject.transform.position + positionOffset;
			}
			if (inheritRotation)
			{
				base.transform.rotation = parentObject.transform.rotation;
			}
			if (inheritScale)
			{
				base.transform.localScale = parentObject.transform.lossyScale;
			}
		}
	}

	public void AttachTo(GameObject newParent)
	{
		if (relativePosition)
		{
			positionOffset = base.transform.position - newParent.transform.position;
		}
		parentObject = newParent;
	}
}
