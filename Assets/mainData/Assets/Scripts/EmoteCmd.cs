using System;

public class EmoteCmd : AutomationCmd
{
	private string emoteName;

	public EmoteCmd(string cmdline, string name)
		: base(cmdline)
	{
		emoteName = name;
		AutomationManager.Instance.nGameWorld++;
	}

	public override bool execute()
	{
		try
		{
			AutomationManager.Instance.LogAttribute("emoteName", emoteName);
			AppShell.Instance.EventMgr.Fire(AutomationManager.Instance.LocalPlayer, new EmoteMessage(AutomationManager.Instance.LocalPlayer, EmotesDefinition.Instance.GetEmoteByCommand(emoteName).id));
		}
		catch (Exception ex)
		{
			AutomationManager.Instance.errGameWorld++;
			base.ErrorCode = "E001";
			base.ErrorMsg += ex.Message;
		}
		return base.execute();
	}
}
