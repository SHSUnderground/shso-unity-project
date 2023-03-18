using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager
{
	public enum DestinyTracks
	{
		None = -1,
		Valor,
		Conquest,
		Might
	}

	public enum EnemyType
	{
		Mob = 1,
		Boss
	}

	public static Dictionary<int, NewAchievement> allAchievements = new Dictionary<int, NewAchievement>();

	public static Dictionary<int, PlayerAchievementData> allPlayerData = new Dictionary<int, PlayerAchievementData>();

	public static List<AchievementDisplayGroup> rootGroups = new List<AchievementDisplayGroup>();

	public static Dictionary<int, AchievementDisplayGroup> allGroups = new Dictionary<int, AchievementDisplayGroup>();

	public static Dictionary<int, List<NewAchievement>> _dependentAchievements = new Dictionary<int, List<NewAchievement>>();

	public static int INVALID_VALUE = -10000;

	public static string tutorialURL = string.Empty;

	public static Dictionary<DestinyTracks, List<NewAchievement>> destinyTrackAchievements = new Dictionary<DestinyTracks, List<NewAchievement>>();

	private static Dictionary<DestinyTracks, int> destinyTrackStartingIDs = new Dictionary<DestinyTracks, int>();

	private static Dictionary<int, int> _stepIDToAchievementID = new Dictionary<int, int>();

	private static AchievementManager _instance;

	public static int totalAchievements = 0;

	public static Dictionary<int, int> totalAchievementsByGroup = new Dictionary<int, int>();

	public static Dictionary<int, AchievementReward> rewards = new Dictionary<int, AchievementReward>();

	private static Dictionary<int, bool> trackedAchievementIDs = new Dictionary<int, bool>();

	private float lastAchievementCheckTime = -1000f;

	private static List<DelayedAchievementEvent> _delayedEvents = new List<DelayedAchievementEvent>();

	public static bool ACHIEVEMENTS_LOADED = false;

	private static int iterCount = 0;

	private static int lastIterCount = -1;

	private MasterAchievementJsonData jsonDict;

	private int stage;

	private static Dictionary<string, int> _mobKillRecord = new Dictionary<string, int>();

	private static int _killSubmitThreshold = 20;

	public AchievementManager()
	{
		_instance = this;
	}

	public void begin()
	{
		AchievementDisplayGroup achievementDisplayGroup = addGroup(new AchievementDisplayGroup(-10, 0, "Overview", "Overview Desc"));
		achievementDisplayGroup = addGroup(new AchievementDisplayGroup(1, 0, "General", "General Desc"));
		addGroup(new AchievementDisplayGroup(100, achievementDisplayGroup.groupID, "Exploration", "Events Desc"));
		addGroup(new AchievementDisplayGroup(101, achievementDisplayGroup.groupID, "Emotes", "Emotes Desc"));
		addGroup(new AchievementDisplayGroup(103, achievementDisplayGroup.groupID, "Titles", "Titles Desc"));
		addGroup(new AchievementDisplayGroup(104, achievementDisplayGroup.groupID, "Crafting", "Crafting Desc"));
		addGroup(new AchievementDisplayGroup(106, achievementDisplayGroup.groupID, "Mystery Boxes", "Mystery Desc"));
		addGroup(new AchievementDisplayGroup(108, achievementDisplayGroup.groupID, "Halloween", "EvHalloweenents Desc"));
		achievementDisplayGroup = addGroup(new AchievementDisplayGroup(2, 0, "Squad", "Squad Desc"));
		addGroup(new AchievementDisplayGroup(200, achievementDisplayGroup.groupID, "General", "Squad General Desc"));
		addGroup(new AchievementDisplayGroup(201, achievementDisplayGroup.groupID, "Activities", "Activities Desc"));
		addGroup(new AchievementDisplayGroup(202, achievementDisplayGroup.groupID, "Sidekicks", "Sidekicks Desc"));
		addGroup(new AchievementDisplayGroup(205, achievementDisplayGroup.groupID, "Defeat Villains", "Defeat Villains Desc"));
		achievementDisplayGroup = addGroup(new MissionAchievementDisplayGroup(3, 0, "Missions", "Missions Desc"));
		addGroup(new AchievementDisplayGroup(300, achievementDisplayGroup.groupID, "Mission General", "Mission General Desc"));
		addGroup(new AchievementDisplayGroup(302, achievementDisplayGroup.groupID, "Survival", "Survival Desc"));
		MissionAchievementDisplayGroup missionAchievementDisplayGroup = achievementDisplayGroup as MissionAchievementDisplayGroup;
		achievementDisplayGroup = addGroup(new HeroAchievementDisplayGroup(4, 0, "Heroes", "Heroes Desc"));
		HeroAchievementDisplayGroup heroAchievementDisplayGroup = achievementDisplayGroup as HeroAchievementDisplayGroup;
		achievementDisplayGroup = addGroup(new AchievementDisplayGroup(5, 0, "Destiny", "Destiny Desc"));
		addGroup(new AchievementDisplayGroup(501, achievementDisplayGroup.groupID, "Valor", "Valor Destiny Desc"));
		addGroup(new AchievementDisplayGroup(502, achievementDisplayGroup.groupID, "Solo Conquest", "Conquest Destiny Desc"));
		achievementDisplayGroup = addGroup(new AchievementDisplayGroup(7, 0, "Card Game", "Card Game Desc"));
		addGroup(new AchievementDisplayGroup(701, achievementDisplayGroup.groupID, "Quests", "Quests Desc"));
		addGroup(new AchievementDisplayGroup(702, achievementDisplayGroup.groupID, "Player vs Player", "Player vs Player Desc"));
		foreach (OwnableDefinition item in OwnableDefinition.HeroesByName)
		{
			AchievementDisplayGroup achievementDisplayGroup2 = heroAchievementDisplayGroup.addGroupForHero(item.name);
			achievementDisplayGroup2.restricted = (item.released == 0);
		}
		foreach (string missionBossName in OwnableDefinition.MissionBossNames)
		{
			missionAchievementDisplayGroup.addGroupForMission(missionBossName);
		}
		destinyTrackAchievements.Add(DestinyTracks.Valor, new List<NewAchievement>());
		destinyTrackAchievements.Add(DestinyTracks.Conquest, new List<NewAchievement>());
		destinyTrackAchievements.Add(DestinyTracks.Might, new List<NewAchievement>());
		destinyTrackStartingIDs.Add(DestinyTracks.Valor, 200000);
		destinyTrackStartingIDs.Add(DestinyTracks.Conquest, 210000);
		destinyTrackStartingIDs.Add(DestinyTracks.Might, 220000);
		stage = 1;
	}

	public void masterLoadFailed()
	{
		stage = 1;
	}

	public void Update()
	{
		switch (stage)
		{
		case 1:
			CspUtils.DebugLog("LoadMasterAchievementData()");
			AppShell.Instance.ServerConnection.LoadMasterAchievementData();
			stage = 2;
			lastAchievementCheckTime = Time.time;
			break;
		case 2:
			if (Time.time - lastAchievementCheckTime > 30f)
			{
				CspUtils.DebugLog("LoadMasterAchievementData() appears to have timed out or died (30 seconds passed) re-attempting");
				stage = 1;
			}
			break;
		case 3:
			if (jsonDict != null)
			{
				stage = 4;
			}
			break;
		case 4:
			CspUtils.DebugLog("beginning parseAchievementContent");
			AppShell.Instance.StartCoroutine(parseAchievementContent(jsonDict));
			stage = 5;
			lastAchievementCheckTime = Time.time;
			lastIterCount = -1;
			break;
		case 5:
			if (!(Time.time - lastAchievementCheckTime > 2f))
			{
				break;
			}
			CspUtils.DebugLog("parseAchievementContent monitor heartbeat");
			if (iterCount == lastIterCount)
			{
				if (!ACHIEVEMENTS_LOADED)
				{
					CspUtils.DebugLogError("looks like the achievement parse failed, trying again");
					stage = 4;
				}
				else
				{
					stage = 6;
				}
			}
			lastAchievementCheckTime = Time.time;
			lastIterCount = iterCount;
			break;
		case 6:
			CspUtils.DebugLog("Achievements loaded, loading player's achievement data");
			AppShell.Instance.ServerConnection.LoadAchievementData((int)AppShell.Instance.Profile.UserId);
			stage = 7;
			break;
		}
		if (stage != 7)
		{
			return;
		}
		int num = 0;
		while (num < _delayedEvents.Count)
		{
			if (_delayedEvents[num].tick(Time.time))
			{
				_delayedEvents.RemoveAt(num);
			}
			else
			{
				num++;
			}
		}
	}

	public void queueAchievementEvent(string heroStr, string type, string subtype, string str1, float delay = 3f)
	{
		_delayedEvents.Add(new DelayedAchievementEvent(heroStr, type, subtype, str1, delay));
	}

	public static void spawnTrackerForAchievement(int achievementID)
	{
		NewAchievement newAchievement = allAchievements[achievementID];
		if (newAchievement.track != DestinyTracks.None && hasPlayerCompletedAchievement((int)AppShell.Instance.Profile.UserId, newAchievement.achievementID))
		{
			stopTrackingAchievement(achievementID);
			int num = 0;
			NewAchievement newAchievement2;
			while (true)
			{
				if (num < destinyTrackAchievements[newAchievement.track].Count)
				{
					newAchievement2 = destinyTrackAchievements[newAchievement.track][num];
					if (!hasPlayerCompletedAchievement((int)AppShell.Instance.Profile.UserId, newAchievement2.achievementID))
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			trackAchievement(newAchievement2.achievementID);
		}
		else
		{
			AchievementTrackerNotificationData data = new AchievementTrackerNotificationData(achievementID, trackedAchievementIDs[achievementID]);
			NotificationHUD.addNotification(data);
		}
	}

	public static bool isTrackingAchievement(int achievementID)
	{
		return trackedAchievementIDs.ContainsKey(achievementID);
	}

	public static bool isTrackedAchievementCollapsed(int achievementID)
	{
		if (trackedAchievementIDs.ContainsKey(achievementID))
		{
			return trackedAchievementIDs[achievementID];
		}
		return false;
	}

	private static void updateTrackerData()
	{
		string text = "0";
		foreach (int key in trackedAchievementIDs.Keys)
		{
			text = text + "," + key;
		}
		AppShell.Instance.Profile.trackerData = text;
		CspUtils.DebugLog("updateTrackerData " + AppShell.Instance.Profile.trackerData);
		WWWForm wWWForm = new WWWForm();
		wWWForm.AddField("tracker_data", AppShell.Instance.Profile.trackerData);
		AppShell.Instance.WebService.StartRequest("resources$users/" + AppShell.Instance.Profile.UserId + "/set_tracker_info/", delegate(ShsWebResponse response)
		{
			if (response.Status != 200)
			{
				CspUtils.DebugLog("AchievementManager  updateTrackerData failure: " + response.Status + ":" + response.Body);
			}
			else
			{
				CspUtils.DebugLog("AchievementManager updateTrackerData success!: " + response.Status + ":" + response.Body);
			}
		}, wWWForm.data);
	}

	public static void trackAchievement(int achievementID)
	{
		if (!trackedAchievementIDs.ContainsKey(achievementID))
		{
			trackedAchievementIDs[achievementID] = false;
			spawnTrackerForAchievement(achievementID);
			updateTrackerData();
		}
	}

	public static void stopTrackingAchievement(int achievementID)
	{
		trackedAchievementIDs.Remove(achievementID);
		AppShell.Instance.EventMgr.Fire(_instance, new StopTrackingAchievementMessage(achievementID));
		updateTrackerData();
	}

	public static void trackedAchievementCollapse(int achievementID)
	{
		if (trackedAchievementIDs.ContainsKey(achievementID))
		{
			trackedAchievementIDs[achievementID] = true;
		}
	}

	public static void trackedAchievementExpand(int achievementID)
	{
		if (trackedAchievementIDs.ContainsKey(achievementID))
		{
			trackedAchievementIDs[achievementID] = false;
		}
	}

	private static void OnAchievementCompleteMessage(AchievementCompleteMessage msg)
	{
		AppShell.callAnalytics("player", "achievement", "complete", string.Empty + msg.achievementID);
		NewAchievement newAchievement = allAchievements[msg.achievementID];
		AchievementData achievementData = getAchievementData((int)AppShell.Instance.Profile.UserId, msg.achievementID);
		if (achievementData != null)
		{
			achievementData.complete = 1;
		}
		AppShell.Instance.Profile.achievementPoints = allPlayerData[(int)AppShell.Instance.Profile.UserId].achievementPoints;
		CspUtils.DebugLog("AchievementManager OnAchievementCompleteMessage " + newAchievement.track);
		stopTrackingAchievement(msg.achievementID);
		if (msg.achievementID == 200030)
		{
			trackAchievement(210000);
		}
		if (newAchievement.track == DestinyTracks.None)
		{
			return;
		}
		int num = 0;
		NewAchievement newAchievement2;
		while (true)
		{
			if (num >= destinyTrackAchievements[newAchievement.track].Count)
			{
				return;
			}
			newAchievement2 = destinyTrackAchievements[newAchievement.track][num];
			if (newAchievement2.achievementID == newAchievement.achievementID)
			{
				num++;
				CspUtils.DebugLog("found match " + num + " " + destinyTrackAchievements[newAchievement.track].Count);
				if (num < destinyTrackAchievements[newAchievement.track].Count)
				{
					break;
				}
			}
			num++;
		}
		newAchievement2 = destinyTrackAchievements[newAchievement.track][num];
		trackAchievement(newAchievement2.achievementID);
	}

	public static int getAchievementIDFromStepID(int stepID)
	{
		if (!_stepIDToAchievementID.ContainsKey(stepID))
		{
			CspUtils.DebugLog("getAchievementIDFromStepID got step ID " + stepID + " that has no corresponding achievement ID!");
			return -1;
		}
		return _stepIDToAchievementID[stepID];
	}

	private static AchievementDisplayGroup addGroup(AchievementDisplayGroup group)
	{
		allGroups.Add(group.groupID, group);
		if (group.parentGroupID == 0)
		{
			rootGroups.Add(group);
		}
		else
		{
			AchievementDisplayGroup achievementDisplayGroup = allGroups[group.parentGroupID];
			achievementDisplayGroup.addChildGroup(group);
		}
		return group;
	}

	public static void initDestinyInfoForProfile(UserProfile profile)
	{
		profile.currentDestinyIDs[DestinyTracks.Conquest] = -1;
		profile.currentDestinyIDs[DestinyTracks.Might] = -1;
		profile.currentDestinyIDs[DestinyTracks.Valor] = -1;
		if (allPlayerData.ContainsKey((int)profile.UserId))
		{
			PlayerAchievementData playerAchievementData = allPlayerData[(int)profile.UserId];
			foreach (DestinyTracks key in destinyTrackStartingIDs.Keys)
			{
				if (key != DestinyTracks.None)
				{
					int num = 0;
					List<NewAchievement> list = destinyTrackAchievements[key];
					if (list.Count > 0)
					{
						for (NewAchievement newAchievement = list[0]; newAchievement != null; newAchievement = list[num])
						{
							profile.currentDestinyIDs[key] = newAchievement.achievementID;
							if (!hasPlayerCompletedAchievement((int)profile.UserId, newAchievement.achievementID))
							{
								break;
							}
							num++;
							if (num >= list.Count)
							{
								CspUtils.DebugLog("player appears to have completed track " + key);
								break;
							}
						}
						CspUtils.DebugLog("player is on achievement " + profile.currentDestinyIDs[key] + " for track " + key);
					}
				}
			}
		}
	}

	public void parseAchievements(string list)
	{
		CspUtils.DebugLog("parseAchievements begins");
		jsonDict = JsonMapper.ToObject<MasterAchievementJsonData>(list);
		CspUtils.DebugLog("parseAchievements check 1");
		totalAchievementsByGroup.Add(0, 0);
		foreach (AchievementDisplayGroup value in allGroups.Values)
		{
			totalAchievementsByGroup.Add(value.groupID, 0);
		}
		CspUtils.DebugLog("parseAchievements pre-check work complete, beginning loop");
		stage = 4;
	}

	private static IEnumerator parseAchievementContent(MasterAchievementJsonData jsonDict)
	{
		CspUtils.DebugLog("parseAchievementContent check 1");
		IEnumerable<AchievementJsonData> achievements = jsonDict.achievements;
		Dictionary<int, NewAchievement> prereqIDDict = new Dictionary<int, NewAchievement>();
		int counter2 = 0;
		foreach (AchievementJsonData achievementData in achievements)
		{
			iterCount++;
			if (!allAchievements.ContainsKey(achievementData.id))
			{
				NewAchievement achievement2 = new NewAchievement(achievementData);
				allAchievements.Add(achievement2.achievementID, achievement2);
				if (achievement2.prereqAchievementID != INVALID_VALUE)
				{
					if (!prereqIDDict.ContainsKey(achievement2.prereqAchievementID))
					{
						prereqIDDict.Add(achievement2.prereqAchievementID, achievement2);
					}
					if (!_dependentAchievements.ContainsKey(achievement2.prereqAchievementID))
					{
						_dependentAchievements.Add(achievement2.prereqAchievementID, new List<NewAchievement>());
					}
					_dependentAchievements[achievement2.prereqAchievementID].Add(achievement2);
				}
				if (achievement2.displayGroupID != -1)
				{
					if (achievement2.displayGroupID == 0)
					{
						CspUtils.DebugLog("received an achievement with displayGroupID of 0!  FIX THIS.  Either make it -1 to hide it, or assign a proper group");
					}
					else if (!allGroups.ContainsKey(achievement2.displayGroupID))
					{
						CspUtils.DebugLog("received an achievement with an unknown displayGroupID of " + achievement2.displayGroupID + "  This achievement will not display properly");
					}
					else
					{
						AchievementDisplayGroup group = allGroups[achievement2.displayGroupID];
						group.addAchievement(achievement2);
					}
				}
				if (achievement2.hidden != 1 && achievement2.enabled == 1)
				{
					totalAchievements++;
					int rootGroupID = getRootGroupID(achievement2.displayGroupID);
					Dictionary<int, int> dictionary;
					Dictionary<int, int> dictionary2 = dictionary = totalAchievementsByGroup;
					int key;
					int key2 = key = rootGroupID;
					key = dictionary[key];
					dictionary2[key2] = key + 1;
					if (rootGroupID != achievement2.displayGroupID)
					{
						Dictionary<int, int> dictionary3;
						Dictionary<int, int> dictionary4 = dictionary3 = totalAchievementsByGroup;
						int key3 = key = achievement2.displayGroupID;
						key = dictionary3[key];
						dictionary4[key3] = key + 1;
					}
					if (achievement2.achievementID == 200000)
					{
						tutorialURL = achievement2.helpURL;
					}
				}
				counter2++;
				if (counter2 >= 100)
				{
					CspUtils.DebugLog("parseAchievementContent yield 1");
					counter2 = 0;
					yield return new WaitForSeconds(0.1f);
				}
			}
		}
		CspUtils.DebugLog("parseAchievementContent check 4");
		counter2 = 0;
		IEnumerable<AchievementStepJsonData> steps = jsonDict.steps;
		foreach (AchievementStepJsonData stepData in steps)
		{
			iterCount++;
			if (!_stepIDToAchievementID.ContainsKey(stepData.id))
			{
				NewAchievementStep step = new NewAchievementStep(stepData);
				_stepIDToAchievementID.Add(step.achievementStepID, step.achievementID);
				if (!allAchievements.ContainsKey(step.achievementID))
				{
					CspUtils.DebugLog("step has bad achievementID.  step " + step.achievementStepID + " with achievementID " + step.achievementID);
				}
				NewAchievement achievement2 = allAchievements[step.achievementID];
				achievement2.addStep(step);
				counter2++;
				if (counter2 >= 100)
				{
					CspUtils.DebugLog("parseAchievementContent yield 2");
					counter2 = 0;
					yield return new WaitForSeconds(0.1f);
				}
			}
		}
		CspUtils.DebugLog("parseAchievementContent check 5");
		IEnumerable<AchievementRewardJsonData> rewardList = jsonDict.rewards;
		foreach (AchievementRewardJsonData rewardData in rewardList)
		{
			iterCount++;
			AchievementReward reward = new AchievementReward(rewardData);
			rewards.Add(reward.achievementRewardID, reward);
		}
		CspUtils.DebugLog("parseAchievementContent check 6");
		foreach (DestinyTracks track in destinyTrackStartingIDs.Keys)
		{
			if (track != DestinyTracks.None)
			{
				int currentID = destinyTrackStartingIDs[track];
				while (true)
				{
					iterCount++;
					if (!allAchievements.ContainsKey(currentID))
					{
						break;
					}
					NewAchievement currentAchievement = allAchievements[currentID];
					destinyTrackAchievements[track].Add(currentAchievement);
					currentAchievement.track = track;
					if (!prereqIDDict.ContainsKey(currentID))
					{
						break;
					}
					currentID = prereqIDDict[currentID].achievementID;
				}
			}
		}
		CspUtils.DebugLog("parseAchievementContent check 7");
		string[] rawTrackerSplit = AppShell.Instance.Profile.trackerData.Split(',');
		string[] array = rawTrackerSplit;
		foreach (string rawID in array)
		{
			iterCount++;
			int achID = int.Parse(rawID);
			if (!allAchievements.ContainsKey(achID))
			{
				CspUtils.DebugLog("invalid achievement found in tracker data: " + achID);
			}
			else
			{
				trackedAchievementIDs[achID] = false;
			}
		}
		CspUtils.DebugLog("parseAchievementContent check 8");
		AppShell.Instance.EventMgr.AddListener<AchievementCompleteMessage>(OnAchievementCompleteMessage);
		ACHIEVEMENTS_LOADED = true;
		CspUtils.DebugLog("Achievements parsed, ACHIEVEMENTS_LOADED is now true");
	}

	public static int getRootGroupID(int displayGroupID)
	{
		if (!allGroups.ContainsKey(displayGroupID))
		{
			return 0;
		}
		AchievementDisplayGroup achievementDisplayGroup = allGroups[displayGroupID];
		while (achievementDisplayGroup.parentGroupID != 0)
		{
			achievementDisplayGroup = allGroups[achievementDisplayGroup.parentGroupID];
		}
		return achievementDisplayGroup.groupID;
	}

	public static PlayerAchievementData parseAchievementData(string list)
	{
		////// block added by CSP to temporarily fix ach data ///////
		string correctString = list.Replace("3870526", AppShell.Instance.Profile.UserId.ToString());
		list = correctString;
		CspUtils.DebugLog("list= " + list);
		/////////////////////////////////

		MasterAchievementDataJsonData masterAchievementDataJsonData = JsonMapper.ToObject<MasterAchievementDataJsonData>(list);
		CspUtils.DebugLog("parseAchievementData " + masterAchievementDataJsonData.playerID);
		int num = int.Parse(masterAchievementDataJsonData.playerID);
		PlayerAchievementData playerAchievementData;
		if (allPlayerData.ContainsKey(num))
		{
			playerAchievementData = allPlayerData[num];
			//playerAchievementData = allPlayerData[3870526];   // CSP always force 3870526 for now
			playerAchievementData.reset();
		}
		else
		{
			playerAchievementData = new PlayerAchievementData(num);
			allPlayerData.Add(num, playerAchievementData);
		}
		IEnumerable<AchievementDataJsonData> data = masterAchievementDataJsonData.data;
		foreach (AchievementDataJsonData item in data)
		{
			AchievementData data2 = new AchievementData(item);
			playerAchievementData.addData(data2);
		}
		IEnumerable<AchievementStepDataJsonData> step_data = masterAchievementDataJsonData.step_data;
		foreach (AchievementStepDataJsonData item2 in step_data)
		{
			AchievementStepData data3 = new AchievementStepData(item2);
			playerAchievementData.addData(data3);
		}
		IEnumerable<AchievementStepDataExJsonData> step_data_ex = masterAchievementDataJsonData.step_data_ex;
		foreach (AchievementStepDataExJsonData item3 in step_data_ex)
		{
			AchievementStepDataEx data4 = new AchievementStepDataEx(item3);
			playerAchievementData.addData(data4);
		}
		if (AppShell.Instance.Profile != null && AppShell.Instance.Profile.UserId == num)
		{
			initDestinyInfoForProfile(AppShell.Instance.Profile);
		}
		return playerAchievementData;
	}

	public static void updateAchievementData(AchievementData data)
	{
		int key = (int)AppShell.Instance.Profile.UserId;
		if (allPlayerData.ContainsKey(key))
		{
			PlayerAchievementData playerAchievementData = allPlayerData[key];
			playerAchievementData.addData(data);
			AppShell.Instance.EventMgr.Fire(null, new AchievementProgressUpdateMessage(data, null, null));
		}
	}

	public static void updateAchievementData(AchievementStepData data)
	{
		int key = (int)AppShell.Instance.Profile.UserId;
		if (allPlayerData.ContainsKey(key))
		{
			PlayerAchievementData playerAchievementData = allPlayerData[key];
			playerAchievementData.addData(data);
			AppShell.Instance.EventMgr.Fire(null, new AchievementProgressUpdateMessage(null, data, null));
		}
	}

	public static void updateAchievementData(AchievementStepDataEx data)
	{
		int key = (int)AppShell.Instance.Profile.UserId;
		if (allPlayerData.ContainsKey(key))
		{
			PlayerAchievementData playerAchievementData = allPlayerData[key];
			playerAchievementData.addData(data);
			AppShell.Instance.EventMgr.Fire(null, new AchievementProgressUpdateMessage(null, null, data));
		}
	}

	public static AchievementData getAchievementData(int userID, int achievementID)
	{
		if (!allPlayerData.ContainsKey(userID))
		{
			return null;
		}
		PlayerAchievementData playerAchievementData = allPlayerData[userID];
		return playerAchievementData.getData(achievementID);
	}

	public static NewAchievement getAchievement(int achievementID)
	{
		if (!allAchievements.ContainsKey(achievementID))
		{
			return null;
		}
		return allAchievements[achievementID];
	}

	public static bool playerOnAchievement(int userID, int achievementID)
	{
		if (hasPlayerCompletedAchievement(userID, achievementID))
		{
			return false;
		}
		NewAchievement achievement = getAchievement(achievementID);
		if (achievement == null)
		{
			return false;
		}
		if (achievement.track == DestinyTracks.None)
		{
			return true;
		}
		achievement = getAchievement(achievementID - 1);
		if (achievement == null)
		{
			return false;
		}
		if (achievement == null)
		{
			return true;
		}
		return hasPlayerCompletedAchievement(userID, achievementID - 1);
	}

	public static bool hasPlayerCompletedAchievement(int userID, int achievementID)
	{
		AchievementData achievementData = getAchievementData(userID, achievementID);
		if (achievementData == null)
		{
			return false;
		}
		return achievementData.complete == 1;
	}

	public static IEnumerator spawnTrackedAchievements()
	{
		bool success = true;
		float startTime = Time.time;
		while (!allPlayerData.ContainsKey((int)AppShell.Instance.Profile.UserId))
		{
			if (Time.time - startTime > 60f)
			{
				CspUtils.DebugLog("failed to get player achievement data in 60 seconds");
				success = false;
				break;
			}
			CspUtils.DebugLog("spawnTrackedAchievements waiting for data to load");
			yield return new WaitForSeconds(1f);
		}
		if (!success)
		{
			yield break;
		}
		if (!hasPlayerCompletedAchievement((int)AppShell.Instance.Profile.UserId, 200000))
		{
			SHSSocialMainWindow socialWindow = GUIManager.Instance["SHSMainWindow/SHSSocialMainWindow"] as SHSSocialMainWindow;
			if (socialWindow != null)
			{
				socialWindow.ShowTutorialVideo(tutorialURL);
			}
		}
		foreach (int trackedAchievementID in trackedAchievementIDs.Keys)
		{
			spawnTrackerForAchievement(trackedAchievementID);
		}
	}

	public static void recordEnemyKill(EnemyType enemyType, string hero, int count)
	{
		if (!_mobKillRecord.ContainsKey(hero))
		{
			_mobKillRecord.Add(hero, 0);
		}
		int num = _mobKillRecord[hero];
		num += count;
		if (num >= _killSubmitThreshold)
		{
			AppShell.Instance.EventReporter.ReportAchievementEvent(hero, "defeat_enemies", string.Empty, _killSubmitThreshold, string.Empty);
			num -= _killSubmitThreshold;
		}
		_mobKillRecord[hero] = num;
	}

	public static List<NewAchievement> getDependentAchievements(int achievementID)
	{
		if (_dependentAchievements.ContainsKey(achievementID))
		{
			return _dependentAchievements[achievementID];
		}
		return null;
	}

	public static void reportTotalKills(string hero)
	{
		CspUtils.DebugLog("reportTotalKills " + hero);
		if (_mobKillRecord.ContainsKey(hero))
		{
			int inc = _mobKillRecord[hero];
			_mobKillRecord[hero] = 0;
			AppShell.Instance.EventReporter.ReportAchievementEvent(hero, "defeat_enemies", string.Empty, inc, string.Empty);
		}
	}

	public static string formatAchievementString(string baseStr, int current = 0, int threshold = 0, string hero = "", int int1 = 0)
	{
		if (baseStr == null || baseStr.Length < 2)
		{
			return baseStr;
		}
		int num = baseStr.IndexOf("#");
		while (num != -1)
		{
			int num2 = baseStr.IndexOf(" ", num);
			string text = (num2 != -1) ? baseStr.Substring(num, num2 - num) : baseStr.Substring(num);
			baseStr = baseStr.Replace(text, AppShell.Instance.stringTable.GetString(text));
			int num3 = num;
			num = baseStr.IndexOf("#", num);
			if (num3 == num)
			{
				num++;
				num = baseStr.IndexOf("#", num);
			}
		}
		baseStr = baseStr.Replace("<c>", string.Empty + current);
		baseStr = baseStr.Replace("<t>", string.Empty + threshold);
		baseStr = baseStr.Replace("<i1>", string.Empty + int1);
		if (hero != null && hero != string.Empty && AppShell.Instance.CharacterDescriptionManager[hero] != null)
		{
			hero = AppShell.Instance.CharacterDescriptionManager[hero].CharacterName;
			baseStr = baseStr.Replace("<h>", string.Empty + hero);
		}
		baseStr = ((threshold <= 1) ? baseStr.Replace("<plural_s>", string.Empty) : baseStr.Replace("<plural_s>", "s"));
		return baseStr;
	}

	public static bool shouldReportAchievementEvent(string eventType, string subType = "", string str1 = "")
	{
		if (subType == "open_door" && !playerOnAchievement((int)AppShell.Instance.Profile.UserId, 200009))
		{
			return false;
		}
		if (subType == "hotspot" && !playerOnAchievement((int)AppShell.Instance.Profile.UserId, 200004) && !playerOnAchievement((int)AppShell.Instance.Profile.UserId, 200028))
		{
			return false;
		}
		if (subType == "swap_hero" && !playerOnAchievement((int)AppShell.Instance.Profile.UserId, 200008) && !playerOnAchievement((int)AppShell.Instance.Profile.UserId, 200027))
		{
			return false;
		}
		if (subType == "open_achievements" && !playerOnAchievement((int)AppShell.Instance.Profile.UserId, 200034))
		{
			return false;
		}
		return true;
	}
}
