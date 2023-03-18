using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AICombatController : CombatController
{
	public class AIAttackData
	{
		public int baseChance;

		public int bonusChancePerExtraTarget;

		public float extraTargetMaximumDistance;

		public bool extraTargetMeasuredFromAttacker;

		public float minimumRange;

		public float maximumRange;

		public int minimumHealthPercentage;

		public int maximumHealthPercentage;

		public string requiredCombatEffect;

		public int minimumPlayers;

		public int maximumPlayers;

		public bool cooldownStartAvailable;

		public float cooldownRefreshTime;

		public float cooldownExpireTime;

		public string cooldownRefreshScenarioEvent;

		public string cooldownExpireScenarioEvent;

		protected bool cooldown;

		protected float refreshTime;

		protected float expireTime;

		public AIAttackData()
		{
		}

		public AIAttackData(int newBaseChance)
		{
			baseChance = newBaseChance;
			maximumHealthPercentage = 100;
			cooldownStartAvailable = true;
			minimumPlayers = 1;
			maximumPlayers = 4;
			cooldown = false;
		}

		public void Initialize()
		{
			cooldown = !cooldownStartAvailable;
			if (!string.IsNullOrEmpty(cooldownRefreshScenarioEvent))
			{
				ScenarioEventManager.Instance.SubscribeScenarioEvent(cooldownRefreshScenarioEvent, OnCooldownRefreshEvent);
			}
			if (!string.IsNullOrEmpty(cooldownExpireScenarioEvent))
			{
				ScenarioEventManager.Instance.SubscribeScenarioEvent(cooldownExpireScenarioEvent, OnCooldownExpireEvent);
			}
		}

		public void Destroy()
		{
			if (ScenarioEventManager.Instance != null)
			{
				if (!string.IsNullOrEmpty(cooldownRefreshScenarioEvent))
				{
					ScenarioEventManager.Instance.UnsubscribeScenarioEvent(cooldownRefreshScenarioEvent, OnCooldownRefreshEvent);
				}
				if (!string.IsNullOrEmpty(cooldownExpireScenarioEvent))
				{
					ScenarioEventManager.Instance.UnsubscribeScenarioEvent(cooldownExpireScenarioEvent, OnCooldownExpireEvent);
				}
			}
		}

		public bool IsCooldown()
		{
			if (refreshTime != 0f && Time.time >= refreshTime)
			{
				refreshTime = 0f;
				cooldown = false;
			}
			if (expireTime != 0f && Time.time >= expireTime)
			{
				expireTime = 0f;
				cooldown = true;
			}
			return cooldown;
		}

		public void AttackChosen()
		{
			if (cooldownRefreshTime > 0f)
			{
				refreshTime = Time.time + cooldownRefreshTime;
				cooldown = true;
			}
			if (!string.IsNullOrEmpty(cooldownRefreshScenarioEvent))
			{
				cooldown = true;
			}
		}

		public void AttackReset()
		{
			if (cooldownRefreshTime > 0f)
			{
				refreshTime = 0f;
				cooldown = false;
			}
		}

		protected void OnCooldownRefreshEvent(string eventName)
		{
			cooldown = false;
			if (cooldownExpireTime > 0f)
			{
				expireTime = Time.time + cooldownExpireTime;
			}
		}

		protected void OnCooldownExpireEvent(string eventName)
		{
			cooldown = true;
			if (cooldownRefreshTime > 0f)
			{
				refreshTime = Time.time + cooldownRefreshTime;
			}
		}
	}

	protected class AIPotentialAttack
	{
		public int chance;

		public int attackIndex;

		public AIPotentialAttack(int newChance, int newIndex)
		{
			chance = newChance;
			attackIndex = newIndex;
		}
	}

	protected List<AIAttackData> attackChanceData;

	protected bool attacksRepeatable;

	protected AttackData chosenAttack;

	protected int prioritizedAttackIndex = -1;

	protected int prioritizedAttackChain;

	[CompilerGenerated]
	private bool _003CMustHaveAttack_003Ek__BackingField;

	public List<AIAttackData> AttackChanceData
	{
		get
		{
			return attackChanceData;
		}
		set
		{
			attackChanceData = value;
		}
	}

	public bool AttacksRepeatable
	{
		get
		{
			return attacksRepeatable;
		}
		set
		{
			attacksRepeatable = value;
		}
	}

	public bool MustHaveAttack
	{
		[CompilerGenerated]
		get
		{
			return _003CMustHaveAttack_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CMustHaveAttack_003Ek__BackingField = value;
		}
	}

	public AttackData PrioritizedAttack
	{
		get
		{
			if (prioritizedAttackIndex == -1)
			{
				return null;
			}
			return attackChain[prioritizedAttackChain][prioritizedAttackIndex];
		}
		set
		{
			if (value == null)
			{
				prioritizedAttackIndex = -1;
				return;
			}
			for (int i = 0; i < attackChanceData.Count; i++)
			{
				if (attackChain[prioritizedAttackChain][i] == value)
				{
					prioritizedAttackIndex = i;
					clearChosenAttack(true);
					return;
				}
			}
			CspUtils.DebugLog("Could not prioritize <" + value.attackName + "> for <" + base.gameObject.name + "> -- It isn't one of this character's attacks.");
		}
	}

	public override void InitializeFromData(DataWarehouse AICombatData, DataWarehouse attackData)
	{
		attacksRepeatable = AICombatData.TryGetBool("attacks_repeatable", false);
		MustHaveAttack = true;
		DataWarehouse data = AICombatData.GetData("combat_controller");
		attackChanceData = new List<AIAttackData>();
		foreach (DataWarehouse item in data.GetIterator("attack"))
		{
			AIAttackData aIAttackData = new AIAttackData();
			aIAttackData.baseChance = item.TryGetInt("chance", 1);
			aIAttackData.bonusChancePerExtraTarget = item.TryGetInt("bonus_chance_per_extra_target", 0);
			aIAttackData.extraTargetMaximumDistance = item.TryGetFloat("extra_target_maximum_distance", 0f);
			aIAttackData.extraTargetMeasuredFromAttacker = item.TryGetBool("extra_target_measured_from_attacker", false);
			aIAttackData.minimumRange = item.TryGetFloat("minimum_range", 0f);
			aIAttackData.maximumRange = item.TryGetFloat("maximum_range", 0f);
			aIAttackData.minimumHealthPercentage = item.TryGetInt("minimum_health_percent", 0);
			aIAttackData.maximumHealthPercentage = item.TryGetInt("maximum_health_percent", 100);
			aIAttackData.requiredCombatEffect = item.TryGetString("required_combat_effect", null);
			aIAttackData.cooldownStartAvailable = item.TryGetBool("cooldown_start_available", true);
			aIAttackData.cooldownRefreshTime = item.TryGetFloat("cooldown_refresh_time", 0f);
			aIAttackData.cooldownExpireTime = item.TryGetFloat("cooldown_expire_time", 0f);
			aIAttackData.cooldownRefreshScenarioEvent = item.TryGetString("cooldown_refresh_scenario_event", null);
			aIAttackData.cooldownExpireScenarioEvent = item.TryGetString("cooldown_expire_scenario_event", null);
			aIAttackData.minimumPlayers = item.TryGetInt("minimum_players", 1);
			aIAttackData.maximumPlayers = item.TryGetInt("maximum_players", 4);
			aIAttackData.Initialize();
			attackChanceData.Add(aIAttackData);
		}
		base.InitializeFromData(data, attackData);
	}

	public override void InitializeFromCopy(CombatController source)
	{
		base.InitializeFromCopy(source);
		AICombatController aICombatController = source as AICombatController;
		attacksRepeatable = aICombatController.attacksRepeatable;
		MustHaveAttack = true;
		attackChanceData = new List<AIAttackData>();
		foreach (AIAttackData attackChanceDatum in aICombatController.attackChanceData)
		{
			AIAttackData aIAttackData = new AIAttackData();
			aIAttackData.baseChance = attackChanceDatum.baseChance;
			aIAttackData.bonusChancePerExtraTarget = attackChanceDatum.bonusChancePerExtraTarget;
			aIAttackData.extraTargetMaximumDistance = attackChanceDatum.extraTargetMaximumDistance;
			aIAttackData.extraTargetMeasuredFromAttacker = attackChanceDatum.extraTargetMeasuredFromAttacker;
			aIAttackData.minimumRange = attackChanceDatum.minimumRange;
			aIAttackData.maximumRange = attackChanceDatum.maximumRange;
			aIAttackData.minimumHealthPercentage = attackChanceDatum.minimumHealthPercentage;
			aIAttackData.maximumHealthPercentage = attackChanceDatum.maximumHealthPercentage;
			aIAttackData.requiredCombatEffect = attackChanceDatum.requiredCombatEffect;
			aIAttackData.cooldownStartAvailable = attackChanceDatum.cooldownStartAvailable;
			aIAttackData.cooldownRefreshTime = attackChanceDatum.cooldownRefreshTime;
			aIAttackData.cooldownExpireTime = attackChanceDatum.cooldownExpireTime;
			aIAttackData.cooldownRefreshScenarioEvent = attackChanceDatum.cooldownRefreshScenarioEvent;
			aIAttackData.cooldownExpireScenarioEvent = attackChanceDatum.cooldownExpireScenarioEvent;
			aIAttackData.minimumPlayers = attackChanceDatum.minimumPlayers;
			aIAttackData.maximumPlayers = attackChanceDatum.maximumPlayers;
			aIAttackData.Initialize();
			attackChanceData.Add(aIAttackData);
		}
	}

	public virtual void InitializeFromSpareData(DataWarehouse AICombatData, DataWarehouse attackData)
	{
		base.InitializeFromData(AICombatData, attackData);
		attacksRepeatable = true;
		MustHaveAttack = true;
		attackChanceData = new List<AIAttackData>();
		List<AttackData> list = new List<AttackData>(attackChain[currentLChain].Count + secondaryAttackChain.Length);
		int num = 1;
		attackChanceData.Add(new AIAttackData(num));
		for (int i = 0; i < attackChain[currentLChain].Count; i++)
		{
			if (i != 0)
			{
				attackChanceData.Add(new AIAttackData(0));
			}
			list.Add(attackChain[currentLChain][i]);
		}
		for (int j = 0; j < secondaryAttackChain.Length; j++)
		{
			num++;
			attackChanceData.Add(new AIAttackData(num));
			list.Add(secondaryAttackChain[j][0]);
		}
		attackChain[0] = list;
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		foreach (AIAttackData attackChanceDatum in attackChanceData)
		{
			attackChanceDatum.Destroy();
		}
	}

	public bool considerAttack(int attackIndex, GameObject target)
	{
		AttackData attackData = attackChain[currentLChain][attackIndex];
		LogAIF("Considering <{0}>...", attackData.attackName);
		if (!attacksRepeatable && currentAttackData != null && attackData.attackName == currentAttackData.attackName)
		{
			LogAI(" skipping due to attacksrepeatable false");
			return false;
		}
		int index = 0;
		if (IsPrimaryAttack(attackData, out index) && GetMaxRegularAttackChain() <= index)
		{
			LogAI(" invalid due to regular attack chain restrictions");
			return false;
		}
		int index2 = 0;
		if (IsSecondaryAttack(attackData, out index2) && IsSecondaryAttackLocked(index2))
		{
			LogAI(" skipping due to locked status");
			return false;
		}
		AIAttackData aIAttackData = attackChanceData[attackIndex];
		if (aIAttackData.IsCooldown())
		{
			LogAI(" skipping due to cooldown");
			return false;
		}
		if (aIAttackData.minimumHealthPercentage > 0 && health.Percent < (float)aIAttackData.minimumHealthPercentage)
		{
			LogAI(" fails minimum health requirement");
			return false;
		}
		if (aIAttackData.maximumHealthPercentage > 0 && aIAttackData.maximumHealthPercentage < 100 && health.Percent > (float)aIAttackData.maximumHealthPercentage)
		{
			LogAI(" fails maximum health requirement");
			return false;
		}
		if (!string.IsNullOrEmpty(aIAttackData.requiredCombatEffect) && !currentActiveEffects.ContainsKey(aIAttackData.requiredCombatEffect))
		{
			LogAI(" fails combat effect requirement");
			return false;
		}
		if (aIAttackData.minimumPlayers > 1 && PlayerCombatController.GetPlayerCount() < aIAttackData.minimumPlayers)
		{
			LogAI(" fails minimum player requirement");
			return false;
		}
		if (aIAttackData.maximumPlayers < 4 && PlayerCombatController.GetPlayerCount() > aIAttackData.maximumPlayers)
		{
			LogAI(" fails maximum player requirement");
			return false;
		}
		if (target != null && (aIAttackData.minimumRange != 0f || aIAttackData.maximumRange != 0f))
		{
			float num = Vector3.Distance(target.transform.position, base.transform.position);
			if (num < aIAttackData.minimumRange)
			{
				LogAI(" fails minimum range requirement");
				return false;
			}
			if (aIAttackData.maximumRange != 0f && num > aIAttackData.maximumRange)
			{
				LogAI(" fails maximum range requirement");
				return false;
			}
		}
		LogAI(" meets all requirements!");
		return true;
	}

	public override AttackData getCurrentAttackData(bool secondaryAttack, bool chainAttack, GameObject target)
	{
		LogAI("----------------  <<< Choosing attack >>> ----------------");
		if (chainAttack)
		{
			LogAIF("chain attack, using current (index {0})", currentAttack);
			chosenAttack = null;
			return getAttackData(currentAttack);
		}
		if (attackChain[currentLChain].Count < 2)
		{
			LogAIF("!! Only one attack, <{0}>", (attackChain[0] != null) ? attackChain[currentLChain][0].attackName : "null");
			return attackChain[currentLChain][0];
		}
		if (chosenAttack != null)
		{
			LogAIF("** attack previously chosen ({0})", (chosenAttack != null) ? chosenAttack.attackName : "null");
			return chosenAttack;
		}
		if (prioritizedAttackIndex >= 0)
		{
			LogAIF("Attack with index {0} is prioritized...", prioritizedAttackIndex);
			if (considerAttack(prioritizedAttackIndex, target))
			{
				currentAttack = prioritizedAttackIndex + 1;
				chosenAttack = PrioritizedAttack;
				LogAIF("!! Picked attack <{0}>", (chosenAttack != null) ? chosenAttack.attackName : "null");
				attackChanceData[prioritizedAttackIndex].AttackChosen();
				return chosenAttack;
			}
		}
		LogAI("-- Determining list of valid attacks --");
		int num = 0;
		List<AIPotentialAttack> list = new List<AIPotentialAttack>();
		for (int i = 0; i < attackChanceData.Count; i++)
		{
			if (!considerAttack(i, target))
			{
				continue;
			}
			int num2 = 0;
			AIAttackData aIAttackData = attackChanceData[i];
			if (aIAttackData.baseChance > 0)
			{
				if (target != null && aIAttackData.bonusChancePerExtraTarget != 0 && aIAttackData.extraTargetMaximumDistance != 0f)
				{
					Vector3 a = (!aIAttackData.extraTargetMeasuredFromAttacker) ? target.transform.position : base.transform.position;
					foreach (PlayerCombatController player in PlayerCombatController.PlayerList)
					{
						if (player != null && player.isHealthInitialized() && !player.isKilled && player.gameObject != target)
						{
							float num3 = Vector3.Distance(a, player.transform.position);
							if (num3 <= aIAttackData.extraTargetMaximumDistance)
							{
								num2 += aIAttackData.bonusChancePerExtraTarget;
							}
						}
					}
				}
				int num4 = aIAttackData.baseChance + num2;
				if (num4 < 0)
				{
					num4 = 0;
				}
				num += num4;
				list.Add(new AIPotentialAttack(num, i));
			}
		}
		LogAI("-- Finished determing list of valid attacks --");
		if (num == 0 || list.Count == 0)
		{
			if (MustHaveAttack)
			{
				CspUtils.DebugLog("getCurrentAttackData in AICombatController had no possible attacks, consider turning on attacks repeatable");
			}
			return null;
		}
		int num5 = Random.Range(0, num - 1);
		foreach (AIPotentialAttack item in list)
		{
			if (num5 < item.chance)
			{
				AttackData attackData = attackChain[currentLChain][item.attackIndex];
				currentAttack = item.attackIndex + 1;
				chosenAttack = attackData;
				LogAIF("!! Picked attack <{0}>", (chosenAttack != null) ? chosenAttack.attackName : "null");
				attackChanceData[item.attackIndex].AttackChosen();
				return chosenAttack;
			}
		}
		CspUtils.DebugLog("getCurrentAttackData in AICombatController failed to find a valid attack");
		return null;
	}

	public bool AttackAvailable(AttackData checkMe)
	{
		int num = -1;
		for (int i = 0; i < attackChain[currentLChain].Count; i++)
		{
			if (checkMe == attackChain[currentLChain][i])
			{
				num = i;
				break;
			}
		}
		if (num != -1)
		{
			return considerAttack(num, null);
		}
		return true;
	}

	public override bool beginAttack(GameObject targetObject, bool secondaryAttack, bool ignoreRange, string attackName)
	{
		bool flag = base.beginAttack(targetObject, secondaryAttack, ignoreRange, attackName);
		if (flag)
		{
			chosenAttack = null;
			PlayNamedAttackVO();
		}
		return flag;
	}

	public void clearChosenAttack(bool resetCooldown)
	{
		if (chosenAttack != null && resetCooldown)
		{
			int i;
			for (i = 0; i < attackChain[currentLChain].Count && attackChain[currentLChain][i] != chosenAttack; i++)
			{
			}
			attackChanceData[i].AttackReset();
		}
		chosenAttack = null;
	}

	protected override bool IsPrimaryAttack(AttackData attack, out int index)
	{
		if (IsSecondaryAttack(attack, out index))
		{
			return false;
		}
		return base.IsPrimaryAttack(attack, out index);
	}

	protected void PlayNamedAttackVO()
	{
		if (charGlobals != null && charGlobals.brawlerCharacterAI != null)
		{
			if (charGlobals.brawlerCharacterAI.CanPlayAttackVO)
			{
				VOManager.Instance.PlayVO("named_attack", base.gameObject);
			}
		}
		else
		{
			VOManager.Instance.PlayVO("named_attack", base.gameObject);
		}
	}

	protected void LogAI(string message)
	{
		if (BrawlerController.Instance != null && BrawlerController.Instance.dumpAIChoices)
		{
			CspUtils.DebugLog("AI (" + base.gameObject.name + "): " + message);
		}
	}

	protected void LogAIF(string format, params object[] paramList)
	{
		LogAI(string.Format(format, paramList));
	}
}
