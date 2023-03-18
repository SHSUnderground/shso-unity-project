using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Serialization;
using System.Xml.XPath;
using UnityEngine;

public class EmotesDefinition : StaticDataDefinition, IStaticDataDefinition
{
	[XmlRoot(ElementName = "emote")]
	public class EmoteDefinition
	{
		public class Key
		{
			public KeyCode keyCode;

			public bool control;

			public bool alt;

			public bool shift;
		}

		public class Requirement
		{
			public string type;

			public string criteria;

			public string info;
		}

		[XmlElement(ElementName = "id")]
		public sbyte id;

		[XmlElement(ElementName = "command")]
		public string command;

		[XmlElement(ElementName = "sequence")]
		public string sequenceName;

		[DefaultValue(false)]
		[XmlElement(ElementName = "logical_seq")]
		public bool isLogicalSequence;

		[DefaultValue(false)]
		[XmlElement(ElementName = "looping")]
		public bool isLooping;

		[XmlElement(ElementName = "key")]
		public Key key = new Key();

		[DefaultValue("")]
		[XmlElement(ElementName = "npcreaction")]
		public string npcReaction;

		[XmlElement(ElementName = "category")]
		[DefaultValue("")]
		public string category;

		[XmlElement(ElementName = "chat_phrase")]
		[DefaultValue("")]
		public string chatPhrase;

		protected Requirement[] requirements;

		[XmlArray(ElementName = "requirements")]
		[XmlArrayItem("requirement")]
		public Requirement[] Requirements
		{
			get
			{
				return requirements;
			}
			set
			{
				requirements = value;
			}
		}

		[XmlIgnore]
		public KeyCodeEntry keyCodeEntry
		{
			get
			{
				return new KeyCodeEntry(key.keyCode, key.control, key.alt, key.shift);
			}
			set
			{
				key = new Key();
				key.keyCode = value.KeyCode;
				key.control = value.Control;
				key.alt = value.Alt;
				key.shift = value.Shift;
			}
		}
	}

	public enum EmoteCategoriesEnum
	{
		Positive,
		Aggressive,
		Reactive,
		PowerEmote,
		Internal,
		BubbleEmotions,
		BubbleExclamations,
		BubbleGreetings,
		BubbleHeroic,
		BubbleInstructions,
		BubblePlay
	}

	public static EmotesDefinition Instance;

	protected Dictionary<string, EmoteDefinition> emotesByCommand;

	protected Dictionary<sbyte, EmoteDefinition> emotesById;

	protected Dictionary<EmoteCategoriesEnum, List<EmoteDefinition>> emotesByCategory;

	public EmotesDefinition()
	{
		emotesByCommand = new Dictionary<string, EmoteDefinition>();
		emotesById = new Dictionary<sbyte, EmoteDefinition>();
		emotesByCategory = new Dictionary<EmoteCategoriesEnum, List<EmoteDefinition>>();
	}

