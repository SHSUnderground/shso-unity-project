public class StageDefinition : StaticDataDefinition, IStaticDataDefinition
{
	protected int id;

	protected string type;

	protected string orders;

	protected string geometryBundleName;

	protected string scenarioBundleName;

	protected string objectiveType;

	protected int minimumPlayers = 1;

	protected int maximumPlayers = 4;

	protected string cinematicBundleName;

	protected string splashImage;

	public int Id
	{
		get
		{
			return id;
		}
	}

	public string Type
	{
		get
		{
			return type;
		}
	}

	public string Orders
	{
		get
		{
			return orders;
		}
	}

	public string GeometryBundleName
	{
		get
		{
			return geometryBundleName;
		}
	}

	public string ScenarioBundleName
	{
		get
		{
			return scenarioBundleName;
		}
	}

	public string ObjectiveType
	{
		get
		{
			return objectiveType;
		}
	}

	public int MinimumPlayers
	{
		get
		{
			return minimumPlayers;
		}
	}

	public int MaximumPlayers
	{
		get
		{
			return maximumPlayers;
		}
	}

	public string CinematicBundleName
	{
		get
		{
			return cinematicBundleName;
		}
	}

	public string SplashImage
	{
		get
		{
			return splashImage;
		}
	}

	public void InitializeFromData(DataWarehouse data)
	{
		id = data.GetInt("stage_number");
		scenarioBundleName = data.GetString("scenario_bundle");
	}

	public void BuildOrdersLookup(string lookUpId)
	{
		if (orders == null)
		{
			orders = lookUpId + "_S" + id + "_ORDERS";
		}
	}
}
