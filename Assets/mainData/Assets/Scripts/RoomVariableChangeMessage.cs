using System.Collections;

public class RoomVariableChangeMessage : ShsEventMessage
{
	public int roomId;

	public Hashtable changedVars;

	public RoomVariableChangeMessage(int roomId, Hashtable changedVars)
	{
		this.roomId = roomId;
		this.changedVars = changedVars;
	}
}
