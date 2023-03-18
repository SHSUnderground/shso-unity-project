using System.Collections.Generic;

public class CardListCard : BattleCard
{
	public CardInventoryCounter counterComponent;

	public bool isFiltered;

	private int available;

	public int score;

	private List<string> tags = new List<string>();

	private bool baseScoreSet;

	private int baseScore;

	private CardAffinity affinity;

	public int Available
	{
		get
		{
			return available;
		}
		set
		{
			if (value < 0)
			{
				CspUtils.DebugLog("Invalid quantity set for card " + type);
				return;
			}
			available = value;
			UpdateCounter();
		}
	}

	public List<string> Tags
	{
		get
		{
			return tags;
		}
	}

	public int BaseScore
	{
		get
		{
			if (!baseScoreSet)
			{
				baseScoreSet = true;
				string[] array = affinityText.Split('\n');
				string[] array2 = array;
				foreach (string text in array2)
				{
					if (text.Length > 0 && text[text.Length - 1] == '!')
					{
						string[] array3 = text.Split('!');
						int result;
						if (int.TryParse(array3[0], out result))
						{
							baseScore = result;
						}
					}
				}
			}
			return baseScore;
		}
	}

	public CardAffinity Affinity
	{
		get
		{
			if (affinity == null)
			{
				affinity = new CardAffinity();
				string[] array = affinityText.Split('\n');
				string[] array2 = array;
				foreach (string text in array2)
				{
					if (text.Length > 0)
					{
						affinity.Add(text);
					}
				}
			}
			return affinity;
		}
	}

	public CardListCard(BattleCard c, int avail)
		: base(c)
	{
		isFiltered = false;
		available = avail;
		counterComponent = null;
		affinity = null;
	}

	public override int CompareTo(object Obj)
	{
		return 0;
	}

	public void UpdateCounter()
	{
		if (counterComponent != null)
		{
			counterComponent.count = available;
		}
	}

	public bool HasFactor(string f)
	{
		return HasFactor(BattleCard.CharToFactor(f[0]));
	}

	public bool HasFactor(Factor factor)
	{
		for (int i = 0; i < attackFactors.Length; i++)
		{
			if (attackFactors[i] == factor)
			{
				return true;
			}
		}
		return false;
	}

	public bool HasBlock(string f)
	{
		return HasBlock(BattleCard.CharToFactor(f[0]));
	}

	public bool HasBlock(Factor factor)
	{
		for (int i = 0; i < blockFactors.Length; i++)
		{
			if (blockFactors[i] == factor)
			{
				return true;
			}
		}
		return false;
	}

	public bool BelongsToTeam(string team)
	{
		return teamName == team;
	}

	public static bool EquivalentCards(CardListCard c, BattleCard b)
	{
		return c.Type == b.Type && c.Foil == b.Foil;
	}

	public void GenerateAffinityTags()
	{
		string[] rulesHeroName = base.RulesHeroName;
		foreach (string text in rulesHeroName)
		{
			tags.Add("hero" + text.Replace(' ', '_'));
		}
		tags.Add("team" + teamName);
		if (isKeeper)
		{
			tags.Add("keeper");
		}
		Factor[] attackFactors = base.attackFactors;
		foreach (Factor f in attackFactors)
		{
			tags.Add("factor" + BattleCard.FactorToChar(f));
			tags.Add("keeper" + BattleCard.FactorToChar(f));
		}
		tags.Add("block" + BattleCard.FactorToChar(blockFactors[0]));
		tags.Add("level" + level);
		tags.Add("card" + type);
	}

	public string TagsString()
	{
		string text = string.Empty;
		foreach (string tag in tags)
		{
			text = text + tag + "\n";
		}
		return text;
	}
}
