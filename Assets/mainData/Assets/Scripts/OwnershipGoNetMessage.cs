using System.Collections.Generic;

public class OwnershipGoNetMessage : ShsEventMessage
{
	public int ownerId;

	public List<GoNetId> goNetIds;

	public OwnershipGoNetMessage(int ownerId, List<GoNetId> goNetIds)
	{
		this.ownerId = ownerId;
		this.goNetIds = goNetIds;
	}
}
