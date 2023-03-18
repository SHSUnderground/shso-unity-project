public class TutorialEvent : ShsEventMessage
{
	public enum TutorialEventType
	{
		HIDE_TEXT,
		TEXT_HIDDEN,
		HERO_MOVE,
		HERO_JUMP,
		MAX
	}

	public TutorialEventType type;

	public TutorialEvent(TutorialEventType type)
	{
		this.type = type;
	}
}
