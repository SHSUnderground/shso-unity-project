public class DeleteEntityMessage : NetworkMessage
{
	public DeleteEntityMessage()
	{
	}

	public DeleteEntityMessage(GoNetId goNetId)
		: base(goNetId)
	{
	}
}
