using System.Collections.Generic;
using System.IO;
using UnityEngine;

[AddComponentMenu("Lab/Misc/Hi-Res Screen Shot")]
public class HiResScreenshot : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string FileNamePrefix;

	public int FullWidth;

	public int FullHeight;

	public int TileWidth;

	public int TileHeight;

	public Texture2D[] results;

	public bool SaveResults;

	private void Update()
	{
		List<Texture2D> list = new List<Texture2D>();
		for (int i = 0; i < FullHeight; i += TileHeight)
		{
			int num = Mathf.Min(i + TileHeight, FullHeight);
			for (int j = 0; j < FullWidth; j += TileWidth)
			{
				int num2 = Mathf.Min(j + TileWidth, FullWidth);
				float num3 = ((float)(num2 - j) * 0.5f + (float)j) / (float)FullWidth * 2f - 1f;
				float num4 = ((float)(num - i) * 0.5f + (float)i) / (float)FullHeight * 2f - 1f;
				Matrix4x4 identity = Matrix4x4.identity;
				identity[0, 0] = (float)FullWidth / (float)(num2 - j);
				identity[1, 1] = (float)FullHeight / (float)(num - i);
				identity[0, 3] = (0f - num3) * identity[0, 0];
				identity[1, 3] = (0f - num4) * identity[1, 1];
				Camera.main.ResetProjectionMatrix();
				Camera.main.projectionMatrix = identity * Camera.main.projectionMatrix;
				Texture2D texture2D = new Texture2D(num2 - j, num - i, TextureFormat.ARGB32, false);
				RenderTexture renderTexture = new RenderTexture(num2 - j, num - i, 24);
				Camera.main.targetTexture = renderTexture;
				Camera.main.Render();
				RenderTexture.active = renderTexture;
				texture2D.ReadPixels(new Rect(0f, 0f, renderTexture.width, renderTexture.height), 0, 0, false);
				texture2D.Apply();
				RenderTexture.active = null;
				byte[] buffer = texture2D.EncodeToPNG();
				string path = FileNamePrefix + (j / TileWidth).ToString("D3") + "x" + (i / TileHeight).ToString("D3") + ".png";
				using (BinaryWriter binaryWriter = new BinaryWriter(File.Open(path, FileMode.Create)))
				{
					binaryWriter.Write(buffer);
				}
				if (SaveResults)
				{
					list.Add(texture2D);
				}
				else
				{
					Object.Destroy(texture2D);
				}
				Object.Destroy(renderTexture);
				Camera.main.ResetProjectionMatrix();
				Camera.main.targetTexture = null;
			}
		}
		results = list.ToArray();
		base.enabled = false;
	}

	private void OnDisable()
	{
		Camera.main.ResetProjectionMatrix();
	}
}
