using System.Collections.Generic;

public class SHSInventoryMysteryBoxWindow : SHSInventorySelectionWindow
{
	private Dictionary<string, SHSInventoryMysteryItem> _boxList;

	private string[] _boxIds;

	public SHSInventoryMysteryBoxWindow(GUISlider slider)
		: base(slider)
	{
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
	}

	public void InitializeExpendableList(List<SHSInventorySelectionItem> selections)
	{
		if (selections != null)
		{
			_boxList = new Dictionary<string, SHSInventoryMysteryItem>();
			foreach (SHSInventorySelectionItem selection in selections)
			{
				SHSInventoryMysteryItem sHSInventoryMysteryItem = selection as SHSInventoryMysteryItem;
				if (sHSInventoryMysteryItem != null)
				{
					_boxList.Add(string.Empty + sHSInventoryMysteryItem.mysteryBox.Definition.ownableTypeID, sHSInventoryMysteryItem);
				}
			}
			_boxIds = new string[_boxList.Count];
			_boxList.Keys.CopyTo(_boxIds, 0);
			AddList(selections);
			SortItemList();
		}
	}
}
