using CardGame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SquadBattleCharacterController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private sealed class _ProcessAction_c__Iterator38 : IDisposable, IEnumerator, IEnumerator<object>
	{
		internal SquadBattleAction action;

		internal CharacterLocatorManager _locatorManager___0;

		internal GameObject _existingCharacter___1;

		internal int _PC;

		internal object _current;

		internal SquadBattleAction ___action;

		internal SquadBattleCharacterController __f__this;

		object IEnumerator<object>.Current
		{
			get
			{
				return this._current;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this._current;
			}
		}

		public bool MoveNext()
		{
			uint num = (uint)this._PC;
			this._PC = -1;
			switch (num)
			{
			case 0u:
				if (this.action.player == SquadBattlePlayerEnum.Left)
				{
					this._locatorManager___0 = this.__f__this.leftCharacterLocatorManager;
				}
				else
				{
					this._locatorManager___0 = this.__f__this.rightCharacterLocatorManager;
				}
				if (string.IsNullOrEmpty(this.action.secondaryAttackingCharacterName) || this.action.attackPattern.DelaySecondCharacterSpawn)
				{
					goto IL_116;
				}
				this.action.secondaryCharacter = this._locatorManager___0.getCharacter(this.action.secondaryAttackingCharacterName);
				if (!(this.action.secondaryCharacter == null))
				{
					goto IL_116;
				}
				this._locatorManager___0.spawnTemporaryCharacter(this.action.secondaryAttackingCharacterName, this.action, null, true, delegate(GameObject spawnedCharacter)
				{
					this.action.secondaryCharacter = spawnedCharacter;
					SquadBattleCharacterAI squadBattleCharacterAI = spawnedCharacter.GetComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI;
					if (squadBattleCharacterAI != null)
					{
						squadBattleCharacterAI.WaitForAction();
					}
				});
				break;
			case 1u:
				break;
			default:
				return false;
			}
			if (this.action.secondaryCharacter == null)
			{
				this._current = 0;
				this._PC = 1;
				return true;
			}
			IL_116:
			this._existingCharacter___1 = this._locatorManager___0.getCharacter(this.action.attackingCharacterName);
			if (this._existingCharacter___1 != null)
			{
				if (this.action.attackerBecomesKeeper)
				{
					this._locatorManager___0.spawnKeeper(this.action.attackingCharacterName, null);
				}
				else if (this.action.attackerKeeperDestroyed)
				{
					this._locatorManager___0.removeKeeper(this.action.attackingCharacterName, true);
				}
				this.__f__this.BeginAction(this._existingCharacter___1, this.action);
			}
			else if (this.action.attackerBecomesKeeper)
			{
				this._locatorManager___0.spawnKeeper(this.action.attackingCharacterName, this.action);
			}
			else
			{
				this._locatorManager___0.spawnTemporaryCharacter(this.action.attackingCharacterName, this.action, null, false, null);
			}
			this._PC = -1;
			return false;
		}

		public void Dispose()
		{
			this._PC = -1;
		}

		public void Reset()
		{
			throw new NotSupportedException();
		}

		internal void __m__48C(GameObject spawnedCharacter)
		{
			this.action.secondaryCharacter = spawnedCharacter;
			SquadBattleCharacterAI squadBattleCharacterAI = spawnedCharacter.GetComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI;
			if (squadBattleCharacterAI != null)
			{
				squadBattleCharacterAI.WaitForAction();
			}
		}
	}

	private sealed class _RewardsRoutine_c__Iterator39 : IDisposable, IEnumerator, IEnumerator<object>
	{
		internal CharacterLocatorManager _currentManager___0;

		internal SquadBattleCharacterAI _ai___1;

		internal int _PC;

		internal object _current;

		internal SquadBattleCharacterController __f__this;

		object IEnumerator<object>.Current
		{
			get
			{
				return this._current;
			}
		}

		object IEnumerator.Current
		{
			get
			{
				return this._current;
			}
		}

		public bool MoveNext()
		{
			uint num = (uint)this._PC;
			this._PC = -1;
			switch (num)
			{
			case 0u:
				this._currentManager___0 = this.__f__this.winner;
				break;
			case 1u:
				break;
			default:
				return false;
			}
			this._ai___1 = (this._currentManager___0.getNextCharacter().GetComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI);
			this._ai___1.characterCamera((float)((this._currentManager___0 != this.__f__this.rightCharacterLocatorManager) ? (-28) : (-22)), true);
			this._current = new WaitForSeconds(3f);
			this._PC = 1;
			return true;
		}

		public void Dispose()
		{
			this._PC = -1;
		}

		public void Reset()
		{
			throw new NotSupportedException();
		}
	}

	protected List<SquadBattleAction> actionQueue;

	protected bool actionInProgress;

	protected string leftPlayerAvatar;

	protected string rightPlayerAvatar;

	protected CharacterLocatorManager leftCharacterLocatorManager;

	protected CharacterLocatorManager rightCharacterLocatorManager;

	protected CharacterLocatorManager winner;

	protected GameObject leftPlayerHighlight;

	protected GameObject rightPlayerHighlight;

	protected int messagePauses;

	protected static SquadBattleCharacterController instance;

	public bool ActionInProgress
	{
		get
		{
			return this.actionInProgress;
		}
	}

	public static SquadBattleCharacterController Instance
	{
		get
		{
			return SquadBattleCharacterController.instance;
		}
	}

	public virtual void Awake()
	{
		SquadBattleCharacterController.instance = this;
	}

	public virtual void Start()
	{
		base.gameObject.AddComponent(typeof(AttackDataManager));
		this.actionQueue = new List<SquadBattleAction>();
		this.actionInProgress = false;
		this.leftCharacterLocatorManager = new CharacterLocatorManager(SquadBattlePlayerEnum.Left);
		this.rightCharacterLocatorManager = new CharacterLocatorManager(SquadBattlePlayerEnum.Right);
		AppShell.Instance.EventMgr.AddListener<CombatCharacterDespawnedMessage>(new ShsEventMgr.GenericDelegate<CombatCharacterDespawnedMessage>(this.OnRevive));
	}

	public void Initialize(string leftPlayerAvatar, string rightPlayerAvatar)
	{
		this.leftPlayerAvatar = leftPlayerAvatar;
		this.rightPlayerAvatar = rightPlayerAvatar;
		this.leftCharacterLocatorManager.spawnKeeper(leftPlayerAvatar, null);
	}

	public void SpawnRightAvatar()
	{
		this.rightCharacterLocatorManager.spawnKeeper(this.rightPlayerAvatar, null);
	}

	private void Update()
	{
		if (!this.actionInProgress && this.actionQueue.Count > 0)
		{
			this.actionInProgress = true;
			base.StartCoroutine(this.ProcessAction(this.actionQueue[0]));
			this.actionQueue.RemoveAt(0);
		}
	}

	public void ActionComplete()
	{
		this.actionInProgress = false;
		AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.CombatFinished());
		AppShell.Instance.EventMgr.Fire(null, new CardGameEvent.NoMoSloMo());
	}

	public void PlayRecoil(SquadBattleRecoil recoil)
	{
		CharacterLocatorManager characterLocatorManager;
		if (recoil.player == SquadBattlePlayerEnum.Left)
		{
			characterLocatorManager = this.leftCharacterLocatorManager;
		}
		else
		{
			characterLocatorManager = this.rightCharacterLocatorManager;
		}
		GameObject character = characterLocatorManager.getCharacter(recoil.characterName);
		if (character != null)
		{
			CharacterGlobals characterGlobals = character.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals;
			if (characterGlobals != null)
			{
				CombatController.ImpactData impactData = new CombatController.ImpactData();
				CombatController.ImpactResultData impactResultData = new CombatController.ImpactResultData();
				impactResultData.recoil = recoil.recoilType;
				impactResultData.knockdownDuration = new ModifierData(3f);
				if (impactResultData.recoil == CombatController.AttackData.RecoilType.Launch)
				{
					impactResultData.launchVelocity = new ModifierData(10f);
				}
				impactData.impactResult = impactResultData;
				characterGlobals.combatController.hitByAttackLocal(character.transform.position, characterGlobals.combatController, "fake_attack", 0f, impactData);
			}
		}
	}

	public void QueueAction(SquadBattleActionBase action)
	{
		SquadBattleAction squadBattleAction = action as SquadBattleAction;
		if (squadBattleAction == null)
		{
			squadBattleAction = new SquadBattleAction(action);
		}
		this.actionQueue.Add(squadBattleAction);
	}

	protected void InsertAction(SquadBattleAction action)
	{
		this.actionQueue.Insert(0, action);
	}

	protected IEnumerator ProcessAction(SquadBattleAction action)
	{
		SquadBattleCharacterController._ProcessAction_c__Iterator38 _ProcessAction_c__Iterator = new SquadBattleCharacterController._ProcessAction_c__Iterator38();
		_ProcessAction_c__Iterator.action = action;
		_ProcessAction_c__Iterator.___action = action;
		_ProcessAction_c__Iterator.__f__this = this;
		return _ProcessAction_c__Iterator;
	}

	public void BeginAction(GameObject character, SquadBattleAction action)
	{
		SquadBattleCharacterAI squadBattleCharacterAI = character.GetComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI;
		if (squadBattleCharacterAI == null)
		{
			CspUtils.DebugLog("Character missing AI component in BeginAction");
			return;
		}
		if (action.targetCharacter == null)
		{
			CharacterLocatorManager characterLocatorManager;
			if (action.player == SquadBattlePlayerEnum.Left)
			{
				characterLocatorManager = this.rightCharacterLocatorManager;
			}
			else
			{
				characterLocatorManager = this.leftCharacterLocatorManager;
			}
			action.targetCharacter = characterLocatorManager.getAvatar();
		}
		if (character.name != action.secondaryAttackingCharacterName)
		{
			squadBattleCharacterAI.StartNewAction(action);
		}
	}

	public void StartBlock(SquadBattleAction action, GameObject character)
	{
		SquadBattleAction action2 = action.GenerateBlockingAction(character);
		base.StartCoroutine(this.ProcessAction(action2));
	}

	public void KeeperNoDamage(SquadBattleAction action)
	{
		CharacterLocatorManager characterLocatorManager;
		if (action.player == SquadBattlePlayerEnum.Left)
		{
			characterLocatorManager = this.leftCharacterLocatorManager;
		}
		else
		{
			characterLocatorManager = this.rightCharacterLocatorManager;
		}
		GameObject character = characterLocatorManager.getCharacter(action.attackingCharacterName);
		if (character != null)
		{
			SquadBattleCharacterAI squadBattleCharacterAI = character.GetComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI;
			squadBattleCharacterAI.KeeperNoDamageEmote(action);
		}
		else
		{
			CspUtils.DebugLog("SquadBattleCharacterController called a Keeper No-damage emote when no keeper was spawned.");
			characterLocatorManager.spawnTemporaryCharacter(action.attackingCharacterName, action, null, false, null);
		}
	}

	public void ApproachTarget(SquadBattlePlayerEnum attackingTeam, string attackingCharacter, bool IsMulti, bool isSecondaryCharacter)
	{
		CharacterLocatorManager characterLocatorManager;
		GameObject avatar;
		if (attackingTeam == SquadBattlePlayerEnum.Left)
		{
			characterLocatorManager = this.leftCharacterLocatorManager;
			avatar = this.rightCharacterLocatorManager.getAvatar();
		}
		else
		{
			characterLocatorManager = this.rightCharacterLocatorManager;
			avatar = this.leftCharacterLocatorManager.getAvatar();
		}
		GameObject character = characterLocatorManager.getCharacter(attackingCharacter);
		if (character != null)
		{
			SquadBattleCharacterAI squadBattleCharacterAI = character.GetComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI;
			squadBattleCharacterAI.ApproachTarget(avatar, !IsMulti);
		}
		else
		{
			characterLocatorManager.spawnTemporaryCharacter(attackingCharacter, null, avatar, isSecondaryCharacter, null);
		}
	}

	public void AdjustServerMessagePauses(int adjustment)
	{
		CspUtils.DebugLog(string.Concat(new object[]
		{
			"adjustservermessagepauses  adjustment = ",
			adjustment,
			"  old total = ",
			this.messagePauses
		}));
		if (this.messagePauses == 0)
		{
			RoomAgent.Suspend();
		}
		this.messagePauses += adjustment;
		if (this.messagePauses == 0)
		{
			RoomAgent.Instance.Resume();
		}
	}

	public void AddKeeper(SquadBattlePlayerEnum player, string characterName)
	{
		if (player == SquadBattlePlayerEnum.Left)
		{
			this.leftCharacterLocatorManager.spawnKeeper(characterName, null);
		}
		else
		{
			this.rightCharacterLocatorManager.spawnKeeper(characterName, null);
		}
	}

	public void RemoveKeeper(SquadBattlePlayerEnum player, string characterName)
	{
		if (player == SquadBattlePlayerEnum.Left)
		{
			this.leftCharacterLocatorManager.removeKeeper(characterName);
		}
		else
		{
			this.rightCharacterLocatorManager.removeKeeper(characterName);
		}
	}

	public int HighestKeeperPosition()
	{
		if (this.leftCharacterLocatorManager == null || this.rightCharacterLocatorManager == null)
		{
			return 0;
		}
		return Mathf.Max(this.leftCharacterLocatorManager.NextAvailableLocator - 1, this.rightCharacterLocatorManager.NextAvailableLocator - 1);
	}

	public void RemoveCharacter(GameObject character)
	{
		UnityEngine.Object.Destroy(character);
	}

	public void Victory(int winningPlayerIndex)
	{
		CharacterLocatorManager characterLocatorManager;
		if (winningPlayerIndex == 0)
		{
			this.winner = this.leftCharacterLocatorManager;
			characterLocatorManager = this.rightCharacterLocatorManager;
		}
		else
		{
			this.winner = this.rightCharacterLocatorManager;
			characterLocatorManager = this.leftCharacterLocatorManager;
		}
		this.winner.VictoryCharacters();
		characterLocatorManager.KillCharacters(this.winner.getAvatar());
	}

	public void OnRevive(CombatCharacterDespawnedMessage message)
	{
		CspUtils.DebugLog("Reviving KO'd character...");
		GameObject character = message.Character;
		if (character == null)
		{
			return;
		}
		SquadBattleCharacterAI squadBattleCharacterAI = character.GetComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI;
		if (squadBattleCharacterAI == null)
		{
			return;
		}
		CharacterGlobals characterGlobals = character.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals;
		BehaviorManager behaviorManager = characterGlobals.behaviorManager;
		BehaviorRecoilGetup behaviorRecoilGetup = behaviorManager.forceChangeBehavior(typeof(BehaviorRecoilGetup)) as BehaviorRecoilGetup;
		behaviorRecoilGetup.Initialize(character, character.transform.position, null);
		squadBattleCharacterAI.Defeat();
	}

	public GameObject GetAvatar(SquadBattlePlayerEnum side)
	{
		if (side == SquadBattlePlayerEnum.Left)
		{
			return this.leftCharacterLocatorManager.getAvatar();
		}
		if (side == SquadBattlePlayerEnum.Right)
		{
			return this.rightCharacterLocatorManager.getAvatar();
		}
		return null;
	}

	public void HighlightAvatar(int playerIndex)
	{
		this.HighlightLeftAvatar(false);
		this.HighlightRightAvatar(false);
		if (playerIndex == 0)
		{
			this.HighlightLeftAvatar(true);
		}
		else if (playerIndex == 1)
		{
			this.HighlightRightAvatar(true);
		}
	}

	protected GameObject CreatePlayerHighlightEffect()
	{
		UnityEngine.Object playerTurnIndicatorPrefab = CardGameController.Instance.playerTurnIndicatorPrefab;
		GameObject gameObject = UnityEngine.Object.Instantiate(playerTurnIndicatorPrefab) as GameObject;
		Animation component = gameObject.GetComponent<Animation>();
		if (component != null)
		{
			component["Take 001"].wrapMode = WrapMode.Once;
			component.Play();
		}
		return gameObject;
	}

	protected void HighlightLeftAvatar(bool show)
	{
		if (show)
		{
			if (this.leftPlayerHighlight == null)
			{
				this.leftPlayerHighlight = this.CreatePlayerHighlightEffect();
				GameObject avatar = this.leftCharacterLocatorManager.getAvatar();
				Utils.AttachGameObject(avatar, this.leftPlayerHighlight);
			}
		}
		else if (this.leftPlayerHighlight != null)
		{
			Utils.DetachGameObject(this.leftPlayerHighlight);
			UnityEngine.Object.Destroy(this.leftPlayerHighlight);
			this.leftPlayerHighlight = null;
		}
	}

	protected void HighlightRightAvatar(bool show)
	{
		if (show)
		{
			if (this.rightPlayerHighlight == null)
			{
				this.rightPlayerHighlight = this.CreatePlayerHighlightEffect();
				GameObject avatar = this.rightCharacterLocatorManager.getAvatar();
				Utils.AttachGameObject(avatar, this.rightPlayerHighlight);
			}
		}
		else if (this.rightPlayerHighlight != null)
		{
			Utils.DetachGameObject(this.rightPlayerHighlight);
			UnityEngine.Object.Destroy(this.rightPlayerHighlight);
			this.rightPlayerHighlight = null;
		}
	}

	public void HighDamageStreak(SquadBattlePlayerEnum winningSide, GameObject attacker, GameObject secondaryAttacker, GameObject defender)
	{
		CharacterLocatorManager characterLocatorManager;
		CharacterLocatorManager characterLocatorManager2;
		if (winningSide == SquadBattlePlayerEnum.Left)
		{
			characterLocatorManager = this.leftCharacterLocatorManager;
			characterLocatorManager2 = this.rightCharacterLocatorManager;
		}
		else
		{
			characterLocatorManager = this.rightCharacterLocatorManager;
			characterLocatorManager2 = this.leftCharacterLocatorManager;
		}
		if (characterLocatorManager.DoSomethingHappy(attacker, secondaryAttacker) || characterLocatorManager2.DoSomethingAngry(defender))
		{
		}
	}

	public void Bow()
	{
		SquadBattleCharacterAI squadBattleCharacterAI = this.leftCharacterLocatorManager.getAvatar().GetComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI;
		SquadBattleCharacterAI squadBattleCharacterAI2 = this.rightCharacterLocatorManager.getAvatar().GetComponent(typeof(SquadBattleCharacterAI)) as SquadBattleCharacterAI;
		if (squadBattleCharacterAI.CanBow() && squadBattleCharacterAI2.CanBow())
		{
			squadBattleCharacterAI.Bow();
			squadBattleCharacterAI2.Bow();
		}
	}

	public void RewardsPresentation()
	{
		base.StartCoroutine(this.RewardsRoutine());
	}

	protected IEnumerator RewardsRoutine()
	{
		SquadBattleCharacterController._RewardsRoutine_c__Iterator39 _RewardsRoutine_c__Iterator = new SquadBattleCharacterController._RewardsRoutine_c__Iterator39();
		_RewardsRoutine_c__Iterator.__f__this = this;
		return _RewardsRoutine_c__Iterator;
	}
}
