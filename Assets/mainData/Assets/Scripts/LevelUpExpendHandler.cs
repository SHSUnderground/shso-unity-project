public class LevelUpExpendHandler : ExpendHandlerBase
{
	private bool receivedLevelUpComplete;

	public override void Initialize(int requestId, ExpendableDefinition definition, ExpendablesManager.ExpendHandlerCompleteCallback managerCallback, ExpendablesManager.ExpendHandlerCompleteCallback onCompleteCallback)
	{
		base.Initialize(requestId, definition, managerCallback, onCompleteCallback);
		AppShell.Instance.EventMgr.AddListener<LeveledUpMessage>(OnLevelUp);
	}

	private void OnLevelUp(LeveledUpMessage arg)
	{
		LogExpendAction("Got Level Up message expected for hero:" + arg.Hero);
		receivedLevelUpComplete = true;
	}

	public override void OnExpendComplete()
	{
		base.OnExpendComplete();
		AppShell.Instance.EventMgr.RemoveListener<LeveledUpMessage>(OnLevelUp);
	}

	public override void Update()
	{
		base.Update();
		if (State == ExpendHandlerState.Expending && receivedLevelUpComplete)
		{
			OnExpendComplete();
		}
	}
}
