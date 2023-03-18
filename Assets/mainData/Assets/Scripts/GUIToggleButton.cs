using UnityEngine;

public class GUIToggleButton : GUIControlWindow
{
	public class InternalToggleButton : GUIButton
	{
		private GUIToggleButton headWindow;

		public InternalToggleButton(GUIToggleButton headWindow)
		{
			this.headWindow = headWindow;
			StyleInfo = new SHSButtonStyleInfo("common_bundle|checkbox");
			SetSize(new Vector2(36f, 36f));
		}

		public override void Draw(DrawModeSetting drawFlags)
		{
			if (headWindow.Value)
			{
				GUI.DrawTexture(base.rect, Style.UnityStyle.active.background, ScaleMode.StretchToFill);
			}
			else if (Hover && headWindow.SupportsHover)
			{
				GUI.DrawTexture(base.rect, Style.UnityStyle.hover.background, ScaleMode.StretchToFill);
			}
			else
			{
				GUI.DrawTexture(base.rect, Style.UnityStyle.normal.background, ScaleMode.StretchToFill);
			}
		}
	}

	public delegate void ChangedEventDelegate(GUIControl sender, GUIChangedEvent eventData);

	protected bool value;

	protected bool supportsHover;

	public InternalToggleButton toggleButton;

	private GUILabel toggleButtonLabel;

	private float spacing;

	private float vspacing;

	public bool Value
	{
		get
		{
			return value;
		}
		set
		{
			if (this.value != value)
			{
				this.value = value;
				if (this.Changed != null)
				{
					this.Changed(this, new GUIChangedEvent((!value) ? 1 : 0, value ? 1 : 0));
				}
			}
		}
	}

	public override bool IsEnabled
	{
		get
		{
			return base.IsEnabled;
		}
		set
		{
			toggleButton.IsEnabled = value;
			toggleButtonLabel.IsEnabled = value;
			base.IsEnabled = value;
		}
	}

	public bool SupportsHover
	{
		get
		{
			return supportsHover;
		}
		set
		{
			supportsHover = true;
		}
	}

	public GUILabel Label
	{
		get
		{
			return toggleButtonLabel;
		}
	}

	public SHSStyleInfo ButtonStyleInfo
	{
		get
		{
			return toggleButton.StyleInfo;
		}
		set
		{
			toggleButton.StyleInfo = value;
		}
	}

	public SHSStyleInfo TextStyleInfo
	{
		get
		{
			return toggleButtonLabel.StyleInfo;
		}
		set
		{
			toggleButtonLabel.StyleInfo = value;
		}
	}

	public string Text
	{
		get
		{
			return toggleButtonLabel.Text;
		}
		set
		{
			toggleButtonLabel.Text = value;
		}
	}

	public object Tag
	{
		get
		{
			return toggleButton.Tag;
		}
		set
		{
			toggleButton.Tag = value;
		}
	}

	public float Spacing
	{
		get
		{
			return spacing;
		}
		set
		{
			spacing = value;
		}
	}

	public float VertSpacing
	{
		get
		{
			return vspacing;
		}
		set
		{
			vspacing = value;
		}
	}

	public event ChangedEventDelegate Changed;

	public GUIToggleButton()
	{
		spacing = 5f;
		vspacing = 0f;
		toggleButton = new InternalToggleButton(this);
		toggleButton.HitTestType = HitTestTypeEnum.Rect;
		toggleButton.SetPosition(DockingAlignmentEnum.TopLeft, AnchorAlignmentEnum.TopLeft);
		toggleButton.Click += OnClickDelegate;
		Add(toggleButton);
		toggleButtonLabel = new GUILabel();
		toggleButtonLabel.SetupText(GUIFontManager.SupportedFontEnum.Komica, 12, Color.white, TextAnchor.UpperLeft);
		toggleButtonLabel.SetPosition(DockingAlignmentEnum.MiddleLeft, AnchorAlignmentEnum.MiddleLeft);
		Add(toggleButtonLabel);
	}

	public void OnClickDelegate(GUIControl sender, GUIClickEvent eventArgs)
	{
		Value = !Value;
	}

	public void SetButtonSize(Vector2 buttonSize)
	{
		toggleButton.SetSize(buttonSize);
	}

	public override void HandleResize(GUIResizeMessage message)
	{
		base.HandleResize(message);
		toggleButtonLabel.Offset = new Vector2(spacing, vspacing);
		Vector2 size = toggleButtonLabel.Style.UnityStyle.CalcSize(new GUIContent(Text));
		toggleButtonLabel.SetSize(size);
	}
}
