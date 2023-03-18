using System.Collections.Generic;
using UnityEngine;

public class HqStaticData : StaticDataDefinition, IStaticDataDefinition
{
	public List<RoomStaticData> rooms;

	public string defaultTheme;

	public List<ThemeStaticData> room_themes;

	public List<ThemeStaticData> item_themes;

	public List<Color> colors;

	public HqStaticData()
	{
		rooms = new List<RoomStaticData>();
		colors = new List<Color>();
		room_themes = new List<ThemeStaticData>();
		item_themes = new List<ThemeStaticData>();
	}

	public void InitializeFromData(DataWarehouse data)
	{
		foreach (DataWarehouse item2 in data.GetIterator("//Rooms/Room"))
		{
			RoomStaticData roomStaticData = new RoomStaticData();
			if (roomStaticData.InitializeFromData(item2))
			{
				rooms.Add(roomStaticData);
			}
		}
		defaultTheme = data.TryGetString("//themes/base/default", string.Empty);
		int num = 0;
		while (true)
		{
			Color item = data.TryGetColorRGB("//themes/base/color" + num, new Color(-1f, -1f, -1f, -1f));
			if ((double)item.r < 0.0)
			{
				break;
			}
			colors.Add(item);
			num++;
		}
		foreach (DataWarehouse item3 in data.GetIterator("//themes/room_themes/theme"))
		{
			ThemeStaticData themeStaticData = new ThemeStaticData();
			if (themeStaticData.InitializeFromData(item3, colors.Count))
			{
				room_themes.Add(themeStaticData);
			}
		}
		foreach (DataWarehouse item4 in data.GetIterator("//themes/item_themes/theme"))
		{
			ThemeStaticData themeStaticData2 = new ThemeStaticData();
			if (themeStaticData2.InitializeFromData(item4, colors.Count - 1))
			{
				item_themes.Add(themeStaticData2);
			}
		}
	}
}
