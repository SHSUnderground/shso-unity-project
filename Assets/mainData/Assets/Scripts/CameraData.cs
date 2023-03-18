using System;
using UnityEngine;

[Serializable]
public class CameraData
{
	public float nearClipPlane = 0.3f;

	public float farClipPlane = 1000f;

	public float fieldOfView = 45f;

	public float focalLength = 15f;

	public float dofOffset;

	public Vector3 position = Vector3.zero;

	public Quaternion rotation = Quaternion.identity;

	public Vector3 audioPosition = Vector3.zero;

	public Quaternion audioRotation = Quaternion.identity;

	public CameraData()
	{
	}

	public CameraData(CameraLite cam)
	{
		nearClipPlane = cam.nearClipPlane;
		farClipPlane = cam.farClipPlane;
		fieldOfView = cam.fieldOfView;
		focalLength = cam.focalLength;
		dofOffset = cam.dofOffset;
		position = cam.transform.position;
		rotation = cam.transform.rotation;
		audioPosition = cam.audioPosition;
		audioRotation = cam.audioRotation;
	}

	public CameraData(Camera cam)
	{
		nearClipPlane = cam.nearClipPlane;
		farClipPlane = cam.farClipPlane;
		fieldOfView = cam.fieldOfView;
		AOSceneCamera component = Utils.GetComponent<AOSceneCamera>(cam);
		if (component != null)
		{
			focalLength = component.DOFTarget;
		}
		else
		{
			focalLength = 15f;
		}
		dofOffset = 0f;
		position = cam.transform.position;
		rotation = cam.transform.rotation;
		audioPosition = cam.transform.position;
		audioRotation = cam.transform.rotation;
	}

	public void CopyToCamera(Camera cam)
	{
		cam.nearClipPlane = nearClipPlane;
		cam.farClipPlane = farClipPlane;
		cam.fieldOfView = fieldOfView;
		AOSceneCamera component = Utils.GetComponent<AOSceneCamera>(cam);
		if (component != null)
		{
			component.DOFTarget = focalLength + dofOffset;
		}
		cam.transform.position = position;
		cam.transform.rotation = rotation;
	}

	public static CameraData Lerp(CameraLite cam1, CameraLite cam2, float time)
	{
		CameraData cameraData = new CameraData();
		cameraData.nearClipPlane = Mathf.Lerp(cam1.nearClipPlane, cam2.nearClipPlane, time);
		cameraData.farClipPlane = Mathf.Lerp(cam1.farClipPlane, cam2.farClipPlane, time);
		cameraData.fieldOfView = Mathf.Lerp(cam1.fieldOfView, cam2.fieldOfView, time);
		cameraData.focalLength = Mathf.Lerp(cam1.focalLength, cam2.focalLength, time);
		cameraData.dofOffset = Mathf.Lerp(cam1.dofOffset, cam2.dofOffset, time);
		cameraData.position = Vector3.Lerp(cam1.transform.position, cam2.transform.position, time);
		cameraData.rotation = Quaternion.Slerp(cam1.transform.rotation, cam2.transform.rotation, time);
		cameraData.audioPosition = Vector3.Lerp(cam1.audioPosition, cam2.audioPosition, time);
		cameraData.audioRotation = Quaternion.Slerp(cam1.audioRotation, cam2.audioRotation, time);
		return cameraData;
	}

	public static CameraData Lerp(CameraData cam1, CameraData cam2, float time)
	{
		CameraData cameraData = new CameraData();
		cameraData.nearClipPlane = Mathf.Lerp(cam1.nearClipPlane, cam2.nearClipPlane, time);
		cameraData.farClipPlane = Mathf.Lerp(cam1.farClipPlane, cam2.farClipPlane, time);
		cameraData.fieldOfView = Mathf.Lerp(cam1.fieldOfView, cam2.fieldOfView, time);
		cameraData.focalLength = Mathf.Lerp(cam1.focalLength, cam2.focalLength, time);
		cameraData.dofOffset = Mathf.Lerp(cam1.dofOffset, cam2.dofOffset, time);
		cameraData.position = Vector3.Lerp(cam1.position, cam2.position, time);
		cameraData.rotation = Quaternion.Slerp(cam1.rotation, cam2.rotation, time);
		cameraData.audioPosition = Vector3.Lerp(cam1.audioPosition, cam2.audioPosition, time);
		cameraData.audioRotation = Quaternion.Slerp(cam1.audioRotation, cam2.audioRotation, time);
		return cameraData;
	}
}
