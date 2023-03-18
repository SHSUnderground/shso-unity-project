using UnityEngine;

public class SnapToGround : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private const int attemptCount = 3;

	private const float retryOffset = 3f;

	public int layerMask = 100608;

	public Vector3 offset = new Vector3(0f, 0.1f, 0f);

	public float maxDistance = 100f;

	private void Start()
	{
		SetTargetPosition(base.transform.position);
	}

	public void SetTargetPosition(Vector3 pos)
	{
		Vector3 origin = pos + new Vector3(0f, 0.1f, 0f);
		Ray ray = new Ray(origin, Vector3.up);
		Ray ray2 = new Ray(origin, Vector3.down);
		RaycastHit result = default(RaycastHit);
		RaycastHit result2 = default(RaycastHit);
		bool flag = false;
		bool flag2 = false;
		int num = 0;
		while (num < 3 && !flag && !flag2)
		{
			flag = Raycast(ray, out result);
			flag2 = Raycast(ray2, out result2);
			num++;
			origin.y += 3f;
			ray.origin = origin;
			ray2.origin = origin;
		}
		if (flag && flag2)
		{
			if (result.distance < result2.distance)
			{
				MoveTo(result);
			}
			else
			{
				MoveTo(result2);
			}
		}
		else if (flag || flag2)
		{
			MoveTo((!flag2) ? result : result2);
		}
	}

	private bool Raycast(Ray ray, out RaycastHit result)
	{
		return Physics.Raycast(ray, out result, maxDistance, layerMask);
	}

	private void MoveTo(RaycastHit rayResult)
	{
		Vector3 position = rayResult.point + offset;
		base.transform.position = position;
	}
}
