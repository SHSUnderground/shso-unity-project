using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;
using UnityEngine;

public class ChallengeManager
{
	public enum ChallengeManagerStateEnum
	{
		Uninitialized,
		Inactive,
		ChallengeInProgress,
		ChallengeDisplayPending,
		ChallengeVerificationPending,
		ChallengeRewardPending,
		AllChallengesCompleted
	}

	public delegate void OnChallengeViewedResponse(bool success);

	public delegate void ChallengeCompleteDelegate(IChallenge challenge);

	public delegate void ConsumedPotionDelegate(IExpendHandler handler);

	public const string CHALLENGE_COUNTER = "ChallengeCounter";

	public const string CHALLENGE_WATCHER_COUNTER = "ChallengeWatcherCounter";

	private readonly Dictionary<int, ChallengeInfo> challengeDictionary;

	private bool enabled;

	private ChallengeManagerStateEnum challengeManagerStatus;

	public static bool LogChallenges = true;

	private bool profileLoaded;

	private ChallengeReconciler reconciler;

	private bool listenersRegistered;

	private Dictionary<int, ConsumedPotionDelegate> potionConsumeCallbacks = new Dictionary<int, ConsumedPotionDelegate>();

	[CompilerGenerated]
	private ChallengeManagerServerStub _003CChallengeServer_003Ek__BackingField;

	[CompilerGenerated]
	private ChallengeManagerFanfareManager _003CFanfareManager_003Ek__BackingField;

	[CompilerGenerated]
	private ISHSCounterType _003CWatcherCounter_003Ek__BackingField;

	[CompilerGenerated]
	private int _003CMaxKnownChallenges_003Ek__BackingField;

	[CompilerGenerated]
	private IChallenge _003CCurrentChallenge_003Ek__BackingField;

	[CompilerGenerated]
	private int _003CLastViewedChallengeId_003Ek__BackingField;

	[CompilerGenerated]
	private int _003CNextChallengeServerId_003Ek__BackingField;

	public ChallengeManagerServerStub ChallengeServer
	{
		[CompilerGenerated]
		get
		{
			return _003CChallengeServer_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CChallengeServer_003Ek__BackingField = value;
		}
	}

	public ChallengeManagerFanfareManager FanfareManager
	{
		[CompilerGenerated]
		get
		{
			return _003CFanfareManager_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CFanfareManager_003Ek__BackingField = value;
		}
	}

	public ISHSCounterType WatcherCounter
	{
		[CompilerGenerated]
		get
		{
			return _003CWatcherCounter_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CWatcherCounter_003Ek__BackingField = value;
		}
	}

	public int MaxKnownChallenges
	{
		[CompilerGenerated]
		get
		{
			return _003CMaxKnownChallenges_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CMaxKnownChallenges_003Ek__BackingField = value;
		}
	}

	public bool Enabled
	{
		get
		{
			return enabled;
		}
	}

	public ChallengeManagerStateEnum ChallengeManagerStatus
	{
		get
		{
			return challengeManagerStatus;
		}
		set
		{
			ChallengeManagerStateEnum lastState = challengeManagerStatus;
			challengeManagerStatus = value;
			AppShell.Instance.EventMgr.Fire(this, new ChallengeManagerStateChangedMessage(lastState, value));
		}
	}

	public Dictionary<int, ChallengeInfo> ChallengeDictionary
	{
		get
		{
			return challengeDictionary;
		}
	}

	public IChallenge CurrentChallenge
	{
		[CompilerGenerated]
		get
		{
			return _003CCurrentChallenge_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CCurrentChallenge_003Ek__BackingField = value;
		}
	}

	public int LastViewedChallengeId
	{
		[CompilerGenerated]
		get
		{
			return _003CLastViewedChallengeId_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CLastViewedChallengeId_003Ek__BackingField = value;
		}
	}

	public int NextChallengeServerId
	{
		[CompilerGenerated]
		get
		{
			return _003CNextChallengeServerId_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CNextChallengeServerId_003Ek__BackingField = value;
		}
	}

