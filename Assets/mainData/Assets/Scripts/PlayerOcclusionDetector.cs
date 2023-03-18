using System.Collections.Generic;
using UnityEngine;

public class PlayerOcclusionDetector : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public static PlayerOcclusionDetector Instance;

	public static bool OcclusionDetectionEnabled = true;

	public Vector3 offset = new Vector3(0f, 1f, 0f);

	public GameObject myPlayer;

	public Camera myCamera;

	public CameraLiteManager myCameraMgr;

	public BoxCollider myCollider;

	public Vector3 ray;

	public bool tempDisable;

	protected Dictionary<GameObject, GameObject> collidingObjects = new Dictionary<GameObject, GameObject>();

	public void OnEnable()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			CspUtils.DebugLog("There should only be one PlayerOcclusionDetector");
		}
	}

	public void OnDisable()
	{
		Instance = null;
	}

	public void Start()
	{
		if (myPlayer == null)
		{
			myPlayer = base.transform.parent.gameObject;
		}
		if (myPlayer == null)
		{
			CspUtils.DebugLog("Could not find player");
		}
		if (myCameraMgr == null)
		{
			myCameraMgr = CameraLiteManager.Instance;
		}
		if (myCameraMgr == null)
		{
			CspUtils.DebugLog("Could not find camera");
		}
		if (myCamera == null)
		{
			myCamera = Camera.main;
		}
		if (myCamera == null)
		{
			CspUtils.DebugLog("Could not find camera");
		}
		if (myCollider == null)
		{
			myCollider = (GetComponent(typeof(BoxCollider)) as BoxCollider);
		}
		if (myCollider == null)
		{
			CspUtils.DebugLog("Could not find collider");
		}
		Component[] componentsInChildren = myPlayer.GetComponentsInChildren(typeof(Collider));
		Component[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Collider collider = (Collider)array[i];
			if (myCollider != collider)
			{
				Physics.IgnoreCollision(myCollider, collider);
			}
		}
		collidingObjects.Clear();
		base.gameObject.tag = "POD";
		base.gameObject.layer = 2;
		base.transform.parent = myPlayer.transform.parent;
	}

	public void LateUpdate()
	{
		//CspUtils.DebugLog("PlayerOcclusionDetector myCameraMgr=" + myCameraMgr);
		//if (myCameraMgr.CameraOverride != null)
		//	CspUtils.DebugLog("PlayerOcclusionDetector myCameraMgr.CameraOverride=" + myCameraMgr.CameraOverride);
		//CspUtils.DebugLog("PlayerOcclusionDetector myPlayer=" + myPlayer);
		if ((myCameraMgr == null || myCameraMgr.CameraOverride == null) && myPlayer != null)
		{
			//CspUtils.DebugLog("PlayerOcclusionDetector myCamera.transform.position=" + myCamera.transform.position);
			ray = myCamera.transform.position - myPlayer.transform.position - offset;
			//CspUtils.DebugLog("PlayerOcclusionDetector ray=" + ray);
			myCollider.size = new Vector3(1f, 1f, ray.magnitude);
			base.transform.position = myPlayer.transform.position + ray * 0.5f + offset;
			//CspUtils.DebugLog("PlayerOcclusionDetector base.transform.position=" + base.transform.position);
			base.transform.LookAt(myCamera.transform);

		}
	}

	public void OnTriggerEnter(Collider hit)
	{
		if (!collidingObjects.ContainsKey(hit.gameObject))
		{
			collidingObjects.Add(hit.gameObject, hit.gameObject);
		}
	}

	public void OnTriggerExit(Collider hit)
	{
		collidingObjects.Remove(hit.gameObject);
	}

	public IEnumerable<GameObject> GetCollidingObjects()
	{
		if (!(myCamera == null) && OcclusionDetectionEnabled && !tempDisable)
		{
			SeeThrough fade = myCamera.GetComponent(typeof(SeeThrough)) as SeeThrough;
			int layerMask = (int)fade.CullingMask | (1 << fade.FadedLayer);
			List<GameObject> toDelete = new List<GameObject>(1);
			foreach (KeyValuePair<GameObject, GameObject> kvp in collidingObjects)
			{
				GameObject go2 = kvp.Key;
				if (go2 != null)
				{
					if (((1 << go2.layer) & layerMask) != 0)
					{
						yield return kvp.Key;
					}
				}
				else
				{
					toDelete.Add(go2);
				}
			}
			foreach (GameObject go in toDelete)
			{
				collidingObjects.Remove(go);
			}
		}
	}
}
