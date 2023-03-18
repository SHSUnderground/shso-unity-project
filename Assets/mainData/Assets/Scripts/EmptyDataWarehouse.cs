using System.Collections.Generic;
using System.Xml.XPath;

public class EmptyDataWarehouse : DataWarehouse
{
	public override XPathNavigator GetValue(string TagPath)
	{
		return null;
	}

	public override int GetCount(string TagPath)
	{
		return 0;
	}

	public override IEnumerable<DataWarehouse> GetIterator(string TagPath)
	{
		yield break;
	}

	public override XPathNodeIterator GetValues(string TagPath)
	{
		return null;
	}
}
