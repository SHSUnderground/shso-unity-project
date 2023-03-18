using System;
using System.Xml.Serialization;

[Serializable]
[XmlRoot(ElementName = "SmartTip")]
public class SmartTip
{
	public enum PlayerTargetEnum
	{
		Subscriber,
		FreePlayer,
		Any
	}

	[XmlElement(ElementName = "TitleKey")]
	public string titleKey;

	[XmlElement(ElementName = "TipKey")]
	public string tipKey;

	[XmlElement(ElementName = "Target")]
	public PlayerTargetEnum Target;

	[XmlElement(ElementName = "Erosion")]
	public float Erosion;

	protected SmartTipCategory[] categories;

	[XmlIgnore]
	public int UseCount;

	[XmlArray(ElementName = "Categories")]
	[XmlArrayItem("Category")]
	public SmartTipCategory[] SmartTipCategories
	{
		get
		{
			return categories;
		}
		set
		{
			categories = value;
		}
	}
}
