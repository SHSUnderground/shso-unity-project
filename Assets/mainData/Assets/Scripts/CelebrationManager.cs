using System.Collections.Generic;
using UnityEngine;

public class CelebrationManager
{
	private const string CHALLENGE_MAX_CHALLENGE_COUNTER = "ChallengeMaxCounter";

	protected List<ShsEventMessage> celebrationMessages = new List<ShsEventMessage>();

	protected bool isCelebrating;

	private bool listenersRegistered;

	private bool shownNewChallenges;

	private bool shownWorldEventWin;

	protected bool HasNewChallenges
	{
		get
		{
			IChallenge currentChallenge = AppShell.Instance.ChallengeManager.CurrentChallenge;
			if (currentChallenge != null && currentChallenge.Id == 1)
			{
				return true;
			}
			Entitlements.ServerMaxEntitlement serverMaxEntitlement = Singleton<Entitlements>.instance.EntitlementsSet[Entitlements.EntitlementFlagEnum.MaxChallengeLevel] as Entitlements.ServerMaxEntitlement;
			if (serverMaxEntitlement != null)
			{
				ISHSCounterType counter = AppShell.Instance.CounterManager.GetCounter("ChallengeMaxCounter");
				if (counter != null)
				{
					int num = (int)counter.GetCurrentValue();
					int lastChallenge = AppShell.Instance.Profile.LastChallenge;
					if (num == 0)
					{
						if (lastChallenge == 25 && serverMaxEntitlement.PlayerMax > 25)
						{
							return true;
						}
					}
					else if (lastChallenge == num && num < serverMaxEntitlement.PlayerMax)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	protected CharacterGlobals CurrentCharGlobals
	{
		get
		{
			GameObject localPlayer = GameController.GetController().LocalPlayer;
			if (localPlayer != null)
			{
				return localPlayer.GetComponent<CharacterGlobals>();
			}
			return null;
		}
	}

	protected bool CanPlayAchievementCompleteSequence(AchievementCompleteMessage e = null)
	{
		if (isCelebrating)
		{
			return false;
		}
		if (AppShell.Instance.ActivityManager.HasActivitiesInProgress)
		{
			return false;
		}
		if (CurrentCharGlobals != null && CurrentCharGlobals.definitionData.CharacterName != "mr_placeholder" && CurrentCharGlobals.motionController != null && CurrentCharGlobals.motionController.IsOnGround())
		{
			BehaviorManager behaviorManager = CurrentCharGlobals.behaviorManager;
			if (behaviorManager != null && behaviorManager.getBehavior() != null && behaviorManager.getBehavior().allowInterrupt(typeof(BehaviorLeveledUp)))
			{
				CspUtils.DebugLog("PLAY ACH " + CurrentCharGlobals.definitionData.CharacterName);
				return true;
			}
		}
		return false;
	}

	protected void OnAchievementCompleteMessage(AchievementCompleteMessage e)
	{
		CspUtils.DebugLog("OnAchievementCompleteMessage");
		if (GameController.GetController() is SocialSpaceController)
		{
			if (celebrationMessages.Count == 0 && CanPlayAchievementCompleteSequence(e))
			{
				PlayAchievementCompleteSequence(e);
				CspUtils.DebugLog("playing celebration");
			}
			else
			{
				celebrationMessages.Add(e);
				CspUtils.DebugLog("queueing celebration");
			}
		}
		else
		{
			CspUtils.DebugLog("ignoring because we aren't in the social space");
		}
	}

	protected void PlayAchievementCompleteSequence(AchievementCompleteMessage e)
	{
		if (!(CurrentCharGlobals != null))
		{
			return;
		}
		isCelebrating = true;
		BehaviorManager behaviorManager = CurrentCharGlobals.behaviorManager;
		if (behaviorManager != null)
		{
			BehaviorLeveledUp behaviorLeveledUp = behaviorManager.requestChangeBehavior<BehaviorLeveledUp>(true);
			if (behaviorLeveledUp != null)
			{
				behaviorLeveledUp.Initialize(true);
			}
		}
	}

	protected void OnAchievementCompleteHideMessage(AchievementCompleteHideMessage e)
	{
		CspUtils.DebugLog("OnAchievementCompleteHideMessage");
		isCelebrating = false;
		if (CurrentCharGlobals != null)
		{
			BehaviorManager behaviorManager = CurrentCharGlobals.behaviorManager;
			if (behaviorManager != null && behaviorManager.getBehavior() != null && behaviorManager.getBehavior().GetType() == typeof(BehaviorLeveledUp))
			{
				behaviorManager.endBehavior();
			}
		}
		int num = celebrationMessages.Count - 1;
		while (true)
		{
			if (num >= 0)
			{
				if (celebrationMessages[num] is AchievementCompleteMessage)
				{
					break;
				}
				num--;
				continue;
			}
			return;
		}
		celebrationMessages.RemoveAt(num);
	}

	protected void OnLeveledUp(LeveledUpMessage e)
	{
		if (!(GameController.GetController() is SocialSpaceController))
		{
			return;
		}
		if (celebrationMessages.Count == 0 && CanPlayLevelUpSequence(e))
		{
			PlayLevelUpSequence(e);
		}
		else
		{
			if (!(CurrentCharGlobals != null) || !(CurrentCharGlobals.definitionData.CharacterName == e.Hero))
			{
				return;
			}
			int i;
			for (i = 0; i < celebrationMessages.Count; i++)
			{
				if (celebrationMessages[i] is LeveledUpMessage && e.NewLevel < (celebrationMessages[i] as LeveledUpMessage).NewLevel)
				{
					celebrationMessages.Insert(i, e);
					break;
				}
			}
			if (i == celebrationMessages.Count)
			{
				celebrationMessages.Add(e);
			}
		}
	}

	protected void OnChallengeUp(ChallengeUpMessage e)
	{
	}

	protected void OnChallengeViewed(ChallengeViewedMessage msg)
	{
		int num = 0;
		while (true)
		{
			if (num < celebrationMessages.Count)
			{
				if (celebrationMessages[num] is ChallengeUpMessage && (celebrationMessages[num] as ChallengeUpMessage).ChallengeId == msg.ChallengeId)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		celebrationMessages.RemoveAt(num);
	}

	protected void PlayLevelUpSequence(LeveledUpMessage e)
	{
		if (!(CurrentCharGlobals != null) || !(CurrentCharGlobals.definitionData.CharacterName == e.Hero))
		{
			return;
		}
		GUIManager.Instance.ShowDynamicWindow(new SHSLevelUpWindow(e), GUIControl.ModalLevelEnum.Full);
		isCelebrating = true;
		BehaviorManager behaviorManager = CurrentCharGlobals.behaviorManager;
		if (behaviorManager != null)
		{
			BehaviorLeveledUp behaviorLeveledUp = behaviorManager.requestChangeBehavior<BehaviorLeveledUp>(true);
			if (behaviorLeveledUp != null)
			{
				behaviorLeveledUp.Initialize(true);
			}
		}
	}

	protected void PlayChallengeUpSequence()
	{
		if (!(CurrentCharGlobals != null))
		{
			return;
		}
		SHSChallengeUpWindow dialogWindow = new SHSChallengeUpWindow(false);
		GUIManager.Instance.ShowDynamicWindow(dialogWindow, GUIControl.ModalLevelEnum.Full);
		BehaviorManager behaviorManager = CurrentCharGlobals.behaviorManager;
		if (behaviorManager != null)
		{
			BehaviorChallengeUp behaviorChallengeUp = behaviorManager.requestChangeBehavior<BehaviorChallengeUp>(true);
			if (behaviorChallengeUp != null)
			{
				behaviorChallengeUp.Initialize(true);
				isCelebrating = true;
			}
		}
	}

	protected void PlayNewChallengeSequence()
	{
		if (!(CurrentCharGlobals != null))
		{
			return;
		}
		SetMaxChallengeCounter();
		SHSChallengeUpWindow dialogWindow = new SHSChallengeUpWindow(true);
		GUIManager.Instance.ShowDynamicWindow(dialogWindow, GUIControl.ModalLevelEnum.Full);
		BehaviorManager behaviorManager = CurrentCharGlobals.behaviorManager;
		if (behaviorManager != null)
		{
			BehaviorChallengeUp behaviorChallengeUp = behaviorManager.requestChangeBehavior<BehaviorChallengeUp>(true);
			if (behaviorChallengeUp != null)
			{
				behaviorChallengeUp.Initialize(false);
				isCelebrating = true;
			}
		}
	}

	private static void SetMaxChallengeCounter()
	{
		Entitlements.ServerMaxEntitlement serverMaxEntitlement = Singleton<Entitlements>.instance.EntitlementsSet[Entitlements.EntitlementFlagEnum.MaxChallengeLevel] as Entitlements.ServerMaxEntitlement;
		if (serverMaxEntitlement != null)
		{
			ISHSCounterType counter = AppShell.Instance.CounterManager.GetCounter("ChallengeMaxCounter");
			if (counter != null && counter.GetCurrentValue() != serverMaxEntitlement.PlayerMax)
			{
				counter.SetCounter(serverMaxEntitlement.PlayerMax, SHSCounterType.ReportingMethodEnum.WebService);
			}
		}
	}

	protected void OnLevelUpWindowHide(LeveledUpAwardHiddenMessage e)
	{
		isCelebrating = false;
		if (CurrentCharGlobals != null && CurrentCharGlobals.definitionData.CharacterName == e.message.Hero)
		{
			BehaviorManager behaviorManager = CurrentCharGlobals.behaviorManager;
			if (behaviorManager != null && behaviorManager.getBehavior() != null && behaviorManager.getBehavior().GetType() == typeof(BehaviorLeveledUp))
			{
				behaviorManager.endBehavior();
			}
		}
	}

	protected void OnChallengeCelebrationHide(ChallengeCelebrationHideMessage msg)
	{
		isCelebrating = false;
		BehaviorManager behaviorManager = CurrentCharGlobals.behaviorManager;
		if (behaviorManager != null && behaviorManager.getBehavior() != null && behaviorManager.getBehavior().GetType() == typeof(BehaviorChallengeUp))
		{
			behaviorManager.endBehavior();
		}
		if (!msg.ShowMySquad)
		{
		}
	}

	protected bool CanPlayLevelUpSequence(LeveledUpMessage e)
	{
		if (isCelebrating)
		{
			return false;
		}
		if (AppShell.Instance.ActivityManager.HasActivitiesInProgress)
		{
			return false;
		}
		if (CurrentCharGlobals != null && CurrentCharGlobals.definitionData.CharacterName == e.Hero)
		{
			if (CurrentCharGlobals.motionController != null && CurrentCharGlobals.motionController.IsOnGround())
			{
				BehaviorManager behaviorManager = CurrentCharGlobals.behaviorManager;
				if (behaviorManager != null && behaviorManager.getBehavior() != null && behaviorManager.getBehavior().allowInterrupt(typeof(BehaviorLeveledUp)))
				{
					return true;
				}
			}
		}
		else if (CurrentCharGlobals != null)
		{
			CspUtils.DebugLog("ERROR - Mismatch in character naming convention between " + CurrentCharGlobals.definitionData.CharacterName + " and " + e.Hero);
		}
		return false;
	}

	protected bool CanPlayChallengeUpSequence()
	{
		if (isCelebrating)
		{
			return false;
		}
		if (!(GameController.GetController() is SocialSpaceController))
		{
			return false;
		}
		if (AppShell.Instance.ActivityManager.HasActivitiesInProgress)
		{
			return false;
		}
		if (GUIManager.Instance.CurrentState != 0)
		{
			return false;
		}
		if (GameController.GetController().GuiFullScreenOverlayEnabled)
		{
			return false;
		}
		if (CurrentCharGlobals != null)
		{
			if (CurrentCharGlobals.networkComponent == null || !CurrentCharGlobals.networkComponent.IsOwner())
			{
				return false;
			}
			if (CurrentCharGlobals.motionController != null && CurrentCharGlobals.motionController.IsOnGround())
			{
				BehaviorBase behavior = CurrentCharGlobals.behaviorManager.getBehavior();
				if (behavior is BehaviorMovement && !(behavior is BehaviorApproach) && CurrentCharGlobals.behaviorManager.currentBehaviorInterruptible(typeof(BehaviorChallengeUp)) && (behavior as BehaviorMovement).CurrentMovementState == BehaviorMovement.MovementState.Idle)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void Start()
	{
		if (!listenersRegistered)
		{
			AppShell.Instance.EventMgr.AddListener<LeveledUpMessage>(OnLeveledUp);
			AppShell.Instance.EventMgr.AddListener<LeveledUpAwardHiddenMessage>(OnLevelUpWindowHide);
			AppShell.Instance.EventMgr.AddListener<ChallengeUpMessage>(OnChallengeUp);
			AppShell.Instance.EventMgr.AddListener<ChallengeCelebrationHideMessage>(OnChallengeCelebrationHide);
			AppShell.Instance.EventMgr.AddListener<ChallengeViewedMessage>(OnChallengeViewed);
			AppShell.Instance.EventMgr.AddListener<AchievementCompleteMessage>(OnAchievementCompleteMessage);
			AppShell.Instance.EventMgr.AddListener<AchievementCompleteHideMessage>(OnAchievementCompleteHideMessage);
		}
		isCelebrating = false;
	}

	public void Suspend()
	{
		if (listenersRegistered)
		{
			AppShell.Instance.EventMgr.RemoveListener<LeveledUpMessage>(OnLeveledUp);
			AppShell.Instance.EventMgr.RemoveListener<LeveledUpAwardHiddenMessage>(OnLevelUpWindowHide);
			AppShell.Instance.EventMgr.RemoveListener<ChallengeUpMessage>(OnChallengeUp);
			AppShell.Instance.EventMgr.RemoveListener<ChallengeCelebrationHideMessage>(OnChallengeCelebrationHide);
			AppShell.Instance.EventMgr.RemoveListener<ChallengeViewedMessage>(OnChallengeViewed);
			AppShell.Instance.EventMgr.RemoveListener<AchievementCompleteMessage>(OnAchievementCompleteMessage);
			AppShell.Instance.EventMgr.RemoveListener<AchievementCompleteHideMessage>(OnAchievementCompleteHideMessage);
		}
	}

	public void Update()
	{
		if (!shownWorldEventWin && CanPlayChallengeUpSequence())
		{
			SHSSocialMainWindow sHSSocialMainWindow = GUIManager.Instance["SHSMainWindow/SHSSocialMainWindow"] as SHSSocialMainWindow;
			if (sHSSocialMainWindow != null)
			{
				sHSSocialMainWindow.OnWorldEventWon("WinnerTopScore");
				sHSSocialMainWindow.OnWorldEventWon("WinnerRandomDraw");
			}
			shownWorldEventWin = true;
		}
		for (int num = celebrationMessages.Count - 1; num >= 0; num--)
		{
			if (celebrationMessages[num] is LeveledUpMessage)
			{
				LeveledUpMessage leveledUpMessage = celebrationMessages[num] as LeveledUpMessage;
				if (!(GameController.GetController() is SocialSpaceController))
				{
					celebrationMessages.RemoveAt(num);
				}
				else if (CurrentCharGlobals == null || CurrentCharGlobals.definitionData.CharacterName != leveledUpMessage.Hero)
				{
					celebrationMessages.RemoveAt(num);
				}
			}
			if (celebrationMessages[num] is ChallengeUpMessage)
			{
				celebrationMessages.RemoveAt(num);
			}
		}
		if (celebrationMessages.Count > 0)
		{
			if (celebrationMessages[0] is LeveledUpMessage && CanPlayLevelUpSequence(celebrationMessages[0] as LeveledUpMessage))
			{
				PlayLevelUpSequence(celebrationMessages[0] as LeveledUpMessage);
				celebrationMessages.RemoveAt(0);
			}
			else if (celebrationMessages[0] is ChallengeUpMessage && CanPlayChallengeUpSequence())
			{
				PlayChallengeUpSequence();
				celebrationMessages.RemoveAt(0);
			}
			else if (celebrationMessages[0] is AchievementCompleteMessage && CanPlayAchievementCompleteSequence(null))
			{
				PlayAchievementCompleteSequence(celebrationMessages[0] as AchievementCompleteMessage);
				celebrationMessages.RemoveAt(0);
			}
		}
	}
}
