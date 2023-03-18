using UnityEngine;

public class GUIAnimatedButton : GUIButton
{
	public enum HoverStates
	{
		Normal,
		Highlight,
		Pressed
	}

	public delegate AnimPath ToNormalPath(float CurrentPercentage, float NormalPercentage);

	public delegate AnimPath ToHighlightPath(float CurrentPercentage, float HighlightPercentage);

	public delegate AnimPath ToPressedPath(float CurrentPercentage, float PressedPercentage);

	public delegate void ButtonStateChangedDelegate(HoverStates newState);

	private Vector2 currentPercentage = new Vector2(1f, 1f);

	public float TransitionTime = 0.1f;

	private Vector2 normalPercentage;

	private Vector2 highlightPercentage;

	private Vector2 pressedPercentage;

	private ToNormalPath normalPathX;

	private ToHighlightPath highlightPathX;

	private ToPressedPath pressedPathX;

	private ToNormalPath normalPathY;

	private ToHighlightPath highlightPathY;

	private ToPressedPath pressedPathY;

	private HoverStates currentState;

	private HoverStates StateToAfterMouseUp;

	private AnimClip curAnim;

	protected string textureSource;

	protected Texture2D texture;

	public Vector2 NormalPercentage
	{
		get
		{
			return normalPercentage;
		}
		set
		{
			normalPercentage = value;
		}
	}

	public Vector2 HighlightPercentage
	{
		get
		{
			return highlightPercentage;
		}
		set
		{
			highlightPercentage = value;
		}
	}

	public Vector2 PressedPercentage
	{
		get
		{
			return pressedPercentage;
		}
		set
		{
			pressedPercentage = value;
		}
	}

	public ToNormalPath NormalPath
	{
		set
		{
			normalPathX = value;
			normalPathY = value;
		}
	}

	public ToHighlightPath HighlightPath
	{
		set
		{
			highlightPathX = value;
			highlightPathY = value;
		}
	}

	public ToPressedPath PressedPath
	{
		set
		{
			pressedPathX = value;
			pressedPathY = value;
		}
	}

	public ToNormalPath NormalPathX
	{
		get
		{
			return normalPathX;
		}
		set
		{
			normalPathX = value;
		}
	}

	public ToHighlightPath HighlightPathX
	{
		get
		{
			return highlightPathX;
		}
		set
		{
			highlightPathX = value;
		}
	}

	public ToPressedPath PressedPathX
	{
		get
		{
			return pressedPathX;
		}
		set
		{
			pressedPathX = value;
		}
	}

	public ToNormalPath NormalPathY
	{
		get
		{
			return normalPathY;
		}
		set
		{
			normalPathY = value;
		}
	}

	public ToHighlightPath HighlightPathY
	{
		get
		{
			return highlightPathY;
		}
		set
		{
			highlightPathY = value;
		}
	}

	public ToPressedPath PressedPathY
	{
		get
		{
			return pressedPathY;
		}
		set
		{
			pressedPathY = value;
		}
	}

