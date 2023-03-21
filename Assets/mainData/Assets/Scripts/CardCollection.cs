using System.Collections.Generic;

public class CardCollection
{
	private static CardCollection _instance = new CardCollection();

	private Dictionary<string, int> collection;

	private bool error;

	private Dictionary<string, DeckProperties> deckList = new Dictionary<string, DeckProperties>();

	private bool isDoneFetching;

	public static Dictionary<string, int> Collection
	{
		get
		{
			return _instance.collection;
		}
	}

	public static bool IsError
	{
		get
		{
			return _instance.error;
		}
	}

	public static Dictionary<string, DeckProperties> DeckList
	{
		get
		{
			return _instance.deckList;
		}
	}

	public static bool IsDoneFetching
	{
		get
		{
			return _instance.isDoneFetching;
		}
	}

	private CardCollection()
	{
		deckList = new Dictionary<string, DeckProperties>();
	}

	public static void Fetch()
	{
		if (AppShell.Instance.Profile == null)
		{
			CspUtils.DebugLog("Trying to fetch card collection before a user profile has been created");
		}
		else
		{
			_instance._Fetch(AppShell.Instance.Profile.UserId);
		}
	}

	public static void Fetch(long userId)
	{
		_instance._Fetch(userId);
	}

	private void _Fetch(long userId)
	{
		isDoneFetching = false;
		string uri = "resources$users/cards.py";
		// string uri = "resources$users/" + userId + "/cards";
		ShsWebService component = Utils.GetComponent<ShsWebService>(AppShell.Instance.gameObject);
		if (component != null)
		{
			error = false;
			component.StartRequest(uri, OnCardCollectionLoaded, ShsWebService.ShsWebServiceType.RASP);
		}
		else
		{
			error = true;
			CspUtils.DebugLog("Unable to find WebService component");
		}
	}

	private void OnCardCollectionLoaded(ShsWebResponse response)
	{
		isDoneFetching = true;
		int num = 0;
		collection = new Dictionary<string, int>();
		if (response.Status == 200)
		{
			DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
			dataWarehouse.Parse();
			foreach (DataWarehouse item in dataWarehouse.GetIterator("cards/card"))
			{
				string @string = item.GetString("type");
				int @int = item.GetInt("count");
				collection[@string] = @int + num;
			}
			error = false;
		}
		else
		{
			error = true;
			CspUtils.DebugLog("Failed to card collection: " + response.Body);
		}
	}

	public static bool Contains(string recipe)
	{
		return _instance._Contains(recipe);
	}

	private bool _Contains(string recipe)
	{
		Dictionary<string, int> dictionary = CardManager.ParseRecipe(recipe);
		if (dictionary.Count < 1)
		{
			return false;
		}
		foreach (KeyValuePair<string, int> item in dictionary)
		{
			if (!collection.ContainsKey(item.Key) || collection[item.Key] < item.Value)
			{
				return false;
			}
		}
		return true;
	}

	public static void EnumerateDecks()
	{
		_instance.deckList.Clear();
		UserProfile profile = AppShell.Instance.Profile;
		if (profile != null)
		{
			string text = "resources$users/decks.py";
			// string text = "resources$users/" + profile.UserId + "/decks/";
			CspUtils.DebugLog("Retrieving user's decks from: " + text);
			ShsWebService component = Utils.GetComponent<ShsWebService>(AppShell.Instance.gameObject);
			if ((bool)component)
			{
				component.StartRequest(text, _instance.OnDeckListLoaded, ShsWebService.ShsWebServiceType.RASP);
			}
			else
			{
				CspUtils.DebugLog("Could not retrieve ShsWebService component from AppShell");
			}
		}
		else
		{
			CspUtils.DebugLog("Could not find user profile in AppShell");
		}
	}

	private void OnDeckListLoaded(ShsWebResponse response)
	{
		if (response.Status == 200)
		{
			DataWarehouse dataWarehouse = new DataWarehouse(response.Body);
			dataWarehouse.Parse();
			int count = dataWarehouse.GetCount("Decks/Deck");
			if (count > 0)
			{
				if (deckList != null)
				{
					deckList.Clear();
				}
				deckList = new Dictionary<string, DeckProperties>(count);
				for (int i = 0; i < count; i++)
				{
					try
					{
						DataWarehouse data = dataWarehouse.GetData("Decks/Deck", i);
						DeckProperties deckProperties = new DeckProperties();
						deckProperties.DeckName = data.GetString("Name");
						deckProperties.DeckRecipe = data.GetString("Cards");
						deckProperties.DeckId = data.GetInt("ID");
						deckProperties.ReadOnly = data.GetBool("Readonly");
						deckProperties.Legal = data.GetBool("Legal");
						deckProperties.HeroName = "spider_man";
						CspUtils.DebugLog(string.Format("Deck {0}, Name: {1}, ID: {2}, Legal: {3}, Readonly: {4}, Recipe: {5}", i, deckProperties.DeckName, deckProperties.DeckId, deckProperties.Legal, deckProperties.ReadOnly, deckProperties.DeckRecipe));
						deckList.Add(deckProperties.DeckName, deckProperties);
					}
					catch
					{
						CspUtils.DebugLog("Failed to parse deck recipe.");
					}
				}
			}
			else
			{
				CspUtils.DebugLog("User has no decks in their inventory; populating with fallback data.");
				PopulateFallbackDecks();
			}
		}
		else
		{
			CspUtils.DebugLog("Failed to retrieve user's deck list; populating with fallback data.");
			PopulateFallbackDecks();
		}
		AppShell.Instance.EventMgr.Fire(this, new CardGameEvent.DeckListLoaded());
	}

	private void PopulateFallbackDecks()
	{
		if (deckList != null)
		{
			deckList.Clear();
		}
		deckList = new Dictionary<string, DeckProperties>(1);
		DeckProperties deckProperties = null;
		deckProperties = new DeckProperties();
		deckProperties.DeckName = "Starter Deck";
		deckProperties.DeckRecipe = "ST005:1;ST038:1;ST047:2;ST048:4;ST105:1;ST142:4;ST148:2;ST150:4;ST165:4;ST172:4;ST183:4;ST187:4;ST194:1;ST272:1;ST353:3";
		deckProperties.DeckId = -1;
		deckProperties.ReadOnly = true;
		deckProperties.Legal = true;
		deckProperties.HeroName = "storm";
		deckList.Add(deckProperties.DeckName, deckProperties);
	}
}
