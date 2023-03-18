public class BrawlerStageCompleteMessage : NetworkMessage
{
	public int stageNumber;

	public BrawlerStageCompleteMessage()
	{
	}

	public BrawlerStageCompleteMessage(GoNetId goNetId)
		: base(goNetId)
	{
	}

	public BrawlerStageCompleteMessage(int stageNumber)
		: base(GoNetId.Invalid)
	{
		this.stageNumber = stageNumber;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(stageNumber);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		stageNumber = reader.ReadInt32();
	}
}