	public ChallengeManager()
	{
		challengeDictionary = new Dictionary<int, ChallengeInfo>();
		ChallengeManagerStatus = ChallengeManagerStateEnum.Uninitialized;
		ChallengeServer = new ChallengeManagerServerStub(this);
		FanfareManager = new ChallengeManagerFanfareManager(this);
		reconciler = new ChallengeReconciler(this);
		Singleton<ChallengeMessageType>.instance.BuildIdSet();
	}

	public void InitializeFromData(DataWarehouse xml)
	{
		XPathNavigator value = xml.GetValue("challenges");
		XPathNodeIterator xPathNodeIterator = value.SelectChildren("challenge", string.Empty);
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(ChallengeInfo));
		while (xPathNodeIterator.MoveNext())
		{
			string outerXml = xPathNodeIterator.Current.OuterXml;
			ChallengeInfo challengeInfo = xmlSerializer.Deserialize(new StringReader(outerXml)) as ChallengeInfo;
			if (challengeInfo != null)
			{
				challengeDictionary[challengeInfo.ChallengeId] = challengeInfo;
			}
		}
		MaxKnownChallenges = challengeDictionary.Count;
		ChallengeManagerStatus = ChallengeManagerStateEnum.Inactive;
		ChallengeServer.Initialize();
	}

	public void Start()
	{
		enabled = Singleton<Entitlements>.instance.PermissionCheck(Entitlements.EntitlementFlagEnum.MaxChallengeLevel);
		if (enabled && listenersRegistered)
		{
		}
	}

	public void Suspend()
	{
		if (listenersRegistered)
		{
			AppShell.Instance.EventMgr.RemoveListener<ChallengeEventMessage>(OnChallengeEventMessage);
			AppShell.Instance.EventMgr.RemoveListener<ChallengeServerMessage>(ProcessServerChallengeAckMessage);
			listenersRegistered = false;
		}
		enabled = false;
	}

	~ChallengeManager()
	{
		Suspend();
	}

	private void OnChallengeEventMessage(ChallengeEventMessage message)
	{
		if (challengeManagerStatus == ChallengeManagerStateEnum.ChallengeInProgress && (CurrentChallenge.ChallengeValidationSource != ChallengeValidationEnum.Server || CurrentChallenge.ForceClientEvents) && CurrentChallenge.Status == ChallengeStatus.InProgress)
		{
			CurrentChallenge.HandleChallengeEvent(message);
		}
	}

	private void OnChallengeComplete(IChallenge challenge)
	{
	}

	public void ProcessServerChallengeAckMessage(ChallengeServerMessage message)
	{
	}

	public void RewardSelected(ChallengeInfo challengeInfo, string hero)
	{
		RewardSelected(challengeInfo, hero, OnManualConsumedPotion);
	}

	protected void RewardSelected(ChallengeInfo challengeInfo, string hero, ConsumedPotionDelegate callback)
	{
		if (potionConsumeCallbacks.Count > 0)
		{
			CspUtils.DebugLog("Consume Potion already being performed. This is not currently ok.");
			return;
		}
		if (challengeInfo.Reward.rewardType != ChallengeRewardType.Hero)
		{
			CspUtils.DebugLog("challenge does not have a hero award.");
			return;
		}
		ChallengeManagerStatus = ChallengeManagerStateEnum.ChallengeRewardPending;
		int key = AppShell.Instance.ExpendablesManager.UseExpendable(challengeInfo.Reward.value, hero, OnConsumedPotionMessage);
		potionConsumeCallbacks[key] = callback;
		AppShell.Instance.EventMgr.Fire(null, new ChallengeRewardSelectedMessage(hero));
	}

	private void OnConsumedPotionMessage(IExpendHandler handler)
	{
	}

	private void OnAutoConsumedPotion(IExpendHandler handler)
	{
	}

	public void OnManualConsumedPotion(IExpendHandler handler)
	{
	}

	private void OnProfileLoaded(UserProfile profile)
	{
		if (ChallengeManagerStatus != ChallengeManagerStateEnum.Inactive)
		{
			CspUtils.DebugLog("Asked to load profile when manager already initialized and in progress.");
			return;
		}
		WatcherCounter = AppShell.Instance.CounterManager.GetCounter("ChallengeWatcherCounter");
		if (WatcherCounter == null)
		{
			CspUtils.DebugLog("Watcher Counter absent. Can't properly handle challenge state");
			return;
		}
		NextChallengeServerId = profile.CurrentChallenge;
		LastViewedChallengeId = profile.LastChallenge;
		Start();
		if (LastViewedChallengeId < NextChallengeServerId - 1)
		{
			ChallengeManagerStatus = ChallengeManagerStateEnum.ChallengeDisplayPending;
		}
		else if (!Singleton<Entitlements>.instance.MaxCheck(Entitlements.EntitlementFlagEnum.MaxChallengeLevel, NextChallengeServerId))
		{
			ChallengeManagerStatus = ChallengeManagerStateEnum.AllChallengesCompleted;
		}
		else
		{
			SetActiveChallenge(NextChallengeServerId);
		}
	}

	public void SetViewedChallenge(int serverChallengeId, OnChallengeViewedResponse callback)
	{
		CspUtils.DebugLog("SetViewedChallenge " + serverChallengeId + " " + NextChallengeServerId);
		if (ChallengeManagerStatus != ChallengeManagerStateEnum.ChallengeDisplayPending && ChallengeManagerStatus != ChallengeManagerStateEnum.ChallengeRewardPending)
		{
			CspUtils.DebugLog("Challenge manager not in a state where it expects a display message.");
			return;
		}
		if (serverChallengeId > NextChallengeServerId)
		{
			CspUtils.DebugLog("Server Challenge Id (" + serverChallengeId + ") passed in is out of bounds of the current active id (" + NextChallengeServerId + ")");
			return;
		}
		if (serverChallengeId <= LastViewedChallengeId)
		{
			CspUtils.DebugLog("Server Challenge Id (" + serverChallengeId + ") passed in equal to or less than already viewed challenge (" + LastViewedChallengeId + ")");
			return;
		}
		string uri = string.Format("{0}/{1}/{2}/{3}/{4}/{5}/", "resources$", "users/", AppShell.Instance.Profile.UserId, "challenge", "celebrated", serverChallengeId);
		AppShell.Instance.WebService.StartRequest(uri, delegate(ShsWebResponse response)
		{
			if (response.Status != 200)
			{
				CspUtils.DebugLog("Challenge Celebration Reporting error: " + response.Status + ":" + response.Body);
				if (callback != null)
				{
					callback(false);
				}
			}
			else
			{
				LastViewedChallengeId = Math.Min(NextChallengeServerId - 1, serverChallengeId);
				AppShell.Instance.EventMgr.Fire(this, new ChallengeViewedMessage(LastViewedChallengeId));
				if (LastViewedChallengeId == NextChallengeServerId - 1)
				{
					if (Singleton<Entitlements>.instance.MaxCheck(Entitlements.EntitlementFlagEnum.MaxChallengeLevel, NextChallengeServerId))
					{
						SetActiveChallenge(NextChallengeServerId);
					}
					else
					{
						ChallengeManagerStatus = ChallengeManagerStateEnum.AllChallengesCompleted;
					}
				}
				else
				{
					ChallengeManagerStatus = ChallengeManagerStateEnum.ChallengeDisplayPending;
				}
				if (callback != null)
				{
					callback(true);
				}
			}
		}, null, ShsWebService.ShsWebServiceType.RASP);
	}

	public void SetActiveChallenge(int serverChallengeId)
	{
		if (CurrentChallenge != null)
		{
			CurrentChallenge.Dispose();
			CurrentChallenge = null;
			ChallengeManagerStatus = ChallengeManagerStateEnum.Inactive;
		}
		if (!Singleton<Entitlements>.instance.MaxCheck(Entitlements.EntitlementFlagEnum.MaxChallengeLevel, NextChallengeServerId))
		{
			ChallengeManagerStatus = ChallengeManagerStateEnum.AllChallengesCompleted;
		}
		else
		{
			if (!challengeDictionary.ContainsKey(serverChallengeId))
			{
				return;
			}
			IChallenge challenge = CreateChallengeFromInfo(challengeDictionary[serverChallengeId]);
			if (challenge != null)
			{
				CurrentChallenge = challenge;
				ChallengeManagerStatus = ChallengeManagerStateEnum.ChallengeInProgress;
				if (ChallengeManagerServerStub.CurrentMode == ChallengeManagerServerStub.ServerMode.ClientSimulation)
				{
					PlayerPrefs.SetInt("CM_CC", serverChallengeId);
				}
				CurrentChallenge.Ready();
			}
			else
			{
				ChallengeManagerStatus = ChallengeManagerStateEnum.Inactive;
			}
		}
	}

	public void GetChallengeManagerDisplayInfo(out ChallengeManagerStateEnum currentState, out IChallenge challenge, out ChallengeInfo challengeInfo)
	{
		currentState = challengeManagerStatus;
		challenge = CurrentChallenge;
		challengeInfo = null;
		if ((challengeManagerStatus == ChallengeManagerStateEnum.ChallengeDisplayPending || challengeManagerStatus == ChallengeManagerStateEnum.ChallengeRewardPending) && challengeDictionary.ContainsKey(LastViewedChallengeId + 1))
		{
			challengeInfo = challengeDictionary[LastViewedChallengeId + 1];
		}
	}

	private IChallenge CreateChallengeFromInfo(ChallengeInfo challengeInfo)
	{
		string className = challengeInfo.ClassName;
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		Type type = executingAssembly.GetType(className, false, true);
		if (type == null)
		{
			CspUtils.DebugLog("Cant create class from name: " + className);
			return null;
		}
		if (!typeof(IChallenge).IsAssignableFrom(type))
		{
			CspUtils.DebugLog("Type: " + className + " does not implement the IChallenge interface.");
			return null;
		}
		IChallenge challenge = Activator.CreateInstance(type) as IChallenge;
		if (challenge == null)
		{
			CspUtils.DebugLog("Attempt to create instance of challenge " + className + "failed");
			return null;
		}
		ISHSCounterType counter = AppShell.Instance.CounterManager.GetCounter("ChallengeCounter");
		challenge.Initialize(this, challengeInfo, counter, OnChallengeComplete);
		return challenge;
	}

	public void Update()
	{
		if (!profileLoaded)
		{
			if (AppShell.Instance.Profile != null && AppShell.Instance.CounterManager.CountersLoaded)
			{
				profileLoaded = true;
				OnProfileLoaded(AppShell.Instance.Profile);
			}
		}
		else if (enabled)
		{
			ChallengeServer.Update();
			FanfareManager.Update();
			reconciler.Update();
		}
	}

	public bool IsChallengeCompleted(int challengeId)
	{
		if (CurrentChallenge == null)
		{
			if (challengeId >= MaxKnownChallenges)
			{
				return true;
			}
			if (challengeManagerStatus == ChallengeManagerStateEnum.ChallengeDisplayPending && challengeId == LastViewedChallengeId + 1)
			{
				return true;
			}
		}
		if (CurrentChallenge != null && challengeId < CurrentChallenge.Id)
		{
			return true;
		}
		return false;
	}

	public void GetChallengeProgressWindow(GUISimpleControlWindow window, int challengeId, bool largeWindow)
	{
		if (!challengeDictionary.ContainsKey(challengeId))
		{
			return;
		}
		ChallengeInfo challengeInfo = challengeDictionary[challengeId];
		if (challengeInfo.DisplayNode == null)
		{
			return;
		}
		XmlNode xmlNode = challengeInfo.DisplayNode["method"];
		if (xmlNode == null)
		{
			return;
		}
		string innerText = xmlNode.InnerText;
		if (innerText != null)
		{
			Type typeFromHandle = typeof(ChallengeProgressView);
			MethodInfo method = typeFromHandle.GetMethod(innerText, BindingFlags.Static | BindingFlags.Public);
			if (method != null)
			{
				method.Invoke(null, new object[3]
				{
					window,
					challengeInfo,
					largeWindow
				});
			}
		}
	}

	public ChallengeInfo GetHeroChallenge(string heroName)
	{
		foreach (ChallengeInfo value in ChallengeDictionary.Values)
		{
			if (value.Reward.rewardType == ChallengeRewardType.Hero && value.Reward.grantMode == ChallengeGrantMode.Auto && value.Reward.qualifier == heroName)
			{
				return value;
			}
		}
		return null;
	}

	public int GetExpectedManualHeroCount(int targetChallengeId)
	{
		return GetExpectedManualHeroCount(targetChallengeId, GetManualHeroRewardChallengeIds());
	}

	public int GetExpectedManualHeroCount(int targetChallengeId, int[] manualHeroChallengeIds)
	{
		int num = 0;
		if (manualHeroChallengeIds != null)
		{
			foreach (int num2 in manualHeroChallengeIds)
			{
				if (num2 <= targetChallengeId)
				{
					num++;
				}
			}
		}
		return num;
	}

	public int GetOwnedManualHeroCount()
	{
		int num = 0;
		UserProfile profile = AppShell.Instance.Profile;
		if (profile == null)
		{
			return num;
		}
		string text = string.Empty;
		foreach (KeyValuePair<int, ChallengeInfo> item in challengeDictionary)
		{
			ChallengeInfo value = item.Value;
			if (value != null && value.Reward != null && value.Reward.rewardType == ChallengeRewardType.Hero && value.Reward.grantMode == ChallengeGrantMode.Manual)
			{
				text = value.Reward.qualifier;
				break;
			}
		}
		if (string.IsNullOrEmpty(text))
		{
			return num;
		}
		string[] array = text.Split(';');
		string[] array2 = array;
		foreach (string key in array2)
		{
			if (profile.AvailableCostumes.ContainsKey(key))
			{
				num++;
			}
		}
		return num;
	}

	public int[] GetManualHeroRewardChallengeIds()
	{
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, ChallengeInfo> item in challengeDictionary)
		{
			ChallengeInfo value = item.Value;
			if (value != null && value.Reward != null && value.Reward.rewardType == ChallengeRewardType.Hero && value.Reward.grantMode == ChallengeGrantMode.Manual)
			{
				list.Add(value.ChallengeId);
			}
		}
		return list.ToArray();
	}

	public bool IsManualHeroRewardChallengeId(int serverChallengeId)
	{
		if (!challengeDictionary.ContainsKey(serverChallengeId))
		{
			return false;
		}
		ChallengeInfo challengeInfo = challengeDictionary[serverChallengeId];
		if (challengeInfo == null || challengeInfo.Reward == null)
		{
			return false;
		}
		return challengeInfo.Reward.grantMode == ChallengeGrantMode.Manual && challengeInfo.Reward.rewardType == ChallengeRewardType.Hero;
	}

	public void OverrideChallenge(ChallengeInfo info)
	{
		if (ChallengeManagerServerStub.CurrentMode == ChallengeManagerServerStub.ServerMode.ClientSimulation)
		{
			if (CurrentChallenge != null)
			{
				CurrentChallenge.Dispose();
				CurrentChallenge = null;
				ChallengeManagerStatus = ChallengeManagerStateEnum.Inactive;
			}
			FanfareManager.FanfareQueue.Clear();
			ChallengeServer.ChallengesPending.Clear();
			SetActiveChallenge(info.ChallengeId);
			LastViewedChallengeId = info.ChallengeId - 1;
		}
		else
		{
			CspUtils.DebugLog("Overriding challenges is only available in client simulation mode.");
		}
	}

	public void ForceChallengeComplete()
	{
		if (CurrentChallenge != null)
		{
			OnChallengeComplete(CurrentChallenge);
		}
	}
}
