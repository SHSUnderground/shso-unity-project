using UnityEngine;

public class GUITextField : GUITextControlBase, ICaptureHandler
{
	public delegate void ChangedEventDelegate(GUIControl sender, GUIChangedEvent eventData);

	public delegate void EnterKeyPressedDelegate(GUIControl sender);

	public delegate void TextFieldGotFocus(GUIControl sender, GUIChangedEvent eventData);

	public Color NoColor = new Color(-1f, -1f, -1f);

	private Color backgroundColor;

	private Texture2D textureBgColor;

	private bool clipboardCopyEnabled;

	protected bool passwordFieldEnabled;

	protected string passwordField = string.Empty;

	protected int maxLength = int.MaxValue;

	protected string cacheText = string.Empty;

	protected char maskCharacter = "*".ToCharArray()[0];

	public Color BackgroundColor
	{
		get
		{
			return backgroundColor;
		}
		set
		{
			backgroundColor = value;
			if (inspector != null)
			{
				((GUITextFieldInspector)inspector).backgroundColor = backgroundColor;
			}
		}
	}

	public bool ClipboardCopyEnabled
	{
		get
		{
			return clipboardCopyEnabled;
		}
		set
		{
			clipboardCopyEnabled = value;
			if (inspector != null)
			{
				((GUITextFieldInspector)inspector).ClipboardCopyEnabled = value;
			}
		}
	}

	public bool PasswordFieldEnabled
	{
		get
		{
			return passwordFieldEnabled;
		}
		set
		{
			passwordFieldEnabled = value;
			if (inspector != null)
			{
				((GUITextFieldInspector)inspector).passwordFieldEnabled = passwordFieldEnabled;
			}
		}
	}

	public int MaxLength
	{
		get
		{
			return maxLength;
		}
		set
		{
			maxLength = value;
			if (inspector != null)
			{
				((GUITextFieldInspector)inspector).maxLength = maxLength;
			}
		}
	}

	public char MaskCharacter
	{
		get
		{
			return maskCharacter;
		}
		set
		{
			maskCharacter = value;
			if (inspector != null)
			{
				((GUITextFieldInspector)inspector).maskCharacter = maskCharacter;
			}
		}
	}

	public event ChangedEventDelegate Changed;

	public event EnterKeyPressedDelegate OnEnter;

	public event TextFieldGotFocus GotFocus;

	public event TextFieldGotFocus LostFocus;

	public GUITextField()
	{
		backgroundColor = NoColor;
		ClipboardCopyEnabled = false;
	}

	public void FireFocusEvent(GUIChangedEvent eventData)
	{
		SHSInput.RegisterKey(new SHSKeyCode(KeyCode.Escape), onEscape, this);
		if (this.GotFocus != null)
		{
			this.GotFocus(this, eventData);
		}
	}

	public void FireLostFocusEvent(GUIChangedEvent eventData)
	{
		SHSInput.UnregisterKey(new SHSKeyCode(KeyCode.Escape), this);
		if (this.LostFocus != null)
		{
			this.LostFocus(this, eventData);
		}
	}

	private void onEscape(SHSKeyCode code)
	{
		GUIManager.Instance.FocusManager.mouseIsClicked(code);
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		GUIFocusManager focusManager = GUIManager.Instance.FocusManager;
		if (GUIFocusManager.mouseClicked && base.rect.Contains(Event.current.mousePosition))
		{
			focusManager.getFocus(this);
		}
		if (BackgroundColor != NoColor)
		{
			Color color = GUI.color;
			GUI.color = BackgroundColor;
			if (textureBgColor == null)
			{
				GUIManager.Instance.LoadTexture("common_bundle|white2x2", out textureBgColor);
			}
			else
			{
				GUI.DrawTexture(base.rect, textureBgColor);
			}
			GUI.color = color;
		}
		if (!string.IsNullOrEmpty(controlName))
		{
			GUI.SetNextControlName(controlName);
		}
		bool flag = false;
		if (passwordFieldEnabled)
		{
			if (Event.current.type == EventType.Repaint || Event.current.type == EventType.MouseDown)
			{
				passwordField = string.Empty;
				for (int i = 0; i < text.Length; i++)
				{
					passwordField += maskCharacter;
				}
			}
			else
			{
				passwordField = text;
			}
			GUI.changed = false;
			cacheText = text;
			if (Style == SHSStyle.NoStyle)
			{
				passwordField = GUI.TextField(base.rect, passwordField, maxLength);
			}
			else
			{
				passwordField = GUI.TextField(base.rect, passwordField, maxLength, Style.UnityStyle);
			}
			if (GUI.changed)
			{
				text = passwordField;
				flag = true;
			}
		}
		else
		{
			cacheText = text;
			if (Style == SHSStyle.NoStyle)
			{
				text = GUI.TextField(base.rect, text, maxLength);
			}
			else
			{
				text = GUI.TextField(base.rect, text, maxLength, Style.UnityStyle);
			}
		}
		if (flag || cacheText != text)
		{
			GUIManager.Instance.FocusManager.getFocus(this);
		}
		bool flag2 = text.EndsWith("\n") || text.EndsWith("\r");
		if (cacheText != text)
		{
			text = text.Replace("\n", string.Empty);
			text = text.Replace("\r", string.Empty);
			text = text.Replace("\t", string.Empty);
		}
		if (flag2 || !(cacheText == text))
		{
			if (!ClipboardCopyEnabled && (Event.current.command || Event.current.control))
			{
				text = cacheText;
			}
			else if (this.Changed != null)
			{
				this.Changed(this, new GUIChangedEvent());
			}
			if (flag2 && this.OnEnter != null)
			{
				this.OnEnter(this);
			}
		}
	}

	protected override void AttachInspector()
	{
		inspector = guiTreeNode.AddComponent(typeof(GUITextFieldInspector));
	}

	protected override void ReflectToInspector()
	{
		base.ReflectToInspector();
		if (inspector != null)
		{
			((GUITextFieldInspector)inspector).backgroundColor = backgroundColor;
			((GUITextFieldInspector)inspector).passwordFieldEnabled = passwordFieldEnabled;
			((GUITextFieldInspector)inspector).maxLength = maxLength;
			((GUITextFieldInspector)inspector).cacheText = cacheText;
			((GUITextFieldInspector)inspector).maskCharacter = maskCharacter;
			((GUITextFieldInspector)inspector).ClipboardCopyEnabled = ClipboardCopyEnabled;
		}
	}

	protected override void ReflectFromInspector()
	{
		base.ReflectFromInspector();
		if (inspector != null)
		{
			backgroundColor = ((GUITextFieldInspector)inspector).backgroundColor;
			passwordFieldEnabled = ((GUITextFieldInspector)inspector).passwordFieldEnabled;
			maxLength = ((GUITextFieldInspector)inspector).maxLength;
			cacheText = ((GUITextFieldInspector)inspector).cacheText;
			maskCharacter = ((GUITextFieldInspector)inspector).maskCharacter;
			clipboardCopyEnabled = ((GUITextFieldInspector)inspector).ClipboardCopyEnabled;
		}
	}

	public override CaptureHandlerResponse HandleCapture(SHSKeyCode code)
	{
		if (GUIManager.Instance.FocusManager.PassthroughAllowed(code) || code.source == this)
		{
			return CaptureHandlerResponse.Passthrough;
		}
		return CaptureHandlerResponse.Block;
	}

	public override void OnCaptureAcquired()
	{
	}

	public override void OnCaptureUnacquired()
	{
	}
}
