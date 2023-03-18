using System.ComponentModel;
using System.Xml.Serialization;

[XmlRoot(ElementName = "configuration")]
public class Zones : StaticDataDefinition, IStaticDataDefinitionTxt
{
	[XmlRoot(ElementName = "layout")]
	public class Layout
	{
		[XmlElement(ElementName = "name")]
		public string name;

		[DefaultValue("Unknown")]
		[XmlElement(ElementName = "title")]
		public string title;

		[DefaultValue(-1)]
		[XmlElement(ElementName = "gameworld_index")]
		public int gameworldID;

		[XmlElement(ElementName = "hard_cap")]
		public int hardCap;

		[XmlElement(ElementName = "soft_cap")]
		public int softCap;

		[XmlElement(ElementName = "primary_zone")]
		[DefaultValue(false)]
		public bool primaryZone;

		[XmlArray("bundles")]
		[XmlArrayItem("bundle")]
		public string[] bundles;
	}

	[XmlArrayItem("layout")]
	[XmlArray("layouts")]
	public Layout[] layouts;

	public void InitializeFromData(string xml)
	{
		Zones zones = Utils.XmlDeserialize<Zones>(xml);
		layouts = zones.layouts;
	}
}
