using System;
using System.Collections.Generic;
using UnityEngine;

public class CoinViewState : InteractiveObjectController
{
	internal class BaseState : IDisposable, IShsState
	{
		protected CoinViewState owner;

		protected EffectSequence sequence;

		public BaseState(CoinViewState owner)
		{
			this.owner = owner;
		}

		public virtual void Enter(Type previousState)
		{
		}

		public virtual void Update()
		{
		}

		public virtual void Leave(Type nextState)
		{
			if (sequence != null)
			{
				UnityEngine.Object.Destroy(sequence.gameObject);
			}
		}

		public virtual void Dispose()
		{
			owner = null;
		}
	}

	internal class OffState : BaseState
	{
		public OffState(CoinViewState owner)
			: base(owner)
		{
		}

		public override void Enter(Type previousState)
		{
			base.Enter(previousState);
			sequence = owner.StartEffectSequence(owner.OffSequence, null);
			if (owner.hideWhileDisabled)
			{
				Renderer[] componentsInChildren = owner.gameObject.GetComponentsInChildren<Renderer>(true);
				Renderer[] array = componentsInChildren;
				foreach (Renderer renderer in array)
				{
					if (string.IsNullOrEmpty(owner.hideOnlyParentNameContains) || renderer.gameObject.name.ToLowerInvariant().Contains(owner.hideOnlyParentNameContains))
					{
						renderer.enabled = false;
					}
				}
				owner.disabledColliders = new List<Collider>();
				Collider[] componentsInChildren2 = owner.gameObject.GetComponentsInChildren<Collider>();
				Collider[] array2 = componentsInChildren2;
				foreach (Collider collider in array2)
				{
					if (!collider.isTrigger)
					{
						owner.disabledColliders.Add(collider);
						collider.isTrigger = true;
					}
				}
				if (owner != null && owner.owner != null)
				{
					owner.owner.hidden = true;
				}
			}
			owner.hideTime = Time.time;
		}

		public override void Update()
		{
			base.Update();
			if (!(Time.time - owner.hideTime > owner.hideDuration))
			{
				return;
			}
			if (owner.hideWhileDisabled)
			{
				Renderer[] componentsInChildren = owner.gameObject.GetComponentsInChildren<Renderer>(true);
				Renderer[] array = componentsInChildren;
				foreach (Renderer renderer in array)
				{
					if (string.IsNullOrEmpty(owner.hideOnlyParentNameContains) || renderer.gameObject.name.ToLowerInvariant().Contains(owner.hideOnlyParentNameContains))
					{
						renderer.enabled = false;
					}
				}
			}
			owner.fsm.GotoState<TurningOnState>();
		}
	}

	internal class TurningOnState : BaseState
	{
		private float startT;

		private bool turningOnDone;

		public TurningOnState(CoinViewState owner)
			: base(owner)
		{
		}

		public override void Enter(Type previousState)
		{
			base.Enter(previousState);
			startT = Time.time;
			turningOnDone = false;
			sequence = owner.StartEffectSequence(owner.TurningOnSequence, delegate
			{
				turningOnDone = true;
			});
		}

		public override void Update()
		{
			base.Update();
			float num = (Time.time - startT) / owner.fadeInTime;
			if (num > 1f || turningOnDone)
			{
				owner.fsm.GotoState<OnState>();
			}
		}
	}

	internal class TurningOffState : BaseState
	{
		private float startT;

		private bool turningOffDone;

		public TurningOffState(CoinViewState owner)
			: base(owner)
		{
		}

		public override void Enter(Type previousState)
		{
			base.Enter(previousState);
			if (owner.lastUsedTimeCounter != null)
			{
				owner.lastUsedTimeCounter.SetCounter((long)ServerTime.time);
			}
			startT = Time.time;
			owner.hideDuration = 600f - owner.fadeOutTime;
			owner.UpdateCooldown();
			turningOffDone = false;
			sequence = owner.StartEffectSequence(owner.TurningOffSequence, delegate
			{
				turningOffDone = true;
			});
		}

		public override void Update()
		{
			base.Update();
			if (sequence == null)
			{
				float num = (Time.time - startT) / owner.fadeOutTime;
				turningOffDone = (num > 1f);
			}
			if (turningOffDone)
			{
				owner.fsm.GotoState<OffState>();
			}
		}
	}

