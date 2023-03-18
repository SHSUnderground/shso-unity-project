using System;
using System.Xml;
using UnityEngine;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

public class BattleCard : IComparable
{
	public enum Factor
	{
		None,
		Universal,
		Strength,
		Speed,
		Elemental,
		Tech,
		Animal,
		Energy
	}

	public enum CardRarity
	{
		None,
		Common,
		Uncommon,
		Rare,
		SuperRare
	}

	public enum CardSet
	{
		None,
		Silver,
		Gold,
		Quest,
		Advanced
	}

	public enum Team
	{
		None = 48,
		Avengers = 65,
		Xmen = 88,
		Brotherhood = 66,
		SS = 83,
		F4 = 70
	}

	private const float revealDuration = 5f;

	protected GameObject cardObj;

	protected CardProperties cardProps;

	protected Texture2D fullTexture;

	protected Texture2D miniTexture;

	protected Animation animationComponent;

	protected Renderer[] rendererComponents;

	private bool isVisible = true;

	protected string type = string.Empty;

	protected string name;

	protected string nameEng;

	protected string abilityText;

	protected string abilityTextEng;

	protected string affinityText;

	protected string flavorText;

	protected string flavorTextEng;

	protected int level;

	protected int damage;

	protected int instance;

	protected int serverID;

	protected Factor[] attackFactors;

	protected Factor[] blockFactors;

	protected string teamName;

	protected string heroName;

	protected string heroNameEng;

	protected bool isIdentified;

	protected bool isMoving;

	protected bool isFoil;

	protected CardRarity rarity;

	protected CardSet set;

	protected string[] internalHeroName;

	protected string[] rulesHeroName;

	protected string ruleScriptText;

	protected SquadBattleAttackPattern attackPattern;

	protected SquadBattleAttackPattern keeperPattern;

	protected string bundleName;

	protected float revealTimestamp;

	protected bool isFaceup;

	protected bool isKeeper;

	protected bool preventKO;

	public static Factor[] FactorList = new Factor[6]
	{
		Factor.Strength,
		Factor.Speed,
		Factor.Elemental,
		Factor.Tech,
		Factor.Animal,
		Factor.Energy
	};

	public bool IsVisible
	{
		get
		{
			return isVisible;
		}
		set
		{
			isVisible = value;
			Renderer[] array = rendererComponents;
			foreach (Renderer renderer in array)
			{
				renderer.enabled = isVisible;
			}
		}
	}

	public string Type
	{
		get
		{
			return type;
		}
		set
		{
			type = value;
		}
	}

	public string Name
	{
		get
		{
			return AppShell.Instance.stringTable[name];
		}
		set
		{
			name = value;
		}
	}

	public string NameEng
	{
		get
		{
			return nameEng;
		}
		set
		{
			nameEng = value;
		}
	}

	public string HeroName
	{
		get
		{
			return AppShell.Instance.stringTable[heroName];
		}
		set
		{
			heroName = value;
		}
	}

	public string HeroNameEng
	{
		get
		{
			return heroNameEng;
		}
		set
		{
			heroNameEng = value;
		}
	}

	public string AbilityText
	{
		get
		{
			return AppShell.Instance.stringTable[abilityText];
		}
		set
		{
			abilityText = value;
		}
	}

	public string AbilityTextEng
	{
		get
		{
			return abilityTextEng;
		}
		set
		{
			abilityTextEng = value;
		}
	}

	public string AffinityText
	{
		get
		{
			return affinityText;
		}
		set
		{
			affinityText = value;
		}
	}

	public string FlavorText
	{
		get
		{
			return AppShell.Instance.stringTable[flavorText];
		}
		set
		{
			flavorText = value;
		}
	}

	public string FlavorTextEng
	{
		get
		{
			return flavorTextEng;
		}
		set
		{
			flavorTextEng = value;
		}
	}

	public string RuleScriptText
	{
		get
		{
			return ruleScriptText;
		}
		set
		{
			ruleScriptText = value;
		}
	}

