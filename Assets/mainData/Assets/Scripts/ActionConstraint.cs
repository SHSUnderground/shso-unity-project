using System;
using System.Reflection;
using UnityEngine;

[Serializable]
public class ActionConstraint
{
	public string ComponentName;

	public string FieldName;

	public float Value;

	public bool Passes(GameObject obj)
	{
		MonoBehaviour monoBehaviour = null;
		foreach (Transform item in Utils.WalkTree(obj.transform))
		{
			if (item != null)
			{
				monoBehaviour = (item.gameObject.GetComponent(ComponentName) as MonoBehaviour);
				if (monoBehaviour != null)
				{
					break;
				}
			}
		}
		if (monoBehaviour != null)
		{
			Type type = monoBehaviour.GetType();
			MemberInfo memberInfo = type.GetMember(FieldName)[0];
			if (memberInfo is PropertyInfo)
			{
				return float.Parse(((PropertyInfo)memberInfo).GetValue(monoBehaviour, null).ToString()) >= Value;
			}
			if (memberInfo is FieldInfo)
			{
				return float.Parse(((FieldInfo)memberInfo).GetValue(monoBehaviour).ToString()) >= Value;
			}
		}
		return false;
	}
}
