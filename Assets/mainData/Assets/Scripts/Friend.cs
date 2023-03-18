public class Friend : ShsCollectionItem
{
	public enum LocationTypeEnum
	{
		CardGame,
		Brawler,
		HQ,
		GameWorld,
		Tutorial,
		Unknown
	}

	protected int id = -1;

	protected string name;

	protected string location;

	protected bool online;

	protected bool availableForActivity;

	protected LocationTypeEnum locationType;

	public int Id
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

	public string Location
	{
		get
		{
			return location;
		}
		set
		{
			location = GetDisplayLocation(value);
		}
	}

	public LocationTypeEnum LocationType
	{
		get
		{
			return locationType;
		}
	}

	public bool Online
	{
		get
		{
			return online;
		}
		set
		{
			online = value;
		}
	}

	public bool AvailableForActivity
	{
		get
		{
			return availableForActivity;
		}
		set
		{
			availableForActivity = value;
		}
	}

	public Friend()
	{
	}

	public Friend(DataWarehouse xmlData)
	{
		InitializeFromData(xmlData);
	}

	public Friend(string name, int id, string location, bool online, bool available)
	{
		this.name = name;
		this.id = id;
		this.location = location;
		this.online = online;
		availableForActivity = available;
	}

	public override bool InitializeFromData(DataWarehouse data)
	{
		base.InitializeFromData(data);
		name = data.TryGetString("name", null);
		id = data.TryGetInt("id", -1);
		string locationKey = data.TryGetString("location", string.Empty);
		online = (data.TryGetString("status", "offline") == "online");
		availableForActivity = (online && data.TryGetString("multiplayer", "0") == "0");
		location = GetDisplayLocation(locationKey);
		if (name == null)
		{
			CspUtils.DebugLog("Cannot load friend name from friend definition");
			return false;
		}
		if (id == -1)
		{
			CspUtils.DebugLog("Cannot load friend id from friend definition");
			return false;
		}
		return true;
	}

	public override void UpdateFromData(DataWarehouse data)
	{
		CspUtils.DebugLog("Needs Implementation!");
	}

	public override string GetKey()
	{
		return id.ToString();
	}

	private string GetDisplayLocation(string locationKey)
	{
		if (!Online)
		{
			return AppShell.Instance.stringTable["#friend_location_offline"];
		}
		locationType = LocationTypeEnum.GameWorld;
		if (!locationKey.Contains(":"))
		{
			return locationKey;
		}
		string[] array = locationKey.Split(':');
		if (array.Length < 1)
		{
			return locationKey;
		}
		string text = array[0];
		string text2 = array[1];
		for (int i = 2; i < array.Length; i++)
		{
			text2 += array[i];
		}
		string value = string.Empty;
		switch (text)
		{
		case "Card":
			locationType = LocationTypeEnum.CardGame;
			value = "#Location_Card_Game";
			break;
		case "Brawler":
			locationType = LocationTypeEnum.Brawler;
			value = "#Location_Mission";
			break;
		case "HQ":
			locationType = LocationTypeEnum.HQ;
			value = "#Location_HQ";
			break;
		case "GameWorld":
			locationType = ((!(text2 == "Tutorial_World")) ? LocationTypeEnum.GameWorld : LocationTypeEnum.Tutorial);
			value = "#Location_" + text2;
			break;
		}
		if (string.IsNullOrEmpty(value))
		{
			return locationKey;
		}
		return AppShell.Instance.stringTable[value];
	}
}
