using System.Collections.Generic;
using UnityEngine;

public class BrawlerStatManager
{
	public class CharacterScoreData
	{
		public bool local;

		public int panel = -1;

		public float comboHeat;

		public int enemiesKOd;

		public int defeats;

		public int individualScoreContribution;

		public float health = 1f;

		public float power;

		public string characterName = string.Empty;

		public string squadName = string.Empty;

		public int multiplayerID = -2;

		public long netOwnerID;

		public string modelName = string.Empty;

		public float characterSize = 5f;

		public Vector3 dioramaPosition;
	}

	public static BrawlerStatManager instance;

	private Dictionary<GameObject, CharacterScoreData> multiplayerIdToStats;

	private Dictionary<int, CharacterScoreData> savedMultiplayerStats;

	private SHSBrawlerMainWindow brawlScreen;

	protected bool active;

	protected int scoredPlayerCount;

	protected int teamScore;

	protected int teamKOs;

	protected int teamDefeats;

	protected float COMBO_LOSS_RATE = 0.05f;

	protected float COMBO_GAIN_PER_DAMAGE = 0.025f;

	protected float COMBO_LOSS_PER_DAMAGE = 0.0125f;

	protected int COMBO_MAX = 3;

	public static bool Active
	{
		get
		{
			if (instance == null)
			{
				return false;
			}
			return instance.active;
		}
	}

	private BrawlerStatManager()
	{
		multiplayerIdToStats = new Dictionary<GameObject, CharacterScoreData>();
		savedMultiplayerStats = new Dictionary<int, CharacterScoreData>();
	}

	public static void Instanciate(SHSBrawlerMainWindow BrawlScreen)
	{
		if (instance == null)
		{
			instance = new BrawlerStatManager();
		}
		else
		{
			instance.Unready();
			instance.ClearStats();
		}
		instance.brawlScreen = BrawlScreen;
	}

	public static void Uninstanciate()
	{
		instance = null;
	}

	public void Ready()
	{
		if (!active)
		{
			active = true;
			if (AppShell.Instance != null)
			{
				AppShell.Instance.EventMgr.AddListener<CharacterStat.StatChangeEvent>(OnCharacterStatChange);
				AppShell.Instance.EventMgr.AddListener<PlayerPowerCostMessage>(OnCharacterPowerCost);
				AppShell.Instance.EventMgr.AddListener<NetworkConnectionProblem>(OnNetworkConnectionProblem);
				AppShell.Instance.EventMgr.AddListener<CombatCharacterKilledMessage>(OnCharacterKilledEvent);
				AppShell.Instance.EventMgr.AddListener<CombatCharacterHitMessage>(OnCharacterHitEvent);
				AppShell.Instance.EventMgr.AddListener<CombatPlayerKilledMessage>(OnPlayerKilledEvent);
				AppShell.Instance.EventMgr.AddListener<ScenarioEventScoreMessage>(OnScenarioScore);
			}
		}
	}

	public void Unready()
	{
		if (active)
		{
			active = false;
			multiplayerIdToStats.Clear();
			savedMultiplayerStats.Clear();
			scoredPlayerCount = 0;
			if (AppShell.Instance != null)
			{
				AppShell.Instance.EventMgr.RemoveListener<CharacterStat.StatChangeEvent>(OnCharacterStatChange);
				AppShell.Instance.EventMgr.RemoveListener<PlayerPowerCostMessage>(OnCharacterPowerCost);
				AppShell.Instance.EventMgr.RemoveListener<NetworkConnectionProblem>(OnNetworkConnectionProblem);
				AppShell.Instance.EventMgr.RemoveListener<CombatCharacterKilledMessage>(OnCharacterKilledEvent);
				AppShell.Instance.EventMgr.RemoveListener<CombatCharacterHitMessage>(OnCharacterHitEvent);
				AppShell.Instance.EventMgr.RemoveListener<CombatPlayerKilledMessage>(OnPlayerKilledEvent);
				AppShell.Instance.EventMgr.RemoveListener<ScenarioEventScoreMessage>(OnScenarioScore);
			}
		}
	}

	public void ClearStats()
	{
		teamScore = 0;
		teamKOs = 0;
		teamDefeats = 0;
		scoredPlayerCount = 0;
	}

