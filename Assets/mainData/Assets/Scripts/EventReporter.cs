using System;
using System.Collections;
using System.Collections.Generic;

public class EventReporter
{
	public delegate void OnEventResponse(bool success, Hashtable args);

	protected int correlationId;

	protected Dictionary<int, OnEventResponse> callbacks;

	protected UserProfile UserProfile
	{
		get
		{
			return AppShell.Instance.Profile;
		}
	}

	public EventReporter()
	{
		correlationId = 0;
		callbacks = new Dictionary<int, OnEventResponse>();
	}

	protected void RegisterCallback(int id, OnEventResponse onEventResponse)
	{
		if (onEventResponse != null)
		{
			AppShell.Instance.TimerMgr.CreateTimer(60f, delegate
			{
				OnEventResponse value;
				if (callbacks.TryGetValue(id, out value))
				{
					callbacks.Remove(id);
					value(false, null);
				}
			});
			callbacks.Add(id, onEventResponse);
		}
	}

	public void OnEventResponseMessage(Hashtable args)
	{
		try
		{
			CspUtils.DebugLog("OnEventResponse dump");
			foreach (object key2 in args.Keys)
			{
				CspUtils.DebugLog(key2 + " = " + args[key2]);
			}
			if (args.ContainsKey("trans_id"))
			{
				int key = int.Parse((string)args["trans_id"]);
				OnEventResponse value;
				if (callbacks.TryGetValue(key, out value))
				{
					callbacks.Remove(key);
					value(true, args);
				}
			}
		}
		catch (Exception ex)
		{
			CspUtils.DebugLog("Exception processing event resposne");
			CspUtils.DebugLog(ex.ToString());
		}
	}

