using System.Collections;

public abstract class EventResultBase
{
	public enum EventResultType
	{
		HeroLevelUp,
		StageEvent,
		MissionEvent,
		SquadBattle
	}

	public readonly EventResultType type;

	protected EventResultBase(EventResultType myType)
	{
		type = myType;
	}

	public abstract void InitializeFromData(DataWarehouse data);

	public abstract void InitializeFromData(Hashtable data);
}
