using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Image Effects/Color Ramp")]
public class ColorRampEffect : ImageEffectBase
{
	public Color TopColor;

	public Color BottomColor;

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		base.material.SetColor("_TopColor", TopColor);
		base.material.SetColor("_BottomColor", BottomColor);
		Graphics.Blit(source, destination, base.material);
	}
}
