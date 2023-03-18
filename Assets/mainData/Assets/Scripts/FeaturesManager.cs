using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Xml.XPath;

public class FeaturesManager
{
	private static Dictionary<string, bool> featuresDict = new Dictionary<string, bool>();

	public static void InitializeFromData(DataWarehouse xml)
	{
		XPathNavigator value = xml.GetValue("features");
		XPathNodeIterator xPathNodeIterator = value.SelectChildren("feature", string.Empty);
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(FeatureDefinition));
		while (xPathNodeIterator.MoveNext())
		{
			string outerXml = xPathNodeIterator.Current.OuterXml;
			FeatureDefinition featureDefinition = xmlSerializer.Deserialize(new StringReader(outerXml)) as FeatureDefinition;
			if (featureDefinition != null)
			{
				featuresDict[featureDefinition.name] = (featureDefinition.enabled == "1");
			}
		}
	}

	public static void enableFeature(string feature, bool enabled = true)
	{
		featuresDict[feature] = enabled;
	}

	public static bool featureEnabled(string featureName)
	{
		if (featuresDict.ContainsKey(featureName))
		{
			return featuresDict[featureName];
		}
		return false;
	}
}
