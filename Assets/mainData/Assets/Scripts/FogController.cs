using UnityEngine;

public class FogController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public bool fogEnabled;

	public Color fogColor = Color.gray;

	public float fogDensity = 0.01f;

	public float haloStrength = 0.5f;

	public float flareStrength = 1f;

	protected float oldHaloStrength;

	protected float oldFlareStrength;

	private void OnEnable()
	{
		oldHaloStrength = RenderSettings.haloStrength;
		oldFlareStrength = RenderSettings.flareStrength;
		RenderSettings.fog = fogEnabled;
		RenderSettings.fogColor = fogColor;
		RenderSettings.fogDensity = fogDensity;
		RenderSettings.haloStrength = haloStrength;
		RenderSettings.flareStrength = flareStrength;
	}

	private void OnDisable()
	{
		RenderSettings.fog = false;
		RenderSettings.haloStrength = oldHaloStrength;
		RenderSettings.flareStrength = oldFlareStrength;
	}

	private void Update()
	{
	}
}
