public class AckPickHeroDeckMessage : ShsEventMessage
{
	public readonly int PlayerId;

	public readonly string HeroName;

	public readonly string DeckRecipe;

	public readonly int DeckId;

	public AckPickHeroDeckMessage(int SelectedPlayerId, string NameOfHero, string Recipe, int IdOfDeck)
	{
		PlayerId = SelectedPlayerId;
		HeroName = NameOfHero;
		DeckRecipe = Recipe;
		DeckId = IdOfDeck;
	}
}
