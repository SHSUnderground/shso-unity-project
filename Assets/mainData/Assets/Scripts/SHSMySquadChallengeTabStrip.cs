using System.Collections.Generic;
using UnityEngine;

public class SHSMySquadChallengeTabStrip : GUISimpleControlWindow
{
	protected struct TabInfo
	{
		public string tabName;

		public TabType tabType;

		public Vector2 buttonOffset;

		public Vector2 selectedTextOffset;

		public TabInfo(string TabName, TabType TType, Vector2 ButtonOffset, Vector2 SelectedTextOffset)
		{
			tabName = TabName;
			tabType = TType;
			buttonOffset = ButtonOffset;
			selectedTextOffset = SelectedTextOffset;
		}
	}

	public class TabChangedEvent : ShsEventMessage
	{
		public TabType selectedTab;

		public TabChangedEvent(TabType SelectedTab)
		{
			selectedTab = SelectedTab;
		}
	}

	public enum TabType
	{
		MySquad,
		Heroes,
		Challenges,
		Fractals,
		Crafting
	}

	public delegate void TabSelectedHandler(TabType tabType);

	protected GUIStrokeTextLabel[] selectedTabLabels;

	protected GUIDropShadowTextLabel[] unselectedTabLabels;

	protected GUIImage[] tabBackgrounds;

	protected TabType currentlySelectedTab;

	public TabType SelectedTab
	{
		get
		{
			return currentlySelectedTab;
		}
		set
		{
			currentlySelectedTab = value;
			for (int i = 0; i < tabBackgrounds.Length; i++)
			{
				tabBackgrounds[i].IsVisible = ((currentlySelectedTab == (TabType)i) ? true : false);
			}
			for (int j = 0; j < unselectedTabLabels.Length; j++)
			{
				unselectedTabLabels[j].IsVisible = ((currentlySelectedTab != (TabType)j) ? true : false);
			}
			for (int k = 0; k < unselectedTabLabels.Length; k++)
			{
				selectedTabLabels[k].IsVisible = ((currentlySelectedTab == (TabType)k) ? true : false);
			}
			if (this.OnTabSelected != null)
			{
				this.OnTabSelected(currentlySelectedTab);
			}
		}
	}

	public event TabSelectedHandler OnTabSelected;

	public SHSMySquadChallengeTabStrip(bool isMySquad, TabType initialTab)
	{
		SetSize(1020f, 300f);
		SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft);
		List<TabInfo> list = new List<TabInfo>();
		list.Add(new TabInfo((!isMySquad) ? "#SQ_TAB_THEIRSQUAD" : "#SQ_TAB_MYSQUAD", TabType.MySquad, new Vector2(23f, 35f), new Vector2(21f, 0f)));
		list.Add(new TabInfo("#SQ_TAB_HEROES", TabType.Heroes, new Vector2(200f, 35f), new Vector2(-32f, -9f)));
		list.Add(new TabInfo("#SQ_TAB_CHALLENGES", TabType.Challenges, new Vector2(362f, 35f), new Vector2(-15f, -9f)));
		list.Add(new TabInfo("#SQ_TAB_FRACTALS", TabType.Fractals, new Vector2(537f, 35f), new Vector2(-18f, 0f)));
		List<TabInfo> list2 = list;
		list2.Add(new TabInfo("#SQ_TAB_CRAFTING", TabType.Crafting, new Vector2(707f, 35f), new Vector2(-18f, 0f)));
		tabBackgrounds = new GUIImage[list2.Count];
		selectedTabLabels = new GUIStrokeTextLabel[list2.Count];
		unselectedTabLabels = new GUIDropShadowTextLabel[list2.Count];
		for (int i = 0; i < list2.Count; i++)
		{
			tabBackgrounds[i] = new GUIImage();
			tabBackgrounds[i].SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(973f, 101f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			tabBackgrounds[i].TextureSource = "mysquadgadget_bundle|mshs_mysquad_challenge_tab_" + (i + 1);
			tabBackgrounds[i].IsVisible = false;
			Add(tabBackgrounds[i]);
		}
		for (int j = 0; j < list2.Count; j++)
		{
			GUIDropShadowTextLabel newLabel = new GUIDropShadowTextLabel();
			newLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 17, GUILabel.GenColor(64, 163, 255), GUILabel.GenColor(0, 14, 55), new Vector2(1f, 2f), TextAnchor.MiddleCenter);
			GUIDropShadowTextLabel gUIDropShadowTextLabel = newLabel;
			TabInfo tabInfo = list2[j];
			gUIDropShadowTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, tabInfo.buttonOffset, new Vector2(200f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			GUIDropShadowTextLabel gUIDropShadowTextLabel2 = newLabel;
			TabInfo tabInfo2 = list2[j];
			gUIDropShadowTextLabel2.Text = tabInfo2.tabName;
			newLabel.Traits.EventHandlingTrait = ControlTraits.EventHandlingEnum.Block;
			newLabel.MouseOver += delegate
			{
				newLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 18, GUILabel.GenColor(189, 231, 255), GUILabel.GenColor(0, 14, 55), new Vector2(1f, 2f), TextAnchor.MiddleCenter);
			};
			newLabel.MouseOut += delegate
			{
				newLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 17, GUILabel.GenColor(64, 163, 255), GUILabel.GenColor(0, 14, 55), new Vector2(1f, 2f), TextAnchor.MiddleCenter);
			};
			TabInfo tabInfo3 = list2[j];
			TabType tabType = tabInfo3.tabType;
			newLabel.MouseDown += delegate
			{
				SelectedTab = tabType;
			};
			newLabel.IsVisible = false;
			unselectedTabLabels[j] = newLabel;
			Add(newLabel);
			selectedTabLabels[j] = new GUIStrokeTextLabel();
			selectedTabLabels[j].SetupText(GUIFontManager.SupportedFontEnum.Grobold, 26, GUILabel.GenColor(172, 214, 9), GUILabel.GenColor(0, 23, 67), GUILabel.GenColor(0, 23, 67), new Vector2(3f, 5f), TextAnchor.MiddleCenter);
			GUIStrokeTextLabel obj = selectedTabLabels[j];
			TabInfo tabInfo4 = list2[j];
			Vector2 buttonOffset = tabInfo4.buttonOffset;
			TabInfo tabInfo5 = list2[j];
			obj.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, buttonOffset + tabInfo5.selectedTextOffset, new Vector2(200f, 50f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
			selectedTabLabels[j].Rotation = -5f;
			GUIStrokeTextLabel obj2 = selectedTabLabels[j];
			TabInfo tabInfo6 = list2[j];
			obj2.Text = tabInfo6.tabName;
			selectedTabLabels[j].IsVisible = false;
			Add(selectedTabLabels[j]);
		}
		SelectedTab = initialTab;
	}

	public override void OnActive()
	{
		base.OnActive();
		AppShell.Instance.EventMgr.AddListener<TabChangedEvent>(OnTabChangedMessage);
	}

	public override void OnInactive()
	{
		base.OnInactive();
		AppShell.Instance.EventMgr.RemoveListener<TabChangedEvent>(OnTabChangedMessage);
	}

	protected void OnTabChangedMessage(TabChangedEvent msg)
	{
		SelectedTab = msg.selectedTab;
	}
}
