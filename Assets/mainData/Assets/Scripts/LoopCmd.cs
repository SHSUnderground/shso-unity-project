using System.Collections.Generic;

public class LoopCmd : AutomationCmd
{
	private int iterations;

	public LoopCmd(string cmdline, int itr)
		: base(cmdline)
	{
		iterations = itr;
	}

	public override bool execute()
	{
		Queue<AutomationCmd> queue = new Queue<AutomationCmd>();
		foreach (AutomationCmd item in AutomationManager.Instance.commandsQ)
		{
			if (item.GetType().Equals(typeof(EndLoopCmd)))
			{
				break;
			}
			queue.Enqueue(item);
		}
		AutomationManager.Instance.commandsQ = IterateCmds(queue, iterations);
		AutomationManager.Instance.LoopIterations = iterations;
		return base.execute();
	}

	public Queue<AutomationCmd> IterateCmds(Queue<AutomationCmd> cmds, int count)
	{
		Queue<AutomationCmd> queue = new Queue<AutomationCmd>();
		while (count > 1)
		{
			foreach (AutomationCmd cmd in cmds)
			{
				queue.Enqueue(cmd);
			}
			count--;
		}
		foreach (AutomationCmd item in AutomationManager.Instance.commandsQ)
		{
			if (!item.GetType().Equals(typeof(EndLoopCmd)))
			{
				queue.Enqueue(item);
			}
		}
		List<string> list = new List<string>();
		foreach (AutomationCmd item2 in queue)
		{
			list.Add(item2.GetType().ToString());
		}
		return queue;
	}
}
