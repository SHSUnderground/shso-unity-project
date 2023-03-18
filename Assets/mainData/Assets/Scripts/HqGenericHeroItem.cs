using System.Collections.Generic;
using UnityEngine;

public class HqGenericHeroItem : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public void Start()
	{
		HqItem component = Utils.GetComponent<HqItem>(base.gameObject);
		if (!(component != null) || component.ItemDefinition == null || string.IsNullOrEmpty(component.ItemDefinition.HeroSet))
		{
			return;
		}
		string heroSet = component.ItemDefinition.HeroSet;
		List<Color> replacements = null;
		if (!HqController2.Instance.GetItemThemeFromHeroName(heroSet, out replacements))
		{
			return;
		}
		MeshFilter[] components = Utils.GetComponents<MeshFilter>(base.gameObject, Utils.SearchChildren, true);
		MeshFilter[] array = components;
		foreach (MeshFilter meshFilter in array)
		{
			Color[] colors = meshFilter.mesh.colors;
			if (colors == null || colors.Length <= 0)
			{
				continue;
			}
			for (int j = 0; j < colors.Length; j++)
			{
				for (int k = 0; k < HqController2.Instance.ThemeBaseColors.Count; k++)
				{
					if (CompareRGB(colors[j], HqController2.Instance.ThemeBaseColors[k], 0.0001f))
					{
						if (k < replacements.Count)
						{
							colors[j] = replacements[k];
						}
						break;
					}
				}
			}
			meshFilter.mesh.colors = colors;
		}
	}

	protected bool CompareRGB(Color rh, Color lh, float tolerance)
	{
		return MathfEx.Approx(rh.r, lh.r, tolerance) && MathfEx.Approx(rh.g, lh.g, tolerance) && MathfEx.Approx(rh.b, lh.b, tolerance);
	}
}
