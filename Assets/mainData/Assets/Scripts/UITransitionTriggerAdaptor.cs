public class UITransitionTriggerAdaptor : UITriggerAdaptor
{
	public GameController.ControllerType TransitionTarget;

	public SpawnPointGroup returnSpawnGroup;

	protected override void OnConfirmed()
	{
		DialogInUse = false;
		SetReturnSpawnGroup();
		AppShell.Instance.Transition(TransitionTarget);
	}

	protected void SetReturnSpawnGroup()
	{
		if (returnSpawnGroup != null && !string.IsNullOrEmpty(returnSpawnGroup.group))
		{
			CspUtils.DebugLog("Setting return spawn point to: " + returnSpawnGroup.group);
			AppShell.Instance.SharedHashTable["SocialSpaceSpawnPoint"] = returnSpawnGroup.group;
		}
		else
		{
			CspUtils.DebugLog("HQ launcher does not have an associated Spawn Point Group");
			AppShell.Instance.SharedHashTable["SocialSpaceSpawnPoint"] = null;
		}
	}
}
