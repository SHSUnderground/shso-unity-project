using UnityEngine;

[AddComponentMenu("Camera/Camera Target Helper")]
public class CameraTargetHelper : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected const float DEFAULT_LOCK_TIME_OUT = 8f;

	protected bool lockX;

	protected bool lockY;

	protected bool lockZ;

	protected Vector3 oldPosition = Vector3.zero;

	protected float lockTimer = 8f;

	protected float lockTimeOut = 8f;

	public void SetTarget(Transform newTarget)
	{
		base.transform.parent = newTarget;  // set parent to newTarget
		base.transform.localPosition = Vector3.zero;  // set position equal to parent
	}

	public void LockAxes(bool x, bool y, bool z, float timeOut)
	{
		lockX = x;
		lockY = y;
		lockZ = z;
		if (lockX)
		{
			
			Vector3 position = base.transform.position;
			oldPosition.x = position.x;
		}
		if (lockY)
		{
			
			Vector3 position2 = base.transform.position;
			oldPosition.y = position2.y;
		}
		if (lockX)
		{
			
			Vector3 position3 = base.transform.position;
			oldPosition.z = position3.z;
		}
		if (lockX || lockY || lockZ)
		{
			lockTimer = 0f;
			lockTimeOut = timeOut;
			if (lockTimeOut == 0f)
			{
				lockTimeOut = 8f;
			}
		}
		else
		{
			lockTimer = lockTimeOut;
		}
	}

	public virtual void Update()
	{
		Vector3 position = base.transform.parent.position;
		if (lockX)
		{
			position.x = oldPosition.x;
		}
		if (lockY)
		{
			position.y = oldPosition.y;
		}
		if (lockZ)
		{
			position.z = oldPosition.z;
		}
		base.transform.position = position;
		if (lockTimer < lockTimeOut)
		{
			lockTimer += Time.deltaTime;
			if (lockTimer >= lockTimeOut)
			{
				LockAxes(false, false, false, 0f);
			}
		}
	}

	public bool AxesLocked()
	{
		return lockX || lockY || lockZ;
	}
}
