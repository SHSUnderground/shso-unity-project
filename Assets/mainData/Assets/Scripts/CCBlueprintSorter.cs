using System.Collections.Generic;

public class CCBlueprintSorter : IComparer<Blueprint>
{
	public int Compare(Blueprint x, Blueprint y)
	{
		if (x == null)
		{
			if (y == null)
			{
				return 0;
			}
			return -1;
		}
		if (y == null)
		{
			return 1;
		}
		if (x.displayPriority < y.displayPriority)
		{
			return 1;
		}
		if (x.displayPriority > y.displayPriority)
		{
			return -1;
		}
		if (x.id < y.id)
		{
			return -1;
		}
		return 1;
	}
}
