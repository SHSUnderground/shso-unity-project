public class CharacterRequestedMessage : ShsEventMessage
{
	public readonly CharacterSelectionBlock data;

	public string CharacterName
	{
		get
		{
			return data.name;
		}
	}

	public int R2Attack
	{
		get
		{
			return data.r2Attack;
		}
	}

	public CharacterRequestedMessage(string CharacterName)
	{
		data = new CharacterSelectionBlock(CharacterName, 1);
	}

	public CharacterRequestedMessage(CharacterSelectionBlock CharacterData)
	{
		data = CharacterData;
	}
}
