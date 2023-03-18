using UnityEngine;

public class Snap : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public static bool DrawSnaps = true;

	private Snap snappedTo;

	public Snap SnappedTo
	{
		get
		{
			return snappedTo;
		}
		set
		{
			snappedTo = value;
		}
	}

	private void OnDrawGizmos()
	{
		if (DrawSnaps)
		{
			if (SnappedTo == null)
			{
				Gizmos.DrawIcon(base.transform.position, "snap_open.png");
			}
			else
			{
				Gizmos.DrawIcon(base.transform.position, "snap_closed.png");
			}
		}
	}
}
