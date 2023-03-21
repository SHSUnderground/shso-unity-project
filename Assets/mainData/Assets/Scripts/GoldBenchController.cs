using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldBenchController : InteractiveObjectController
{
	protected class PlayerBlob
	{
		protected GoldBenchController owner;

		protected GameObject player;

		protected OnDone onDone;

		protected BehaviorManager behaviorManager;

		protected CharacterGlobals globs;

		protected DockPoint sitDock;

		protected Animation animComp;

		public PlayerBlob(GoldBenchController owner, GameObject player, OnDone onDone)
		{
			this.owner = owner;
			this.player = player;
			this.onDone = onDone;
			globs = player.GetComponent<CharacterGlobals>();
			behaviorManager = globs.behaviorManager;
		}

		public bool Start()
		{
			if (player == null)
			{
				return false;
			}
			if (behaviorManager == null)
			{
				return false;
			}
			sitDock = owner.GetFirstEmptyDockPoint();
			if (sitDock == null)
			{
				return false;
			}
			BehaviorApproach behaviorApproach = behaviorManager.requestChangeBehavior(typeof(BehaviorApproach), false) as BehaviorApproach;
			if (behaviorApproach == null)
			{
				CspUtils.DebugLog("Could not change player behavior to BehaviorApproach when trying to sit");
				return false;
			}
			behaviorApproach.Initialize(sitDock.transform.position, sitDock.transform.rotation, true, OnApproachFinished, OnApproachCancelled, 0.1f, 2f, true, false);
			if (Utils.IsLocalPlayer(globs))
			{
				animComp = owner.gameObject.GetComponentInChildren<Animation>();
				if (animComp != null)
				{
					animComp.Rewind();
					animComp.Sample();
					animComp.Stop();
				}
			}
			return true;
		}

		public void OnApproachFinished(GameObject player)
		{
			if (owner.docksInUse.Contains(sitDock))
			{
				OnApproachCancelled(player);
				return;
			}
			globs.motionController.teleportTo(sitDock.transform.position, sitDock.transform.forward);
			BehaviorSit behaviorSit = behaviorManager.requestChangeBehavior(typeof(BehaviorSit), false) as BehaviorSit;
			if (behaviorSit != null)
			{
				owner.docksInUse.Add(sitDock);
				owner.StartCoroutine(FailSafe(player, behaviorSit, sitDock, onDone));
				behaviorSit.Initialize(true);
				behaviorSit.OnFinishedSitting += OnSittingFinished;
				behaviorSit.OnFinishedStanding += OnStandingFinished;
				if (player == GameController.GetController().LocalPlayer && AppShell.Instance.Profile != null)
				{
					AppShell.Instance.delayedAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "social_zone", owner.achievementEvent, string.Empty, 1f);
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

		public virtual void OnSittingFinished(BehaviorSit sit)
		{
			if (player == null)
			{
				return;
			}
			if (owner.typeToAdd != null)
			{
				Component component = player.GetComponent(owner.typeToAdd);
				if (component == null)
				{
					component = player.AddComponent(owner.typeToAdd);
				}
				IComponentTimeInit componentTimeInit = component as IComponentTimeInit;
				if (componentTimeInit != null)
				{
					componentTimeInit.SetDelay(owner.componentDelay);
					componentTimeInit.SetDuration(owner.componentDuration);
				}
				component.SendMessage("SetApplySequence", owner.effectSequenceOnApply, SendMessageOptions.DontRequireReceiver);
				component.SendMessage("SetRevertSequence", owner.effectSequenceOnRevert, SendMessageOptions.DontRequireReceiver);
				component.SendMessage("SetSequenceOwner", owner.effectSequenceParentObject, SendMessageOptions.DontRequireReceiver);
				//owner.effectSequenceParentObject
			}
			sit.stand();
		}

		public virtual void OnStandingFinished(BehaviorSit sit)
		{
			owner.StopAllCoroutines();
			owner.docksInUse.Remove(sitDock);
			if (animComp != null && !string.IsNullOrEmpty(owner.animationOnStand))
			{
				animComp.Play(owner.animationOnStand);
			}
			if (onDone != null)
			{
				onDone(player, CompletionStateEnum.Success);
			}
		}

		protected IEnumerator FailSafe(GameObject player, BehaviorSit sit, DockPoint dockPt, OnDone onDone)
		{
			yield return new WaitForSeconds(sit.SitAnimationLength + sit.StandAnimationLength + 1f);
			if (player == null)
			{
				owner.docksInUse.Remove(dockPt);
				if (onDone != null)
				{
					onDone(null, CompletionStateEnum.Unknown);
				}
			}
		}
	}

	public DockPoint[] dockPoints;

	public EffectSequence effectSequenceOnApply;

	public EffectSequence effectSequenceOnRevert;

	public GameObject effectSequenceParentObject;

	public string animationOnStand = string.Empty;

	public string componentToAdd = string.Empty;

	public float componentDelay = 0.25f;

	public float componentDuration = 30f;

	public string achievementEvent = string.Empty;

	protected List<DockPoint> docksInUse = new List<DockPoint>();

	protected Type typeToAdd;

	public void Start()
	{
		if (!string.IsNullOrEmpty(componentToAdd))
		{
			typeToAdd = Type.GetType(componentToAdd);
			if (typeToAdd == null)
			{
				CspUtils.DebugLog("Unable to find type <" + componentToAdd + ">");
			}
		}
	}

	public override bool ShouldIgnoreMouseClick(GameObject player)
	{
		if (typeToAdd != null)
		{
			Component component = player.GetComponent(typeToAdd);
			if (component != null)
			{
				return true;
			}
		}
		return base.ShouldIgnoreMouseClick(player);
	}

	public override bool CanPlayerUse(GameObject player)
	{
		if (GetFirstEmptyDockPoint() == null)
		{
			return false;
		}
		CharacterGlobals component = player.GetComponent<CharacterGlobals>();
		if (component == null || component.behaviorManager == null)
		{
			return false;
		}
		if (component.behaviorManager.getBehavior() is BehaviorSit)
		{
			return false;
		}
		if (typeToAdd != null)
		{
			Component component2 = player.GetComponent(typeToAdd);
			if (component2 != null)
			{
				return false;
			}
		}
		return true;
	}

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		PlayerBlob playerBlob = new PlayerBlob(this, player, onDone);
		return playerBlob.Start();
	}

	protected DockPoint GetFirstEmptyDockPoint()
	{
		DockPoint[] array = dockPoints;
		foreach (DockPoint dockPoint in array)
		{
			if (!docksInUse.Contains(dockPoint))
			{
				return dockPoint;
			}
		}
		return null;
	}
}
