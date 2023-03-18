using System;

public class CardFilter
{
	public bool active = true;

	public CardFilterType type;

	public BattleCard.Factor factor;

	public int iData;

	public string sData;

	public CardFilter(CardFilterType _type, BattleCard.Factor fac)
	{
		type = _type;
		factor = fac;
		active = false;
	}

	public CardFilter(CardFilterType _type, int data)
	{
		type = _type;
		iData = data;
	}

	public CardFilter(CardFilterType _type, string data)
	{
		type = _type;
		sData = data;
		active = false;
	}

	public bool Test(CardListCard card)
	{
		switch (type)
		{
		case CardFilterType.BlockStrength:
		case CardFilterType.BlockSpeed:
		case CardFilterType.BlockElemental:
		case CardFilterType.BlockTech:
		case CardFilterType.BlockAnimal:
		case CardFilterType.BlockEnergy:
			return card.HasBlock(factor);
		case CardFilterType.FactorStrength:
		case CardFilterType.FactorSpeed:
		case CardFilterType.FactorElemental:
		case CardFilterType.FactorTech:
		case CardFilterType.FactorAnimal:
		case CardFilterType.FactorEnergy:
			return card.HasFactor(factor);
		case CardFilterType.TeamAvengers:
		case CardFilterType.TeamXmen:
		case CardFilterType.TeamBrotherhood:
		case CardFilterType.TeamSS:
		case CardFilterType.TeamF4:
		case CardFilterType.TeamSpideyFriends:
			return card.BelongsToTeam(sData);
		case CardFilterType.MinLevel:
			return card.Level >= iData;
		case CardFilterType.MaxLevel:
			return card.Level <= iData;
		case CardFilterType.TextSearch:
			return false || card.Name.IndexOf(sData, StringComparison.OrdinalIgnoreCase) >= 0 || card.HeroName.IndexOf(sData, StringComparison.OrdinalIgnoreCase) >= 0 || card.AbilityText.IndexOf(sData, StringComparison.OrdinalIgnoreCase) >= 0;
		default:
			return false;
		}
	}
}
