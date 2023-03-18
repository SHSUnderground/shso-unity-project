using System.Xml.XPath;

public class CharacterDefinition : StaticDataDefinition, IStaticDataDefinition
{
	public const string ICON_PATH = "characters_bundle|";

	protected string id;

	protected string name;

	protected string bossBarIconName;

	protected string assetBundle;

	protected CharacterControllerData characterController;

	protected CharacterModelData characterModel;

	protected BehaviorManagerData behahaviorManager;

	protected CharacterMotionControllerData characterMotionController;

	protected CombatControllerData combatController;

	protected PlayerCombatControllerData playerCombatController;

	protected CharacterStatsData characterStats;

	protected EffectSequenceListData effectSequenceList;

	protected AIControllerData aiController;

	protected BossAIControllerData bossAIController;

	protected EnemyScoringData enemyScoringData;

	protected string inventoryIconName;

	public bool isVillain;

	public EnemyScoringData EnemyScoringData
	{
		get
		{
			return enemyScoringData;
		}
	}

	public string InventoryIconName
	{
		get
		{
			return InventoryIconButtonPath + "_normal";
		}
	}

	public string InventoryIconButtonPath
	{
		get
		{
			return "characters_bundle|" + inventoryIconName;
		}
	}

	public string TokenIconName
	{
		get
		{
			string inventoryIconButtonPath = InventoryIconButtonPath;
			return inventoryIconButtonPath.Replace("inventory_", "token_");
		}
	}

	public string CharacterName
	{
		get
		{
			return name;
		}
	}

	public string AssetBundle
	{
		get
		{
			return assetBundle;
		}
	}

	public string BossBarIconName
	{
		get
		{
			return bossBarIconName;
		}
	}

	public void InitializeFromData(DataWarehouse data)
	{
		XPathNavigator value = data.GetValue(".");
		value.MoveToFirstChild();
		id = data.GetString("//name");
		name = id;
		assetBundle = data.GetString("//asset_bundle");
		bossBarIconName = data.TryGetString("//boss_bar_icon_name", null);
		DataWarehouse data2 = data.GetData("//scoring");
		if (data2 != null)
		{
			enemyScoringData = new EnemyScoringData(data);
		}
		else
		{
			enemyScoringData = new EnemyScoringData();
		}
		inventoryIconName = data.TryGetString("//inventory_icon_name", "inventory_character_" + name);
		isVillain = data.TryGetBool("//is_villain", false);
	}
}
