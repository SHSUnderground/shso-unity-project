using System;
using System.Collections.Generic;
using UnityEngine;

public class GoToTarget : AutomationCmd
{
	private string iTarget;

	private InteractiveObject target;

	private Queue<InteractiveObject> iObjects;

	public GoToTarget(string cmdline)
		: base(cmdline)
	{
		iTarget = cmdline.Substring("gototarget".Length).Trim();
	}

	public override bool isReady()
	{
		bool result = base.isReady();
		iObjects = new Queue<InteractiveObject>();
		try
		{
			InteractiveObject[] array = Utils.FindObjectsOfType<InteractiveObject>();
			InteractiveObject[] array2 = array;
			foreach (InteractiveObject interactiveObject in array2)
			{
				if (interactiveObject.name.Contains(iTarget))
				{
					iObjects.Enqueue(interactiveObject);
				}
			}
			return result;
		}
		catch (Exception ex)
		{
			base.ErrorCode = "P001";
			base.ErrorMsg = "Error: " + ex.Message;
			return false;
		}
	}

	public override bool execute()
	{
		bool flag = base.execute();
		try
		{
			target = iObjects.Dequeue();
			Vector3 position = target.transform.position;
			float x = position.x;
			Vector3 position2 = target.transform.position;
			float y = position2.y + 1f;
			Vector3 position3 = target.transform.position;
			Vector3 location = new Vector3(x, y, position3.z);
			return AutomationBehavior.Instance.teleport(location);
		}
		catch (Exception ex)
		{
			base.ErrorCode = "E001";
			base.ErrorMsg = "Error: " + ex.Message;
			return false;
		}
	}

	public override bool isCompleted()
	{
		bool flag = base.isCompleted();
		Vector3 position = target.transform.position;
		Vector3 position2 = AutomationManager.Instance.LocalPlayer.transform.position;
		if (AutomationManager.Instance.GetDistance(position.x, position.y, position.z, position2.x, position2.y, position2.z) < 3f)
		{
			return AutomationBehavior.Instance.Interact(target);
		}
		return false;
	}
}