	public List<CharacterScoreData> GetAllStatBlocks()
	{
		List<CharacterScoreData> list = new List<CharacterScoreData>();
		list.AddRange(multiplayerIdToStats.Values);
		list.AddRange(savedMultiplayerStats.Values);
		return list;
	}

	public bool ReservePanelSlot(GameObject caller, int multiplayerID, string characterName, string squadName)
	{
		CharacterScoreData characterScoreData = null;
		if (savedMultiplayerStats.ContainsKey(multiplayerID))
		{
			characterScoreData = savedMultiplayerStats[multiplayerID];
			savedMultiplayerStats.Remove(multiplayerID);
			CharacterGlobals component = caller.GetComponent<CharacterGlobals>();
			if (component != null && component.stats != null)
			{
				characterScoreData.health = component.stats.GetStat(CharacterStats.StatType.Health).Percent / 100f;
				characterScoreData.power = component.stats.GetStat(CharacterStats.StatType.Power).Percent / 100f;
			}
		}
		else if (multiplayerIdToStats.ContainsKey(caller))
		{
			characterScoreData = multiplayerIdToStats[caller];
		}
		else
		{
			characterScoreData = new CharacterScoreData();
			characterScoreData.multiplayerID = multiplayerID;
			scoredPlayerCount++;
			CharacterGlobals component2 = caller.GetComponent<CharacterGlobals>();
			if (component2 != null && component2.spawnData != null)
			{
				SpawnData spawnData = component2.spawnData;
				characterScoreData.modelName = spawnData.modelName;
				characterScoreData.characterSize = spawnData.sizeRank;
				if (multiplayerID == -1)
				{
					characterScoreData.netOwnerID = Utils.GetGazillionID(AppShell.Instance.ServerConnection.GetGameUserId());
				}
				else
				{
					characterScoreData.netOwnerID = Utils.GetGazillionID(multiplayerID);
				}
			}
		}
		multiplayerIdToStats[caller] = characterScoreData;
		if (characterScoreData.panel == -1)
		{
			characterScoreData.panel = brawlScreen.GetInfoPanel(multiplayerID, characterName, squadName, characterScoreData.health, characterScoreData.power, caller);
		}
		characterScoreData.local = (multiplayerID == -1);
		characterScoreData.characterName = characterName;
		characterScoreData.squadName = squadName;
		return characterScoreData.panel != -1;
	}

	public void ReleasePanelSlot(GameObject caller)
	{
		if (multiplayerIdToStats.ContainsKey(caller))
		{
			CharacterScoreData characterScoreData = multiplayerIdToStats[caller];
			brawlScreen.ReturnInfoPanel(characterScoreData.panel);
			if (characterScoreData.panel == 0)
			{
				brawlScreen.ReportBuffRemovedAll();
			}
			characterScoreData.panel = -1;
			characterScoreData.health = 1f;
			characterScoreData.power = 0f;
			savedMultiplayerStats[characterScoreData.multiplayerID] = characterScoreData;
			multiplayerIdToStats.Remove(caller);
		}
	}

	public void HidePanelSlot(GameObject caller)
	{
		if (multiplayerIdToStats.ContainsKey(caller))
		{
			CharacterScoreData characterScoreData = multiplayerIdToStats[caller];
			brawlScreen.ReturnInfoPanel(characterScoreData.panel);
			characterScoreData.panel = -1;
		}
	}

	public void ReportEmotionalEvent(GameObject caller, int happiness)
	{
		if (multiplayerIdToStats.ContainsKey(caller))
		{
			brawlScreen.UpdatePlayerEmotion(multiplayerIdToStats[caller].panel, happiness);
		}
	}

	public int ReportBuffAdded(GameObject caller, string iconPath, string buffName, string alertTexture)
	{
		if (multiplayerIdToStats.ContainsKey(caller))
		{
			return brawlScreen.ReportBuffAdded(multiplayerIdToStats[caller].panel, iconPath, buffName, alertTexture);
		}
		return -1;
	}

	public void ReportBuffRemoved(GameObject caller, int buffID)
	{
		if (multiplayerIdToStats.ContainsKey(caller))
		{
			brawlScreen.ReportBuffRemoved(multiplayerIdToStats[caller].panel, buffID);
		}
	}

	public void ReportBuffCountdown(GameObject caller, int buffID)
	{
		if (multiplayerIdToStats.ContainsKey(caller) && multiplayerIdToStats[caller].panel == 0)
		{
			brawlScreen.ReportBuffCountdown(buffID);
		}
	}

