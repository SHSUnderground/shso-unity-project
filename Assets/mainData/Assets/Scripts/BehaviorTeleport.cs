using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorTeleport : BehaviorSplineBase
{
	protected float teleportStartTime;

	protected float teleportTime;

	protected Vector3 teleportVelocity = default(Vector3);

	protected bool teleportStarted;

	protected GameObject teleportObject;

	protected GameObject teleportPurgatory;

	protected GameObject teleportDestination;

	protected bool useSpline = true;

	public void Initialize(OnBehaviorDone onDone, GameObject purgatory, float teleportStart, bool useSpline, SplineController spline, GameObject destination, float teleportTime, bool rotate)
	{
		base.spline = spline;
		charController = charGlobals.characterController;
		effectsList = charGlobals.effectsList;
		activeEffects = new Dictionary<string, EffectSequence>();
		onBehaviorDone = onDone;
		followRotations = rotate;
		ignoreCollision = true;
		freeFall = false;
		teleportStartTime = teleportStart;
		this.teleportTime = teleportTime;
		teleportPurgatory = purgatory;
		teleportDestination = destination;
		this.useSpline = useSpline;
		if (useSpline)
		{
			state = States.SplineTeleport;
			InitSplineMovement();
		}
	}

	public override void behaviorUpdate()
	{
		base.behaviorUpdate();
		if (!HasTeleportStarted() && CanTeleportStart())
		{
			StartTeleport();
		}
		if (!HasTeleportStarted())
		{
			return;
		}
		if (useSpline)
		{
			States state = base.state;
			if (state == States.SplineTeleport)
			{
				StateSpline(teleportObject);
				return;
			}
			CspUtils.DebugLog("Unexpected state");
			if (onBehaviorDone != null)
			{
				onBehaviorDone(owningObject);
			}
			if (charGlobals.behaviorManager.getBehavior() == this)
			{
				charGlobals.behaviorManager.endBehavior();
			}
		}
		else if (elapsedTime - teleportStartTime >= teleportTime)
		{
			if (onBehaviorDone != null)
			{
				onBehaviorDone(owningObject);
			}
			if (charGlobals.behaviorManager.getBehavior() == this)
			{
				charGlobals.behaviorManager.endBehavior();
			}
		}
		else
		{
			teleportObject.transform.position = teleportObject.transform.position + teleportVelocity * Time.deltaTime;
		}
	}

	public override void behaviorEnd()
	{
		base.behaviorEnd();
		EndTeleport();
	}

	protected void StartTeleport()
	{
		teleportStarted = true;
		teleportObject = new GameObject("Teleporter");
		teleportObject.transform.position = owningObject.transform.position;
		if (followRotations)
		{
			teleportObject.transform.rotation = owningObject.transform.rotation;
		}
		charGlobals.motionController.teleportTo(teleportPurgatory.transform.position);
		SkinnedMeshRenderer componentInChildren = owningObject.GetComponentInChildren<SkinnedMeshRenderer>();
		if (componentInChildren != null)
		{
			componentInChildren.castShadows = false;
			componentInChildren.receiveShadows = false;
		}
		if (Utils.IsLocalPlayer(charGlobals))
		{
			PlayerOcclusionDetector.Instance.tempDisable = true;
			CameraTargetHelper componentInChildren2 = owningObject.GetComponentInChildren<CameraTargetHelper>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.SetTarget(teleportObject.transform);
			}
			ShsAudioSource.PlayAutoSound(GUIManager.Instance.GetUISound("camera_whoosh"));
		}
		if (!useSpline && teleportDestination != null)
		{
			Vector3 vector = teleportDestination.transform.position - teleportObject.transform.position;
			teleportVelocity = vector.normalized * (vector.magnitude / teleportTime);
		}
	}

	protected void EndTeleport()
	{
		Transform transform = (!(teleportDestination != null)) ? teleportObject.transform : teleportDestination.transform;
		charGlobals.motionController.setDestination(transform.position);
		charGlobals.motionController.teleportTo(transform.position);
		if (followRotations)
		{
			owningObject.transform.rotation = transform.rotation;
		}
		SkinnedMeshRenderer componentInChildren = owningObject.GetComponentInChildren<SkinnedMeshRenderer>();
		if (componentInChildren != null)
		{
			componentInChildren.castShadows = true;
			componentInChildren.receiveShadows = true;
		}
		if (Utils.IsLocalPlayer(charGlobals))
		{
			PlayerOcclusionDetector.Instance.tempDisable = false;
			CameraTargetHelper componentInChildren2 = teleportObject.GetComponentInChildren<CameraTargetHelper>();
			if (componentInChildren2 != null)
			{
				componentInChildren2.SetTarget(owningObject.transform);
			}
			if (teleportTime <= 0f)
			{
				charGlobals.StartCoroutine(ResetCameraOnNextFrame());
			}
		}
		Object.Destroy(teleportObject);
		teleportObject = null;
	}

	protected bool CanTeleportStart()
	{
		return elapsedTime >= teleportStartTime;
	}

	protected bool HasTeleportStarted()
	{
		return teleportStarted;
	}

	private IEnumerator ResetCameraOnNextFrame()
	{
		yield return 0;
		CameraLiteManager.Instance.GetCurrentCamera().Reset();
	}
}
