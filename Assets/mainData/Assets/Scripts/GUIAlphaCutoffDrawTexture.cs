using UnityEngine;

public class GUIAlphaCutoffDrawTexture : GUIDrawTexture
{
	private string alphaCutoffTextureSource;

	private bool alphaCutoffTextureLoaded;

	public Material mat;

	private float alphaCutoff;

	public string AlphaCutoffTextureSource
	{
		get
		{
			return alphaCutoffTextureSource;
		}
		set
		{
			alphaCutoffTextureSource = value;
			alphaCutoffTextureLoaded = false;
		}
	}

	public float AlphaCutoff
	{
		get
		{
			return alphaCutoff;
		}
		set
		{
			mat.SetFloat("_Cutoff", value);
			alphaCutoff = value;
		}
	}

	public GUIAlphaCutoffDrawTexture()
	{
		//mat = new Material(Shader.Find("GUI/DualCutout"));
		Shader s = Shader.Find("Specular");
		if (s == null)
			CspUtils.DebugLog("GUIalpha - shader find returns null!");
		mat = new Material(s);  // CSP seems to be a problem with the above line, just use Standard for now.
		AlphaCutoff = 0f;
	}

	public void AnimateCutoff(float x)
	{
		AlphaCutoff = x;
	}

	private void LoadAlphaCutoffTexture()
	{
		Texture texture = GUIManager.Instance.LoadTexture(AlphaCutoffTextureSource);
		mat.SetTexture("_AlphaTex", texture);
		alphaCutoffTextureLoaded = true;
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		if (!alphaCutoffTextureLoaded)
		{
			LoadAlphaCutoffTexture();
		}
		if ((resourcesInitialized || base.Texture != null) && Event.current.type == EventType.Repaint)
		{
			Graphics.DrawTexture(base.rect, base.Texture, mat);
		}
	}
}
