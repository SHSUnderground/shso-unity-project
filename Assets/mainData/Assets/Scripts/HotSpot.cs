using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotSpot : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public SplineController spline;

	public string introEffectSequence = string.Empty;

	public CameraLite alternativeCamera;

	public float cameraBlendInTime = 0.5f;

	public float cameraBlendOutTime = 0.5f;

	public float cameraBlendOutStartTime = -1f;

	protected GameObject activeRoot;

	protected GameObject inactiveRoot;

	protected List<GameObject> collidingObjects;

	protected bool bCameraOverriden;

	private GameObject hoverGameObject;

	private void Start()
	{
		foreach (Transform item in base.transform)
		{
			if (item.gameObject.name == "OnActive")
			{
				activeRoot = item.gameObject;
			}
			else if (item.gameObject.name == "OnInactive")
			{
				inactiveRoot = item.gameObject;
			}
		}
		collidingObjects = new List<GameObject>(2);
		bCameraOverriden = false;
		DeactiveHotSpot();
	}

	public void OnTriggerEnter(Collider other)
	{
		CspUtils.DebugLog("OnTriggerEnter .. " + other.name);
		if ((bool)spline)
		{
			if (other.gameObject.GetComponent(typeof(PlayerInputController)) != null && !collidingObjects.Exists(delegate(GameObject a)
			{
				return a == other.gameObject;
			}))
			{
				collidingObjects.Add(other.gameObject);
			}
			if (collidingObjects.Count > 0)
			{
				ActivateHotSpot();
			}
		}
	}

	public void OnTriggerExit(Collider other)
	{
		CspUtils.DebugLog("OnTriggerExit .. " + other.name);
		if ((bool)spline)
		{
			collidingObjects.RemoveAll(delegate(GameObject a)
			{
				return a == other.gameObject;
			});
			if (collidingObjects.Count <= 0)
			{
				DeactiveHotSpot();
			}
		}
	}

	public void OnMouseRolloverEnter(object data)
	{
		MouseRollover mouseRollover = data as MouseRollover;
		if (mouseRollover.allowUserInput)
		{
			hoverGameObject = mouseRollover.character;
		}
		if (collidingObjects.Count > 0)
		{
			ActivateHotSpot();
		}
	}

	public void Update()
	{
		if ((bool)hoverGameObject)
		{
			GUIManager.Instance.CursorManager.SetCursorType((collidingObjects.Count != 0) ? GUICursorManager.CursorType.Interactable : GUICursorManager.CursorType.Uninteractable);
		}
	}

	public void OnMouseRolloverExit()
	{
		hoverGameObject = null;
		if (collidingObjects.Count <= 0)
		{
			DeactiveHotSpot();
		}
	}

	public bool OnMouseClick(GameObject clicker)
	{
		if (UseHotSpot(clicker))
		{
			NetworkComponent component = Utils.GetComponent<NetworkComponent>(clicker);
			NetActionHotSpot action = new NetActionHotSpot(clicker, this);
			component.QueueNetAction(action);
			return true;
		}
		return false;
	}

	public bool UseHotSpot(GameObject player)
	{
		if (collidingObjects.Count <= 0)
		{
			return false;
		}
		BehaviorManager behaviorManager = player.GetComponent(typeof(BehaviorManager)) as BehaviorManager;
		if (behaviorManager == null)
		{
			CspUtils.DebugLog("Player should always have a BehaviorManager");
			return false;
		}
		BehaviorApproach behaviorApproach = behaviorManager.requestChangeBehavior(typeof(BehaviorApproach), false) as BehaviorApproach;
		if (behaviorApproach == null)
		{
			return false;
		}
		Vector3 pos;
		Quaternion rot;
		spline.GetFirstPoint(out pos, out rot);
		behaviorApproach.Initialize(pos, rot, false, ApproachArrived, null, 0f, 2f, true, false);
		return true;
	}

	protected void ApproachArrived(GameObject obj)
	{
		EffectSequence effectSequence = null;
		Vector3 pos;
		Quaternion rot;
		spline.GetFirstPoint(out pos, out rot);
		Vector3 position = obj.transform.position;
		pos.y = position.y;
		obj.transform.position = pos;
		obj.transform.rotation = rot;
		if (introEffectSequence != string.Empty)
		{
			EffectSequenceList effectSequenceList = obj.GetComponent(typeof(EffectSequenceList)) as EffectSequenceList;
			if (effectSequenceList != null)
			{
				effectSequence = effectSequenceList.GetLogicalEffectSequence(introEffectSequence);
			}
			if (effectSequence == null)
			{
				CspUtils.DebugLog("Unable to find Effect Sequence " + introEffectSequence + " on the character");
			}
		}
		bool flag = false;
		SpawnData spawnData = obj.GetComponent(typeof(SpawnData)) as SpawnData;
		if (spawnData != null)
		{
			flag = ((spawnData.spawnType & CharacterSpawn.Type.Local) != 0);
		}
		bCameraOverriden = false;
		if (alternativeCamera != null && flag)
		{
			CameraTarget cameraTarget = alternativeCamera.GetComponent(typeof(CameraTarget)) as CameraTarget;
			if (cameraTarget != null && cameraTarget.Target == null)
			{
				cameraTarget.Target = obj.transform;
			}
			CameraLiteManager instance = CameraLiteManager.Instance;
			if (instance != null)
			{
				instance.PushCamera(alternativeCamera, cameraBlendInTime);
				bCameraOverriden = true;
				if (cameraBlendOutStartTime > 0f)
				{
					StartCoroutine(CameraWait(cameraBlendOutStartTime));
				}
			}
		}
		if (effectSequence != null)
		{
			BehaviorManager behaviorManager = obj.GetComponent(typeof(BehaviorManager)) as BehaviorManager;
			if (!(behaviorManager == null))
			{
				BehaviorEffectSequence behaviorEffectSequence = behaviorManager.requestChangeBehavior(typeof(BehaviorEffectSequence), true) as BehaviorEffectSequence;
				if (behaviorEffectSequence != null)
				{
					behaviorEffectSequence.Initialize(effectSequence, EffectSequenceDone);
				}
				else
				{
					Object.Destroy(effectSequence);
				}
			}
		}
		else
		{
			EffectSequenceDone(obj);
		}
	}

	protected void EffectSequenceDone(GameObject obj)
	{
		BehaviorManager behaviorManager = obj.GetComponent(typeof(BehaviorManager)) as BehaviorManager;
		if (!(behaviorManager == null))
		{
			BehaviorSpline behaviorSpline = behaviorManager.requestChangeBehavior(typeof(BehaviorSpline), true) as BehaviorSpline;
			if (behaviorSpline != null)
			{
				behaviorSpline.Initialize(spline, true, SplineDone, false, true);
			}
		}
	}

	protected void SplineDone(GameObject obj)
	{
		CameraExit();
	}

	protected IEnumerator CameraWait(float time)
	{
		yield return new WaitForSeconds(time);
		CameraExit();
	}

	protected void CameraExit()
	{
		if (bCameraOverriden)
		{
			CameraLiteManager instance = CameraLiteManager.Instance;
			if (instance != null)
			{
				instance.PopCamera(cameraBlendOutTime);
				bCameraOverriden = false;
			}
		}
	}

	protected void ActivateHotSpot()
	{
		activeRoot.SetActiveRecursively(true);
		inactiveRoot.SetActiveRecursively(false);
	}

	protected void DeactiveHotSpot()
	{
		activeRoot.SetActiveRecursively(false);
		inactiveRoot.SetActiveRecursively(true);
	}
}
