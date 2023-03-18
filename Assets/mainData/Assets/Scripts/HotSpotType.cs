using System;
using UnityEngine;

public class HotSpotType
{
	[Flags]
	public enum Style
	{
		None = 0x0,
		Flying = 0x1,
		Web = 0x2,
		Strength = 0x4,
		Teleport = 0x8,
		Ranged = 0x10,
		Launch = 0x20,
		GroundSpeed = 0x40,
		WallSpeed = 0x80
	}

	protected const string STR_GENERIC_CANNOT_USE_HOTSPOT = "#hotspot_generic_cannot_use";

	protected const string STR_FLY_CANNOT_USE_HOTSPOT = "#hotspot_fly_cannot_use";

	protected const string STR_CLIMB_CANNOT_USE_HOTSPOT = "#hotspot_climb_cannot_use";

	public static Style GetEnumFromString(string name)
	{
		//Discarded unreachable code: IL_001c, IL_0039
		try
		{
			return (Style)(int)Enum.Parse(typeof(Style), name, true);
		}
		catch
		{
			CspUtils.DebugLogWarning("Invalid hot spot type: " + name);
			return Style.None;
		}
	}

	public static int GetStyleIndex(Style style)
	{
		if (style == Style.None)
		{
			return 0;
		}
		return (int)Mathf.Log((float)style, 2f);
	}

	public static string GetCannotUseString(Style type)
	{
		string text = "#hotspot_generic_cannot_use";
		switch (type)
		{
		case Style.Flying:
			return "#hotspot_fly_cannot_use";
		case Style.Web:
			return "#hotspot_climb_cannot_use";
		default:
			return "#hotspot_generic_cannot_use";
		}
	}
}