	public Factor[] AttackFactors
	{
		get
		{
			return attackFactors;
		}
		set
		{
			attackFactors = (Factor[])value.Clone();
		}
	}

	public Factor[] BlockFactors
	{
		get
		{
			return blockFactors;
		}
		set
		{
			blockFactors = (Factor[])value.Clone();
		}
	}

	public string[] InternalHeroName
	{
		get
		{
			return internalHeroName;
		}
		set
		{
			internalHeroName = value;
		}
	}

	public string[] RulesHeroName
	{
		get
		{
			return rulesHeroName;
		}
		set
		{
			rulesHeroName = value;
		}
	}

	public int Instance
	{
		get
		{
			return instance;
		}
		set
		{
			instance = value;
		}
	}

	public int Level
	{
		get
		{
			return level;
		}
		set
		{
			level = value;
		}
	}

	public int Damage
	{
		get
		{
			return damage;
		}
		set
		{
			damage = value;
		}
	}

	public int ServerID
	{
		get
		{
			return serverID;
		}
		set
		{
			serverID = value;
		}
	}

	public bool Identified
	{
		get
		{
			return isIdentified;
		}
		set
		{
			isIdentified = value;
		}
	}

	public bool Moving
	{
		get
		{
			return isMoving;
		}
		set
		{
			isMoving = value;
		}
	}

	public bool Foil
	{
		get
		{
			return isFoil;
		}
		set
		{
			isFoil = value;
		}
	}

	public CardRarity Rarity
	{
		get
		{
			return rarity;
		}
		set
		{
			rarity = value;
		}
	}

	public CardSet Set
	{
		get
		{
			return set;
		}
		set
		{
			set = value;
		}
	}

	public Texture2D FullTexture
	{
		get
		{
			return fullTexture;
		}
		set
		{
			fullTexture = value;
		}
	}

	public string BundleName
	{
		get
		{
			return bundleName;
		}
	}

	public bool Faceup
	{
		get
		{
			return isFaceup;
		}
		set
		{
			isFaceup = value;
		}
	}

	public bool IsKeeper
	{
		get
		{
			return isKeeper;
		}
		set
		{
			isKeeper = value;
		}
	}

	public bool PreventKO
	{
		get
		{
			return preventKO;
		}
		set
		{
			preventKO = value;
		}
	}

	public SquadBattleAttackPattern AttackPattern
	{
		get
		{
			return attackPattern;
		}
		set
		{
			attackPattern = new SquadBattleAttackPattern(value);
		}
	}

	public SquadBattleAttackPattern KeeperPattern
	{
		get
		{
			return keeperPattern;
		}
		set
		{
			keeperPattern = new SquadBattleAttackPattern(value);
		}
	}

	public GameObject CardObj
	{
		get
		{
			return cardObj;
		}
		set
		{
			cardObj = value;
			cardProps = Utils.GetComponent<CardProperties>(cardObj);
			animationComponent = (cardObj.GetComponentInChildren(typeof(Animation)) as Animation);
			rendererComponents = cardObj.GetComponentsInChildren<Renderer>();
			cardProps.Card = this;
		}
	}

	public Animation Animation
	{
		get
		{
			return animationComponent;
		}
	}

	public CardProperties CardProps
	{
		get
		{
			return cardProps;
		}
	}

	public Texture2D MiniTexture
	{
		get
		{
			return miniTexture;
		}
		set
		{
			miniTexture = value;
			if ((bool)cardObj && (bool)cardObj.renderer)
			{
				CspUtils.DebugLog("setting mainTexture to " + miniTexture);  // CSP
				cardObj.renderer.materials[cardObj.renderer.materials.Length - 1].mainTexture = miniTexture;
			}
		}
	}

	public BattleCard()
	{
		attackFactors = new Factor[1];
		blockFactors = new Factor[1];
		internalHeroName = new string[1];
		rulesHeroName = new string[1];
		teamName = null;
		fullTexture = null;
		miniTexture = null;
		isFoil = false;
		rarity = CardRarity.Common;
		set = CardSet.None;
		isFaceup = false;
		isKeeper = false;
	}

