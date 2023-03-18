public class NetActionStand : NetAction
{
	public override NetActionType getType()
	{
		return NetActionType.NetActionStand;
	}

	public override string ToString()
	{
		return "NetActionStand: " + base.ToString();
	}
}
