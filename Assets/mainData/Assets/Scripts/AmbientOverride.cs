using UnityEngine;

[ExecuteInEditMode]
public class AmbientOverride : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public Color ambientColor = new Color(0.5f, 0.5f, 0.5f);

	protected Color currentColor = Color.white;

	public void Start()
	{
		UpdateAmbientColor(ambientColor);
	}

	public void Update()
	{
		if (currentColor != ambientColor)
		{
			UpdateAmbientColor(ambientColor);
		}
	}

	protected void UpdateAmbientColor(Color c)
	{
		currentColor = c;
		RenderSettings.ambientLight = c;
	}
}