	public BattleCard(int sid)
	{
		attackFactors = new Factor[1];
		blockFactors = new Factor[1];
		internalHeroName = new string[1];
		rulesHeroName = new string[1];
		teamName = null;
		fullTexture = null;
		miniTexture = null;
		isFoil = false;
		isIdentified = false;
		rarity = CardRarity.Common;
		set = CardSet.None;
		serverID = sid;
		isFaceup = false;
		isKeeper = false;
	}

	public BattleCard(string cardType)
	{
		CspUtils.DebugLog("BattleCard cardType=" + cardType);

		attackFactors = new Factor[1];
		blockFactors = new Factor[1];
		internalHeroName = new string[1];
		rulesHeroName = new string[1];
		teamName = null;
		miniTexture = null;
		isFoil = false;
		isIdentified = false;
		rarity = CardRarity.Common;
		set = CardSet.None;
		isFaceup = false;
		isKeeper = false;
		BattleCard battleCard = CardManager.ParseSingleCardData(cardType);
		if (battleCard != null)
		{
			Copy(battleCard);
		}
		fullTexture = CardManager.LoadCardTexture(cardType);
		miniTexture = fullTexture; // added by CSP for testing
	}

	public BattleCard(BattleCard ExistingCard)
	{
		Copy(ExistingCard);
	}

	public static Factor CharToFactor(char C)
	{
		switch (C)
		{
		case 'S':
			return Factor.Strength;
		case 'P':
			return Factor.Speed;
		case 'E':
			return Factor.Elemental;
		case 'T':
			return Factor.Tech;
		case 'A':
			return Factor.Animal;
		case 'N':
			return Factor.Energy;
		case 'U':
			return Factor.Universal;
		default:
			return Factor.None;
		}
	}

	public static char FactorToChar(Factor F)
	{
		switch (F)
		{
		case Factor.Strength:
			return 'S';
		case Factor.Speed:
			return 'P';
		case Factor.Elemental:
			return 'E';
		case Factor.Tech:
			return 'T';
		case Factor.Animal:
			return 'A';
		case Factor.Energy:
			return 'N';
		case Factor.Universal:
			return 'U';
		default:
			return '0';
		}
	}

	public static string FactorToString(Factor F)
	{
		switch (F)
		{
		case Factor.Strength:
			return "strength";
		case Factor.Speed:
			return "speed";
		case Factor.Elemental:
			return "elemental";
		case Factor.Tech:
			return "tech";
		case Factor.Animal:
			return "animal";
		case Factor.Energy:
			return "energy";
		case Factor.Universal:
			return "universal";
		default:
			return "unknown";
		}
	}

	public static CardRarity CharToRarity(char c)
	{
		switch (c)
		{
		case 'C':
			return CardRarity.Common;
		case 'U':
			return CardRarity.Uncommon;
		case 'R':
			return CardRarity.Rare;
		case 'S':
			return CardRarity.SuperRare;
		default:
			return CardRarity.None;
		}
	}

	public static char RarityToChar(CardRarity r)
	{
		switch (r)
		{
		case CardRarity.Common:
			return 'C';
		case CardRarity.Uncommon:
			return 'U';
		case CardRarity.Rare:
			return 'R';
		case CardRarity.SuperRare:
			return 'S';
		default:
			return '0';
		}
	}

	public static int RarityToScore(CardRarity r)
	{
		switch (r)
		{
		case CardRarity.Common:
			return 0;
		case CardRarity.Uncommon:
			return 0;
		case CardRarity.Rare:
			return 1;
		case CardRarity.SuperRare:
			return 1;
		default:
			return 1;
		}
	}

	public static CardSet CharToCardSet(char c)
	{
		switch (c)
		{
		case 'S':
			return CardSet.Silver;
		case 'G':
			return CardSet.Gold;
		case 'Q':
			return CardSet.Quest;
		case 'A':
			return CardSet.Advanced;
		default:
			return CardSet.None;
		}
	}

