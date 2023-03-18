using ShsAudio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoppingWindow : GUIDialogWindow
{
	public const float VO_DELAY_TIME = 2f;

	private GUIImage _background;

	private GUIImage _overlay;

	private ShoppingWindowTopPanel _topPanel;

	private ShoppingWindowLeftPanel _leftPanel;

	private ShoppingWindowDetailPanel _detailPanel;

	private ShoppingWindowContentPanel _contentPanel;

	private ShoppingWindowUpsellPanel _upsellPanel;

	private ShoppingWindowNotificationPanel _notificationPanel;

	public static ShoppingWindow instance;

	private GameObject voPlayer;

	protected static readonly string BACKGROUND_MUSIC_PREFAB_NAME = "Music_SHS_Mission_Complete_Loop_audio";

	private NewShoppingManager.ShoppingCategory _defaultCategory;

	private int _defaultOwnableID = -1;

	private List<int> _specifiedIDList;

	public GUIDialogNotificationSink.DialogEventNotificationDelegate doorCloseDelegate;

	public ShoppingWindow(NewShoppingManager.ShoppingCategory defaultCategory, List<int> specificIDs = null)
	{
		CspUtils.DebugLog("ShoppingWindow " + defaultCategory);
		_defaultCategory = defaultCategory;
		_specifiedIDList = specificIDs;
		init();
	}

	public ShoppingWindow(int ownableTypeID)
	{
		CspUtils.DebugLog("ShoppingWindow invoked with default ownable ID of " + ownableTypeID);
		_defaultOwnableID = ownableTypeID;
		init();
	}

	public ShoppingWindow()
	{
		if (RTCClient.paid == 0) {
			CspUtils.DebugLog("Only paid accounts can access shop!");
			throw new Exception();
		}
		init();
	}

	public void launch()
	{
		GUIManager.Instance.ShowDynamicWindow(this, ModalLevelEnum.Default);
	}

	public new void init()
	{
		instance = this;
		Traits.FullScreenOpaqueBackgroundTrait = ControlTraits.FullScreenOpaqueBackgroundTraitEnum.HasFullScreenOpaqueBackground;
		Traits.EventListenerRegistrationTrait = ControlTraits.EventListenerRegistrationTraitEnum.Ignore;
		SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		SetPosition(QuickSizingHint.Centered);
		_background = new GUIImage();
		_background.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		_background.Position = Vector2.zero;
		_background.TextureSource = "shopping_bundle|bg";
		_background.Id = "Background";
		Add(_background);
		_topPanel = new ShoppingWindowTopPanel();
		_topPanel.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft, Vector2.zero);
		_topPanel.IsVisible = true;
		Add(_topPanel);
		_leftPanel = new ShoppingWindowLeftPanel();
		_leftPanel.SetPosition(DockingAlignmentEnum.BottomLeft, AnchorAlignmentEnum.BottomLeft, new Vector2(0f, 0f));
		_leftPanel.IsVisible = true;
		Add(_leftPanel);
		_contentPanel = new ShoppingWindowContentPanel(this);
		_contentPanel.SetPosition(DockingAlignmentEnum.BottomMiddle, AnchorAlignmentEnum.BottomMiddle, new Vector2(-25f, 0f));
		_contentPanel.IsVisible = true;
		Add(_contentPanel);
		_detailPanel = new ShoppingWindowDetailPanel();
		_detailPanel.SetPosition(DockingAlignmentEnum.TopRight, AnchorAlignmentEnum.TopRight, new Vector2(0f, 70f));
		_detailPanel.IsVisible = true;
		Add(_detailPanel);
		_upsellPanel = new ShoppingWindowUpsellPanel();
		_upsellPanel.SetPosition(DockingAlignmentEnum.BottomRight, AnchorAlignmentEnum.BottomRight, new Vector2(0f, 0f));
		Add(_upsellPanel);
		_upsellPanel.IsVisible = true;
		_notificationPanel = new ShoppingWindowNotificationPanel();
		_notificationPanel.SetPosition(DockingAlignmentEnum.BottomRight, AnchorAlignmentEnum.BottomRight, new Vector2(0f, 0f));
		Add(_notificationPanel);
		_notificationPanel.IsVisible = false;
		_notificationPanel.Alpha = 0f;
		GUIButton gUIButton = GUIControl.CreateControlTopLeftFrame<GUIButton>(new Vector2(39f, 40f), new Vector2(972f, 9f));
		gUIButton.StyleInfo = new SHSButtonStyleInfo("shopping_bundle|closebutton", SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight);
		gUIButton.Click += delegate
		{
			Close();
		};
		gUIButton.ToolTip = new NamedToolTipInfo("#TT_COMMON_1");
		Add(gUIButton);
		_overlay = new GUIImage();
		_overlay.SetSize(new Vector2(1f, 1f), AutoSizeTypeEnum.Percentage, AutoSizeTypeEnum.Percentage);
		_overlay.Position = Vector2.zero;
		_overlay.TextureSource = "shopping_bundle|overlay";
		_overlay.Id = "Overlay";
		Add(_overlay);
		_overlay.Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		_overlay.Traits.HitTestType = HitTestTypeEnum.Transparent;
		_overlay.SetControlFlag(ControlFlagSetting.HitTestIgnore, true, false);
		CardManager.LoadTextureBundle(true);
		SHSInput.SetInputBlockingMode(this, SHSInput.InputBlockType.BlockWorld);
		AppShell.Instance.EventMgr.Fire(this, new CloseHudMessage());
		PlayerStatus.SetLocalStatus(PlayerStatusDefinition.Instance.GetStatus("Shopping"));
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("global_window_in"));
		PlayBackgroundMusic();
		PlayVO();
		ShsWebService.SafeJavaScriptCall("HEROUPNS.TrackGameStoreOpen();");
		if (_defaultOwnableID > 0)
		{
			OwnableDefinition def = OwnableDefinition.getDef(_defaultOwnableID);
			if (def != null)
			{
				NewShoppingManager.ShoppingCategory defaultCategory = NewShoppingManager.itemIDToCategory(_defaultOwnableID);
				if (def.subscriberOnly == 1)
				{
					_defaultCategory = NewShoppingManager.ShoppingCategory.AgentOnly;
				}
				else
				{
					_defaultCategory = defaultCategory;
				}
			}
		}
		List<CatalogItem> list = NewShoppingManager.getList(_defaultCategory);
		if (list.Count == 0)
		{
			_defaultCategory = NewShoppingManager.ShoppingCategory.Hero;
		}
		showCategory(_defaultCategory);
		filterByIDs(_specifiedIDList);
		SetControlFlag(ControlFlagSetting.AlphaCascade, true, true);
		AppShell.Instance.EventReporter.ReportAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "generic_event", "open_store", 1, string.Empty);
	}

	public void displayError(string title, string message)
	{
		_notificationPanel.displayMessage(title, message);
	}

	public void displayBuyGold()
	{
		_notificationPanel.displayBuyGold();
	}

	public void displayBuyMembership()
	{
		_notificationPanel.displayBuyMembership();
	}

	public void displayGetFractals()
	{
		_notificationPanel.displayGetFractals();
	}

	public void selectTab(ShoppingTabButton button)
	{
		CspUtils.DebugLog(button.tabName);
		_contentPanel.selectTab(button);
	}

	public void resetError()
	{
		_notificationPanel.Hide();
	}

	public void upsellClick(CatalogItem item)
	{
		_detailPanel.setCatalogItem(item);
	}

	public void buyMembership()
	{
		LauncherSequences.InitiateLaunchSequence(LauncherTypeEnum.Subscribe);
	}

	public void filterByText(string text)
	{
		_contentPanel.filterByText(text);
	}

	public void filterByIDs(List<int> specificIDs)
	{
		_contentPanel.filterByIDs(specificIDs);
	}

	public void buyFractals()
	{
		showOwnable(303943);
	}

	public void buyGold()
	{
		LauncherSequences.InitiateLaunchSequence(LauncherTypeEnum.BuyGold);
	}

	public void Close()
	{
		IsVisible = false;
		if (PlayerStatus.GetLocalStatus() == PlayerStatusDefinition.Instance.GetStatus("Shopping"))
		{
			PlayerStatus.ClearLocalStatus();
		}
		AppShell.Instance.AudioManager.RequestCrossfade(null);
		ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("global_window_out"));
		if (voPlayer != null)
		{
			UnityEngine.Object.Destroy(voPlayer);
		}
		if (doorCloseDelegate != null)
		{
			doorCloseDelegate(string.Empty, DialogState.Ok);
			doorCloseDelegate = null;
		}
		SHSInput.RevertInputBlockingMode(this);
		instance = null;
		OnHide();
		Dispose();
	}

	protected void PlayVO()
	{
		voPlayer = CoroutineContainer.Spawn("shopping_vo_container", CoPlayVO());
	}

	protected IEnumerator CoPlayVO()
	{
		yield return new WaitForSeconds(2f);
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		if (localPlayer != null)
		{
			VOManager.Instance.PlayVO("going_shopping", localPlayer);
		}
	}

	protected void PlayBackgroundMusic()
	{
		AppShell.Instance.BundleLoader.FetchAssetBundle(Helpers.GetAudioBundleName(SABundle.Shopping), OnShoppingBundleLoaded, null, true);
	}

	private void OnShoppingBundleLoaded(AssetBundleLoadResponse response, object extraData)
	{
		if (response.Error != null)
		{
			CspUtils.DebugLog("Error while loading the shopping asset bundle <" + response.Path + ">: " + response.Error);
			return;
		}
		GameObject gameObject = response.Bundle.Load(BACKGROUND_MUSIC_PREFAB_NAME) as GameObject;
		if (gameObject != null && IsVisible)
		{
			ShsAudioSource.PlayAutoSound(gameObject);
		}
	}

	public void showCategory(NewShoppingManager.ShoppingCategory category)
	{
		resetError();
		_contentPanel.setMainCategory(category);
		_topPanel.resetSearch();
		if (_defaultOwnableID > 0)
		{
			_contentPanel.scrollToItem(_defaultOwnableID);
			_defaultOwnableID = 0;
		}
		if (_contentPanel.firstItem != null)
		{
			_detailPanel.setCatalogItem(_contentPanel.firstItem);
		}
		_upsellPanel.IsVisible = true;
	}

	public void showOwnable(int ownableTypeID)
	{
		OwnableDefinition def = OwnableDefinition.getDef(ownableTypeID);
		showCategory(NewShoppingManager.ownableCategoryToShoppingCategory(def.category));
		_contentPanel.scrollToItem(ownableTypeID);
	}

	public void selectCatalogItem(CatalogItem item)
	{
		resetError();
		_detailPanel.setCatalogItem(item);
	}

	public static Vector2 constrain(Vector2 actualSize, Vector2 maxSize)
	{
		if ((maxSize.x == -1f || actualSize.x <= maxSize.x) && (maxSize.y == -1f || actualSize.y <= maxSize.y))
		{
			return actualSize;
		}
		float val = actualSize.x / maxSize.x;
		float val2 = actualSize.y / maxSize.y;
		float num = Math.Max(val, val2);
		return new Vector2((float)(int)actualSize.x / num, (float)(int)actualSize.y / num);
	}

	public static string convertNumber(int number)
	{
		if (number < 1000)
		{
			return string.Empty + number;
		}
		string arg = string.Empty;
		if (number > 999999)
		{
			arg = arg + number / 1000000 + ",";
			number %= 1000000;
		}
		arg = arg + number / 1000 % 1000 + ",";
		int num = number % 1000;
		if (num < 100)
		{
			arg += "0";
		}
		if (num < 10)
		{
			arg += "0";
		}
		return arg + num;
	}
}
