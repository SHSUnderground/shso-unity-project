using UnityEngine;

public class ShaderUtil
{
	public static bool NeedsVertexColor(Shader s)
	{
		switch (s.name)
		{
		case "Chad/Vertex Color":
		case "Marvel/Base/Self-Illuminated":
		case "Marvel/AO/Vertex Color":
		case "Marvel/Lightmap/Vertex Color":
			return true;
		case "Diffuse":
		case "Marvel/Base/Self-Illuminated-Tex":
		case "Marvel/AO/Diffuse":
		case "Marvel/Lightmap/Diffuse":
		case "Marvel/Lightmap/Self-Illuminated-Tex":
			return false;
		default:
			return false;
		}
	}

	public static bool HasDiffuseTexture(Shader s)
	{
		switch (s.name)
		{
		case "Diffuse":
		case "Marvel/Base/Self-Illuminated-Tex":
		case "Marvel/AO/Diffuse":
		case "Marvel/Lightmap/Diffuse":
		case "Marvel/Lightmap/Self-Illuminated-Tex":
			return true;
		default:
			return false;
		}
	}

	public static int RankAsSharedMaterial(Material mat)
	{
		switch (mat.shader.name)
		{
		case "Chad/Vertex Color":
			return 10;
		case "Marvel/Lightmap/Vertex Color":
		case "Marvel/AO/Vertex Color":
			return 5;
		case "Marvel/Lightmap/Self-Illuminated-Tex":
			return 4;
		default:
			return 0;
		}
	}

	public static bool CanCollapseInto(Material a, Material b)
	{
		if (a == null || b == null)
		{
			return false;
		}
		if (a.shader.name != b.shader.name)
		{
			return false;
		}
		switch (a.shader.name)
		{
		case "Chad/Vertex Color":
		case "Marvel/Base/Self-Illuminated":
			return true;
		case "Diffuse":
		case "Transparent/Diffuse":
		case "Marvel/Base/Diffuse 2-Sided":
		case "Marvel/Base/Self-Illuminated-Tex":
		{
			Color color = a.GetColor("_Color");
			Color color2 = a.GetColor("_Color");
			Texture texture7 = a.GetTexture("_MainTex");
			Texture texture8 = b.GetTexture("_MainTex");
			return Utils.ColorEqual(color, color2) && texture7 == texture8;
		}
		case "Marvel/Lightmap/Vertex Color":
		{
			Texture texture5 = a.GetTexture("_LightMap");
			Texture texture6 = b.GetTexture("_LightMap");
			return texture5 == texture6;
		}
		case "Marvel/Lightmap/Diffuse":
		case "Marvel/Lightmap/Self-Illuminated-Tex":
		{
			Texture texture11 = a.GetTexture("_MainTex");
			Texture texture12 = b.GetTexture("_MainTex");
			Texture texture13 = a.GetTexture("_LightMap");
			Texture texture14 = b.GetTexture("_LightMap");
			return texture11 == texture12 && texture13 == texture14;
		}
		case "Marvel/AO/Vertex Color":
		{
			Texture texture9 = a.GetTexture("_AOMap");
			Texture texture10 = b.GetTexture("_AOMap");
			return texture9 == texture10;
		}
		case "Marvel/AO/Diffuse":
		{
			Texture texture = a.GetTexture("_MainTex");
			Texture texture2 = b.GetTexture("_MainTex");
			Texture texture3 = a.GetTexture("_AOMap");
			Texture texture4 = b.GetTexture("_AOMap");
			return texture == texture2 && texture3 == texture4;
		}
		default:
			return true;
		}
	}

	public static void Blit(Texture source, RenderTexture dest)
	{
		Shader.SetGlobalVector("_MainTex_Size", new Vector4(source.width, source.height, 0f, 0f));
		Graphics.Blit(source, dest);
	}

	public static void Blit(Texture source, RenderTexture dest, Material material)
	{
		Shader.SetGlobalVector("_MainTex_Size", new Vector4(source.width, source.height, 0f, 0f));
		Graphics.Blit(source, dest, material);
	}

	public static void Blit(Texture source, RenderTexture dest, Material material, int pass)
	{
		Shader.SetGlobalVector("_MainTex_Size", new Vector4(source.width, source.height, 0f, 0f));
		Graphics.Blit(source, dest, material, pass);
	}
}
