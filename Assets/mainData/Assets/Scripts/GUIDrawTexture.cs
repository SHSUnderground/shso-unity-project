using UnityEngine;

public class GUIDrawTexture : GUIChildControl
{
	private static Texture loadingTexture;

	protected bool isLoading;

	protected float loadingAnimRotation;

	protected float loadingRPS = 140f;

	protected ScaleMode scaleMode;

	protected bool alphaBlend = true;

	protected float imageAspect;

	protected bool autoSizeToTexture;

	protected string textureSource;

	protected Texture texture;

	public ScaleMode ScaleMode
	{
		get
		{
			return scaleMode;
		}
		set
		{
			scaleMode = value;
			if (inspector != null)
			{
				((GUIDrawTextureInspector)inspector).scaleMode = value;
			}
		}
	}

	public bool AlphaBlend
	{
		get
		{
			return alphaBlend;
		}
		set
		{
			alphaBlend = value;
			if (inspector != null)
			{
				((GUIDrawTextureInspector)inspector).alphaBlend = value;
			}
		}
	}

	public float ImageAspect
	{
		get
		{
			return imageAspect;
		}
		set
		{
			imageAspect = value;
			if (inspector != null)
			{
				((GUIDrawTextureInspector)inspector).imageAspect = value;
			}
		}
	}

	public bool AutoSizeToTexture
	{
		get
		{
			return autoSizeToTexture;
		}
		set
		{
			autoSizeToTexture = value;
			if (inspector != null)
			{
				((GUIDrawTextureInspector)inspector).autoSizeToTexture = value;
			}
		}
	}

	public virtual string TextureSource
	{
		get
		{
			return textureSource;
		}
		set
		{
			textureSource = value;
			
			resourcesInitialized = false;  // CSP temporarily commented out.

			if (inspector != null)
			{
				((GUIDrawTextureInspector)inspector).textureSource = value;
			}
		}
	}

	public Texture Texture
	{
		get
		{
			if (isLoading)
			{
				return null;
			}
			if (resourcesInitialized && texture != null)
			{
				return texture;
			}
			
			if (!string.IsNullOrEmpty(textureSource))
			{
				loadTexture();
				if (autoSizeToTexture)
				{
					initTextureSize();
				}
				return texture;
			}
			return null;
		}
		set
		{
			texture = value;
			resourcesInitialized = (texture != null);
			if (resourcesInitialized && autoSizeToTexture)
			{
				initTextureSize();
			}
		}
	}

	public GUIDrawTexture()
	{
		Traits.ResourceLoadingPhaseTrait = ControlTraits.ResourceLoadingPhaseTraitEnum.Manual;
	}

	static GUIDrawTexture()
	{
		if (loadingTexture == null)
		{
			loadingTexture = GUIManager.Instance.LoadTexture("gui/loading/animation_marker");
		}
	}

	protected virtual void loadTexture()
	{
		if (Traits.ResourceLoadingTrait == ControlTraits.ResourceLoadingTraitEnum.Sync)
		{
			texture = GUIManager.Instance.LoadTexture(textureSource);
			if (autoSizeToTexture)
			{
				initTextureSize();
			}
			resourcesInitialized = (texture != null);
		}
		else if (Traits.ResourceLoadingTrait == ControlTraits.ResourceLoadingTraitEnum.Async)
		{
			GUIManager.Instance.LoadTexture(textureSource, OnResourceAsyncLoaded, 0);
			isLoading = true;
			if (inspector != null)
			{
				((GUIDrawTextureInspector)inspector).isLoading = true;
			}
		}
	}

	public override bool InitializeResources(bool reload)
	{
		texture = null;
		resourcesInitialized = false;
		return base.InitializeResources(reload);
	}

	public override void OnLocaleChange(string newLocale)
	{
		base.OnLocaleChange(newLocale);
	}

	protected virtual void OnResourceAsyncLoaded(Object obj, AssetBundle bundle, object extraData)
	{
		texture = (Texture2D)obj;
		resourcesInitialized = true;
		isLoading = false;
		if (inspector != null)
		{
			((GUIDrawTextureInspector)inspector).isLoading = false;
		}
	}

	protected virtual void initTextureSize()
	{
		if (texture != null)
		{
			SetSize(texture.width, texture.height);
		}
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		AlphaOutIfDisabled();
		if (isLoading)
		{
			UpdateLoadingState();
		}
		else if (resourcesInitialized || Texture != null)
		{
			GUI.DrawTexture(base.rect, texture, scaleMode, alphaBlend, imageAspect);
		}
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		if (isLoading)
		{
			loadingAnimRotation += loadingRPS * Time.deltaTime;
			if (inspector != null)
			{
				((GUIDrawTextureInspector)inspector).loadingAnimRotation = loadingAnimRotation;
			}
		}
	}

	protected void UpdateLoadingState()
	{
		Rect position = new Rect(base.rect.width / 2f - (float)(loadingTexture.width / 2) + base.rect.x, base.rect.height / 2f - (float)(loadingTexture.height / 2) + base.rect.y, loadingTexture.width, loadingTexture.height);
		Matrix4x4 matrix = GUI.matrix;
		GUIUtility.RotateAroundPivot(loadingAnimRotation, base.RotationPoint);
		GUI.DrawTexture(position, loadingTexture, scaleMode, alphaBlend, imageAspect);
		GUI.matrix = matrix;
	}

	protected override void AttachInspector()
	{
		inspector = guiTreeNode.AddComponent(typeof(GUIDrawTextureInspector));
	}

	protected override void ReflectToInspector()
	{
		base.ReflectToInspector();
		if (inspector != null)
		{
			((GUIDrawTextureInspector)inspector).isLoading = isLoading;
			((GUIDrawTextureInspector)inspector).loadingAnimRotation = loadingAnimRotation;
			((GUIDrawTextureInspector)inspector).loadingRPS = loadingRPS;
			((GUIDrawTextureInspector)inspector).scaleMode = ScaleMode;
			((GUIDrawTextureInspector)inspector).alphaBlend = AlphaBlend;
			((GUIDrawTextureInspector)inspector).imageAspect = imageAspect;
			((GUIDrawTextureInspector)inspector).autoSizeToTexture = autoSizeToTexture;
			((GUIDrawTextureInspector)inspector).textureSource = textureSource;
			((GUIDrawTextureInspector)inspector).texture = texture;
		}
	}

	protected override void ReflectFromInspector()
	{
		base.ReflectFromInspector();
		if (inspector != null)
		{
			loadingRPS = ((GUIDrawTextureInspector)inspector).loadingRPS;
			scaleMode = ((GUIDrawTextureInspector)inspector).scaleMode;
			alphaBlend = ((GUIDrawTextureInspector)inspector).alphaBlend;
			imageAspect = ((GUIDrawTextureInspector)inspector).imageAspect;
			autoSizeToTexture = ((GUIDrawTextureInspector)inspector).autoSizeToTexture;
			if (texture != ((GUIDrawTextureInspector)inspector).texture)
			{
				Texture = ((GUIDrawTextureInspector)inspector).texture;
			}
		}
	}
}
