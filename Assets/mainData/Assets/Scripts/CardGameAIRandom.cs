using CardGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGameAIRandom : CardGameAI
{
	public CardGameAIRandom()
	{
	}

	public CardGameAIRandom(CardGameAIPlayer _player)
	{
		SetPlayer(_player);
	}

	public override IEnumerator ThinkPickCard(List<int> cards, bool canpass, PickCardType pickCardType, int opposingCard)
	{
		BattleCard card2 = null;
		if (pickCardType != PickCardType.Discard)
		{
			yield return new WaitForSeconds(CardGameAI.kDefaultThinkTime);
		}
		List<int> refinedList = new List<int>(cards);
		if (pickCardType == PickCardType.Destroy)
		{
			bool opponentChoice = false;
			foreach (int cardIdx2 in cards)
			{
				if (player.opponent.FindCardByIndex(cardIdx2) != null)
				{
					opponentChoice = true;
					break;
				}
			}
			if (opponentChoice)
			{
				foreach (int cardIdx in cards)
				{
					if (player.FindCardByIndex(cardIdx) != null)
					{
						refinedList.Remove(cardIdx);
					}
				}
			}
			else if (cards.Count > 1 && refinedList.Contains(opposingCard))
			{
				refinedList.Remove(opposingCard);
			}
		}

		//if (cards.Count > 0 && (!canpass || pickCardType != PickCardType.Block || Random.Range(0f, 1f) < 0.7f) && pickCardType != PickCardType.Done)
		if ((cards.Count > 0) &&  (pickCardType != PickCardType.Done))  // CSP changed above line for testing
		{
			int c = refinedList[Random.Range(0, refinedList.Count - 1)];
			card2 = player.FindCardByIndex(c);
			if (card2 == null)
			{
				card2 = player.opponent.FindCardByIndex(c);
			}
			if (card2 == null)
			{
				CspUtils.DebugLog("AI failed to find a valid card");
			}

			////////////// this block added by CSP ////////////////////////
			if (pickCardType == PickCardType.Block) {
				card2 = player.FindCardByIndex(cards[0]);
				CspUtils.DebugLog("Chosen as block card for AI: " + card2.ServerID);
			}
			///////////////////////////////////////////////////////

			player.SendPickCard(card2, false, pickCardType);
		}
		else  // PASS
		{
			player.SendPickCard(null, true, pickCardType);
		}
	}
}
