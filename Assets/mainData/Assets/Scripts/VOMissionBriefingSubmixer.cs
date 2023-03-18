public class VOMissionBriefingSubmixer : VOQueueMixer
{
	private bool waitingForAppShell = true;

	private bool openToRelationships;

	private IVOMixerItem storedRelationship;

	public void ReleasePendingRelationship()
	{
		openToRelationships = true;
		if (storedRelationship != null)
		{
			base.SendVO(storedRelationship);
			storedRelationship = null;
		}
	}

	public override void Update()
	{
		base.Update();
		if (waitingForAppShell && AppShell.Instance != null && AppShell.Instance.EventMgr != null)
		{
			AppShell.Instance.EventMgr.AddListener<BrawlerStartedMessage>(OnBrawlerStartedMessage);
			waitingForAppShell = false;
		}
	}

	public override void SendVO(IVOMixerItem item)
	{
		if (item.Action.VOAction.Name == "relationships")
		{
			if (openToRelationships)
			{
				base.SendVO(item);
			}
			else
			{
				storedRelationship = item;
			}
		}
		else if (item.Action.VOAction.Name == "mission_briefing")
		{
			base.SendVO(item);
			ReleasePendingRelationship();
		}
	}

	protected void OnBrawlerStartedMessage(BrawlerStartedMessage e)
	{
		openToRelationships = false;
		storedRelationship = null;
	}
}
