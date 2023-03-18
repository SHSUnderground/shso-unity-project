using UnityEngine;

public class OffsetTracker : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject toTrack;

	public Vector3 positionOffset = new Vector3(0f, 0f, 0f);

	private void Start()
	{
	}

	private void Update()
	{
		if (toTrack != null)
		{
			base.transform.position = toTrack.transform.position + positionOffset;
		}
	}
}
