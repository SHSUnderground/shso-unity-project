using System.Collections.Generic;
using UnityEngine;

public class CameraShakeManager
{
	protected List<CameraShake> shakes;

	protected int lastUpdateFrame;

	protected Vector3 lastCameraOffset;

	protected CameraLiteManager lastCameraMgr;

	protected Camera lastCamera;

	protected bool shakesEnabled = true;

	protected static CameraShakeManager _instance;

	public static CameraShakeManager Instance
	{
		get
		{
			return _instance;
		}
	}

	protected CameraShakeManager()
	{
		shakes = new List<CameraShake>();
	}

	public static CameraShakeManager CreateSingleton()
	{
		if (_instance == null)
		{
			_instance = new CameraShakeManager();
		}
		return _instance;
	}

	public void AddShake(CameraShake shake)
	{
		shakes.Remove(shake);
		shakes.Add(shake);
		if (shake != null)
		{
			shake.EnableShaking = shakesEnabled;
		}
	}

	public void RemoveShake(CameraShake shake)
	{
		shakes.Remove(shake);
		if (shakes.Count == 0)
		{
			ApplyOffset(Vector3.zero);
		}
	}

	protected void ApplyOffset(Vector3 offset)
	{
		if (CameraLiteManager.Instance != null)
		{
			if (lastCameraMgr == CameraLiteManager.Instance)
			{
				CameraLiteManager.Instance.cameraOffset -= lastCameraOffset;
			}
			else
			{
				lastCameraMgr = CameraLiteManager.Instance;
			}
			CameraLiteManager.Instance.cameraOffset += offset;
		}
		else if (Camera.main != null)
		{
			if (lastCamera == Camera.main)
			{
				Camera.main.transform.position -= lastCameraOffset;
			}
			else
			{
				lastCamera = Camera.main;
			}
			Camera.main.transform.position += offset;
		}
		lastCameraOffset = offset;
	}

	public void Update()
	{
		if (Time.frameCount != lastUpdateFrame)
		{
			lastUpdateFrame = Time.frameCount;
			Vector3 offset = default(Vector3);
			foreach (CameraShake shake in shakes)
			{
				offset += shake.ShakeOffset;
			}
			ApplyOffset(offset);
		}
	}

	public void EnableShakes(bool enable)
	{
		shakesEnabled = enable;
		foreach (CameraShake shake in shakes)
		{
			shake.EnableShaking = enable;
		}
	}
}
