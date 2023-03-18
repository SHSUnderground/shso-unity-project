using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Hq/Switch/Activate Object Controller")]
public class HqActivateObjectController : HqSwitchController
{
	public List<GameObject> ObjectsToActivate = new List<GameObject>();

	protected bool isOn = true;

	protected void Update()
	{
		foreach (GameObject item in ObjectsToActivate)
		{
			if (item.active && !isOn)
			{
				Utils.ActivateTree(item, false);
			}
			else if (!item.active && isOn)
			{
				Utils.ActivateTree(item, true);
			}
			HqTrigger[] components = Utils.GetComponents<HqTrigger>(item, Utils.SearchChildren, true);
			if (components != null && components.Length > 0)
			{
				HqTrigger hqTrigger = components[0];
				if (!isOn && hqTrigger.IsOn)
				{
					hqTrigger.TurnOff();
				}
				else if (isOn && !hqTrigger.IsOn)
				{
					hqTrigger.TurnOn();
				}
			}
		}
	}

	public override bool CanUse()
	{
		return true;
	}

	public override void Flip()
	{
		isOn = !isOn;
	}
}
