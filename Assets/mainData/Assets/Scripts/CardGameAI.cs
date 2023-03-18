using CardGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardGameAI
{
	protected CardGameAIPlayer player;

	protected static float kDefaultThinkTime = 1.5f;

	public CardGameAI()
	{
	}

	public void SetPlayer(CardGameAIPlayer _player)
	{
		player = _player;
	}

	public IEnumerator ThinkPickFactor()
	{
		yield return new WaitForSeconds(kDefaultThinkTime);
		int factor = Random.Range(2, 8);
		player.SendPickFactor((BattleCard.Factor)factor);
	}

	public IEnumerator ThinkPickNumber(int min, int max)
	{
		yield return new WaitForSeconds(kDefaultThinkTime);
		int number = Random.Range(min, max);
		player.SendPickNumber(number);
	}

	public IEnumerator ThinkPickYesNo()
	{
		yield return new WaitForSeconds(kDefaultThinkTime);
		bool yes = false;
		if (Random.Range(0, 2) == 0)
		{
			yes = true;
		}
		player.SendPickYesNo(yes);
	}

	public abstract IEnumerator ThinkPickCard(List<int> cards, bool canpass, PickCardType pickCardType, int opposingCard);
}
