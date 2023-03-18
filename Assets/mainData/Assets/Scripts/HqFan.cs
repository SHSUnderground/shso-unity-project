using UnityEngine;

public class HqFan : HqAntiGrav
{
	public float fanForce = 150f;

	public override bool IsOn
	{
		get
		{
			if (hqObj != null)
			{
				return hqObj.State == typeof(HqObject2.HqObjectFlinga);
			}
			return false;
		}
	}

	protected override GameObject GetRelevantGameObject(GameObject go)
	{
		AIControllerHQ component = Utils.GetComponent<AIControllerHQ>(go);
		if (component != null)
		{
			return go;
		}
		HqObject2 component2 = Utils.GetComponent<HqObject2>(go);
		if (component2 != null)
		{
			return go;
		}
		if (go.transform.parent != null && go.transform.parent != base.gameObject.transform.parent)
		{
			component2 = Utils.GetComponent<HqObject2>(go.transform.parent.gameObject);
			if (component2 != null)
			{
				return go.transform.parent.gameObject;
			}
		}
		return null;
	}

	protected override bool IsValidGameObject(GameObject go)
	{
		if (go == null || go == base.gameObject)
		{
			return false;
		}
		for (int i = 0; i < go.transform.childCount; i++)
		{
			if (go.transform.GetChild(i).gameObject == base.gameObject)
			{
				return false;
			}
		}
		HqObject2 component = Utils.GetComponent<HqObject2>(go);
		if (component != null)
		{
			return true;
		}
		if (go.transform.parent != null && go.transform.parent != base.gameObject.transform.parent)
		{
			hqObj = Utils.GetComponent<HqObject2>(go.transform.parent.gameObject);
			if (hqObj != null)
			{
				return true;
			}
		}
		AIControllerHQ component2 = Utils.GetComponent<AIControllerHQ>(go);
		if (component2 != null)
		{
			return true;
		}
		return false;
	}

	protected override void ApplyForce(GameObject go)
	{
		if (go == null || !IsOn)
		{
			return;
		}
		Vector3 vector = base.gameObject.transform.InverseTransformPoint(go.transform.position);
		CapsuleCollider capsuleCollider = base.collider as CapsuleCollider;
		float num = (capsuleCollider.height - vector.z) / capsuleCollider.height;
		if (num <= 0f)
		{
			return;
		}
		Vector3 direction = new Vector3(0f, 0f, vector.z);
		Vector3 position = go.transform.position;
		Vector3 start = position + base.gameObject.transform.TransformDirection(direction) * -1f;
		RaycastHit hitInfo;
		if (Physics.Linecast(start, position, out hitInfo, 4694016) && hitInfo.collider.gameObject != go && (!(hitInfo.collider.gameObject.transform.parent != null) || hitInfo.collider.gameObject.transform.parent.gameObject != go))
		{
			return;
		}
		Rigidbody rigidbody = HqController2.Instance.GetRigidbody(go);
		if (rigidbody != null)
		{
			Vector3 force = rigidbody.mass * num * fanForce * base.gameObject.transform.forward;
			rigidbody.rigidbody.AddForce(force);
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
		else
		{
			CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(go);
			if (component != null)
			{
				Vector3 newVelocity = num * (fanForce * 0.5f) * base.gameObject.transform.forward;
				component.setForcedVelocity(newVelocity, 1f);
			}
		}
	}
}
