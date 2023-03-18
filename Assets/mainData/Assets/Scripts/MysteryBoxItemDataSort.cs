using System.Collections.Generic;

public class MysteryBoxItemDataSort : IComparer<MysteryBoxItemData>
{
	public int Compare(MysteryBoxItemData a, MysteryBoxItemData b)
	{
		if (a.rarity < b.rarity)
		{
			return 1;
		}
		if (a.rarity > b.rarity)
		{
			return -1;
		}
		return 0;
	}
}
