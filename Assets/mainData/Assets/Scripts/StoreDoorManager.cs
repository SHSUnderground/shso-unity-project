using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

public class StoreDoorManager : DoorManager
{
	public class StorePlayerBlob : PlayerBlob
	{
		public StorePlayerBlob(DoorManager owner, GameObject player, OnDone onDone)
			: base(owner, player, onDone)
		{
		}

		protected override void PlayEnterVO(GameObject player)
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
						ShoppingWindow shopping = new ShoppingWindow();
						shopping.launch();
						shopping.doorCloseDelegate = OnShopClosed;
					}
				}
			}
			else
			{
				onDone(player, CompletionStateEnum.Unknown);
			}
		}

		private void OnShopClosed(string Id, GUIDialogWindow.DialogState state)
		{
			if (player != null && GUIManager.Instance.CurrentState != GUIManager.ModalStateEnum.Transition)
			{
				owner.ExitWithPlayer(player, onDone, haveChangedCameras);
				NetworkComponent component = Utils.GetComponent<NetworkComponent>(player);
				NetActionExitDoor action = new NetActionExitDoor(player, owner);
				component.QueueNetAction(action);
				player = null;
				onDone = null;
				owner = null;
			}
		}
	}

	public new void Start()
	{
		base.Start();
		controller.highlightOnHover = true;
		controller.clickAcceptedForEnable = true;
	}

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		if (Utils.IsLocalPlayer(player) && !LauncherSequences.DependencyCheck(AssetBundleLoader.BundleGroup.Characters))
		{
			PleaseWait();
			return false;
		}
		StorePlayerBlob storePlayerBlob = new StorePlayerBlob(this, player, onDone);
		return storePlayerBlob.StartEnter();
	}

	protected void PleaseWait()
	{
		if (!SHSStagedDownloadWindow.DownloadStatusCurrentlyShowing)
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			Type type = executingAssembly.GetType("SHSStagedDownloadWindow");
			GUIWindow gUIWindow = (GUIWindow)Activator.CreateInstance(type);
			GUIManager.Instance.UIRoots[GUIManager.UILayer.System].Add(gUIWindow);
			gUIWindow.Show(GUIControl.ModalLevelEnum.Default);
		}
	}

	public new void ExitWithPlayer(GameObject player, OnDone onDone, bool resetCamera)
	{
		StorePlayerBlob storePlayerBlob = new StorePlayerBlob(this, player, onDone);
		storePlayerBlob.StartExit(resetCamera);
	}
}
