using UnityEngine;

public class NetActionJumpCancel : NetActionPositionFull
{
	private float _jumpDuration;

	private bool _secondJump;

	public float JumpDuration
	{
		get
		{
			return _jumpDuration;
		}
		set
		{
			_jumpDuration = value;
		}
	}

	public bool SecondJump
	{
		get
		{
			return _secondJump;
		}
		set
		{
			_secondJump = value;
		}
	}

	public NetActionJumpCancel()
	{
	}

	public NetActionJumpCancel(GameObject initObject)
		: base(initObject)
	{
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionJumpCancel;
	}

	public override void SerializeToBinary(ShsSerializer.ShsWriter writer)
	{
		base.SerializeToBinary(writer);
		writer.Write(JumpDuration);
		writer.Write(SecondJump);
	}

	public override void DeserializeFromBinary(ShsSerializer.ShsReader reader)
	{
		base.DeserializeFromBinary(reader);
		JumpDuration = reader.ReadFloat();
		SecondJump = reader.ReadBoolean();
	}

	public override string ToString()
	{
		return "NetActionJumpCancel: " + timestamp + ", duration = " + JumpDuration + ", second jump = " + SecondJump + ", position = " + position;
	}
}
