using System.Collections.Generic;

public class NewTutorialManager
{
	private static int masterTutorialStep;

	private static string activeTutorialID;

	private static Dictionary<string, Tutorial> _tutorials;

	private static NewTutorialManager _instance;

	private static NewTutorialDialog _dlg;

	static NewTutorialManager()
	{
		masterTutorialStep = 0;
		activeTutorialID = string.Empty;
		_tutorials = new Dictionary<string, Tutorial>();
		_instance = new NewTutorialManager();
		_tutorials["master"] = new Tutorial_Master(_instance);
	}

	public static void setDlg(NewTutorialDialog dlg)
	{
		if (_dlg != null)
		{
			_dlg.IsVisible = false;
		}
		_dlg = null;
		_dlg = dlg;
	}

	public static NewTutorialDialog getDlg()
	{
		return _dlg;
	}

	public static bool allowDailyRewardWindow()
	{
		return false;
	}

	public static bool allowFullScreenWindow()
	{
		return false;
	}

	public static void playerSpawnBegun()
	{
		if (FeaturesManager.featureEnabled("tahiti") && activeTutorialID == string.Empty)
		{
			activeTutorialID = "master";
			_tutorials[activeTutorialID].begin();
		}
	}

	public void tutorialComplete(Tutorial target)
	{
	}

	public static void clearFlags()
	{
	}

	public static void sendEvent(TutorialEvent.TutorialEventType type)
	{
		AppShell.Instance.EventMgr.Fire(_instance, new TutorialEvent(type));
	}
}
