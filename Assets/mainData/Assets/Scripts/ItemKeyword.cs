public class ItemKeyword
{
	protected int keywordID;

	protected int strength;

	public int Strength
	{
		get
		{
			return strength;
		}
	}

	public ItemKeyword(int newKeywordID, int newStrength)
	{
		keywordID = newKeywordID;
		strength = newStrength;
	}
}
