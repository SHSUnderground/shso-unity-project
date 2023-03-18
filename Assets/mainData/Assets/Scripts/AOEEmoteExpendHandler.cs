public class AOEEmoteExpendHandler : ExpendHandlerBase
{
	public override void OnExpendStart()
	{
		CspUtils.DebugLog("OnExpendStart for AOEEmoteExpendHandler " + ExpendableDefinition.Parameters[0].Value[0]);
		ForceEmoteMessage forceEmoteMessage = new ForceEmoteMessage(new GoNetId(GoNetId.PLAYER_ID_FLAG, AppShell.Instance.ServerConnection.GetGameUserId()));
		forceEmoteMessage.emoteID = int.Parse(string.Empty + ExpendableDefinition.Parameters[0].Value[0]);
		AppShell.Instance.ServerConnection.SendGameMsg(forceEmoteMessage);
	}

	public override void Update()
	{
		base.Update();
		if (State == ExpendHandlerState.Expending)
		{
			OnExpendComplete();
		}
	}
}
