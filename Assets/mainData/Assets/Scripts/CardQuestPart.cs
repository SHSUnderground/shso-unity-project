using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

public class CardQuestPart : IComparable<CardQuestPart>
{
	public class QuestBattle
	{
		[CompilerGenerated]
		private int _003CId_003Ek__BackingField;

		[CompilerGenerated]
		private int _003CStage_003Ek__BackingField;

		[CompilerGenerated]
		private string _003CName_003Ek__BackingField;

		[CompilerGenerated]
		private string _003CEnemy_003Ek__BackingField;

		[CompilerGenerated]
		private string _003CArena_003Ek__BackingField;

		[CompilerGenerated]
		private string _003CDeckList_003Ek__BackingField;

		[CompilerGenerated]
		private string _003CPrereq_003Ek__BackingField;

		[CompilerGenerated]
		private string _003CReward_003Ek__BackingField;

		[CompilerGenerated]
		private string _003CEpilogue_003Ek__BackingField;

		[CompilerGenerated]
		private string _003CHint_003Ek__BackingField;

		[CompilerGenerated]
		private string _003CHint2_003Ek__BackingField;

		[CompilerGenerated]
		private int _003CSilver_003Ek__BackingField;

		[CompilerGenerated]
		private int _003CXP_003Ek__BackingField;

		[CompilerGenerated]
		private int _003CTickets_003Ek__BackingField;

		[CompilerGenerated]
		private string _003CRules_003Ek__BackingField;

		[CompilerGenerated]
		private string _003CRulesText_003Ek__BackingField;

		[CompilerGenerated]
		private int _003CRating_003Ek__BackingField;

		[CompilerGenerated]
		private int _003CScenario_003Ek__BackingField;

		[CompilerGenerated]
		private QuestBattle _003CNextQuest_003Ek__BackingField;

		[CompilerGenerated]
		private CardQuestPart _003CParentCardQuestPart_003Ek__BackingField;

		[CompilerGenerated]
		private string _003CDescription_003Ek__BackingField;

		[CompilerGenerated]
		private int _003CTimesLost_003Ek__BackingField;

		public int Id
		{
			[CompilerGenerated]
			get
			{
				return _003CId_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CId_003Ek__BackingField = value;
			}
		}

		public int Stage
		{
			[CompilerGenerated]
			get
			{
				return _003CStage_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CStage_003Ek__BackingField = value;
			}
		}

		public string Name
		{
			[CompilerGenerated]
			get
			{
				return _003CName_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CName_003Ek__BackingField = value;
			}
		}

		public string Enemy
		{
			[CompilerGenerated]
			get
			{
				return _003CEnemy_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CEnemy_003Ek__BackingField = value;
			}
		}

		public string Arena
		{
			[CompilerGenerated]
			get
			{
				return _003CArena_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CArena_003Ek__BackingField = value;
			}
		}

		public string DeckList
		{
			[CompilerGenerated]
			get
			{
				return _003CDeckList_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CDeckList_003Ek__BackingField = value;
			}
		}

		public string Prereq
		{
			[CompilerGenerated]
			get
			{
				return _003CPrereq_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CPrereq_003Ek__BackingField = value;
			}
		}

		public string Reward
		{
			[CompilerGenerated]
			get
			{
				return _003CReward_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CReward_003Ek__BackingField = value;
			}
		}

		public string Epilogue
		{
			[CompilerGenerated]
			get
			{
				return _003CEpilogue_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CEpilogue_003Ek__BackingField = value;
			}
		}

		public string Hint
		{
			[CompilerGenerated]
			get
			{
				return _003CHint_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CHint_003Ek__BackingField = value;
			}
		}

		public string Hint2
		{
			[CompilerGenerated]
			get
			{
				return _003CHint2_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CHint2_003Ek__BackingField = value;
			}
		}

		public int Silver
		{
			[CompilerGenerated]
			get
			{
				return _003CSilver_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CSilver_003Ek__BackingField = value;
			}
		}

		public int XP
		{
			[CompilerGenerated]
			get
			{
				return _003CXP_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CXP_003Ek__BackingField = value;
			}
		}

		public int Tickets
		{
			[CompilerGenerated]
			get
			{
				return _003CTickets_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CTickets_003Ek__BackingField = value;
			}
		}

