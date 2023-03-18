using System;
using System.Collections.Generic;
using UnityEngine;

public class SHSCardGamePickFactor : GUIDynamicWindow
{
	private char[] FactorList = new char[6]
	{
		'S',
		'P',
		'E',
		'T',
		'A',
		'N'
	};

	private Action<BattleCard.Factor> onFactorSelected_;

	private SHSGlowOutlineWindow glowWindow;

	private Dictionary<BattleCard.Factor, GUIAnimatedButton> FactorButtons;

	public SHSCardGamePickFactor(Action<BattleCard.Factor> onFactorSelected)
	{
		onFactorSelected_ = onFactorSelected;
		SetSize(new Vector2(530f, 169f));
		SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
		GUIImage gUIImage = new GUIImage();
		gUIImage.TextureSource = "cardgame_bundle|mshs_cg_hand_container";
		gUIImage.SetSize(new Vector2(530f, 169f));
		gUIImage.SetPosition(DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero);
		Add(gUIImage);
		List<Vector2> selectedGlowPath = SHSGlowOutlineWindow.GenerateCircularPath(40f, 16);
		glowWindow = new SHSGlowOutlineWindow(selectedGlowPath, "cardgame_bundle|mshs_cg_dot_a", 2, 1f);
		glowWindow.SetPositionAndSize(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(95f, 95f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		glowWindow.HighlightSpeed = 150f;
		FactorButtons = new Dictionary<BattleCard.Factor, GUIAnimatedButton>();
		char[] factorList = FactorList;
		foreach (char c in factorList)
		{
			char factorChar = c;
			BattleCard.Factor factor = BattleCard.CharToFactor(factorChar);
			GUIAnimatedButton factorButton = new GUIAnimatedButton();
			factorButton.Id = "factorButton" + factorChar;
			factorButton.TextureSource = "cardgame_bundle|mshs_cg_factor_" + factorChar;
			factorButton.SetSize(new Vector2(84f, 84f));
			factorButton.SetupButton(1f, 1.05f, 0.95f);
			factorButton.HitTestType = HitTestTypeEnum.Circular;
			factorButton.HitTestSize = new Vector2(0.85f, 0.85f);
			factorButton.Click += delegate
			{
				if (onFactorSelected_ != null)
				{
					onFactorSelected_(factor);
				}
				Hide();
			};
			factorButton.MouseOver += delegate
			{
				glowWindow.IsVisible = true;
				glowWindow.Highlight(true);
				glowWindow.SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, factorButton.Offset);
				glowWindow.BeadTextureSource = "cardgame_bundle|mshs_cg_dot_" + factorChar;
			};
			factorButton.MouseOut += delegate
			{
				glowWindow.IsVisible = false;
			};
			FactorButtons[factor] = factorButton;
			Add(factorButton);
		}
		Add(glowWindow);
		FactorButtons[BattleCard.Factor.Animal].SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(58f, 30f));
		FactorButtons[BattleCard.Factor.Elemental].SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(135f, -13f));
		FactorButtons[BattleCard.Factor.Energy].SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(222f, -37f));
		FactorButtons[BattleCard.Factor.Speed].SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(308f, -37f));
		FactorButtons[BattleCard.Factor.Strength].SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(395f, -13f));
		FactorButtons[BattleCard.Factor.Tech].SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(471f, 30f));
	}

	public override void OnShow()
	{
		base.OnShow();
		glowWindow.IsVisible = false;
	}
}
