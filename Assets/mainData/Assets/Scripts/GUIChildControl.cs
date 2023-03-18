using UnityEngine;

public class GUIChildControl : GUIControl
{
	protected GUIContent content;

	protected string text = string.Empty;

	protected string textId = string.Empty;

	protected object tag = string.Empty;

	protected Texture2D image;

	protected string controlName;

	public GUIContent Content
	{
		get
		{
			if (content != null)
			{
				return content;
			}
			content = new GUIContent(text, image, tooltipKey);
			return content;
		}
		set
		{
			content = value;
		}
	}

	public virtual string Text
	{
		get
		{
			return text;
		}
		set
		{
			textId = value;
			text = AppShell.Instance.stringTable[value];
			if (inspector != null)
			{
				((GUIChildControlInspector)inspector).text = textId;
			}
			content = null;
		}
	}

	public object Tag
	{
		get
		{
			return tag;
		}
		set
		{
			tag = value;
		}
	}

	public Texture2D Image
	{
		get
		{
			return image;
		}
		set
		{
			image = value;
			content = null;
			if (inspector != null)
			{
				((GUIChildControlInspector)inspector).image = image;
			}
		}
	}

	public string ControlName
	{
		get
		{
			return controlName;
		}
		set
		{
			controlName = value;
			if (inspector != null)
			{
				((GUIChildControlInspector)inspector).controlName = controlName;
			}
		}
	}

	public GUIChildControl()
		: base(ControlTraits.ChildDefault)
	{
		cachedVisible = true;
		isVisible = true;
	}

	protected override void AttachInspector()
	{
		inspector = guiTreeNode.AddComponent(typeof(GUIChildControlInspector));
	}

	protected override void ReflectToInspector()
	{
		base.ReflectToInspector();
		if (inspector != null)
		{
			((GUIChildControlInspector)inspector).text = Text;
			((GUIChildControlInspector)inspector).image = Image;
			((GUIChildControlInspector)inspector).controlName = ControlName;
		}
	}

	protected override void ReflectFromInspector()
	{
		base.ReflectFromInspector();
		if (inspector != null)
		{
			if (!text.Equals(((GUIChildControlInspector)inspector).text))
			{
				Text = ((GUIChildControlInspector)inspector).text;
			}
			Image = ((GUIChildControlInspector)inspector).image;
			ControlName = ((GUIChildControlInspector)inspector).controlName;
		}
	}

	public override void OnLocaleChange(string newLocale)
	{
		Text = textId;
		base.OnLocaleChange(newLocale);
	}
}
