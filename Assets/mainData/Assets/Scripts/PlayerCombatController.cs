using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerCombatController : CombatController
{
	protected string heroupName;

	protected string[] heroupNames;

	protected float[] healthIncreases;

	protected bool prestigeEffectsSpawned;

	protected static List<PlayerCombatController> playerList;

	private bool initialized;

	public bool autoChaining = true;

	public float PowerDamageReceivedMultiplier = 0.5f;

	public float PowerDamageDealtMultiplier = 0.5f;

	private TextBillboard textBillboard;

	private CharacterStat power;

	protected GameObject storedPersistentHeroUpEffect;

	protected GameObject startUpHeroUpEffect;

	private float heroUpEffectCounter;

	private bool inAir;

	protected bool suppressHeroUpEffect;

	protected bool heardPowerMaxEvent;

	protected float showHealthTimer = -1f;

	public static int PlayerCount
	{
		get
		{
			return playerList.Count;
		}
	}

	public static List<PlayerCombatController> PlayerList
	{
		get
		{
			return playerList;
		}
	}

	public void UsePower()
	{
		if (power == null)
		{
			return;
		}
		BehaviorAttackSpecial behaviorAttackSpecial = behaviorManager.getBehavior() as BehaviorAttackSpecial;
		if (behaviorAttackSpecial != null)
		{
			float num = (!behaviorAttackSpecial.firstUpdate) ? 0f : behaviorAttackSpecial.getAttackData().powerCost;
			if (behaviorAttackSpecial.category != BehaviorAttackBase.AttackCategory.Super && num + power.Value >= power.MaximumValue && !behaviorAttackSpecial.AnyImpactsFired())
			{
				if (BrawlerStatManager.Active)
				{
					BrawlerStatManager.instance.ReportPowerRefund();
				}
				power.Value = power.MaximumValue;
				behaviorManager.endBehavior();
			}
		}
		if (!checkPowerFull() || base.IsAttackRestricted)
		{
			return;
		}
		if (heroupName == string.Empty)
		{
			CspUtils.DebugLog("You have no power attack defined");
			return;
		}
		AttackData attackData = AttackDataManager.Instance.getAttackData(heroupName);
		if (attackData == null)
		{
			CspUtils.DebugLog("Specified power attack " + heroupName + " does not exist!");
			return;
		}
		Type type = Type.GetType(attackData.behaviorName);
		if (behaviorManager.currentBehaviorInterruptible(type) && createAttackBehavior(null, attackData, false, false))
		{
			BehaviorAttackBase behaviorAttackBase = behaviorManager.getBehavior() as BehaviorAttackBase;
			if (behaviorAttackBase != null)
			{
				behaviorAttackBase.category = BehaviorAttackBase.AttackCategory.Super;
			}
			checkForInitialSummons(attackData);
			charGlobals.motionController.dropThrowable();
			if (BrawlerController.Instance != null && BrawlerController.Instance.HeroUpCharacterEffect != string.Empty)
			{
				createEffect(BrawlerController.Instance.HeroUpCharacterEffect, base.gameObject);
			}
		}
	}

	public string GetHeroupAttackName(int index)
	{
		if (index > 0 && heroupNames != null && heroupNames.Length >= index)
		{
			return heroupNames[index - 1];
		}
		return "UNDEFINED";
	}

	public override void InitializeFromData(DataWarehouse combatData, DataWarehouse attackData)
	{
		if (combatData.GetCount("combat_controller") > 0)
		{
			base.InitializeFromData(combatData.GetData("combat_controller"), attackData);
		}
		else
		{
			base.InitializeFromData(combatData, attackData);
		}
		heroupNames = new string[StatLevelReqsDefinition.POWER_ATTACKS_COUNT];
		heroupNames[0] = combatData.TryGetString("power_attack_1", string.Empty);
		heroupNames[1] = combatData.TryGetString("power_attack_2", string.Empty);
		heroupNames[2] = combatData.TryGetString("power_attack_3", string.Empty);
		heroupName = heroupNames[0];
		healthIncreases = new float[StatLevelReqsDefinition.HEALTH_RANK_COUNT];
		healthIncreases[0] = combatData.TryGetFloat("health_increase_1", 0f);
		healthIncreases[1] = combatData.TryGetFloat("health_increase_2", 0f);
		healthIncreases[2] = combatData.TryGetFloat("health_increase_3", 0f);
		healthIncreases[3] = combatData.TryGetFloat("health_increase_4", 0f);
		healthIncreases[4] = combatData.TryGetFloat("health_increase_5", 0f);
		healthIncreases[5] = combatData.TryGetFloat("health_increase_6", 0f);
		healthIncreases[6] = combatData.TryGetFloat("health_increase_7", 0f);
		healthIncreases[7] = combatData.TryGetFloat("health_increase_8", 0f);
		PowerDamageReceivedMultiplier = combatData.TryGetFloat("power_damage_received_multiplier", 0.5f);
		PowerDamageDealtMultiplier = combatData.TryGetFloat("power_damage_dealt_multiplier", 0.5f);
	}

	public override void InitializeFromCopy(CombatController source)
	{
		base.InitializeFromCopy(source);
		PlayerCombatController playerCombatController = source as PlayerCombatController;
		heroupNames = playerCombatController.heroupNames;
		healthIncreases = playerCombatController.healthIncreases;
		PowerDamageReceivedMultiplier = playerCombatController.PowerDamageReceivedMultiplier;
		PowerDamageDealtMultiplier = playerCombatController.PowerDamageDealtMultiplier;
	}

	public bool checkPowerFull()
	{
		if (power.Value == power.MaximumValue)
		{
			return true;
		}
		return false;
	}

	public virtual bool checkPower(AttackData attack)
	{
		if (power == null)
		{
			return true;
		}
		if (power.Value < attack.powerCost)
		{
			return false;
		}
		return true;
	}

	public override bool IsAttackAvailable(bool secondaryAttack)
	{
		AttackData currentAttackData = getCurrentAttackData(secondaryAttack, false);
		return checkPower(currentAttackData);
	}

	public override bool IsAttackAvailable(string attackName)
	{
		AttackData attackData = AttackDataManager.Instance.getAttackData(attackName);
		return checkPower(attackData);
	}

	public override bool beginAttack(GameObject targetObject, bool secondaryAttack)
	{
		AttackData currentAttackData = getCurrentAttackData(secondaryAttack, false);
		if (!checkPower(currentAttackData))
		{
			return false;
		}
		AppShell.Instance.EventMgr.Fire(this, new BrawlerUnstuckMessage());
		return base.beginAttack(targetObject, secondaryAttack);
	}

	public override List<AttackData> getAllAttackData()
	{
		List<AttackData> allAttackData = base.getAllAttackData();
		AttackData attackData = AttackDataManager.Instance.getAttackData(heroupName);
		if (attackData != null)
		{
			allAttackData.Add(attackData);
		}
		return allAttackData;
	}

	public void changeLevel(int newLevel)
	{
		if (newLevel == characterLevel)
		{
			return;
		}
		StatLevelReqsDefinition instance = StatLevelReqsDefinition.Instance;
		characterLevel = newLevel;
		if (instance == null)
		{
			CspUtils.DebugLog("Level up stats object not found!  Can't change stats!");
			return;
		}
		health.MaximumValue = health.InitialMaximum;
		for (int num = instance.GetNumberOfHealthRanksForLevel(characterLevel); num > 0; num--)
		{
			health.InitialValue += healthIncreases[num - 1];
			health.MaximumValue += healthIncreases[num - 1];
			health.InitialMaximum += healthIncreases[num - 1];
		}
		if (characterLevel > 20)
		{
			health.InitialValue += (float)((characterLevel - 20) * 10);
			health.MaximumValue += (float)((characterLevel - 20) * 10);
			health.InitialMaximum += (float)((characterLevel - 20) * 10);
			if (characterLevel >= 40)
			{
				health.InitialValue += 100f;
				health.MaximumValue += 100f;
				health.InitialMaximum += 100f;
			}
		}
		health.Value = health.MaximumValue;
		attackPower.Value = attackPower.InitialValue;
		attackPower.level = characterLevel;
		specialPower.Value = specialPower.InitialValue;
		specialPower.level = characterLevel;
		SetMaxRegularAttackChain(instance.GetMaxCombo(characterLevel));
		maximumSecondaryAttackChain = instance.GetMaxPowerAttackUnlockedAt(characterLevel);
		heroupName = GetHeroupAttackName(instance.GetNumberOfHeroupRanksForLevel(characterLevel));
		if (currentSecondaryAttackChain != null)
		{
			for (int i = 0; i < currentSecondaryAttackChain.Length; i++)
			{
				int maxPowerAttackRankUnlockedAt = instance.GetMaxPowerAttackRankUnlockedAt(i + 1, characterLevel);
				currentSecondaryAttackChain[i] = GetSecondaryAttackDataAtRank(i, maxPowerAttackRankUnlockedAt);
			}
		}
		currentLChain = instance.GetLChainIndexAt(characterLevel);
		CspUtils.DebugLog(base.gameObject.name + " is level " + characterLevel + " setting L Chain to " + currentLChain);
		if (attackChain[currentLChain].Count == 0)
		{
			CspUtils.DebugLog("ERROR:  character " + base.gameObject.name + " does not have data for LChain index " + currentLChain + "reverting to chain 0");
			currentLChain = 0;
		}
		if (newLevel == StatLevelReqsDefinition.MAX_HERO_LEVEL_BADGE2)
		{
			spawnPrestigeEffects();
		}
	}

	protected override void Start()
	{
		base.Start();
		initialized = true;
		textBillboard = (GetComponentInChildren(typeof(TextBillboard)) as TextBillboard);
		CharacterStats characterStats = GetComponentInChildren(typeof(CharacterStats)) as CharacterStats;
		if (characterStats != null)
		{
			power = characterStats.GetStat(CharacterStats.StatType.Power);
		}
		if (!base.isHidden)
		{
			GetStatusBar();
			if (!playerList.Contains(this))
			{
				playerList.Add(this);
			}
		}
		if (BrawlerController.Instance != null)
		{
			CharacterMotionController motionController = base.motionController;
			motionController.landCallback = (CharacterMotionController.MotionCallback)Delegate.Combine(motionController.landCallback, new CharacterMotionController.MotionCallback(OnEntityLand));
			CharacterMotionController motionController2 = base.motionController;
			motionController2.airborneCallback = (CharacterMotionController.MotionCallback)Delegate.Combine(motionController2.airborneCallback, new CharacterMotionController.MotionCallback(OnEntityAirborne));
		}
		spawnPrestigeEffects();
	}

	public void spawnPrestigeEffects()
	{
		if (BrawlerController.Instance != null || prestigeEffectsSpawned || base.gameObject.name == "mr_placeholder" || networkComponent == null || AppShell.Instance.ServerConnection == null)
		{
			return;
		}
		CspUtils.DebugLog("spawned a player, checking for prestige effects " + prestigeEffectsSpawned + " " + base.gameObject.name + " " + characterLevel + " " + networkComponent.goNetId.childId + " " + AppShell.Instance.ServerConnection.GetGameUserId());
		if (characterLevel < 40 && !TitleManager.checkHeroPrestige(networkComponent.goNetId.childId, base.gameObject.name))
		{
			return;
		}
		CspUtils.DebugLog("spawning!");
		BehaviorManager component = base.gameObject.GetComponent<BehaviorManager>();
		if (component == null)
		{
			CspUtils.DebugLog("Attempting to apply ExpendableDefinition but the player has no BehaviorManager defined!");
		}
		else
		{
			if (component.charGlobals == null)
			{
				return;
			}
			if (GraphicsOptions.PrestigeEffects)
			{
				BehaviorEffectExpendable behaviorEffectExpendable = component.requestChangeBehavior<BehaviorEffectExpendable>(true);
				if (behaviorEffectExpendable == null)
				{
					CspUtils.DebugLog("No BehaviorEffectExpendable defined for this potion.");
					return;
				}
				ExpendableDefinition def = ExpendablesManager.instance.ExpendableTypes[prestigeEffectID];
				behaviorEffectExpendable.Initialize(def, null);
			}
			if (networkComponent.goNetId.childId == AppShell.Instance.ServerConnection.GetGameUserId())
			{
				CspUtils.DebugLog("local player spawn, sending prestige message");
				SpawnPrestigeMessage spawnPrestigeMessage = new SpawnPrestigeMessage(new GoNetId(GoNetId.PLAYER_ID_FLAG, AppShell.Instance.ServerConnection.GetGameUserId()));
				spawnPrestigeMessage.hero = charGlobals.spawnData.spawner.CharacterName;
				AppShell.Instance.ServerConnection.SendGameMsg(spawnPrestigeMessage);
			}
			prestigeEffectsSpawned = true;
		}
	}

	protected override void OnEnable()
	{
		if (playerList == null)
		{
			playerList = new List<PlayerCombatController>();
		}
		AppShell.Instance.EventMgr.AddListener<CharacterStat.StatChangeEvent>(OnCharacterStatChange);
		AppShell.Instance.EventMgr.AddListener<LocalPlayerChangedMessage>(OnLocalPlayerChanged);
		AppShell.Instance.EventMgr.AddListener<SpawnPrestigeRequest>(OnSpawnPrestigeRequest);
	}

	protected void OnSpawnPrestigeRequest(SpawnPrestigeRequest msg)
	{
		CspUtils.DebugLog("got OnSpawnPrestigeRequest " + msg.UserID + " " + networkComponent.goNetId.childId);
		if (msg.UserID == networkComponent.goNetId.childId)
		{
			CspUtils.DebugLog("Ids match, spawning effects");
			spawnPrestigeEffects();
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if (initialized)
		{
			if (playerList.Contains(this))
			{
				playerList.Remove(this);
			}
			if (BrawlerStatManager.Active)
			{
				BrawlerStatManager.instance.ReleasePanelSlot(base.gameObject);
			}
		}
		AppShell.Instance.EventMgr.RemoveListener<CharacterStat.StatChangeEvent>(OnCharacterStatChange);
		AppShell.Instance.EventMgr.RemoveListener<LocalPlayerChangedMessage>(OnLocalPlayerChanged);
	}

	protected override void Update()
	{
		base.Update();
		if (heroUpEffectCounter > 0f && !suppressHeroUpEffect)
		{
			heroUpEffectCounter -= Time.deltaTime;
			if (heroUpEffectCounter <= 0f)
			{
				heroUpEffectCounter = 0f;
				UpdateHeroUpEffect(false);
			}
		}
		if (showHealthTimer >= 0f)
		{
			showHealthTimer -= Time.deltaTime;
			if (showHealthTimer < 0f)
			{
				OnShowHealthTag(null);
			}
		}
	}

	protected void OnTextBillboardChanged(GameObject newBillboard)
	{
		if (newBillboard != null)
		{
			textBillboard = newBillboard.GetComponent<TextBillboard>();
		}
	}

	protected void GetStatusBar()
	{
		if (!(BrawlerController.Instance != null) || !(charGlobals.spawnData != null) || (charGlobals.spawnData.spawnType & CharacterSpawn.Type.Player) == 0)
		{
			return;
		}
		int multiplayerID = -1;
		if (BrawlerStatManager.Active)
		{
			NetworkComponent networkComponent = charGlobals.networkComponent;
			if (!networkComponent.goNetId.IsLocalPlayer())
			{
				multiplayerID = ((!(networkComponent != null)) ? (-2) : networkComponent.goNetId.ChildId);
			}
			string squadName;
			SpawnData.PlayerType playerType;
			charGlobals.spawnData.GetSquadRelation(out squadName, out playerType, true);
			BrawlerStatManager.instance.ReservePanelSlot(base.gameObject, multiplayerID, charGlobals.spawnData.modelName, squadName);
			SetSecondaryAttack(selectedSecondaryAttack);
		}
	}

	public override void SetSecondaryAttack(int newSelection)
	{
		if (Application.platform != RuntimePlatform.WindowsEditor && StatLevelReqsDefinition.Instance.GetMaxPowerAttackRankUnlockedAt(newSelection, characterLevel) <= 0)
		{
			CspUtils.DebugLog("rejecting SetSecondaryAttack " + newSelection);
			return;
		}
		base.SetSecondaryAttack(newSelection);
		AppShell.Instance.EventMgr.Fire(null, new PlayerChangedSecondaryAttackMessage(newSelection));
		if (secondaryAttackChain != null && secondaryAttackChain.Length != 0)
		{
			AttackData secondaryAttackData = getSecondaryAttackData(selectedSecondaryAttack);
			AppShell.Instance.EventMgr.Fire(null, new PlayerPowerCostMessage(secondaryAttackData.powerCost, base.transform.root.gameObject));
		}
	}

	protected override void takeDamage(float damage, GameObject source)
	{
		if (networkComponent == null)
		{
			base.takeDamage(damage, source);
			return;
		}
		if (damage > 0f && health.Value > health.MinimumValue && power != null && !power.TimedUpdateActive)
		{
			power.Value += damage * PowerDamageReceivedMultiplier;
		}
		base.takeDamage(damage, source);
	}

	protected override void onDealtDamage(CombatController TargetController, float damage, AttackData attackData)
	{
		if (power != null && !power.TimedUpdateActive && (attackData == null || (!(attackData.powerCost > 0f) && !attackData.environmentalAttack)))
		{
			power.Value += damage * PowerDamageDealtMultiplier;
		}
	}

	public override GameObject RemoteSpawn(Vector3 spawnLoc, Quaternion spawnRot, GoNetId newID, string prefabName, GameObject parent)
	{
		if (BrawlerController.Instance != null && prefabName == BrawlerController.Instance.throwableTargetPrefab.name)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(BrawlerController.Instance.throwableTargetPrefab) as GameObject;
			gameObject.transform.position = spawnLoc;
			if (newID.IsValid())
			{
				NetworkComponent networkComponent = gameObject.GetComponent(typeof(NetworkComponent)) as NetworkComponent;
				if (networkComponent != null)
				{
					networkComponent.goNetId = newID;
				}
			}
			return gameObject;
		}
		return base.RemoteSpawn(spawnLoc, spawnRot, newID, prefabName, parent);
	}

	private void AttackBegin(int attackID)
	{
		if (BrawlerController.Instance != null && BrawlerController.Instance.showAttackChainNumbers)
		{
			textBillboard.Text = attackID.ToString();
		}
	}

	private void AttackEnd()
	{
		if (BrawlerController.Instance != null && BrawlerController.Instance.showAttackChainNumbers)
		{
			textBillboard.Text = string.Empty;
		}
	}

	public override void playKnockdownEffect()
	{
		base.playKnockdownEffect();
		if (BrawlerStatManager.instance != null)
		{
			BrawlerStatManager.instance.ReportEmotionalEvent(base.gameObject, 0);
		}
	}

	public float getPower()
	{
		if (power == null)
		{
			return 0f;
		}
		return power.Value;
	}

	public float getMaxPower()
	{
		if (power == null)
		{
			return 0f;
		}
		return power.MaximumValue;
	}

	public void addPower(float amount)
	{
		power.Value += amount;
		if (power.Value > power.MaximumValue)
		{
			power.Value = power.MaximumValue;
		}
	}

	public override bool IsPlayer()
	{
		return true;
	}

	public void decreasePower(float amount)
	{
		if (power != null)
		{
			power.Value -= amount;
			if (power.Value < power.MinimumValue)
			{
				power.Value = power.MinimumValue;
			}
		}
	}

	public void setPower(float amount)
	{
		if (power != null)
		{
			power.Value = amount;
		}
	}

	public static int GetPlayerCount()
	{
		if (BrawlerController.Instance != null && BrawlerController.Instance.UseDebugPlayerCount())
		{
			return BrawlerController.Instance.debugPlayerCount;
		}
		return PlayerCount;
	}

	public static int GetPlayerKilledCount()
	{
		return Enumerable.Count(Enumerable.Where((IEnumerable<PlayerCombatController>)playerList, (Func<PlayerCombatController, bool>)delegate(PlayerCombatController player)
		{
			return player.isKilled;
		}));
	}

	public static void PlayerListClear()
	{
		if (playerList != null)
		{
			playerList.Clear();
		}
	}

	private void OnCharacterStatChange(CharacterStat.StatChangeEvent evt)
	{
		if (evt != null && evt.StatType == CharacterStats.StatType.Power && !(Mathf.Abs(evt.NewValue - evt.OldValue) <= float.Epsilon) && !(evt.Character == null) && !(evt.Character != base.gameObject))
		{
			if (suppressHeroUpEffect)
			{
				heardPowerMaxEvent = true;
			}
			else
			{
				UpdateHeroUpEffect(true);
			}
		}
	}

	protected void OnLocalPlayerChanged(LocalPlayerChangedMessage msg)
	{
		ActiveMission activeMission = AppShell.Instance.SharedHashTable["ActiveMission"] as ActiveMission;
		if (activeMission != null)
		{
			changeLevel(characterLevel);
		}
	}

	protected override void OnEntityPolymorph(EntityPolymorphMessage msg)
	{
		base.OnEntityPolymorph(msg);
		if (msg.original == base.gameObject && Utils.IsPlayer(msg.polymorph))
		{
			PlayerCombatController component = msg.polymorph.GetComponent<PlayerCombatController>();
			component.setPower(getPower());
			if (BrawlerStatManager.Active)
			{
				BrawlerStatManager.instance.ReleasePanelSlot(base.gameObject);
				component.GetStatusBar();
			}
			playerList.Remove(this);
			if (!playerList.Contains(component))
			{
				playerList.Add(component);
			}
		}
	}

	protected void OnEntityLand()
	{
		if (inAir)
		{
			inAir = false;
			heroUpEffectCounter = 0.064f;
		}
	}

	protected void OnEntityAirborne()
	{
		if (!inAir)
		{
			inAir = true;
			if (motionController.getVerticalVelocity() < 0f)
			{
				heroUpEffectCounter = 0.25f;
			}
			else
			{
				heroUpEffectCounter = 0.064f;
			}
		}
	}

	protected void UpdateHeroUpEffect(bool fullEffect)
	{
		if (heroUpEffectCounter != 0f)
		{
			return;
		}
		if (inAir)
		{
			if (storedPersistentHeroUpEffect != null)
			{
				Utils.DetachGameObject(storedPersistentHeroUpEffect);
				UnityEngine.Object.Destroy(storedPersistentHeroUpEffect);
				storedPersistentHeroUpEffect = null;
			}
			if (startUpHeroUpEffect != null)
			{
				Utils.DetachGameObject(startUpHeroUpEffect);
				UnityEngine.Object.Destroy(startUpHeroUpEffect);
				startUpHeroUpEffect = null;
			}
		}
		else
		{
			if (!(BrawlerController.Instance != null))
			{
				return;
			}
			if (storedPersistentHeroUpEffect == null)
			{
				if (power.Percent == 100f)
				{
					storedPersistentHeroUpEffect = createEffect(BrawlerController.Instance.PersistentHeroUpCharacterEffect, base.gameObject);
					if (fullEffect)
					{
						startUpHeroUpEffect = createEffect(BrawlerController.Instance.HeroUpCharacterEffect, base.gameObject);
					}
				}
			}
			else if (power.Percent < 100f)
			{
				if (storedPersistentHeroUpEffect != null)
				{
					Utils.DetachGameObject(storedPersistentHeroUpEffect);
					UnityEngine.Object.Destroy(storedPersistentHeroUpEffect);
					storedPersistentHeroUpEffect = null;
				}
				if (startUpHeroUpEffect != null)
				{
					Utils.DetachGameObject(startUpHeroUpEffect);
					startUpHeroUpEffect = null;
				}
			}
		}
	}

	[AnimTag("showhealth")]
	public void OnShowHealthTag(AnimationEvent evt)
	{
		GetStatusBar();
	}

	[AnimTag("hidehealth")]
	public void OnHideHealthTag(AnimationEvent evt)
	{
		if (!initialized || !BrawlerStatManager.Active)
		{
			return;
		}
		BrawlerStatManager.instance.HidePanelSlot(base.gameObject);
		string[] array = evt.stringParameter.Split(':');
		if (array.Length > 1)
		{
			string text = array[1];
			float num = 0f;
			if (text != null && text != string.Empty)
			{
				num = float.Parse(text);
			}
			if (num > 0f)
			{
				num = (showHealthTimer = num / base.animation[base.animation.clip.name].speed);
			}
		}
	}

	public void SuppressHeroUpEffect(bool suppress)
	{
		suppressHeroUpEffect = suppress;
		if (!suppress)
		{
			UpdateHeroUpEffect(heardPowerMaxEvent);
			heardPowerMaxEvent = false;
			return;
		}
		if (storedPersistentHeroUpEffect != null)
		{
			Utils.DetachGameObject(storedPersistentHeroUpEffect);
			UnityEngine.Object.Destroy(storedPersistentHeroUpEffect);
			storedPersistentHeroUpEffect = null;
		}
		if (startUpHeroUpEffect != null)
		{
			Utils.DetachGameObject(startUpHeroUpEffect);
			UnityEngine.Object.Destroy(startUpHeroUpEffect);
			startUpHeroUpEffect = null;
		}
	}
}
