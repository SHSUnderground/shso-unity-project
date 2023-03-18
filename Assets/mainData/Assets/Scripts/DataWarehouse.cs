using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;
using UnityEngine;

public class DataWarehouse
{
	private string fileName;

	public string xmlString;

	private XPathDocument document;

	public XPathNavigator navigator;

	private bool hasParsed;

	public XPathNavigator Navigator
	{
		get
		{
			return navigator;
		}
	}

	public DataWarehouse()
	{
		Reset();
	}

	public DataWarehouse(string XmlDataString)
	{
		Reset();
		xmlString = XmlDataString;
	}

	public DataWarehouse(XPathNavigator ExistingNavigator)
	{
		if (ExistingNavigator != null)
		{
			navigator = ExistingNavigator;
			hasParsed = true;
		}
	}

	protected void Reset()
	{
		hasParsed = false;
		xmlString = null;
	}

	public void ParseString(string XmlDataString)
	{
		if (xmlString != null || hasParsed)
		{
			Reset();
		}
		xmlString = XmlDataString;
		Parse();
	}

	public void ParseFile(string XmlFileName)
	{
		if (XmlFileName != null || hasParsed)
		{
			Reset();
		}
		fileName = XmlFileName;
		Parse();
	}

	public void Parse()
	{
		if (xmlString == null && fileName == null)
		{
			CspUtils.DebugLog("DataWarehouse refused to parse an empty XML string");
			return;
		}
		if (fileName != null)
		{
			document = new XPathDocument(fileName);
		}
		else
		{
			document = new XPathDocument(new StringReader(xmlString));
		}
		navigator = document.CreateNavigator();
		hasParsed = true;
	}

	public int GetInt(string TagPath)
	{
		return GetValue(TagPath).ValueAsInt;
	}

	public int TryGetInt(string TagPath, int defaultValue)
	{
		//Discarded unreachable code: IL_001c, IL_0029
		XPathNavigator value = GetValue(TagPath);
		if (value == null)
		{
			return defaultValue;
		}
		try
		{
			return value.ValueAsInt;
		}
		catch
		{
			return defaultValue;
		}
	}

	public long GetLong(string TagPath)
	{
		return GetValue(TagPath).ValueAsLong;
	}

	public long TryGetLong(string TagPath, long defaultValue)
	{
		//Discarded unreachable code: IL_001c, IL_0029
		XPathNavigator value = GetValue(TagPath);
		if (value == null)
		{
			return defaultValue;
		}
		try
		{
			return value.ValueAsLong;
		}
		catch
		{
			return defaultValue;
		}
	}

	public float GetFloat(string TagPath)
	{
		return (float)GetValue(TagPath).ValueAsDouble;
	}

	public float TryGetFloat(string TagPath, float defaultValue)
	{
		//Discarded unreachable code: IL_001d, IL_002a
		XPathNavigator value = GetValue(TagPath);
		if (value == null)
		{
			return defaultValue;
		}
		try
		{
			return (float)value.ValueAsDouble;
		}
		catch
		{
			return defaultValue;
		}
	}

	public double GetDouble(string TagPath)
	{
		return GetValue(TagPath).ValueAsDouble;
	}

	public double TryGetDouble(string TagPath, double defaultValue)
	{
		//Discarded unreachable code: IL_001c, IL_0029
		XPathNavigator value = GetValue(TagPath);
		if (value == null)
		{
			return defaultValue;
		}
		try
		{
			return value.ValueAsDouble;
		}
		catch
		{
			return defaultValue;
		}
	}

	public string GetString(string TagPath)
	{
		return GetValue(TagPath).Value;
	}

	public string TryGetString(string TagPath, string defaultValue)
	{
		//Discarded unreachable code: IL_001c, IL_0029
		XPathNavigator value = GetValue(TagPath);
		if (value == null)
		{
			return defaultValue;
		}
		try
		{
			return value.Value;
		}
		catch
		{
			return defaultValue;
		}
	}

	public bool GetBool(string TagPath)
	{
		return GetValue(TagPath).ValueAsBoolean;
	}

	public bool TryGetBool(string TagPath, bool defaultValue)
	{
		//Discarded unreachable code: IL_001c, IL_0029
		XPathNavigator value = GetValue(TagPath);
		if (value == null)
		{
			return defaultValue;
		}
		try
		{
			return value.ValueAsBoolean;
		}
		catch
		{
			return defaultValue;
		}
	}

