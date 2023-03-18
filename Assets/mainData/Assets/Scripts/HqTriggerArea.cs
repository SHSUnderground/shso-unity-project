using System.Collections.Generic;
using UnityEngine;

public class HqTriggerArea : HqTrigger
{
	public class Entry
	{
		public float lastUpdate;

		public float drag;

		public Component hqComp;
	}

	protected const float timeWindow = 0.15f;

	protected Dictionary<GameObject, Entry> objects = new Dictionary<GameObject, Entry>();

	protected List<GameObject> deleted;

	public float dragOverride = 1f;

	protected virtual void AddNewEntry(out Entry data, GameObject obj)
	{
		if (!IsOn)
		{
			data = null;
			return;
		}
		if (!IsValidGameObject(obj))
		{
			data = null;
			return;
		}
		Rigidbody rigidbody = obj.rigidbody;
		data = new Entry();
		data.lastUpdate = Time.time;
		if (rigidbody != null)
		{
			data.drag = rigidbody.drag;
			rigidbody.drag = dragOverride;
		}
		else
		{
			data.drag = 0f;
		}
		data.hqComp = Utils.GetComponent<HqObject2>(obj);
		objects[obj] = data;
		OnHqTriggerEnter(obj);
	}

	protected virtual void RemoveEntry(GameObject go)
	{
		if (go != null && go.rigidbody != null)
		{
			Entry value = null;
			objects.TryGetValue(go, out value);
			if (value != null && go.rigidbody != null)
			{
				go.rigidbody.drag = value.drag;
			}
		}
		objects.Remove(go);
	}

	protected virtual void OnHqTriggerEnter(GameObject go)
	{
	}

	protected virtual GameObject GetRelevantGameObject(GameObject go)
	{
		Rigidbody rigidbody = HqController2.Instance.GetRigidbody(go);
		if (rigidbody == null)
		{
			return null;
		}
		return rigidbody.gameObject;
	}

	protected virtual bool IsValidGameObject(GameObject go)
	{
		if (go == null)
		{
			return false;
		}
		HqObject2 component = Utils.GetComponent<HqObject2>(go);
		if (component == null)
		{
			return false;
		}
		Rigidbody rigidbody = go.rigidbody;
		if (rigidbody == null)
		{
			return false;
		}
		return true;
	}

	public override void Start()
	{
		base.Start();
		objects = new Dictionary<GameObject, Entry>();
		deleted = new List<GameObject>();
	}

	public virtual void OnEnable()
	{
		AppShell.Instance.EventMgr.AddListener<HQRoomRespawnMessage>(OnHqRoomRespawnMessage);
	}

	public virtual void OnDisable()
	{
		AppShell.Instance.EventMgr.RemoveListener<HQRoomRespawnMessage>(OnHqRoomRespawnMessage);
	}

	public void OnTriggerStay(Collider c)
	{
		GameObject relevantGameObject = GetRelevantGameObject(c.gameObject);
		if (!(relevantGameObject == null))
		{
			Entry value;
			if (objects.TryGetValue(relevantGameObject, out value))
			{
				value.lastUpdate = Time.time;
			}
			else
			{
				AddNewEntry(out value, relevantGameObject);
			}
		}
	}

	public void OnTriggerExit(Collider otherCollider)
	{
		if (objects.ContainsKey(otherCollider.gameObject))
		{
			RemoveEntry(otherCollider.gameObject);
		}
	}

	public virtual void Update()
	{
		foreach (KeyValuePair<GameObject, Entry> @object in objects)
		{
			GameObject key = @object.Key;
			if (!IsValidGameObject(key))
			{
				deleted.Add(key);
			}
			else if (Time.time - @object.Value.lastUpdate >= 0.15f)
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

	public override void TurnOff()
	{
		base.TurnOff();
		if (objects != null)
		{
			GameObject[] array = new GameObject[objects.Keys.Count];
			objects.Keys.CopyTo(array, 0);
			for (int num = array.Length - 1; num >= 0; num--)
			{
				RemoveEntry(array[num]);
			}
		}
	}

	protected void OnHqRoomRespawnMessage(HQRoomRespawnMessage msg)
	{
		if (objects.ContainsKey(msg.respawnObj))
		{
			RemoveEntry(msg.respawnObj);
		}
	}
}
