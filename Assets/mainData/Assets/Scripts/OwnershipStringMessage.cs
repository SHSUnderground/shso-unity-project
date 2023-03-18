using System.Collections.Generic;

public class OwnershipStringMessage : ShsEventMessage
{
	public int ownerId;

	public List<string> strings;

	public OwnershipStringMessage(int ownerId, List<string> strings)
	{
		this.ownerId = ownerId;
		this.strings = strings;
	}
}
