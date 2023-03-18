public class MatchmakingInvitationMessage : NetworkMessage
{
	public string invitation;

	public bool accepted;

	public MatchmakingInvitationMessage()
	{
	}

	public MatchmakingInvitationMessage(GoNetId goNetId)
		: base(goNetId)
	{
	}

	public MatchmakingInvitationMessage(string invite, bool accept)
		: base(GoNetId.Invalid)
	{
		invitation = invite;
		accepted = accept;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(invitation);
		writer.Write(accepted);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		invitation = reader.ReadString();
		accepted = reader.ReadBoolean();
	}
}
