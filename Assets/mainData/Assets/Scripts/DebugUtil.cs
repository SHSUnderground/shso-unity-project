using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

internal class DebugUtil
{
	private const string LineIndent = "      ";

	public static UnityEngine.Object[] GetTopLevelObjects()
	{
		List<Transform> list = new List<Transform>();
		Transform[] array = (Transform[])UnityEngine.Object.FindObjectsOfType(typeof(Transform));
		Transform[] array2 = array;
		foreach (Transform transform in array2)
		{
			Transform root = transform.root;
			if (!list.Contains(root))
			{
				list.Add(root);
			}
		}
		UnityEngine.Object[] array3 = new UnityEngine.Object[list.Count];
		for (int j = 0; j < list.Count; j++)
		{
			if (list[j] == null)
			{
				array3[j] = null;
			}
			else
			{
				array3[j] = list[j].gameObject;
			}
		}
		Array.Sort(array3, delegate(UnityEngine.Object o1, UnityEngine.Object o2)
		{
			return string.Compare(o1.name, o2.name);
		});
		return array3;
	}

	public static UnityEngine.Object[] GetUnityObjectsWithName(string ChildName, UnityEngine.Object Parent)
	{
		List<UnityEngine.Object> list = new List<UnityEngine.Object>();
		if (Parent == null)
		{
			Transform[] array = (Transform[])UnityEngine.Object.FindObjectsOfType(typeof(Transform));
			Transform[] array2 = array;
			foreach (Transform transform in array2)
			{
				if (string.Compare(transform.name, ChildName) == 0)
				{
					list.Add(transform.gameObject);
				}
			}
		}
		else
		{
			GameObject gameObject = Parent as GameObject;
			if (gameObject == null)
			{
				return new UnityEngine.Object[0];
			}
			int childCount = gameObject.transform.childCount;
			for (int j = 0; j < childCount; j++)
			{
				if (string.Compare(gameObject.transform.GetChild(j).name, ChildName) == 0)
				{
					list.Add(gameObject.transform.GetChild(j).gameObject);
				}
			}
			if (list.Count == 0)
			{
				Component[] components = gameObject.transform.GetComponents(typeof(Component));
				Component[] array3 = components;
				foreach (Component component in array3)
				{
					if (string.Compare(component.GetType().ToString(), ChildName) == 0)
					{
						list.Add(component);
					}
				}
			}
		}
		if (list.Count == 0 && ChildName[ChildName.Length - 1] == ']')
		{
			int num = ChildName.IndexOf('[');
			if (num > 0)
			{
				string childName = ChildName.Substring(0, num);
				return GetUnityObjectsWithName(childName, Parent);
			}
		}
		return list.ToArray();
	}

