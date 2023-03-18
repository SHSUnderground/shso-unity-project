using System.Collections.Generic;
using UnityEngine;

public class Tutorial
{
	public string name;

	private List<ScriptBase> _scripts = new List<ScriptBase>();

	public NewTutorialManager tm;

	private int _currentScriptIndex;

	private float _lastFrameTime;

	public Tutorial(string name, NewTutorialManager manager)
	{
		this.name = name;
		tm = manager;
		init();
	}

	public virtual void init()
	{
	}

	public void begin()
	{
		CspUtils.DebugLog("Tutorial begin");
		_lastFrameTime = Time.time;
		_currentScriptIndex = 0;
		_scripts[_currentScriptIndex].activate();
	}

	public void addScript(ScriptBase script)
	{
		_scripts.Add(script);
	}

	public void interrupt()
	{
		if (_currentScriptIndex < _scripts.Count)
		{
			_scripts[_currentScriptIndex].interrupt();
			_currentScriptIndex = 0;
		}
		NewTutorialManager.clearFlags();
	}

	public void tick()
	{
		float delta = Time.time - _lastFrameTime;
		_lastFrameTime = Time.time;
		if (_currentScriptIndex < _scripts.Count)
		{
			_scripts[_currentScriptIndex].tick(delta);
		}
	}

	public void scriptComplete(ScriptBase script)
	{
		_currentScriptIndex++;
		CspUtils.DebugLog("scriptComplete, current index is now " + _currentScriptIndex + " " + _scripts.Count);
		if (_currentScriptIndex >= _scripts.Count)
		{
			tm.tutorialComplete(this);
		}
		else
		{
			_scripts[_currentScriptIndex].activate();
		}
	}

	public static DynamicPoint adjustPoint(DynamicPoint pt, float x, float y)
	{
		return pt.offset(new DynamicPoint(x, y));
	}
}