	public static char CardSetToChar(CardSet r)
	{
		switch (r)
		{
		case CardSet.Silver:
			return 'S';
		case CardSet.Gold:
			return 'G';
		case CardSet.Quest:
			return 'Q';
		case CardSet.Advanced:
			return 'A';
		default:
			return '0';
		}
	}

	public void Copy(BattleCard ExistingCard)
	{
		type = ExistingCard.type;
		name = ExistingCard.name;
		nameEng = ExistingCard.nameEng;
		heroName = ExistingCard.heroName;
		heroNameEng = ExistingCard.heroNameEng;
		teamName = ExistingCard.teamName;
		abilityText = ExistingCard.abilityText;
		abilityTextEng = ExistingCard.abilityTextEng;
		affinityText = ExistingCard.affinityText;
		flavorText = ExistingCard.flavorText;
		flavorTextEng = ExistingCard.flavorTextEng;
		level = ExistingCard.level;
		serverID = ExistingCard.ServerID;
		damage = ExistingCard.damage;
		instance = ExistingCard.instance;
		isIdentified = false;
		isMoving = false;
		rarity = ExistingCard.rarity;
		set = ExistingCard.set;
		isFoil = ExistingCard.isFoil;
		internalHeroName = ExistingCard.internalHeroName;
		attackPattern = ExistingCard.attackPattern;
		keeperPattern = ExistingCard.keeperPattern;
		fullTexture = ExistingCard.fullTexture;
		miniTexture = ExistingCard.miniTexture;
		AttackFactors = ExistingCard.AttackFactors;
		BlockFactors = ExistingCard.BlockFactors;
		ruleScriptText = ExistingCard.RuleScriptText;
		bundleName = ExistingCard.bundleName;
		isFaceup = ExistingCard.Faceup;
		isKeeper = ExistingCard.IsKeeper;
		preventKO = ExistingCard.preventKO;
		internalHeroName = (string[])ExistingCard.internalHeroName.Clone();
		rulesHeroName = (string[])ExistingCard.rulesHeroName.Clone();
	}

	public bool ParseTypeXml(string CardXML)
	{
		DataWarehouse dataWarehouse = new DataWarehouse(CardXML);
		dataWarehouse.Parse();
		return ParseTypeXml(dataWarehouse);
	}

