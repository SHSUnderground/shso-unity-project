using UnityEngine;

public class HqRoomAssignInteractiveController : InteractiveObjectController
{
	public override bool CanPlayerUse(GameObject player)
	{
		return true;
	}

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		if (HqController2.Instance != null)
		{
			HqAssignableRoom hqAssignableRoom = HqController2.Instance.ActiveRoom as HqAssignableRoom;
			if (hqAssignableRoom != null)
			{
				hqAssignableRoom.IsInAssignmentMode = !hqAssignableRoom.IsInAssignmentMode;
				CspUtils.DebugLog("Room assignment mode is " + hqAssignableRoom.IsInAssignmentMode);
			}
		}
		if (onDone != null)
		{
			onDone(player, CompletionStateEnum.Success);
		}
		return true;
	}
}
