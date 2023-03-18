using System;
using UnityEngine;

public class SHSCharacterSelectScanlineTransition : SHSGadget.GadgetCenterWindow
{
	public delegate void HeroClickedDelegate(string heroName);

	private SHSScanlineTransitionWindow<string> scanlineWindow;

	public event HeroClickedDelegate HeroClicked;

	public void CharacterSelectScanlineTransition(SHSCharacterSelect characterSelect, Func<string, GUIWindow> getWindow, string defaultHero, float scanlineTime, Vector2 scanlineWindowSize)
	{
		scanlineWindow = new SHSScanlineTransitionWindow<string>(SHSScanlineTransitionWindow<string>.DefaultScanlineWindowSize, scanlineWindowSize, scanlineTime, getWindow);
		scanlineWindow.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(172f, -284f));
		Add(scanlineWindow);
		characterSelect.HeroClicked += SelectHero;
		characterSelect.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, SHSGadget.LEFT_WINDOW_OFFSET);
		characterSelect.SetSize(SHSGadget.LEFT_WINDOW_SIZE);
		characterSelect.AnimationOnOpen = null;
		characterSelect.AnimationOnClose = null;
		Add(characterSelect);
		if (!string.IsNullOrEmpty(defaultHero))
		{
			scanlineWindow.InitialSetup(defaultHero);
		}
	}

	public void CharacterSelectScanlineTransition(SHSCharacterSelect characterSelect, Func<string, GUIWindow> getWindow, string defaultHero)
	{
		CharacterSelectScanlineTransition(characterSelect, getWindow, defaultHero, SHSScanlineTransitionWindow<string>.DefaultScanlineTime, SHSScanlineTransitionWindow<string>.DefaultScanlineWindowSize);
	}

	public void SelectHero(string heroName)
	{
		if (this.HeroClicked != null)
		{
			this.HeroClicked(heroName);
		}
		scanlineWindow.BeginTransition(heroName);
	}
}
