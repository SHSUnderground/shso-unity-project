using System;
using UnityEngine;

public class PhysicsInit : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public const float maxMass = 10f;

	protected PhysicMaterialEx.MaterialType physicsMaterial;

	public float volume = -1f;

	public bool IsImmobile
	{
		get
		{
			if (physicsMaterial != 0)
			{
				PhysicMaterialEx physicMaterialEx = PhysicMatMapping.Instance[physicsMaterial];
				if (physicMaterialEx != null)
				{
					return physicMaterialEx.isImmobile;
				}
			}
			return false;
		}
	}

	public PhysicMaterialEx.MaterialType PhysicsMaterial
	{
		get
		{
			return physicsMaterial;
		}
		set
		{
			physicsMaterial = value;
			if (physicsMaterial != 0)
			{
				PhysicMaterialEx physics = PhysicMatMapping.Instance[physicsMaterial];
				Utils.ForEachTree(base.gameObject, delegate(GameObject go)
				{
					if (go.collider != null)
					{
						go.collider.material = physics.baseMaterial;
					}
				});
				if (volume > 0f)
				{
					base.rigidbody.mass = volume * physics.density * 0.001f;
				}
			}
		}
	}

	public void Start()
	{
		if (physicsMaterial != 0)
		{
			PhysicMaterialEx physics = PhysicMatMapping.Instance[physicsMaterial];
			Utils.ForEachTree(base.gameObject, delegate(GameObject go)
			{
				if (go.collider != null)
				{
					go.collider.material = physics.baseMaterial;
				}
			});
			if (volume < 0f)
			{
				volume = CalculateVolume(base.gameObject, Vector3.one);
			}
			base.rigidbody.mass = volume * physics.density * 0.001f;
		}
	}

	public static float CalculateVolume(GameObject me, Vector3 scale)
	{
		scale = Vector3.Scale(scale, me.transform.localScale);
		float num = 0f;
		if (me.collider != null)
		{
			HqTrigger component = Utils.GetComponent<HqTrigger>(me, Utils.SearchChildren);
			if (component == null)
			{
				num = CalculateVolumeForCollider(me.collider, scale);
			}
		}
		foreach (Transform item in me.transform)
		{
			num += CalculateVolume(item.gameObject, scale);
		}
		return num;
	}

	protected static float CalculateVolumeForCollider(Collider c, Vector3 scale)
	{
		SphereCollider sphereCollider = c as SphereCollider;
		if (sphereCollider != null)
		{
			float num = sphereCollider.radius * scale.x;
			return 4.18878937f * num * num * num;
		}
		BoxCollider boxCollider = c as BoxCollider;
		if (boxCollider != null)
		{
			Vector3 vector = Vector3.Scale(boxCollider.size, scale);
			return vector.x * vector.y * vector.z;
		}
		MeshCollider meshCollider = c as MeshCollider;
		if (meshCollider != null && meshCollider.convex)
		{
			return computeMeshVolume(meshCollider.sharedMesh, scale);
		}
		CspUtils.DebugLog("Unsupported collider type on <" + c.gameObject.name + ">");
		return 0f;
	}

	protected static float tetrahedronVolume(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		Vector3 lhs = p1 - p0;
		Vector3 lhs2 = p2 - p0;
		Vector3 rhs = p3 - p0;
		Vector3 rhs2 = Vector3.Cross(lhs2, rhs);
		float num = Vector3.Dot(lhs, rhs2);
		if (num < 0f)
		{
			return 0f - num;
		}
		return num;
	}

	protected static float computeMeshVolume(Mesh mesh, Vector3 scale)
	{
		float num = 0f;
		Vector3[] vertices = mesh.vertices;
		for (int i = 0; i < mesh.subMeshCount; i++)
		{
			int[] triangles = mesh.GetTriangles(i);
			Vector3 p = Vector3.Scale(vertices[triangles[0]], scale);
			for (int j = 0; j < triangles.Length; j += 3)
			{
				Vector3 p2 = Vector3.Scale(vertices[triangles[j]], scale);
				Vector3 p3 = Vector3.Scale(vertices[triangles[j + 1]], scale);
				Vector3 p4 = Vector3.Scale(vertices[triangles[j + 2]], scale);
				num += tetrahedronVolume(p, p2, p3, p4);
			}
		}
		return num * (355f / (678f * (float)Math.PI));
	}
}
