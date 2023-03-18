using UnityEngine;

public class DioramaPositioner : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public enum DioramaSizePosition
	{
		Huge,
		Large,
		Medium,
		Small
	}

	public DioramaSizePosition positionSlot;

	public float positionSnapOffset = 0.1f;

	private void Start()
	{
		MeshRenderer meshRenderer = GetComponentInChildren(typeof(MeshRenderer)) as MeshRenderer;
		if (meshRenderer != null)
		{
			meshRenderer.enabled = false;
		}
	}

	private void Update()
	{
	}
}
