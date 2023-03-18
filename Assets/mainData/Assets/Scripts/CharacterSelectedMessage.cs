public class CharacterSelectedMessage : ShsEventMessage
{
	public readonly CharacterSelectionBlock data;

	public object sender;

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

	public CharacterSelectedMessage(string CharacterName)
	{
		data = new CharacterSelectionBlock(CharacterName, 1);
	}

	public CharacterSelectedMessage(CharacterSelectionBlock CharacterData)
	{
		data = CharacterData;
	}
}
