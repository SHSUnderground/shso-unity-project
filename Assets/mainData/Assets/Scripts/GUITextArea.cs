using UnityEngine;

public class GUITextArea : GUITextControlBase
{
	public delegate void ChangedEventDelegate(GUIControl sender, GUIChangedEvent eventData);

	private string cacheText;

	protected int maxLength = int.MaxValue;

	public int MaxLength
	{
		get
		{
			return maxLength;
		}
		set
		{
			maxLength = value;
			if (text != null)
			{
				text = text.Substring(0, maxLength - 1);
			}
		}
	}

	public event ChangedEventDelegate Changed;

	public override void Draw(DrawModeSetting drawFlags)
	{
		cacheText = text;
		if (Style != SHSStyle.NoStyle)
		{
			text = GUI.TextArea(base.rect, text, Style.UnityStyle);
		}
		else
		{
			text = GUI.TextArea(base.rect, text);
		}
		if (text != cacheText && this.Changed != null)
		{
			this.Changed(this, new GUIChangedEvent());
		}
	}
}
