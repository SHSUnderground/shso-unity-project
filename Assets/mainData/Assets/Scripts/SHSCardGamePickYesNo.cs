using System;
using UnityEngine;

public class SHSCardGamePickYesNo : GUIDynamicWindow
{
	private Action<int> onChoiceMade_;

	public SHSCardGamePickYesNo(int yesButtonID, int noButtonID, Action<int> onChoiceMade)
	{
		onChoiceMade_ = onChoiceMade;
		SetSize(new Vector2(505f, 312f));
		SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
		GUIImage gUIImage = new GUIImage();
		gUIImage.TextureSource = "cardgame_bundle|mshs_cg_yes_no_background";
		gUIImage.SetSize(new Vector2(505f, 312f));
		gUIImage.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
		Add(gUIImage);
		GUIButton gUIButton = new GUIButton();
		gUIButton.Id = "BUTTON_" + yesButtonID.ToString();
		gUIButton.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-114f, -26f), new Vector2(256f, 256f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIButton.StyleInfo = new SHSButtonStyleInfo("cardgame_bundle|mshs_cg_yes_no_" + yesButtonID.ToString());
		gUIButton.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton.ToolTip = new NamedToolTipInfo("#CARD_GAME_YES_NO_" + yesButtonID + "_TOOLTIP");
		gUIButton.Click += delegate
		{
			if (onChoiceMade_ != null)
			{
				onChoiceMade_(yesButtonID);
			}
			Hide();
		};
		Add(gUIButton);
		GUIButton gUIButton2 = new GUIButton();
		gUIButton2.Id = "BUTTON_" + noButtonID.ToString();
		gUIButton2.SetPositionAndSize(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(96f, -26f), new Vector2(256f, 256f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIButton2.StyleInfo = new SHSButtonStyleInfo("cardgame_bundle|mshs_cg_yes_no_" + noButtonID.ToString());
		gUIButton2.HitTestType = HitTestTypeEnum.Alpha;
		gUIButton2.ToolTip = new NamedToolTipInfo("#CARD_GAME_YES_NO_" + noButtonID + "_TOOLTIP");
		gUIButton2.Click += delegate
		{
			if (onChoiceMade_ != null)
			{
				onChoiceMade_(noButtonID);
			}
			Hide();
		};
		Add(gUIButton2);
	}
}
