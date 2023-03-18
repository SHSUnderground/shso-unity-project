using UnityEngine;

public class BrawlerSpawnSource : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string spawnAnimation = string.Empty;

	public float spawnAnimationSpeed = 1f;

	public float spawnInSpeed = 12f;

	public float startAngle;

	public float endAngle;

	public Vector3 startRotation = Vector3.zero;

	public Vector3 endRotation = Vector3.zero;

	public bool freeFallOnCompletion = true;

	public bool playFullAnimation;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(base.transform.position, 0.25f);
		Gizmos.DrawLine(base.transform.TransformPoint(0f, 0f, 0f), base.transform.TransformPoint(0f, 1f, 0f));
	}

	public static void InitMotionVectors(GameObject source, Vector3 destination, ref Vector3 upVector, ref Vector3 atVector, ref float vectorLength)
	{
		upVector = source.transform.TransformDirection(0f, 1f, 0f);
		atVector = destination - source.transform.position;
		vectorLength = atVector.magnitude;
		float num = Mathf.Sqrt(1f - upVector.y * upVector.y);
		Vector3 vector = new Vector3(atVector.x, 0f, atVector.z);
		vector.Normalize();
		upVector.x = vector.x * num * vectorLength;
		upVector.y *= vectorLength;
		upVector.z = vector.z * num * vectorLength;
	}

	public static Vector3 EvalPosition(float curveTime, Vector3 source, Vector3 upVector, Vector3 atVector)
	{
		return source + upVector * curveTime * (1f - curveTime) + atVector * curveTime * curveTime;
	}
}
