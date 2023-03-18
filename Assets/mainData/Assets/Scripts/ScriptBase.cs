public class ScriptBase
{
	private Tutorial _parent;

	public ScriptBase(Tutorial parentTutorial)
	{
		_parent = parentTutorial;
	}

	public virtual void activate()
	{
	}

	public virtual void tick(float delta)
	{
	}

	public virtual void complete()
	{
		_parent.scriptComplete(this);
	}

	protected Tutorial getParent()
	{
		return _parent;
	}

	public void dispatchEvent(ShsEventMessage msg)
	{
		AppShell.Instance.EventMgr.Fire(this, msg);
	}

	public virtual void interrupt()
	{
	}
}
