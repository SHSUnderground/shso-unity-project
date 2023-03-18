using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using LitJson;

public class GUIFontManager
{
	public enum SupportedFontEnum
	{
		Undefined,
		Komica,
		Zooom,
		Grobold
	}

	public delegate void FontBankLoadedDelegate(bool success, string error, AssetBundle bundle);

	private Dictionary<SupportedFontEnum, string> supportedFontNames;

	private static AssetBundle fontBankBundle;

	private Dictionary<string, Font> fontDict;

	private GUISkin fontSkin;

	public static AssetBundle FontBankBundle
	{
		get
		{
			return fontBankBundle;
		}
	}

	public Font this[SupportedFontEnum fontType]
	{
		get
		{
			string key = supportedFontNames[fontType];
			return this[key];
		}
	}

	public Font this[string key]
	{
		get
		{
			if (key == null)
			{
				return GUIManager.Instance.fallbackFont;
			}
			if (!fontDict.ContainsKey(key))
			{
				return GUIManager.Instance.fallbackFont;
			}
			return fontDict[key];
		}
	}

	public GUIFontManager()
	{
		InitializeFontBundle();
		supportedFontNames = new Dictionary<SupportedFontEnum, string>();
		supportedFontNames[SupportedFontEnum.Komica] = "Komica";
		supportedFontNames[SupportedFontEnum.Zooom] = "Zooom";
		supportedFontNames[SupportedFontEnum.Grobold] = "Grobold";
	}

	public static void LoadFontBank(string locale, FontBankLoadedDelegate onLoadedDelegate)
	{
		string bundlePath = string.Format("GUI/i18n/{0}/fonts/{0}_font_bundle", "en_us");
		AppShell.Instance.BundleLoader.FetchAssetBundle(bundlePath, OnFontBankLoaded, onLoadedDelegate, false);
	}

	private static void OnFontBankLoaded(AssetBundleLoadResponse response, object extraData)
	{
		FontBankLoadedDelegate fontBankLoadedDelegate = (FontBankLoadedDelegate)extraData;
		if (!string.IsNullOrEmpty(response.Error) || response.Bundle == null)
		{
			fontBankLoadedDelegate(false, response.Error, null);
			return;
		}
		fontBankBundle = response.Bundle;
		fontBankLoadedDelegate(true, response.Error, response.Bundle);
	}

	public void InitializeFontBundle()
	{
		fontDict = new Dictionary<string, Font>();
		if (fontBankBundle == null)
		{
			CspUtils.DebugLog("No Font Bank Loaded. Can't initialized");
			return;
		}

		
		// CSP - LoadAll does work, though.
		// UnityEngine.Object[] materials = fontBankBundle.LoadAll(typeof(GUISkin));
		// foreach (UnityEngine.Object m in materials)
		// {	CspUtils.DebugLog("loaded asset name:" + m.name);
		// 	fontSkin = m as GUISkin;
		// }
	
		//fontSkin = (fontBankBundle.Load("FontSkin") as GUISkin);

		///////////////////////////////////////////////////
		//if (fontSkin == null) { // CSP - this if-block needed as a workaround to Load() problem.
		//	fontBankBundle.LoadAll();  
		//	fontSkin = (fontBankBundle.Load("FontSkin") as GUISkin);
		//}
		////////////////////////////////////////////////////////

		fontSkin = CspUtils.CspLoad(fontBankBundle, "FontSkin") as GUISkin;   // CSP


		if (fontSkin == null)
		{
			CspUtils.DebugLog("Can't retrieve font skin from font bundle. Can't initialize.");
			return;
		}
		GUIStyle[] customStyles = fontSkin.customStyles;
		foreach (GUIStyle gUIStyle in customStyles)
		{
			fontDict[gUIStyle.name] = gUIStyle.font;
		}
	}
}
