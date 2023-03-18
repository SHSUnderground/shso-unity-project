using System.Collections.Generic;
using UnityEngine;

public class SnapParent : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private const float SNAP_THRESHOLD = 1f;

	private static List<Snap> allSnaps = new List<Snap>();

	private bool firstUpdate = true;

	private List<Snap> snapChildren = new List<Snap>();

	private void Reset()
	{
		snapChildren.Clear();
		firstUpdate = true;
	}

	private void OnDrawGizmos()
	{
		if (firstUpdate)
		{
			initializeSnapParent();
			firstUpdate = false;
		}
	}

	private Transform GetTileRoot(GameObject o)
	{
		Transform parent = o.transform.parent;
		while (parent != null && Utils.GetComponent<TileInfo>(parent) == null)
		{
			parent = parent.parent;
		}
		if (parent == null)
		{
			return o.transform.root;
		}
		return parent;
	}

	private void OnDrawGizmosSelected()
	{
		snapToNearestSnapParent();
	}

	private void snapToNearestSnapParent()
	{
		bool flag = false;
		Snap snap = null;
		Snap snap2 = null;
		float num = float.MaxValue;
		List<Snap> list = findAllSnaps();
		if (list == null)
		{
			return;
		}
		foreach (Snap item in list)
		{
			if (item == null)
			{
				flag = true;
			}
			else if (!(GetTileRoot(item.gameObject) == GetTileRoot(base.gameObject)))
			{
				foreach (Snap snapChild in snapChildren)
				{
					float num2 = Vector3.Distance(snapChild.transform.position, item.transform.position);
					if (num2 < num)
					{
						snap2 = snapChild;
						snap = item;
						num = num2;
					}
				}
			}
		}
		if (snap == null)
		{
			return;
		}
		if (num <= 1f && !Input.GetKeyDown(KeyCode.LeftShift) && !Input.GetKeyDown(KeyCode.RightShift))
		{
			Vector3 vector = snap.transform.position - snap2.transform.position;
			GetTileRoot(base.gameObject).position += vector;
			snap2.SnappedTo = snap;
			snap.SnappedTo = snap2;
			foreach (Snap snapChild2 in snapChildren)
			{
				if (!(snapChild2 == snap2) && snapChild2.SnappedTo != null)
				{
					snapChild2.SnappedTo.SnappedTo = null;
					snapChild2.SnappedTo = null;
				}
			}
			snapOverlappingSnaps();
			if (!flag)
			{
				return;
			}
			for (int i = 0; i < allSnaps.Count; i++)
			{
				if (allSnaps[i] == null)
				{
					allSnaps.RemoveAt(i);
					i--;
				}
			}
		}
		else
		{
			foreach (Snap snapChild3 in snapChildren)
			{
				if (snapChild3.SnappedTo != null)
				{
					snapChild3.SnappedTo.SnappedTo = null;
					snapChild3.SnappedTo = null;
				}
			}
		}
	}

	private void snapOverlappingSnaps()
	{
		List<Snap> list = findAllSnaps();
		foreach (Snap item in list)
		{
			if (!(item == null) && !(GetTileRoot(item.gameObject) == GetTileRoot(base.gameObject)))
			{
				foreach (Snap snapChild in snapChildren)
				{
					if (Vector3.Distance(snapChild.transform.position, item.transform.position) < 0.0001f)
					{
						item.SnappedTo = snapChild;
						snapChild.SnappedTo = item;
						break;
					}
				}
			}
		}
	}

	private void initializeSnapParent()
	{
		if (snapChildren.Count == 0)
		{
			snapChildren = findChildSnaps();
			foreach (Snap snapChild in snapChildren)
			{
				allSnaps.Add(snapChild);
			}
		}
		snapOverlappingSnaps();
	}

	private List<Snap> findChildSnaps()
	{
		List<Snap> list = new List<Snap>();
		GameObject[] array = GameObject.FindGameObjectsWithTag("SnapAnchor");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (GetTileRoot(gameObject.gameObject) == GetTileRoot(base.gameObject))
			{
				Snap snap = gameObject.GetComponent("Snap") as Snap;
				if (snap != null)
				{
					list.Add(snap);
				}
			}
		}
		return list;
	}

	private List<Snap> findAllSnaps()
	{
		if (allSnaps.Count > 0)
		{
			return allSnaps;
		}
		CspUtils.DebugLog("Lost cached snap list - rebuilding list...");
		GameObject[] array = GameObject.FindGameObjectsWithTag("SnapAnchor");
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			Snap snap = gameObject.GetComponent("Snap") as Snap;
			if (snap != null)
			{
				allSnaps.Add(snap);
			}
		}
		return allSnaps;
	}
}
