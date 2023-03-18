using CardGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("CardGame/Private/AI Player")]
public class CardGameAIPlayer : CardGamePlayer
{
	public CardGameAI brain;

	public CardGameAIPlayer()
	{
		Deck.DeckId = CardGroup.R3DemoWolverineStorm;
		Deck.DeckRecipe = CardGroup.R4DemoWolverineRecipe;
		brain = new CardGameAIRandom(this);
	}

	public void SetBrain(CardGameAI newbrain)
	{
		CspUtils.DebugLog("SetBrain() called!");
		brain = newbrain;
		newbrain.SetPlayer(this);
	}

	public override void OnPickCard(List<int> cards, bool canpass, PickCardType pickCardType, int opposingCard, int passButtonId)
	{
		base.OnPickCard(cards, canpass, pickCardType, opposingCard, passButtonId);
		if (pickCardType == PickCardType.Block)
		{
			BattleCard card;
			CardPile loc;
			FindCardAndPile(opposingCard, out card, out loc);
			if (loc == Keepers)
			{
				QueueAnimation("AI Passing", delegate
				{
					StartCoroutine(PauseAndPassBlock());
				}, true);
				return;
			}
		}
		QueueAnimation("Fake Thinking (Pick card)", delegate
		{
			StartCoroutine(brain.ThinkPickCard(cards, canpass, pickCardType, opposingCard));
		}, true);
		if (!canpass && cards.Count == 0)
		{
			CspUtils.DebugLog("User was forced to pass because they had no cards to select but were required to make a selection anyway. Potential broken rule.");
			SendPickCard(null, true, pickCardType);
		}
	}

	public IEnumerator PauseAndPassBlock()
	{
		yield return new WaitForSeconds(1f);
		SendPickCard(null, true, PickCardType.Block);
	}

	public override void OnPickFactor()
	{
		base.OnPickFactor();
		QueueAnimation("Fake Thinking (Pick factor)", delegate
		{
			StartCoroutine(brain.ThinkPickFactor());
		}, true);
	}

	public override void OnPickNumber(int min, int max)
	{
		base.OnPickNumber(min, max);
		QueueAnimation("Fake Thinking (Pick number)", delegate
		{
			StartCoroutine(brain.ThinkPickNumber(min, max));
		}, true);
	}

	public override void OnPickYesNo(int yesButtonID, int noButtonID)
	{
		base.OnPickYesNo(yesButtonID, noButtonID);
		QueueAnimation("Fake Thinking (Pick yesno)", delegate
		{
			StartCoroutine(brain.ThinkPickYesNo());
		}, true);
	}
}