	public Vector3 TryGetVector(string TagPath, Vector3 defaultValue)
	{
		Vector3 result = default(Vector3);
		result = defaultValue;
		XPathNavigator value = GetValue(TagPath);
		if (value != null)
		{
			value.MoveToFirstChild();
			do
			{
				switch (value.LocalName.ToLower())
				{
				case "x":
					result.x = (float)value.ValueAsDouble;
					continue;
				case "y":
					result.y = (float)value.ValueAsDouble;
					continue;
				case "z":
					result.z = (float)value.ValueAsDouble;
					continue;
				}
				if (value.Value.Contains(","))
				{
					string[] array = value.Value.Split(',');
					double result2;
					if (array.Length == 3 && double.TryParse(array[0], out result2))
					{
						result.x = (float)result2;
						if (double.TryParse(array[1], out result2))
						{
							result.y = (float)result2;
							if (double.TryParse(array[2], out result2))
							{
								result.z = (float)result2;
								return result;
							}
						}
					}
				}
				CspUtils.DebugLog("Ignoring unexpected element (" + value.LocalName.ToLower() + ") with value (" + value.Value + ") while populating a vector values.");
			}
			while (value.MoveToNext());
		}
		return result;
	}

	public Quaternion TryGetQuaternion(string TagPath, Quaternion defaultValue)
	{
		Quaternion result = default(Quaternion);
		result = defaultValue;
		XPathNavigator value = GetValue(TagPath);
		if (value != null)
		{
			value.MoveToFirstChild();
			do
			{
				switch (value.LocalName.ToLower())
				{
				case "x":
					result.x = (float)value.ValueAsDouble;
					break;
				case "y":
					result.y = (float)value.ValueAsDouble;
					break;
				case "z":
					result.z = (float)value.ValueAsDouble;
					break;
				case "w":
					result.w = (float)value.ValueAsDouble;
					break;
				default:
					CspUtils.DebugLog("Ignoring unexpected element while populating a vector values.");
					break;
				}
			}
			while (value.MoveToNext());
		}
		return result;
	}

	public Color TryGetColorRGB(string TagPath, Color defaultValue)
	{
		//Discarded unreachable code: IL_0054, IL_0062
		try
		{
			string[] array = GetString(TagPath).Split(',');
			float r = float.Parse(array[0]) / 255f;
			float g = float.Parse(array[1]) / 255f;
			float b = float.Parse(array[2]) / 255f;
			return new Color(r, g, b);
		}
		catch
		{
			return defaultValue;
		}
	}

	public string GetXml(string TagPath)
	{
		return GetValue(TagPath).InnerXml;
	}

	public string TryGetXml(string TagPath, string defaultValue)
	{
		//Discarded unreachable code: IL_001c, IL_0029
		XPathNavigator value = GetValue(TagPath);
		if (value == null)
		{
			return defaultValue;
		}
		try
		{
			return value.InnerXml;
		}
		catch
		{
			return defaultValue;
		}
	}

	public DataWarehouse GetData(string TagPath)
	{
		return new DataWarehouse(GetValue(TagPath));
	}

	public DataWarehouse TryGetData(string TagPath, DataWarehouse defaultValue)
	{
		XPathNavigator value = GetValue(TagPath);
		if (value == null)
		{
			return defaultValue;
		}
		return new DataWarehouse(value);
	}

	public virtual XPathNavigator GetValue(string TagPath)
	{
		//Discarded unreachable code: IL_0063
		if (!hasParsed)
		{
			CspUtils.DebugLog("DataWarehouse.GetValue() cannot retrieve " + TagPath + " because it has not parsed XML yet.  Call Parse() before calling methods like GetValue(), GetCount(), etc.");
			return null;
		}
		string xpath = TagPath.Replace('.', '/');
		XPathNodeIterator xPathNodeIterator = null;
		try
		{
			xPathNodeIterator = navigator.Select(xpath);
		}
		catch
		{
			CspUtils.DebugLog("Exception in XDataWarehouse: trying to evaluate " + TagPath);
			return null;
		}
		if (xPathNodeIterator == null)
		{
			CspUtils.DebugLog("DataWarehouse could not find a simple value for tag " + TagPath);
			return null;
		}
		if (xPathNodeIterator.Count > 0)
		{
			xPathNodeIterator.MoveNext();
			return xPathNodeIterator.Current;
		}
		return null;
	}

