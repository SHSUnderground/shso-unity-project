using System.Collections.Generic;
using UnityEngine;

public class HotSpotManager : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public bool usable = true;

	protected GameObject activeRoot;

	protected GameObject inactiveRoot;

	protected HotSpotController[] nodes;

	protected List<GameObject> collidingObjects;

	public void Start()
	{
		nodes = Utils.GetComponents<HotSpotController>(this, Utils.SearchChildren);
		foreach (Transform item in base.transform)
		{
			if (item.gameObject.name == "OnActive")
			{
				activeRoot = item.gameObject;
			}
			else if (item.gameObject.name == "OnInactive")
			{
				inactiveRoot = item.gameObject;
			}
		}
		collidingObjects = new List<GameObject>(2);
		if (usable)
		{
			DeactiveHotSpot();
		}
		else
		{
			HideHotSpot();
		}
	}

	public void OnTriggerEnter(Collider other)
	{
		if (!usable)
		{
			return;
		}
		PlayerInputController component = Utils.GetComponent<PlayerInputController>(other, Utils.SearchChildren);
		if (!(component == null) && CanPlayerUse(other.gameObject))
		{
			AddPlayerToColliding(other.gameObject);
			if (collidingObjects.Count >= 1)
			{
				ActivateHotSpot();
				component.ForceMouseRollverUpdate();
			}
		}
	}

	public void OnTriggerExit(Collider other)
	{
		if (usable)
		{
			RemovePlayerFromColliding(other.gameObject);
			if (collidingObjects.Count <= 0)
			{
				DeactiveHotSpot();
			}
		}
	}

	public void OnMouseRolloverEnter(object data)
	{
		if (activeRoot.active)
		{
			MouseRollover mouseRollover = data as MouseRollover;
			if (mouseRollover.allowUserInput)
			{
				GUIManager.Instance.CursorManager.SetCursorType(GUICursorManager.CursorType.Interactable);
			}
		}
	}

	public void OnMouseRolloverExit()
	{
	}

	public bool OnMouseClick(GameObject clicker)
	{
		if (!usable)
		{
			return false;
		}
		if (!activeRoot.active)
		{
			return false;
		}
		List<HotSpotController> list = new List<HotSpotController>(1);
		HotSpotController[] array = nodes;
		foreach (HotSpotController hotSpotController in array)
		{
			if (hotSpotController.CanPlayerUse(clicker))
			{
				list.Add(hotSpotController);
			}
		}
		if (list.Count <= 0)
		{
			return false;
		}
		HotSpotController hotSpotController2 = list[Random.Range(0, list.Count)];
		if (!hotSpotController2.StartWithPlayer(clicker))
		{
			return false;
		}
		NetworkComponent component = Utils.GetComponent<NetworkComponent>(clicker);
		NetActionHotSpotController action = new NetActionHotSpotController(clicker, hotSpotController2);
		component.QueueNetAction(action);
		return true;
	}

	public void SetUsable()
	{
		usable = true;
		DeactiveHotSpot();
	}

	public void SetUnUsable()
	{
		usable = false;
		HideHotSpot();
	}

	protected void ActivateHotSpot()
	{
		activeRoot.SetActiveRecursively(true);
		inactiveRoot.SetActiveRecursively(false);
	}

	protected void DeactiveHotSpot()
	{
		activeRoot.SetActiveRecursively(false);
		inactiveRoot.SetActiveRecursively(true);
	}

	protected void HideHotSpot()
	{
		activeRoot.SetActiveRecursively(false);
		inactiveRoot.SetActiveRecursively(false);
	}

	protected bool CanPlayerUse(GameObject p)
	{
		HotSpotController[] array = nodes;
		foreach (HotSpotController hotSpotController in array)
		{
			if (hotSpotController.CanPlayerUse(p))
			{
				return true;
			}
		}
		return false;
	}

	protected void AddPlayerToColliding(GameObject p)
	{
		if (!collidingObjects.Exists(delegate(GameObject a)
		{
			return a == p;
		}))
		{
			collidingObjects.Add(p);
		}
	}

	protected void RemovePlayerFromColliding(GameObject p)
	{
		collidingObjects.RemoveAll(delegate(GameObject a)
		{
			return a == p;
		});
	}
}
