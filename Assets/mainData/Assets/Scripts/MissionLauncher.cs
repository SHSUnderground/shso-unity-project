using UnityEngine;

public class MissionLauncher : GlowableInteractiveController
{
	public enum MissionType
	{
		Solo,
		Anyone
	}

	public class Use : BaseUse
	{
		private MissionLauncher owner;

		private bool skipApproach;

		public Use(GameObject player, GlowableInteractiveController owner, OnDone onDone, bool skipApproach)
			: base(player, owner, onDone)
		{
			this.owner = (owner as MissionLauncher);
			this.skipApproach = skipApproach;
		}

		public override bool Start()
		{
			if (base.Start())
			{
				if (skipApproach)
				{
					base.Done();
					OnApproachArrived(null);
				}
				else
				{
					Approach(owner.transform, owner.approachDistance, true);
				}
				return true;
			}
			return false;
		}

		protected override void OnApproachArrived(GameObject player)
		{
			if (base.IsLocal && CanLaunchMission())
			{
				AppShell.Instance.DataManager.LoadGameData("Missions/" + owner.missionID, OnMissionDefinitionLoaded, new MissionDefinition());
			}
			base.OnApproachArrived(player);
		}

		protected bool CanLaunchMission()
		{
			if (!Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.MissionsPermitSet))
			{
				GUIManager.Instance.ShowErrorDialog(SHSErrorCodes.Code.MissionsUnavailable, string.Empty);
				return false;
			}
			return true;
		}

		protected void RequestLaunchMission()
		{
			if (owner.requestConfirmation)
			{
				string titleText = string.Empty;
				if (owner.confirmationString != string.Empty)
				{
					titleText = owner.confirmationString;
				}
				SHSSocialMayhemModeConfirmation dialogWindow = new SHSSocialMayhemModeConfirmation(owner.iconName, titleText, owner.confirmationStringColor, LaunchMission);
				GUIManager.Instance.ShowDynamicWindow(dialogWindow, GUIControl.ModalLevelEnum.Full);
			}
			else
			{
				LaunchMission();
			}
		}

		protected void OnMissionDefinitionLoaded(GameDataLoadResponse response, object extraData)
		{
			if (response.Error != null && response.Error != string.Empty)
			{
				CspUtils.DebugLog("MissionLauncher: The following error occurred while fetching game data for <" + response.Path + ">: " + response.Error);
				return;
			}
			MissionDefinition missionDefinition = response.DataDefinition as MissionDefinition;
			foreach (string blacklistCharacter in missionDefinition.blacklistCharacters)
			{
				bool flag = true;
				if (blacklistCharacter == "non_villains")
				{
					GameObject localPlayer = GameController.GetController().LocalPlayer;
					if (localPlayer != null)
					{
						CharacterGlobals component = localPlayer.GetComponent<CharacterGlobals>();
						if (!component.definitionData.isVillain)
						{
							CspUtils.DebugLogWarning("This mission only allows villains, and " + component.definitionData.CharacterName + " is not a villain, check their XML.");
							flag = false;
						}
					}
				}
				if (blacklistCharacter == AppShell.Instance.Profile.LastSelectedCostume || !flag)
				{
					SHSErrorNotificationWindow sHSErrorNotificationWindow = new SHSErrorNotificationWindow(SHSErrorNotificationWindow.ErrorIcons.MissionNoGo);
					sHSErrorNotificationWindow.TitleText = "#error_oops";
					sHSErrorNotificationWindow.Text = "#BRAWLER_BANNED_HERO_NOTIF";
					GUIManager.Instance.ShowDynamicWindow(sHSErrorNotificationWindow, GUIControl.ModalLevelEnum.Full);
					return;
				}
			}
			RequestLaunchMission();
		}

		protected void LaunchMission()
		{
			Transform parent = owner.gameObject.transform.parent;
			while (parent != null)
			{
				MayhemModeObject component = parent.gameObject.GetComponent<MayhemModeObject>();
				if (component != null)
				{
					component.MarkUsed();
					break;
				}
				parent = parent.parent;
			}
			if (owner.missionType == MissionType.Solo)
			{
				AppShell.Instance.Matchmaker2.SoloBrawler(owner.missionID, OnRecievedTicket);
			}
			else if (owner.requiresMissionOwnership)
			{
				AppShell.Instance.Matchmaker2.AnyoneBrawler(OnRecievedTicket, "");  // CSP
			}
			else
			{
				AppShell.Instance.Matchmaker2.MiniBrawler(owner.missionID, OnRecievedTicket);
			}
		}

		private void OnRecievedTicket(Matchmaker2.Ticket ticket)
		{
			if (ticket != null && ticket.status == Matchmaker2.Ticket.Status.SUCCESS)
			{
				ticket.local = true;
				AppShell.Instance.SharedHashTable["BrawlerTicket"] = ticket;
				AppShell.Instance.SharedHashTable["BrawlerAirlockPlaySolo"] = (owner.missionType == MissionType.Solo);
				AppShell.Instance.SharedHashTable["BrawlerAirlockReturnsToGadget"] = false;
				AppShell.Instance.QueueLocationInfo();
				if (owner.returnSpawnPoint != null)
				{
					AppShell.Instance.SharedHashTable["SocialSpaceSpawnPoint"] = owner.returnSpawnPoint.group;
				}
				ActiveMission activeMission = new ActiveMission(owner.missionID);
				activeMission.BecomeActiveMission();
				AppShell.Instance.Transition(GameController.ControllerType.Brawler);
			}
			else
			{
				SHSErrorNotificationWindow sHSErrorNotificationWindow = new SHSErrorNotificationWindow(SHSErrorNotificationWindow.ErrorIcons.MissionNoGo);
				sHSErrorNotificationWindow.TitleText = "#error_oops";
				sHSErrorNotificationWindow.Text = "#missioninvite_error";
				GUIManager.Instance.ShowDynamicWindow(sHSErrorNotificationWindow, GUIControl.ModalLevelEnum.Full);
			}
		}
	}

	public MissionType missionType;

	public string missionID;

	public bool requiresMissionOwnership = true;

	public bool skipApproach;

	public float approachDistance = 3f;

	public SpawnPoint returnSpawnPoint;

	public AssetBundleLoader.BundleGroup minimumDownloadedBundleGroup = AssetBundleLoader.BundleGroup.SpecialMission;

	public bool requestConfirmation = true;

	public string confirmationString = string.Empty;

	public Color confirmationStringColor = new Color(56f / 255f, 56f / 255f, 56f / 255f);

	public string iconName = string.Empty;

	public override void SetGlowable(bool enable)
	{
		if (!skipApproach)
		{
			base.SetGlowable(enable);
		}
	}

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		return new Use(player, this, onDone, skipApproach).Start();
	}
}
