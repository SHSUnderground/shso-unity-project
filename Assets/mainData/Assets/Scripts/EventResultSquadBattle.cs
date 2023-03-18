using CardGame;
using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;

public class EventResultSquadBattle : EventResultBase
{
	protected string matchType;

	protected int time;

	protected int cards_left;

	protected int coins;

	protected int tickets;

	protected List<PlayerData> players;

	public string Type
	{
		get
		{
			return matchType;
		}
	}

	public int Time
	{
		get
		{
			return time;
		}
	}

	public int CardsLeft
	{
		get
		{
			return cards_left;
		}
	}

	public int Coins
	{
		get
		{
			return coins;
		}
	}

	public int Tickets
	{
		get
		{
			return tickets;
		}
	}

	public List<PlayerData> Players
	{
		get
		{
			return players;
		}
	}

	public EventResultSquadBattle()
		: base(EventResultType.SquadBattle)
	{
	}

	public override void InitializeFromData(DataWarehouse data)
	{
		matchType = data.GetString("type");
		time = data.GetInt("time");
		cards_left = data.GetInt("cards_left");
		players = new List<PlayerData>(2);
		XPathNodeIterator values = data.GetValues("player");
		foreach (XPathNavigator item in values)
		{
			PlayerData playerData = new PlayerData();
			playerData.UserId = item.SelectSingleNode("userid").ValueAsInt;
			playerData.Hero = item.SelectSingleNode("hero").Value;
			playerData.Status = item.SelectSingleNode("status").Value;
			players.Add(playerData);
		}
		coins = data.GetInt("coins");
		tickets = data.GetInt("tickets");
	}

	public override void InitializeFromData(Hashtable data)
	{
		matchType = (string)data["type"];
		time = int.Parse((string)data["time"]);
		cards_left = int.Parse((string)data["cards_left"]);
		coins = int.Parse((string)data["tokens_won"]);
		tickets = int.Parse((string)data["tickets_won"]);
		players = new List<PlayerData>(2);
		PlayerData playerData = new PlayerData();
		playerData.UserId = int.Parse((string)data["playerid_a"]);
		playerData.Hero = (string)data["hero_a"];
		playerData.Status = (string)data["status_a"];
		players.Add(playerData);
		PlayerData playerData2 = new PlayerData();
		playerData2.UserId = int.Parse((string)data["playerid_b"]);
		playerData2.Hero = (string)data["hero_b"];
		playerData2.Status = (string)data["status_b"];
		players.Add(playerData2);
	}
}
