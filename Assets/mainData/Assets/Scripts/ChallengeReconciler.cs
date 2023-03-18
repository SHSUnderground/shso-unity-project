public class ChallengeReconciler
{
	private ChallengeManager manager;

	public ChallengeReconciler(ChallengeManager manager)
	{
		this.manager = manager;
		AppShell.Instance.EventMgr.AddListener<ZoneLoadedMessage>(OnZoneLoaded);
	}

	~ChallengeReconciler()
	{
		AppShell.Instance.EventMgr.RemoveListener<ZoneLoadedMessage>(OnZoneLoaded);
	}

	private void OnZoneLoaded(ZoneLoadedMessage message)
	{
		switch (manager.ChallengeManagerStatus)
		{
		case ChallengeManager.ChallengeManagerStateEnum.Uninitialized:
		case ChallengeManager.ChallengeManagerStateEnum.Inactive:
		case ChallengeManager.ChallengeManagerStateEnum.ChallengeVerificationPending:
		case ChallengeManager.ChallengeManagerStateEnum.AllChallengesCompleted:
			break;
		case ChallengeManager.ChallengeManagerStateEnum.ChallengeInProgress:
		case ChallengeManager.ChallengeManagerStateEnum.ChallengeDisplayPending:
			ReconcileCheck();
			break;
		default:
			CspUtils.DebugLog("Challenge Manager is in an unknown state.");
			break;
		}
	}

	public void ReconcileCheck()
	{
		ReconcileAutoPotion();
		ReconcileManualPotion();
	}

	private void ReconcileAutoPotion()
	{
	}

	private void ReconcileManualPotion()
	{
		UserProfile profile = AppShell.Instance.Profile;
		if (profile == null)
		{
			return;
		}
		ChallengeManager.ChallengeManagerStateEnum currentState;
		IChallenge challenge;
		ChallengeInfo challengeInfo;
		manager.GetChallengeManagerDisplayInfo(out currentState, out challenge, out challengeInfo);
		if (currentState != ChallengeManager.ChallengeManagerStateEnum.ChallengeDisplayPending || challengeInfo == null || !manager.IsManualHeroRewardChallengeId(challengeInfo.ChallengeId) || challengeInfo.Reward == null)
		{
			return;
		}
		bool flag = profile.ExpendablesCollection.ContainsKey(challengeInfo.Reward.value);
		if (manager.GetOwnedManualHeroCount() >= manager.GetExpectedManualHeroCount(challengeInfo.ChallengeId))
		{
			if (flag)
			{
				CspUtils.DebugLog("ChallengeReconciler::ReconcileManualPotion() - player already owns " + manager.GetOwnedManualHeroCount() + " heroes and has a potion for this challenge tier");
			}
			CspUtils.DebugLog("ChallengeReconciler::ReconcileManualPotion() - attempting to reconcile player with expected hero count by moving them past manual hero reward challenge");
			manager.SetViewedChallenge(challengeInfo.ChallengeId, null);
		}
	}

	private static void OnReconciledPotionMessage(IExpendHandler handler)
	{
		if (handler.State != ExpendHandlerState.Completed)
		{
			CspUtils.DebugLog("Unable to reconcile potion consume. Will retry on next reconcile check");
			return;
		}
		CspUtils.DebugLog("RECONCILED potion: " + handler.OwnableTypeId);
		AppShell.Instance.Profile.StartHeroFetch();
	}

	public void Update()
	{
	}
}
