using System.Collections.Generic;
using System.Xml.XPath;
using UnityEngine;

public class MissionDefinition : StaticDataDefinition, IStaticDataDefinition
{
	public enum Ratings
	{
		None = -1,
		Bronze,
		Silver,
		Gold,
		Adamantium
	}

	public const int SUPPORTED_PLAYERS = 4;

	protected string id;

	protected string name;

	protected string description;

	protected string cinematicBundleName;

	protected string coverPageName;

	protected string titleKey;

	protected string missionBriefingSpeaker;

	protected string missionSplashScreenOverride;

	protected string missionPurchaseOverride;

	protected bool displayTimer;

	protected int targetLevel;

	protected Vector2 mapCoordinates;

	public List<string> blacklistCharacters = new List<string>();

	protected int[][] scoreNeededForRating;

	protected int minimumLevel = 1;

	protected int maximumLevel = 50;

	protected string prerequisitMission;

	protected Dictionary<int, StageDefinition> stages;

	protected int lastStage;

	public string Id
	{
		get
		{
			return id;
		}
	}

	public string Name
	{
		get
		{
			return name;
		}
	}

	public string Description
	{
		get
		{
			return description;
		}
	}

	public string CinematicBundleName
	{
		get
		{
			return cinematicBundleName;
		}
	}

	public string CoverPageName
	{
		get
		{
			return coverPageName;
		}
	}

	public string TitleKey
	{
		get
		{
			return titleKey;
		}
	}

	public Vector2 MapCoordinates
	{
		get
		{
			return mapCoordinates;
		}
	}

	public int TargetLevel
	{
		get
		{
			return targetLevel;
		}
	}

	public int MinimumLevel
	{
		get
		{
			return minimumLevel;
		}
	}

	public int MaximumLevel
	{
		get
		{
			return maximumLevel;
		}
	}

	public string PrerequisitMission
	{
		get
		{
			return prerequisitMission;
		}
	}

	public int LastStage
	{
		get
		{
			return lastStage;
		}
	}

	public string MissionSplashScreenOverride
	{
		get
		{
			return missionSplashScreenOverride;
		}
	}

	public string MissionPurchaseOverride
	{
		get
		{
			return missionPurchaseOverride;
		}
	}

	public bool DisplayTimer
	{
		get
		{
			return displayTimer;
		}
	}

	public MissionDefinition()
	{
		scoreNeededForRating = new int[4][];
		for (int i = 0; i < 4; i++)
		{
			scoreNeededForRating[i] = new int[4];
		}
		stages = new Dictionary<int, StageDefinition>();
	}

	public StageDefinition StageDefinition(int stageNumber)
	{
		return stages[stageNumber];
	}

	public int ScoreNeededForRating(Ratings ratingValue, int playerCount)
	{
		if (playerCount < 1 || playerCount > 4)
		{
			CspUtils.DebugLog("Invalid number of players used for scoring lookup: " + playerCount);
			playerCount = 1;
		}
		return scoreNeededForRating[playerCount - 1][(int)ratingValue];
	}

	public void ReadMedalTable(DataWarehouse data, Ratings medalRank, string medalPath)
	{
		for (int i = 0; i < 4; i++)
		{
			scoreNeededForRating[i][(int)medalRank] = data.GetInt(medalPath + "/player", i);
		}
	}

	public void InitializeFromData(DataWarehouse data)
	{
		XPathNavigator value = data.GetValue(".");
		value.MoveToFirstChild();
		CspUtils.DebugLog("Parsing mission data for <" + value.LocalName + ">. " + data.TryGetString("//blacklist_characters", string.Empty));
		id = value.LocalName.ToUpper();
		string text = '#' + "MIS_" + id;
		name = text + "_NAME";
		description = text + "_DESC";
		cinematicBundleName = string.Empty;
		coverPageName = string.Empty;
		titleKey = string.Empty;
		if (data.TryGetString("//blacklist_characters", string.Empty) != string.Empty)
		{
			string @string = data.GetString("//blacklist_characters");
			CspUtils.DebugLog("MissionDefinition: " + name + " blacklist is " + @string);
			if (@string != null && @string.Length > 0)
			{
				string[] array = @string.Split(',');
				string[] array2 = array;
				foreach (string text2 in array2)
				{
					blacklistCharacters.Add(text2);
					CspUtils.DebugLog("MissionDefinition: adding " + text2 + " to blacklist");
				}
			}
		}
		missionBriefingSpeaker = data.TryGetString("//mission_briefing_speaker", null);
		if (missionBriefingSpeaker != null)
		{
			VOInputMissionBriefingSpeaker.AddMissionBriefingSpeaker(value.LocalName, missionBriefingSpeaker);
		}
		missionSplashScreenOverride = data.TryGetString("//mission_splash_screen_override", null);
		missionPurchaseOverride = data.TryGetString("//mission_purchase_override", null);
		displayTimer = data.TryGetBool("//display_timer", false);
		mapCoordinates.x = data.GetFloat("//map/x");
		mapCoordinates.y = data.GetFloat("//map/y");
		targetLevel = data.GetInt("//target_level");
		minimumLevel = data.TryGetInt("//prerequisites/minimum_level", minimumLevel);
		maximumLevel = data.TryGetInt("//prerequisites/maximum_level", maximumLevel);
		prerequisitMission = data.TryGetString("//prerequisites/completed", null);
		ReadMedalTable(data, Ratings.Bronze, "//rating/bronze/score_needed");
		ReadMedalTable(data, Ratings.Silver, "//rating/silver/score_needed");
		ReadMedalTable(data, Ratings.Gold, "//rating/gold/score_needed");
		ReadMedalTable(data, Ratings.Adamantium, "//rating/adamantium/score_needed");
		XPathNodeIterator values = data.GetValues("//stage");
		foreach (XPathNavigator item in values)
		{
			DataWarehouse data2 = new DataWarehouse(item);
			StageDefinition stageDefinition = new StageDefinition();
			stageDefinition.InitializeFromData(data2);
			stageDefinition.BuildOrdersLookup(text);
			if (stageDefinition.Id >= 0)
			{
				stages.Add(stageDefinition.Id, stageDefinition);
			}
			else
			{
				CspUtils.DebugLog("Failed to get a stage ID while parsing mission <" + id + "> so we could not add it to the list of stages.");
			}
		}
		lastStage = 0;
		foreach (int key in stages.Keys)
		{
			if (lastStage < key)
			{
				lastStage = key;
			}
		}
	}

	public static Ratings RatingForRatingChar(string ratingChar)
	{
		Ratings result = Ratings.Bronze;
		if (ratingChar.Length < 1)
		{
			CspUtils.DebugLog("Asked to convert invalid rating <" + ratingChar + "> into a rating.  Failing.");
		}
		else
		{
			switch (ratingChar.ToLower()[0])
			{
			case 'a':
				result = Ratings.Adamantium;
				break;
			case 'g':
				result = Ratings.Gold;
				break;
			case 's':
				result = Ratings.Silver;
				break;
			case 'b':
				result = Ratings.Bronze;
				break;
			default:
				CspUtils.DebugLog("Asked to convert invalid character <" + ratingChar + "> into a rating.  Failing.");
				result = Ratings.Bronze;
				break;
			}
		}
		return result;
	}
}
