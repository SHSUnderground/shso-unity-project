using System;
using System.Collections;
using System.Xml.Serialization;

[XmlRoot(ElementName = "achievement")]
public class Achievement : IAchievement
{
	public enum AchievementTypeEnum
	{
		Threshold,
		Custom
	}

	public enum AchievementSourceEnum
	{
		Counter,
		Custom
	}

	public enum AchievementLevelEnum
	{
		Bronze,
		Silver,
		Gold,
		Adamantium,
		Unknown,
		NotAchieved
	}

	private Hashtable achievementNameLookup = new Hashtable();

	protected string id;

	protected string name;

	protected string shortDescription;

	protected string description;

	protected AchievementTypeEnum type;

	private string _xmltypestring;

	[XmlIgnore]
	public AchievementsManager manager;

	protected AchievementSourceEnum source;

	private string _xmlsourcestring;

	protected int[] thresholds;

	protected string[] counters;

	protected bool isConfigured;

	[XmlElement(ElementName = "id")]
	public string Id
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
		}
	}

	[XmlElement(ElementName = "name")]
	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
		}
	}

	[XmlElement(ElementName = "shortdesc")]
	public string ShortDescription
	{
		get
		{
			return shortDescription;
		}
		set
		{
			shortDescription = value;
		}
	}

	[XmlElement(ElementName = "description")]
	public string Description
	{
		get
		{
			return description;
		}
		set
		{
			description = value;
		}
	}

	public AchievementTypeEnum Type
	{
		get
		{
			return type;
		}
	}

	[XmlElement(ElementName = "type")]
	public string _XmlTypeString
	{
		get
		{
			return _xmltypestring;
		}
		set
		{
			_xmltypestring = value;
			type = (AchievementTypeEnum)(int)Enum.Parse(typeof(AchievementTypeEnum), _xmltypestring);
		}
	}

	[XmlIgnore]
	public AchievementsManager Manager
	{
		get
		{
			return manager;
		}
		set
		{
			manager = value;
		}
	}

	public AchievementSourceEnum Source
	{
		get
		{
			return source;
		}
	}

	[XmlElement(ElementName = "source")]
	public string _XmlSourceString
	{
		get
		{
			return _xmlsourcestring;
		}
		set
		{
			_xmlsourcestring = value;
			source = (AchievementSourceEnum)(int)Enum.Parse(typeof(AchievementSourceEnum), _xmlsourcestring);
		}
	}

	[XmlArrayItem("threshold")]
	[XmlArray(ElementName = "thresholds")]
	public int[] Thresholds
	{
		get
		{
			return thresholds;
		}
		set
		{
			thresholds = value;
		}
	}

	[XmlArray(ElementName = "counters")]
	[XmlArrayItem("counter")]
	public string[] Counters
	{
		get
		{
			return counters;
		}
		set
		{
			counters = value;
		}
	}

	public bool IsConfigured
	{
		get
		{
			return isConfigured;
		}
	}

	public Achievement()
	{
		achievementNameLookup[AchievementLevelEnum.Gold] = "Gold";
		achievementNameLookup[AchievementLevelEnum.Adamantium] = "Adamantium";
		achievementNameLookup[AchievementLevelEnum.Bronze] = "Bronze";
		achievementNameLookup[AchievementLevelEnum.NotAchieved] = "Not Achieved";
		achievementNameLookup[AchievementLevelEnum.Silver] = "Silver";
		achievementNameLookup[AchievementLevelEnum.Unknown] = "Unknown";
	}

	public virtual void Reset()
	{
	}

	public virtual void Initialize(object initData)
	{
	}

	public virtual AchievementLevelEnum GetLevel(string heroKey, object extraData)
	{
		return AchievementLevelEnum.Unknown;
	}

	public virtual AchievementLevelEnum GetLevel(string heroKey)
	{
		return AchievementLevelEnum.Unknown;
	}

	public string GetAchievementName(AchievementLevelEnum levelEnum)
	{
		if (achievementNameLookup.ContainsKey(levelEnum))
		{
			return (string)achievementNameLookup[levelEnum];
		}
		return string.Empty;
	}

	public static AchievementLevelEnum GetNextHighestLevel(AchievementLevelEnum current)
	{
		switch (current)
		{
		case AchievementLevelEnum.NotAchieved:
			return AchievementLevelEnum.Bronze;
		case AchievementLevelEnum.Bronze:
			return AchievementLevelEnum.Silver;
		case AchievementLevelEnum.Silver:
			return AchievementLevelEnum.Gold;
		case AchievementLevelEnum.Gold:
			return AchievementLevelEnum.Adamantium;
		case AchievementLevelEnum.Adamantium:
			return AchievementLevelEnum.Adamantium;
		default:
			return AchievementLevelEnum.Unknown;
		}
	}

	public virtual long GetLevelThreshold(AchievementLevelEnum level)
	{
		if (level == AchievementLevelEnum.NotAchieved || level == AchievementLevelEnum.Unknown)
		{
			return 0L;
		}
		if (type == AchievementTypeEnum.Threshold)
		{
			Array values = Enum.GetValues(typeof(AchievementLevelEnum));
			for (int i = 0; i < values.Length; i++)
			{
				if ((int)values.GetValue(i) == (int)level && thresholds.Length > i)
				{
					return thresholds[i];
				}
			}
			return -1L;
		}
		throw new NotImplementedException("Only threshold based achievements are supported.");
	}

	public virtual long GetCharacterLevelValue(string heroKey, object extraData)
	{
		throw new NotImplementedException("Level value must be overridden in subclass");
	}

	public virtual long GetCharacterLevelValue(string heroKey)
	{
		throw new NotImplementedException("Level value must be overridden in subclass");
	}

	public virtual string GetNameForLevel(AchievementLevelEnum level)
	{
		return AppShell.Instance.stringTable[name].Replace("{threshold}", GetLevelThreshold(level).ToString());
	}

	public virtual string GetDescriptionForLevel(AchievementLevelEnum level)
	{
		return AppShell.Instance.stringTable[description].Replace("{threshold}", GetLevelThreshold(level).ToString());
	}

	public virtual void ProcessSourceUpdate(object sourceData)
	{
	}

	public virtual void OnSourceUpdated(object sourceData)
	{
	}

	public virtual bool AchievementUpTest(AchievementLevelEnum prevLevel, AchievementLevelEnum nextLevel)
	{
		switch (prevLevel)
		{
		case AchievementLevelEnum.NotAchieved:
			return nextLevel != AchievementLevelEnum.NotAchieved;
		case AchievementLevelEnum.Bronze:
			return nextLevel != AchievementLevelEnum.NotAchieved && nextLevel != AchievementLevelEnum.Bronze;
		case AchievementLevelEnum.Silver:
			return nextLevel == AchievementLevelEnum.Gold || nextLevel == AchievementLevelEnum.Adamantium;
		case AchievementLevelEnum.Gold:
			return nextLevel == AchievementLevelEnum.Adamantium;
		default:
			return false;
		}
	}

	public virtual void DebugSetSourceValue(string heroKey, long value)
	{
	}
}
