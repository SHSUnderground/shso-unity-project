using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Camera/Camera Lite Manager")]
public class CameraLiteManager : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public static CameraLiteManager Instance;

	public CameraLite defaultCamera;

	public Camera realCamera;

	public CameraLite[] cameras;

	public Vector3 cameraOffset = default(Vector3);

	public int cameraOffsetFrameCount;

	public AudioListener audioListener;

	public Vector3 audioListenerOffset;

	public Quaternion audioListenerRotation;

	protected Stack<CameraLite> cameraStack = new Stack<CameraLite>();

	private CameraLite cameraOverride;

	protected float blendTime;

	protected float blendScale = 1f;

	protected CameraData camFrom;

	public Stack<CameraLite> CameraStack
	{
		get
		{
			return cameraStack;
		}
	}

	public CameraLite CameraOverride
	{
		get
		{
			return cameraOverride;
		}
	}

	public void ReplaceCamera(CameraLite newCam, float inBlendTime)
	{
		if (newCam == null)
		{
			throw new ArgumentNullException("newCam");
		}
		if (inBlendTime == 0f)
		{
			inBlendTime = -1f;
		}
		if (inBlendTime > 0f)
		{			
			camFrom = new CameraData(realCamera);
		}
		else
		{
			camFrom = null;
		}
		if (cameraStack.Count > 0)
		{
			cameraStack.Peek().SleepFromMgr();
			cameraStack.Pop();
		}
		cameraStack.Push(newCam);
		cameraStack.Peek().WakeFromMgr();
		blendTime = inBlendTime;
		blendScale = 1f / blendTime;
	}

	public void PushCamera(CameraLite newCam, float inBlendTime)
	{
		if (newCam == null)
		{
			throw new ArgumentNullException("newCam");
		}
		if (inBlendTime == 0f)
		{
			inBlendTime = -1f;
		}
		if (inBlendTime > 0f)
		{
			camFrom = new CameraData(realCamera);
			camFrom.position -= cameraOffset;
		}
		else
		{
			camFrom = null;
		}
		if (cameraStack.Count > 0)
		{
			cameraStack.Peek().SleepFromMgr();
		}
		cameraStack.Push(newCam);
		cameraStack.Peek().WakeFromMgr();
		blendTime = inBlendTime;
		blendScale = 1f / blendTime;
	}

	public void PopCamera(float inBlendTime)
	{
		if (cameraStack.Count > 1)
		{
			if (inBlendTime == 0f)
			{
				inBlendTime = -1f;
			}
			if (inBlendTime > 0f)
			{
				camFrom = new CameraData(realCamera);
				camFrom.position -= cameraOffset;
			}
			else
			{
				camFrom = null;
			}
			cameraStack.Peek().SleepFromMgr();
			cameraStack.Pop();
			cameraStack.Peek().WakeFromMgr();
			blendTime = inBlendTime;
			blendScale = 1f / blendTime;
		}
	}

	public void OverrideCamera(CameraLite camera, float inBlendTime)
	{
		if (camera == null)
		{
			if (cameraOverride != null)
			{
				cameraOverride.SleepFromMgr();
			}
			if (cameraStack.Count > 0)
			{
				cameraStack.Peek().WakeFromMgr();
			}
			cameraOverride = null;
		}
		else
		{
			if (cameraStack.Count > 0)
			{
				cameraStack.Peek().SleepFromMgr();
			}
			cameraOverride = camera;
			cameraOverride.WakeFromMgr();
		}
		if (inBlendTime == 0f)
		{
			inBlendTime = -1f;
		}
		if (inBlendTime > 0f)
		{
			camFrom = new CameraData(realCamera);
			camFrom.position -= cameraOffset;
		}
		else
		{
			camFrom = null;
		}
		blendTime = inBlendTime;
		blendScale = 1f / blendTime;
	}

	public CameraLite GetCurrentCamera()
	{
		return cameraStack.Peek();
	}

	public virtual void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}

	public virtual void Start()
	{
		if (realCamera == null)
		{
			GameObject gameObject = GameObject.FindGameObjectWithTag("MainCamera");
			if (gameObject != null)
			{
				//CspUtils.DebugLog("**CLM gameObject.name=" + gameObject.name);
				realCamera = (gameObject.GetComponent(typeof(Camera)) as Camera);
				//CspUtils.DebugLog("**CLM realCamera.name=" + realCamera.name);
			}
		}
		audioListener = realCamera.GetComponentInChildren<AudioListener>();
		if (audioListener != null)
		{
			if (audioListener.gameObject == realCamera.gameObject)
			{
				GameObject gameObject2 = new GameObject("AudioListener");
				audioListenerOffset = Vector3.zero;
				audioListenerRotation = Quaternion.identity;
				gameObject2.transform.position = realCamera.gameObject.transform.position;
				gameObject2.transform.rotation = realCamera.gameObject.transform.rotation;
				UnityEngine.Object.Destroy(audioListener);
				audioListener = gameObject2.AddComponent<AudioListener>();
			}
			else
			{
				audioListenerOffset = audioListener.transform.localPosition;
				audioListenerRotation = audioListener.transform.localRotation;
				Vector3 position = audioListener.transform.position;
				Quaternion rotation = audioListener.transform.rotation;
				audioListener.transform.parent = null;
				audioListener.transform.position = position;
				audioListener.transform.rotation = rotation;
			}
		}
		else
		{
			audioListener = (UnityEngine.Object.FindObjectOfType(typeof(AudioListener)) as AudioListener);
			if (audioListener != null)
			{
				audioListenerOffset = Vector3.zero;
				audioListenerRotation = Quaternion.identity;
			}
		}
		initializeCameras();
	}

	public virtual void LateUpdate()
	{
		if (cameraStack.Count <= 0 && cameraOverride == null)
		{
			return;
		}
		CameraLite cameraLite = (!(cameraOverride != null)) ? cameraStack.Peek() : cameraOverride;
		if (cameraLite == null)
		{
			return;
		}
		float num = Time.deltaTime;
		while (num > 0f)
		{
			float num2 = num;
			if (num2 > 71f / (678f * (float)Math.PI))
			{
				num2 = 71f / (678f * (float)Math.PI);
			}
			num -= num2;
			if (cameraLite != null)
			{
				cameraLite.UpdateFromMgr(num2);
				cameraLite.UpdateAudioFromMgr(num2);
			}
		}
		CameraData cameraData = null;
		if (camFrom != null)
		{
			CameraData cameraData2 = new CameraData(cameraLite);
			blendTime -= Time.deltaTime;
			if (blendTime <= 0f)
			{
				camFrom = null;
				cameraData = cameraData2;
			}
			else
			{
				cameraData = CameraData.Lerp(cameraData2, camFrom, blendTime * blendScale);
			}
		}
		else
		{
			cameraData = new CameraData(cameraLite);
		}
		if (realCamera != null)
		{
			cameraData.CopyToCamera(realCamera);
			if (audioListener != null)
			{
				audioListener.transform.position = cameraData.audioPosition;
				audioListener.transform.rotation = cameraData.audioRotation;
			}
			realCamera.transform.position += cameraOffset;
			//CspUtils.DebugLog("**CLM realCamera.name=" + realCamera.name);
			//CspUtils.DebugLog("**CLM realCamera.transform.position= " + realCamera.transform.position);
		}
	}

	protected virtual void initializeCameras()
	{
		if (cameras.Length > 0)
		{
			if (defaultCamera == null)
			{
				defaultCamera = cameras[0];
			}
			for (int i = 0; i < cameras.Length; i++)
			{
				CameraLite cameraLite = cameras[i];
				cameraLite.InitFromMgr();
				cameraLite.SleepFromMgr();
			}
			PushCamera(defaultCamera, -1f);
		}
	}
}
