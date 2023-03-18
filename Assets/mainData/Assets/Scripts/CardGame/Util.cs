using System;

namespace CardGame
{
	internal class Util
	{
		public static string[] ArenaList = new string[12]
		{
			"asteroid_m",
			"factory",
			"rooftops",
			"sewers",
			"skrull_ship",
			"villainville",
			"museum",
			"training",
			"subway",
			"asgard",
			"fire",
			"frost"
		};

		public static string GetRandomArena()
		{
			Random random = new Random();
			return ArenaList[random.Next(ArenaList.Length)];
		}

		public static string GetArenaName(int index)
		{
			return ArenaList[index];
		}

		public static string GetPrefabHeroName(string hero)
		{
			hero = hero.Trim().ToLower();
			switch (hero)
			{
			case "iron man":
			case "ironman":
				return "iron_man";
			case "iron_man":
			case "hulk":
			case "storm":
			case "wolverine":
				return hero;
			default:
				return hero;
			}
		}
	}
}
