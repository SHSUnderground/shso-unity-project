public class VerifyOpenChat : AutomationCmd
{
	private string chatMessage;

	private string roomId;

	private int playerId;

	public VerifyOpenChat(string cmdline)
		: base(cmdline)
	{
		chatMessage = cmdline.Substring("verifyopenchat".Length);
		playerId = -1;
	}

	public override bool isReady()
	{
		bool flag = base.isReady();
		if (flag)
		{
			flag = (GameController.ControllerType.SocialSpace == AutomationManager.Instance.activeController);
		}
		if (!flag)
		{
			base.ErrorCode = "P001";
			base.ErrorMsg = "Error - User must be in the gameworld before it can send a message! Active Controller: " + AutomationManager.Instance.activeController;
			AutomationManager.Instance.errGameWorld++;
		}
		return flag;
	}

	public override bool execute()
	{
		try
		{
			roomId = AppShell.Instance.ServerConnection.GetRoomName();
			playerId = AppShell.Instance.PlayerDictionary.GetPlayerId(AppShell.Instance.ServerConnection.GetGameUserId());
			AppShell.Instance.EventReporter.ReportOpenChatMessage(playerId, roomId, chatMessage);
		}
		catch
		{
			base.ErrorMsg = "Error - failed to send message!";
			base.ErrorCode = "E001";
			AutomationManager.Instance.errGameWorld++;
		}
		return base.execute();
	}

	public override bool isCompleted()
	{
		bool flag = base.isCompleted();
		string a = playerId + ":" + chatMessage;
		if (flag)
		{
			flag = (!AutomationManager.Instance.chatMessageReceived.Equals(string.Empty) && a == AutomationManager.Instance.chatMessageReceived);
		}
		else
		{
			base.ErrorMsg = "Error - message failed! [RoomId: " + roomId + " PlayerId: " + playerId + " Message: " + chatMessage + "]";
			base.ErrorCode = "C001";
			AutomationManager.Instance.errGameWorld++;
		}
		return flag;
	}
}
