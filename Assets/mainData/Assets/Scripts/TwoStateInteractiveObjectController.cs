using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Interactive Object/Two State Controller")]
public class TwoStateInteractiveObjectController : InteractiveObjectController
{
	public class DefaultState : IShsState
	{
		protected TwoStateInteractiveObjectController owner;

		public DefaultState(TwoStateInteractiveObjectController owner)
		{
			this.owner = owner;
		}

		public void Enter(Type previousState)
		{
			owner.states[0].model.SetActiveRecursively(true);
			owner.states[1].model.SetActiveRecursively(false);
		}

		public void Update()
		{
		}

		public void Leave(Type nextState)
		{
		}
	}

	public class ChangedState : IShsState
	{
		protected float timeFX;

		protected float timeChange;

		protected float timeRevert;

		protected TwoStateInteractiveObjectController owner;

		protected GameObject player;

		protected ActivateInfo info;

		protected EffectSequence fxSeq;

		public ChangedState(TwoStateInteractiveObjectController owner)
		{
			this.owner = owner;
		}

		public void Enter(Type previousState)
		{
			owner.owner.GotoBestState();
		}

		public void Update()
		{
			if (fxSeq != null && timeFX >= 0f)
			{
				timeFX -= Time.deltaTime;
				if (timeFX < 0f)
				{
					fxSeq.StartSequence();
				}
			}
			if ((double)timeChange > 0.0)
			{
				timeChange -= Time.deltaTime;
				if (timeChange <= 0f)
				{
					owner.states[0].model.SetActiveRecursively(false);
					owner.states[1].model.SetActiveRecursively(true);
					if (fxSeq != null)
					{
						fxSeq.Cancel();
						fxSeq = null;
					}
				}
			}
			timeRevert -= Time.deltaTime;
			if (timeRevert <= 0f)
			{
				owner.fsm.GotoState<DefaultState>();
			}
		}

		public void Leave(Type nextState)
		{
			if (fxSeq != null)
			{
				fxSeq.Cancel();
				fxSeq = null;
			}
			owner.owner.GotoBestState();
		}

		public void SetPlayer(GameObject player, BehaviorManager behaviorManager)
		{
			this.player = player;
			info = null;
			if (player == owner.cachedActivatePlayer)
			{
				info = owner.cachedActivateInfo;
			}
			else
			{
				owner.infoDict.TryGetValue(player.name.ToLower(), out info);
			}
			if (info == null)
			{
				info = new ActivateInfo();
				info.hero = "unknown";
				info.animation = "emote_poke";
				info.time = 1f;
			}
			BehaviorAnimate behaviorAnimate = behaviorManager.requestChangeBehavior(typeof(BehaviorAnimate), true) as BehaviorAnimate;
			behaviorAnimate.Initialize(info.animation, null);
			if (owner.states[1].fxPrefab != null)
			{
				GameObject g = UnityEngine.Object.Instantiate(owner.states[1].fxPrefab.gameObject, owner.states[1].model.transform.position, Quaternion.identity) as GameObject;
				fxSeq = Utils.GetComponent<EffectSequence>(g);
				fxSeq.Initialize(null, null, null);
			}
			else
			{
				fxSeq = null;
			}
			timeFX = info.fxdelay;
			timeChange = info.time;
			timeRevert = owner.duration;
		}
	}

	protected class PlayerBlob
	{
		protected GameObject player;

		protected TwoStateInteractiveObjectController owner;

		protected BehaviorManager behaviorManager;

		protected DockPoint bestPoint;

		protected OnDone onDone;

		public PlayerBlob(GameObject player, TwoStateInteractiveObjectController owner, OnDone onDone)
		{
			this.player = player;
			this.owner = owner;
			behaviorManager = Utils.GetComponent<BehaviorManager>(player);
			bestPoint = null;
			this.onDone = onDone;
		}

		public bool Start()
		{
			DockPoint[] components = Utils.GetComponents<DockPoint>(owner.states[0].model, Utils.SearchChildren, true);
			float num = float.MaxValue;
			bestPoint = null;
			for (int i = 0; i < components.Length; i++)
			{
				float sqrMagnitude = (player.transform.position - components[i].transform.position).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					bestPoint = components[i];
				}
			}
			if (bestPoint == null)
			{
				CspUtils.DebugLog("Failed to find dock point on " + owner.states[0].model.name);
				return false;
			}
			BehaviorApproach behaviorApproach = behaviorManager.requestChangeBehavior(typeof(BehaviorApproach), false) as BehaviorApproach;
			if (behaviorApproach == null)
			{
				return false;
			}
			CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(player);
			if (component != null)
			{
				component.DisableNetUpdates(true);
			}
			behaviorApproach.Initialize(bestPoint.transform.position, bestPoint.transform.rotation, false, ArrivedDock, CancelDock, 0.15f, 2f, false, false);
			return true;
		}

		protected void ArrivedDock(GameObject player)
		{
			owner.fsm.GotoState<ChangedState>();
			ChangedState changedState = (ChangedState)owner.fsm.GetCurrentStateObject();
			changedState.SetPlayer(player, behaviorManager);
			CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(player);
			if (component != null)
			{
				component.DisableNetUpdates(false);
			}
			if (onDone != null)
			{
				onDone(player, CompletionStateEnum.Success);
			}
		}

		protected void CancelDock(GameObject player)
		{
			player.transform.position = bestPoint.transform.position;
			player.transform.rotation = bestPoint.transform.rotation;
			ArrivedDock(player);
		}
	}

	[Serializable]
	public class StateInfo
	{
		public GameObject model;

		public EffectSequence fxPrefab;
	}

	[Serializable]
	public class ActivateInfo
	{
		public string hero;

		public string animation;

		public float fxdelay;

		public float time;
	}

	public float duration = 10f;

	public ActivateInfo[] activationInfo;

	public StateInfo[] states = new StateInfo[2];

	protected ShsFSM fsm;

	protected Dictionary<string, ActivateInfo> infoDict;

	protected GameObject cachedActivatePlayer;

	protected ActivateInfo cachedActivateInfo;

	public void Start()
	{
		fsm = new ShsFSM();
		fsm.AddState(new DefaultState(this));
		fsm.AddState(new ChangedState(this));
		fsm.GotoState<DefaultState>();
		infoDict = new Dictionary<string, ActivateInfo>();
		ActivateInfo[] array = activationInfo;
		foreach (ActivateInfo activateInfo in array)
		{
			infoDict.Add(activateInfo.hero.ToLower(), activateInfo);
		}
		cachedActivatePlayer = null;
		cachedActivateInfo = null;
	}

	public void Update()
	{
		fsm.Update();
	}

	public override InteractiveObject.StateIdx GetStateForPlayer(GameObject player)
	{
		if (CanPlayerUse(player))
		{
			return InteractiveObject.StateIdx.Enable;
		}
		return InteractiveObject.StateIdx.Disable;
	}

	public override bool CanPlayerUse(GameObject player)
	{
		if (player == null)
		{
			return false;
		}
		if (cachedActivatePlayer != player)
		{
			cachedActivatePlayer = player;
			cachedActivateInfo = null;
			if (!infoDict.TryGetValue(player.name.ToLower(), out cachedActivateInfo))
			{
				return false;
			}
		}
		if (fsm.GetCurrentState() == typeof(ChangedState))
		{
			return false;
		}
		return true;
	}

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		PlayerBlob playerBlob = new PlayerBlob(player, this, onDone);
		return playerBlob.Start();
	}
}
