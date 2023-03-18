using System.Runtime.CompilerServices;

public class BrawlerResultsMessage : ShsEventMessage
{
	[CompilerGenerated]
	private EventResultMissionEvent _003CResults_003Ek__BackingField;

	public EventResultMissionEvent Results
	{
		[CompilerGenerated]
		get
		{
			return _003CResults_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CResults_003Ek__BackingField = value;
		}
	}

	public BrawlerResultsMessage(EventResultMissionEvent results)
	{
		Results = results;
	}
}
