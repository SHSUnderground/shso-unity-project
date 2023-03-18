public class GUIListItem : GUIControlWindow
{
	protected bool isSelected;

	protected string itemId;

	protected object data;

	public bool IsSelected
	{
		get
		{
			return isSelected;
		}
		set
		{
			isSelected = value;
		}
	}

	public string ItemId
	{
		get
		{
			return itemId;
		}
		set
		{
			itemId = value;
		}
	}

	public object Data
	{
		get
		{
			return data;
		}
		set
		{
			data = value;
		}
	}

	public GUIListItem()
	{
		Traits.BlockTestType = BlockTestTypeEnum.Transparent;
		data = null;
	}
}
