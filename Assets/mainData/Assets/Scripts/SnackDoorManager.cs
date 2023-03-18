using System.Collections;
using UnityEngine;

public class SnackDoorManager : DoorManager
{
	public class SnackPlayerBlob : PlayerBlob
	{
		private GameObject newSnack;

		protected bool doneEating;

		private SnackDoorManager sdm;

		public SnackPlayerBlob(DoorManager owner, GameObject player, OnDone onDone)
			: base(owner, player, onDone)
		{
			sdm = (owner as SnackDoorManager);
		}

		public override void StartExit(bool hasChangedCameras)
		{
			base.StartExit(hasChangedCameras);
		}

		protected override void PlayEnterVO(GameObject player)
		{
			string text = null;
			GameObject[] snackMenu = sdm.snackMenu;
			foreach (GameObject gameObject in snackMenu)
			{
				if (gameObject.name.ToLower().Contains("pie"))
				{
					text = "hi_aunt_may";
					break;
				}
				if (gameObject.name.ToLower().Contains("pizza"))
				{
					text = "pizza";
					break;
				}
			}
			if (text == null)
			{
				base.PlayEnterVO(player);
			}
			else
			{
				VOManager.Instance.PlayVO(text, player);
			}
		}

		protected void AttachSnack()
		{
			newSnack = null;
			if (sdm.snackMenu.Length <= 0)
			{
				return;
			}
			newSnack = (Object.Instantiate(sdm.snackMenu[Random.Range(0, sdm.snackMenu.Length)]) as GameObject);
			BehaviorManager component = player.GetComponent<BehaviorManager>();
			CharacterGlobals characterGlobals = player.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals;
			if (component != null && characterGlobals != null && newSnack != null)
			{
				Transform transform = Utils.FindNodeInChildren(player.transform, characterGlobals.characterController.pickupBone);
				if (transform == null)
				{
					CspUtils.DebugLog("Cannot find attach node in SnackDoorManager.SnackPlayerBlob");
					return;
				}
				CspUtils.DebugLog("attachNode is ok, called " + transform.gameObject.name);
				Utils.AttachGameObject(transform.gameObject, newSnack);
				newSnack.transform.localPosition = default(Vector3);
			}
		}

		protected override void ArrivedExitDoor(GameObject player)
		{
			CharacterController component = Utils.GetComponent<CharacterController>(player);
			foreach (Collider doorCollider in sdm.doorColliders)
			{
				if (doorCollider.gameObject.active)
				{
					Physics.IgnoreCollision(doorCollider, component, false);
				}
			}
			sdm.controller.StartCoroutine(WaitForExitDoorClose());
			TurnFadingOn();
			if (haveChangedCameras)
			{
				CameraLiteManager.Instance.PopCamera(owner.cameraBlendToPlayer);
			}
			sdm.StartCoroutine(ConsumeSnack());
		}

		protected IEnumerator ConsumeSnack()
		{
			if (!(player != null))
			{
				yield break;
			}
			BehaviorManager behaviorManager2 = Utils.GetComponent<BehaviorManager>(player);
			if (behaviorManager2 != null)
			{
				if (behaviorManager2.getBehavior() is BehaviorApproachUninterruptable)
				{
					behaviorManager2.endBehavior();
				}
				behaviorManager2.requestChangeBehavior<BehaviorWait>(false);
			}
			yield return new WaitForSeconds(owner.AnimLength);
			if (behaviorManager2 != null && behaviorManager2.getBehavior() is BehaviorWait)
			{
				behaviorManager2.endBehavior();
			}
			if (player == null)
			{
				yield break;
			}
			AttachSnack();
			doneEating = true;
			if (newSnack != null)
			{
				behaviorManager2 = Utils.GetComponent<BehaviorManager>(player);
				if (behaviorManager2 != null)
				{
					BehaviorEat eatBehavior = behaviorManager2.requestChangeBehavior(typeof(BehaviorEat), false) as BehaviorEat;
					if (eatBehavior != null)
					{
						doneEating = false;
						eatBehavior.Initialize(newSnack, true, OnDoneEating);
					}
				}
			}
			while (!doneEating)
			{
				yield return 0;
			}
			CharacterMotionController mc = Utils.GetComponent<CharacterMotionController>(player);
			if (mc != null)
			{
				mc.DisableNetUpdates(false);
			}
			if (onDone != null)
			{
				onDone(player, CompletionStateEnum.Success);
			}
		}

		protected void OnDoneEating(GameObject obj)
		{
			if (obj != null)
			{
				Object.Destroy(obj);
			}
			doneEating = true;
		}
	}

	public GameObject[] snackMenu;

	public new void Start()
	{
		base.Start();
		controller.highlightOnHover = true;
		controller.clickAcceptedForEnable = true;
	}

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		SnackPlayerBlob snackPlayerBlob = new SnackPlayerBlob(this, player, onDone);
		return snackPlayerBlob.StartEnter();
	}

	public override void ExitWithPlayer(GameObject player, OnDone onDone, bool resetCamera)
	{
		SnackPlayerBlob snackPlayerBlob = new SnackPlayerBlob(this, player, onDone);
		snackPlayerBlob.StartExit(resetCamera);
	}
}
