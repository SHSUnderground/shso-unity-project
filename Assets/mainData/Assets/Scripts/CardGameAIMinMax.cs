using CardGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGameAIMinMax : CardGameAI
{
	public CardGameAIMinMax()
	{
	}

	public CardGameAIMinMax(CardGameAIPlayer _player)
	{
		SetPlayer(_player);
	}

	public override IEnumerator ThinkPickCard(List<int> cards, bool canpass, PickCardType pickCardType, int opposingCard)
	{
		BattleCard card = null;
		BattleCard biggest = null;
		if (pickCardType != PickCardType.Discard)
		{
			yield return new WaitForSeconds(CardGameAI.kDefaultThinkTime);
		}
		if (pickCardType != PickCardType.Done)
		{
			foreach (int x in cards)
			{
				if (player.Hand.ContainsKey(x))
				{
					card = player.Hand.Lookup(x);
					if (biggest == null || ((pickCardType != 0) ? (card.Level < biggest.Level) : (card.Level > biggest.Level)))
					{
						biggest = card;
					}
				}
			}
		}
		if (card != null)
		{
			player.SendPickCard(card, false, pickCardType);
			yield break;
		}
		if (!canpass)
		{
			CspUtils.DebugLog("AI failed to find a valid card");
		}
		player.SendPickCard(null, true, pickCardType);
	}
}
