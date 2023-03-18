using UnityEngine;
using System;

public class QuadTexture : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string textureSource;

	public bool scaleToFit;

	public bool loadedTexture;

	public float initialAlpha = 1f;

	private void Awake()
	{
		MeshFilter meshFilter = base.gameObject.GetComponent<MeshFilter>();
		if (meshFilter == null)
		{
			meshFilter = base.gameObject.AddComponent<MeshFilter>();
		}
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
		Vector3[] array = new Vector3[4]
		{
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, -1f),
			new Vector3(0f, 0f, -1f)
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
		mesh.RecalculateNormals();
		meshFilter.mesh = mesh;
		MeshRenderer meshRenderer = base.gameObject.GetComponent<MeshRenderer>();
		if (meshRenderer == null)
		{
			meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
		}

		// try-catch added by CSP
		try {
			//Material material = new Material(Shader.Find("Marvel/Base/Self-Illuminated-Full Bright"));
			Material material = new Material(Shader.Find("Transparent/Specular"));  // CSP
			material.color = new Color(0f, 0f, 0f, initialAlpha);
			meshRenderer.material = material;
		}
		catch (Exception e) {

		}
		if (!string.IsNullOrEmpty(textureSource))
		{
			GUIManager.Instance.LoadTexture(textureSource, OnTextureLoaded, null);
			loadedTexture = true;
		}
	}

	private void OnTextureLoaded(UnityEngine.Object obj, AssetBundle bundle, object extraData)
	{
		Texture2D texture = obj as Texture2D;
		SetTexture(texture);
	}

	private void Update()
	{
		if (!loadedTexture && !string.IsNullOrEmpty(textureSource))
		{
			GUIManager.Instance.LoadTexture(textureSource, OnTextureLoaded, null);
			loadedTexture = true;
		}
	}

	public void SetTexture(Texture2D tex)
	{
		MeshRenderer component = base.gameObject.GetComponent<MeshRenderer>();
		Material material = component.material;
		if (tex == null)
		{
			material.color = new Color(1f, 1f, 1f, 0f);
		}
		else
		{
			material.color = new Color(1f, 1f, 1f, initialAlpha);
			material.SetTexture("_MainTex", tex);
			if (scaleToFit)
			{
				float num = tex.width;
				float num2 = tex.height;
				base.transform.localScale = new Vector3(num / 100f, num2 / 100f, 1f);
			}
		}
		component.material = material;
	}
}
