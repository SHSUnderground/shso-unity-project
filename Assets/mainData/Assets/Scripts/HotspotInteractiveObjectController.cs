using UnityEngine;

public abstract class HotspotInteractiveObjectController : InteractiveObjectController
{
	public HotSpotType.Style hotSpotType;

	public override InteractiveObject.StateIdx GetStateForPlayer(GameObject player)
	{
		if (player == null)
		{
			return InteractiveObject.StateIdx.Disable;
		}
		if (IsControllerHotSpotType(player))
		{
			return InteractiveObject.StateIdx.Enable;
		}
		return InteractiveObject.StateIdx.Disable;
	}

	public void reportHotSpotUse(GameObject player)
	{
		CspUtils.DebugLog("reportHotSpotUse " + hotSpotType + " " + player);
		string text = string.Empty;
		CspUtils.DebugLog("StartWithPlaye4 " + hotSpotType);
		switch (hotSpotType)
		{
		case HotSpotType.Style.Flying:
			text = "flight";
			break;
		case HotSpotType.Style.GroundSpeed:
			text = "racetrack";
			break;
		case HotSpotType.Style.Web:
			text = "wallcrawl";
			break;
		case HotSpotType.Style.Teleport:
			text = "teleport";
			break;
		case HotSpotType.Style.Strength:
			if (base.gameObject.name.Contains("FeatOfStrengthHeavy") && player == GameController.GetController().LocalPlayer && AppShell.Instance.Profile != null)
			{
				AppShell.Instance.delayedAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "social_zone", "asgard_strength", string.Empty, 1f);
			}
			break;
		}
		if (text != string.Empty && AppShell.Instance != null && AppShell.Instance.Profile != null && player == GameController.GetController().LocalPlayer && AchievementManager.shouldReportAchievementEvent("generic_event", "hotspot", text))
		{
			AppShell.Instance.delayedAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "generic_event", "hotspot", text, 3f);
		}
	}

	public override bool CanPlayerUse(GameObject player)
	{
		CspUtils.DebugLog("CanPlayerUse ");
		return IsControllerHotSpotType(player) && !IsPlayerAirborne(player);
	}

	public override void AttemptedInvalidUse(GameObject player)
	{
		if (player != null && !IsControllerHotSpotType(player))
		{
			GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.OkDialog, HotSpotType.GetCannotUseString(hotSpotType), (IGUIDialogNotification)null, GUIControl.ModalLevelEnum.Default);
		}
	}

	public virtual bool IsControllerHotSpotType(GameObject player)
	{
		if (hotSpotType == HotSpotType.Style.None)
		{
			return true;
		}
		int currentPet = PetDataManager.getCurrentPet();
		PetData data = PetDataManager.getData(currentPet);
		CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(player);
		return (component != null && (component.hotSpotType & hotSpotType) != 0) || (data != null && (data.hotSpotType & hotSpotType) != 0);
	}

	public bool IsPlayerAirborne(GameObject player)
	{
		CharacterMotionController component = player.GetComponent<CharacterMotionController>();
		return component != null && !component.IsOnGround();
	}
}
