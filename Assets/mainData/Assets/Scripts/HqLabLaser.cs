using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Hq/Trigger/Lab Laser")]
public class HqLabLaser : HqTriggerArea
{
	protected struct LaserEffect
	{
		public GameObject effectObject;

		public float startTime;
	}

	public GameObject ParticleEffectPrefab;

	public float EffectDuration = 10f;

	protected Dictionary<GameObject, LaserEffect> objectsAffected = new Dictionary<GameObject, LaserEffect>();

	protected List<GameObject> objectsToStopAffecting = new List<GameObject>();

	public override void Start()
	{
		base.Start();
		isOn = false;
	}

	public override void OnDisable()
	{
		base.OnDisable();
		if (!unloading)
		{
			foreach (LaserEffect value in objectsAffected.Values)
			{
				Object.Destroy(value.effectObject);
			}
		}
		objectsAffected.Clear();
		objects.Clear();
	}

	protected override GameObject GetRelevantGameObject(GameObject go)
	{
		AIControllerHQ component = Utils.GetComponent<AIControllerHQ>(go);
		if (component != null)
		{
			return go;
		}
		HqObject2 component2 = Utils.GetComponent<HqObject2>(go, Utils.SearchParents);
		if (component2 != null)
		{
			return go;
		}
		hqObj = Utils.GetComponent<HqObject2>(go, Utils.SearchChildren);
		if (hqObj != null)
		{
			return go;
		}
		return null;
	}

	private void ApplyLaserEffect(GameObject go)
	{
		GameObject gameObject = Object.Instantiate(ParticleEffectPrefab, go.transform.position, Quaternion.identity) as GameObject;
		gameObject.transform.parent = go.transform;
		LaserEffect value = default(LaserEffect);
		value.effectObject = gameObject;
		value.startTime = Time.time;
		objectsAffected[go] = value;
	}

	public override void Update()
	{
		if (!HqController2.Instance.IsInPlayMode())
		{
			foreach (LaserEffect value4 in objectsAffected.Values)
			{
				Object.Destroy(value4.effectObject);
			}
			objectsAffected.Clear();
			objects.Clear();
			return;
		}
		foreach (KeyValuePair<GameObject, LaserEffect> item in objectsAffected)
		{
			float time = Time.time;
			LaserEffect value = item.Value;
			if (time - value.startTime > EffectDuration)
			{
				objectsToStopAffecting.Add(item.Key);
				LaserEffect value2 = item.Value;
				Object.Destroy(value2.effectObject);
			}
		}
		foreach (GameObject item2 in objectsToStopAffecting)
		{
			objectsAffected.Remove(item2);
		}
		objectsToStopAffecting.Clear();
		base.Update();
		if (HqController2.Instance.IsInPlayMode())
		{
			foreach (KeyValuePair<GameObject, Entry> @object in objects)
			{
				GameObject key = @object.Key;
				if (!objectsAffected.ContainsKey(key))
				{
					ApplyLaserEffect(key);
				}
				else
				{
					LaserEffect value3 = objectsAffected[key];
					value3.startTime = Time.time;
					objectsAffected[key] = value3;
				}
			}
		}
	}

	protected override bool IsValidGameObject(GameObject go)
	{
		if (go == null || go == base.gameObject)
		{
			if (isOn)
			{
				CspUtils.DebugLog(go.name + " is not a valid game object");
			}
			return false;
		}
		for (int i = 0; i < go.transform.childCount; i++)
		{
			if (go.transform.GetChild(i).gameObject == base.gameObject)
			{
				if (isOn)
				{
					CspUtils.DebugLog(go.name + " is not a valid game object");
				}
				return false;
			}
		}
		HqObject2 component = Utils.GetComponent<HqObject2>(go, Utils.SearchParents);
		if (component != null)
		{
			return true;
		}
		component = Utils.GetComponent<HqObject2>(go, Utils.SearchChildren);
		if (component != null)
		{
			return true;
		}
		AIControllerHQ component2 = Utils.GetComponent<AIControllerHQ>(go);
		if (component2 != null)
		{
			return true;
		}
		return false;
	}
}
