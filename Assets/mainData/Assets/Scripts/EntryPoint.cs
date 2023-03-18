using System.Collections.Generic;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public class Entry
	{
		public float lastUpdate;
	}

	protected const float timeWindow = 0.15f;

	protected bool claimedByAI;

	public Dictionary<GameObject, Entry> collidingObjects;

	protected List<GameObject> deleted;

	public GameObject[] CollidingObjects
	{
		get
		{
			return new List<GameObject>(collidingObjects.Keys).ToArray();
		}
	}

	public bool IsBlocked
	{
		get
		{
			if (collidingObjects == null || collidingObjects.Keys.Count == 0)
			{
				return false;
			}
			return true;
		}
	}

	public Vector3 FacingDirection
	{
		get
		{
			return base.transform.forward;
		}
		set
		{
			base.transform.rotation = Quaternion.LookRotation(value);
		}
	}

	public void Start()
	{
		collidingObjects = new Dictionary<GameObject, Entry>();
		deleted = new List<GameObject>();
		Transform transform = base.gameObject.transform;
		Vector3 position = base.gameObject.transform.position;
		float x = position.x;
		Vector3 position2 = base.gameObject.transform.position;
		float y = position2.y + PathNodeBase.StepHeight;
		Vector3 position3 = base.gameObject.transform.position;
		transform.position = new Vector3(x, y, position3.z);
	}

	protected virtual void AddNewEntry(out Entry data, GameObject obj)
	{
		bool flag = obj != base.gameObject && obj.transform.parent != base.gameObject.transform.parent;
		AIControllerHQ component = Utils.GetComponent<AIControllerHQ>(obj);
		if (component == null && obj.transform.parent != null)
		{
			component = Utils.GetComponent<AIControllerHQ>(obj.transform.parent.gameObject);
		}
		if (component == null)
		{
			data = new Entry();
			data.lastUpdate = Time.time;
			collidingObjects[obj] = data;
		}
		else
		{
			data = null;
		}
	}

	protected virtual void RemoveEntry(GameObject go)
	{
		if (collidingObjects.ContainsKey(go))
		{
			collidingObjects.Remove(go);
		}
	}

	public void OnTriggerStay(Collider c)
	{
		GameObject gameObject = c.gameObject;
		Entry value;
		if (collidingObjects.TryGetValue(gameObject, out value))
		{
			value.lastUpdate = Time.time;
		}
		else
		{
			AddNewEntry(out value, gameObject);
		}
	}

	public void OnTriggerExit(Collider otherCollider)
	{
		if (collidingObjects.ContainsKey(otherCollider.gameObject))
		{
			collidingObjects.Remove(otherCollider.gameObject);
		}
	}

	public virtual void Update()
	{
		foreach (KeyValuePair<GameObject, Entry> collidingObject in collidingObjects)
		{
			GameObject key = collidingObject.Key;
			if (Time.time - collidingObject.Value.lastUpdate >= 0.15f)
			{
				deleted.Add(key);
			}
		}
		foreach (GameObject item in deleted)
		{
			RemoveEntry(item);
		}
		deleted.Clear();
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(base.transform.position, new Vector3(0.25f, 0.25f, 0.25f));
		Gizmos.color = Color.red;
		Gizmos.DrawRay(base.transform.position, FacingDirection);
	}
}
