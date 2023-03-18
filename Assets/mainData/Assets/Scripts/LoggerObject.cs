using System;

public class LoggerObject
{
	private Type originalType;

	private string serializedObject;

	public Type OriginalType
	{
		get
		{
			return originalType;
		}
	}

	public string SerializedObject
	{
		get
		{
			return serializedObject;
		}
	}

	public LoggerObject(Type originalType, string serializedObject)
	{
		this.originalType = originalType;
		this.serializedObject = serializedObject;
	}
}
