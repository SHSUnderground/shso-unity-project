using System;

public class ScriptWaitForEvent : ScriptBase
{
	protected TutorialEvent.TutorialEventType _targetType = TutorialEvent.TutorialEventType.MAX;

	protected Action<TutorialEvent> _callback;

	public ScriptWaitForEvent(Tutorial parent, TutorialEvent.TutorialEventType targetType)
		: base(parent)
	{
		_targetType = targetType;
	}

	public ScriptWaitForEvent(Tutorial parent, Action<TutorialEvent> callback = null)
		: base(parent)
	{
		_callback = callback;
	}

	public override void activate()
	{
		AppShell.Instance.EventMgr.AddListener<TutorialEvent>(OnTutorialEvent);
	}

	protected virtual void OnTutorialEvent(TutorialEvent evt)
	{
		if (_callback != null)
		{
			_callback(evt);
		}
		else if (evt.type == _targetType)
		{
			complete();
		}
	}

	public override void complete()
	{
		AppShell.Instance.EventMgr.RemoveListener<TutorialEvent>(OnTutorialEvent);
		base.complete();
	}

	public override void interrupt()
	{
		AppShell.Instance.EventMgr.RemoveListener<TutorialEvent>(OnTutorialEvent);
	}
}
