using System.Collections;
using UnityEngine;

public class StrengthActionPickUpAndSpin : HotspotAction
{
	public GameObject objectToPickUp;

	public string spinAnimationName;

	public GameObject highlightState;

	public float pickupOffset;

	public float lootDropCoolDown = 60f;

	public string postActionEmote = "pose";

	protected GameObject copyOfObjectToPickup;

	protected bool isHighlightOnHover;

	protected bool isHighlightOnProximity;

	protected GameObject user;

	protected float nextLootDropTime;

	private Vector3 pickupLocalPosition;

	private OnFinished onFinished;

	public void Start()
	{
		user = null;
		AppShell.Instance.EventMgr.AddListener<EntityDespawnMessage>(OnCharacterDespawned);
		AppShell.Instance.EventMgr.AddListener<RoomUserLeaveMessage>(OnUserLeaveRoom);
	}

	public void OnDisable()
	{
		user = null;
		AppShell.Instance.EventMgr.RemoveListener<EntityDespawnMessage>(OnCharacterDespawned);
		AppShell.Instance.EventMgr.RemoveListener<RoomUserLeaveMessage>(OnUserLeaveRoom);
	}

	public override void PerformAction(CharacterGlobals player, OnFinished onFinished)
	{
		this.onFinished = onFinished;
		user = player.gameObject;
		if (objectToPickUp.transform.parent != null)
		{
			InteractiveObject component = Utils.GetComponent<InteractiveObject>(objectToPickUp.transform.parent.gameObject);
			if (component != null)
			{
				isHighlightOnHover = component.highlightOnHover;
				isHighlightOnProximity = component.highlightOnProximity;
				component.highlightOnHover = false;
				component.highlightOnProximity = false;
				component.GotoBestState();
				if (player.gameObject == GameController.GetController().LocalPlayer)
				{
					AppShell.Instance.EventMgr.Fire(this, new InteractiveObjectUsedMessage(component));
				}
			}
		}
		copyOfObjectToPickup = (Object.Instantiate(objectToPickUp, objectToPickUp.transform.position, objectToPickUp.transform.rotation) as GameObject);
		Utils.ActivateTree(objectToPickUp, false);
		CharacterMotionController motionController = player.motionController;
		if (motionController != null)
		{
			motionController.DisableNetUpdates(true);
		}
		if (highlightState != null)
		{
			Utils.ActivateTree(highlightState, false);
		}
		BehaviorTurnTo behaviorTurnTo = player.behaviorManager.requestChangeBehavior<BehaviorTurnTo>(true);
		if (behaviorTurnTo != null)
		{
			behaviorTurnTo.Initialize(copyOfObjectToPickup.transform.position, delegate
			{
				PerformPickup(player);
			});
		}
	}

	protected void OnCharacterDespawned(EntityDespawnMessage e)
	{
		if (user != null && e.go == user)
		{
			SwapBackOriginalObject(null);
			user = null;
		}
	}

	protected void OnUserLeaveRoom(RoomUserLeaveMessage e)
	{
		GameObject gameObjectFromNetId = AppShell.Instance.ServerConnection.Game.GetGameObjectFromNetId(new GoNetId(GoNetId.PLAYER_ID_FLAG, e.userId));
		if (user != null && gameObjectFromNetId == user)
		{
			SwapBackOriginalObject(null);
			user = null;
		}
	}

	protected void PerformPickup(CharacterGlobals player)
	{
		BehaviorPickup behaviorPickup = player.behaviorManager.requestChangeBehavior<BehaviorPickup>(false);
		if (behaviorPickup != null)
		{
			behaviorPickup.Initialize(copyOfObjectToPickup, pickupOffset, OnObjectPickedUp);
			StartCoroutine(GetLocalStartPosition(behaviorPickup));
		}
	}

