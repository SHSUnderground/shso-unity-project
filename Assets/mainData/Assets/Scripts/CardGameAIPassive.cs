using CardGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGameAIPassive : CardGameAI
{
	public CardGameAIPassive()
	{
	}

	public CardGameAIPassive(CardGameAIPlayer _player)
	{
		SetPlayer(_player);
	}

	public override IEnumerator ThinkPickCard(List<int> cards, bool canpass, PickCardType pickCardType, int opposingCard)
	{
		BattleCard card = null;
		if (pickCardType != PickCardType.Discard)
		{
			yield return new WaitForSeconds(CardGameAI.kDefaultThinkTime);
		}
		if (canpass)
		{
			player.SendPickCard(null, true, pickCardType);
			yield break;
		}
		int c = cards[Random.Range(0, cards.Count - 1)];
		if (player.Hand.ContainsKey(c))
		{
			card = player.Hand.Lookup(c);
		}
		else if (player.Keepers.ContainsKey(c))
		{
			card = player.Keepers.Lookup(c);
		}
		else if (player.Stock.ContainsKey(c))
		{
			card = player.Stock.Lookup(c);
		}
		if (card == null)
		{
			CspUtils.DebugLog("AI failed to find a valid card");
		}
		player.SendPickCard(card, false, pickCardType);
	}
}
