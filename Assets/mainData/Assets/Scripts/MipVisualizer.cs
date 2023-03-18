using UnityEngine;

public class MipVisualizer : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public void Start()
	{
		if (base.renderer == null)
		{
			return;
		}
		Material[] materials = base.renderer.materials;
		if (materials == null || materials.Length <= 0)
		{
			return;
		}
		Color[] array = new Color[9]
		{
			Color.red,
			Color.green,
			Color.blue,
			Color.yellow,
			Color.grey,
			Color.cyan,
			Color.magenta,
			Color.white,
			Color.white
		};
		Material[] array2 = materials;
		foreach (Material material in array2)
		{
			if (material.mainTexture == null)
			{
				continue;
			}
			Texture2D texture2D = new Texture2D(material.mainTexture.width, material.mainTexture.height, TextureFormat.ARGB32, true);
			if (texture2D == null)
			{
				continue;
			}
			int num = Mathf.Min(array.Length, texture2D.mipmapCount);
			for (int j = 0; j < num; j++)
			{
				Color[] pixels = texture2D.GetPixels(j);
				for (int k = 0; k < pixels.Length; k++)
				{
					pixels[k] = array[j];
				}
				texture2D.SetPixels(pixels, j);
			}
			texture2D.Apply(false);
			texture2D.Compress(false);
			material.mainTexture = texture2D;
		}
		base.renderer.materials = materials;
	}
}
