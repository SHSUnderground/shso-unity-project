public class VOMixerItem : IVOMixerItem
{
	protected ResolvedVOAction action;

	protected IResolvedVOActionHandler handler;

	protected bool started;

	protected bool finished;

	public ResolvedVOAction Action
	{
		get
		{
			return action;
		}
	}

	public bool Started
	{
		get
		{
			return started;
		}
	}

	public bool Finished
	{
		get
		{
			return finished;
		}
	}

	public VOMixerItem(ResolvedVOAction action, IResolvedVOActionHandler handler)
	{
		this.action = action;
		this.handler = handler;
	}

	public void Play()
	{
		if (!started && !finished)
		{
			started = true;
			if (handler != null)
			{
				handler.HandleResolvedVOAction(action, OnFinished);
			}
			else
			{
				OnFinished();
			}
		}
	}

	public void Cancel()
	{
		if (started && !finished && handler != null)
		{
			handler.CancelVOAction(action);
		}
		OnFinished();
	}

	protected void OnFinished()
	{
		if (!finished)
		{
			finished = true;
			if (action != null && action.OnFinished != null)
			{
				action.OnFinished(this);
			}
		}
	}
}
