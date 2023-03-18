using System.Collections;
using UnityEngine;

[AddComponentMenu("Rendering/DOF Setup")]
public class DOFSetup : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public bool On = true;

	public float Target = 15f;

	public float Range = 7f;

	public float Falloff = 10f;

	public float Radius = 2.5f;

	public bool Once = true;

	public AOSceneCamera DefaultCamera
	{
		get
		{
			return (!(Camera.main != null)) ? null : Utils.GetComponent<AOSceneCamera>(Camera.main);
		}
	}

	private void Start()
	{
		ApplyToCamera();
	}

	private void OnEnable()
	{
		ApplyToCamera();
	}

	public void ApplyToCamera()
	{
		ApplyToCamera(DefaultCamera);
	}

	public void ApplyToCamera(AOSceneCamera camera)
	{
		if (!(camera == null))
		{
			camera.DepthOfField = (On & GraphicsOptions.DOF);
			camera.DOFTarget = Target;
			camera.DOFRange = Range;
			camera.DOFFalloff = Falloff;
			camera.DOFRadius = Radius;
			if (Once)
			{
				StartCoroutine(DestroyNextFrame());
			}
		}
	}

	public void CopyFromCamera()
	{
		CopyFromCamera(DefaultCamera);
	}

	public void CopyFromCamera(AOSceneCamera camera)
	{
		On = camera.DepthOfField;
		Target = camera.DOFTarget;
		Range = camera.DOFRange;
		Falloff = camera.DOFFalloff;
		Radius = camera.DOFRadius;
	}

	protected IEnumerator DestroyNextFrame()
	{
		yield return 0;
		if (base.gameObject.GetComponents<MonoBehaviour>().Length == 1)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Object.Destroy(this);
		}
	}
}
