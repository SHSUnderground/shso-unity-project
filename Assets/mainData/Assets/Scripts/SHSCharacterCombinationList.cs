using UnityEngine;

public class SHSCharacterCombinationList : SHSSelectionWindow<SHSCharacterCombinationItem, GUISimpleControlWindow>
{
	//public SHSCharacterCombinationList(GUISlider slider)
	//{
	//	Vector2 cOMBINATION_ITEM_SIZE = SHSCharacterCombinationWindow.COMBINATION_ITEM_SIZE;
	//	base._002Ector(slider, cOMBINATION_ITEM_SIZE.x, SHSCharacterCombinationWindow.COMBINATION_ITEM_SIZE, 8, (GetBackgroundLocation)GetComboItemBackgroundLocation);
	//}

	public SHSCharacterCombinationList(GUISlider slider) : base(slider, SHSCharacterCombinationWindow.COMBINATION_ITEM_SIZE.x, SHSCharacterCombinationWindow.COMBINATION_ITEM_SIZE, 8, new SHSSelectionWindow<SHSCharacterCombinationItem, GUISimpleControlWindow>.GetBackgroundLocation(SHSCharacterCombinationList.GetComboItemBackgroundLocation))
	{
	}


	public static string GetComboItemBackgroundLocation(SHSSelectionItem<GUISimpleControlWindow>.SelectionState selectionState, bool oddInSequence)
	{
		return "persistent_bundle|brawler_gadget_combo_item_bg";
	}
}
