using UnityEngine;

public class TeleportCmd : AutomationCmd
{
	private string position;

	public TeleportCmd(string cmdline, string vector)
		: base(cmdline)
	{
		position = vector;
	}

	public override bool execute()
	{
		Vector3 location = AutomationBrawler.instance.POIMap(position);
		AutomationBehavior.Instance.teleport(location);
		return base.execute();
	}

	public override bool isCompleted()
	{
		bool flag = base.isCompleted();
		float num = 0f;
		Vector3 vector = AutomationManager.Instance.LocalPlayer.transform.position;
		Vector3 vector2 = AutomationBrawler.instance.POIMap(position);
		if (flag)
		{
			num = AutomationManager.Instance.GetDistance(vector.x, vector.y, vector.z, vector2.x, vector2.y, vector2.z);
			flag = ((!(num > 2f)) ? true : false);
		}
		else
		{
			base.ErrorCode = "C001";
			base.ErrorMsg = "Timeout!";
			AutomationManager.Instance.errOther++;
		}
		return flag;
	}
}
