using System.Collections.Generic;

namespace CardGame
{
	internal class StartInfo
	{
		public int QuestID = -1;

		public int QuestNodeID = -1;

		public string QuestConditions = string.Empty;

		public string QuestKeeper = string.Empty;

		public int TicketsAwarded;

		public int XPAwarded;

		public int SilverAwarded;

		public string RewardCard = string.Empty;

		public string ArenaName = "villainville";

		public int ArenaScenario = 1;

		public Matchmaker2.Ticket RoomTicket;

		public PlayerInfo[] Players = new PlayerInfo[2];

		public StartInfo()
		{
			PlayerInfo[] players = new PlayerInfo[2];
			Init(players);
		}

		public StartInfo(PlayerInfo[] players)
		{
			Init(players);
		}

		public StartInfo(Matchmaker2.Ticket ticket)
		{
			RoomTicket = ticket;
			CspUtils.DebugLog("ticket: " + ticket.ticket);
			DataWarehouse dataWarehouse = new DataWarehouse(ticket.ticket);
			dataWarehouse.Parse();
			ArenaName = dataWarehouse.GetString("ticket/arena");
			string[] array = dataWarehouse.GetString("ticket/allowed_users").Split(',');
			List<int> list = new List<int>(2);
			list.Add(int.Parse(array[0]));
			list.Add(int.Parse(array[1]));
			bool flag = list[0] == AppShell.Instance.Profile.UserId;
			int num = (!flag) ? list[0] : list[1];
			string tagPath = (!flag) ? "ticket/hero_b" : "ticket/hero_a";
			string tagPath2 = (!flag) ? "ticket/hero_a" : "ticket/hero_b";
			PlayerInfo[] array2 = new PlayerInfo[2]
			{
				new PlayerInfo(),
				null
			};
			array2[0].DeckRecipe = dataWarehouse.GetString("ticket/my_deck");
			array2[0].DeckCode = dataWarehouse.GetString("ticket/my_deck_code");
			array2[0].Hero = dataWarehouse.GetString(tagPath);
			array2[0].HeroCode = dataWarehouse.GetString("ticket/hero_code");
			array2[0].Type = PlayerType.Human;
			array2[0].PlayerID = AppShell.Instance.Profile.UserId;
			array2[0].Name = AppShell.Instance.Profile.PlayerName;
			array2[0].ShieldAgent = AppShell.Instance.Profile.IsShieldPlayCapable;
			array2[1] = new PlayerInfo();
			array2[1].Hero = dataWarehouse.GetString(tagPath2);
			array2[1].Type = PlayerType.Network;
			array2[1].PlayerID = num;
			array2[1].Name = RTCClient.DecodeString(dataWarehouse.GetString((!flag) ? "ticket/player_a" : "ticket/player_b"));
			array2[1].ShieldAgent = dataWarehouse.GetBool((!flag) ? "ticket/player_a_shield_play_allow" : "ticket/player_b_shield_play_allow");
			array2[1].IsFriend = bool.Parse(dataWarehouse.GetString("ticket/are_friends"));
			Init(array2);
		}

		private void Init(PlayerInfo[] players)
		{
			Players = players;
			if (Players.Length < 2)
			{
				CspUtils.DebugLog("Invalid argument to StartInfo()");
				return;
			}
			if (Players[0] == null)
			{
				CspUtils.DebugLog("Using default settings for player 0");
				UserProfile profile = AppShell.Instance.Profile;
				PlayerInfo playerInfo = new PlayerInfo();
				playerInfo.Hero = "Hulk";
				playerInfo.DeckID = CardGroup.R3DemoHulkIronman;
				playerInfo.DeckRecipe = CardGroup.R4DemoHulkRecipe;
				playerInfo.Type = PlayerType.Human;
				playerInfo.PlayerID = profile.UserId;
				playerInfo.Name = profile.PlayerName;
				playerInfo.ShieldAgent = profile.IsShieldPlayCapable;
				if (playerInfo.PlayerID == -1)
				{
					CspUtils.DebugLog("Player user ID (" + playerInfo.PlayerID + ") is invalid.  Using a different ID to avoid overlapping with AI.");
					playerInfo.PlayerID = 1L;
				}
				Players[0] = playerInfo;
			}
			if (Players[1] == null)
			{
				CspUtils.DebugLog("Using default settings for player 1");
				PlayerInfo playerInfo2 = new PlayerInfo();
				playerInfo2.Hero = "Wolverine";
				playerInfo2.DeckID = CardGroup.R3DemoWolverineStorm;
				playerInfo2.DeckRecipe = CardGroup.R4DemoWolverineRecipe;
				playerInfo2.Type = PlayerType.AI;
				playerInfo2.PlayerID = -1L;
				playerInfo2.Name = AppShell.Instance.CharacterDescriptionManager[playerInfo2.Hero].CharacterName;
				Players[1] = playerInfo2;
			}
		}
	}
}
