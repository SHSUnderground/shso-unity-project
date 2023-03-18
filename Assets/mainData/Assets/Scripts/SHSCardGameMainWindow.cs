using UnityEngine;

public class SHSCardGameMainWindow : GUITopLevelWindow
{
	private const float HUD_SCALAR = 0.75f;

	private const float NUDGE_Y = 318f;

	protected ShsEventMgr eventMgr;

	private SHSCardGameBlockIcon leftBlockIcon;

	private SHSCardGameBlockIcon rightBlockIcon;

	private GUISimpleControlWindow _globalNavAnchor;

	private GlobalNav _globalNav;

	private SHSCardGameDamageIndicator leftDamageIndicator;

	private SHSCardGameDamageIndicator rightDamageIndicator;

	private SHSCardGamePassButton passButton;

	private SHSCardGameCompleteWindow cardCompletePanel;

	private GUIDrawTexture fullCardDetails;

	private SHSCardGameDealerChatWindow dealerChatWindow;

	private SHSCardGamePowerIndicator powerIndicator;

	private SHSCardGamePokeButton pokeButton;

	private bool pokeButtonShouldBeVisible;

	public SHSCardGameMainWindow()
		: base("SHSCardGameMainWindow")
	{
		Id = "SHSCardGameMainWindow";
		eventMgr = AppShell.Instance.EventMgr;
		passButton = new SHSCardGamePassButton();
		passButton.SetPosition(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(63f, -287f));
		Add(passButton);
		pokeButton = new SHSCardGamePokeButton();
		pokeButton.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-48f, 141f));
		Add(pokeButton);
		leftBlockIcon = new SHSCardGameBlockIcon();
		leftBlockIcon.playerIndex = 0;
		Add(leftBlockIcon);
		rightBlockIcon = new SHSCardGameBlockIcon();
		rightBlockIcon.playerIndex = 1;
		Add(rightBlockIcon);
		leftDamageIndicator = new SHSCardGameDamageIndicator();
		leftDamageIndicator.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(130f, 380f));
		leftDamageIndicator.playerIndex = 0;
		Add(leftDamageIndicator);
		rightDamageIndicator = new SHSCardGameDamageIndicator();
		rightDamageIndicator.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.Middle, OffsetType.Absolute, new Vector2(-128f, 148f));
		rightDamageIndicator.playerIndex = 1;
		Add(rightDamageIndicator);
		dealerChatWindow = new SHSCardGameDealerChatWindow();
		Add(dealerChatWindow);
		powerIndicator = new SHSCardGamePowerIndicator();
		Add(powerIndicator);
		cardCompletePanel = new SHSCardGameCompleteWindow();
		cardCompletePanel.Traits.ActivationTrait = ControlTraits.ActivationTraitEnum.Auto;
		cardCompletePanel.SetPositionAndSize(Vector2.zero, DockingAlignmentEnum.Middle, AnchorAlignmentEnum.Middle, OffsetType.Absolute, Vector2.zero, new Vector2(786f, 570f), AutoSizeTypeEnum.Absolute, AutoSizeTypeEnum.Absolute);
		Add(cardCompletePanel);
		fullCardDetails = GUIControl.CreateControlTopFrame<GUIDrawTexture>(new Vector2(366f, 512f), new Vector2(0f, 0f));
		fullCardDetails.Traits.HitTestType = HitTestTypeEnum.Transparent;
		fullCardDetails.Texture = null;
		fullCardDetails.Anchor = AnchorAlignmentEnum.TopLeft;
		Add(fullCardDetails);
		_globalNav = new GlobalNav(true, true);
		_globalNav.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, Vector2.zero);
		_globalNavAnchor = GUIControl.CreateControlBottomRightFrame<GUISimpleControlWindow>(_globalNav.Size, new Vector2(0f, 0f));
		Add(_globalNavAnchor);
		_globalNavAnchor.IsVisible = true;
		_globalNavAnchor.Add(_globalNav);
		_globalNav.hideCurrency();
	}

	public override void ConfigureKeyBanks()
	{
		base.ConfigureKeyBanks();
		keyBanks[KeyInputState.Active].AddKey(new KeyCodeEntry(KeyCode.U, false, true, true), OnUIToggle);
	}

	private void OnUIToggle(SHSKeyCode code)
	{
		CardGameController.Instance.Hud.Camera.enabled = !CardGameController.Instance.Hud.Camera.enabled;
	}

	private void SetFullCardDetails(CardGameEvent.FullCardDetails evt)
	{
		fullCardDetails.Texture = evt.textureSource;
		if (evt.leftSide)
		{
			fullCardDetails.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, OffsetType.Absolute, Vector2.zero);
		}
		else
		{
			fullCardDetails.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, OffsetType.Absolute, Vector2.zero);
		}
	}

	protected void OnEnablePassButton(CardGameEvent.EnablePassButton evt)
	{
		passButton.ShowPass(evt.pickCardType, evt.ButtonType);
	}

	protected void OnDisablePassButton(CardGameEvent.DisablePassButton evt)
	{
		passButton.IsVisible = false;
		dealerChatWindow.FadeOut(0.3f);
	}

	protected void OnShowCardGameHud(CardGameEvent.ShowCardGameHud evt)
	{
		GUIManager.Instance.TogglePersistentGUI(GUIManager.PersistentGUIToggleOption.Visibility, false);
	}

	protected void OnShowPokeButton(CardGameEvent.ShowPokeButton evt)
	{
		pokeButtonShouldBeVisible = true;
		pokeButton.IsVisible = true;
	}

	protected void OnEnablePokeButton(CardGameEvent.EnablePokeButton evt)
	{
		if (pokeButtonShouldBeVisible)
		{
			pokeButton.PokingIsAllowed = evt.shouldBeEnabled;
		}
	}

	protected void OnHidePokeButton(CardGameEvent.HidePokeButton evt)
	{
		pokeButton.IsVisible = false;
	}

	protected override void InitializeBundleList()
	{
		base.InitializeBundleList();
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("cardgame_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("brawler_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
	}

	public override void OnShow()
	{
		GUIManager.Instance.TogglePersistentGUI(GUIManager.PersistentGUIToggleOption.Visibility, true);
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.ShowCardGameHud>(OnShowCardGameHud);
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.ShowPokeButton>(OnShowPokeButton);
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.EnablePokeButton>(OnEnablePokeButton);
		AppShell.Instance.EventMgr.AddListener<CardGameEvent.HidePokeButton>(OnHidePokeButton);
		base.OnShow();
	}

	public override void OnHide()
	{
		GUIManager.Instance.TogglePersistentGUI(GUIManager.PersistentGUIToggleOption.Visibility, false);
		AppShell.Instance.EventMgr.RemoveListener<CardGameEvent.ShowCardGameHud>(OnShowCardGameHud);
		AppShell.Instance.EventMgr.RemoveListener<CardGameEvent.ShowPokeButton>(OnShowPokeButton);
		AppShell.Instance.EventMgr.RemoveListener<CardGameEvent.EnablePokeButton>(OnEnablePokeButton);
		AppShell.Instance.EventMgr.RemoveListener<CardGameEvent.HidePokeButton>(OnHidePokeButton);
		base.OnHide();
	}

	public override void OnActive()
	{
		base.OnActive();
		UserProfile profile = AppShell.Instance.Profile;
		if (profile == null)
		{
			CspUtils.DebugLog("No profile. Offline? Controller is:" + GameController.GetController());
			return;
		}
		eventMgr.AddListener<CardGameEvent.FullCardDetails>(SetFullCardDetails);
		eventMgr.AddListener<CardGameEvent.EnablePassButton>(OnEnablePassButton);
		eventMgr.AddListener<CardGameEvent.DisablePassButton>(OnDisablePassButton);
	}

	public override void OnInactive()
	{
		base.OnInactive();
		eventMgr.RemoveListener<CardGameEvent.FullCardDetails>(SetFullCardDetails);
		eventMgr.RemoveListener<CardGameEvent.EnablePassButton>(OnEnablePassButton);
		eventMgr.RemoveListener<CardGameEvent.DisablePassButton>(OnDisablePassButton);
	}

	public void ShowDealerChat()
	{
		if (!string.IsNullOrEmpty(dealerChatWindow.Text))
		{
			dealerChatWindow.IsVisible = true;
			dealerChatWindow.Alpha = 1f;
		}
	}

	public void RetreatHud()
	{
		dealerChatWindow.IsVisible = false;
		powerIndicator.IsVisible = false;
		pokeButton.IsVisible = false;
		GUIManager.Instance.TogglePersistentGUI(GUIManager.PersistentGUIToggleOption.Visibility, true);
	}

	public void AdvanceHud()
	{
		dealerChatWindow.IsVisible = true;
		powerIndicator.IsVisible = true;
		if (pokeButtonShouldBeVisible)
		{
			pokeButton.IsVisible = true;
		}
		GUIManager.Instance.TogglePersistentGUI(GUIManager.PersistentGUIToggleOption.Visibility, false);
	}

	public void HideHudElements()
	{
		dealerChatWindow.Hide();
		powerIndicator.Hide();
		passButton.Hide();
		pokeButton.Hide();
	}
}
