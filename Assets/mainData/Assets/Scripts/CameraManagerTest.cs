using System.Collections.Generic;
using UnityEngine;

public class CameraManagerTest : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected List<Camera> m_cameras = new List<Camera>();

	protected int m_idxCamera = -1;

	public Camera activeCamera
	{
		get
		{
			return m_cameras[m_idxCamera];
		}
	}

	private void Awake()
	{
		foreach (Transform item in base.transform)
		{
			Camera camera = item.gameObject.camera;
			if (camera != null)
			{
				if (camera.gameObject.active && m_idxCamera < 0)
				{
					m_idxCamera = m_cameras.Count;
				}
				else
				{
					camera.gameObject.active = false;
				}
				m_cameras.Add(camera);
				SkyCamera skyCamera = camera.GetComponent(typeof(SkyCamera)) as SkyCamera;
				if ((bool)skyCamera)
				{
					skyCamera.CameraInit();
				}
			}
			else
			{
				CspUtils.DebugLog("Ignoring an unexpected non-camera");
			}
		}
		if (m_idxCamera < 0)
		{
			m_idxCamera = 0;
			m_cameras[0].gameObject.active = true;
		}
	}

	private void Update()
	{
		if (SHSInput.GetKeyDown(KeyCode.C))
		{
			int num = (m_idxCamera + 1) % m_cameras.Count;
			if (num != m_idxCamera)
			{
				m_cameras[m_idxCamera].gameObject.active = false;
				m_cameras[num].gameObject.active = true;
				m_idxCamera = num;
			}
		}
	}
}
