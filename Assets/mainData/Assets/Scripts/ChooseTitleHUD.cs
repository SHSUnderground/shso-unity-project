using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseTitleHUD : GUISimpleControlWindow
{
	private const int baseX = 20;

	private const int baseY = 15;

	protected GUIImage background;

	private int currX = 20;

	private int currY = 15;

	private int deltY = 40;

	private bool init;

	public int state;

	private List<GUISimpleControlWindow> buttons = new List<GUISimpleControlWindow>();

	private GUISimpleControlWindow scrollPaneMask;

	private GUISimpleControlWindow scrollPane;

	private GUISlider slider;

	private int basePaneSize = 270;

	private GUISimpleControlWindow noTitlesPanel;

	public ChooseTitleHUD()
	{
		SetSize(new Vector2(529f, 366f));
		background = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(529f, 366f), new Vector2(0f, 0f));
		background.TextureSource = "options_bundle|options_tabless_panel_1";
		Add(background);
		BlockTestSpot blockTestSpot = GUIControl.CreateControlFrameCentered<BlockTestSpot>(new Vector2(529f, 346f), new Vector2(-200f, 20f));
		blockTestSpot.BlockTestType = BlockTestTypeEnum.Rect;
		blockTestSpot.HitTestType = HitTestTypeEnum.Rect;
		Add(blockTestSpot);
		scrollPaneMask = new GUISimpleControlWindow();
		scrollPaneMask.SetSize(579f, basePaneSize);
		scrollPaneMask.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(5f, 70f));
		Add(scrollPaneMask);
		scrollPane = new GUISimpleControlWindow();
		scrollPane.SetSize(500f, 4800f);
		scrollPane.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f));
		scrollPaneMask.Add(scrollPane);
		slider = new GUISlider();
		slider.Changed += slider_Changed;
		slider.UseMouseWheelScroll = true;
		slider.MouseScrollWheelAmount = 40f;
		slider.TickValue = 40f;
		slider.ArrowsEnabled = true;
		slider.SetSize(40f, 320f);
		slider.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(480f, 45f));
		slider.Rotation = 4f;
		Add(slider);
		slider_Changed(null, null);
		noTitlesPanel = new GUISimpleControlWindow();
		noTitlesPanel.SetSize(500f, 800f);
		noTitlesPanel.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f));
		GUIDropShadowTextLabel gUIDropShadowTextLabel = new GUIDropShadowTextLabel();
		gUIDropShadowTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(50f, 50f), new Vector2(342f, 185f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIDropShadowTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Zooom, 28, Color.black, TextAnchor.MiddleCenter);
		gUIDropShadowTextLabel.TextOffset = new Vector2(-2f, 2f);
		gUIDropShadowTextLabel.FrontColor = GUILabel.GenColor(255, 155, 65);
		gUIDropShadowTextLabel.BackColor = GUILabel.GenColor(0, 14, 55);
		gUIDropShadowTextLabel.Text = "#TITLE_NO_TITLES_OWNED";
		noTitlesPanel.Add(gUIDropShadowTextLabel);
		GUIButton gUIButton = new GUIButton();
		gUIButton.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(106f, 187f), new Vector2(210f, 145f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		gUIButton.StyleInfo = new SHSButtonStyleInfo("persistent_bundle|L_buynow_button");
		gUIButton.HitTestSize = new Vector2(0.87f, 0.79f);
		gUIButton.ToolTip = new NamedToolTipInfo("#TITLE_GO_TO_SHOP");
		noTitlesPanel.Add(gUIButton);
		gUIButton.Click += delegate
		{
			ShoppingWindow shoppingWindow = new ShoppingWindow(NewShoppingManager.ShoppingCategory.Title);
			shoppingWindow.launch();
		};
		if (!init)
		{
			if (TitleManager.getOwnedTitles().Count == 0)
			{
				Add(noTitlesPanel);
			}
			else
			{
				Remove(noTitlesPanel);
				foreach (int key in TitleManager.getOwnedTitles().Keys)
				{
					addButton(key);
				}
				addButton(-1);
			}
		}
		GUIButton gUIButton2 = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(48f, 48f), new Vector2(455f, 29f));
		gUIButton2.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|button_mysquad_close");
		gUIButton2.Click += delegate
		{
			state = 0;
			IsVisible = false;
			SetPosition(SHSMySquadChallengeMySquadWindow.TITLE_WINDOW_CLOSED);
		};
		gUIButton2.ToolTip = new NamedToolTipInfo("#TT_COMMON_1");
		Add(gUIButton2);
		AppShell.Instance.EventMgr.AddListener<TitlePurchasedEvent>(OnTitlePurchase);
	}

	private void slider_Changed(GUIControl sender, GUIChangedEvent eventData)
	{
		if (currY < basePaneSize)
		{
			scrollPane.Offset = new Vector2(0f, 0f);
			return;
		}
		float num = (float)(currY - (basePaneSize - 10)) / (float)deltY;
		scrollPane.Offset = new Vector2(0f, (0f - slider.Value) * (num / 100f) * (float)deltY);
	}

	private void OnTitlePurchase(TitlePurchasedEvent evt)
	{
		Remove(noTitlesPanel);
		addButton(evt.id);
	}

	private void addButton(int titleID)
	{
		TitleData title = TitleManager.getTitle(titleID);
		if (title == null && titleID != -1)
		{
			CspUtils.DebugLog("\tno title data for id " + titleID);
			return;
		}
		GUISimpleControlWindow gUISimpleControlWindow = new GUISimpleControlWindow();
		gUISimpleControlWindow.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(currX, currY), new Vector2(456f, 45f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		TitleButton titleButton = new TitleButton();
		titleButton.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f), new Vector2(456f, 45f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		titleButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|green_button_wide", SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		if (titleID == -1)
		{
			titleButton.ToolTip = new NamedToolTipInfo("#CHOOSETITLE_REMOVETITLE_BUTTON");
		}
		else
		{
			titleButton.ToolTip = new NamedToolTipInfo(title.titleString);
		}
		titleButton.HitTestSize = new Vector2(1f, 1f);
		titleButton.HitTestType = HitTestTypeEnum.Circular;
		titleButton.Click += changeTitle;
		titleButton.titleID = titleID;
		gUISimpleControlWindow.Add(titleButton);
		GUIStrokeTextLabel gUIStrokeTextLabel = new GUIStrokeTextLabel();
		gUIStrokeTextLabel.Id = "nextChallengeLabel";
		gUIStrokeTextLabel.SetupText(GUIFontManager.SupportedFontEnum.Grobold, 18, GUILabel.GenColor(255, 255, 255), GUILabel.GenColor(63, 86, 0), GUILabel.GenColor(120, 170, 4), new Vector2(2f, 3f), TextAnchor.MiddleLeft);
		gUIStrokeTextLabel.SetPositionAndSize(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(22f, -3f), new Vector2(430f, 45f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		if (titleID == -1)
		{
			gUIStrokeTextLabel.Text = AppShell.Instance.stringTable.GetString("#CHOOSETITLE_REMOVETITLE_BUTTON");
		}
		else
		{
			gUIStrokeTextLabel.Text = AppShell.Instance.stringTable.GetString(title.titleString);
		}
		gUISimpleControlWindow.Add(gUIStrokeTextLabel);
		scrollPane.Add(gUISimpleControlWindow);
		buttons.Add(gUISimpleControlWindow);
		currY += deltY;
		scrollPane.SetSize(500f, currY + deltY);
		slider.Value = 0f;
		slider_Changed(null, null);
	}

	protected void changeTitle(GUIControl sender, GUIClickEvent EventData)
	{
		if (sender is TitleButton)
		{
			TitleManager.currentTitleID = (sender as TitleButton).titleID;
			AppShell.Instance.StartCoroutine(ToggleIconLocks());
			AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "generic_event", "set_title", 1, string.Empty);
		}
		else
		{
			CspUtils.DebugLog("Got changeTitle from a control that isn't a title button?");
		}
	}

	protected void setIconLock(bool locked)
	{
		foreach (GUISimpleControlWindow button in buttons)
		{
			button.IsEnabled = !locked;
		}
	}

	protected IEnumerator ToggleIconLocks()
	{
		setIconLock(true);
		yield return new WaitForSeconds(5f);
		setIconLock(false);
	}
}