	public void ReportPowerRefund()
	{
		brawlScreen.ReportPowerRefund();
	}

	private void OnCharacterStatChange(CharacterStat.StatChangeEvent e)
	{
		if (e != null && e.Character != null && multiplayerIdToStats.ContainsKey(e.Character))
		{
			CharacterScoreData characterScoreData = multiplayerIdToStats[e.Character];
			if (e.StatType == CharacterStats.StatType.Health)
			{
				characterScoreData.health = e.NewValue / e.MaxValue;
				brawlScreen.ReportHealthChange(characterScoreData.panel, characterScoreData.health);
			}
			else if (e.StatType == CharacterStats.StatType.Power)
			{
				characterScoreData.power = e.NewValue / e.MaxValue;
				brawlScreen.ReportPowerChange(characterScoreData.panel, characterScoreData.power);
			}
		}
	}

	private void OnCharacterPowerCost(PlayerPowerCostMessage message)
	{
		if (message != null && multiplayerIdToStats.ContainsKey(message.owner))
		{
			brawlScreen.ReportPowerMoveLevel(multiplayerIdToStats[message.owner].panel, message.powerCost);
		}
	}

	private void OnNetworkConnectionProblem(NetworkConnectionProblem message)
	{
		int panelIndex = 0;
		if (message.character != null)
		{
			if (!multiplayerIdToStats.ContainsKey(message.character))
			{
				return;
			}
			panelIndex = multiplayerIdToStats[message.character].panel;
		}
		brawlScreen.SetNetworkDisconnect(panelIndex, message.responding);
	}

	private void OnCharacterKilledEvent(CombatCharacterKilledMessage e)
	{
		CspUtils.DebugLog("OnCharacterKilledEvent called!");
		if (e.CharacterCombat == null || !e.CharacterCombat.IsEnemy() || (e.CharacterCombat.CharGlobals.spawnData != null && !e.CharacterCombat.CharGlobals.spawnData.rewardsPoints))
		{
			return;
		}
		int defeat = e.CharacterCombat.CharGlobals.definitionData.EnemyScoringData.Defeat;
		int enemyKOValue = BrawlerScoringDefinition.Instance.GetEnemyKOValue(defeat);
		if (e.SourceCharacterCombat == null || e.SourceCharacterCombat.faction != 0)
		{
			AppShell.Instance.EventMgr.Fire(this, new ScenarioEventScoreMessage(enemyKOValue * COMBO_MAX, e.CharacterCombat));
			return;
		}
		if (!multiplayerIdToStats.ContainsKey(e.SourceCharacterCombat.gameObject))
		{
			AppShell.Instance.EventMgr.Fire(this, new ScenarioEventScoreMessage(enemyKOValue * COMBO_MAX, e.CharacterCombat));
			return;
		}
		CharacterScoreData characterScoreData = multiplayerIdToStats[e.SourceCharacterCombat.gameObject];
		characterScoreData.enemiesKOd++;
		teamKOs++;
		int num = Mathf.FloorToInt(Mathf.Max(Mathf.Min(characterScoreData.comboHeat, COMBO_MAX - 1), 0f));
		int num2 = enemyKOValue * (num + 1);
		characterScoreData.individualScoreContribution += num2;
		teamScore += num2;
		brawlScreen.TeamScoreChanged((int)((float)teamScore * BrawlerScoringDefinition.Instance.GetSurvivalBonus(teamDefeats)));
		e.CharacterCombat.showPopupScore(num2);
		if (characterScoreData.local)
		{
			CspUtils.DebugLog("characterScoreData.local=true!");
			AppShell.Instance.EventReporter.ReportEnemyDefeatedSingle(e.Character.name, 1, defeat);
			AppShell.Instance.EventReporter.ReportEnemyDefeated(e.Character.name, 1, defeat);
			AppShell.Instance.EventReporter.ReportComboBonus(num, defeat);
		}
	}

