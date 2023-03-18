using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEngine;

[RequireComponent(typeof(CharacterMotionController))]
[RequireComponent(typeof(BehaviorManager))]
[RequireComponent(typeof(PlayerCombatController))]
public class PlayerInputControllerDebug : PlayerInputController
{
	public EffectSequence effectSeq;

	protected CharacterStats characterStats;

	protected TextBillboard textBillboard;

	protected PlayerCombatController playerCombatController;

	private List<KeyCodeEntry> debugKeys;

	private bool initialized;

	private int currentLod = -1;

	protected override void Start()
	{
		base.Start();
		characterStats = charGlobals.stats;
		textBillboard = (GetComponentInChildren(typeof(TextBillboard)) as TextBillboard);
		playerCombatController = (combatController as PlayerCombatController);
		RegisterEvents();
		if (combatController != null && !combatController.isHidden)
		{
			RegisterDebugKeys();
		}
		initialized = true;
	}

	protected void RegisterEvents()
	{
		AppShell.Instance.EventMgr.AddListener<EatMessage>(OnEatMessage);
		AppShell.Instance.EventMgr.AddListener<SetLevelMessage>(OnSetLevelMessage);
		AppShell.Instance.EventMgr.AddListener<FillPowerBarMessage>(OnFillPowerBarMessage);
		AppShell.Instance.EventMgr.AddListener<SendGameMessageMessage>(OnSendGameMessageMessage);
		AppShell.Instance.EventMgr.AddListener<ChangeBehaviorMessage>(OnChangeBehaviorMessage);
		AppShell.Instance.EventMgr.AddListener<CallMethodOnBehavior>(OnCallMethodOnBehaviorMessage);
		AppShell.Instance.EventMgr.AddListener<ShowCodeRedemptionDialogMessage>(OnShowCodeRedemptionDialogMessage);
		AppShell.Instance.EventMgr.AddListener<FollowMessage>(OnFollowMessage);
		AppShell.Instance.EventMgr.AddListener<EntityPolymorphMessage>(OnEntityPolymorphMessage);
	}

	protected void UnregisterEvents()
	{
		AppShell.Instance.EventMgr.RemoveListener<EatMessage>(OnEatMessage);
		AppShell.Instance.EventMgr.RemoveListener<SetLevelMessage>(OnSetLevelMessage);
		AppShell.Instance.EventMgr.RemoveListener<FillPowerBarMessage>(OnFillPowerBarMessage);
		AppShell.Instance.EventMgr.RemoveListener<SendGameMessageMessage>(OnSendGameMessageMessage);
		AppShell.Instance.EventMgr.RemoveListener<ChangeBehaviorMessage>(OnChangeBehaviorMessage);
		AppShell.Instance.EventMgr.RemoveListener<CallMethodOnBehavior>(OnCallMethodOnBehaviorMessage);
		AppShell.Instance.EventMgr.RemoveListener<ShowCodeRedemptionDialogMessage>(OnShowCodeRedemptionDialogMessage);
		AppShell.Instance.EventMgr.RemoveListener<FollowMessage>(OnFollowMessage);
		AppShell.Instance.EventMgr.RemoveListener<EntityPolymorphMessage>(OnEntityPolymorphMessage);
	}

