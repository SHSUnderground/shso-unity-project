using System.Collections.Generic;

public class PlayerTitleData
{
	public int titleID = -1;

	public int medallionID = -1;

	public int parentPlayerID;

	public Dictionary<string, bool> prestigeHeroes = new Dictionary<string, bool>();

	public PlayerTitleData(int titleID, int medallionID, int parentPlayerID)
	{
		this.titleID = titleID;
		this.medallionID = medallionID;
		this.parentPlayerID = parentPlayerID;
	}

	public bool checkHeroPrestige(string hero)
	{
		return prestigeHeroes.ContainsKey(hero);
	}
}
