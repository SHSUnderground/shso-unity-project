using System;

public class HeroPersisted : ShsCollectionItem
{
	private string name;

	private bool placed;

	private string authCode = string.Empty;

	private SafeInt xp; //int xp;

	private int tier;

	private bool readOnly;

	public int maxScavengeObjects = 5;

	public int objectsCollected;

	private string _scavengerInfo = string.Empty;

	public string scavengerInfo
	{
		get
		{
			return _scavengerInfo;
		}
		set
		{
			_scavengerInfo = value;
			string[] array = _scavengerInfo.Split(',');
			maxScavengeObjects = array.Length;
			if (maxScavengeObjects < 3)
			{
				CspUtils.DebugLog("ERROR:  possible bad data for scavenge info on " + name + ": " + _scavengerInfo);
			}
			objectsCollected = 0;
			for (int i = 0; i < maxScavengeObjects; i++)
			{
				if (array[i] == "0")
				{
					objectsCollected++;
				}
			}
		}
	}

	public override bool ShieldAgentOnly
	{
		get
		{
			return shieldAgentOnly;
		}
	}

	public string Name
	{
		get
		{
			return name;
		}
	}

	public int MaxLevel
	{
		get
		{
			if (tier == 1)
			{
				return StatLevelReqsDefinition.MAX_HERO_LEVEL_BADGE1;
			}
			if (tier == 2)
			{
				return StatLevelReqsDefinition.MAX_HERO_LEVEL_BADGE2;
			}
			return StatLevelReqsDefinition.MAX_HERO_LEVEL_NORMAL;
		}
	}

	public int Tier
	{
		get
		{
			return tier;
		}
	}

	public int Level
	{
		get
		{
			if (XpToLevelDefinition.Instance != null)
			{
				return XpToLevelDefinition.Instance.GetLevelForXp(Xp, tier);
			}
			return 1;
		}
	}

	public int Xp
	{
		get
		{
			return xp;
		}
		set
		{
			CspUtils.DebugLog("Xp set called!");

			if (readOnly)
			{
				throw new NotSupportedException();
			}
			if (value >= xp)
			{
				xp = value;
				AppShell.Instance.EventMgr.Fire(this, new HeroXPUpdateMessage(Name));
			}
			else
			{
				CspUtils.DebugLog(string.Format("Asked to set xp to {0}, but the value is lower then current client's view of xp {1}", value, xp));
			}
		}
	}

	public string Code
	{
		get
		{
			return authCode;
		}
	}

	public bool Placed
	{
		get
		{
			return placed;
		}
		set
		{
			if (readOnly)
			{
				throw new NotSupportedException();
			}
			placed = value;
			AppShell.Instance.EventMgr.Fire(this, new HeroCollectionUpdateMessage(Name));
		}
	}

	public HeroPersisted()
	{
	}

	public HeroPersisted(string heroName)
	{
		name = heroName;
	}

	public HeroPersisted(DataWarehouse xmlData)
	{
		InitializeFromData(xmlData);
	}

	public HeroPersisted(RemotePlayerProfileJsonHeroData jsonData)
	{
		InitializeFromData(jsonData);
	}

	public bool InitializeFromData(RemotePlayerProfileJsonHeroData jsonData)
	{
		name = jsonData.name.ToLower();
		readOnly = true;
		int xpTemp = 0;  // CSP
		bool retval =  int.TryParse(jsonData.xp, out xpTemp);  // CSP
		xp.SetValue(xpTemp);  // CSP 
		return retval;  // CSP 
	}

	public override bool InitializeFromData(DataWarehouse data)
	{
		base.InitializeFromData(data);
		string text = data.TryGetString("name", null);
		if (text != null)
		{
			name = text.ToLower();
		}
		Xp = data.TryGetInt("xp", Xp);
		tier = data.TryGetInt("tier", 0);
		authCode = data.TryGetString("code", string.Empty);
		if (string.IsNullOrEmpty(authCode))
		{
			shieldAgentOnly = true;
		}
		return true;
	}

	public override void UpdateFromData(DataWarehouse data)
	{
		if (readOnly)
		{
			throw new NotSupportedException();
		}
		string text = data.TryGetString("name", null);
		if (text != null)
		{
			text = text.ToLower();
		}
		if (text != name)
		{
			CspUtils.DebugLog("Hero names <" + name + ", " + text + "> do not match while updating from data!  Cannot continue.");
			return;
		}
		Xp = data.TryGetInt("xp", Xp);
		tier = data.TryGetInt("tier", tier);
		authCode = data.TryGetString("code", string.Empty);
	}

	public void UpdateFromResult(MissionResults characterData)
	{
		if (readOnly)
		{
			throw new NotSupportedException();
		}
		if (characterData.earnedXp > 0)
		{
			if (characterData.currentXp == -1)
			{
				CspUtils.DebugLog("Using old method to update XP:" + characterData.earnedXp);
				UpdateXp(characterData.earnedXp);
			}
			else
			{
				Xp = characterData.currentXp;
			}
		}
	}

	public void UpdateXp(int addXp, bool addMultiplier = false)
	{
		if (readOnly)
		{
			throw new NotSupportedException();
		}
		if (addMultiplier)
		{
			addXp = (int)((double)(float)addXp * AppShell.Instance.Profile.xpMultiplier);
		}
		Xp += addXp;
	}

	public override string GetKey()
	{
		return name.ToLower();
	}
}
