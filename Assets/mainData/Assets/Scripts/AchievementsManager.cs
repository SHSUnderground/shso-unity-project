using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using System.Xml.XPath;

public class AchievementsManager : StaticDataDefinition, IStaticDataDefinition
{
	private Dictionary<string, IAchievement> achievements;

	private List<string> counterCache;

	public Dictionary<string, IAchievement> Achievements
	{
		get
		{
			return achievements;
		}
	}

	public AchievementsManager()
	{
		achievements = new Dictionary<string, IAchievement>();
		counterCache = new List<string>();
	}

	public void InitializeFromData(DataWarehouse config)
	{
		XPathNavigator value = config.GetValue("achievements");
		XPathNodeIterator xPathNodeIterator = value.SelectChildren("achievement", string.Empty);
		if (xPathNodeIterator != null)
		{
			while (xPathNodeIterator.MoveNext())
			{
				string value2 = xPathNodeIterator.Current.SelectSingleNode("id").Value;
				string value3 = xPathNodeIterator.Current.SelectSingleNode("class").Value;
				if (value3 == null)
				{
					CspUtils.DebugLog("Unable to load achievement: " + value2);
					continue;
				}
				if (value3 == null)
				{
					CspUtils.DebugLog("Can't get type: " + value3 + " from executing assembly.");
					continue;
				}
				Assembly executingAssembly = Assembly.GetExecutingAssembly();
				Type type = executingAssembly.GetType(value3);
				if (!typeof(IAchievement).IsAssignableFrom(type))
				{
					CspUtils.DebugLog("Type: " + value3.ToString() + " does not implement the ISHSActivity interface.");
					continue;
				}
				XmlSerializer xmlSerializer = new XmlSerializer(type);
				string outerXml = xPathNodeIterator.Current.OuterXml;
				IAchievement achievement = xmlSerializer.Deserialize(new StringReader(outerXml)) as IAchievement;
				if (achievement != null)
				{
					AddAchievement(achievement);
				}
			}
		}
		configureListeners();
		resetAchievements(GameController.GetController().LocalCharacter);
	}

	public void AddAchievement(IAchievement achievement)
	{
		if (achievements.ContainsKey(achievement.Id))
		{
			CspUtils.DebugLog("Duplicate achievement detected: " + achievement.Id);
			return;
		}
		achievements[achievement.Id] = achievement;
		achievement.Manager = this;
	}

	private void configureListeners()
	{
		AppShell.Instance.EventMgr.AddListener<CounterUpdatedMessage>(OnCounterUpdated);
		AppShell.Instance.EventMgr.AddListener<LocalPlayerChangedMessage>(OnCharacterChanged);
	}

	private void unregisterListeners()
	{
		AppShell.Instance.EventMgr.RemoveListener<LocalPlayerChangedMessage>(OnCharacterChanged);
		AppShell.Instance.EventMgr.RemoveListener<CounterUpdatedMessage>(OnCounterUpdated);
	}

	private void OnCounterUpdated(CounterUpdatedMessage message)
	{
		bool flag = false;
		foreach (string item in counterCache)
		{
			if (message.Path.ToUpper().Contains(item.ToUpper()))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			processCounterUpdate(message);
		}
	}

	private void processCounterUpdate(CounterUpdatedMessage message)
	{
		foreach (Achievement value in achievements.Values)
		{
			if (value.Source == Achievement.AchievementSourceEnum.Counter)
			{
				value.ProcessSourceUpdate(message);
			}
		}
	}

	private void OnCharacterChanged(LocalPlayerChangedMessage message)
	{
		resetAchievements(GameController.GetController().LocalCharacter);
	}

	private void resetAchievements(string currentCharacter)
	{
		foreach (IAchievement value in achievements.Values)
		{
			value.Reset();
			value.Initialize(currentCharacter);
		}
	}

	public void RegisterCounterNotification(string[] counters)
	{
		counterCache.AddRange(Array.FindAll(counters, delegate(string stringIn)
		{
			return !counterCache.Contains(stringIn);
		}));
	}
}
