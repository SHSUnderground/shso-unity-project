using System;
using System.Collections;
using UnityEngine;

public class ArcadeDoorManager : DoorManager
{
	public class ArcadePlayerBlob : PlayerBlob
	{
		public ArcadePlayerBlob(DoorManager owner, GameObject player, OnDone onDone)
			: base(owner, player, onDone)
		{
		}

		protected override IEnumerator OnDoorEntered()
		{
			if (player != null)
			{
				yield return new WaitForSeconds(owner.AnimLength);
				ArcadeDoorManager arcadeOwner = owner as ArcadeDoorManager;
				arcadeOwner.controller.highlightOnHover = true;
				arcadeOwner.controller.highlightOnProximity = true;
				arcadeOwner.controller.GotoBestState();
				if (!(player == null))
				{
					owner.PlayEnterEffects(player);
					if (isLocalPlayer)
					{
						AppShell.Instance.SharedHashTable["SocialSpaceSpawnPoint"] = arcadeOwner.createdSpawnPoint.group;
						PlayerOcclusionDetector.OcclusionDetectionEnabled = false;
						LauncherSequences.LaunchArcade((Action)(object)(Action)delegate
						{
							owner.StartCoroutine(DelayedExit());
						}, null);
					}
				}
			}
			else if (onDone != null)
			{
				onDone(player, CompletionStateEnum.Unknown);
			}
		}

		protected IEnumerator DelayedExit()
		{
			yield return 0;
			owner.ExitWithPlayer(player, OnExitFinished, haveChangedCameras);
			NetworkComponent net = Utils.GetComponent<NetworkComponent>(player);
			NetActionExitDoor action = new NetActionExitDoor(player, owner);
			net.QueueNetAction(action);
		}

		public void OnExitFinished(GameObject player, CompletionStateEnum completionState)
		{
			if (isLocalPlayer)
			{
				AppShell.Instance.SharedHashTable["SocialSpaceSpawnPoint"] = null;
				PlayerOcclusionDetector.OcclusionDetectionEnabled = true;
			}
			if (onDone != null)
			{
				onDone(player, completionState);
			}
		}

		protected override void PlayEnterVO(GameObject player)
		{
		}
	}

	protected SpawnPoint createdSpawnPoint;

	protected Vector3 spawnPointOffset = new Vector3(0f, 3.5f, 0f);

	public string SpawnPointGroupName
	{
		get
		{
			return "returnspawn_" + base.gameObject.name;
		}
	}

	public override void Start()
	{
		base.Start();
		CreateSpawnPoint();
	}

	protected void CreateSpawnPoint()
	{
		GameObject gameObject = new GameObject("ReturnSpawnPoint");
		Utils.AttachGameObject(base.gameObject, gameObject);
		gameObject.transform.position = insideDockPoint.transform.position + spawnPointOffset;
		gameObject.transform.Rotate(90f, 0f, 0f);
		createdSpawnPoint = gameObject.AddComponent<SpawnPoint>();
		createdSpawnPoint.group = SpawnPointGroupName;
		createdSpawnPoint.cameraOverride = cameraOverride;
		createdSpawnPoint.keepCameraOnStack = true;
		gameObject.AddComponent<ExitDoorOnSpawn>().door = this;
	}

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		if (Utils.IsLocalPlayer(player) && !LauncherSequences.CanLaunchArcade())
		{
			return false;
		}
		ArcadePlayerBlob arcadePlayerBlob = new ArcadePlayerBlob(this, player, onDone);
		return arcadePlayerBlob.StartEnter();
	}
}
