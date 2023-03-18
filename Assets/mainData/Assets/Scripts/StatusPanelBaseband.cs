using UnityEngine;

public class StatusPanelBaseband : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject testObject;

	public int[] triangles;

	public void Start()
	{
		CspUtils.DebugLog("StatusPanelBaseband.Start()");
		Transform transform = base.gameObject.transform;
		MeshFilter meshFilter = transform.GetComponent(typeof(MeshFilter)) as MeshFilter;
		Mesh mesh = meshFilter.mesh;
		mesh.triangles = new int[6]
		{
			1,
			0,
			3,
			3,
			0,
			2
		};
		triangles = mesh.triangles;
	}
}
