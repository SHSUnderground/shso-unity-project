using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[AddComponentMenu("Test/Kevin/AlignmentTest")]
public class AlignmentTest : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public Texture2D sourceTexture;

	public RenderTexture filteredTexture;

	public RenderTexture unfilteredTexture;

	public RenderTexture resultsTexture;

	public Material testMaterial;

	private void OnEnable()
	{
		sourceTexture = new Texture2D(256, 256);
		for (int i = 0; i < 256; i++)
		{
			for (int j = 0; j < 256; j++)
			{
				sourceTexture.SetPixel(j, i, new Color(j & 1, i & 1, 0f, 0f));
			}
		}
		sourceTexture.Apply();
		sourceTexture.wrapMode = TextureWrapMode.Clamp;
		filteredTexture = new RenderTexture(256, 256, 0);
		filteredTexture.isPowerOfTwo = false;
		filteredTexture.filterMode = FilterMode.Bilinear;
		filteredTexture.wrapMode = TextureWrapMode.Clamp;
		unfilteredTexture = new RenderTexture(256, 256, 0);
		unfilteredTexture.isPowerOfTwo = false;
		unfilteredTexture.filterMode = FilterMode.Point;
		unfilteredTexture.wrapMode = TextureWrapMode.Clamp;
		resultsTexture = new RenderTexture(256, 256, 0);
		resultsTexture.isPowerOfTwo = true;
		resultsTexture.wrapMode = TextureWrapMode.Clamp;
		resultsTexture.filterMode = FilterMode.Point;
		testMaterial = new Material(Shader.Find("Test/Blit Alignment"));
	}

	private void OnWillRenderObject()
	{
		Graphics.Blit(sourceTexture, filteredTexture);
		Graphics.Blit(sourceTexture, unfilteredTexture);
		testMaterial.SetTexture("_UnfilteredTex", unfilteredTexture);
		Graphics.Blit(filteredTexture, resultsTexture, testMaterial);
		GetComponent<MeshRenderer>().sharedMaterial.SetTexture("_MainTex", resultsTexture);
	}

	private void OnDisable()
	{
		Object.Destroy(filteredTexture);
		filteredTexture = null;
		Object.Destroy(unfilteredTexture);
		unfilteredTexture = null;
		Object.Destroy(sourceTexture);
		sourceTexture = null;
		Object.Destroy(resultsTexture);
		resultsTexture = null;
		testMaterial.SetTexture("_MainTex", null);
		Object.Destroy(testMaterial);
		testMaterial = null;
	}
}
