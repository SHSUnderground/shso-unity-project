using UnityEngine;

public class NetActionJump : NetActionPositionFull
{
	public NetActionJump()
	{
	}

	public NetActionJump(GameObject initObject)
		: base(initObject)
	{
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionJump;
	}

	public override string ToString()
	{
		return "NetActionJump: " + timestamp + ", position = " + position;
	}
}
