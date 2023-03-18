using System.Collections.Generic;

public class VOQueueMixer : IVOMixer
{
	protected Queue<IVOMixerItem> items = new Queue<IVOMixerItem>();

	protected IVOMixer output;

	public virtual void Update()
	{
		if (items.Count <= 0)
		{
			return;
		}
		IVOMixerItem iVOMixerItem = items.Peek();
		if (iVOMixerItem.Finished)
		{
			items.Dequeue();
			if (items.Count > 0)
			{
				SendToOutput(items.Peek());
			}
		}
	}

	public void SetOutput(IVOMixer output)
	{
		this.output = output;
	}

	public virtual void SendVO(IVOMixerItem item)
	{
		if (items.Count == 0)
		{
			SendToOutput(item);
		}
		items.Enqueue(item);
	}

	public IVOMixerItem Peek()
	{
		object result;
		if (items.Count > 0)
		{
			IVOMixerItem iVOMixerItem = items.Peek();
			result = iVOMixerItem;
		}
		else
		{
			result = null;
		}
		return (IVOMixerItem)result;
	}

	public void Cancel(ResolvedVOAction action)
	{
		Queue<IVOMixerItem> queue = new Queue<IVOMixerItem>();
		while (items.Count > 0)
		{
			IVOMixerItem iVOMixerItem = items.Dequeue();
			if (iVOMixerItem.Action == action)
			{
				iVOMixerItem.Cancel();
			}
			else
			{
				queue.Enqueue(iVOMixerItem);
			}
		}
		items = queue;
	}

	public void Cancel()
	{
		while (items.Count > 0)
		{
			items.Dequeue().Cancel();
		}
	}

	public bool InUse()
	{
		return items.Count > 0 && !items.Peek().Finished;
	}

	protected void SendToOutput(IVOMixerItem item)
	{
		if (output != null)
		{
			output.SendVO(item);
		}
	}
}
