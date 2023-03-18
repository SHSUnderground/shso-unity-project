using System;
using System.Reflection;
using UnityEngine;

public class GenericComponentCopier : IComponentCopier
{
	public virtual bool CopiesFields
	{
		get
		{
			return true;
		}
	}

	public virtual bool CopiesProperties
	{
		get
		{
			return false;
		}
	}

	public virtual Type CopiedType
	{
		get
		{
			return typeof(Component);
		}
	}

	public virtual void Copy(Type type, Component source, Component destination)
	{
		if (CopiesFields)
		{
			CopyFields(type, source, destination);
		}
		if (CopiesProperties)
		{
			CopyProperties(type, source, destination);
		}
	}

	private void CopyFields(Type type, Component source, Component destination)
	{
		FieldInfo[] fields = type.GetFields();
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			fieldInfo.SetValue(destination, fieldInfo.GetValue(source));
		}
	}

	private void CopyProperties(Type type, Component source, Component destination)
	{
		PropertyInfo[] properties = type.GetProperties();
		PropertyInfo[] array = properties;
		foreach (PropertyInfo propertyInfo in array)
		{
			if (propertyInfo.CanRead && propertyInfo.CanWrite && propertyInfo.GetIndexParameters().Length == 0 && propertyInfo.DeclaringType == type)
			{
				propertyInfo.SetValue(destination, propertyInfo.GetValue(source, null), null);
			}
		}
	}
}
