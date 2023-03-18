using System.Collections.Generic;
using System.Runtime.CompilerServices;

public class PresetCombinationsApplyMessage : ShsEventMessage
{
	[CompilerGenerated]
	private List<BaseCharacterCombination> _003CCombinations_003Ek__BackingField;

	public List<BaseCharacterCombination> Combinations
	{
		[CompilerGenerated]
		get
		{
			return _003CCombinations_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CCombinations_003Ek__BackingField = value;
		}
	}

	public PresetCombinationsApplyMessage(List<BaseCharacterCombination> combinations)
	{
		Combinations = combinations;
	}
}
