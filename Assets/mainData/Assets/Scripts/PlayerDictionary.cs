using UnityEngine;
using System.Collections.Generic;

public class PlayerDictionary : Dictionary<int, PlayerDictionary.Player>
{
	public class Player
	{
		private readonly int userId;

		private readonly string name;

		private readonly int playerId;

		private readonly bool isShieldAgent;

		private readonly bool isModerator;

		private readonly int heroLevel;

		private readonly int squadLevel;

		public int UserId
		{
			get
			{
				return userId;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public int PlayerId
		{
			get
			{
				return playerId;
			}
		}

		public bool IsShieldAgent
		{
			get
			{
				return isShieldAgent;
			}
		}

		public bool IsModerator
		{
			get
			{
				return isModerator;
			}
		}

		public int HeroLevel
		{
			get
			{
				return heroLevel;
			}
		}

		public int SquadLevel
		{
			get
			{
				return squadLevel;
			}
		}

		public Player(int userId, int playerId, string name, bool isShieldAgent, bool isModerator, int heroLevel, int squadLevel)
		{
			CspUtils.DebugLog("PlayerDictionary " + userId + " " + playerId + " " + name);
			this.userId = userId;
			this.playerId = playerId;
			this.name = name;
			this.isShieldAgent = isShieldAgent;
			this.isModerator = isModerator;
			this.heroLevel = heroLevel;
			this.squadLevel = squadLevel;
		}

		public override string ToString()
		{
			return string.Format("PlayerId {0}, {1} ({2}) HL {3} SL {4} {5}", playerId, name, userId, heroLevel, squadLevel, (!isShieldAgent) ? string.Empty : "(Shield Agent)");
		}
	}

	public void Update(Player player)
	{
		if (ContainsKey(player.UserId))
		{
			Remove(player.UserId);
		}
		Add(player.UserId, player);
		AppShell.Instance.EventMgr.Fire(this, new PlayerInfoUpdateMessage(player.UserId, player));
	}

	public int GetPlayerId(int userId)
	{
		Player value;
		if (TryGetValue(userId, out value))
		{
			return value.PlayerId;
		}
		return -1;
	}
}
