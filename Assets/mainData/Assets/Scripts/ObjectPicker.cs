using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ObjectPicker
{
	public class PickResult
	{
		public enum ResultType
		{
			Player,
			Object
		}

		public GameObject gameObject;

		public ResultType type;

		public PickResult(GameObject gameObject, ResultType type)
		{
			this.gameObject = gameObject;
			this.type = type;
		}
	}

	private const int layerMask = 1077760;

	private GameObject owningPlayer;

	private Ray mouseRay;

	private List<RaycastHit> lastHits;

	private GameObject lastHitObject;

	private List<GameObject> objsToIgnore = new List<GameObject>();

	[CompilerGenerated]
	private GameObject _003CSelectedPlayer_003Ek__BackingField;

	public GameObject SelectedPlayer
	{
		[CompilerGenerated]
		get
		{
			return _003CSelectedPlayer_003Ek__BackingField;
		}
		[CompilerGenerated]
		set
		{
			_003CSelectedPlayer_003Ek__BackingField = value;
		}
	}

	public ObjectPicker(GameObject owningPlayer)
	{
		this.owningPlayer = owningPlayer;
	}

	public GameObject GetFirstObjectUnderCursor(float range)
	{
		if (SHSInput.IsOverUI())
		{
			return null;
		}
		lastHits = null;
		mouseRay = Camera.main.ScreenPointToRay(SHSInput.mousePosition);
		if (range == 0f)
		{
			range = float.PositiveInfinity;
		}
		RaycastHit hitInfo;
		GameObject gameObject = (!Physics.Raycast(mouseRay, out hitInfo, range, 1077760)) ? null : ObjFromRayHit(hitInfo);
		if (gameObject == owningPlayer)
		{
			gameObject = null;
			SortedRaycast();
			foreach (RaycastHit lastHit in lastHits)
			{
				GameObject gameObject2 = ObjFromRayHit(lastHit);
				if (gameObject2 != owningPlayer)
				{
					gameObject = gameObject2;
					break;
				}
			}
		}
		lastHitObject = gameObject;
		return gameObject;
	}

	public PickResult PickPlayerOrObject()
	{
		if (lastHitObject == null)
		{
			objsToIgnore.Clear();
			return null;
		}
		if (!Utils.IsPlayer(lastHitObject))
		{
			objsToIgnore.Clear();
			return new PickResult(lastHitObject, PickResult.ResultType.Object);
		}
		if (!objsToIgnore.Contains(lastHitObject))
		{
			objsToIgnore.Clear();
			objsToIgnore.Add(lastHitObject);
			return new PickResult(lastHitObject, PickResult.ResultType.Player);
		}
		if (lastHits == null)
		{
			SortedRaycast();
		}
		GameObject gameObject = null;
		foreach (RaycastHit lastHit in lastHits)
		{
			GameObject gameObject2 = ObjFromRayHit(lastHit);
			if (gameObject2 != owningPlayer && !objsToIgnore.Contains(gameObject2))
			{
				gameObject = gameObject2;
				break;
			}
		}
		if (gameObject != null)
		{
			if (!Utils.IsPlayer(gameObject))
			{
				objsToIgnore.Clear();
				return new PickResult(gameObject, PickResult.ResultType.Object);
			}
			objsToIgnore.Add(gameObject);
			return new PickResult(gameObject, PickResult.ResultType.Player);
		}
		objsToIgnore.Clear();
		GameObject gameObject3 = null;
		foreach (RaycastHit lastHit2 in lastHits)
		{
			GameObject gameObject4 = ObjFromRayHit(lastHit2);
			if (gameObject4 != owningPlayer && gameObject4 != SelectedPlayer)
			{
				gameObject = gameObject4;
				break;
			}
			if (gameObject3 == null && gameObject4 != owningPlayer)
			{
				gameObject3 = gameObject4;
			}
		}
		if (gameObject != null)
		{
			objsToIgnore.Add(gameObject);
			return new PickResult(gameObject, PickResult.ResultType.Player);
		}
		objsToIgnore.Add(gameObject3);
		return new PickResult(gameObject3, PickResult.ResultType.Player);
	}

	private void SortedRaycast()
	{
		lastHits = new List<RaycastHit>(Physics.RaycastAll(mouseRay, float.PositiveInfinity, 1077760));
		lastHits.Sort(delegate(RaycastHit first, RaycastHit second)
		{
			return (!(first.distance < second.distance)) ? 1 : (-1);
		});
	}

	private GameObject ObjFromRayHit(RaycastHit hit)
	{
		if (hit.collider.gameObject.layer == 14)
		{
			return hit.collider.transform.parent.gameObject;
		}
		return hit.collider.gameObject;
	}
}
