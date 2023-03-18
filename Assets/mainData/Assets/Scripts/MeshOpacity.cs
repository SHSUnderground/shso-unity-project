using UnityEngine;

public class MeshOpacity : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private float alpha_ = 1f;

	public float alpha = 1f;

	private Renderer[] renderers;

	public void SetOpacity(float a)
	{
		alpha_ = a;
		alpha = a;
		if (renderers == null)
		{
			return;
		}
		Renderer[] array = renderers;
		foreach (Renderer renderer in array)
		{
			Material[] materials = renderer.materials;
			foreach (Material material in materials)
			{
				Color color = material.color;
				color.a = a;
				material.color = color;
			}
		}
	}

	private void Start()
	{
		renderers = base.gameObject.GetComponentsInChildren<Renderer>();
		SetOpacity(alpha);
	}

	private void Update()
	{
		if (alpha != alpha_)
		{
			SetOpacity(alpha);
		}
	}
}