	public void ReportCharacterSelection(string character)
	{
		if (UserProfile != null)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("hero", character);
			AppShell.Instance.ServerConnection.ReportEvent("character_selection", hashtable);
		}
	}

	public void ReportTransition(string from, string to)
	{
		if (UserProfile != null)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("from", from);
			hashtable.Add("to", to);
			AppShell.Instance.ServerConnection.ReportEvent("controller_transition", hashtable);
		}
	}

	public void ReportWheelSpin()
	{
		if (UserProfile != null)
		{
			Hashtable args = new Hashtable();
			AppShell.Instance.ServerConnection.ReportEvent("wheel_spin", args);
		}
	}

	public void ReportWheelSpinAll(OnEventResponse onEventResponse)
	{
		if (UserProfile != null)
		{
			int num = ++correlationId;
			RegisterCallback(num, onEventResponse);
			Hashtable hashtable = new Hashtable();
			hashtable.Add("trans_id", num);
			AppShell.Instance.ServerConnection.ReportEvent("wheel_spin_all", hashtable);
		}
	}

	public void ReportAwardTokens(int coins)
	{
		if (UserProfile != null)
		{
			UserProfile.Silver += coins;
			Hashtable hashtable = new Hashtable();
			hashtable.Add("tokens", coins);
			AppShell.Instance.ServerConnection.ReportEvent("add_tokens", hashtable);
		}
	}

	public void ReportAwardTickets(int tickets)
	{
		if (UserProfile != null)
		{
			UserProfile.Tickets += tickets;
			Hashtable hashtable = new Hashtable();
			hashtable.Add("tickets", tickets);
			AppShell.Instance.ServerConnection.ReportEvent("add_tickets", hashtable);
		}
	}

	public void ReportAddXp(string hero, int xp)
	{
		HeroPersisted value;
		if (UserProfile != null && UserProfile.AvailableCostumes.TryGetValue(hero, out value))
		{
			value.UpdateXp(xp, true);
			Hashtable hashtable = new Hashtable();
			hashtable.Add("xp", xp);
			hashtable.Add("hero", hero);
			AppShell.Instance.ServerConnection.ReportEvent("add_xp", hashtable);
		}
	}

	public void ReportAchievementEvent(string hero, string eventType, string eventSubType, int inc = 1, string str1 = "")
	{
		ReportAchievementEvent(hero, eventType, eventSubType, inc, -10000, -10000, str1, string.Empty);
	}

	public void ReportAchievementEvent(string hero, string eventType, string eventSubType, int inc, int data1, int data2, string str1, string str2)
	{
		HeroPersisted value;
		if (UserProfile != null && (hero == null || !(hero != string.Empty) || UserProfile.AvailableCostumes.TryGetValue(hero, out value)))
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("event_type", eventType);
			hashtable.Add("event_sub_type", eventSubType);
			hashtable.Add("inc", inc);
			hashtable.Add("data1", data1);
			hashtable.Add("data2", data2);
			hashtable.Add("str1", str1);
			hashtable.Add("str2", str2);
			hashtable.Add("hero", string.Empty + hero);
			hashtable.Add("userID",AppShell.Instance.ServerConnection.getNotificationServer().PlayerId);  // added by CSP
			CspUtils.DebugLog("ReportAchievementEvent " + hero + " " + eventType + " " + eventSubType + " " + str1 + " " + str2);
			AppShell.Instance.ServerConnection.ReportEvent("achievement_event", hashtable);
		}
	}

	public void ConsumePotion(string hero, int ownableTypeId)
	{
		if (UserProfile != null)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("hero", hero);
			hashtable.Add("ownable_type_id", ownableTypeId);
			AppShell.Instance.ServerConnection.ReportEvent("consume_potion", hashtable);
		}
	}

	public void ReportEnemyDefeatedAllUpdate()
	{
		try
		{
			if (UserProfile != null)
			{
				ISHSCounterType counter = AppShell.Instance.CounterManager.GetCounter("BestThereIsCounter");
				string selectedCostume = AppShell.Instance.Profile.SelectedCostume;
				long num = counter.QualifierValues[AppShell.Instance.CounterManager.DefaultCounterBank][selectedCostume];
				Hashtable hashtable = new Hashtable();
				hashtable["id"] = counter.Id;
				hashtable["v"] = num.ToString();
				hashtable["h"] = AppShell.Instance.Profile.SelectedCostume;
				AppShell.Instance.ServerConnection.ReportEvent("set_counter", hashtable);
			}
		}
		catch (Exception ex)
		{
			CspUtils.DebugLog("ReportEnemyDefeatedAll: exception: " + ex.Message);
		}
	}

	public void ReportEnemyDefeatedSingle(string enemyType, int numberDefeated, int scoreValue)
	{
		try
		{
			if (UserProfile != null)
			{
			}
		}
		catch (Exception ex)
		{
			CspUtils.DebugLog("ReportEnemyDefeatedSingle: exception: " + ex.Message);
		}
	}

	public void ReportEnemyDefeated(string enemyType, int numberDefeated, int scoreValue)
	{
		if (UserProfile != null)
		{
			CspUtils.DebugLog("UserProfile != null");
			Hashtable hashtable = new Hashtable();
			hashtable.Add("defeated", numberDefeated);
			hashtable.Add("scorevalue", scoreValue);
			hashtable.Add("enemyid", enemyType);
			AppShell.Instance.ServerConnection.ReportBrawlerEvent("enemy_defeated", hashtable);
		}
	}

	public void ReportComboBonus(int comboBonus, int scoreValue)
	{
		if (UserProfile != null && AppShell.Instance.ServerConnection != null)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("combobonus", comboBonus);
			hashtable.Add("scorevalue", scoreValue);
			AppShell.Instance.ServerConnection.ReportBrawlerEvent("attack_combo", hashtable);
		}
	}

	public void ReportPickup(string pickupId)
	{
		if (UserProfile != null)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("pickupid", pickupId);
			AppShell.Instance.ServerConnection.ReportBrawlerEvent("pickup", hashtable);
		}
	}

	public void ReportGimmickScore(int gimmickScore)
	{
		if (UserProfile != null)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("scorevalue", gimmickScore);
			AppShell.Instance.ServerConnection.ReportBrawlerEvent("gimmick_score", hashtable);
		}
	}

	public void ReportPlayerKOed(int numberKOs)
	{
		if (UserProfile != null)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("kos", numberKOs);
			AppShell.Instance.ServerConnection.ReportBrawlerEvent("player_ko", hashtable);
		}
	}

	public void ReportObjectiveCompleted()
	{
		if (UserProfile != null)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("objective", 1);
		}
	}

	public void ReportStageStatus(ActiveMission mission, int stageId, bool isStageStarting, string hero)
	{
		if (UserProfile != null)
		{
			if (mission.CurrentStage == 1 && isStageStarting)
			{
				Hashtable hashtable = new Hashtable();
				hashtable.Add("mission_id", mission.Id);
				hashtable.Add("hero", hero);
				AppShell.Instance.ServerConnection.ReportBrawlerEvent("mission_start", hashtable);
			}
			else if (mission.CurrentStage == mission.LastStage && !isStageStarting)
			{
				Hashtable hashtable2 = new Hashtable();
				hashtable2.Add("mission_id", mission.Id);
				hashtable2.Add("hero", hero);
				AppShell.Instance.ServerConnection.ReportBrawlerEvent("mission_end", hashtable2);
			}
		}
	}

	public void ReportOpenChatMessage(int userRTCId, string roomname, string message)
	{
		Hashtable hashtable = new Hashtable();
		hashtable.Add("sender_player_id", userRTCId);
		hashtable.Add("room_name", roomname);
		hashtable.Add("message", RTCClient.EncodeString(message)); //Doggo
		//hashtable.Add("message", message); //Doggo
		AppShell.Instance.ServerConnection.ReportChatEvent("send_room_message", hashtable);
	}

	// this method added by CSP
	public void ReportMissionResults(Hashtable hashtable) {
		AppShell.Instance.ServerConnection.ReportEvent("send_mission_results", hashtable);
	}

	public void ChallengeMet(long playerId, long challengeId)
	{
		if (UserProfile != null)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("player_id", playerId);
			hashtable.Add("challenge_id", challengeId);
			hashtable.Add("authority", "C");
			CspUtils.DebugLog("Sending Challenge Met report");
			AppShell.Instance.ServerConnection.ReportEvent("challenge_met", hashtable);
		}
	}

	public void ChallengeViewed(long playerId, long challengeId)
	{
		if (UserProfile != null)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("player_id", playerId);
			hashtable.Add("challenge_id", challengeId);
			CspUtils.DebugLog("Sending Challenge Viewed report");
			AppShell.Instance.ServerConnection.ReportEvent("challenge_celebrated", hashtable);
		}
	}
}
