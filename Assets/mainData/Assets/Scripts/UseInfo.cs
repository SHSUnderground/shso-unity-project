using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Xml.XPath;

public class UseInfo
{
	protected Dictionary<string, SequenceInfo> sequences;

	protected Dictionary<string, Use> uses;

	public Dictionary<string, Use> Uses
	{
		get
		{
			return uses;
		}
	}

	public List<SequenceInfo> Sequences
	{
		get
		{
			return new List<SequenceInfo>(sequences.Values);
		}
	}

	public UseInfo(DataWarehouse data)
	{
		InitializeFromData(data);
	}

	public void InitializeFromData(DataWarehouse data)
	{
		ParseSequences(data);
		ParseUses(data);
	}

	public Use GetUseByDockPointName(string dockPointName)
	{
		if (uses != null && uses.ContainsKey(dockPointName))
		{
			return uses[dockPointName];
		}
		return null;
	}

	public SequenceInfo GetSequenceByName(string sequenceName)
	{
		if (sequences != null && sequences.ContainsKey(sequenceName))
		{
			return sequences[sequenceName];
		}
		return null;
	}

	private void ParseSequences(DataWarehouse data)
	{
		string text = data.TryGetXml("sequences", null);
		if (text == null)
		{
			return;
		}
		sequences = new Dictionary<string, SequenceInfo>();
		XPathNavigator value = data.GetValue("sequences");
		XPathNodeIterator xPathNodeIterator = value.SelectChildren("sequence", string.Empty);
		if (xPathNodeIterator == null)
		{
			return;
		}
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(SequenceInfo));
		while (xPathNodeIterator.MoveNext())
		{
			XPathNavigator current = xPathNodeIterator.Current;
			if (current != null)
			{
				string outerXml = current.OuterXml;
				SequenceInfo sequenceInfo = xmlSerializer.Deserialize(new StringReader(outerXml)) as SequenceInfo;
				if (sequenceInfo != null && sequenceInfo.SequenceName != null)
				{
					sequences[sequenceInfo.SequenceName] = sequenceInfo;
				}
			}
		}
	}

	private void ParseUses(DataWarehouse data)
	{
		DataWarehouse dataWarehouse = data.TryGetData("uses", null);
		if (dataWarehouse != null)
		{
			uses = new Dictionary<string, Use>();
			foreach (DataWarehouse item in dataWarehouse.GetIterator("use"))
			{
				Use use = new Use(item);
				uses[use.DockPointName] = use;
			}
		}
	}
}
