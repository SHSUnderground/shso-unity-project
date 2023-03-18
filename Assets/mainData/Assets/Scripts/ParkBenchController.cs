using System.Collections.Generic;
using UnityEngine;

public class ParkBenchController : InteractiveObjectController
{
	protected class PlayerBlob
	{
		protected ParkBenchController owner;

		protected GameObject player;

		protected OnDone onDone;

		protected BehaviorManager behaviorManager;

		protected CharacterGlobals globs;

		protected int seatIndex;

		protected DockPoint sitDock;

		protected Animation animComp;

		public PlayerBlob(ParkBenchController owner, GameObject player, OnDone onDone)
		{
			this.owner = owner;
			this.player = player;
			this.onDone = onDone;
			globs = Utils.GetComponent<CharacterGlobals>(player);
			behaviorManager = globs.behaviorManager;
		}

		public virtual bool Start()
		{
			if (behaviorManager == null)
			{
				CspUtils.DebugLog("Can't sit; no BehaviorManager attached to " + player.name);
				return false;
			}
			sitDock = owner.GetFirstEmptyDockPoint();
			BehaviorApproach behaviorApproach = behaviorManager.requestChangeBehavior(typeof(BehaviorApproach), false) as BehaviorApproach;
			if (behaviorApproach == null)
			{
				CspUtils.DebugLog("Could not change player behavior to BehaviorApproach when trying to sit");
				return false;
			}
			behaviorApproach.Initialize(sitDock.transform.position, sitDock.transform.rotation, true, OnApproachFinished, OnApproachCancelled, 0.1f, 2f, true, false);
			if (player == GameController.GetController().LocalPlayer)
			{
				animComp = Utils.GetComponent<Animation>(owner.gameObject, Utils.SearchChildren);
				if (animComp != null)
				{
					animComp.Rewind();
					animComp.Sample();
					animComp.Stop();
				}
			}
			return true;
		}

		public virtual void OnApproachFinished(GameObject player)
		{
			if (owner.docksInUse.Contains(sitDock))
			{
				OnApproachCancelled(player);
				return;
			}
			globs.motionController.teleportTo(sitDock.transform.position, sitDock.transform.forward);
			BehaviorInterruptableSit behaviorInterruptableSit = behaviorManager.requestChangeBehavior(typeof(BehaviorInterruptableSit), false) as BehaviorInterruptableSit;
			if (behaviorInterruptableSit != null)
			{
				owner.docksInUse.Add(sitDock);
				behaviorInterruptableSit.Initialize(true, OnFinishedStanding);
				if (player == GameController.GetController().LocalPlayer)
				{
					AppShell.Instance.EventMgr.Fire(this, new InteractiveObjectUsedMessage(owner.owner));
				}
			}
		}

		public virtual void OnApproachCancelled(GameObject player)
		{
			NetworkComponent component = Utils.GetComponent<NetworkComponent>(player);
			if (component != null)
			{
				component.QueueNetAction(new NetActionCancel());
			}
			if (onDone != null)
			{
				onDone(player, CompletionStateEnum.Canceled);
			}
		}

		public virtual void OnFinishedStanding(BehaviorSit sit)
		{
			owner.docksInUse.Remove(sitDock);
			sit.dispatchStand();
			if (animComp != null && !string.IsNullOrEmpty(owner.animationOnStand))
			{
				animComp.Play(owner.animationOnStand);
			}
			if (onDone != null)
			{
				onDone(player, CompletionStateEnum.Success);
			}
		}
	}

	public List<DockPoint> dockPoints;

	public string animationOnStand = string.Empty;

	protected List<DockPoint> docksInUse = new List<DockPoint>();

	public override bool CanPlayerUse(GameObject player)
	{
		if (GetFirstEmptyDockPoint() != null)
		{
			CharacterGlobals component = Utils.GetComponent<CharacterGlobals>(player);
			if (component != null && component.behaviorManager != null)
			{
				return !(component.behaviorManager.getBehavior() is BehaviorSit);
			}
			return false;
		}
		return false;
	}

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		PlayerBlob playerBlob = new PlayerBlob(this, player, onDone);
		return playerBlob.Start();
	}

	protected DockPoint GetFirstEmptyDockPoint()
	{
		foreach (DockPoint dockPoint in dockPoints)
		{
			if (!docksInUse.Contains(dockPoint))
			{
				return dockPoint;
			}
		}
		return null;
	}
}
