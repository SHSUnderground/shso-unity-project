using UnityEngine;

public class GlowableInteractiveController : InteractiveObjectController
{
	public class BaseUse
	{
		private CharacterGlobals player;

		private GlowableInteractiveController baseOwner;

		private OnDone onDone;

		public bool InUse
		{
			get
			{
				return baseOwner.inUse;
			}
			protected set
			{
				baseOwner.inUse = value;
				baseOwner.SetGlowable(!baseOwner.inUse);
			}
		}

		public bool IsLocal
		{
			get
			{
				return Utils.IsLocalPlayer(player);
			}
		}

		public CharacterGlobals Player
		{
			get
			{
				return player;
			}
		}

		public BaseUse(GameObject player, GlowableInteractiveController owner, OnDone onDone)
		{
			this.player = player.GetComponent<CharacterGlobals>();
			baseOwner = owner;
			this.onDone = onDone;
		}

		public virtual bool Start()
		{
			return !InUse;
		}

		public virtual void Approach(Transform location)
		{
			Approach(location, 0.1f, false);
		}

		public virtual void Approach(Transform location, float tolerance, bool lookAtLocation)
		{
			BehaviorApproach behaviorApproach = player.behaviorManager.requestChangeBehavior<BehaviorApproach>(false);
			behaviorApproach.Initialize(location.position, location.rotation, true, CheckApproachComplete, OnApproachCancelled, tolerance, tolerance + 0.5f, true, lookAtLocation);
		}

		private void CheckApproachComplete(GameObject player)
		{
			if (InUse)
			{
				OnApproachCancelled(player);
				return;
			}
			InUse = true;
			OnApproachArrived(player);
		}

		protected virtual void OnApproachArrived(GameObject player)
		{
			Done();
		}

		protected virtual void OnApproachCancelled(GameObject player)
		{
			Done();
		}

		public virtual void Done()
		{
			InUse = false;
			if (onDone != null)
			{
				onDone(player.gameObject, CompletionStateEnum.Success);
			}
		}

		public T ChangeBehavior<T>(bool canBeQueued) where T : BehaviorBase
		{
			return player.behaviorManager.requestChangeBehavior<T>(canBeQueued);
		}
	}

	public const float DEFAULT_APPROACH_TOLERANCE = 0.1f;

	public const float APPROACH_ROTATE_BUFFER = 0.5f;

	protected bool inUse;

	public virtual void SetGlowable(bool enable)
	{
		owner.highlightOnProximity = enable;
		owner.highlightOnHover = enable;
		owner.GotoBestState();
	}
}
