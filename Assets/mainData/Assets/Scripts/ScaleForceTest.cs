using UnityEngine;

public class ScaleForceTest : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public Vector3 oldScale;

	public Vector3 oldLocalScale;

	public Vector3 calculatedScale;

	public Vector3 parentScale;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void LateUpdate()
	{
		oldScale = base.transform.lossyScale;
		oldLocalScale = base.transform.localScale;
		Vector3 localScale = base.transform.localScale;
		float x = localScale.x;
		Vector3 lossyScale = base.transform.lossyScale;
		Vector3 localScale2 = default(Vector3);
		localScale2.x = x * (1f / lossyScale.x) * 3f;
		Vector3 localScale3 = base.transform.localScale;
		float y = localScale3.y;
		Vector3 lossyScale2 = base.transform.lossyScale;
		localScale2.y = y * (1f / lossyScale2.y) * 3f;
		Vector3 localScale4 = base.transform.localScale;
		float z = localScale4.z;
		Vector3 lossyScale3 = base.transform.lossyScale;
		localScale2.z = z * (1f / lossyScale3.z) * 3f;
		base.transform.localScale = localScale2;
		calculatedScale = base.transform.lossyScale;
		parentScale = base.transform.parent.transform.lossyScale;
	}
}
