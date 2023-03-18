using UnityEngine;

public class FSEColorRamp : FullScreenEffect
{
	private ColorRampEffect colorRampEffect;

	public Color TopColor = new Color(1.1f, 1.1f, 1.1f, 1.1f);

	public Color BottomColor = new Color(1.1f, 1.1f, 1.1f, 0f);

	private void Update()
	{
		if (!(colorRampEffect == null))
		{
			colorRampEffect.TopColor = TopColor;
			colorRampEffect.BottomColor = BottomColor;
		}
	}

	public override void LoadResources()
	{
		if (!(activeCamera == null))
		{
			if (activeCamera.GetComponent(typeof(ColorRampEffect)) == null)
			{
				colorRampEffect = (activeCamera.gameObject.AddComponent(typeof(ColorRampEffect)) as ColorRampEffect);
			}
			colorRampEffect.shader = Shader.Find("Hidden/ColorRamp Effect");
			if (colorRampEffect.shader == null)
			{
				CspUtils.DebugLog("FSEColorRamp.LoadResources() error - Unable to load shader !");
			}
		}
	}

	public override void ReleaseResources()
	{
		if (colorRampEffect != null)
		{
			Object.Destroy(colorRampEffect);
			colorRampEffect = null;
		}
	}
}