	public static bool GetMemberWithName(string ChildName, object Parent, out object ResultingValue)
	{
		bool result = false;
		ResultingValue = null;
		Type type = Parent.GetType();
		int num = IndexFromName(ChildName);
		FieldInfo field = type.GetField(ChildName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		if (field == null && num >= 0)
		{
			ChildName = ChildName.Substring(0, ChildName.IndexOf('['));
			field = type.GetField(ChildName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			if (field.FieldType == typeof(Array))
			{
				Array array = (Array)field.GetValue(Parent);
				if (array != null)
				{
					ResultingValue = array.GetValue(num);
					result = true;
				}
			}
		}
		else if (field != null)
		{
			ResultingValue = field.GetValue(Parent);
			result = true;
		}
		return result;
	}

	private static int IndexFromName(string Name)
	{
		int result = -1;
		if (Name[Name.Length - 1] == ']')
		{
			int num = Name.IndexOf('[');
			if (num > 0 && !int.TryParse(Name.Substring(num + 1, Name.Length - num - 2), out result))
			{
				result = -1;
			}
		}
		return result;
	}

	public static bool GetObjectFromObjPathString(object Context, string Children, out string Error, out object ResultingValue)
	{
		Error = string.Empty;
		string[] array = Children.Split('/');
		if (Children.Length == 0 || array.Length == 0)
		{
			ResultingValue = Context;
			return true;
		}
		string text = array[0];
		object ResultingValue2 = null;
		if (Context is GameObject || Context == null)
		{
			UnityEngine.Object[] unityObjectsWithName = GetUnityObjectsWithName(text, (GameObject)Context);
			if (unityObjectsWithName.Length == 0)
			{
				Error = "ERROR: Couldn't find object or component named \"" + text + "\"";
				ResultingValue = null;
				return false;
			}
			if (unityObjectsWithName.Length > 1)
			{
				if (IndexFromName(text) < 0)
				{
					Error = "ERROR: More than one \"" + text + "\" found.  Must specify an indexer.";
					ResultingValue = null;
					return false;
				}
				ResultingValue2 = unityObjectsWithName[IndexFromName(text)];
			}
			else
			{
				ResultingValue2 = unityObjectsWithName[0];
			}
		}
		else if (!GetMemberWithName(text, Context, out ResultingValue2))
		{
			Error = "ERROR: Couldn't find object member named \"" + text + "\"";
		}
		else if (ResultingValue2 == null)
		{
			if (Children.Length == text.Length)
			{
				ResultingValue = null;
				return true;
			}
			Error = "ERROR: \"" + text + "\" is null\n";
		}
		if (ResultingValue2 != null)
		{
			return GetObjectFromObjPathString(ResultingValue2, Children.Substring(Math.Min(Children.Length, text.Length + 1)), out Error, out ResultingValue);
		}
		ResultingValue = null;
		return false;
	}

	public static void DumpArray(StringBuilder sb, int Indent, Array a)
	{
		string text = string.Empty;
		for (int i = 0; i < Indent; i++)
		{
			text += " ";
		}
		for (int j = 0; j < a.Length; j++)
		{
			sb.Append(text + "[" + j + "]: " + a.GetValue(j).ToString() + "\n");
		}
	}

	public static string Dump(string Obj, bool OutputErrors)
	{
		string Error = string.Empty;
		StringBuilder stringBuilder = new StringBuilder();
		if (string.IsNullOrEmpty(Obj))
		{
			stringBuilder.Append("      Top-Level Scene Objects\n");
			stringBuilder.Append("      ------------------------------------------------------------\n");
			UnityEngine.Object[] topLevelObjects = GetTopLevelObjects();
			UnityEngine.Object[] array = topLevelObjects;
			foreach (UnityEngine.Object @object in array)
			{
				stringBuilder.Append("      " + @object.name + "\n");
			}
			return stringBuilder.ToString();
		}
		object ResultingValue = null;
		if (!GetObjectFromObjPathString(null, Obj, out Error, out ResultingValue))
		{
			if (OutputErrors)
			{
				return Error;
			}
			return string.Empty;
		}
		if (ResultingValue is GameObject)
		{
			GameObject gameObject = (GameObject)ResultingValue;
			stringBuilder.Append("      Childen\n");
			stringBuilder.Append("      ------------------------------------------------------------\n");
			int childCount = gameObject.transform.childCount;
			if (childCount == 0)
			{
				stringBuilder.Append("      <no children found>\n");
			}
			else
			{
				for (int j = 0; j < childCount; j++)
				{
					Transform child = gameObject.transform.GetChild(j);
					stringBuilder.Append("      " + child.name + "\n");
				}
			}
			stringBuilder.Append("\n");
			stringBuilder.Append("      Components\n");
			stringBuilder.Append("      ------------------------------------------------------------\n");
			Component[] components = gameObject.GetComponents(typeof(Component));
			if (components == null || components.Length == 0)
			{
				stringBuilder.Append("      <no components found>\n");
			}
			else
			{
				Component[] array2 = components;
				foreach (Component component in array2)
				{
					if (component != null)
					{
						stringBuilder.Append("      " + component.GetType().ToString() + "\n");
					}
					else
					{
						stringBuilder.Append("      <null component>\n");
					}
				}
			}
		}
		else if (ResultingValue == null)
		{
			stringBuilder.Append("      null\n");
		}
		else if (ResultingValue.GetType().IsPrimitive)
		{
			stringBuilder.Append("      " + ResultingValue.ToString());
		}
		else
		{
			stringBuilder.Append("      Members of " + ResultingValue.ToString() + " (" + ResultingValue.GetType().ToString() + ")\n");
			stringBuilder.Append("      ------------------------------------------------------------\n");
			FieldInfo[] fields = ResultingValue.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
			Array.Sort(fields, delegate(FieldInfo f1, FieldInfo f2)
			{
				return string.Compare(f1.Name, f2.Name);
			});
			FieldInfo[] array3 = fields;
			foreach (FieldInfo fieldInfo in array3)
			{
				object value = fieldInfo.GetValue(ResultingValue);
				stringBuilder.Append("      " + fieldInfo.Name + ": " + ((value != null) ? value.ToString() : "null") + "\n");
				if (value is Array)
				{
					DumpArray(stringBuilder, 6 + fieldInfo.Name.Length, (Array)value);
				}
			}
		}
		return stringBuilder.ToString();
	}
}
