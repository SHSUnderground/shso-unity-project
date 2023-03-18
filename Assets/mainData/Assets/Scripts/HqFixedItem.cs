using UnityEngine;

[AddComponentMenu("Hq/Fixed Item")]
public class HqFixedItem : HqItem
{
	public string ItemType;

	public void Awake()
	{
		if (ItemType == null)
		{
			CspUtils.DebugLog("No ItemType specified for fixed item!");
			return;
		}
		ItemDefinition value;
		AppShell.Instance.ItemDictionary.TryGetValue(ItemType, out value);
		if (value == null)
		{
			CspUtils.DebugLog("No item definition found for fixed item!");
			return;
		}
		HqRoom2 component = Utils.GetComponent<HqRoom2>(base.gameObject, Utils.SearchParents);
		if (component == null)
		{
			CspUtils.DebugLog("Could not find parent room for fixed item!");
		}
		else
		{
			Initialize(component, value);
		}
	}
}
