using UnityEngine;

[AddComponentMenu("Cut Scene Clips/Camera Target")]
internal class CutSceneCameraTargetEvent : CutSceneEvent
{
	public CameraLiteSpring eventCamera;

	public string eventCameraTarget;

	public Vector3 targetToCamera = Vector3.zero;

	public float distanceFromTarget;

	private Transform mOrgTarget;

	private Vector3 mOrgTargetToCamera = Vector3.zero;

	private float mOrgDistance;

	public override void StartEvent()
	{
		base.StartEvent();
		if (eventCamera == null)
		{
			LogEventError("Camera not set for camera target event");
		}
		GameObject gameObject = null;
		if (eventCameraTarget == null)
		{
			LogEventError("Target not set for camera target event");
		}
		else
		{
			gameObject = GameObject.Find(eventCameraTarget);
		}
		if (gameObject == null)
		{
			LogEventError("Target not found for camera target event");
		}
		if (eventCamera != null)
		{
			mOrgTarget = eventCamera.GetTarget();
			mOrgTargetToCamera = eventCamera.targetToCamera;
			mOrgDistance = eventCamera.distanceFromTarget;
			if (gameObject != null)
			{
				eventCamera.SetTarget(gameObject.transform);
			}
			eventCamera.targetToCamera = targetToCamera;
			eventCamera.distanceFromTarget = distanceFromTarget;
		}
	}

	public override void EndEvent()
	{
		base.EndEvent();
		if (eventCamera != null)
		{
			eventCamera.SetTarget(mOrgTarget);
			eventCamera.targetToCamera = mOrgTargetToCamera;
			eventCamera.distanceFromTarget = mOrgDistance;
		}
	}

	public void OnDrawGizmosSelected()
	{
		if (eventCamera != null)
		{
			DrawGizmoToTargetTransform(eventCamera.transform);
		}
	}
}
