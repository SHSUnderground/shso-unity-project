using System;

public class SHSButtonStyleInfo : SHSStyleInfo
{
	[Flags]
	public enum SupportedStatesEnum
	{
		Normal = 0x1,
		Highlight = 0x2,
		Pressed = 0x4
	}

	public enum SizeCategoryEnum
	{
		Auto,
		Small,
		Large
	}

	protected SupportedStatesEnum supportedStates = SupportedStatesEnum.Normal | SupportedStatesEnum.Highlight | SupportedStatesEnum.Pressed;

	protected SizeCategoryEnum sizeCategory;

	public string buttonSourceRoot;

	public bool useHighlightAudio = true;

	public string downAudio;

	public string upAudio;

	public string overAudio;

	public bool isStateless;

	public SupportedStatesEnum SupportedStates
	{
		get
		{
			return supportedStates;
		}
		set
		{
			supportedStates = value;
		}
	}

	public SizeCategoryEnum SizeCategory
	{
		get
		{
			return sizeCategory;
		}
		set
		{
			sizeCategory = value;
		}
	}

	public SHSButtonStyleInfo(string ButtonSourceRoot)
	{
		buttonSourceRoot = ButtonSourceRoot;
	}

	public SHSButtonStyleInfo(string ButtonSourceRoot, SupportedStatesEnum states)
	{
		buttonSourceRoot = ButtonSourceRoot;
		supportedStates = states;
	}

	public SHSButtonStyleInfo(string ButtonSourceRoot, SizeCategoryEnum size)
	{
		buttonSourceRoot = ButtonSourceRoot;
		SizeCategory = size;
	}

	public SHSButtonStyleInfo(string ButtonSourceRoot, bool isStateless)
	{
		buttonSourceRoot = ButtonSourceRoot;
		this.isStateless = isStateless;
		supportedStates = SupportedStatesEnum.Normal;
	}

	public bool IsLarge(IGUIControl control)
	{
		if (SizeCategory == SizeCategoryEnum.Auto)
		{
			return control.Rect.height >= 100f && control.Rect.width >= 100f;
		}
		return SizeCategory == SizeCategoryEnum.Large;
	}
}