	public int GetInt(string TagPath, int Index)
	{
		return GetInt(TagPath + "[" + (Index + 1) + "]");
	}

	public float GetFloat(string TagPath, int Index)
	{
		return GetFloat(TagPath + "[" + (Index + 1) + "]");
	}

	public double GetDouble(string TagPath, int Index)
	{
		return GetDouble(TagPath + "[" + (Index + 1) + "]");
	}

	public string GetString(string TagPath, int Index)
	{
		return GetString(TagPath + "[" + (Index + 1) + "]");
	}

	public bool GetBool(string TagPath, int Index)
	{
		return GetBool(TagPath + "[" + (Index + 1) + "]");
	}

	public string GetXml(string TagPath, int Index)
	{
		return GetXml(TagPath + "[" + (Index + 1) + "]");
	}

	public DataWarehouse GetData(string TagPath, int Index)
	{
		return GetData(TagPath + "[" + (Index + 1) + "]");
	}

	public XPathNavigator GetValue(string TagPath, int Index)
	{
		return GetValue(TagPath + "[" + (Index + 1) + "]");
	}

	public virtual int GetCount(string TagPath)
	{
		string xpath = TagPath.Replace('.', '/');
		XPathNodeIterator xPathNodeIterator = navigator.Select(xpath);
		if (xPathNodeIterator == null)
		{
			return 0;
		}
		return xPathNodeIterator.Count;
	}

	public virtual IEnumerable<DataWarehouse> GetIterator(string TagPath)
	{
		if (!hasParsed)
		{
			CspUtils.DebugLog("DataWarehouse.GetIterator() cannot retrieve " + TagPath + " because it has not parsed XML yet.  Call Parse() before calling methods like GetValue(), GetCount(), etc.");
			yield break;
		}
		XPathNodeIterator NodeSet2 = null;
		try
		{
			NodeSet2 = navigator.Select(TagPath);
		}
		catch
		{
			CspUtils.DebugLog("Exception in XDataWarehouse: trying to evaluate " + TagPath);
			yield break;
		}
		if (NodeSet2 == null)
		{
			CspUtils.DebugLog("DataWarehouse could not find a simple value for tag " + TagPath);
			yield break;
		}
		while (NodeSet2.MoveNext())
		{
			yield return new DataWarehouse(NodeSet2.Current);
		}
	}

	public virtual XPathNodeIterator GetValues(string TagPath)
	{
		//Discarded unreachable code: IL_0058
		if (!hasParsed)
		{
			CspUtils.DebugLog("DataWarehouse.GetValues() cannot retrieve " + TagPath + " because it has not parsed XML yet.  Call Parse() before calling methods like GetValue(), GetCount(), etc.");
			return null;
		}
		XPathNodeIterator xPathNodeIterator = null;
		try
		{
			xPathNodeIterator = navigator.Select(TagPath);
		}
		catch
		{
			CspUtils.DebugLog("Exception in XDataWarehouse: trying to evaluate " + TagPath);
			return null;
		}
		if (xPathNodeIterator == null)
		{
			CspUtils.DebugLog("DataWarehouse could not find a simple value for tag " + TagPath);
			return null;
		}
		return xPathNodeIterator;
	}

	public int GetInt(string TagPath, string Attribute)
	{
		return int.Parse(GetValue(TagPath).GetAttribute(Attribute, string.Empty));
	}

	public float GetFloat(string TagPath, string Attribute)
	{
		return float.Parse(GetValue(TagPath).GetAttribute(Attribute, string.Empty));
	}

	public double GetDouble(string TagPath, string Attribute)
	{
		return double.Parse(GetValue(TagPath).GetAttribute(Attribute, string.Empty));
	}

	public string GetString(string TagPath, string Attribute)
	{
		return GetValue(TagPath).GetAttribute(Attribute, string.Empty);
	}

	public bool GetBool(string TagPath, string Attribute)
	{
		return bool.Parse(GetValue(TagPath).GetAttribute(Attribute, string.Empty));
	}
}
