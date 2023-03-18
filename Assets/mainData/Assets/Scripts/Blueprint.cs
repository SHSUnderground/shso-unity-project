using System;
using System.Collections.Generic;

public class Blueprint
{
	public int id;

	public string name;

	public string description;

	public int outputOwnableTypeID;

	public int outputQuantity;

	public int restricted;

	public int partsOnly;

	public int displayPriority = 1;

	public List<BlueprintPiece> pieces;

	public List<int> prereqs = new List<int>();

	public Blueprint(BlueprintJsonData data)
	{
		id = data.id;
		name = data.name;
		description = data.desc;
		outputOwnableTypeID = data.output_id;
		outputQuantity = data.q;
		restricted = data.r;
		partsOnly = data.parts;
		displayPriority = data.display;
		pieces = new List<BlueprintPiece>();
		if (data.prereqs.Length <= 0)
		{
			return;
		}
		string[] array = data.prereqs.Split(',');
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text.Length > 0)
			{
				prereqs.Add(Convert.ToInt32(text));
			}
		}
	}

	public void addPiece(BlueprintPiece piece)
	{
		pieces.Add(piece);
	}
}
