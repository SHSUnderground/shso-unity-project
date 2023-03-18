using System.Collections.Generic;
using UnityEngine;

public class SHSCharacterCombination : GUISimpleControlWindow
{
	private readonly Vector2 CHARACTER_ICON_SIZE = new Vector2(70f, 70f);

	private readonly Vector2 COMBINATION_ICON_SIZE = new Vector2(70f, 70f);

	private readonly int COMBINATION_FONT_SIZE = 14;

	public SHSCharacterCombination(string combinationName, string combinationIcon, List<string> characterNames)
	{
		GUILabel gUILabel = GUIControl.CreateControlFrameCentered<GUILabel>(SHSCharacterCombinationWindow.COMBINATION_ITEM_SIZE, Vector2.zero);
		gUILabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, COMBINATION_FONT_SIZE, Color.black, TextAnchor.MiddleCenter);
		gUILabel.Text = combinationName;
		Add(gUILabel);
		float x = CHARACTER_ICON_SIZE.x;
		float num = 0f - x;
		foreach (string characterName in characterNames)
		{
			num += x;
			GUIImage gUIImage = GUIControl.CreateControl<GUIImage>(CHARACTER_ICON_SIZE, new Vector2(num, 0f), DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft);
			gUIImage.TextureSource = "characters_bundle|inventory_character_" + characterName + "_normal";
			Add(gUIImage);
		}
		GUIImage gUIImage2 = GUIControl.CreateControl<GUIImage>(COMBINATION_ICON_SIZE, Vector2.zero, DockingAlignmentEnum.BottomRight, AnchorAlignmentEnum.BottomRight);
		gUIImage2.TextureSource = "brawler_bundle|" + combinationIcon;
		Add(gUIImage2);
	}
}
