using UnityEngine;

[AddComponentMenu("GUI/Quad")]
public class GUIQuad : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private void Start()
	{
		MeshFilter component = base.gameObject.GetComponent<MeshFilter>();
		Vector3[] vertices = new Vector3[4]
		{
			new Vector3(-0.5f, -0.5f, 0f),
			new Vector3(0.5f, -0.5f, 0f),
			new Vector3(0.5f, 0.5f, 0f),
			new Vector3(-0.5f, 0.5f, 0f)
		};
		Vector2[] uv = new Vector2[4]
		{
			new Vector2(0f, 0f),
			new Vector2(1f, 0f),
			new Vector2(1f, 1f),
			new Vector2(0f, 1f)
		};
		int[] triangles = new int[12]
		{
			0,
			3,
			2,
			2,
			1,
			0,
			0,
			1,
			2,
			2,
			3,
			0
		};
		Mesh mesh = new Mesh();
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
		component.mesh = mesh;
	}

	private void Update()
	{
		Camera main = Camera.main;
		if (!(main == null))
		{
			Vector3 lhs = main.transform.position - base.transform.position;
			Vector3 b = Vector3.Dot(lhs, main.transform.forward) * main.transform.forward;
			base.transform.LookAt(base.transform.position - b);
		}
	}
}
