using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CombatController))]
[AddComponentMenu("Brawler/Brawler Hot Target")]
public class BrawlerHotTarget : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum HotTargetType
	{
		HotCombatant,
		HotPickup
	}

	public enum HotState
	{
		Inactive,
		Hot,
		Cool
	}

	public float hotTime;

	public float hotRadius;

	public CombatController.Faction hotFaction;

	[HideInInspector]
	public HotTargetType hotType;

	public float cooldownTime;

	public HotState comeIn = HotState.Hot;

	public string receiveHotEvent = string.Empty;

	public string receiveCoolEvent = string.Empty;

	public string receiveInactiveEvent = string.Empty;

	public CharacterSpawn[] mothTypes;

	public string[] mothTypeNames;

	private float _startTime;

	private CombatController.Faction _coolFaction;

	private HotState _currentState;

	private CombatController _targetCombat;

	private bool _disabled;

	public void Flash()
	{
		if (_targetCombat == null)
		{
			CspUtils.DebugLog("cannot heat the brawler hot target <" + base.gameObject.name + "> without a combat controller");
			return;
		}
		if (IsHot())
		{
			CspUtils.DebugLog("reheating hot brawler hot target <" + base.gameObject.name + ">");
		}
		_startTime = Time.time;
		_currentState = HotState.Hot;
		_targetCombat.ChangeFaction(hotFaction);
		List<AIControllerBrawler> moths = GetMoths();
		AttractMoths(moths);
	}

	public void Extinguish()
	{
		if (_targetCombat == null)
		{
			CspUtils.DebugLog("cannot cool the brawler hot target <" + base.gameObject.name + "> without a combat controller");
			return;
		}
		if (IsCooling())
		{
			CspUtils.DebugLog("cooling cool brawler hot target <" + base.gameObject.name + ">");
		}
		_startTime = Time.time;
		_currentState = HotState.Cool;
		_targetCombat.ChangeFaction(_coolFaction);
	}

	public void Deactivate()
	{
		if (_targetCombat == null)
		{
			CspUtils.DebugLog("cannot deactivate the brawler hot target <" + base.gameObject.name + "> without a combat controller");
			return;
		}
		_startTime = 0f;
		_currentState = HotState.Inactive;
		if (base.gameObject.active)
		{
			_targetCombat.ChangeFaction(_coolFaction);
		}
	}

	public bool IsHot()
	{
		return _currentState == HotState.Hot;
	}

	public bool IsCooling()
	{
		return _currentState == HotState.Cool;
	}

	public bool IsInactive()
	{
		return _currentState == HotState.Inactive;
	}

	public bool IsMoth(CombatController combatObject)
	{
		if (combatObject == null || !combatObject.gameObject.active)
		{
			return false;
		}
		if (combatObject.CharGlobals == null || combatObject.CharGlobals.brawlerCharacterAI == null)
		{
			return false;
		}
		if ((hotFaction == CombatController.Faction.Player && combatObject.faction != CombatController.Faction.Enemy) || (hotFaction == CombatController.Faction.Enemy && combatObject.faction != 0))
		{
			return false;
		}
		if ((mothTypes == null && mothTypeNames == null) || (mothTypes.Length == 0 && mothTypeNames.Length == 0))
		{
			return true;
		}
		CharacterSpawn[] array = mothTypes;
		foreach (CharacterSpawn characterSpawn in array)
		{
			if (characterSpawn != null && characterSpawn.CharacterName == combatObject.gameObject.name)
			{
				return true;
			}
		}
		string[] array2 = mothTypeNames;
		foreach (string text in array2)
		{
			if (text != null && text == combatObject.gameObject.name)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsMothInHotRadius(CombatController moth)
	{
		if (moth == null || !moth.gameObject.active)
		{
			return false;
		}
		if (hotRadius <= 0f)
		{
			return true;
		}
		return (moth.transform.position - base.transform.position).sqrMagnitude <= hotRadius * hotRadius;
	}

	private List<AIControllerBrawler> GetMoths()
	{
		List<AIControllerBrawler> list = new List<AIControllerBrawler>();
		CombatController.Faction faction = CombatController.Faction.None;
		if (hotFaction == CombatController.Faction.Player)
		{
			faction = CombatController.Faction.Enemy;
		}
		else if (hotFaction == CombatController.Faction.Enemy)
		{
			faction = CombatController.Faction.Player;
		}
		if (faction == CombatController.Faction.None)
		{
			return list;
		}
		List<CombatController> factionList = CombatController.GetFactionList(faction);
		if (factionList == null)
		{
			return list;
		}
		foreach (CombatController item in factionList)
		{
			if (IsMoth(item) && IsMothInHotRadius(item))
			{
				list.Add(item.CharGlobals.brawlerCharacterAI);
			}
		}
		return list;
	}

	private void AttractMoths(List<AIControllerBrawler> moths)
	{
		foreach (AIControllerBrawler moth in moths)
		{
			if (moth.GetTarget() != base.transform.root.gameObject)
			{
				moth.EndAttackOnTarget(moth.GetTarget());
			}
		}
	}

	private void Initialize()
	{
		if (ScenarioEventManager.Instance != null)
		{
			if (!string.IsNullOrEmpty(receiveHotEvent))
			{
				ScenarioEventManager.Instance.SubscribeScenarioEvent(receiveHotEvent, OnHotEvent);
			}
			if (!string.IsNullOrEmpty(receiveCoolEvent))
			{
				ScenarioEventManager.Instance.SubscribeScenarioEvent(receiveCoolEvent, OnCoolEvent);
			}
			if (!string.IsNullOrEmpty(receiveInactiveEvent))
			{
				ScenarioEventManager.Instance.SubscribeScenarioEvent(receiveInactiveEvent, OnInactiveEvent);
			}
		}
		hotType = HotTargetType.HotCombatant;
		Pickup component = base.gameObject.GetComponent<Pickup>();
		if (component != null)
		{
			hotType = HotTargetType.HotPickup;
		}
		_startTime = 0f;
		_currentState = HotState.Inactive;
		_coolFaction = _targetCombat.faction;
		switch (comeIn)
		{
		case HotState.Hot:
			Flash();
			break;
		case HotState.Cool:
			Extinguish();
			break;
		}
	}

	private void OnHotEvent(string eventName)
	{
		Flash();
	}

	private void OnCoolEvent(string eventName)
	{
		Extinguish();
	}

	private void OnInactiveEvent(string eventName)
	{
		Deactivate();
	}

	private void Start()
	{
		_targetCombat = base.gameObject.GetComponent<CombatController>();
		if (_targetCombat == null)
		{
			CspUtils.DebugLog("no combat controller found for brawler hot target <" + base.gameObject.name + ">");
		}
		else
		{
			Initialize();
		}
	}

	private void Update()
	{
		if (_currentState == HotState.Inactive)
		{
			return;
		}
		switch (_currentState)
		{
		case HotState.Hot:
			if (hotTime > 0f && Time.time - _startTime >= hotTime)
			{
				if (cooldownTime > 0f)
				{
					Extinguish();
				}
				else
				{
					Deactivate();
				}
			}
			break;
		case HotState.Cool:
			if (cooldownTime > 0f && Time.time - _startTime >= cooldownTime)
			{
				if (hotTime > 0f)
				{
					Flash();
				}
				else
				{
					Deactivate();
				}
			}
			break;
		}
	}

	private void OnEnable()
	{
		if (_disabled)
		{
			Initialize();
			_disabled = false;
		}
	}

	private void OnDisable()
	{
		if (ScenarioEventManager.Instance != null)
		{
			if (!string.IsNullOrEmpty(receiveHotEvent))
			{
				ScenarioEventManager.Instance.UnsubscribeScenarioEvent(receiveHotEvent, OnHotEvent);
			}
			if (!string.IsNullOrEmpty(receiveCoolEvent))
			{
				ScenarioEventManager.Instance.UnsubscribeScenarioEvent(receiveCoolEvent, OnCoolEvent);
			}
			if (!string.IsNullOrEmpty(receiveInactiveEvent))
			{
				ScenarioEventManager.Instance.UnsubscribeScenarioEvent(receiveInactiveEvent, OnInactiveEvent);
			}
		}
		Deactivate();
		_disabled = true;
	}

	private void OnDrawGizmosSelected()
	{
		if (hotRadius > 0f)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(base.transform.position, hotRadius);
		}
	}
}
