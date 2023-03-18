using System;
using System.IO;

public class NetworkMessage
{
	public int senderRTCId;

	public GoNetId goNetId;

	public NetworkMessage()
	{
		senderRTCId = -1;
		goNetId = GoNetId.Invalid;
	}

	public NetworkMessage(GoNetId goNetId)
	{
		senderRTCId = -1;
		this.goNetId = goNetId;
	}

	public static string ToEncodedString(NetworkMessage msg)
	{
		MemoryStream memoryStream = new MemoryStream(64);
		ShsSerializer.ShsWriter shsWriter = new ShsSerializer.ShsWriter(memoryStream);
		shsWriter.Write(msg);
		return Base85.ToBase85String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
	}

	public static NetworkMessage FromEncodedString(string raw)
	{
		byte[] buffer = Base85.FromBase85String(raw);
		MemoryStream input = new MemoryStream(buffer);
		ShsSerializer.ShsReader shsReader = new ShsSerializer.ShsReader(input);
		return shsReader.ReadNetworkMessage();
	}

	public static string ToEncodedString64(NetworkMessage msg)
	{
		MemoryStream memoryStream = new MemoryStream(64);
		ShsSerializer.ShsWriter shsWriter = new ShsSerializer.ShsWriter(memoryStream);
		shsWriter.Write(msg);
		return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
	}

	public static NetworkMessage FromEncodedString64(string raw)
	{
		byte[] buffer = Convert.FromBase64String(raw);
		MemoryStream input = new MemoryStream(buffer);
		ShsSerializer.ShsReader shsReader = new ShsSerializer.ShsReader(input);
		return shsReader.ReadNetworkMessage();
	}

	public virtual void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		writer.Write(goNetId);
	}

	public virtual void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		goNetId = reader.ReadGoNetId();
	}
}
