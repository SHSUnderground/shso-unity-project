using UnityEngine;

public class PlayerStatusTriggerProxy : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private PlayerStatus _parent;

	protected static readonly float sphereRadius = 5f;

	protected PlayerStatus Parent
	{
		get
		{
			return _parent;
		}
		set
		{
			_parent = value;
			CheckColliders();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == GameController.GetController().LocalPlayer)
		{
			Parent.InProximity = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject == GameController.GetController().LocalPlayer)
		{
			Parent.InProximity = false;
		}
	}

	private void OnEnable()
	{
		if (Parent != null)
		{
			CheckColliders();
		}
	}

	private void CheckColliders()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, sphereRadius);
		foreach (Collider other in array)
		{
			OnTriggerEnter(other);
		}
	}

	public static PlayerStatusTriggerProxy AttachProxyCollider(PlayerStatus parent)
	{
		GameObject gameObject = new GameObject("PlayerStatusTrigger");
		gameObject.transform.parent = parent.transform;
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.layer = 2;
		PlayerStatusTriggerProxy playerStatusTriggerProxy = Utils.AddComponent<PlayerStatusTriggerProxy>(gameObject);
		playerStatusTriggerProxy.Parent = parent;
		SphereCollider sphereCollider = Utils.AddComponent<SphereCollider>(gameObject);
		sphereCollider.isTrigger = true;
		sphereCollider.radius = sphereRadius;
		return playerStatusTriggerProxy;
	}
}
