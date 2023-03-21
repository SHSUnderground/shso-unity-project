using LitJson;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class TheirSquad
{
	public class TheirSquadJson
	{
		public List<HeroJson> heroes;

		public int shield_play_allow;

		public string player_name;

		public int last_celebrated;

		public int current_challenge;

		public List<CounterJson> counters;
	}

	public class HeroJson
	{
		public string xp;

		public string name;
	}

	public class CounterJson
	{
		public string hero;

		public int type;

		public int n;
	}

	[CompilerGenerated]
	private bool _003CIsShieldPlayAllow_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CPlayerName_003Ek__BackingField;

	[CompilerGenerated]
	private HeroCollection _003CHeroes_003Ek__BackingField;

	[CompilerGenerated]
	private List<CounterJson> _003CCounters_003Ek__BackingField;

	[CompilerGenerated]
	private int _003CLastCelebratedChallenge_003Ek__BackingField;

	[CompilerGenerated]
	private int _003CCurrentChallenge_003Ek__BackingField;

	public bool IsShieldPlayAllow
	{
		[CompilerGenerated]
		get
		{
			return _003CIsShieldPlayAllow_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CIsShieldPlayAllow_003Ek__BackingField = value;
		}
	}

	public string PlayerName
	{
		[CompilerGenerated]
		get
		{
			return _003CPlayerName_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CPlayerName_003Ek__BackingField = value;
		}
	}

	public HeroCollection Heroes
	{
		[CompilerGenerated]
		get
		{
			return _003CHeroes_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CHeroes_003Ek__BackingField = value;
		}
	}

	public List<CounterJson> Counters
	{
		[CompilerGenerated]
		get
		{
			return _003CCounters_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CCounters_003Ek__BackingField = value;
		}
	}

	public int LastCelebratedChallenge
	{
		[CompilerGenerated]
		get
		{
			return _003CLastCelebratedChallenge_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CLastCelebratedChallenge_003Ek__BackingField = value;
		}
	}

	public int CurrentChallenge
	{
		[CompilerGenerated]
		get
		{
			return _003CCurrentChallenge_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CCurrentChallenge_003Ek__BackingField = value;
		}
	}

	private TheirSquad(TheirSquadJson json)
	{
		IsShieldPlayAllow = ((json.shield_play_allow == 1) ? true : false);
		PlayerName = json.player_name;
		LastCelebratedChallenge = json.last_celebrated;
		CurrentChallenge = json.current_challenge;
		Heroes = new HeroCollection();
		foreach (HeroJson hero in json.heroes)
		{
			HeroPersisted heroPersisted = new HeroPersisted(hero.name);
			heroPersisted.Xp = int.Parse(hero.xp);
			Heroes.Add(hero.name, heroPersisted);
		}
		Counters = json.counters;
	}

	public override string ToString()
	{
		return string.Format("{0}'s squad, IsShieldPlayAllow {1}, with {2} heroes and {3} counters.", PlayerName, IsShieldPlayAllow, Heroes.Count, Counters.Count);
	}

	public static void GetSquad(long playerId)
	{
		AppShell.Instance.WebService.StartRequest("resources$users/get_squad.py", OnGetSquadResponse);
	}

	private static void OnGetSquadResponse(ShsWebResponse response)
	{
		CspUtils.DebugLog(response.Body);
		if (response.Status == 200)
		{
			TheirSquadJson json = JsonMapper.ToObject<TheirSquadJson>(response.Body);
			TheirSquad arg = new TheirSquad(json);
			CspUtils.DebugLog("Received their squad: " + arg);
		}
		else
		{
			CspUtils.DebugLog("SERVER: Failed to get squad data for player: <" + response.Status + ">: " + response.Body);
		}
	}
}
