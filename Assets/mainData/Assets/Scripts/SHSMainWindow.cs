using UnityEngine;

public class SHSMainWindow : GUITopLevelWindow
{
	private readonly SHSHudWheels hudWheels;

	private NewEmoteChatBar emoteChatBar;

	public SHSMainWindow(string name)
		: base(name)
	{
		HitTestType = HitTestTypeEnum.Transparent;
		BlockTestType = BlockTestTypeEnum.Rect;
		MenuChatMainWindow control = new MenuChatMainWindow();
		Add(control, DrawOrder.DrawFirst, DrawPhaseHintEnum.PostDraw);
		ChangeHudButtonsAndTraitsMessage changeHudButtonsAndTraitsMessage = new ChangeHudButtonsAndTraitsMessage();
		changeHudButtonsAndTraitsMessage.EnableAllBut(default(SHSHudWheels.ButtonType));
		AppShell.Instance.EventMgr.Fire(null, changeHudButtonsAndTraitsMessage);
		AppShell.Instance.EventMgr.AddListener<ShowEmoteBarSettingMessage>(OnShowEmoteBarSettingMessage);
		AppShell.Instance.EventMgr.AddListener<FeatureEnabledEvent>(OnFeatureEnabledEvent);
	}

	private void OnFeatureEnabledEvent(FeatureEnabledEvent msg)
	{
		if (hudWheels != null)
		{
			hudWheels.Dispose();
			Remove(hudWheels);
		}
	}

	private void OnShowEmoteBarSettingMessage(ShowEmoteBarSettingMessage msg)
	{
		if (ShsPlayerPrefs.GetInt(ShsPlayerPrefs.Keys.ShowEmoteBar, 1) == 0)
		{
			if (emoteChatBar != null)
			{
				Remove(emoteChatBar);
				emoteChatBar.Dispose();
				emoteChatBar = null;
			}
		}
		else if (!(GameController.GetController() is CardGameController) && !(GameController.GetController() is BrawlerController) && emoteChatBar == null)
		{
			emoteChatBar = new NewEmoteChatBar();
			Add(emoteChatBar, DrawOrder.DrawFirst, DrawPhaseHintEnum.PostDraw);
			emoteChatBar.IsVisible = true;
		}
	}

	private void button_Click(GUIControl sender, GUIClickEvent EventData)
	{
		AppShell.Instance.EventMgr.Fire(this, new MenuChatActivateMessage());
	}

	protected override void InitializeBundleList()
	{
		base.InitializeBundleList();
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("persistent_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("hud_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("arcade_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("missions_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
		supportingAssetBundles.Add(new SupportingAssetBundleInfo("missionflyers_bundle", SupportingAssetBundleInfoTypeEnum.BlockUntilLoaded));
	}

	public override void OnActive()
	{
		base.OnActive();
		if (emoteChatBar != null)
		{
			Remove(emoteChatBar);
			emoteChatBar.Dispose();
			emoteChatBar = null;
		}
		if ((Application.loadedLevelName == "Audio Tester" || Application.loadedLevelName == "basic attack" || Application.loadedLevelName == "Basic Attack" || GameController.GetController() is SocialSpaceController) && ShsPlayerPrefs.GetInt(ShsPlayerPrefs.Keys.ShowEmoteBar, 1) != 0)
		{
			emoteChatBar = new NewEmoteChatBar();
			Add(emoteChatBar, DrawOrder.DrawFirst, DrawPhaseHintEnum.PostDraw);
			emoteChatBar.IsVisible = true;
		}
	}

	public override void ConfigureKeyBanks()
	{
		base.ConfigureKeyBanks();
		keyBanks[KeyInputState.Active].AddKey(new KeyCodeEntry(KeyCode.U, false, true, false), OnUICursorToggle);
		keyBanks[KeyInputState.Active].AddKey(new KeyCodeEntry(KeyCode.U, false, true, true), OnUIToggle);
		keyBanks[KeyInputState.Active].AddKey(new KeyCodeEntry(KeyCode.U, true, true, false), OnEmoteBarToggle);
	}

	public void OnEmoteBarToggle(SHSKeyCode code)
	{
		ShsPlayerPrefs.SetInt(ShsPlayerPrefs.Keys.ShowEmoteBar, (ShsPlayerPrefs.GetInt(ShsPlayerPrefs.Keys.ShowEmoteBar, 1) != 1) ? 1 : 0);
		OnShowEmoteBarSettingMessage(null);
	}

	public void OnUIToggle(SHSKeyCode code)
	{
		GUIManager.Instance.DrawingEnabled = !GUIManager.Instance.DrawingEnabled;
		if (GUIManager.Instance.DrawingEnabled)
		{
			GUIManager.Instance.CursorManager.CursorEnabled = true;
		}
	}

	public void OnUICursorToggle(SHSKeyCode code)
	{
		GUIManager.Instance.DrawingEnabled = !GUIManager.Instance.DrawingEnabled;
		GUIManager.Instance.CursorManager.CursorEnabled = GUIManager.Instance.DrawingEnabled;
	}

	public void OnEnter(SHSKeyCode code)
	{
		if (!GUIManager.Instance[GUIManager.UILayer.Debug, "SHSDebugWindow/SHSDebugConsoleWindow"].IsVisible)
		{
		}
	}
}
