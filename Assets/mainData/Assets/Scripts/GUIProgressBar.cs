using UnityEngine;

public class GUIProgressBar : GUISimpleControlWindow
{
	public class FillWindow : GUISimpleControlWindow
	{
		public override void HandleResize(GUIResizeMessage message)
		{
			base.HandleResize(message);
		}
	}

	public class BarLogic : SHSBaseLogicBar
	{
		public float windowMax;

		public float windowMin;

		public float windowHeight;

		public float barHeight;

		public override void OnUpdate()
		{
			float percentage = GetPercentage();
			float adjustedWidth = getAdjustedWidth(percentage);
			GUIControl gUIControl = Parent as GUIControl;
			if (gUIControl != null)
			{
				gUIControl.SetSize(adjustedWidth, barHeight);
			}
		}

		private float getAdjustedWidth(float percentage)
		{
			return windowMin + (windowMax - windowMin) * percentage;
		}
	}

	protected BarLogic barLogic;

	protected GUIImage barBackground;

	protected GUIImage foregroundImage;

	protected FillWindow fillWindow;

	protected GUIImage nubImage;

	private string backgroundTextureSource = string.Empty;

	private string foregroundTextureSource = string.Empty;

	protected Vector2 backgroundSize;

	protected Vector2 foregroundSize;

	protected float foregroundMin;

	protected float foregroundMax = 800f;

	protected float barHeight = 30f;

	private float updateSpeed = 3000f;

	public string BackgroundTexture
	{
		set
		{
			backgroundTextureSource = value;
		}
	}

	public string ForegroundTexture
	{
		set
		{
			foregroundTextureSource = value;
		}
	}

	public Vector2 BackgroundSize
	{
		get
		{
			return backgroundSize;
		}
		set
		{
			backgroundSize = value;
		}
	}

	public Vector2 ForegroundSize
	{
		get
		{
			return foregroundSize;
		}
		set
		{
			foregroundSize = value;
			if (barLogic != null)
			{
				barLogic.windowMax = foregroundSize.x;
				barLogic.barHeight = foregroundSize.y;
			}
		}
	}

	public float ForegroundMin
	{
		get
		{
			return foregroundMin;
		}
		set
		{
			foregroundMin = value;
			if (barLogic != null)
			{
				barLogic.windowMin = value;
			}
		}
	}

	public float ForegroundMax
	{
		get
		{
			return foregroundMax;
		}
		set
		{
			foregroundMax = value;
			if (barLogic != null)
			{
				barLogic.windowMax = value;
			}
		}
	}

	public float BarHeight
	{
		get
		{
			return barHeight;
		}
		set
		{
			barHeight = value;
			if (barLogic != null)
			{
				barLogic.barHeight = value;
			}
		}
	}

	public virtual float Value
	{
		get
		{
			if (barLogic != null)
			{
				return barLogic.Value;
			}
			return 0f;
		}
		set
		{
			if (barLogic != null)
			{
				barLogic.windowMin = foregroundMin;
				barLogic.windowMax = foregroundMax;
				barLogic.barHeight = barHeight;
				barLogic.Value = value;
			}
		}
	}

	public virtual float CurrentValue
	{
		get
		{
			if (barLogic != null)
			{
				return barLogic.CurrentValue;
			}
			return 0f;
		}
	}

	public virtual float UpdateSpeed
	{
		get
		{
			if (barLogic != null)
			{
				return barLogic.UpdateSpeed;
			}
			return updateSpeed;
		}
		set
		{
			if (barLogic != null)
			{
				barLogic.UpdateSpeed = value;
			}
			updateSpeed = value;
		}
	}

	public BarLogic GetBarLogic()
	{
		return barLogic;
	}

	public override void SetSize(float width, float height)
	{
		foregroundMin = 0f;
		foregroundMax = width;
		if (barLogic != null)
		{
			barLogic.windowMin = 0f;
			barLogic.windowMax = width;
			barLogic.barHeight = height;
		}
		base.SetSize(width, height);
	}

	public override bool InitializeResources(bool reload)
	{
		barBackground = new GUIImage();
		barBackground.Id = "barBackground";
		barBackground.SetSize(ForegroundMax, BarHeight);
		barBackground.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft);
		barBackground.TextureSource = ((!(backgroundTextureSource == string.Empty)) ? backgroundTextureSource : string.Empty);
		fillWindow = new FillWindow();
		fillWindow.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft);
		fillWindow.Id = "fillWindow";
		foregroundImage = new GUIImage();
		foregroundImage.Id = "foregroundImage";
		foregroundImage.SetSize(ForegroundMax, BarHeight);
		foregroundImage.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft);
		foregroundImage.TextureSource = ((!(foregroundTextureSource == string.Empty)) ? foregroundTextureSource : "gui/loading/bar_filled");
		barLogic = new BarLogic();
		barLogic.Id = "barLogic";
		barLogic.UpdateSpeed = updateSpeed;
		barLogic.InitializeValue(0f);
		barLogic.windowMax = foregroundMax;
		barLogic.windowMin = foregroundMin;
		Add(barBackground);
		fillWindow.Add(foregroundImage);
		fillWindow.Add(barLogic);
		Add(fillWindow);
		return base.InitializeResources(reload);
	}
}
