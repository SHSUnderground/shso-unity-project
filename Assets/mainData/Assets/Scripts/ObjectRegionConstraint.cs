using UnityEngine;

public class ObjectRegionConstraint : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum ConstraintType
	{
		Inclusion,
		Exclusion
	}

	public ConstraintType constraintType;

	public bool replicationOnly;

	protected BoxCollider boxCollider;

	protected Vector3 boxMinimums;

	protected Vector3 boxMaximums;

	protected SphereCollider sphereCollider;

	private void Start()
	{
		boxCollider = (GetComponent(typeof(BoxCollider)) as BoxCollider);
		if (boxCollider != null)
		{
			//ref Vector3 reference = ref boxMinimums;
			Vector3 center = boxCollider.center;
			float x = center.x;
			Vector3 size = boxCollider.size;
			boxMinimums.x = x - size.x * 0.5f;
			//ref Vector3 reference2 = ref boxMinimums;
			Vector3 center2 = boxCollider.center;
			float y = center2.y;
			Vector3 size2 = boxCollider.size;
			boxMinimums.y = y - size2.y * 0.5f;
			//ref Vector3 reference3 = ref boxMinimums;
			Vector3 center3 = boxCollider.center;
			float z = center3.z;
			Vector3 size3 = boxCollider.size;
			boxMinimums.z = z - size3.z * 0.5f;
			//ref Vector3 reference4 = ref boxMaximums;
			Vector3 center4 = boxCollider.center;
			float x2 = center4.x;
			Vector3 size4 = boxCollider.size;
			boxMaximums.x = x2 + size4.x * 0.5f;
			//ref Vector3 reference5 = ref boxMaximums;
			Vector3 center5 = boxCollider.center;
			float y2 = center5.y;
			Vector3 size5 = boxCollider.size;
			boxMaximums.y = y2 + size5.y * 0.5f;
			//ref Vector3 reference6 = ref boxMaximums;
			Vector3 center6 = boxCollider.center;
			float z2 = center6.z;
			Vector3 size6 = boxCollider.size;
			boxMaximums.z = z2 + size6.z * 0.5f;
		}
		sphereCollider = (GetComponent(typeof(SphereCollider)) as SphereCollider);
		if (boxCollider == null && sphereCollider == null)
		{
			CspUtils.DebugLog("ObjectSpawnRegionConstrant (" + base.gameObject.name + ") has no box or sphere collider and will not function.");
		}
	}

	private void Update()
	{
	}

	public bool checkPoint(Vector3 point)
	{
		if (boxCollider != null)
		{
			Vector3 vector = base.transform.InverseTransformPoint(point);
			if (vector.x < boxMinimums.x || vector.x > boxMaximums.x || vector.y < boxMinimums.y || vector.y > boxMaximums.y || vector.z < boxMinimums.z || vector.z > boxMaximums.z)
			{
				return false;
			}
			return true;
		}
		if (sphereCollider != null)
		{
			return Vector3.Distance(point, sphereCollider.transform.position) < sphereCollider.radius;
		}
		return true;
	}
}