	internal class OnState : BaseState
	{
		public OnState(CoinViewState owner)
			: base(owner)
		{
		}

		public override void Enter(Type previousState)
		{
			base.Enter(previousState);
			if (owner.hideWhileDisabled)
			{
				Renderer[] componentsInChildren = owner.gameObject.GetComponentsInChildren<Renderer>(true);
				Renderer[] array = componentsInChildren;
				foreach (Renderer renderer in array)
				{
					if (string.IsNullOrEmpty(owner.hideOnlyParentNameContains) || renderer.gameObject.name.ToLowerInvariant().Contains(owner.hideOnlyParentNameContains))
					{
						renderer.enabled = true;
					}
				}
				foreach (Collider disabledCollider in owner.disabledColliders)
				{
					disabledCollider.isTrigger = false;
				}
				if (owner != null && owner.owner != null)
				{
					owner.owner.hidden = false;
					owner.owner.GotoBestState();
				}
			}
			sequence = owner.StartEffectSequence(owner.OnSequence, null);
		}

		public override void Update()
		{
			base.Update();
		}
	}

	private const int kRespawnTimeSeconds = 600;

	public bool hideWhileDisabled = true;

	public string hideOnlyParentNameContains;

	public EffectSequence OffSequence;

	public EffectSequence TurningOnSequence;

	public EffectSequence OnSequence;

	public EffectSequence TurningOffSequence;

	public float fadeInTime = 1f;

	public float fadeOutTime = 1f;

	public int coinSpawnerIndex;

	private float hideTime;

	private float hideDuration;

	private ShsFSM fsm;

	private List<Collider> disabledColliders;

	private ISHSCounterType lastUsedTimeCounter;

	public CoinViewState()
	{
		fsm = new ShsFSM();
		fsm.AddState(new OffState(this));
		fsm.AddState(new TurningOnState(this));
		fsm.AddState(new TurningOffState(this));
		fsm.AddState(new OnState(this));
	}

	public void Start()
	{
		if (coinSpawnerIndex == 0)
		{
			CspUtils.DebugLog("Coin Spawner \"" + base.gameObject.name + "\" has not been given a spawner index, and will thus be destroyed");
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		string counterType = "CoinSpawners.CS" + coinSpawnerIndex;
		lastUsedTimeCounter = AppShell.Instance.CounterManager.GetCounter(counterType);
		if (lastUsedTimeCounter != null)
		{
			hideDuration = (float)Math.Max(600.0 - (ServerTime.time - (double)lastUsedTimeCounter.GetCurrentValue()), 0.0);
		}
		else
		{
			hideDuration = 0f;
		}
		fsm.GotoState<OffState>();
	}

	public void Reset()
	{
		fsm.GotoState<TurningOffState>();
	}

	public void ForceShow()
	{
		fsm.GotoState<OnState>();
	}

	public void Update()
	{
		fsm.Update();
	}

	public override bool CanPlayerUse(GameObject player)
	{
		return fsm.GetCurrentStateObject() is OnState || fsm.GetCurrentStateObject() is OffState;
	}

	public bool IsReadyToSpew()
	{
		return fsm.GetCurrentStateObject() is OnState;
	}

	private void UpdateCooldown()
	{
		float cooldown = fadeInTime + hideDuration + fadeOutTime;
		InteractiveObjectCooldown component = Utils.GetComponent<InteractiveObjectCooldown>(base.gameObject, Utils.SearchChildren);
		if (component != null)
		{
			component.cooldown = cooldown;
		}
	}

	protected EffectSequence StartEffectSequence(EffectSequence es, EffectSequence.OnSequenceDone onDone)
	{
		if (es == null)
		{
			return null;
		}
		GameObject g = UnityEngine.Object.Instantiate(es.gameObject) as GameObject;
		EffectSequence component = Utils.GetComponent<EffectSequence>(g);
		GameObject gameObject = model;
		if (gameObject == null)
		{
			gameObject = Utils.FindNodeInChildren(base.transform, "Model").gameObject;
		}
		if (component != null)
		{
			component.Initialize(gameObject, onDone, null);
			component.StartSequence();
			return component;
		}
		return null;
	}
}