	public void InitializeFromData(DataWarehouse data)
	{
		XPathNavigator value = data.GetValue(".");
		value.MoveToFirstChild();
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(EmoteDefinition));
		XPathNodeIterator xPathNodeIterator = value.Select("//emotes/emote");
		while (xPathNodeIterator.MoveNext())
		{
			string outerXml = xPathNodeIterator.Current.OuterXml;
			EmoteDefinition emoteDefinition = xmlSerializer.Deserialize(new StringReader(outerXml)) as EmoteDefinition;
			if (emoteDefinition != null && emoteDefinition.command != null && emoteDefinition.sequenceName != null)
			{
				emotesById.Add(emoteDefinition.id, emoteDefinition);
				emotesByCommand.Add(emoteDefinition.command, emoteDefinition);
				EmoteCategoriesEnum key = (EmoteCategoriesEnum)(int)Enum.Parse(typeof(EmoteCategoriesEnum), emoteDefinition.category);
				if (!emotesByCategory.ContainsKey(key))
				{
					emotesByCategory[key] = new List<EmoteDefinition>();
				}
				emotesByCategory[key].Add(emoteDefinition);
			}
			else
			{
				XPathNavigator current = xPathNodeIterator.Current;
				XPathNodeIterator xPathNodeIterator2 = current.Select("command");
				string str = "Unknown";
				if (xPathNodeIterator2.MoveNext())
				{
					str = xPathNodeIterator2.Current.Value;
				}
				CspUtils.DebugLog("Could not deserialize EmoteDefinition in emotes.xml command:" + str);
			}
		}
	}

	public EmoteDefinition GetEmoteById(sbyte id)
	{
		EmoteDefinition value = null;
		if (!emotesById.TryGetValue(id, out value))
		{
			CspUtils.DebugLog("Asked to lookup an unknown emote ID <" + id + ">.  Returning nothing.");
		}
		return value;
	}

	public EmoteDefinition GetEmoteByCommand(string command)
	{
		EmoteDefinition value = null;
		if (!emotesByCommand.TryGetValue(command, out value))
		{
			CspUtils.DebugLog("Asked to lookup an unknown emote command <" + command + ">.  Returning nothing.");
		}
		return value;
	}

	public List<EmoteDefinition> GetEmotesByCategory(EmoteCategoriesEnum category)
	{
		if (emotesByCategory.ContainsKey(category))
		{
			return emotesByCategory[category];
		}
		return null;
	}

	public IEnumerator GetEnumerator()
	{
		return emotesByCommand.Values.GetEnumerator();
	}

	public bool RequirementsCheck(sbyte id, GameObject characterObject, out string failReason)
	{
		failReason = string.Empty;
		if (AppShell.Instance.Profile == null)
		{
			return true;
		}
		HeroPersisted value;
		if (!AppShell.Instance.Profile.AvailableCostumes.TryGetValue(characterObject.name, out value))
		{
			return false;
		}
		int num = value.Level;
		PlayerCombatController playerCombatController = characterObject.GetComponent(typeof(PlayerCombatController)) as PlayerCombatController;
		if (playerCombatController != null)
		{
			num = Math.Max(playerCombatController.characterLevel, num);
		}
		return requirementsCheck(id, num, out failReason);
	}

	public bool RequirementsCheck(sbyte id, string characterName, out string failReason)
	{
		return RequirementsCheck(id, characterName, AppShell.Instance.Profile, out failReason);
	}

	public bool RequirementsCheck(sbyte id, string characterName, UserProfile profile, out string failReason)
	{
		failReason = string.Empty;
		if (profile == null || profile.AvailableCostumes == null || profile.Offline)
		{
			return true;
		}
		HeroPersisted value;
		if (!profile.AvailableCostumes.TryGetValue(characterName, out value))
		{
			return false;
		}
		return requirementsCheck(id, value.Level, out failReason);
	}

	private bool requirementsCheck(sbyte id, int charLevel, out string failReason)
	{
		failReason = string.Empty;
		EmoteDefinition emoteById = GetEmoteById(id);
		if (emoteById == null)
		{
			return false;
		}
		if (emoteById.Requirements == null)
		{
			return true;
		}
		bool result = true;
		string text = null;
		EmoteDefinition.Requirement[] requirements = emoteById.Requirements;
		foreach (EmoteDefinition.Requirement requirement in requirements)
		{
			text = requirement.criteria;
			switch (requirement.type)
			{
			case "Level":
			{
				int result2 = -1;
				if (!int.TryParse(text, out result2))
				{
					if (!(text == "MAX"))
					{
						result = false;
						break;
					}
					result2 = XpToLevelDefinition.Instance.MaxLevel;
				}
				if (result2 > charLevel)
				{
					result = false;
					failReason = requirement.info;
				}
				break;
			}
			default:
				CspUtils.DebugLog("Unknown Requirement type specified for emote.");
				result = false;
				break;
			}
		}
		return result;
	}
}
