using System.Collections.Generic;

public class ImportCmd : AutomationCmd
{
	private string scriptfile;

	public ImportCmd(string cmdline, string script)
		: base(cmdline)
	{
		scriptfile = AutomationManager.Instance.subScriptPath + script + ".txt";
		CspUtils.DebugLog(scriptfile);
	}

	public override bool execute()
	{
		string[] array = AutomationManager.Instance.LoadCommandScriptFile(scriptfile);
		Queue<AutomationCmd> queue = new Queue<AutomationCmd>();
		string[] array2 = array;
		foreach (string text in array2)
		{
			CspUtils.DebugLog("ScriptLine: " + text);
			string text2 = text.Trim();
			if (text2[0] != '#' && text2.Length > 0)
			{
				AutomationCmd automationCmd = null;
				automationCmd = AutomationManager.Instance.CmdFactory(text2);
				if (automationCmd != null)
				{
					CspUtils.DebugLog("adding " + text2);
					queue.Enqueue(automationCmd);
				}
				else
				{
					CspUtils.DebugLog("Error with " + text2);
				}
			}
		}
		while (AutomationManager.Instance.commandsQ.Count > 0)
		{
			queue.Enqueue(AutomationManager.Instance.commandsQ.Dequeue());
		}
		AutomationManager.Instance.commandsQ = queue;
		return base.execute();
	}
}
