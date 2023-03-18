public class Tutorial_Master : Tutorial
{
	public Tutorial_Master(NewTutorialManager tutman)
		: base("master", tutman)
	{
	}

	public override void init()
	{
		addScript(new ScriptShowText(this, "ARE YOU READY TO RRRRRRUMMMBBBLLLLLEEEE", true));
		addScript(new ScriptShowText(this, "Click anywhere to move your hero around"));
		addScript(new ScriptWaitForEvent(this, TutorialEvent.TutorialEventType.HERO_MOVE));
		addScript(new ScriptShowText(this, "Holy crap that worked?"));
		addScript(new ScriptWaitForEvent(this, TutorialEvent.TutorialEventType.HIDE_TEXT));
	}
}
