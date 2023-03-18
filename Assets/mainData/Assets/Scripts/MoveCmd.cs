using System;
using UnityEngine;

public class MoveCmd : AutomationCmd
{
	private string location;

	public float charDistance;

	public MoveCmd(string cmdline)
		: base(cmdline)
	{
	}

	public MoveCmd(string cmdline, string map)
		: base(cmdline)
	{
		location = map;
		AutomationManager.Instance.nOther++;
	}

	public override bool precheckOk()
	{
		bool flag = base.precheckOk();
		bool flag2 = AutomationManager.Instance.activeController == GameController.ControllerType.Brawler;
		CspUtils.DebugLog("status: " + flag);
		if (!flag || !flag2)
		{
			base.ErrorCode = "P01";
			base.ErrorMsg = "MoveCmd  failed - ActiveController not the Brawler";
		}
		return flag;
	}

	public override bool isReady()
	{
		bool flag = base.isReady();
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		charDistance = 0f;
		if (flag)
		{
			try
			{
				if (!(AutomationManager.Instance.LocalPlayer != null))
				{
					return false;
				}
				zero = AutomationManager.Instance.LocalPlayer.transform.position;
				zero2 = AutomationManager.Instance.objLocation;
				charDistance = AutomationManager.Instance.GetDistance(zero.x, zero.y, zero.z, zero2.x, zero2.y, zero2.z);
				return flag;
			}
			catch (Exception ex)
			{
				flag = false;
				AutomationManager.Instance.errOther++;
				base.ErrorCode = "P001";
				base.ErrorMsg = "Unexpected Exception was Caught: " + ex.Message;
				return flag;
			}
		}
		base.ErrorCode = "P001";
		AutomationManager.Instance.errOther++;
		base.ErrorMsg = "Timeout!";
		return true;
	}

	public override bool execute()
	{
		Vector3 vector = AutomationBrawler.instance.POIMap(location);
		try
		{
			AutomationBrawler.instance.move(vector);
			AutomationManager.Instance.moveInProgress = true;
		}
		catch (Exception ex)
		{
			base.ErrorCode = "E001";
			base.ErrorMsg = ex.Message;
		}
		return base.execute();
	}

	public override bool isCompleted()
	{
		bool flag = base.isCompleted();
		float num = 0f;
		Vector3 position = AutomationManager.Instance.LocalPlayer.transform.position;
		Vector3 objLocation = AutomationManager.Instance.objLocation;
		if (flag)
		{
			num = AutomationManager.Instance.GetDistance(position.x, position.y, position.z, objLocation.x, objLocation.y, objLocation.z);
			if (num > 2f)
			{
				flag = false;
				if (DateTime.Now - base.StartTime > TimeSpan.FromSeconds(1.0))
				{
					CspUtils.DebugLogWarning("Spawned Objects: " + AutomationManager.Instance.spawnObjCount);
					if (!isBrawlerMultiplayer)
					{
						if (AutomationManager.Instance.spawnObjCount == 1)
						{
							AutomationBrawler.instance.move(objLocation);
						}
					}
					else
					{
						AutomationBrawler.instance.move(objLocation);
					}
					base.StartTime = DateTime.Now;
				}
			}
		}
		else
		{
			AutomationBrawler.instance.teleport(objLocation);
			VerifyBrawlerSpawnEnemies verifyBrawlerSpawnEnemies = new VerifyBrawlerSpawnEnemies("verifybrawlerenemies");
			verifyBrawlerSpawnEnemies.execute();
			AutomationBrawler.instance.DefeatActiveEnemies();
			base.ErrorCode = "C001";
			base.ErrorMsg = "Timeout!";
			AutomationManager.Instance.errOther++;
			flag = true;
		}
		return flag;
	}
}
