using System.Collections.Generic;

public class CharacterDescriptionsManager : StaticDataDefinition, IStaticDataDefinition
{
	public class CharacterDescription
	{
		public const string StringTablePrefix = "CIN";

		private readonly bool isVillain;

		private string characterKey;

		private string characterFamilyInternal;

		public string LongDescription
		{
			get
			{
				return getSTLookupKey("LDESC");
			}
		}

		public string ShortDescription
		{
			get
			{
				return getSTLookupKey("SDESC");
			}
		}

		public string CharacterName
		{
			get
			{
				return getSTLookupKey("EXNM");
			}
		}

		public string CharacterFamily
		{
			get
			{
				return getSTLookupKey("FMLY");
			}
		}

		public bool IsVillain
		{
			get
			{
				return isVillain;
			}
		}

		public string CharacterKey
		{
			get
			{
				return characterKey;
			}
		}

		public string CharacterFamilyInternal
		{
			get
			{
				return characterFamilyInternal;
			}
		}

		public CharacterDescription(string characterKey, string characterFamily, bool isVillain)
		{
			this.characterKey = characterKey;
			characterFamilyInternal = characterFamily;
			this.isVillain = isVillain;
		}

		private string getSTLookupKey(string prefix)
		{
			string id = string.Format("{0}{1}_{2}_{3}", '#', "CIN", characterKey, prefix).ToUpper();
			return AppShell.Instance.stringTable[id];
		}
	}

	private Dictionary<string, CharacterDescription> desLookup = new Dictionary<string, CharacterDescription>();

	private List<string> villains = new List<string>();

	public CharacterDescription this[string characterKey]
	{
		get
		{
			CharacterDescription characterDescription = null;
			if (!string.IsNullOrEmpty(characterKey))
			{
				characterDescription = ((!desLookup.ContainsKey(characterKey)) ? null : desLookup[characterKey]);
			}
			if (characterDescription != null)
			{
				return characterDescription;
			}
			CspUtils.DebugLog("Character '" + characterKey + "' does not exist in the characterdescription.xml");
			return new CharacterDescription(characterKey, "NoFamily", false);
		}
	}

	public List<string> CharacterList
	{
		get
		{
			return new List<string>(desLookup.Keys);
		}
	}

	public List<string> VillainList
	{
		get
		{
			return villains;
		}
	}

	public bool Contains(string characterKey)
	{
		if (!string.IsNullOrEmpty(characterKey))
		{
			return desLookup.ContainsKey(characterKey);
		}
		return false;
	}

	public List<string> GetCharacterList()
	{
		return new List<string>(desLookup.Keys);
	}

	public void InitializeFromData(DataWarehouse data)
	{
		string text = "notFound";
		desLookup.Clear();
		foreach (DataWarehouse item in data.GetIterator("//heros/hero"))
		{
			string text2 = item.TryGetString("name", text);
			string text3 = item.TryGetString("family", text);
			if (text2 != text)
			{
				if (text3 == text)
				{
					text3 = text2;
				}
				if (!desLookup.ContainsKey(text2))
				{
					desLookup.Add(text2, new CharacterDescription(text2, text3, false));
				}
			}
			else
			{
				CspUtils.DebugLog("incorrect xml formating for character descritpion");
			}
		}
		foreach (DataWarehouse item2 in data.GetIterator("//villains/villain"))
		{
			string text4 = item2.TryGetString("name", text);
			string text5 = item2.TryGetString("family", text);
			if (text4 != text)
			{
				villains.Add(text4);
				if (text5 == text)
				{
					text5 = text4;
				}
				if (!desLookup.ContainsKey(text4))
				{
					desLookup.Add(text4, new CharacterDescription(text4, text5, true));
				}
			}
			else
			{
				CspUtils.DebugLog("incorrect xml formating for character descritpion");
			}
		}
	}
}
