using UnityEngine;

public class CameraRenderTest : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected RenderTexture testTex;

	public void Start()
	{
	}

	public void OnDisable()
	{
	}

	public void OnPreCull()
	{
		ReleaseTexture();
		CreateTexture();
	}

	protected void CreateTexture()
	{
		if (testTex == null)
		{
			testTex = RenderTexture.GetTemporary(512, 512);
		}
	}

	protected void ReleaseTexture()
	{
		if (testTex == null)
		{
			RenderTexture.ReleaseTemporary(testTex);
			testTex = null;
		}
	}
}
