using System.Collections.Generic;

public class OwnableSet
{
	public int ownableTypeID;

	public int quantity = 1;

	public OwnableSet(int ownableTypeID, int quantity)
	{
		this.ownableTypeID = ownableTypeID;
		this.quantity = quantity;
	}

	public static List<OwnableSet> parseFromString(string ownables)
	{
		List<OwnableSet> list = new List<OwnableSet>();
		string[] array = ownables.Split('|');
		string[] array2 = array;
		foreach (string text in array2)
		{
			string[] array3 = text.Split(':');
			list.Add(new OwnableSet(int.Parse(array3[0]), int.Parse(array3[1])));
		}
		return list;
	}
}
