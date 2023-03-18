using System;

public class ChangeRoomCmd : AutomationCmd
{
	private string roomId;

	private DateTime start;

	private int ELAPSED_TIME = 1;

	public ChangeRoomCmd(string cmdline, string room)
		: base(cmdline)
	{
		roomId = room;
		AutomationManager.Instance.nHeadQuarters++;
	}

	public override bool execute()
	{
		try
		{
			AutomationManager.Instance.LogAttribute("roomId", roomId);
			AppShell.Instance.EventMgr.Fire(null, new HQRoomChangeRequestMessage(roomId));
			start = DateTime.Now;
		}
		catch (Exception ex)
		{
			AutomationManager.Instance.errHeadQuareter++;
			base.ErrorCode = "E001";
			base.ErrorMsg += ex.Message;
		}
		return base.execute();
	}

	public override bool isCompleted()
	{
		bool flag = base.isCompleted();
		if (flag)
		{
			if (DateTime.Now - start > TimeSpan.FromSeconds(ELAPSED_TIME))
			{
				flag = (AutomationManager.Instance.HQRoomId == roomId);
				if (!flag)
				{
					base.ErrorMsg = "HQRoomId is not set to " + roomId;
				}
			}
			else
			{
				flag = false;
			}
		}
		else
		{
			AutomationManager.Instance.errHeadQuareter++;
			base.ErrorCode = "C001";
			base.ErrorMsg = "ChangeRoomCmd Timed Out";
		}
		return flag;
	}

	public override bool isReady()
	{
		bool flag = base.isReady();
		if (flag)
		{
			flag = (AutomationManager.Instance.activeController == GameController.ControllerType.HeadQuarters);
			if (!flag)
			{
				base.ErrorMsg = "ActiveController is not set to HeadQuarters";
			}
		}
		else
		{
			AutomationManager.Instance.errHeadQuareter++;
			base.ErrorCode = "P001";
			base.ErrorMsg = "ChangeRoomCmd Timed Out";
		}
		return flag;
	}
}
