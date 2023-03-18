using UnityEngine;

public class ScriptShowText : ScriptBase
{
	private string _txt;

	private bool _showNextButton;

	private Vector2 _pos;

	private NewTutorialDialog _dlg;

	public ScriptShowText(Tutorial parent, string txt, bool showNextButton = false)
		: base(parent)
	{
		_txt = txt;
		_showNextButton = showNextButton;
	}

	public void setPos(Vector2 pos)
	{
		_pos = pos;
	}

	public override void activate()
	{
		_dlg = NewTutorialDialog.CreateWindow();
		_dlg.InitializeResources(false);
		_dlg.SetText(_txt);
		if (_showNextButton)
		{
			AppShell.Instance.EventMgr.AddListener<TutorialEvent>(OnTutorialEvent);
		}
		else
		{
			complete();
		}
	}

	protected void OnTutorialEvent(TutorialEvent evt)
	{
		if (evt.type == TutorialEvent.TutorialEventType.TEXT_HIDDEN)
		{
			complete();
		}
	}

	public override void complete()
	{
		if (_showNextButton)
		{
			AppShell.Instance.EventMgr.RemoveListener<TutorialEvent>(OnTutorialEvent);
		}
		base.complete();
	}
}
