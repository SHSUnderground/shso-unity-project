using System.Collections.Generic;

public class RoomStaticData
{
	public string id;

	public string typeid;

	public string bundleName;

	public List<string> fixedBundleNames;

	public int item_cap = 40;

	public RoomStaticData()
	{
		id = null;
		typeid = null;
		bundleName = null;
		fixedBundleNames = null;
	}

	public bool InitializeFromData(DataWarehouse data)
	{
		id = data.TryGetString("id", string.Empty);
		if (id == string.Empty)
		{
			return false;
		}
		typeid = data.TryGetString("typeid", string.Empty);
		if (typeid == string.Empty)
		{
			return false;
		}
		bundleName = data.TryGetString("bundle", string.Empty);
		if (bundleName == string.Empty)
		{
			return false;
		}
		bundleName = "HQ/" + bundleName;
		foreach (DataWarehouse item in data.GetIterator("fixed_bundles"))
		{
			string text = item.TryGetString("bundle", null);
			if (text != null)
			{
				if (fixedBundleNames == null)
				{
					fixedBundleNames = new List<string>();
				}
				text = "HQ/" + text;
				fixedBundleNames.Add(text);
			}
		}
		item_cap = data.TryGetInt("item_cap", 40);
		return true;
	}
}
