using System.Collections.Generic;

public class SHSCharacterSelectionSimulation
{
	public static bool SimulationActive = false;

	public static bool AllowCharacterSelectClose = true;

	public static string SimulatedCharacter = "cyclops";

	protected static Dictionary<string, int> ExperienceChart = new Dictionary<string, int>();

	protected static List<string> Heroes = new List<string>();

	public static int GetCharacterExperience(string charName)
	{
		if (!ExperienceChart.ContainsKey(charName))
		{
			return 0;
		}
		return ExperienceChart[charName];
	}

	public static void ClearHeroList()
	{
		Heroes.Clear();
		ExperienceChart.Clear();
	}

	public static void AddSimulatedCharacter(string name, int xp)
	{
		Heroes.Add(name);
		ExperienceChart.Add(name, xp);
	}

	public static List<SHSCharacterSelect.CharacterItem> GenerateCharacterItemList(SHSCharacterSelect.HeroClickedDelegate heroClickDelegate)
	{
		List<SHSCharacterSelect.CharacterItem> list = new List<SHSCharacterSelect.CharacterItem>();
		foreach (string hero in Heroes)
		{
			HeroPersisted heroPersisted = new HeroPersisted(hero);
			heroPersisted.UpdateXp(GetCharacterExperience(hero));
			list.Add(new SHSCharacterSelect.CharacterItem(heroPersisted, heroClickDelegate, SHSSelectionItem<SHSItemLoadingWindow>.SelectionState.Active));
		}
		return list;
	}
}
