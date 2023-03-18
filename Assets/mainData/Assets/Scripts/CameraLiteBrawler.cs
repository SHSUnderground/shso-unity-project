using System;
using UnityEngine;

[AddComponentMenu("Camera/Camera Lite Brawler")]
public class CameraLiteBrawler : CameraLite
{
	[Serializable]
	public class CameraMap : IComparable<CameraMap>
	{
		public float min;

		public float max = 1f;

		public CameraLite camera;

		public int CompareTo(CameraMap other)
		{
			return min.CompareTo(other.min);
		}
	}

	protected class CameraBlock
	{
		public CameraLite cam0;

		public CameraLite cam1;

		public float min = -1f;

		public float max = -1f;
	}

	public float zoomDistance = 15f;

	public float zoomSpeed = 750f;

	public float zoomSmooth = 10f;

	public CameraMap[] cameras = new CameraMap[2];

	protected CameraBlock[] blocks = new CameraBlock[3];

	protected float zoomMin;

	protected float zoomMax;

	protected float distance;

	protected float velocity;

	public override void InitFromMgr()
	{
		CameraMap[] array = cameras;
		foreach (CameraMap cameraMap in array)
		{
			if (cameraMap.camera.transform.parent == base.transform)
			{
				cameraMap.camera.transform.parent = base.transform.parent;
			}
		}
		Array.Sort(cameras);
		blocks[0] = new CameraBlock();
		blocks[0].cam0 = cameras[0].camera;
		blocks[0].cam1 = null;
		blocks[0].min = cameras[0].min;
		blocks[0].max = cameras[1].min;
		blocks[1] = new CameraBlock();
		blocks[1].cam0 = cameras[0].camera;
		blocks[1].cam1 = cameras[1].camera;
		blocks[1].min = cameras[1].min;
		blocks[1].max = cameras[0].max;
		blocks[2] = new CameraBlock();
		blocks[2].cam0 = cameras[1].camera;
		blocks[2].cam1 = null;
		blocks[2].min = cameras[0].max;
		blocks[2].max = cameras[1].max;
		zoomMin = blocks[0].min;
		zoomMax = blocks[2].max;
		distance = zoomDistance;
	}

	public override void WakeFromMgr()
	{
		base.WakeFromMgr();
		CameraMap[] array = cameras;
		foreach (CameraMap cameraMap in array)
		{
			cameraMap.camera.WakeFromMgr();
		}
	}

	public override void UpdateFromMgr(float deltaTime)
	{
		if (targetComponent != null && target == null)
		{
			base.transform.position = Camera.main.transform.position;
			base.transform.rotation = Camera.main.transform.rotation;
			return;
		}
		float mouseWheelDelta = SHSInput.GetMouseWheelDelta();
		if (mouseWheelDelta != 0f)
		{
			float num = mouseWheelDelta * deltaTime * (0f - zoomSpeed);
			zoomDistance = Mathf.Clamp(zoomDistance + num, zoomMin, zoomMax);
		}
		if (!Mathf.Approximately(distance, zoomDistance))
		{
			float num2 = deltaTime * zoomSmooth;
			float f = zoomDistance - distance;
			if (Mathf.Abs(f) <= num2)
			{
				distance = zoomDistance;
			}
			else
			{
				distance += num2 * Mathf.Sign(f);
			}
		}
		CameraMap[] array = cameras;
		foreach (CameraMap cameraMap in array)
		{
			cameraMap.camera.SetDistance(distance);
			cameraMap.camera.UpdateFromMgr(deltaTime);
		}
		CameraBlock blockFromDistance = GetBlockFromDistance(distance);
		if (blockFromDistance.cam1 == null)
		{
			SetFromCameraLite(blockFromDistance.cam0);
		}
		else
		{
			float time = (distance - blockFromDistance.min) / (blockFromDistance.max - blockFromDistance.min);
			SetFromCameraData(CameraData.Lerp(blockFromDistance.cam0, blockFromDistance.cam1, time));
		}
		base.UpdateFromMgr(deltaTime);
	}

	public override void SleepFromMgr()
	{
		CameraMap[] array = cameras;
		foreach (CameraMap cameraMap in array)
		{
			cameraMap.camera.SleepFromMgr();
		}
	}

	protected CameraBlock GetBlockFromDistance(float d)
	{
		CameraBlock[] array = blocks;
		foreach (CameraBlock cameraBlock in array)
		{
			if (cameraBlock.min <= d && d < cameraBlock.max)
			{
				return cameraBlock;
			}
		}
		return blocks[blocks.Length - 1];
	}
}