	public bool ParseTypeXml(DataWarehouse CardData)
	{
		//Discarded unreachable code: IL_0352, IL_036f
		try
		{
			type = CardData.GetString("Cards.Card.ID");
			name = CardData.GetString("Cards.Card.Subname");
			nameEng = CardData.GetString("Cards.Card.SubnameEng");
			heroName = CardData.GetString("Cards.Card.HeroName");
			heroNameEng = CardData.GetString("Cards.Card.HeroNameEng");
			teamName = CardData.GetString("Cards.Card.TeamName");
			abilityText = CardData.GetString("Cards.Card.AbilityText");
			abilityTextEng = CardData.GetString("Cards.Card.AbilityTextEng");
			affinityText = CardData.GetString("Cards.Card.Affinity");
			flavorText = CardData.GetString("Cards.Card.FlavorText");
			flavorTextEng = CardData.GetString("Cards.Card.FlavorTextEng");
			ruleScriptText = CardData.GetString("Cards.Card.Rules");
			level = CardData.TryGetInt("Cards.Card.Level", 0);
			damage = CardData.TryGetInt("Cards.Card.Damage", 0);
			rarity = CharToRarity(CardData.TryGetString("Cards.Card.Rarity", "0")[0]);
			set = CharToCardSet(CardData.TryGetString("Cards.Card.Set", "0")[0]);
			bundleName = CardData.TryGetString("Cards.Card.Bundle", "cards01");
			string @string = CardData.GetString("Cards.Card.InternalHeroName");
			bool delaySecondCharacterSpawn = false;
			if (@string.Contains("*"))
			{
				delaySecondCharacterSpawn = true;
				internalHeroName = @string.Split('*');
			}
			else
			{
				internalHeroName = @string.Split(';');
			}
			rulesHeroName = CardData.GetString("Cards.Card.RulesHeroName").Split(';');
			attackPattern = new SquadBattleAttackPattern();
			attackPattern.AttackSequenceString = CardData.TryGetString("Cards.Card.AttackChoreography", "L1");
			attackPattern.RepeatSequence = CardData.TryGetBool("Cards.Card.RepeatAttackChain", true);
			attackPattern.ZeroDamageEmote = CardData.TryGetString("Cards.Card.ZeroDamageEmote", "emote_cheer");
			attackPattern.DelaySecondCharacterSpawn = delaySecondCharacterSpawn;
			keeperPattern = new SquadBattleAttackPattern();
			keeperPattern.AttackSequenceString = CardData.TryGetString("Cards.Card.KeeperChoreography", "L1");
			keeperPattern.RepeatSequence = CardData.TryGetBool("Cards.Card.RepeatKeeperChain", true);
			keeperPattern.ZeroDamageEmote = CardData.TryGetString("Cards.Card.KeeperZeroDamageEmote", "emote_cheer");
			keeperPattern.DelaySecondCharacterSpawn = delaySecondCharacterSpawn;
			string string2 = CardData.GetString("Cards.Card.AttackFactors");
			attackFactors = new Factor[string2.Length];
			for (int i = 0; i < string2.Length; i++)
			{
				attackFactors[i] = CharToFactor(string2[i]);
			}
			string string3 = CardData.GetString("Cards.Card.BlockingFactors");
			blockFactors = new Factor[string3.Length];
			for (int j = 0; j < string3.Length; j++)
			{
				blockFactors[j] = CharToFactor(string3[j]);
			}
			preventKO = ruleScriptText.Contains("#PreventKO");
			isKeeper = ruleScriptText.Contains("keeper = 'true'");
			return true;
		}
		catch (XmlException)
		{
			CspUtils.DebugLog("BattleCard encountered an XmlException while parsing CardData");
			return false;
		}
	}

	public Factor[] GetAttackFactors()
	{
		return attackFactors;
	}

	public Factor[] GetBlockFactors()
	{
		return blockFactors;
	}

	public virtual int CompareTo(object Obj)
	{
		if (Obj is BattleCard)
		{
			BattleCard battleCard = (BattleCard)Obj;
			if (battleCard.Level == level)
			{
				return battleCard.Damage.CompareTo(damage);
			}
			return level.CompareTo(battleCard.Level);
		}
		throw new ArgumentException("Obj is not a BattleCard");
	}

	public void SetSelectable()
	{
		SetColor(Color.white);
	}

	public void SetUnselectable()
	{
		SetColor(Color.grey);
	}

	public void SetColor(Color newColor)
	{
		Renderer[] array = rendererComponents;
		foreach (Renderer renderer in array)
		{
			renderer.materials[renderer.materials.Length - 1].color = newColor;
		}
	}

	public void SetRevealTimestamp()
	{
		revealTimestamp = Time.time + 5f;
	}

	public bool IsRevealComplete()
	{
		return revealTimestamp < Time.time;
	}
}

public static class ObjectExtensions
{
	public static void Dump(this object obj, TextWriter writer)
	{
		if (obj == null)
		{
			CspUtils.DebugLog("Object is null");
			return;
		}

		CspUtils.DebugLog("Hash: " + obj.GetHashCode());
		CspUtils.DebugLog("Type: " + obj.GetType());

		var props = GetProperties(obj);

		if (props.Count > 0)
		{
			CspUtils.DebugLog("-------------------------");
		}

		foreach (var prop in props)
		{
			CspUtils.DebugLog(prop.Key + ": " + prop.Value);
		}
	}

	private static Dictionary<string, string> GetProperties(object obj)
	{
		var props = new Dictionary<string, string>();
		if (obj == null)
			return props;

		var type = obj.GetType();
		foreach (var prop in type.GetProperties())
		{
			var val = prop.GetValue(obj, new object[] { });
			var valStr = val == null ? "" : val.ToString();
			props.Add(prop.Name, valStr);
		}

		return props;
	}
}
