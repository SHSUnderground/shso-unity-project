using System.Collections.Generic;
using UnityEngine;

public class HqAntiGrav : HqTriggerArea
{
	public float modifier = -2f;

	public float rndForce = 2f;

	public float rndTorque = 2f;

	public override void Start()
	{
		base.Start();
		isOn = false;
	}

	protected override GameObject GetRelevantGameObject(GameObject go)
	{
		AIControllerHQ component = Utils.GetComponent<AIControllerHQ>(go);
		if (component != null && component.FlingaObject != null)
		{
			return component.FlingaObject.gameObject;
		}
		HqObject2 component2 = Utils.GetComponent<HqObject2>(go, Utils.SearchParents);
		if (component2 != null)
		{
			return component2.gameObject;
		}
		return null;
	}

	protected override bool IsValidGameObject(GameObject go)
	{
		if (go == null)
		{
			return false;
		}
		if (base.gameObject.collider != null && base.gameObject.collider is CapsuleCollider)
		{
			CapsuleCollider capsuleCollider = base.gameObject.collider as CapsuleCollider;
			Vector3 position = go.transform.position;
			float x = position.x;
			Vector3 position2 = go.transform.position;
			Vector2 a = new Vector2(x, position2.z);
			Vector3 position3 = base.gameObject.transform.position;
			float x2 = position3.x;
			Vector3 position4 = base.gameObject.transform.position;
			Vector2 b = new Vector2(x2, position4.z);
			float magnitude = (a - b).magnitude;
			if (magnitude > capsuleCollider.radius + 1f)
			{
				return false;
			}
		}
		AIControllerHQ x3 = Utils.GetComponent<AIControllerHQ>(go);
		if (x3 == null)
		{
			HqObject2 component = Utils.GetComponent<HqObject2>(go, Utils.SearchChildren);
			if (component != null)
			{
				x3 = component.AIController;
			}
		}
		if (x3 != null)
		{
			return true;
		}
		return base.IsValidGameObject(go);
	}

	protected override void AddNewEntry(out Entry data, GameObject obj)
	{
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

	protected override void RemoveEntry(GameObject go)
	{
		if (go != null)
		{
			HqObject2 component = Utils.GetComponent<HqObject2>(go, Utils.SearchChildren);
			if (component != null && component.AIController != null)
			{
				component.AIController.transform.position = component.transform.position;
			}
		}
		base.RemoveEntry(go);
	}

	protected virtual void ApplyForce(GameObject go)
	{
		if (!isOn)
		{
			return;
		}
		HqObject2[] components = Utils.GetComponents<HqObject2>(go, Utils.SearchChildren, true);
		if (components == null || components.Length < 1)
		{
			return;
		}
		HqObject2 hqObject = components[0];
		if (hqObject.AIController != null && !hqObject.AIController.IsFlingaObjectActive && HqController2.Instance.State == typeof(HqController2.HqControllerFlinga))
		{
			if (!hqObject.AIController.CanPickUp || hqObject != hqObject.AIController.FlingaObject || hqObject.State == typeof(HqObject2.HqObjectFlingaSelected))
			{
				return;
			}
			hqObject.AIController.PickUpAI();
			hqObject.GotoFlingaMode();
			go.rigidbody.isKinematic = false;
			go.rigidbody.useGravity = true;
			go.collider.isTrigger = false;
		}
		if (hqObject.State == typeof(HqObject2.HqObjectFlinga) && !(go.rigidbody == null) && !go.rigidbody.isKinematic && go.rigidbody.useGravity)
		{
			Rigidbody rigidbody = go.rigidbody;
			Vector3 max = base.collider.bounds.max;
			float y = max.y;
			Vector3 position = go.transform.position;
			float num = y - position.y;
			Vector3 max2 = base.collider.bounds.max;
			float y2 = max2.y;
			Vector3 min = base.collider.bounds.min;
			float num2 = num / (y2 - min.y);
			rigidbody.rigidbody.AddForce(rigidbody.mass * modifier * num2 * HqController2.Instance.gravity);
			if (rndForce > 0f)
			{
				Vector3 a = new Vector3(Random.Range(0f - rndForce, rndForce), Random.Range(0f - rndForce, rndForce), Random.Range(0f - rndForce, rndForce));
				rigidbody.AddForce(rigidbody.mass * a);
			}
			if (rndTorque > 0f)
			{
				Vector3 a2 = new Vector3(Random.Range(0f - rndForce, rndForce), Random.Range(0f - rndForce, rndForce), Random.Range(0f - rndForce, rndForce));
				rigidbody.AddTorque(rigidbody.mass * a2);
			}
		}
	}

	public void FixedUpdate()
	{
		foreach (KeyValuePair<GameObject, Entry> @object in objects)
		{
			GameObject key = @object.Key;
			if (key != null && IsValidGameObject(key))
			{
				ApplyForce(key);
			}
		}
	}
}
