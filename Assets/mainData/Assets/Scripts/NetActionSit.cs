using UnityEngine;

public class NetActionSit : NetActionPositionFullRotation
{
	public NetActionSit()
	{
	}

	public NetActionSit(GameObject initObject)
		: base(initObject)
	{
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionSit;
	}

	public override string ToString()
	{
		return "NetActionSit: " + base.ToString();
	}
}
