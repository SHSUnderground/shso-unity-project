using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.XPath;

public class DeckBuilderConfigDefinition : StaticDataDefinition, IStaticDataDefinition
{
	public static DeckBuilderConfigDefinition Instance;

	public List<DeckTheme> themes = new List<DeckTheme>();

	[CompilerGenerated]
	private int _003CLevelWeightMultiplier_003Ek__BackingField;

	[CompilerGenerated]
	private int _003CByLevelExpected_003Ek__BackingField;

	[CompilerGenerated]
	private int _003CBlockWeightMultiplier_003Ek__BackingField;

	[CompilerGenerated]
	private int _003CByBlockExpected_003Ek__BackingField;

	[CompilerGenerated]
	private int _003CFactorWeightMultiplier_003Ek__BackingField;

	[CompilerGenerated]
	private int _003CMax_Level_003Ek__BackingField;

	[CompilerGenerated]
	private float _003CBaseTopPickChance_003Ek__BackingField;

	[CompilerGenerated]
	private float _003CNextPickChanceMult_003Ek__BackingField;

	[CompilerGenerated]
	private float _003CLevelMagnitude_003Ek__BackingField;

	[CompilerGenerated]
	private float _003CFactorMagnitude_003Ek__BackingField;

	[CompilerGenerated]
	private float _003CBlockMagnitude_003Ek__BackingField;

	public int LevelWeightMultiplier
	{
		[CompilerGenerated]
		get
		{
			return _003CLevelWeightMultiplier_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CLevelWeightMultiplier_003Ek__BackingField = value;
		}
	}

	public int ByLevelExpected
	{
		[CompilerGenerated]
		get
		{
			return _003CByLevelExpected_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CByLevelExpected_003Ek__BackingField = value;
		}
	}

	public int BlockWeightMultiplier
	{
		[CompilerGenerated]
		get
		{
			return _003CBlockWeightMultiplier_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CBlockWeightMultiplier_003Ek__BackingField = value;
		}
	}

	public int ByBlockExpected
	{
		[CompilerGenerated]
		get
		{
			return _003CByBlockExpected_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CByBlockExpected_003Ek__BackingField = value;
		}
	}

	public int FactorWeightMultiplier
	{
		[CompilerGenerated]
		get
		{
			return _003CFactorWeightMultiplier_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CFactorWeightMultiplier_003Ek__BackingField = value;
		}
	}

	public int Max_Level
	{
		[CompilerGenerated]
		get
		{
			return _003CMax_Level_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CMax_Level_003Ek__BackingField = value;
		}
	}

	public float BaseTopPickChance
	{
		[CompilerGenerated]
		get
		{
			return _003CBaseTopPickChance_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CBaseTopPickChance_003Ek__BackingField = value;
		}
	}

	public float NextPickChanceMult
	{
		[CompilerGenerated]
		get
		{
			return _003CNextPickChanceMult_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CNextPickChanceMult_003Ek__BackingField = value;
		}
	}

	public float LevelMagnitude
	{
		[CompilerGenerated]
		get
		{
			return _003CLevelMagnitude_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CLevelMagnitude_003Ek__BackingField = value;
		}
	}

	public float FactorMagnitude
	{
		[CompilerGenerated]
		get
		{
			return _003CFactorMagnitude_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CFactorMagnitude_003Ek__BackingField = value;
		}
	}

	public float BlockMagnitude
	{
		[CompilerGenerated]
		get
		{
			return _003CBlockMagnitude_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CBlockMagnitude_003Ek__BackingField = value;
		}
	}

	public DeckTheme FindTheme(string name)
	{
		foreach (DeckTheme theme in themes)
		{
			if (theme.name == name)
			{
				return theme;
			}
		}
		return null;
	}

	public void InitializeFromData(DataWarehouse data)
	{
		if (data.GetCount("deckbuilder_config") != 1)
		{
			CspUtils.DebugLog("Invalid number of autodeck config data blocks!");
		}
		DataWarehouse data2 = data.GetData("deckbuilder_config");
		LevelWeightMultiplier = data2.TryGetInt("level_weight_multiplier", 10);
		ByLevelExpected = data2.TryGetInt("by_level_expected", 1);
		BlockWeightMultiplier = data2.TryGetInt("block_weight_multiplier", 25);
		ByBlockExpected = data2.TryGetInt("by_block_expected", 7);
		FactorWeightMultiplier = data2.TryGetInt("factor_weight_multiplier", 10);
		Max_Level = data2.TryGetInt("max_level", 12);
		BaseTopPickChance = data2.TryGetFloat("base_top_pick_chance", 0.5f);
		NextPickChanceMult = data2.TryGetFloat("next_pick_chance_mult", 0.5f);
		LevelMagnitude = data2.TryGetFloat("level_magnitude", 1f);
		BlockMagnitude = data2.TryGetFloat("block_magnitude", 1f);
		FactorMagnitude = data2.TryGetFloat("factor_magnitude", 1f);
		DataWarehouse data3 = data2.GetData("themes");
		foreach (DataWarehouse item in data3.GetIterator("theme"))
		{
			DeckTheme deckTheme = new DeckTheme();
			deckTheme.name = item.TryGetString("name", string.Empty);
			deckTheme.decklist = item.TryGetString("deck", string.Empty);
			XPathNodeIterator values = item.GetValues("affinity");
			while (values.MoveNext())
			{
				deckTheme.affinity.Add(values.Current.Value);
			}
			themes.Add(deckTheme);
		}
	}
}
