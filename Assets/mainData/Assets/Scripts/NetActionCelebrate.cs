using UnityEngine;

public abstract class NetActionCelebrate : NetActionPositionFull
{
	public NetActionCelebrate()
	{
	}

	public NetActionCelebrate(GameObject initObject)
		: base(initObject)
	{
	}

	public abstract void Process(CharacterGlobals character);

	public override string ToString()
	{
		return "NetActionLevelUp: " + base.ToString();
	}
}
