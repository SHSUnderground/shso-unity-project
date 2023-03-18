using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoosePetHUD : GUISimpleControlWindow
{
	private const int baseX = 0;

	private const int baseY = 0;

	protected GUIImage background;

	private int currX;

	private int currY;

	private int rowCount;

	private int rowCountMax = 5;

	private int deltX = 55;

	private int deltY = 55;

	private bool init;

	public int state;

	private List<PetButton> buttons = new List<PetButton>();

	private GUISimpleControlWindow scrollPaneMask;

	private GUISimpleControlWindow scrollPane;

	private GUISlider slider;

	public ChoosePetHUD()
	{
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		SetSize(new Vector2(529f, 366f));
		HitTestType = HitTestTypeEnum.Rect;
		BlockTestType = BlockTestTypeEnum.Rect;
		base.HitTestSize = new Vector2(1f, 1f);
		background = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(529f, 366f), new Vector2(-200f, 0f));
		background.TextureSource = "options_bundle|options_tab_panel_1";
		Add(background);
		BlockTestSpot blockTestSpot = GUIControl.CreateControlFrameCentered<BlockTestSpot>(new Vector2(529f, 346f), new Vector2(-200f, 20f));
		blockTestSpot.BlockTestType = BlockTestTypeEnum.Rect;
		blockTestSpot.HitTestType = HitTestTypeEnum.Rect;
		Add(blockTestSpot);
		scrollPaneMask = new GUISimpleControlWindow();
		scrollPaneMask.SetSize(529f, 270f);
		scrollPaneMask.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(5f, 70f));
		Add(scrollPaneMask);
		scrollPane = new GUISimpleControlWindow();
		scrollPane.SetSize(400f, 1600f);
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
			loadFromCollection();
		}
		GUIButton gUIButton = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(48f, 48f), new Vector2(255f, 29f));
		gUIButton.StyleInfo = new SHSButtonStyleInfo("mysquadgadget_bundle|button_mysquad_close");
		gUIButton.Click += delegate
		{
			state = 0;
			IsVisible = false;
			SetPosition(SHSSocialCharacterDisplayWindow.PET_WINDOW_CLOSED);
		};
		gUIButton.ToolTip = new NamedToolTipInfo("#TT_COMMON_1");
		Add(gUIButton);
		AppShell.Instance.EventMgr.AddListener<PetPurchasedEvent>(OnPetPurchase);
		AppShell.Instance.EventMgr.AddListener<PetCollectionRefreshed>(OnPetCollectionRefreshed);
	}

	private void loadFromCollection()
	{
		foreach (PetButton button in buttons)
		{
			scrollPane.Remove(button);
			button.Dispose();
		}
		buttons.Clear();
		currX = 0;
		currY = 0;
		rowCount = 0;
		foreach (int key in PetDataManager.getOwnedPets().Keys)
		{
			addButton(key);
		}
	}

	private void slider_Changed(GUIControl sender, GUIChangedEvent eventData)
	{
		float num = (float)currY / (float)deltY;
		scrollPane.Offset = new Vector2(0f, (0f - slider.Value) * (num / 100f) * (float)deltY);
	}

	private void OnPetCollectionRefreshed(PetCollectionRefreshed evt)
	{
		loadFromCollection();
	}

	private void OnPetPurchase(PetPurchasedEvent evt)
	{
		addButton(evt.id);
	}

	private void addButton(int petID)
	{
		PetData data = PetDataManager.getData(petID);
		if (data == null && petID != -1)
		{
			CspUtils.DebugLog("\tno pet data for id " + petID);
			return;
		}
		PetButton petButton;
		if (petID == -1)
		{
			petButton = GUIControl.CreateControlFrameCentered<PetButton>(new Vector2(54f, 54f), new Vector2(0f, 0f));
			petButton.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(currX, currY));
		}
		else
		{
			GUIImage gUIImage = GUIControl.CreateControlFrameCentered<GUIImage>(new Vector2(100f, 100f), new Vector2(0f, 0f));
			gUIImage.TextureSource = "persistent_bundle|inventory_iconback";
			gUIImage.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(currX - 23, currY - 23));
			scrollPane.Add(gUIImage);
			petButton = GUIControl.CreateControlFrameCentered<PetButton>(new Vector2(80f, 80f), new Vector2(0f, 0f));
			petButton.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, new Vector2(currX - 12, currY - 15));
		}
		if (data != null && data.inventoryIconBase != null)
		{
			petButton.StyleInfo = new SHSButtonStyleInfo("shopping_bundle|" + data.inventoryIconBase, SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		}
		else
		{
			petButton.StyleInfo = new SHSButtonStyleInfo("gameworld_bundle|icon_changesidekick");
		}
		if (petID == -1)
		{
			petButton.ToolTip = new NamedToolTipInfo("#GAMEWORLD_REMOVESIDEKICK_BUTTON");
		}
		else
		{
			petButton.ToolTip = new NamedToolTipInfo(data.displayName);
		}
		petButton.HitTestSize = new Vector2(1f, 1f);
		petButton.HitTestType = HitTestTypeEnum.Circular;
		petButton.Click += changePet;
		petButton.petID = petID;
		scrollPane.Add(petButton);
		buttons.Add(petButton);
		rowCount++;
		if (rowCount >= rowCountMax)
		{
			rowCount = 0;
			currX = 0;
			currY += deltY;
		}
		else
		{
			currX += deltX;
		}
	}

	protected void changePet(GUIControl sender, GUIClickEvent EventData)
	{
		if (sender is PetButton)
		{
			PetDataManager.changeCurrentPet((sender as PetButton).petID);
			AppShell.Instance.StartCoroutine(ToggleIconLocks());
			PetDataManager.submitSidekickInfo();
			AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "generic_event", "set_sidekick", 1, (sender as PetButton).petID, -10000, string.Empty, string.Empty);
		}
		else
		{
			CspUtils.DebugLog("Got changePet from a control that isn't a pet button?");
		}
	}

	protected void setIconLock(bool locked)
	{
		foreach (PetButton button in buttons)
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