		public string Rules
		{
			[CompilerGenerated]
			get
			{
				return _003CRules_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CRules_003Ek__BackingField = value;
			}
		}

		public string RulesText
		{
			[CompilerGenerated]
			get
			{
				return _003CRulesText_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CRulesText_003Ek__BackingField = value;
			}
		}

		public int Rating
		{
			[CompilerGenerated]
			get
			{
				return _003CRating_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CRating_003Ek__BackingField = value;
			}
		}

		public int Scenario
		{
			[CompilerGenerated]
			get
			{
				return _003CScenario_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CScenario_003Ek__BackingField = value;
			}
		}

		public QuestBattle NextQuest
		{
			[CompilerGenerated]
			get
			{
				return _003CNextQuest_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CNextQuest_003Ek__BackingField = value;
			}
		}

		public CardQuestPart ParentCardQuestPart
		{
			[CompilerGenerated]
			get
			{
				return _003CParentCardQuestPart_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CParentCardQuestPart_003Ek__BackingField = value;
			}
		}

		public string Description
		{
			[CompilerGenerated]
			get
			{
				return _003CDescription_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CDescription_003Ek__BackingField = value;
			}
		}

		public int TimesLost
		{
			[CompilerGenerated]
			get
			{
				return _003CTimesLost_003Ek__BackingField;
			}
			[CompilerGenerated]
			set
			{
				_003CTimesLost_003Ek__BackingField = value;
			}
		}

		public string CurrentHint
		{
			get
			{
				if (string.IsNullOrEmpty(Hint2))
				{
					return Hint;
				}
				return (TimesLost % 2 != 0) ? Hint2 : Hint;
			}
		}

		public int NumberOfCardsWon
		{
			get
			{
				if (Reward != null && !string.IsNullOrEmpty(Reward))
				{
					string[] array = Reward.Split(';');
					if (array.Length == 1 || (array.Length > 1 && string.IsNullOrEmpty(array[1])))
					{
						string text = array[0];
						string[] array2 = text.Split(':');
						if (array2.Length == 2 && CardCollection.Collection.ContainsKey(array2[0]))
						{
							return CardCollection.Collection[array2[0]];
						}
					}
				}
				return 0;
			}
		}

		public QuestBattle(CardQuestPart questPart, CardQuestBattleJson json)
		{
			Id = json.id;
			Stage = json.stage;
			Name = json.name;
			Enemy = json.enemy;
			Prereq = json.prereq;
			Description = json.story;
			Epilogue = json.epilogue;
			Hint = json.hint;
			Hint2 = json.hint2;
			Arena = json.arena;
			if (string.IsNullOrEmpty(Arena))
			{
				Arena = "rooftops";
			}
			Reward = json.reward;
			Silver = json.silver;
			XP = json.xp;
			Tickets = json.tickets;
			Scenario = ((json.scenario <= 0) ? 1 : json.scenario);
			DeckList = json.decklist;
			Rating = json.difficulty;
			Rules = json.rules;
			RulesText = json.rules_text;
			ParentCardQuestPart = questPart;
			TimesLost = 0;
		}