	private HoverStates CurrentState
	{
		get
		{
			return currentState;
		}
		set
		{
			currentState = value;
			if (this.OnButtonStateChanged != null)
			{
				this.OnButtonStateChanged(currentState);
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
			resourcesInitialized = false;
		}
	}

	public Texture2D Texture
	{
		get
		{
			if (resourcesInitialized && texture != null)
			{
				return texture;
			}
			if (!string.IsNullOrEmpty(textureSource))
			{
				loadTexture();
				return texture;
			}
			return null;
		}
		set
		{
			texture = value;
			resourcesInitialized = (texture != null);
		}
	}

	public event ButtonStateChangedDelegate OnButtonStateChanged;

	public GUIAnimatedButton()
	{
		Traits.ResourceLoadingPhaseTrait = ControlTraits.ResourceLoadingPhaseTraitEnum.Manual;
		MouseOver += GUIAnimatedButton_MouseOver;
		MouseOut += GUIAnimatedButton_MouseOut;
		MouseDown += GUIAnimatedButton_MouseDown;
		MouseUp += GUIAnimatedButton_MouseUp;
		NormalPath = delegate(float CP, float NP)
		{
			return AnimClipBuilder.Path.Linear(CP, NP, TransitionTime);
		};
		HighlightPath = delegate(float CP, float HP)
		{
			return AnimClipBuilder.Path.Linear(CP, HP, TransitionTime);
		};
		PressedPath = delegate(float CP, float PP)
		{
			return AnimClipBuilder.Path.Linear(CP, PP, TransitionTime);
		};
		NormalPercentage = new Vector2(1f, 1f);
		HighlightPercentage = new Vector2(1f, 1f);
		PressedPercentage = new Vector2(1f, 1f);
	}

	public void SetupButton(float NormalPercentage, float HighlightPercentage, float PressedPercentage)
	{
		SetPercOfButton(NormalPercentage, HighlightPercentage, PressedPercentage);
		currentPercentage = this.NormalPercentage;
	}

	public void SetupButton(Vector2 NormalPercentage, Vector2 HighlightPercentage, Vector2 PressedPercentage)
	{
		SetPercOfButton(NormalPercentage, HighlightPercentage, PressedPercentage);
		currentPercentage = NormalPercentage;
	}

	public void AnimSetupButton(float NormalPercentage, float HighlightPercentage, float PressedPercentage)
	{
		SetPercOfButton(NormalPercentage, HighlightPercentage, PressedPercentage);
		GoToCurrentState();
	}

	public void AnimSetupButton(Vector2 NormalPercentage, Vector2 HighlightPercentage, Vector2 PressedPercentage)
	{
		SetPercOfButton(NormalPercentage, HighlightPercentage, PressedPercentage);
		GoToCurrentState();
	}

	private void SetPercOfButton(float NormalPercentage, float HighlightPercentage, float PressedPercentage)
	{
		this.NormalPercentage = new Vector2(NormalPercentage, NormalPercentage);
		this.HighlightPercentage = new Vector2(HighlightPercentage, HighlightPercentage);
		this.PressedPercentage = new Vector2(PressedPercentage, PressedPercentage);
	}

	private void SetPercOfButton(Vector2 NormalPercentage, Vector2 HighlightPercentage, Vector2 PressedPercentage)
	{
		this.NormalPercentage = NormalPercentage;
		this.HighlightPercentage = HighlightPercentage;
		this.PressedPercentage = PressedPercentage;
	}

	public void LinkToSourceButton(GUIControl source)
	{
		source.MouseOver += delegate(GUIControl sender, GUIMouseEvent EventData)
		{
			FireMouseOver(EventData);
		};
		source.MouseOut += delegate(GUIControl sender, GUIMouseEvent EventData)
		{
			FireMouseOut(EventData);
		};
		source.MouseUp += delegate(GUIControl sender, GUIMouseEvent EventData)
		{
			FireMouseUp(EventData);
		};
		source.MouseDown += delegate(GUIControl sender, GUIMouseEvent EventData)
		{
			FireMouseDown(EventData);
		};
	}

	public override void OnShow()
	{
		base.OnShow();
		currentPercentage = NormalPercentage;
		CurrentState = HoverStates.Normal;
	}

	private void GUIAnimatedButton_MouseOut(GUIControl sender, GUIMouseEvent EventData)
	{
		StateToAfterMouseUp = HoverStates.Normal;
		if (CurrentState != HoverStates.Pressed)
		{
			CurrentState = HoverStates.Normal;
			GoToCurrentState();
		}
	}

	private void GUIAnimatedButton_MouseOver(GUIControl sender, GUIMouseEvent EventData)
	{
		StateToAfterMouseUp = HoverStates.Highlight;
		if (CurrentState != HoverStates.Pressed)
		{
			CurrentState = HoverStates.Highlight;
			GoToCurrentState();
		}
	}

	private void GUIAnimatedButton_MouseDown(GUIControl sender, GUIMouseEvent EventData)
	{
		CurrentState = HoverStates.Pressed;
		GoToCurrentState();
	}

	private void GUIAnimatedButton_MouseUp(GUIControl sender, GUIMouseEvent EventData)
	{
		CurrentState = StateToAfterMouseUp;
		GoToCurrentState();
	}

	public void GoToCurrentState()
	{
		GUIWindow gUIWindow = Parent as GUIWindow;
		if (gUIWindow == null)
		{
			return;
		}
		switch (CurrentState)
		{
		case HoverStates.Normal:
			if (normalPathX != null && normalPathY != null)
			{
				gUIWindow.AnimationPieceManager.SwapOut(ref curAnim, AnimClipBuilder.Custom.Function(normalPathX(currentPercentage.x, normalPercentage.x), ModPercX) ^ AnimClipBuilder.Custom.Function(normalPathY(currentPercentage.y, normalPercentage.y), ModPercY));
			}
			break;
		case HoverStates.Highlight:
			if (highlightPathX != null && highlightPathY != null)
			{
				gUIWindow.AnimationPieceManager.SwapOut(ref curAnim, AnimClipBuilder.Custom.Function(highlightPathX(currentPercentage.x, highlightPercentage.x), ModPercX) ^ AnimClipBuilder.Custom.Function(highlightPathY(currentPercentage.y, highlightPercentage.y), ModPercY));
			}
			break;
		case HoverStates.Pressed:
			if (pressedPathX != null && pressedPathY != null)
			{
				gUIWindow.AnimationPieceManager.SwapOut(ref curAnim, AnimClipBuilder.Custom.Function(pressedPathX(currentPercentage.x, pressedPercentage.x), ModPercX) ^ AnimClipBuilder.Custom.Function(pressedPathY(currentPercentage.y, pressedPercentage.y), ModPercY));
			}
			break;
		}
	}

	public void ModPercX(float x)
	{
		currentPercentage.x = x;
	}

	public void ModPercY(float y)
	{
		currentPercentage.y = y;
	}

	public override bool InitializeResources(bool reload)
	{
		texture = null;
		resourcesInitialized = false;
		return base.InitializeResources(reload);
	}

	public override void DrawPreprocess()
	{
		if (Traits.HitTestType == HitTestTypeEnum.Alpha && Mask == null && Texture != null)
		{
			SetMask(Texture);
		}
		base.DrawPreprocess();
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		AlphaOutIfDisabled();
		if (resourcesInitialized || Texture != null)
		{
			GUI.DrawTexture(PercAdjRect(), texture);
		}
	}

	private Rect PercAdjRect()
	{
		Vector2 b = new Vector2(size.x * currentPercentage.x, size.y * currentPercentage.y);
		Vector2 vector = Size - b;
		return new Rect(base.rect.x + vector.x * 0.5f, base.rect.y + vector.y * 0.5f, b.x, b.y);
	}

	protected virtual void loadTexture()
	{
		texture = GUIManager.Instance.LoadTexture(textureSource);
		resourcesInitialized = (texture != null);
	}
}
