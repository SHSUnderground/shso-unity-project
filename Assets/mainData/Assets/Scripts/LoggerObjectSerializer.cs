using SmartFoxClientAPI.Data;
using System;
using System.Collections;

public class LoggerObjectSerializer
{
	public static LoggerObject fromInt(int from)
	{
		return new LoggerObject(typeof(int), "<int>" + from + "</int>");
	}

	public static LoggerObject fromBool(bool from)
	{
		return new LoggerObject(typeof(bool), "<bool>" + from + "</bool>");
	}

	public static LoggerObject fromString(string from)
	{
		if (from == null)
		{
			return new LoggerObject(typeof(string), "<null />");
		}
		return new LoggerObject(typeof(string), "<string><![CDATA[" + from + "]]></string>");
	}

	public static LoggerObject fromHashtable(Hashtable from)
	{
		if (from == null)
		{
			return new LoggerObject(typeof(Hashtable), "<null />");
		}
		string text = string.Empty;
		foreach (DictionaryEntry item in from)
		{
			text += "<entry>";
			string text2 = text;
			text = text2 + "<key>" + fromObject(item.Key).SerializedObject + "</key><value>" + fromObject(item.Value).SerializedObject + "</value>";
			text += "</entry>";
		}
		return new LoggerObject(typeof(Hashtable), "<hashtable>" + text + "</hashtable>");
	}

	public static LoggerObject fromArrayList(ArrayList from)
	{
		if (from == null)
		{
			return new LoggerObject(typeof(ArrayList), "<null />");
		}
		string text = string.Empty;
		foreach (object item in from)
		{
			text = text + "<value>" + fromObject(item).SerializedObject + "</value>";
		}
		return new LoggerObject(typeof(ArrayList), "<arrayList>" + text + "</arrayList>");
	}

	public static LoggerObject fromCollection(ICollection from)
	{
		LoggerObject loggerObject = null;
		if (from is Hashtable)
		{
			loggerObject = fromHashtable((Hashtable)from);
		}
		else if (from is ArrayList)
		{
			loggerObject = fromArrayList((ArrayList)from);
		}
		if (from == null)
		{
			return new LoggerObject(typeof(ICollection), "<null />");
		}
		if (loggerObject == null)
		{
			return new LoggerObject(typeof(ICollection), "<unkownCollectionType>" + Uri.EscapeUriString(from.GetType().Name) + "</unkownCollectionType>");
		}
		return new LoggerObject(typeof(ICollection), loggerObject.SerializedObject);
	}

	public static LoggerObject fromObject(object from)
	{
		LoggerObject loggerObject = null;
		if (from is string)
		{
			loggerObject = fromString((string)from);
		}
		else if (from is bool)
		{
			loggerObject = fromBool((bool)from);
		}
		else if (from is int)
		{
			loggerObject = fromInt((int)from);
		}
		else if (from is SFSObject)
		{
			loggerObject = fromSFSObject((SFSObject)from);
		}
		else if (from is Hashtable)
		{
			loggerObject = fromHashtable((Hashtable)from);
		}
		else if (from is ArrayList)
		{
			loggerObject = fromArrayList((ArrayList)from);
		}
		else if (from is string[])
		{
			loggerObject = fromStringArray((string[])from);
		}
		else if (from is object[])
		{
			loggerObject = fromObjectArray((object[])from);
		}
		if (from == null)
		{
			return new LoggerObject(typeof(object), "<null />");
		}
		if (loggerObject == null)
		{
			return new LoggerObject(typeof(object), "<unkownObjectType>" + Uri.EscapeUriString(from.GetType().Name) + "</unkownObjectType>");
		}
		return new LoggerObject(typeof(object), loggerObject.SerializedObject);
	}

	public static LoggerObject fromStringArray(string[] from)
	{
		if (from == null)
		{
			return new LoggerObject(typeof(string[]), "<null />");
		}
		string text = string.Empty;
		foreach (string from2 in from)
		{
			text = text + "<value>" + fromObject(from2).SerializedObject + "</value>";
		}
		return new LoggerObject(typeof(string[]), "<stringArray>" + text + "</stringArray>");
	}

	public static LoggerObject fromObjectArray(object[] from)
	{
		if (from == null)
		{
			return new LoggerObject(typeof(object[]), "<null />");
		}
		string text = string.Empty;
		foreach (object from2 in from)
		{
			text = text + "<value>" + fromObject(from2).SerializedObject + "</value>";
		}
		return new LoggerObject(typeof(object[]), "<objectArray>" + text + "</objectArray>");
	}

	public static LoggerObject fromSFSObject(SFSObject from)
	{
		if (from == null)
		{
			return new LoggerObject(typeof(SFSObject), "<null />");
		}
		string text = string.Empty;
		foreach (object item in from.Keys())
		{
			text += "<entry>";
			string text2 = text;
			text = text2 + "<key>" + fromObject(item).SerializedObject + "</key><value>" + fromObject(from.Get(item)).SerializedObject + "</value>";
			text += "</entry>";
		}
		return new LoggerObject(typeof(SFSObject), "<sfsObject>" + text + "</sfsObject>");
	}
}
