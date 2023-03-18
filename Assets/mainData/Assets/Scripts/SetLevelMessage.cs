public class SetLevelMessage : ShsEventMessage
{
	private int m_Level;

	public int Level
	{
		get
		{
			return m_Level;
		}
		set
		{
			m_Level = value;
		}
	}

	public SetLevelMessage(int level)
	{
		Level = level;
	}
}
