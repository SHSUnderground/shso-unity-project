using System.Xml.XPath;

public class GearDataLoadCollection : ShsCollectionBase<GearDataCollectionItem>
{
	public GearDataLoadCollection(DataWarehouse xmlData)
	{
		CspUtils.DebugLog("GearDataLoadCollection " + xmlData.ToString());
		DataWarehouse data = xmlData.GetData("//masterinventory//gear");
		XPathNodeIterator values = data.GetValues("item");
		int count = values.Count;
		CspUtils.DebugLog("count is <" + count + ">");
		foreach (XPathNavigator item in Utils.Enumerate(values))
		{
			CspUtils.DebugLog(item.InnerXml);
			DataWarehouse data2 = new DataWarehouse(item);
			GearDataCollectionItem gearDataCollectionItem = new GearDataCollectionItem();
			if (gearDataCollectionItem.InitializeFromData(data2))
			{
				Add(gearDataCollectionItem.GetKey(), gearDataCollectionItem);
			}
			else
			{
				CspUtils.DebugLog("Gear init failed for gear " + item.InnerXml);
			}
		}
	}
}
