using UnityEngine;

public class NetActionChargeCollision : NetActionPositionFull
{
	public NetActionChargeCollision()
	{
	}

	public NetActionChargeCollision(GameObject obj)
		: base(obj)
	{
	}

	public override NetActionType getType()
	{
		return NetActionType.NetActionChargeCollision;
	}

	public override string ToString()
	{
		return "NetActionChargeCollision: " + base.ToString();
	}
}