	protected void RegisterDebugKeys()
	{
		debugKeys = new List<KeyCodeEntry>();
		KeyCodeEntry keyCodeEntry = new KeyCodeEntry(KeyCode.E, true, true, true);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, Eat);
		keyCodeEntry = new KeyCodeEntry(KeyCode.Equals, true, true, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, LevelUp);
		keyCodeEntry = new KeyCodeEntry(KeyCode.Minus, true, true, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, LevelDown);
		keyCodeEntry = new KeyCodeEntry(KeyCode.KeypadPlus, true, true, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, FillPowerBar);
		keyCodeEntry = new KeyCodeEntry(KeyCode.R, true, true, true);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, CodeRedemptionDialog);
		keyCodeEntry = new KeyCodeEntry(KeyCode.O, true, true, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, Follow);
		keyCodeEntry = new KeyCodeEntry(KeyCode.S, true, true, true);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, Sit);
		keyCodeEntry = new KeyCodeEntry(KeyCode.M, true, true, true);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, Manipulate);
		keyCodeEntry = new KeyCodeEntry(KeyCode.T, true, true, true);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, Stun);
		keyCodeEntry = new KeyCodeEntry(KeyCode.F3, false, false, false);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, Inspect);
		keyCodeEntry = new KeyCodeEntry(KeyCode.L, true, true, true);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, CycleLOD);
		keyCodeEntry = new KeyCodeEntry(KeyCode.K, true, true, true);
		debugKeys.Add(keyCodeEntry);
		SHSDebugInput.Inst.AddKeyListener(keyCodeEntry, PlayBlockAnim);
	}

	protected void UnregisterDebugKeys()
	{
		if (debugKeys != null)
		{
			foreach (KeyCodeEntry debugKey in debugKeys)
			{
				SHSDebugInput.Inst.RemoveKeyListener(debugKey);
			}
			debugKeys = null;
		}
	}

	protected new void OnDisable()
	{
		if (initialized)
		{
			UnregisterDebugKeys();
			UnregisterEvents();
		}
		base.OnDisable();
	}

	protected override void Update()
	{
		if (!(behaviorManager != null) || !behaviorManager.allowUserInput())
		{
		}
		base.Update();
	}

	protected void OnSequenceDone(EffectSequence finishedSeq)
	{
		CspUtils.DebugLog("Effect sequence finished");
	}

	protected void OnDialogReply(ShowDialogReplyMessage message)
	{
		CspUtils.DebugLog("Got reply: " + message.Message);
	}

	protected void OnTextBillboardChanged(GameObject newBillboard)
	{
		if (newBillboard != null)
		{
			textBillboard = newBillboard.GetComponent<TextBillboard>();
		}
	}

	public IEnumerator ShowBillboardText(string message, float duration)
	{
		textBillboard.Text = message;
		yield return new WaitForSeconds(duration);
		textBillboard.Text = string.Empty;
	}

	private void OnEatMessage(EatMessage msg)
	{
		Eat(msg.ItemHeight);
	}

	[Description("Make character eat an apple")]
	private void Eat(SHSKeyCode code)
	{
		float heightPerc = 0.3f;
		if (SHSInput.GetKey(KeyCode.LeftShift))
		{
			heightPerc = 0.75f;
		}
		else if (SHSInput.GetKey(KeyCode.RightShift))
		{
			heightPerc = 1.5f;
		}
		Eat(heightPerc);
	}

	private void Eat(float heightPerc)
	{
		CspUtils.DebugLog("Starting eat behavior.");
		BehaviorEat behaviorEat = behaviorManager.getBehavior() as BehaviorEat;
		if (behaviorEat != null)
		{
			return;
		}
		BehaviorEat behaviorEat2 = behaviorManager.requestChangeBehavior(typeof(BehaviorEat), false) as BehaviorEat;
		if (behaviorEat2 != null)
		{
			GameObject original = Resources.Load("TestData/TestEatApple", typeof(GameObject)) as GameObject;
			GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
			if (gameObject != null)
			{
				gameObject.transform.position = base.gameObject.transform.position + base.gameObject.transform.forward * 1.5f + base.gameObject.transform.up * heightPerc;
				behaviorEat2.Initialize(gameObject, DoneEating);
			}
		}
	}

	private void DoneEating(GameObject obj)
	{
		if (obj != null)
		{
			UnityEngine.Object.Destroy(obj);
		}
	}

	[Description("Increment character's level")]
	private void LevelUp(SHSKeyCode code)
	{
		SetLevel(playerCombatController.characterLevel + 1);
	}

	[Description("Decrement character's level")]
	private void LevelDown(SHSKeyCode code)
	{
		SetLevel(playerCombatController.characterLevel - 1);
	}

	private void OnSetLevelMessage(SetLevelMessage msg)
	{
		SetLevel(msg.Level);
	}

	private void SetLevel(int level)
	{
		CspUtils.DebugLog("SETTING LEVEL IN PlayerInputControllerDebug " + level + " " + characterStats.name);
		if (level > 0)
		{
			playerCombatController.changeLevel(level);
		}
		StartCoroutine(ShowBillboardText(playerCombatController.characterLevel.ToString(), 2f));
	}

	[Description("Fill the power bar")]
	private void FillPowerBar(SHSKeyCode code)
	{
		OnFillPowerBarMessage(null);
	}

	private void OnFillPowerBarMessage(FillPowerBarMessage msg)
	{
		if (combatController is PlayerCombatController && characterStats != null)
		{
			CharacterStat stat = characterStats.GetStat(CharacterStats.StatType.Power);
			stat.Value = 100f;
		}
	}

	private void OnShowCodeRedemptionDialogMessage(ShowCodeRedemptionDialogMessage msg)
	{
	}

	[Description("Bring up code redemption dialog")]
	private void CodeRedemptionDialog(SHSKeyCode code)
	{
		CspUtils.DebugLog("Bringing up code redemption dialog.");
		AppShell.Instance.EventMgr.Fire(this, new ShowDialogMessage(ShowDialogMessage.DialogTypeEnum.Code, "Hi there, I'm a wide mouthed frog, and I eat BUGZ!", OnDialogReply));
	}

	private void OnSendGameMessageMessage(SendGameMessageMessage msg)
	{
		SendGameMessage(msg.Message);
	}

	private void SendGameMessage(string message)
	{
		TestMessage testMessage = new TestMessage();
		testMessage.testString = message;
		int[] gameAllUserIds = AppShell.Instance.ServerConnection.GetGameAllUserIds();
		int[] array = gameAllUserIds;
		foreach (int userId in array)
		{
			AppShell.Instance.ServerConnection.SendGameMsg(testMessage, userId);
		}
	}

	private void OnFollowMessage(FollowMessage msg)
	{
		Follow(new SHSKeyCode(KeyCode.F));
	}

	private void OnEntityPolymorphMessage(EntityPolymorphMessage msg)
	{
		if (msg.original == base.gameObject)
		{
			base.AllowInput = false;
			stopInputStrict = true;
			PlayerInputControllerDebug component = msg.polymorph.GetComponent<PlayerInputControllerDebug>();
			if (component != null)
			{
				UnregisterDebugKeys();
				component.RegisterDebugKeys();
			}
		}
		else if (msg.polymorph == base.gameObject)
		{
			gameController.LocalPlayer = base.gameObject;
			base.AllowInput = true;
			stopInputStrict = false;
		}
	}

	[Description("Follow character under cursor")]
	private void Follow(SHSKeyCode code)
	{
		CspUtils.DebugLog("Follow called");
		RaycastHit hit;
		if (Utils.FindObjectUnderCursor(out hit))
		{
			GameObject gameObject = hit.collider.gameObject;
			if (gameObject.layer == 14)
			{
				gameObject = gameObject.transform.parent.gameObject;
			}
			if (gameObject.GetComponent(typeof(CharacterController)) != null)
			{
				motionController.setDestination(gameObject, 1.5f);
				CspUtils.DebugLog("Following " + gameObject.name);
			}
		}
	}

	private void OnCallMethodOnBehaviorMessage(CallMethodOnBehavior msg)
	{
		CallMethodOnCurrentBehavior(msg.MethodName);
	}

	private void CallMethodOnCurrentBehavior(string methodName)
	{
		BehaviorBase behavior = behaviorManager.getBehavior();
		Type type = behavior.GetType();
		MethodInfo method = type.GetMethod(methodName);
		if (method != null)
		{
			method.Invoke(behavior, new UnityEngine.Object[0]);
		}
	}

	private void OnChangeBehaviorMessage(ChangeBehaviorMessage msg)
	{
		Type type = Type.GetType(msg.BehaviorName);
		Type typeFromHandle = typeof(BehaviorBase);
		if (type.IsSubclassOf(typeFromHandle))
		{
			ChangeBehavior(type);
		}
	}

	private void ChangeBehavior(Type behavior)
	{
		BehaviorBase behavior2 = behaviorManager.getBehavior();
		if (behavior != behavior2.GetType())
		{
			behaviorManager.requestChangeBehavior(behavior, false);
		}
		else
		{
			behaviorManager.endBehavior();
		}
	}

	[Description("Cause character to sit")]
	private void Sit(SHSKeyCode code)
	{
		BehaviorSit behaviorSit = behaviorManager.getBehavior() as BehaviorSit;
		if (behaviorSit == null)
		{
			behaviorManager.requestChangeBehavior(typeof(BehaviorSit), false);
		}
		else
		{
			behaviorSit.stand();
		}
	}

	[Description("Cause character to manipulate")]
	private void Manipulate(SHSKeyCode code)
	{
		BehaviorManipulate behaviorManipulate = behaviorManager.getBehavior() as BehaviorManipulate;
		if (behaviorManipulate == null)
		{
			CspUtils.DebugLog("Starting manipulate behavior.");
			behaviorManager.requestChangeBehavior(typeof(BehaviorManipulate), false);
		}
		else
		{
			CspUtils.DebugLog("Stopping manipulate behavior.");
			behaviorManager.endBehavior();
		}
	}

	[Description("Cause character to play or stop their stun animation")]
	private void Stun(SHSKeyCode code)
	{
		if (base.animation.IsPlaying("recoil_stun"))
		{
			base.animation.wrapMode = WrapMode.Default;
			base.animation.CrossFade("movement_idle");
		}
		else
		{
			base.animation.wrapMode = WrapMode.Loop;
			base.animation.CrossFade("recoil_stun");
		}
	}

	[Description("Make next click an inspection")]
	private void Inspect(SHSKeyCode code)
	{
		DebugNextClick();
	}

	private void CycleLOD(SHSKeyCode code)
	{
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		if (localPlayer != null)
		{
			currentLod++;
			if (currentLod > 3)
			{
				currentLod = -1;
			}
			localPlayer.GetComponent<LodCharacter>().ForceSetLod(currentLod);
			CspUtils.DebugLog("Setting LOD for <" + localPlayer.name + "> to {" + ((currentLod != -1) ? currentLod.ToString() : "automatic") + "}");
		}
	}

	private void PlayBlockAnim(SHSKeyCode code)
	{
		GameObject localPlayer = GameController.GetController().LocalPlayer;
		if (localPlayer != null)
		{
			localPlayer.GetComponent<Animation>().Play("block");
		}
	}
}
