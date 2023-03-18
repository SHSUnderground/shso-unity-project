using UnityEngine;

public class HqHockeyNet : HqTriggerArea
{
	protected float triggerEndTime;

	public float triggerPeriod = 5f;

	public GameObject triggerableObject;

	private EffectSequence effectSeq;

	private GameObject parentGameObject;

	private EffectSequence EffectSequence
	{
		get
		{
			if (effectSeq == null && triggerableObject != null)
			{
				GameObject go = Object.Instantiate(triggerableObject) as GameObject;
				effectSeq = Utils.GetComponent<EffectSequence>(go, Utils.SearchChildren);
				if (effectSeq == null)
				{
					CspUtils.DebugLog("Effect sequence in prefab is null: " + triggerableObject.name + " Please make sure the item definition is correct.");
				}
				else
				{
					effectSeq.Initialize(parentGameObject, null, null);
				}
			}
			return effectSeq;
		}
	}

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

	public override void Start()
	{
		base.Start();
		triggerEndTime = 0f;
		parentGameObject = null;
		if (base.transform.parent != null)
		{
			parentGameObject = base.transform.parent.gameObject;
		}
	}

	public override void Update()
	{
		base.Update();
		if (Time.time > triggerEndTime && EffectSequence != null)
		{
			EffectSequence.Cancel();
		}
	}

	protected override void OnHqTriggerEnter(GameObject go)
	{
		base.OnHqTriggerEnter(go);
		if (Time.time > triggerEndTime)
		{
			if (EffectSequence != null)
			{
				EffectSequence.StartSequence();
			}
			triggerEndTime = Time.time + triggerPeriod;
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
		return null;
	}

	protected override bool IsValidGameObject(GameObject go)
	{
		if (go == null || go == base.gameObject || go == parentGameObject || isSibling(go))
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
		if (go.transform.parent != null)
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

	protected bool isSibling(GameObject go)
	{
		if (go.transform.parent != null && go.transform.parent.gameObject == parentGameObject)
		{
			return true;
		}
		return false;
	}
}
