public class CharacterSelectionBlock
{
	public string name;

	public int r2Attack;

	public CharacterSelectionBlock(string Name, int R2Attack)
	{
		name = Name;
		r2Attack = R2Attack;
	}

	public override string ToString()
	{
		return name;
	}
}
