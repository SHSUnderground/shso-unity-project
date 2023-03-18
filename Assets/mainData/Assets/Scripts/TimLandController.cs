public class TimLandController : GameController
{
	private bool hackzoneloaded;

	public override void Start()
	{
		base.Start();
	}

	private void Update()
	{
		if (!hackzoneloaded)
		{
			AppShell.Instance.EventMgr.Fire(this, new ZoneLoadedMessage("TimTown"));
			hackzoneloaded = true;
		}
	}

	public override void OnOldControllerUnloading(AppShell.GameControllerTypeData currentGameData, AppShell.GameControllerTypeData newGameData)
	{
		base.OnOldControllerUnloading(currentGameData, newGameData);
	}
}
