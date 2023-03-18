using LitJson;
using System;
using System.Collections.Generic;
using System.Text;

public class CardQuestManager
{
	private readonly Dictionary<int, CardQuestPart> cardQuestPartDictionary;

	private readonly Dictionary<string, CardQuest> cardQuestDictionary;

	public Dictionary<int, CardQuestPart>.ValueCollection CardQuestParts
	{
		get
		{
			return cardQuestPartDictionary.Values;
		}
	}

	public CardQuestManager()
	{
		cardQuestPartDictionary = new Dictionary<int, CardQuestPart>();
		cardQuestDictionary = new Dictionary<string, CardQuest>();
	}

	public void InitializeFromData(ShsWebResponse response)
	{
		Dictionary<string, List<CardQuestPart.CardQuestPartJson>> dictionary = JsonMapper.ToObject<Dictionary<string, List<CardQuestPart.CardQuestPartJson>>>(response.Body);
		cardQuestPartDictionary.Clear();
		cardQuestDictionary.Clear();
		foreach (CardQuestPart.CardQuestPartJson item in dictionary["card_quests"])
		{
			CardQuestPart cardQuestPart = new CardQuestPart(item);
			if (cardQuestPart.Nodes.Count > 0)
			{
				if (cardQuestPartDictionary.ContainsKey(cardQuestPart.Id))
				{
					CspUtils.DebugLog("Quest:" + cardQuestPart.Id + " already exists in the card quest dictionary.");
				}
				else
				{
					cardQuestPartDictionary[cardQuestPart.Id] = cardQuestPart;
					string sponsor = cardQuestPart.Sponsor;
					if (!cardQuestDictionary.ContainsKey(sponsor))
					{
						CardQuest value = new CardQuest(sponsor);
						cardQuestDictionary[sponsor] = value;
					}
					cardQuestDictionary[sponsor].AddQuestPart(cardQuestPart);
				}
			}
			else
			{
				CspUtils.DebugLog("Card Quest ID " + item.id + " has no battles (Quest Nodes)!");
			}
		}
	}

	public CardQuest GetQuestByPartId(int QuestPartId)
	{
		if (cardQuestPartDictionary.ContainsKey(QuestPartId))
		{
			return cardQuestPartDictionary[QuestPartId].ParentQuest;
		}
		CspUtils.DebugLog("Can't find quest by part id: " + QuestPartId);
		return null;
	}

	public CardQuestPart GetQuestPart(int QuestPartId)
	{
		if (cardQuestPartDictionary.ContainsKey(QuestPartId))
		{
			return cardQuestPartDictionary[QuestPartId];
		}
		CspUtils.DebugLog("Can't find quest by part id: " + QuestPartId);
		return null;
	}

	public void Clear()
	{
		cardQuestPartDictionary.Clear();
		cardQuestDictionary.Clear();
	}

	public override string ToString()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("----- Card Quest Manager data -----" + Environment.NewLine);
		foreach (CardQuest value in cardQuestDictionary.Values)
		{
			stringBuilder.Append(value.ToString());
		}
		return stringBuilder.ToString();
	}
}
