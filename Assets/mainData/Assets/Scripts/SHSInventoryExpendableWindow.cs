using System.Collections.Generic;

public class SHSInventoryExpendableWindow : SHSInventorySelectionWindow
{
	private Dictionary<string, SHSInventoryExpendableItem> _expendableList;

	private Dictionary<string, PrerequisiteCheckResult> _expendablePreqResults;

	private string[] _expendableIds;

	public SHSInventoryExpendableWindow(GUISlider slider)
		: base(slider)
	{
		_expendablePreqResults = new Dictionary<string, PrerequisiteCheckResult>();
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		AppShell.Instance.ExpendablesManager.CanExpend(_expendableIds, ref _expendablePreqResults);
		foreach (KeyValuePair<string, PrerequisiteCheckResult> expendablePreqResult in _expendablePreqResults)
		{
			if (!_expendableList.ContainsKey(expendablePreqResult.Key))
			{
				CspUtils.DebugLog("InventoryExpendableWindow::OnUpdate() - stored expendable list does not contain expendable with id <" + expendablePreqResult.Key + ">");
			}
			else
			{
				_expendableList[expendablePreqResult.Key].Enable(expendablePreqResult.Value);
			}
		}
	}

	public void InitializeExpendableList(List<SHSInventorySelectionItem> selections)
	{
		if (selections != null)
		{
			_expendableList = new Dictionary<string, SHSInventoryExpendableItem>();
			foreach (SHSInventorySelectionItem selection in selections)
			{
				SHSInventoryExpendableItem sHSInventoryExpendableItem = selection as SHSInventoryExpendableItem;
				if (sHSInventoryExpendableItem != null)
				{
					_expendableList.Add(sHSInventoryExpendableItem.expendable.Definition.OwnableTypeId, sHSInventoryExpendableItem);
				}
			}
			_expendableIds = new string[_expendableList.Count];
			_expendableList.Keys.CopyTo(_expendableIds, 0);
			AddList(selections);
			SortItemList();
		}
	}
}
