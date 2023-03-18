using UnityEngine;

[ExecuteInEditMode]
public class FullScreenEffect : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected Camera activeCamera;

	private void Start()
	{
		activeCamera = Camera.main;
		LoadResources();
	}

	private void Update()
	{
	}

	public void OnDisable()
	{
		ReleaseResources();
		activeCamera = null;
	}

	public virtual void LoadResources()
	{
	}

	public virtual void ReleaseResources()
	{
	}
}
