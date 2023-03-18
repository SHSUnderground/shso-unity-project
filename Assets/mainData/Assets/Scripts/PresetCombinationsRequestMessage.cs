using System.Collections.Generic;

public class PresetCombinationsRequestMessage : ShsEventMessage
{
	public List<string> characterNames;

	public PresetCombinationsRequestMessage(List<string> characterNames)
	{
		this.characterNames = characterNames;
	}
}
