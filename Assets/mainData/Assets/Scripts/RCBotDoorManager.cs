using System.Collections;
using UnityEngine;

public class RCBotDoorManager : DoorManager
{
	public class RCBotPlayerBlob : PlayerBlob
	{
		private RCBotDoorManager _rcBotOwner;

		protected bool spawned;

		protected CharacterGlobals rcBotGlobals;

		protected CameraLite transitionCamera;

		protected RCBotDoorManager RCBotOwner
		{
			get
			{
				if (_rcBotOwner == null)
				{
					_rcBotOwner = (owner as RCBotDoorManager);
				}
				return _rcBotOwner;
			}
		}

		public RCBotPlayerBlob(DoorManager owner, GameObject player, OnDone onDone)
			: base(owner, player, onDone)
		{
		}

		protected override IEnumerator OnDoorEntered()
		{
			if (player != null)
			{
				yield return new WaitForSeconds(owner.AnimLength);
				if (!(player == null))
				{
					owner.PlayEnterEffects(player);
					if (isLocalPlayer)
					{
						StartSpawn();
					}
					if (player == GameController.GetController().LocalPlayer && AppShell.Instance.Profile != null)
					{
						AppShell.Instance.delayedAchievementEvent(AppShell.Instance.Profile.SelectedCostume, "social_zone", "vv_eyebot", string.Empty, 1f);
					}
				}
			}
			else if (onDone != null)
			{
				onDone(player, CompletionStateEnum.Unknown);
			}
		}

		protected void StartSpawn()
		{
			RCBotOwner.spawner.onSpawnCallback += OnLocalBotSpawned;
			RCBotOwner.spawner.SpawnOnTime(0f);
			RCBotOwner.StartCoroutine(WaitForSpawn());
		}

		protected IEnumerator WaitForSpawn()
		{
			yield return new WaitForSeconds(10f);
			if (!spawned && player != null)
			{
				RCBotOwner.spawner.onSpawnCallback -= OnLocalBotSpawned;
				Finish();
			}
		}

		protected void OnLocalBotSpawned(GameObject rcBotObj)
		{
			spawned = true;
			RCBot component = rcBotObj.GetComponent<RCBot>();
			if (component != null)
			{
				component.TakeControlLocally(player, OnBotDespawned);
				RCBotOwner.StartCoroutine(WaitForTimeout(component));
				if (AppShell.Instance != null && AppShell.Instance.EventMgr != null)
				{
					AppShell.Instance.EventMgr.Fire(rcBotObj, new EntityToRCBotMessage(player, true));
				}
			}
			else
			{
				OnBotDespawned(null);
			}
			if (haveChangedCameras)
			{
				transitionCamera = CameraLiteManager.Instance.GetCurrentCamera();
				CameraLiteManager.Instance.PopCamera(-1f);
			}
		}

		protected IEnumerator WaitForTimeout(RCBot rcBot)
		{
			yield return new WaitForSeconds(RCBotOwner.duration);
			if (rcBot != null)
			{
				rcBot.RelinquishLocalControl(player, OnBotDespawned);
			}
		}

		protected void OnBotDespawned(RCBot rcBot)
		{
			if (haveChangedCameras)
			{
				CameraLiteManager.Instance.PushCamera(transitionCamera, -1f);
			}
			Finish();
			if (AppShell.Instance != null && AppShell.Instance.EventMgr != null)
			{
				AppShell.Instance.EventMgr.Fire(rcBot, new EntityToRCBotMessage(player, false));
			}
		}

		protected void Finish()
		{
			owner.ExitWithPlayer(player, onDone, haveChangedCameras);
			NetworkComponent component = Utils.GetComponent<NetworkComponent>(player);
			NetActionExitDoor action = new NetActionExitDoor(player, owner);
			component.QueueNetAction(action);
		}
	}

	protected const float SPAWN_TIMEOUT_SECONDS = 10f;

	public CharacterSpawn spawner;

	public float duration = 30f;

	public override void OnInteractiveObjectCreated(InteractiveObject owner)
	{
		owner.clickAcceptedForEnable = false;
		owner.highlightOnHover = false;
		owner.highlightOnProximity = false;
		base.OnInteractiveObjectCreated(owner);
	}

	public override void OnExitDoorClosed()
	{
	}

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		if (spawner == null)
		{
			CspUtils.DebugLog("No spawner associated with this RCBotDoorManager instance.  No usage is possible.");
			return false;
		}
		RCBotPlayerBlob rCBotPlayerBlob = new RCBotPlayerBlob(this, player, onDone);
		return rCBotPlayerBlob.StartEnter();
	}
}
