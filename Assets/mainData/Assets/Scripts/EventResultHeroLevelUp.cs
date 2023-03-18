using System.Collections;

public class EventResultHeroLevelUp : EventResultBase
{
	protected int userId;

	protected string causingEvent;

	protected DataWarehouse heroLevelData;

	protected string heroName;

	protected int level;

	protected int xp;

	public int UserId
	{
		get
		{
			return userId;
		}
	}

	public string CausingEvent
	{
		get
		{
			return causingEvent;
		}
	}

	public DataWarehouse HeroLevelData
	{
		get
		{
			return heroLevelData;
		}
	}

	public string HeroName
	{
		get
		{
			return heroName;
		}
	}

	public int Level
	{
		get
		{
			return level;
		}
	}

	public int Xp
	{
		get
		{
			return xp;
		}
	}

	public EventResultHeroLevelUp()
		: base(EventResultType.HeroLevelUp)
	{
	}

	public override void InitializeFromData(DataWarehouse data)
	{
		userId = data.GetInt("userid");
		causingEvent = data.GetString("event");
		heroLevelData = data.GetData("hero");
		heroName = data.GetString("hero/name");
		level = data.GetInt("hero/level");
		xp = data.GetInt("hero/xp");
	}

	public override void InitializeFromData(Hashtable data)
	{
		userId = int.Parse((string)data["user_id"]);
		causingEvent = (string)data["event"];
		heroName = (string)data["hero"];
		level = int.Parse((string)data["level"]);
		xp = int.Parse((string)data["xp"]);
		string xmlDataString = "<hero><name>" + heroName + "</name><level>" + level + "</level><xp>" + xp + "</xp></hero>";
		heroLevelData = new DataWarehouse(xmlDataString);
	}
}
