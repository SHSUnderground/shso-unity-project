using System.Collections;
using UnityEngine;

public class SplineInteractiveObjectBaseController : HotspotInteractiveObjectController
{
	public SplineController spline;

	public CameraLite alternativeCamera;

	public float cameraBlendInTime = 0.5f;

	public float cameraBlendOutTime = 0.5f;

	public float cameraBlendOutStartTime = -1f;

	protected bool bCameraOverriden;

	protected CameraLiteManager cameraMgr;

	protected bool allowApproachCancel = true;

	public override bool StartWithPlayer(GameObject player, OnDone onDone)
	{
		if (!base.StartWithPlayer(player, onDone))
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
		reportHotSpotUse(player);
		PlayVO(player);
		Vector3 pos;
		Quaternion rot;
		spline.GetFirstPoint(out pos, out rot);
		behaviorApproach.Initialize(pos, rot, allowApproachCancel, ApproachArrived, ApproachCancel, 0.15f, 2f, false, false);
		if (onDone != null)
		{
			onDone(player, CompletionStateEnum.Success);
		}
		return true;
	}

	protected virtual void ApproachArrived(GameObject obj)
	{
		CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(obj);
		if (component != null)
		{
			component.DisableNetUpdates(true);
		}
		Launch(obj);
		CharacterGlobals component2 = obj.GetComponent<CharacterGlobals>();
		if (component2 != null && component2.activeSidekick != null && (component2.activeSidekick.motionController.hotSpotType & HotSpotType.Style.Flying) != 0)
		{
			PetCommandManager component3 = component2.activeSidekick.gameObject.GetComponent<PetCommandManager>();
			component3.AddCommand(new PetFlyCommand(), true);
			ApproachArrived(component2.activeSidekick.gameObject);
		}
	}

	protected virtual void ApproachCancel(GameObject obj)
	{
	}

	protected void Launch(GameObject obj)
	{
		Vector3 pos;
		Quaternion rot;
		spline.GetFirstPoint(out pos, out rot);
		Vector3 position = obj.transform.position;
		pos.y = position.y;
		obj.transform.position = pos;
		obj.transform.rotation = rot;
		bool flag = Utils.IsLocalPlayer(obj);
		bCameraOverriden = false;
		if (!flag)
		{
			return;
		}
		if (cameraMgr == null)
		{
			cameraMgr = CameraLiteManager.Instance;
		}
		if (alternativeCamera != null)
		{
			CameraTarget cameraTarget = alternativeCamera.GetComponent(typeof(CameraTarget)) as CameraTarget;
			if (cameraTarget != null && cameraTarget.Target == null)
			{
				cameraTarget.Target = obj.transform;
			}
			if (cameraMgr != null)
			{
				cameraMgr.PushCamera(alternativeCamera, cameraBlendInTime);
				bCameraOverriden = true;
				if (cameraBlendOutStartTime > 0f)
				{
					StartCoroutine(CameraWait(cameraBlendOutStartTime, obj));
				}
			}
		}
		if (cameraMgr != null)
		{
			cameraMgr.GetCurrentCamera().SplineState(true);
		}
	}

	protected virtual void SplineDone(GameObject obj)
	{
		CharacterMotionController component = Utils.GetComponent<CharacterMotionController>(obj);
		if (component != null)
		{
			component.DisableNetUpdates(false);
		}
		AppShell.Instance.EventMgr.Fire(base.gameObject, new PetFlyCommand.PetFlightEndedEvent(obj));
		CameraExit(obj);
	}

	protected IEnumerator CameraWait(float time, GameObject obj)
	{
		yield return new WaitForSeconds(time);
		CameraExit(obj);
	}

	protected void CameraExit(GameObject obj)
	{
		if (Utils.IsLocalPlayer(obj) && cameraMgr != null)
		{
			cameraMgr.GetCurrentCamera().SplineState(false);
			if (bCameraOverriden)
			{
				cameraMgr.PopCamera(cameraBlendOutTime);
				bCameraOverriden = false;
			}
		}
	}

	protected void PlayVO(GameObject player)
	{
		if (HotspotVOCooldown.OkToPlay(player, hotSpotType))
		{
			HotspotVOCooldown.StartCooldown(player, hotSpotType);
			if ((hotSpotType & HotSpotType.Style.Flying) != 0)
			{
				VOManager.Instance.PlayVO("use_hotspot", player);
			}
			else if ((hotSpotType & HotSpotType.Style.Web) != 0)
			{
				VOManager.Instance.PlayVO("use_web_hotspot", player);
			}
			else if ((hotSpotType & HotSpotType.Style.GroundSpeed) != 0)
			{
				VOManager.Instance.PlayVO("use_groundspeed_hotspot", player);
			}
		}
	}
}
