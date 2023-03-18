using UnityEngine;

public class PlayerPowerCostMessage : ShsEventMessage
{
	public readonly float powerCost;

	public readonly GameObject owner;

	public PlayerPowerCostMessage(float PowerCost, GameObject Owner)
	{
		powerCost = PowerCost;
		owner = Owner;
	}
}
