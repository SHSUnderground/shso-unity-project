public class HeroSpecialAbility : SpecialAbility
{
	public string heroName = string.Empty;

	public HeroSpecialAbility(string heroName)
	{
		this.heroName = heroName;
		specialAbilityID = SpecialAbility.nextSpecialAbilityID++;
	}

	public override bool sameAs(SpecialAbility ab)
	{
		if (!base.sameAs(ab))
		{
			return false;
		}
		HeroSpecialAbility heroSpecialAbility = ab as HeroSpecialAbility;
		if (heroName == heroSpecialAbility.heroName)
		{
			return true;
		}
		return false;
	}

	public static HeroSpecialAbility fromKeyword(string heroName, Keyword keyword)
	{
		HeroSpecialAbility heroSpecialAbility = new HeroSpecialAbility(heroName);
		heroSpecialAbility.icon = keyword.icon;
		heroSpecialAbility.name = keyword.tooltip;
		heroSpecialAbility.displaySpace = "social";
		return heroSpecialAbility;
	}
}
