using System.Runtime.CompilerServices;
using UnityEngine;

public class VOEmitterMixer : IVOMixer
{
	protected VOQueueMixer queue = new VOQueueMixer();

	[CompilerGenerated]
	private GameObject _003CEmitter_003Ek__BackingField;

	[CompilerGenerated]
	private bool _003CLoggingEnabled_003Ek__BackingField;

	public GameObject Emitter
	{
		[CompilerGenerated]
		get
		{
			return _003CEmitter_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CEmitter_003Ek__BackingField = value;
		}
	}

	public bool LoggingEnabled
	{
		[CompilerGenerated]
		get
		{
			return _003CLoggingEnabled_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CLoggingEnabled_003Ek__BackingField = value;
		}
	}

	public VOEmitterMixer(GameObject emitter)
	{
		Emitter = emitter;
	}

	public void Update()
	{
		queue.Update();
	}

	public void SetOutput(IVOMixer output)
	{
		queue.SetOutput(output);
	}

	public void SendVO(IVOMixerItem item)
	{
		if (queue.InUse())
		{
			IVOMixerItem iVOMixerItem = queue.Peek();
			switch (item.Action.CompareTo(iVOMixerItem.Action))
			{
			case VORoutingInfo.ComparisonResolution.Replace:
				ReplaceCurrent(item);
				break;
			case VORoutingInfo.ComparisonResolution.Queue:
				QueueItem(item);
				break;
			default:
				Log("Ignoring <" + item.Action.VOAction.Name + ">");
				break;
			}
		}
		else
		{
			QueueItem(item);
		}
	}

	public void Cancel(ResolvedVOAction action)
	{
		Log("Canceling <" + action.VOAction.Name + ">");
		queue.Cancel(action);
	}

	public void Cancel()
	{
		Log("Canceling all items");
		queue.Cancel();
	}

	public bool InUse()
	{
		return queue.InUse();
	}

	protected void QueueItem(IVOMixerItem item)
	{
		Log("Queuing <" + item.Action.VOAction.Name + ">");
		queue.SendVO(item);
	}

	protected void ReplaceCurrent(IVOMixerItem item)
	{
		Log("Replacing queue with <" + item.Action.VOAction.Name + ">");
		queue.Cancel();
		queue.SendVO(item);
	}

	protected void Log(object message)
	{
		if (LoggingEnabled)
		{
			string arg = "Mixer [" + ((!(Emitter == null)) ? Emitter.name : "null") + "]: ";
			CspUtils.DebugLog(arg + message);
		}
	}
}
