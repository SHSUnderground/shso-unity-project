using UnityEngine;

public class GoToZone : AutomationCmd
{
	public string zone;

	public GoToZone(string cmdline, string zn)
		: base(cmdline)
	{
		/////// block added by CSP to free up memory ////////////////
		GameObject go = GameObject.Find("static_bundles");
		if (go != null) {
		  GameObject.Destroy(go);
		  CspUtils.DebugLog("SHSBrawlerGadget destroying static_bundles!");
		}

		garbageCollect();
		////////////////////////////////////
		zone = zn;
		AutomationManager.Instance.nGameWorld++;
	}

	static void garbageCollect () {		// method added by CSP

		Resources.UnloadUnusedAssets();
		System.GC.Collect();
	}

	public override bool execute()
	{
		bool flag = base.execute();
		if (flag)
		{
			string value = null;
			if (GameController.GetController() != null && GameController.GetController().LocalPlayer != null)
			{
				value = GameController.GetController().LocalPlayer.name;
			}
			AppShell.Instance.SharedHashTable["SocialSpaceLevel"] = zone;
			AppShell.Instance.SharedHashTable["SocialSpaceSpawnPoint"] = null;
			AppShell.Instance.SharedHashTable["SocialSpaceCharacter"] = value;
			AppShell.Instance.Transition(GameController.ControllerType.SocialSpace);
		}
		return flag;
	}
}
