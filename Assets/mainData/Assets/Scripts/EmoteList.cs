using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EmoteList
{
	public enum SelectionType
	{
		Sequential,
		Random
	}

	[SerializeField]
	private List<string> emotes;

	[SerializeField]
	private SelectionType selectType;

	private int nextEmoteIndex;

	public List<string> Emotes
	{
		get
		{
			return emotes;
		}
		set
		{
			emotes = value;
		}
	}

	public SelectionType SelectType
	{
		get
		{
			return selectType;
		}
		set
		{
			selectType = value;
		}
	}

	public EmoteList(IEnumerable<string> emotes, SelectionType selectionType)
	{
		Emotes = new List<string>(emotes);
		SelectType = selectionType;
	}

	public string GetNextEmote()
	{
		if (SelectType == SelectionType.Sequential)
		{
			string result = Emotes[nextEmoteIndex++];
			if (nextEmoteIndex == Emotes.Count)
			{
				nextEmoteIndex = 0;
			}
			return result;
		}
		return Emotes[UnityEngine.Random.Range(0, Emotes.Count)];
	}
}
