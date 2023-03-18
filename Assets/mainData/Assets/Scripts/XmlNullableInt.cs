using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using UnityEngine;

public struct XmlNullableInt : IXmlSerializable
{
	private int value;

	private bool hasValue;

	public bool HasValue
	{
		get
		{
			return hasValue;
		}
	}

	public int Value
	{
		get
		{
			return value;
		}
	}

	private XmlNullableInt(int value)
	{
		hasValue = true;
		this.value = value;
	}

	void IXmlSerializable.ReadXml(XmlReader reader)
	{
		if (reader.GetAttribute("nil") == "true")
		{
			ReadNullValue();
		}
		else
		{
			TryReadNonNullValue(reader);
		}
	}

	void IXmlSerializable.WriteXml(XmlWriter writer)
	{
		throw new NotSupportedException();
	}

	public XmlSchema GetSchema()
	{
		return null;
	}

	private void ReadNullValue()
	{
		hasValue = false;
	}

	private void TryReadNonNullValue(XmlReader reader)
	{
		if (reader.IsEmptyElement)
		{
			ReadNullValue();
			return;
		}
		reader.ReadStartElement();
		string str = reader.ReadString().Trim();
		if (string.IsNullOrEmpty(str))
		{
			ReadNullValue();
		}
		else
		{
			try
			{
				value = Convert.ToInt32(str);
				hasValue = true;
			}
			catch (FormatException)
			{
				CspUtils.DebugLogError("FormatException <int32>: " + str);
				CspUtils.DebugLogError("Reader: " + reader.ReadOuterXml());
			}
		}
		reader.ReadEndElement();
	}

	public static implicit operator XmlNullableInt(int value)
	{
		return new XmlNullableInt(value);
	}
}
