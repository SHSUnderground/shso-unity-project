public class SHSButtonNoHighlightStyleInfo : SHSStyleInfo
{
	public enum SizeCategoryEnum
	{
		Auto,
		Small,
		Large
	}

	protected SizeCategoryEnum sizeCategory;

	public string buttonSourceRoot;

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

	public SHSButtonNoHighlightStyleInfo(string ButtonSourceRoot)
	{
		buttonSourceRoot = ButtonSourceRoot;
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
