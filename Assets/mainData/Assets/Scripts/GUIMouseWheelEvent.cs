public class GUIMouseWheelEvent : GUIEvent
{
	private float wheelDelta;

	private int wheelDirection;

	public float WheelDelta
	{
		get
		{
			return wheelDelta;
		}
		set
		{
			wheelDelta = value;
		}
	}

	public int WheelDirection
	{
		get
		{
			return wheelDirection;
		}
		set
		{
			wheelDirection = value;
		}
	}

	public GUIMouseWheelEvent()
	{
	}

	public GUIMouseWheelEvent(float delta, int direction)
	{
		wheelDelta = delta;
		wheelDirection = direction;
	}
}
