public class HqAssignRoomSwitchController : HqSwitchController
{
	public override void Flip()
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
	}

	public override bool CanUse()
	{
		return true;
	}
}
