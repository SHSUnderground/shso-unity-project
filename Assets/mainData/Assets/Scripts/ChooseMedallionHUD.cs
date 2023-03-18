using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseMedallionHUD : GUISimpleControlWindow
{
	private const int baseX = 0;

	private const int baseY = 0;

	protected GUIImage background;

	private int currX;

	private int currY;

	private int basePaneSize = 270;

	private int rowCount;

	private int rowCountMax = 5;

	private int deltX = 55;

	private int deltY = 48;

	private bool init;

	public int state;

	private List<MedallionButton> buttons = new List<MedallionButton>();

	private GUISimpleControlWindow scrollPaneMask;

	private GUISimpleControlWindow scrollPane;

	private GUISlider slider;

	public ChooseMedallionHUD()
	{
		SetSize(new Vector2(529f, 366f));
		background = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(529f, 366f), new Vector2(-200f, 0f));
		background.TextureSource = "options_bundle|options_tab_panel_1";
		Add(background);
		BlockTestSpot blockTestSpot = GUIControl.CreateControlFrameCentered<BlockTestSpot>(new Vector2(529f, 346f), new Vector2(-200f, 20f));
		blockTestSpot.BlockTestType = BlockTestTypeEnum.Rect;
		blockTestSpot.HitTestType = HitTestTypeEnum.Rect;
		Add(blockTestSpot);
		scrollPaneMask = new GUISimpleControlWindow();
		scrollPaneMask.SetSize(529f, basePaneSize);
		scrollPaneMask.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(5f, 70f));
		Add(scrollPaneMask);
		scrollPane = new GUISimpleControlWindow();
		scrollPane.SetSize(400f, 800f);
		scrollPane.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(0f, 0f));
		scrollPaneMask.Add(scrollPane);
		slider = new GUISlider();
		slider.Changed += slider_Changed;
		slider.UseMouseWheelScroll = true;
		slider.MouseScrollWheelAmount = 40f;
		slider.TickValue = 40f;
		slider.ArrowsEnabled = true;
		slider.SetSize(40f, 320f);
		slider.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(280f, 45f));
		slider.Rotation = 4f;
		Add(slider);
		slider_Changed(null, null);
		if (!init)
		{
			addButton(-1);
			foreach (int key in TitleManager.getOwnedMedallions().Keys)
			{
				addButton(key);
			}
		}
		GUIButton gUIButton = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(48f, 48f), new Vector2(255f, 29f));
		gUIButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|button_mysquad_close");
		gUIButton.Click += delegate
		{
			state = 0;
			IsVisible = false;
			SetPosition(SHSMySquadChallengeMySquadWindow.MEDALLION_WINDOW_CLOSED);
		};
		gUIButton.ToolTip = new NamedToolTipInfo("#TT_COMMON_1");
		Add(gUIButton);
		AppShell.Instance.EventMgr.AddListener<MedallionPurchasedEvent>(OnMedallionPurchase);
	}

	private void slider_Changed(GUIControl sender, GUIChangedEvent eventData)
	{
		if (currY < basePaneSize)
		{
			scrollPane.Offset = new Vector2(0f, 0f);
			return;
		}
		Vector2 size = scrollPane.Size;
		float num = (size.y - (float)(basePaneSize - 10)) / (float)deltY;
		scrollPane.Offset = new Vector2(0f, (0f - slider.Value) * (num / 100f) * (float)deltY);
	}

	private void OnMedallionPurchase(MedallionPurchasedEvent evt)
	{
		addButton(evt.id);
	}

	private void addButton(int medallionID)
	{
		MedallionData medallion = TitleManager.getMedallion(medallionID);
		if (medallion == null && medallionID != -1)
		{
			CspUtils.DebugLog("\tno medallion data for id " + medallionID);
			return;
		}
		string text = string.Empty;
		MedallionButton medallionButton;
		if (medallionID == -1)
		{
			medallionButton = GUIControl.CreateControlFrameCentered<MedallionButton>(new Vector2(64f, 64f), new Vector2(0f, 0f));
			medallionButton.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(currX - 5, currY - 5));
		}
		else
		{
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(100f, 100f), new Vector2(0f, 0f));
			gUIImage.TextureSource = "persistent_bundle|inventory_iconback";
			gUIImage.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(currX - 23, currY - 23));
			scrollPane.Add(gUIImage);
			medallionButton = GUIControl.CreateControlFrameCentered<MedallionButton>(new Vector2(54f, 54f), new Vector2(0f, 0f));
			medallionButton.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(currX - 1, currY - 1));
			text = medallion.medallionTextureSource;
			if (text.Contains("_normal"))
			{
				text = text.Substring(0, medallion.medallionTextureSource.IndexOf("_normal"));
			}
		}
		if (medallionButton != null && text != string.Empty)
		{
			medallionButton.TextureSource = text;
		}
		else
		{
			medallionButton.TextureSource = "achievement_bundle|button_close_large_normal";
		}
		if (medallionID == -1)
		{
			medallionButton.ToolTip = new NamedToolTipInfo("#MEDALLION_REMOVE");
		}
		else
		{
			medallionButton.ToolTip = new NamedToolTipInfo(medallion.name);
		}
		medallionButton.HitTestSize = new Vector2(1f, 1f);
		medallionButton.HitTestType = HitTestTypeEnum.Circular;
		medallionButton.Click += changeMedallion;
		medallionButton.medallionID = medallionID;
		scrollPane.Add(medallionButton);
		buttons.Add(medallionButton);
		rowCount++;
		if (rowCount >= rowCountMax)
		{
			rowCount = 0;
			currX = 0;
			currY += deltY;
			scrollPane.SetSize(400f, currY + deltY * 2);
			slider.Value = 0f;
			slider_Changed(null, null);
		}
		else
		{
			currX += deltX;
		}
	}

	protected void changeMedallion(GUIControl sender, GUIClickEvent EventData)
	{
		if (sender is MedallionButton)
		{
			TitleManager.currentMedallionID = (sender as MedallionButton).medallionID;
			AppShell.Instance.StartCoroutine(ToggleIconLocks());
			AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "generic_event", "set_medallion", 1, string.Empty);
		}
		else
		{
			CspUtils.DebugLog("Got changeTitle from a control that isn't a medallion button?");
		}
	}

	protected void setIconLock(bool locked)
	{
		foreach (MedallionButton button in buttons)
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
