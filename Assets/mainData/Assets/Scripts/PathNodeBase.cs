using UnityEngine;

public class PathNodeBase : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum NodeType
	{
		Normal,
		Door,
		NPC,
		Rally
	}

	public static bool drawPathNodes;

	public bool autoSnap = true;

	public PathNode[] links;

	public static float StepHeight = 0.3f;

	public static float MaxDistanceSqrd = 100f;

	public NodeType Type;

	public virtual void Awake()
	{
	}

	public virtual void Start()
	{
	}

	public GameObject ClearLineTo(PathNode node)
	{
		Vector3 start = base.transform.position + new Vector3(0f, StepHeight, 0f);
		Vector3 end = node.transform.position + new Vector3(0f, StepHeight, 0f);
		RaycastHit hitInfo;
		if (Physics.Linecast(start, end, out hitInfo, 4694016) && hitInfo.collider.gameObject != node.gameObject)
		{
			return hitInfo.collider.gameObject;
		}
		return null;
	}

	public virtual void OnDrawGizmos()
	{
		if (!drawPathNodes)
		{
			return;
		}
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere(base.transform.position, 0.4f);
		if (links != null)
		{
			PathNode[] array = links;
			foreach (PathNode pathNode in array)
			{
				Gizmos.DrawLine(base.transform.position, pathNode.transform.position);
			}
		}
	}

	public virtual void OnDrawGizmosSelected()
	{
		if (autoSnap)
		{
			SnapToGround(1f);
		}
	}

	public virtual bool SnapToGround(float tolerance)
	{
		if (!autoSnap)
		{
			return true;
		}
		RaycastHit hitInfo;
		if (Physics.Linecast(base.transform.position + new Vector3(0f, 0.5f, 0f), base.transform.position + new Vector3(0f, 0f - tolerance, 0f), out hitInfo))
		{
			base.transform.position = hitInfo.point;
			return true;
		}
		return false;
	}
}
