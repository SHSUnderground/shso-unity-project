using System;
using System.Collections.Generic;

public class ChangeHudButtonsAndTraitsMessage : ShsEventMessage
{
	public List<SHSHudWheels.ButtonType> toEnable = new List<SHSHudWheels.ButtonType>();

	public List<SHSHudWheels.ButtonType> toDisable = new List<SHSHudWheels.ButtonType>();

	public void EnableAllBut(params SHSHudWheels.ButtonType[] disable)
	{
		toDisable.AddRange(disable);
		foreach (int value in Enum.GetValues(typeof(SHSHudWheels.ButtonType)))
		{
			if (!toDisable.Contains((SHSHudWheels.ButtonType)value))
			{
				toEnable.Add((SHSHudWheels.ButtonType)value);
			}
		}
	}

	public void DisableAllBut(params SHSHudWheels.ButtonType[] enable)
	{
		toEnable.AddRange(enable);
		foreach (int value in Enum.GetValues(typeof(SHSHudWheels.ButtonType)))
		{
			if (!toEnable.Contains((SHSHudWheels.ButtonType)value))
			{
				toDisable.Add((SHSHudWheels.ButtonType)value);
			}
		}
	}
}
