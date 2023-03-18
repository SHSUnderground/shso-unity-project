using System.Collections.Generic;
using UnityEngine;

public class HeroCompletionsWindow : GUISimpleControlWindow
{
	public class HeroHeadButton : GUIButton
	{
		public string heroName = string.Empty;
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

	private int deltY = 55;

	private bool init;

	private string missionName = string.Empty;

	public int state;

	private Dictionary<string, GUIImageWithEvents> _checkMarks = new Dictionary<string, GUIImageWithEvents>();

	private Dictionary<string, HeroHeadButton> _buttons = new Dictionary<string, HeroHeadButton>();

	private GUISimpleControlWindow scrollPaneMask;

	private GUISimpleControlWindow scrollPane;

	private GUISlider slider;

	public HeroCompletionsWindow()
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
		foreach (OwnableDefinition item in OwnableDefinition.HeroesByName)
		{
			if (item.released == 1)
			{
				addButton(item);
			}
		}
		Vector2 size4 = new Vector2(48f, 48f);
		Vector2 size5 = scrollPaneMask.Size;
		GUIButton gUIButton = GUIControl.CreateControlTopLeftFrame<GUIButton>(size4, new Vector2(size5.x - 27f, -10f));
		gUIButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|button_mysquad_close");
		gUIButton.Click += delegate
		{
			IsVisible = false;
		};
		gUIButton.ToolTip = new NamedToolTipInfo("#TT_COMMON_1");
		Add(gUIButton);
	}

	public void updateList(string heroList, string missionName, bool localPlayer)
	{
		this.missionName = missionName;
		string[] array = heroList.Split(',');
		foreach (GUIImageWithEvents value in _checkMarks.Values)
		{
			value.IsVisible = false;
		}
		OwnableDefinition missionDef = OwnableDefinition.getMissionDef(this.missionName);
		bool flag = localPlayer && AppShell.Instance.Profile.AvailableMissions.ContainsKey(string.Empty + missionDef.ownableTypeID);
		foreach (OwnableDefinition item in OwnableDefinition.HeroesByName)
		{
			if (item.released != 0)
			{
				if (flag && localPlayer && AppShell.Instance.Profile.AvailableCostumes.ContainsKey(item.name))
				{
					_buttons[item.name].IsEnabled = true;
					_buttons[item.name].ToolTip = new NamedToolTipInfo(AppShell.Instance.stringTable.GetString("#ACHIEVEMENT_JUMP_HERO") + " " + AppShell.Instance.stringTable.GetString(item.shoppingName));
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
						_buttons[item.name].ToolTip = new NamedToolTipInfo("#ACHIEVEMENT_JUMP_HERO_UNOWNED");
					}
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
					CspUtils.DebugLog("Error:  found unknown hero name " + text + " in mission completion group achievement information");
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

	private void addButton(OwnableDefinition heroDef)
	{
		GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(100f, 100f), new Vector2(0f, 0f));
		gUIImage.TextureSource = "persistent_bundle|inventory_iconback";
		gUIImage.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(currX - 23, currY - 23));
		scrollPane.Add(gUIImage);
		HeroHeadButton heroHeadButton = GUIControl.CreateControlFrameCentered<HeroHeadButton>(new Vector2(80f, 80f), new Vector2(0f, 0f));
		heroHeadButton.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(currX - 12, currY - 15));
		heroHeadButton.StyleInfo = new SHSButtonStyleInfo("characters_bundle|inventory_character_" + heroDef.name, SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		heroHeadButton.ToolTip = new NamedToolTipInfo(heroDef.shoppingName);
		heroHeadButton.HitTestSize = new Vector2(1f, 1f);
		heroHeadButton.HitTestType = HitTestTypeEnum.Circular;
		heroHeadButton.Click += clickedHero;
		heroHeadButton.heroName = heroDef.name;
		scrollPane.Add(heroHeadButton);
		GUIImageWithEvents gUIImageWithEvents = new GUIImageWithEvents();
		gUIImageWithEvents.SetSize(new Vector2(27f, 30f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIImageWithEvents.Position = heroHeadButton.Position + new Vector2(40f, 40f);
		gUIImageWithEvents.TextureSource = "achievement_bundle|objective_check";
		gUIImageWithEvents.IsVisible = false;
		scrollPane.Add(gUIImageWithEvents);
		_checkMarks.Add(heroDef.name, gUIImageWithEvents);
		_buttons.Add(heroDef.name, heroHeadButton);
		rowCount++;
		if (rowCount >= rowCountMax)
		{
			rowCount = 0;
			currX = 8;
			currY += deltY;
		}
		else
		{
			currX += deltX;
		}
	}

	protected void clickedHero(GUIControl sender, GUIClickEvent EventData)
	{
		if (sender is HeroHeadButton)
		{
			JumpActionExecutor jumpActionExecutor = new JumpActionExecutor(string.Empty);
			jumpActionExecutor.jumpToMission(missionName, (sender as HeroHeadButton).heroName);
		}
	}
}
