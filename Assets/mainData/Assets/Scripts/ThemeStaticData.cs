using System.Collections.Generic;
using UnityEngine;

public class ThemeStaticData
{
	public string name;

	public List<Color> colors;

	public string hero;

	public ThemeStaticData()
	{
		name = null;
		colors = null;
	}

	public bool InitializeFromData(DataWarehouse data, int numColors)
	{
		name = data.TryGetString("name", string.Empty);
		if (name == string.Empty)
		{
			return false;
		}
		hero = data.TryGetString("hero", null);
		colors = new List<Color>(numColors);
		for (int i = 0; i < numColors; i++)
		{
			Color item = data.TryGetColorRGB("color" + i, new Color(-1f, -1f, -1f));
			if (item.r < 0f)
			{
				return false;
			}
			colors.Add(item);
		}
		return true;
	}
}
