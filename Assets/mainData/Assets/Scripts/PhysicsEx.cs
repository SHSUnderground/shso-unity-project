using UnityEngine;

public class PhysicsEx
{
	public static bool ShapeCastXXX(Rigidbody rb, Vector3 vel, float dt, out RaycastHit hitInfo, int layerMask)
	{
		Vector3 position = rb.position;
		Vector3 end = rb.position + vel * dt;
		return Physics.Linecast(position, end, out hitInfo, layerMask);
	}

	public static bool ShapeCast(Rigidbody rb, Vector3 vel, float dt, out RaycastHit hitInfo, int layerMask)
	{
		Vector3 normalized = vel.normalized;
		float magnitude = vel.magnitude;
		float num = magnitude * dt + 0.05f;
		if (rb.collider is MeshCollider)
		{
			Vector3[] array = MeshColliderToPoints(rb.collider as MeshCollider);
			foreach (Vector3 vector in array)
			{
				Vector3 end = vector + normalized * num;
				Debug.DrawLine(vector, end, Color.red);
				if (Physics.Linecast(vector, end, out hitInfo, layerMask))
				{
					return true;
				}
			}
		}
		else
		{
			RaycastHit[] array2 = rb.rigidbody.SweepTestAll(normalized, num);
			for (int j = 0; j < array2.Length; j++)
			{
				if (!array2[j].collider.isTrigger)
				{
					int num2 = 1 << array2[j].collider.gameObject.layer;
					if ((num2 & 0x478000) != 0)
					{
						hitInfo = array2[j];
						return true;
					}
				}
			}
		}
		hitInfo = default(RaycastHit);
		return false;
	}

	public static string ToString(Vector3 pt)
	{
		return "<" + pt.x + "," + pt.y + "," + pt.z + ">";
	}

	public static Vector3[] BoundsToPoints(Bounds bounds)
	{
		Vector3[] array = new Vector3[8];
		//ref Vector3 reference = ref array[0];
		Vector3 min = bounds.min;
		float x = min.x;
		Vector3 min2 = bounds.min;
		float y = min2.y;
		Vector3 min3 = bounds.min;
		array[0] = new Vector3(x, y, min3.z);
		//ref Vector3 reference2 = ref array[1];
		Vector3 min4 = bounds.min;
		float x2 = min4.x;
		Vector3 min5 = bounds.min;
		float y2 = min5.y;
		Vector3 max = bounds.max;
		array[1] = new Vector3(x2, y2, max.z);
		//ref Vector3 reference3 = ref array[2];
		Vector3 min6 = bounds.min;
		float x3 = min6.x;
		Vector3 max2 = bounds.max;
		float y3 = max2.y;
		Vector3 min7 = bounds.min;
		array[2] = new Vector3(x3, y3, min7.z);
		//ref Vector3 reference4 = ref array[3];
		Vector3 min8 = bounds.min;
		float x4 = min8.x;
		Vector3 max3 = bounds.max;
		float y4 = max3.y;
		Vector3 max4 = bounds.max;
		array[3] = new Vector3(x4, y4, max4.z);
		//ref Vector3 reference5 = ref array[4];
		Vector3 max5 = bounds.max;
		float x5 = max5.x;
		Vector3 min9 = bounds.min;
		float y5 = min9.y;
		Vector3 min10 = bounds.min;
		array[4] = new Vector3(x5, y5, min10.z);
		//ref Vector3 reference6 = ref array[5];
		Vector3 max6 = bounds.max;
		float x6 = max6.x;
		Vector3 min11 = bounds.min;
		float y6 = min11.y;
		Vector3 max7 = bounds.max;
		array[5] = new Vector3(x6, y6, max7.z);
		//ref Vector3 reference7 = ref array[6];
		Vector3 max8 = bounds.max;
		float x7 = max8.x;
		Vector3 max9 = bounds.max;
		float y7 = max9.y;
		Vector3 min12 = bounds.min;
		array[6] = new Vector3(x7, y7, min12.z);
		//ref Vector3 reference8 = ref array[7];
		Vector3 max10 = bounds.max;
		float x8 = max10.x;
		Vector3 max11 = bounds.max;
		float y8 = max11.y;
		Vector3 max12 = bounds.max;
		array[7] = new Vector3(x8, y8, max12.z);
		return array;
	}

	public static Vector3[] MeshColliderToPoints(MeshCollider mc)
	{
		Vector3[] vertices = mc.sharedMesh.vertices;
		for (int i = 0; i < vertices.Length; i++)
		{
			vertices[i] = mc.gameObject.transform.TransformPoint(vertices[i]);
		}
		return vertices;
	}
}
