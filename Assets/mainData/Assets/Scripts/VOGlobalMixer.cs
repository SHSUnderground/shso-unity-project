using System.Collections.Generic;

public class VOGlobalMixer : IVOMixer
{
	protected LinkedList<IVOMixerItem> items = new LinkedList<IVOMixerItem>();

	public void Update()
	{
		bool flag = false;
		foreach (LinkedListNode<IVOMixerItem> item in Utils.RemovableEnumerate(items))
		{
			IVOMixerItem value = item.Value;
			if (!value.Started)
			{
				if (flag)
				{
					items.Remove(item);
				}
				else
				{
					value.Play();
				}
			}
			if (value.Finished)
			{
				items.Remove(item);
			}
			if (value.Started && !value.Finished && value.Action.Routing.important)
			{
				flag = true;
			}
		}
	}

	public void SetOutput(IVOMixer output)
	{
		throw new InvalidVOOutputException("The VO global mixer cannot be assigned an output.");
	}

	public void SendVO(IVOMixerItem item)
	{
		if (item.Action.Routing.important)
		{
			StopAll();
		}
		items.AddLast(item);
	}

	public void StopAll()
	{
		foreach (LinkedListNode<IVOMixerItem> item in Utils.RemovableEnumerate(items))
		{
			IVOMixerItem value = item.Value;
			if (!value.Started)
			{
				items.Remove(item);
			}
			else if (!value.Finished)
			{
				value.Cancel();
			}
		}
	}
}
