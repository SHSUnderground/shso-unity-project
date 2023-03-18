using System.Collections.Generic;
using UnityEngine;

public class CardManager
{
	private static CardManager _instance = new CardManager();

	private Dictionary<string, AssetBundle> cardTextureBundles;

	private int bundleRequests = -1;

	private DataWarehouse cardSetData;

	private string deckList;

	private Dictionary<string, BattleCard> library;

	public static bool TextureBundleLoaded
	{
		get
		{
			return _instance.bundleRequests == 0;
		}
	}

	public static bool CardDataLoaded
	{
		get
		{
			return _instance.cardSetData != null;
		}
	}

	public static string DeckList
	{
		get
		{
			return _instance.deckList;
		}
	}

	public static Dictionary<string, BattleCard> Library
	{
		get
		{
			return _instance.library;
		}
	}

	private CardManager()
	{
		library = new Dictionary<string, BattleCard>();
	}

	public static Dictionary<string, int> ParseRecipe(string recipe)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		string[] array = recipe.Split(';');
		string[] array2 = array;
		foreach (string text in array2)
		{
			string[] array3 = text.Split(':');
			if (array3.Length >= 2)
			{
				dictionary[array3[0]] = int.Parse(array3[1]);
			}
		}
		return dictionary;
	}

	public static void LoadCardData()
	{
		_instance.cardSetData = null;
		AppShell.Instance.DataManager.LoadGameData("Cards/ST", OnCardDataLoaded);
	}

	private static void OnCardDataLoaded(GameDataLoadResponse loadResponse, object extraData)
	{
		_instance.cardSetData = loadResponse.Data;
	}

	public static void LoadTextureBundle(bool unloadOnSceneChange)
	{
		if (_instance.cardTextureBundles != null)
		{
			return;
		}
		unloadOnSceneChange = false;
		_instance.cardTextureBundles = new Dictionary<string, AssetBundle>();
		int num = 1;
		_instance.bundleRequests = 0;
		while (true)
		{
			string text = "CardGame/Cards/" + AppShell.Instance.Locale + "/" + AppShell.Instance.Locale + "_cards" + ((num >= 10) ? string.Empty : "0") + num.ToString();
			CspUtils.DebugLog("checking for card bundle " + text.ToLower());
			if (!ShsCacheManager.Manifest.ContainsKey(text.ToLower() + ".unity3d"))
			{
				break;
			}
			AppShell.Instance.BundleLoader.FetchAssetBundle(text, OnTextureBundleLoaded, num, unloadOnSceneChange);
			_instance.bundleRequests++;
			num++;
		}
	}

	private static void OnTextureBundleLoaded(AssetBundleLoadResponse response, object extraData)
	{
		_instance.bundleRequests--;
		if (!string.IsNullOrEmpty(response.Error))
		{
			CspUtils.DebugLog(string.Format("Unable to load required asset bundle for <{0}> due to error <{1}>.", response.Path, response.Error));
		}
		int num = (int)extraData;
		_instance.cardTextureBundles["cards" + ((num >= 10) ? string.Empty : "0") + num.ToString()] = response.Bundle;
	}

	public static Dictionary<string, BattleCard> ParseCardDataSet(ICollection<string> desiredSet)
	{
		return _instance._ParseCardDataSet(desiredSet);
	}

	private Dictionary<string, BattleCard> _ParseCardDataSet(ICollection<string> desiredSet)
	{
		Dictionary<string, BattleCard> dictionary = new Dictionary<string, BattleCard>();
		int count = cardSetData.GetCount("Cards.Card");
		for (int i = 0; i < count; i++)
		{
			DataWarehouse data = cardSetData.GetData("Cards.Card", i);
			string @string = data.GetString("ID");
			if (desiredSet.Contains(@string))
			{
				string xml = cardSetData.GetXml("Cards.Card", i);
				string cardXML = "<Cards><Card>" + xml + "</Card></Cards>";
				BattleCard battleCard = new BattleCard();
				if (battleCard.ParseTypeXml(cardXML))
				{
					dictionary.Add(@string, battleCard);
				}
			}
		}
		return dictionary;
	}

	public static BattleCard ParseSingleCardData(string cardType)
	{
		return _instance._ParseSingleCardData(cardType);
	}

	private BattleCard _ParseSingleCardData(string cardType)
	{
		int count = cardSetData.GetCount("Cards.Card");
		for (int i = 0; i < count; i++)
		{
			DataWarehouse data = cardSetData.GetData("Cards.Card", i);
			if (cardType == data.GetString("ID"))
			{
				string xml = cardSetData.GetXml("Cards.Card", i);
				string cardXML = "<Cards><Card>" + xml + "</Card></Cards>";
				BattleCard battleCard = new BattleCard();
				if (battleCard.ParseTypeXml(cardXML))
				{
					return battleCard;
				}
			}
		}
		return null;
	}

	public static string GetCardBundle(string cardType)
	{
		BattleCard battleCard = _instance._ParseSingleCardData(cardType.Replace("_mini", string.Empty));
		if (battleCard == null)
		{
			//CspUtils.DebugLog("Unknown card " + cardType + " in GetCardBundle");
			return string.Empty;
		}
		//CspUtils.DebugLog("battleCard.BundleName=" + battleCard.BundleName);
		return battleCard.BundleName;
	}

	public static Texture2D LoadCardTexture(string cardType)
	{
		string cardBundle = GetCardBundle(cardType);
		if (string.IsNullOrEmpty(cardBundle))
		{
			return null;
		}
		return LoadCardTexture(cardType, cardBundle);
	}

	public static Texture2D LoadCardTexture(string cardType, string bundleName)
	{
		if (!_instance.cardTextureBundles.ContainsKey(bundleName))
		{
			CspUtils.DebugLog("Unknown bundle " + bundleName + " in LoadCardTexture");
			return null;
		}
		CspUtils.DebugLog("bundleName=" + bundleName);
		CspUtils.DebugLog("cardType=" + cardType);
		Object obj = _instance.cardTextureBundles[bundleName].Load(cardType, typeof(Texture2D));
		if (obj == null) {
			//CspUtils.DebugLog("LoadCardTexture Load result is null!");
		}
		return obj as Texture2D;
	}
}
