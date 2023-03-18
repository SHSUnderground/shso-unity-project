using UnityEngine;

public class GUIButton : GUIChildControl
{
	private GUIStyle textStyle;

	protected SHSButtonStyleInfo.SupportedStatesEnum supportedStates = SHSButtonStyleInfo.SupportedStatesEnum.Normal | SHSButtonStyleInfo.SupportedStatesEnum.Highlight | SHSButtonStyleInfo.SupportedStatesEnum.Pressed;

	protected bool isSelected;

	private Entitlements.EntitlementFlagEnum? entitlementFlag;

	public bool IsSelected
	{
		get
		{
			return isSelected;
		}
		set
		{
			isSelected = value;
			if (inspector != null)
			{
				((GUIButtonInspector)inspector).isSelected = value;
			}
		}
	}

	public Entitlements.EntitlementFlagEnum? EntitlementFlag
	{
		get
		{
			return entitlementFlag;
		}
		set
		{
			entitlementFlag = value;
			if (inspector != null)
			{
				((GUIButtonInspector)inspector).entitlementFlag = value;
			}
		}
	}

	public override bool IsEnabled
	{
		get
		{
			bool flag = true;
			if (entitlementFlag.HasValue)
			{
				flag = (AppShell.Instance == null || (AppShell.Instance != null && AppShell.Instance.Profile == null) || Singleton<Entitlements>.instance.PermissionCheck(entitlementFlag.Value));
			}
			if (flag)
			{
				flag &= base.IsEnabled;
			}
			if (ToolTip != null)
			{
				ToolTip.Context.ContextStatus = ((!flag) ? GUIContext.Status.Disabled : GUIContext.Status.Default);
			}
			return flag;
		}
		set
		{
			base.IsEnabled = value;
			if (inspector != null)
			{
				((GUIButtonInspector)inspector).isEnabled = value;
			}
		}
	}

	public override SHSStyleInfo StyleInfo
	{
		get
		{
			return styleInfo;
		}
		set
		{
			if (value is SHSButtonStyleInfo)
			{
				supportedStates = ((SHSButtonStyleInfo)value).SupportedStates;
			}
			styleInfo = value;
			style = null;
			if (inspector != null)
			{
				((GUIButtonInspector)inspector).styleInfo = ((styleInfo != null) ? styleInfo.ToString() : null);
			}
		}
	}

	protected GUIStyle getTextStyle()
	{
		if (textStyle != null)
		{
			return textStyle;
		}
		if (Style == SHSStyle.NoStyle)
		{
			return null;
		}
		textStyle = new GUIStyle(Style.UnityStyle);
		textStyle.active.background = null;
		textStyle.normal.background = null;
		textStyle.hover.background = null;
		return textStyle;
	}

	public override bool InitializeResources(bool reload)
	{
		style = null;
		textStyle = null;
		return base.InitializeResources(reload);
	}

	public override void Draw(DrawModeSetting drawFlags)
	{
		AlphaOutIfDisabled();
		if (isSelected)
		{
			if ((supportedStates & SHSButtonStyleInfo.SupportedStatesEnum.Pressed) != 0)
			{
				GUI.DrawTexture(base.rect, (Style == SHSStyle.NoStyle) ? GUI.skin.button.active.background : Style.UnityStyle.active.background);
			}
			else if ((supportedStates & SHSButtonStyleInfo.SupportedStatesEnum.Normal) != 0)
			{
				GUI.DrawTexture(base.rect, (Style == SHSStyle.NoStyle) ? GUI.skin.button.normal.background : Style.UnityStyle.normal.background);
			}
			else if ((supportedStates & SHSButtonStyleInfo.SupportedStatesEnum.Highlight) != 0)
			{
				GUI.DrawTexture(base.rect, (Style == SHSStyle.NoStyle) ? GUI.skin.button.hover.background : Style.UnityStyle.hover.background);
			}
			return;
		}
		if (isDownState)
		{
			if ((supportedStates & SHSButtonStyleInfo.SupportedStatesEnum.Pressed) != 0)
			{
				GUI.DrawTexture(base.rect, (Style == SHSStyle.NoStyle) ? GUI.skin.button.active.background : Style.UnityStyle.active.background);
			}
			else if ((supportedStates & SHSButtonStyleInfo.SupportedStatesEnum.Highlight) != 0)
			{
				GUI.DrawTexture(base.rect, (Style == SHSStyle.NoStyle) ? GUI.skin.button.hover.background : Style.UnityStyle.hover.background);
			}
		}
		else if (Hover)
		{
			if ((supportedStates & SHSButtonStyleInfo.SupportedStatesEnum.Highlight) != 0)
			{
				GUI.DrawTexture(base.rect, (Style == SHSStyle.NoStyle) ? GUI.skin.button.hover.background : Style.UnityStyle.hover.background);
			}
			else if ((supportedStates & SHSButtonStyleInfo.SupportedStatesEnum.Pressed) != 0)
			{
				GUI.DrawTexture(base.rect, (Style == SHSStyle.NoStyle) ? GUI.skin.button.active.background : Style.UnityStyle.active.background);
			}
			else if ((supportedStates & SHSButtonStyleInfo.SupportedStatesEnum.Normal) != 0)
			{
				GUI.DrawTexture(base.rect, (Style == SHSStyle.NoStyle) ? GUI.skin.button.normal.background : Style.UnityStyle.normal.background);
			}
		}
		else if ((supportedStates & SHSButtonStyleInfo.SupportedStatesEnum.Normal) != 0)
		{
			GUI.DrawTexture(base.rect, (Style == SHSStyle.NoStyle) ? GUI.skin.button.normal.background : Style.UnityStyle.normal.background);
		}
		else if ((supportedStates & SHSButtonStyleInfo.SupportedStatesEnum.Highlight) != 0)
		{
			GUI.DrawTexture(base.rect, (Style == SHSStyle.NoStyle) ? GUI.skin.button.hover.background : Style.UnityStyle.hover.background);
		}
		else if ((supportedStates & SHSButtonStyleInfo.SupportedStatesEnum.Pressed) != 0)
		{
			GUI.DrawTexture(base.rect, (Style == SHSStyle.NoStyle) ? GUI.skin.button.active.background : Style.UnityStyle.active.background);
		}
		if (text != null && text != string.Empty)
		{
			GUI.Label(base.rect, text, (Style == SHSStyle.NoStyle) ? GUI.skin.label : getTextStyle());
		}
	}

	public override bool FireMouseOver(GUIMouseEvent data)
	{
		ShsAudioSource.PlayAutoSound(Style.AudioOver);
		return base.FireMouseOver(data);
	}

	public override bool FireMouseOut(GUIMouseEvent data)
	{
		ShsAudioSource.PlayAutoSound(Style.AudioOut);
		return base.FireMouseOut(data);
	}

	public override bool FireMouseDown(GUIMouseEvent data)
	{
		ShsAudioSource.PlayAutoSound(Style.AudioDown);
		return base.FireMouseDown(data);
	}

	public override bool FireMouseUp(GUIMouseEvent data)
	{
		ShsAudioSource.PlayAutoSound(Style.AudioUp);
		return base.FireMouseUp(data);
	}

	protected override void AttachInspector()
	{
		inspector = guiTreeNode.AddComponent(typeof(GUIButtonInspector));
	}

	protected override void ReflectToInspector()
	{
		base.ReflectToInspector();
		if (!(inspector != null))
		{
		}
	}

	protected override void ReflectFromInspector()
	{
		base.ReflectFromInspector();
		if (!(inspector != null))
		{
		}
	}
}
