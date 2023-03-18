using System.Collections.Generic;

public class SHSCharacterCombinationItem : SHSSelectionItem<GUISimpleControlWindow>
{
	public SHSCharacterCombinationItem(string combinationName, string combinationIcon, List<string> characterNames)
	{
		item = new SHSCharacterCombination(combinationName, combinationIcon, characterNames);
		item.HitTestType = GUIControl.HitTestTypeEnum.Transparent;
		itemSize = SHSCharacterCombinationWindow.COMBINATION_ITEM_SIZE;
		currentState = SelectionState.Passive;
	}
}
