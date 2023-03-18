using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Xml.XPath;

[XmlRoot(ElementName = "arcadegames")]
public class ArcadeManager
{
	private readonly Dictionary<string, ArcadeGame> games;

	public Dictionary<string, ArcadeGame> Games
	{
		get
		{
			return games;
		}
	}

	public ArcadeManager()
	{
		games = new Dictionary<string, ArcadeGame>();
	}

	public void InitializeFromData(DataWarehouse xml)
	{
		XPathNavigator value = xml.GetValue("arcadegames");
		XPathNodeIterator xPathNodeIterator = value.SelectChildren("game", string.Empty);
		while (xPathNodeIterator.MoveNext())
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(ArcadeGame));
			string outerXml = xPathNodeIterator.Current.OuterXml;
			ArcadeGame arcadeGame = xmlSerializer.Deserialize(new StringReader(outerXml)) as ArcadeGame;
			if (arcadeGame != null)
			{
				games[arcadeGame.Keyword] = arcadeGame;
			}
		}
	}
}
