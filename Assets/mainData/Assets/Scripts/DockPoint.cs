using UnityEngine;

public class DockPoint : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum Posture
	{
		stand,
		sit
	}

	protected Posture postureType;

	public Vector3 FacingDirection
	{
		get
		{
			return base.transform.forward;
		}
	}

	public string Name
	{
		get
		{
			return base.name;
		}
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(base.transform.position, new Vector3(0.25f, 0.25f, 0.25f));
		Gizmos.color = Color.red;
		Gizmos.DrawRay(base.transform.position, FacingDirection);
	}
}
