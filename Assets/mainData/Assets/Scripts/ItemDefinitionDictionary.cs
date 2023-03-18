using System.Xml.XPath;

public class ItemDefinitionDictionary : StaticDataDefinitionDictionary<ItemDefinition>
{
	public ItemDefinitionDictionary(DataWarehouse data)
	{
		XPathNavigator value = data.GetValue(".");
		value.MoveToFirstChild();
		value.MoveToFirstChild();
		do
		{
			DataWarehouse data2 = new DataWarehouse(value);
			ItemDefinition itemDefinition = new ItemDefinition();
			itemDefinition.InitializeFromData(data2);
			Add(itemDefinition.Id, itemDefinition);
		}
		while (value.MoveToNext());
	}
}
