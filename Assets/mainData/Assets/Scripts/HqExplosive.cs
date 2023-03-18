using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Hq/Explosives/Explosive")]
public class HqExplosive : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected const int numObjectsPerFrame = 5;

	public float Radius = 5f;

	public float Power = 10000f;

	public float UpwardsModifier;

	public GameObject Effect;

	protected bool IsExploding;

	protected List<GameObject> objectsToForce;

	protected Vector3 explosionPosition;

	protected HqObject2 hqObj;

	protected bool playedEffect;

	protected GameObject playingSequence;

	protected Renderer[] renderers;

	protected bool unloading;

	private Renderer[] Renderers
	{
		get
		{
			if (renderers == null)
			{
				renderers = Utils.GetComponents<Renderer>(base.gameObject, Utils.SearchChildren, true);
			}
			return renderers;
		}
	}

	protected bool IsEnabled
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

	public void OnUnload()
	{
		unloading = true;
	}

	public void OnDisable()
	{
		if (playedEffect)
		{
			DestroyObject();
		}
	}

	public virtual void Start()
	{
		hqObj = Utils.GetComponent<HqObject2>(base.gameObject);
		if (hqObj == null)
		{
			hqObj = Utils.GetComponent<HqObject2>(base.gameObject, Utils.SearchParents);
		}
		if (!(hqObj == null) && hqObj.State == typeof(HqObject2.HqObjectFlinga))
		{
			playedEffect = false;
		}
	}

	public virtual void Update()
	{
		if (!IsExploding)
		{
			return;
		}
		if (Effect != null && !playedEffect)
		{
			Renderer[] array = Renderers;
			foreach (Renderer renderer in array)
			{
				renderer.enabled = false;
			}
			HqItem component = Utils.GetComponent<HqItem>(base.gameObject, Utils.SearchChildren);
			if (component != null)
			{
				component.IsDestroyed = true;
			}
			playingSequence = (Object.Instantiate(Effect, hqObj.gameObject.transform.position, Quaternion.identity) as GameObject);
			EffectSequence component2 = Utils.GetComponent<EffectSequence>(playingSequence);
			playingSequence.transform.parent = base.gameObject.transform;
			if (component2 != null)
			{
				component2.Initialize(base.gameObject, OnExplosionDone, null);
				component2.StartSequence();
			}
			playedEffect = true;
		}
		int j;
		for (j = 0; j < 5 && j < objectsToForce.Count; j++)
		{
			GameObject gameObject = objectsToForce[j];
			if (!(gameObject != null))
			{
				continue;
			}
			AIControllerHQ component3 = Utils.GetComponent<AIControllerHQ>(gameObject);
			if (component3 != null)
			{
				component3.ReactToExplosion(explosionPosition, Power);
				continue;
			}
			Rigidbody rigidbody = gameObject.rigidbody;
			if (rigidbody == null)
			{
				rigidbody = HqController2.Instance.GetRigidbody(gameObject);
			}
			if (rigidbody != null)
			{
				rigidbody.AddExplosionForce(Power, explosionPosition, Radius, UpwardsModifier);
			}
		}
		if (j == 5 && objectsToForce.Count > j)
		{
			objectsToForce.RemoveRange(0, j);
			return;
		}
		objectsToForce.Clear();
		IsExploding = false;
	}

	protected void OnExplosionDone(EffectSequence seq)
	{
		DestroyObject();
	}

	private void DestroyObject()
	{
		if (!(hqObj != null))
		{
			return;
		}
		HqItem item = HqController2.Instance.ActiveRoom.GetItem(hqObj.PlacedId);
		if (item != null)
		{
			HqController2.Instance.ActiveRoom.DelItem(hqObj.gameObject);
			PlacedItem placedItem = item as PlacedItem;
			if (placedItem != null)
			{
				placedItem.Consume();
			}
		}
		if (!unloading)
		{
			Object.Destroy(hqObj.gameObject);
		}
	}

	protected bool IsValidGameObject(GameObject go)
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
		HqObject2 component = Utils.GetComponent<HqObject2>(go, Utils.SearchParents);
		if (component != null)
		{
			return true;
		}
		AIControllerHQ component2 = Utils.GetComponent<AIControllerHQ>(go);
		if (component2 != null)
		{
			return true;
		}
		CspUtils.DebugLog(go.name + " is not a valid object.");
		return false;
	}

	public int CompareGameObjectsByDistance(GameObject obj1, GameObject obj2)
	{
		float sqrMagnitude = (obj1.transform.position - base.gameObject.transform.position).sqrMagnitude;
		float sqrMagnitude2 = (obj2.transform.position - base.gameObject.transform.position).sqrMagnitude;
		if (sqrMagnitude > sqrMagnitude2)
		{
			return -1;
		}
		if (sqrMagnitude2 > sqrMagnitude)
		{
			return 1;
		}
		return 0;
	}

	public void Explode()
	{
		if (IsExploding)
		{
			return;
		}
		explosionPosition = base.transform.position;
		Collider[] array = Physics.OverlapSphere(explosionPosition, Radius);
		objectsToForce = new List<GameObject>();
		Collider[] array2 = array;
		foreach (Collider collider in array2)
		{
			if (!(collider == null))
			{
				GameObject gameObject = collider.gameObject;
				if (IsValidGameObject(gameObject))
				{
					objectsToForce.Add(gameObject);
				}
			}
		}
		objectsToForce.Sort(CompareGameObjectsByDistance);
		IsExploding = true;
	}
}
