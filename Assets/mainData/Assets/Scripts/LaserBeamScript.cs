using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserBeamScript : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float InitialBeamLength = 1f;

	public Vector2 texture_animation;

	public bool maintain_texture_size = true;

	public float texture_size = 1f;

	protected LineRenderer lineRenderer;

	public bool updateLineRenderer;

	protected float beamLength = 3f;

	public float BeamLength
	{
		get
		{
			return beamLength;
		}
		set
		{
			beamLength = value;
			UpdateLineRenderer();
		}
	}

	private void Start()
	{
		lineRenderer = Utils.GetComponent<LineRenderer>(this);
		lineRenderer.SetVertexCount(2);
		lineRenderer.SetPosition(0, Vector3.zero);
		BeamLength = InitialBeamLength;
	}

	public void Update()
	{
		if (updateLineRenderer)
		{
			UpdateLineRenderer();
			updateLineRenderer = false;
		}
	}

	protected void UpdateLineRenderer()
	{
		if (lineRenderer != null)
		{
			lineRenderer.SetPosition(1, Vector3.forward * beamLength);
			if (maintain_texture_size)
			{
				Vector2 mainTextureScale = lineRenderer.material.mainTextureScale;
				float y = mainTextureScale.y;
				lineRenderer.material.mainTextureScale = new Vector2(beamLength / texture_size, y);
			}
			if (texture_animation.sqrMagnitude > 0f)
			{
				lineRenderer.material.mainTextureOffset += texture_animation * Time.deltaTime;
			}
		}
	}
}
