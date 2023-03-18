public class SHSBrawlerAirlockHostWindow : GUIControlWindow
{
	public const int NO_VALUE = -1;

	public const string NO_VALUE_S = "";

	public override void OnActive()
	{
		AppShell.Instance.EventMgr.AddListener<CharacterSelectedMessage>(BeginGame);
		if (AppShell.Instance.SharedHashTable["BrawlerAirlockPlaySolo"] != null && (bool)AppShell.Instance.SharedHashTable["BrawlerAirlockPlaySolo"])
		{
			AppShell.Instance.SharedHashTable["BrawlerAirlockPlaySolo"] = null;
			AppShell.Instance.SharedHashTable["GUIHudPlaySolo"] = true;
			UserProfile profile = AppShell.Instance.Profile;
			AppShell.Instance.EventMgr.Fire(null, new CharacterRequestedMessage(new CharacterSelectionBlock(profile.LastSelectedCostume, profile.LastSelectedPower)));
			CharacterSelectedMessage msg = new CharacterSelectedMessage(new CharacterSelectionBlock(profile.LastSelectedCostume, profile.LastSelectedPower));
			AppShell.Instance.EventMgr.Fire(null, msg);
		}
		else
		{
			AppShell.Instance.SharedHashTable["BrawlerAirlockPlaySolo"] = null;
			AppShell.Instance.SharedHashTable["GUIHudPlaySolo"] = false;
			GUIManager.Instance.ShowDialog(typeof(SHSBrawlerAirlockGadget), null, ModalLevelEnum.Default);
		}
		base.OnActive();
	}

	private void BeginGame(CharacterSelectedMessage message)
	{
		AppShell.Instance.EventMgr.RemoveListener<CharacterSelectedMessage>(BeginGame);
	}
}
