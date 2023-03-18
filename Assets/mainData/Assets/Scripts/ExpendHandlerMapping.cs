using System;
using System.Xml.Serialization;

[Serializable]
[XmlRoot(ElementName = "handler_mapping")]
public class ExpendHandlerMapping
{
	[XmlElement(ElementName = "name")]
	public string HandlerName;

	[XmlElement(ElementName = "classname")]
	public string HandlerClassName;
}
