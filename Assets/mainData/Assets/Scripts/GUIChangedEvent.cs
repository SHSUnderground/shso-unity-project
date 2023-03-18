public class GUIChangedEvent : GUIEvent
{
	public float OldValue;

	public float NewValue;

	public GUIChangedEvent()
	{
	}

	public GUIChangedEvent(float oldValue, float newValue)
	{
		OldValue = oldValue;
		NewValue = newValue;
	}
}
