using CardGame;
using System.Collections.Generic;

public class CardGameResults
{
	public List<PlayerData> players;

	public int CardsRemaining;

	public readonly List<ContributingFactor> ContributingFactors;

	public readonly List<FixedAward> FixedAwards;

	public int CoinsEarned;

	public int TicketsEarned;

	public CardGameResults()
	{
		ContributingFactors = new List<ContributingFactor>();
		FixedAwards = new List<FixedAward>();
		CoinsEarned = 0;
		TicketsEarned = 0;
		CardsRemaining = 0;
		players = new List<PlayerData>(2);
	}

	public void ProcessCardGameEndEventResult(EventResultSquadBattle eventResult)
	{
		CoinsEarned = eventResult.Coins;
		TicketsEarned = eventResult.Tickets;
		players = eventResult.Players;
		CardsRemaining = eventResult.CardsLeft;
		if (TicketsEarned > 0)
		{
			FixedAwards.Add(new FixedAward(FixedAward.AwardType.Tickets, TicketsEarned + " Tickets", "notification_bundle|inventory_currency_tickets_icon_f01", TicketsEarned));
		}
		if (CoinsEarned > 0)
		{
			FixedAwards.Add(new FixedAward(FixedAward.AwardType.Coins, CoinsEarned + " Coins", "notification_bundle|inventory_currency_coins_icon_f01", CoinsEarned));
		}
		int num = eventResult.Time / 60;
		int num2 = eventResult.Time % 60;
		ContributingFactors.Add(new ContributingFactor(ContributingFactor.FactorType.PlayTime, "Total Time", num.ToString() + ":" + ((num2 >= 10) ? string.Empty : "0") + num2.ToString(), 0, "notification_bundle|factor_time_icon"));
		ContributingFactors.Add(new ContributingFactor(ContributingFactor.FactorType.Unknown, "Cards Left", CardsRemaining.ToString(), 0, "notification_bundle|factor_cards_icon"));
	}
}
