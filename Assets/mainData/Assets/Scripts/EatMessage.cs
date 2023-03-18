public class EatMessage : ShsEventMessage
{
	private float itemHeight;

	public float ItemHeight
	{
		get
		{
			return itemHeight;
		}
		set
		{
			itemHeight = value;
		}
	}

	public EatMessage(float itemHeight)
	{
		ItemHeight = itemHeight;
	}
}
