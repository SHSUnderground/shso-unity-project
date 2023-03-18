using System.Collections.Generic;
using UnityEngine;

public class MissionCompletionsWindow : GUISimpleControlWindow
{
	public class MissionButton : GUIButton
	{
		public string missionName = string.Empty;
	}

	private const int baseX = 8;

	private const int baseY = 5;

	protected GUIImage _openFill;

	protected GUIImage _openFooter;

	private int currX = 8;

	private int currY = 5;

	private int rowCount;

	private int rowCountMax = 9;

	private int deltX = 55;

	private int deltY = 48;

	private bool init;

	private string heroName = string.Empty;

	public int state;

	private Dictionary<string, GUIImageWithEvents> _checkMarks = new Dictionary<string, GUIImageWithEvents>();

	private Dictionary<string, MissionButton> _buttons = new Dictionary<string, MissionButton>();

	private GUISimpleControlWindow scrollPaneMask;

	private GUISimpleControlWindow scrollPane;

	private GUISlider slider;

	public MissionCompletionsWindow()
	{
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		SetSize(new Vector2(550f, 366f));
		HitTestType = HitTestTypeEnum.Rect;
		BlockTestType = BlockTestTypeEnum.Rect;
		base.HitTestSize = new Vector2(1f, 1f);
		_openFill = new GUIImage();
		_openFill.SetSize(new Vector2(550f, 224f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		_openFill.Position = new Vector2(0f, 0f);
		_openFill.TextureSource = "achievement_bundle|detail_centerfill";
		_openFill.IsVisible = true;
		Add(_openFill);
		_openFooter = new GUIImage();
		_openFooter.SetSize(new Vector2(550f, 54f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		GUIImage openFooter = _openFooter;
		Vector2 size = _openFill.Size;
		openFooter.Position = new Vector2(0f, size.y);
		_openFooter.TextureSource = "achievement_bundle|detail_bottom_norewards";
		_openFooter.IsVisible = true;
		Add(_openFooter);
		BlockTestSpot blockTestSpot = GUIControl.CreateControlFrameCentered<BlockTestSpot>(new Vector2(529f, 346f), new Vector2(-200f, 20f));
		blockTestSpot.BlockTestType = BlockTestTypeEnum.Rect;
		blockTestSpot.HitTestType = HitTestTypeEnum.Rect;
		Add(blockTestSpot);
		scrollPaneMask = new GUISimpleControlWindow();
		scrollPaneMask.SetSize(529f, 268f);
		scrollPaneMask.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f));
		Add(scrollPaneMask);
		scrollPane = new GUISimpleControlWindow();
		scrollPane.SetSize(500f, 1200f);
		scrollPane.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f));
		scrollPaneMask.Add(scrollPane);
		slider = new GUISlider();
		slider.Changed += slider_Changed;
		slider.TickValue = 40f;
		GUISlider gUISlider = slider;
		Vector2 size2 = scrollPaneMask.Size;
		gUISlider.SetSize(40f, size2.y - 30f);
		GUISlider gUISlider2 = slider;
		Vector2 size3 = _openFill.Size;
		gUISlider2.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(size3.x - 45f, 30f));
		slider.Rotation = 0f;
		Add(slider);
		slider_Changed(null, null);
		foreach (OwnableDefinition item in OwnableDefinition.MissionsByName)
		{
			if (item.released == 1)
			{
				addButton(item);
			}
		}
		GUISimpleControlWindow gUISimpleControlWindow = scrollPane;
		Vector2 size4 = scrollPane.Size;
		gUISimpleControlWindow.SetSize(size4.x, currY + 50);
		Vector2 size5 = new Vector2(48f, 48f);
		Vector2 size6 = scrollPaneMask.Size;
		GUIButton gUIButton = GUIControl.CreateControlTopLeftFrame<GUIButton>(size5, new Vector2(size6.x - 27f, -10f));
		gUIButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|button_mysquad_close");
		gUIButton.Click += delegate
		{
			IsVisible = false;
		};
		gUIButton.ToolTip = new NamedToolTipInfo("#TT_COMMON_1");
		Add(gUIButton);
	}

	public void updateList(string contentList, string heroName, bool localPlayer)
	{
		this.heroName = heroName;
		string[] array = contentList.Split(',');
		bool flag = localPlayer && AppShell.Instance.Profile.AvailableCostumes.ContainsKey(heroName);
		foreach (GUIImageWithEvents value in _checkMarks.Values)
		{
			value.IsVisible = false;
		}
		foreach (OwnableDefinition item in OwnableDefinition.MissionsByName)
		{
			if (localPlayer && flag && AppShell.Instance.Profile.AvailableMissions.ContainsKey(string.Empty + item.ownableTypeID))
			{
				_buttons[item.name].IsEnabled = true;
				_buttons[item.name].ToolTip = new NamedToolTipInfo("#ACHIEVEMENT_JUMP_MISSION");
			}
			else
			{
				_buttons[item.name].IsEnabled = false;
				if (!localPlayer || !flag)
				{
					_buttons[item.name].ToolTip = new NamedToolTipInfo(item.shoppingName);
				}
				else
				{
					_buttons[item.name].ToolTip = new NamedToolTipInfo("#ACHIEVEMENT_JUMP_MISSION_UNOWNED");
				}
			}
		}
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (text != null && text.Length >= 2)
			{
				if (!_checkMarks.ContainsKey(text))
				{
					CspUtils.DebugLog("Error:  found unknown mission name " + text + " in mission completion group achievement information");
				}
				else
				{
					_checkMarks[text].IsVisible = true;
				}
			}
		}
	}

	private void slider_Changed(GUIControl sender, GUIChangedEvent eventData)
	{
		float num = (float)currY / (float)deltY;
		scrollPane.Offset = new Vector2(0f, (0f - slider.Value) * (num / 100f) * (float)deltY);
	}

	private void addButton(OwnableDefinition itemDef)
	{
		MissionButton missionButton = new MissionButton();
		missionButton.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(currX, currY), new Vector2(456f, 45f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		missionButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|green_button_wide", SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		missionButton.HitTestSize = new Vector2(1f, 1f);
		missionButton.HitTestType = HitTestTypeEnum.Circular;
		missionButton.Click += clickedItem;
		missionButton.missionName = itemDef.name;
		scrollPane.Add(missionButton);
		GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 14, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(63, 86, 0), GUILabel.GenColor(120, 170, 4), new Vector2(2f, 3f), TextAnchor.MiddleLeft);
		gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, missionButton.Position + new Vector2(22f, -3f), new Vector2(440f, 45f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIStrokeTextLabel.VerticalKerning += 4;
		gUIStrokeTextLabel.WordWrap = true;
		gUIStrokeTextLabel.Text = itemDef.shoppingName;
		scrollPane.Add(gUIStrokeTextLabel);
		GUIImageWithEvents gUIImageWithEvents = new GUIImageWithEvents();
		gUIImageWithEvents.SetSize(new Vector2(27f, 30f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIImageWithEvents.Position = missionButton.Position + new Vector2(0f, 0f);
		gUIImageWithEvents.TextureSource = "achievement_bundle|objective_check";
		gUIImageWithEvents.IsVisible = false;
		scrollPane.Add(gUIImageWithEvents);
		_checkMarks.Add(itemDef.name, gUIImageWithEvents);
		_buttons.Add(itemDef.name, missionButton);
		currY += deltY;
	}

	protected void clickedItem(GUIControl sender, GUIClickEvent EventData)
	{
		if (sender is MissionButton)
		{
			JumpActionExecutor jumpActionExecutor = new JumpActionExecutor(string.Empty);
			jumpActionExecutor.jumpToMission((sender as MissionButton).missionName, heroName);
		}
	}
}
