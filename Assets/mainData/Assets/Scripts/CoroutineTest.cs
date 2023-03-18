using System;
using System.Collections;
using System.Reflection;
using UnityEngine;

public class CoroutineTest : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private IEnumerator PollingFunction()
	{
		CspUtils.DebugLog("0, frame = " + Time.frameCount);
		yield return 0;
		CspUtils.DebugLog("1, frame = " + Time.frameCount);
		yield return 1;
		CspUtils.DebugLog("2, frame = " + Time.frameCount);
		yield return 2;
	}

	private void Start()
	{
		IEnumerator enumerator = PollingFunction();
		Type type = enumerator.GetType();
		Type[] interfaces = type.GetInterfaces();
		Type[] array = interfaces;
		foreach (Type type2 in array)
		{
			CspUtils.DebugLog("Interface: " + type2.ToString());
		}
		MemberInfo[] members = type.GetMembers();
		MemberInfo[] array2 = members;
		foreach (MemberInfo memberInfo in array2)
		{
			CspUtils.DebugLog("Member: " + memberInfo.ToString());
		}
		PropertyInfo[] properties = type.GetProperties();
		PropertyInfo[] array3 = properties;
		foreach (PropertyInfo propertyInfo in array3)
		{
			CspUtils.DebugLog("Props: " + propertyInfo.ToString());
		}
		FieldInfo[] fields = type.GetFields();
		FieldInfo[] array4 = fields;
		foreach (FieldInfo fieldInfo in array4)
		{
			CspUtils.DebugLog("Fields: " + fieldInfo.ToString());
		}
		StartCoroutine(new PollingObject());
	}
}
