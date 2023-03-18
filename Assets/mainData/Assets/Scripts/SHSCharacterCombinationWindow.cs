using System.Collections.Generic;
using UnityEngine;

public class SHSCharacterCombinationWindow : SHSGadget.GadgetTopWindow
{
	public static readonly Vector2 COMBINATION_WINDOW_SIZE = new Vector2(592f, 141f);

	public static readonly Vector2 COMBINATION_ITEM_SIZE = new Vector2(542f, 130f);

	public static readonly Vector2 SLIDER_SIZE = new Vector2(50f, 130f);

	private SHSCharacterCombinationList mCombinationList;

	public SHSCharacterCombinationWindow()
	{
		GUIImage gUIImage = new GUIImage();
		gUIImage.SetPosition(0f, 0f);
		gUIImage.SetSize(COMBINATION_WINDOW_SIZE);
		gUIImage.TextureSource = "persistent_bundle|brawler_gadget_combo_bg";
		GUISlider gUISlider = new GUISlider();
		gUISlider.SetPosition(DockingAlignmentEnum.MiddleRight, AnchorAlignmentEnum.MiddleRight, OffsetType.Absolute, Vector2.zero);
		gUISlider.SetSize(SLIDER_SIZE);
		mCombinationList = new SHSCharacterCombinationList(gUISlider);
		mCombinationList.SetPosition(0f, 0f);
		mCombinationList.SetSize(COMBINATION_WINDOW_SIZE);
		Add(gUIImage);
		Add(mCombinationList);
		Add(gUISlider);
	}

	public void AddCombination(string combinationName, string combinationIcon, List<string> characterNames)
	{
		mCombinationList.AddItem(new SHSCharacterCombinationItem(combinationName, combinationIcon, characterNames));
	}

	public void ClearCombinations()
	{
		mCombinationList.ClearItems();
	}
}
