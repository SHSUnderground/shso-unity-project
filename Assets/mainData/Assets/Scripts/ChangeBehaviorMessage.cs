public class ChangeBehaviorMessage : ShsEventMessage
{
	private string behaviorName;

	public string BehaviorName
	{
		get
		{
			return behaviorName;
		}
		set
		{
			behaviorName = value;
		}
	}

	public ChangeBehaviorMessage(string behaviorName)
	{
		BehaviorName = behaviorName;
	}
}
