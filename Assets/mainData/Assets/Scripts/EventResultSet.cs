using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;

public class EventResultSet : Queue<EventResultBase>
{
	public static string EVENT_RESULT_SET_TEST_DATA = "<event_results>\r\n              <stage_event>\r\n                <userid>3</userid>\r\n                <hero>iron_man</hero>\r\n                <event>Stage Complete</event>\r\n                <mission_id> m_0001_1_SampleMission</mission_id>\r\n                <stage_id>3</stage_id>\r\n                <player_score>\r\n                  <userid>3</userid>\r\n                  <attack_combo>450</attack_combo>\r\n                  <pickups>30</pickups>\r\n                  <enemy_defeats>400</enemy_defeats>\r\n                  <survivor_bonus>1000</survivor_bonus>\r\n                  <score>1880</score>\r\n                </player_score>\r\n                <player_score>\r\n                  <userid>13</userid>\r\n                  <attack_combo>450</attack_combo>\r\n                  <pickups>10</pickups>\r\n                  <enemy_defeats>800</enemy_defeats>\r\n                  <survivor_bonus>1000</survivor_bonus>\r\n                  <score>2260</score>\r\n                </player_score>\r\n                <player_score>\r\n                  <userid>44</userid>\r\n                  <attack_combo>200</attack_combo>\r\n                  <pickups>60</pickups>\r\n                  <enemy_defeats>400</enemy_defeats>\r\n                  <survivor_bonus>4000</survivor_bonus>\r\n                  <score>4660</score>\r\n                </player_score>\r\n                <battle_bonus>2300</battle_bonus >\r\n                <speedy_bonus>1500</speedy_bonus>\r\n                <team_score>8800</team_score>\r\n                <rating>g</rating>\r\n                <coins>100</coins>\r\n                <tickets>2</tickets>\r\n                <xp>100</xp>\r\n              </stage_event>\r\n              <mission_event>\r\n                <userid>3</userid>\r\n                <hero>iron_man</hero>\r\n                <event>Mission Complete</event>\r\n                <mission_id> m_0001_1_SampleMission</mission_id>\r\n                <stage_scores>\r\n                  <stage>\r\n                    <stage_num>1</stage_num>\r\n                    <stage_score>4000</stage_score>\r\n                  </stage>\r\n                  <stage>\r\n                    <stage_num>2</stage_num>\r\n                    <stage_score>3200</stage_score>\r\n                  </stage>\r\n                  <stage>\r\n                    <stage_num>1</stage_num>\r\n                    <stage_score>8800</stage_score>\r\n                  </stage>\r\n                </stage_scores>\r\n                <mission_score>16000</mission_score>\r\n                <rating>g</rating>\r\n              </mission_event>\r\n              <hero_level_up>\r\n                <userid>3</userid>\r\n                <event>Stage Complete</event>\r\n                <hero>\r\n                  <name>iron_man</name>\r\n                  <level>2</level>\r\n                  <xp>520</xp>\r\n                </hero>\r\n              </hero_level_up>\r\n              <hero_level_up>\r\n                <userid>44</userid>\r\n                <event>Stage Complete</event>\r\n                <hero>\r\n                  <name>storm</name>\r\n                  <level>2</level>\r\n                  <xp>600</xp>\r\n                </hero>\r\n              </hero_level_up>\r\n            <squad_battle>\r\n\t            <type>pvp</type>\r\n\t            <time>350</time>\r\n\t            <player>\r\n\t\t            <userid>3</userid>\r\n\t\t            <hero>iron_man</hero>\r\n\t\t            <status>W</status>\r\n\t\t            <cards_left>15</cards_left>\r\n\t            </player>\r\n\t            <player>\r\n\t\t            <userid>4</userid>\r\n\t\t            <hero>storm</hero>\r\n\t\t            <status>L</status>\r\n\t\t            <cards_left>0</cards_left>\r\n\t            </player>\r\n                <coins>100</coins>\r\n                <tickets>2</tickets>\r\n            </squad_battle>\r\n          </event_results>";

	public void Enqueue(string eventResultsXml)
	{
		DataWarehouse dataWarehouse = new DataWarehouse(eventResultsXml);
		dataWarehouse.Parse();
		Enqueue(dataWarehouse);
	}

	public void Enqueue(DataWarehouse eventResultsData)
	{
		XPathNavigator value = eventResultsData.GetValue("//event_results");
		if (!value.MoveToFirstChild())
		{
			CspUtils.DebugLog("Asked to process event results, but the data was empty!");
			return;
		}
		do
		{
			CspUtils.DebugLog("Processing event result of type <" + value.LocalName + ">.");
			EventResultBase eventResultBase = NewEventResultFromData(value);
			if (eventResultBase != null)
			{
				Enqueue(eventResultBase);
			}
		}
		while (value.MoveToNext());
	}

	public bool Enqueue(Hashtable msg)
	{
		EventResultBase eventResultBase = NewEventResultFromData(msg);
		if (eventResultBase != null)
		{
			Enqueue(eventResultBase);
			return true;
		}
		return false;
	}

	public EventResultBase NewEventResultFromData(Hashtable msg)
	{
		EventResultBase eventResultBase = null;
		string text = (string)msg["message_type"];
		CspUtils.DebugLog("NewEventResultFromData: " + text);
		switch (text)
		{
		case "brawler_scoring":
			eventResultBase = new EventResultMissionEvent();
			break;
		case "hero_level_up":
			eventResultBase = new EventResultHeroLevelUp();
			break;
		case "card_results":
			eventResultBase = new EventResultSquadBattle();
			break;
		}
		if (eventResultBase != null)
		{
			eventResultBase.InitializeFromData(msg);
		}
		return eventResultBase;
	}

	protected EventResultBase NewEventResultFromData(XPathNavigator navigator)
	{
		EventResultBase result = null;
		switch (navigator.LocalName)
		{
		case "mission_event":
			result = new EventResultMissionEvent();
			break;
		case "hero_level_up":
			result = new EventResultHeroLevelUp();
			break;
		case "squad_battle":
			result = new EventResultSquadBattle();
			break;
		default:
			CspUtils.DebugLog("Asked to process unknown event result type <" + navigator.LocalName + ">.");
			return result;
		}
		result.InitializeFromData(new DataWarehouse(navigator));
		return result;
	}
}