	private void OnPlayerKilledEvent(CombatPlayerKilledMessage e)
	{
		if (!multiplayerIdToStats.ContainsKey(e.Character))
		{
			return;
		}
		CharacterScoreData characterScoreData = multiplayerIdToStats[e.Character];
		teamDefeats++;
		characterScoreData.defeats++;
		characterScoreData.comboHeat = 0f;
		if (characterScoreData.local)
		{
			int numberKOs = 1;
			ActiveMission activeMission = (ActiveMission)AppShell.Instance.SharedHashTable["ActiveMission"];
			if (activeMission != null && activeMission.IsSurvivalMode)
			{
				numberKOs = 0;
			}
			AppShell.Instance.EventReporter.ReportPlayerKOed(numberKOs);
		}
		brawlScreen.TeamScoreChanged((int)((float)teamScore * BrawlerScoringDefinition.Instance.GetSurvivalBonus(teamDefeats)));
	}

	private void OnCharacterHitEvent(CombatCharacterHitMessage e)
	{
		if (!(e.SourceCharacterCombat == null))
		{
			if (multiplayerIdToStats.ContainsKey(e.SourceCharacterCombat.gameObject) && e.CharacterCombat.faction == CombatController.Faction.Enemy)
			{
				CharacterScoreData characterScoreData = multiplayerIdToStats[e.SourceCharacterCombat.gameObject];
				characterScoreData.comboHeat += e.Damage * COMBO_GAIN_PER_DAMAGE;
				characterScoreData.comboHeat = Mathf.Min(characterScoreData.comboHeat, COMBO_MAX);
			}
			if (multiplayerIdToStats.ContainsKey(e.CharacterCombat.gameObject))
			{
				CharacterScoreData characterScoreData2 = multiplayerIdToStats[e.CharacterCombat.gameObject];
				characterScoreData2.comboHeat -= e.Damage * COMBO_LOSS_PER_DAMAGE;
				characterScoreData2.comboHeat = Mathf.Max(characterScoreData2.comboHeat, 0f);
			}
		}
	}

	public void UpdateComboLoss()
	{
		foreach (CharacterScoreData value in multiplayerIdToStats.Values)
		{
			value.comboHeat -= COMBO_LOSS_RATE * Time.deltaTime;
			value.comboHeat = Mathf.Max(value.comboHeat, 0f);
			if (value.panel == 0)
			{
				brawlScreen.GetComboUpdate(value.comboHeat);
			}
		}
	}

	private void OnScenarioScore(ScenarioEventScoreMessage e)
	{
		int num = (int)((float)e.eventScore * BrawlerScoringDefinition.Instance.GimmickBonus);
		teamScore += num;
		brawlScreen.TeamScoreChanged((int)((float)teamScore * BrawlerScoringDefinition.Instance.GetSurvivalBonus(teamDefeats)));
		if (e.eventScoreTarget != null)
		{
			e.eventScoreTarget.showPopupScore(num);
		}
		if (AppShell.Instance.ServerConnection.IsGameHost())
		{
			AppShell.Instance.EventReporter.ReportGimmickScore(num);
		}
	}

	public void OutputCurrentScores()
	{
		string empty = string.Empty;
		string text = empty;
		empty = text + "Team Score: " + teamScore + "\n";
		text = empty;
		empty = text + "Total Enemies KOd: " + teamKOs + "\n";
		text = empty;
		empty = text + "Total Player KOs: " + teamDefeats + "\n";
		text = empty;
		empty = text + "Current Survival Bonus is: " + BrawlerScoringDefinition.Instance.GetSurvivalBonus(teamDefeats) + " putting current score at: " + (float)teamScore * BrawlerScoringDefinition.Instance.GetSurvivalBonus(teamDefeats) + "\n";
		foreach (CharacterScoreData value in multiplayerIdToStats.Values)
		{
			text = empty;
			empty = text + "-- Character: " + value.characterName + ", Squad: " + value.squadName + " --\n";
			text = empty;
			empty = text + "\tScore Contribution: " + value.individualScoreContribution + "\n";
			text = empty;
			empty = text + "\tEnemies Defeated: " + value.enemiesKOd + "\n";
			text = empty;
			empty = text + "\tSelf KOs: " + value.defeats + "\n";
			text = empty;
			empty = text + "\tCombo Level: " + value.comboHeat + "\n";
			empty += "\tIs Local: ";
			empty = ((!value.local) ? (empty + "Nope.") : (empty + "Yes!"));
		}
		CspUtils.DebugLog(empty);
	}

	public int GetScoredPlayerCount()
	{
		return scoredPlayerCount;
	}
}
