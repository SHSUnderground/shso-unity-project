using UnityEngine;

[AddComponentMenu("Cut Scene Clips/Camera")]
public class CutSceneCameraEvent : CutSceneEvent
{
	public CameraLite eventCamera;

	private bool eventCameraPushed;

	public override void StartEvent()
	{
		base.StartEvent();
		if (eventCamera == null)
		{
			LogEventError("Camera not set for camera event");
			return;
		}
		CameraLiteManager.Instance.PushCamera(eventCamera, 0f);
		eventCameraPushed = true;
	}

	public override void EndEvent()
	{
		base.EndEvent();
		if (eventCameraPushed)
		{
			CameraLiteManager.Instance.PopCamera(0f);
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
