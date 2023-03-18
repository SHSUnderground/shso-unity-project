using CardGame;
using System.Collections.Generic;
using UnityEngine;

public class AutomationCardGame
{
	public CardGamePlayer player;

	public BattleCard biggest;

	public AutomationCardGame()
	{
	}

	public AutomationCardGame(CardGamePlayer p)
	{
		player = p;
		biggest = null;
	}

	public void StartPlayerAI(List<int> cards, bool canpass, PickCardType pickCardType, int playType)
	{
		switch (playType)
		{
		case 1:
			RunAIRandom(cards, canpass, pickCardType);
			break;
		case 2:
			RunAIMinMax(cards, canpass, pickCardType);
			break;
		case 3:
			RunAIPassive(cards, canpass, pickCardType);
			break;
		}
	}

	public void RunAIRandom(List<int> cards, bool canpass, PickCardType pickCardType)
	{
		BattleCard battleCard = null;
		if (canpass && (pickCardType == PickCardType.Attack || Random.Range(0f, 1f) < 0.7f))
		{
			foreach (int card in cards)
			{
				CspUtils.DebugLog("Cards Leng:" + cards.Count + "id:" + card);
				if (player.Hand.ContainsKey(card))
				{
					battleCard = player.Hand.Lookup(card);
					if (battleCard.Level <= CardGameController.Instance.powerLevel)
					{
						player.SendPickCard(battleCard, false, pickCardType);
						CspUtils.DebugLog("Choosing to Play " + battleCard.NameEng);
					}
				}
			}
			if (battleCard != null)
			{
				player.SendPickCard(null, true, pickCardType);
				CspUtils.DebugLog("Choosing to Pass!");
			}
		}
	}

	public void RunAIPassive(List<int> cards, bool canpass, PickCardType pickCardType)
	{
		BattleCard battleCard = null;
		if (canpass)
		{
			player.SendPickCard(null, true, pickCardType);
			return;
		}
		int iD = cards[Random.Range(0, cards.Count - 1)];
		if (player.Hand.ContainsKey(iD))
		{
			battleCard = player.Hand.Lookup(iD);
		}
		else if (player.Keepers.ContainsKey(iD))
		{
			battleCard = player.Keepers.Lookup(iD);
		}
		else if (player.Stock.ContainsKey(iD))
		{
			battleCard = player.Stock.Lookup(iD);
		}
		if (battleCard == null)
		{
			CspUtils.DebugLog("AI failed to find a valid card");
		}
		player.SendPickCard(battleCard, false, pickCardType);
	}

	public void RunAIMinMax(List<int> cards, bool canpass, PickCardType pickCardType)
	{
		BattleCard battleCard = null;
		BattleCard battleCard2 = null;
		foreach (int card in cards)
		{
			if (player.Hand.ContainsKey(card))
			{
				battleCard = player.Hand.Lookup(card);
				if (battleCard2 == null)
				{
					battleCard2 = battleCard;
				}
				else if (battleCard2.Level > battleCard.Level)
				{
					battleCard = battleCard2;
				}
				else
				{
					battleCard2 = battleCard;
				}
			}
		}
		if (battleCard != null)
		{
			CspUtils.DebugLog("Level: " + battleCard.Level);
			player.SendPickCard(battleCard, false, pickCardType);
		}
		else
		{
			player.SendPickCard(null, true, pickCardType);
		}
	}
}
