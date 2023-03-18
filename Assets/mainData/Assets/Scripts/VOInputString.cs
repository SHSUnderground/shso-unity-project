using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VOInputString : IEnumerable, IEnumerable<IVOInputResolver>, IVOInputResolver
{
	private string value;

	public VOInputString()
	{
	}

	public VOInputString(string value)
	{
		this.value = value;
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		yield return this;
	}

	public static IEnumerable<IVOInputResolver> FromStrings(params string[] values)
	{
		return Utils.Map((IEnumerable<string>)values, (Converter<string, IVOInputResolver>)delegate(string str)
		{
			return new VOInputString(str);
		});
	}

	public void SetVOParams(string[] parameters)
	{
		if (parameters.Length > 0)
		{
			value = parameters[0];
		}
	}

	public string ResolveVOInput(GameObject emitter, IEnumerable<string> previousInputs)
	{
		return value;
	}

	public IEnumerator<IVOInputResolver> GetEnumerator()
	{
		yield return this;
	}
}