	protected IEnumerator GetLocalStartPosition(BehaviorPickup pickup)
	{
		while (pickup != null && !pickup.ObjectAttached)
		{
			yield return 0;
		}
		if (pickup != null)
		{
			pickupLocalPosition = pickup.LocalStartPosition;
		}
		else
		{
			pickupLocalPosition = Vector3.zero;
		}
	}

	protected void OnObjectPickedUp(CharacterGlobals player, GameObject pickedUpObject)
	{
		player.animationComponent.Play("pickup");
		player.animationComponent["pickup"].wrapMode = WrapMode.ClampForever;
		player.animationComponent["pickup"].time = player.animationComponent["pickup"].length;
		player.behaviorManager.requestChangeBehavior<BehaviorWait>(false);
		if (player != null)
		{
			player.behaviorManager.endBehavior();
			BehaviorPutDown behaviorPutDown = player.behaviorManager.requestChangeBehavior<BehaviorPutDown>(false);
			if (behaviorPutDown != null)
			{
				behaviorPutDown.Initialize(copyOfObjectToPickup, pickupLocalPosition, OnObjectPutDown);
			}
		}
	}

	protected IEnumerator Spin(CharacterGlobals player)
	{
		Animation animComp = copyOfObjectToPickup.animation;
		animComp.Play(spinAnimationName);
		yield return new WaitForSeconds(animComp[spinAnimationName].length);
		bool puttingDown = false;
		if (player != null)
		{
			player.behaviorManager.endBehavior();
			BehaviorPutDown putdown = player.behaviorManager.requestChangeBehavior<BehaviorPutDown>(false);
			if (putdown != null)
			{
				putdown.Initialize(copyOfObjectToPickup, pickupLocalPosition, OnObjectPutDown);
				puttingDown = true;
			}
		}
		if (!puttingDown)
		{
			SwapBackOriginalObject(player);
		}
	}

	protected void OnObjectPutDown(CharacterGlobals player, GameObject putDownObject)
	{
		SwapBackOriginalObject(player);
		if (highlightState != null)
		{
			Utils.ActivateTree(highlightState, true);
		}
		if (player != null)
		{
			CharacterMotionController motionController = player.motionController;
			if (motionController != null)
			{
				motionController.DisableNetUpdates(false);
			}
		}
		PlayEmote(player);
	}

	private void SwapBackOriginalObject(CharacterGlobals player)
	{
		if (copyOfObjectToPickup != null)
		{
			Object.Destroy(copyOfObjectToPickup);
			copyOfObjectToPickup = null;
		}
		if (objectToPickUp != null)
		{
			if (objectToPickUp.transform.parent != null)
			{
				InteractiveObject component = Utils.GetComponent<InteractiveObject>(objectToPickUp.transform.parent.gameObject);
				if (component != null)
				{
					component.highlightOnHover = isHighlightOnHover;
					component.highlightOnProximity = isHighlightOnProximity;
					component.GotoBestState();
				}
			}
			Utils.ActivateTree(objectToPickUp, true);
		}
		if (onFinished != null)
		{
			DropLoot(player);
			onFinished();
		}
	}

	protected void DropLoot(CharacterGlobals player)
	{
		if (player != null && player.networkComponent.IsOwner() && !(Time.time < nextLootDropTime))
		{
			nextLootDropTime = Time.time + lootDropCoolDown;
			CoinsTriggerAdaptor component = Utils.GetComponent<CoinsTriggerAdaptor>(this, Utils.SearchChildren);
			if (component != null)
			{
				component.Triggered();
			}
		}
	}

	protected void PlayEmote(CharacterGlobals player)
	{
		if (player != null && !string.IsNullOrEmpty(postActionEmote))
		{
			BehaviorEmote behaviorEmote = player.behaviorManager.requestChangeBehavior<BehaviorEmote>(false);
			behaviorEmote.Initialize(EmotesDefinition.Instance.GetEmoteByCommand(postActionEmote).id, true, null);
		}
	}
}