		public static string ParseRulesText(string input)
		{
			string text = string.Empty;
			string[] array = input.Split(';');
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				if (string.IsNullOrEmpty(text2) || text2.Length <= 0)
				{
					continue;
				}
				string[] array3 = text2.Split(':');
				string text3 = AppShell.Instance.stringTable["#card_q_" + array3[0]];
				if (array3.Length > 1)
				{
					if (array3.Length > 2)
					{
						string newValue = array3[1];
						List<string> list = new List<string>(1);
						list.Add(array3[1]);
						Dictionary<string, BattleCard> dictionary = CardManager.ParseCardDataSet(list);
						if (dictionary.ContainsKey(array3[1]))
						{
							BattleCard battleCard = dictionary[array3[1]];
							newValue = AppShell.Instance.stringTable[battleCard.Name];
						}
						text3 = text3.Replace("%c", newValue);
						text3 = text3.Replace("%d", array3[2]);
					}
					else
					{
						text3 = text3.Replace("%d", array3[1]);
					}
				}
				text = text + text3 + "\n";
			}
			return text;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("---Quest Battle ID:" + Id + Environment.NewLine);
			stringBuilder.Append("---Quest Battle Stage:" + Stage + Environment.NewLine);
			stringBuilder.Append("---Quest Battle Name:" + Name + Environment.NewLine);
			stringBuilder.Append("---Quest Battle Enemy:" + Enemy + Environment.NewLine);
			stringBuilder.Append("---Quest Battle Description:" + Description + Environment.NewLine);
			stringBuilder.Append("---Quest Battle Arena:" + Arena + Environment.NewLine);
			return stringBuilder.ToString();
		}
	}

	public class CardQuestPartJson
	{
		public int id;

		public string gatekeeper;

		public string subname1;

		public string subname2;

		public List<CardQuestBattleJson> nodes;
	}

	public class CardQuestBattleJson
	{
		public int tickets;

		public string enemy;

		public string story;

		public string name;

		public int scenario;

		public int difficulty;

		public string ai;

		public string hint;

		public string hint2;

		public string prereq;

		public string arena;

		public int silver;

		public string rules;

		public string rules_text;

		public string start_emote;

		public int xp;

		public string reward;

		public string epilogue;

		public int id;

		public string decklist;

		public bool visible;

		public int stage;
	}

	public const int RequiredQuestBattleCount = 3;

	[CompilerGenerated]
	private int _003CId_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CSponsor_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CSubname1_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CSubname2_003Ek__BackingField;

	[CompilerGenerated]
	private CardQuestPartsTypeEnum _003CPartType_003Ek__BackingField;

	[CompilerGenerated]
	private List<QuestBattle> _003CNodes_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CIsValid_003Ek__BackingField;

	[CompilerGenerated]
	private CardQuest _003CParentQuest_003Ek__BackingField;

	public int Id
	{
		[CompilerGenerated]
		get
		{
			return _003CId_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CId_003Ek__BackingField = value;
		}
	}

	public string Sponsor
	{
		[CompilerGenerated]
		get
		{
			return _003CSponsor_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CSponsor_003Ek__BackingField = value;
		}
	}

	public string Subname1
	{
		[CompilerGenerated]
		get
		{
			return _003CSubname1_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CSubname1_003Ek__BackingField = value;
		}
	}

	public string Subname2
	{
		[CompilerGenerated]
		get
		{
			return _003CSubname2_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CSubname2_003Ek__BackingField = value;
		}
	}

	public CardQuestPartsTypeEnum PartType
	{
		[CompilerGenerated]
		get
		{
			return _003CPartType_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CPartType_003Ek__BackingField = value;
		}
	}

	public List<QuestBattle> Nodes
	{
		[CompilerGenerated]
		get
		{
			return _003CNodes_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CNodes_003Ek__BackingField = value;
		}
	}

	public bool IsValid
	{
		[CompilerGenerated]
		get
		{
			return _003CIsValid_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CIsValid_003Ek__BackingField = value;
		}
	}

	public CardQuest ParentQuest
	{
		[CompilerGenerated]
		get
		{
			return _003CParentQuest_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CParentQuest_003Ek__BackingField = value;
		}
	}

	public CardQuestPart(CardQuestPartJson partJson)
	{
		Nodes = new List<QuestBattle>();
		Sponsor = partJson.gatekeeper;
		Id = partJson.id;
		Subname1 = partJson.subname1;
		Subname2 = partJson.subname2;
		QuestBattle questBattle = null;
		partJson.nodes.Sort(delegate(CardQuestBattleJson b1, CardQuestBattleJson b2)
		{
			return b1.stage - b2.stage;
		});
		foreach (CardQuestBattleJson node in partJson.nodes)
		{
			QuestBattle questBattle2 = new QuestBattle(this, node);
			if (questBattle != null)
			{
				questBattle.NextQuest = questBattle2;
			}
			questBattle = questBattle2;
			Nodes.Add(questBattle2);
		}
		IsValid = (Nodes.Count == 3);
	}

	public int CompareTo(CardQuestPart other)
	{
		return Id - other.Id;
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Quest Part ID:" + Id + Environment.NewLine);
		foreach (QuestBattle node in Nodes)
		{
			stringBuilder.Append("=========" + Environment.NewLine);
			stringBuilder.Append(node.ToString());
		}
		return stringBuilder.ToString();
	}
}
