public class CharacterIconSelected : ShsEventMessage
{
	public readonly string characterName;

	public CharacterIconSelected(string CharacterName)
	{
		characterName = CharacterName;
	}
}
