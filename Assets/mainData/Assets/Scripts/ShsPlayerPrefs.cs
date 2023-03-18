using UnityEngine;

public class ShsPlayerPrefs
{
	public enum Keys
	{
		UserName,
		RememberMe,
		Password,
		DebugWindowSizeX,
		DebugWindowSizeY,
		DebugWindowAlpha,
		AudioEffectsVolume,
		AudioMusicVolume,
		AudioVOXVolume,
		Locale,
		ProTips,
		ShowEmoteBar,
		ProfiledGraphics,
		DOF,
		DisablePrestigeEffects,
		ShadowLevel,
		GraphicsQuality,
		RenderFidelity,
		PetName,
		TitleID,
		MedallionID
	}

	public static bool HasKey(Keys key)
	{
		return PlayerPrefs.HasKey(key.ToString());
	}

	public static void SetString(Keys key, string value)
	{
		PlayerPrefs.SetString(key.ToString(), value);
	}

	public static string GetString(Keys key)
	{
		return PlayerPrefs.GetString(key.ToString());
	}

	public static void SetInt(Keys key, int value)
	{
		PlayerPrefs.SetInt(key.ToString(), value);
	}

	public static int GetInt(Keys key)
	{
		return PlayerPrefs.GetInt(key.ToString());
	}

	public static int GetInt(Keys key, int defaultValue)
	{
		return PlayerPrefs.GetInt(key.ToString(), defaultValue);
	}

	public static void SetFloat(Keys key, float value)
	{
		PlayerPrefs.SetFloat(key.ToString(), value);
	}

	public static float GetFloat(Keys key)
	{
		return PlayerPrefs.GetFloat(key.ToString());
	}

	public static void DeleteKey(Keys key)
	{
		PlayerPrefs.DeleteKey(key.ToString());
	}

	public static void DeleteAll()
	{
		PlayerPrefs.DeleteAll();
	}
}
