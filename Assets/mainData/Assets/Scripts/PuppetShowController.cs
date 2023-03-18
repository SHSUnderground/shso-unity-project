using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppetShowController : InteractiveObjectController
{
	public class Use
	{
		private PuppetShowController owner;

		private CharacterGlobals player;

		private OnDone onDone;

		private BehaviorSit sit;

		private BehaviorWait wait;

		private List<CharacterGlobals> hiddenPlayers;

		public bool InUse
		{
			get
			{
				return owner.inUse;
			}
			protected set
			{
				owner.inUse = value;
				owner.SetGlowable(!owner.inUse);
			}
		}

		public bool IsLocal
		{
			get
			{
				return Utils.IsLocalPlayer(player);
			}
		}

		public Use(GameObject player, PuppetShowController owner, OnDone onDone)
		{
			this.player = player.GetComponent<CharacterGlobals>();
			this.owner = owner;
			this.onDone = onDone;
		}

		public bool Start()
		{
			if (InUse)
			{
				return false;
			}
			ApproachBench();
			return true;
		}

		private void ApproachBench()
		{
			BehaviorApproach behaviorApproach = player.behaviorManager.requestChangeBehavior<BehaviorApproach>(false);
			behaviorApproach.Initialize(owner.benchDockPoint.transform.position, owner.benchDockPoint.transform.rotation, true, OnSitApproachFinished, OnSitApproachCancelled, 0.1f, 2f, true, false);
		}

		private void OnSitApproachFinished(GameObject player)
		{
			if (InUse)
			{
				OnSitApproachCancelled(player);
			}
			else
			{
				InUse = true;
			}
			if (IsLocal)
			{
				ZoomCameraIn();
			}
			SitDown();
			PlayPuppetShow();
			if (player == GameController.GetController().LocalPlayer && AppShell.Instance.Profile != null)
			{
				AppShell.Instance.delayedAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "social_zone", "asgard_puppet", string.Empty, 1f);
			}
			owner.StartCoroutine(WaitForPuppetShow());
		}

		private void OnSitApproachCancelled(GameObject player)
		{
			if (onDone != null)
			{
				onDone(player, CompletionStateEnum.Canceled);
			}
		}

		private void SitDown()
		{
			player.motionController.teleportTo(owner.benchDockPoint.transform.position, owner.benchDockPoint.transform.rotation);
			if (player.motionController.SitOverride == null)
			{
				sit = player.behaviorManager.requestChangeBehavior<BehaviorSit>(false);
				sit.Initialize(false, false);
			}
			else
			{
				wait = player.behaviorManager.requestChangeBehavior<BehaviorWait>(false);
			}
		}

		private void StandUp()
		{
			if (sit != null)
			{
				sit.stand();
			}
			if (wait != null)
			{
				wait.behaviorEnd();
				player.behaviorManager.endBehavior();
			}
		}

		private void ZoomCameraIn()
		{
			CameraLiteManager.Instance.PushCamera(owner.viewCamera, owner.zoomInTime);
		}

		private void ZoomCameraOut()
		{
			CameraLiteManager.Instance.PopCamera(owner.zoomOutTime);
		}

		private void PlayPuppetShow()
		{
			owner.puppetShowAnimation.Rewind();
			owner.puppetShowAnimation.Sample();
			owner.puppetShowAnimation[owner.puppetShowAnimationName].wrapMode = WrapMode.Once;
			owner.puppetShowAnimation.Play(owner.puppetShowAnimationName);
			if (IsLocal)
			{
				HideOtherPlayers();
				if (owner.effectSequence != null)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(owner.effectSequence.gameObject) as GameObject;
					EffectSequence component = gameObject.GetComponent<EffectSequence>();
					component.Initialize(owner.gameObject, DestroySequence, null);
					component.StartSequence();
				}
			}
		}

		private void DestroySequence(EffectSequence sequence)
		{
			UnityEngine.Object.Destroy(sequence.gameObject);
		}

		private IEnumerator WaitForPuppetShow()
		{
			yield return new WaitForSeconds(owner.puppetShowAnimation[owner.puppetShowAnimationName].length);
			OnPuppetShowFinished();
		}

		private void OnPuppetShowFinished()
		{
			if (player == null)
			{
				InUse = false;
				return;
			}
			if (IsLocal)
			{
				ZoomCameraOut();
				ShowOtherPlayers();
			}
			StandUp();
			InUse = false;
			if (onDone != null)
			{
				onDone(player.gameObject, CompletionStateEnum.Success);
			}
		}

		private void HideOtherPlayers()
		{
			hiddenPlayers = new List<CharacterGlobals>(Utils.FindObjectsOfType<CharacterGlobals>());
			foreach (CharacterGlobals hiddenPlayer in hiddenPlayers)
			{
				Utils.ActivateRenderers(hiddenPlayer.gameObject, false);
			}
			AppShell.Instance.EventMgr.AddListener<EntitySpawnMessage>(OnNewPlayerEnter);
			PuppetShowController puppetShowController = owner;
			puppetShowController.OnPuppetShowDestroyed = (OnPuppetShowDestroyedDelegate)Delegate.Combine(puppetShowController.OnPuppetShowDestroyed, new OnPuppetShowDestroyedDelegate(ShowOtherPlayers));
		}

		private void ShowOtherPlayers()
		{
			PuppetShowController puppetShowController = owner;
			puppetShowController.OnPuppetShowDestroyed = (OnPuppetShowDestroyedDelegate)Delegate.Remove(puppetShowController.OnPuppetShowDestroyed, new OnPuppetShowDestroyedDelegate(ShowOtherPlayers));
			AppShell.Instance.EventMgr.RemoveListener<EntitySpawnMessage>(OnNewPlayerEnter);
			foreach (CharacterGlobals hiddenPlayer in hiddenPlayers)
			{
				if (hiddenPlayer != null)
				{
					Utils.ActivateRenderers(hiddenPlayer.gameObject, true);
				}
			}
		}

		private void OnNewPlayerEnter(EntitySpawnMessage e)
		{
			CharacterGlobals component = e.go.GetComponent<CharacterGlobals>();
			if (component != null)
			{
				Utils.ActivateRenderers(component.gameObject, false);
				hiddenPlayers.Add(component);
			}
		}
	}

	public delegate void OnPuppetShowDestroyedDelegate();

	public CameraLite viewCamera;

	public float zoomInTime = 0.5f;

	public float zoomOutTime = 0.5f;

	public Animation puppetShowAnimation;

	public string puppetShowAnimationName = "Take 001";

	public DockPoint benchDockPoint;

	public EffectSequence effectSequence;

	public OnPuppetShowDestroyedDelegate OnPuppetShowDestroyed;

	private bool inUse;

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		Use use = new Use(player, this, onDone);
		return use.Start();
	}

	public void SetGlowable(bool enable)
	{
		owner.highlightOnProximity = enable;
		owner.highlightOnHover = enable;
		owner.GotoBestState();
	}

	protected void OnDestroy()
	{
		if (OnPuppetShowDestroyed != null)
		{
			OnPuppetShowDestroyed();
		}
	}
}
