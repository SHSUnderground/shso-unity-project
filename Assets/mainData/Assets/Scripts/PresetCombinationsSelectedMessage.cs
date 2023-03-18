using System.Collections.Generic;

public class PresetCombinationsSelectedMessage : ShsEventMessage
{
	public List<BaseCharacterCombination> comboSelectedList;

	public PresetCombinationsSelectedMessage(List<BaseCharacterCombination> comboSelectedList)
	{
		this.comboSelectedList = comboSelectedList;
	}
}
