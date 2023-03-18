using System.Collections.Generic;
using UnityEngine;

public class VOInputHeroOrBoss : IVOInputResolver
{
	public void SetVOParams(string[] parameters)
	{
	}

	public string ResolveVOInput(GameObject emitter, IEnumerable<string> previousInputs)
	{
		string text = null;
		if (emitter == null)
		{
			foreach (string previousInput in previousInputs)
			{
				text = previousInput;
			}
		}
		else
		{
			text = VOEmitter.GetEmitterName(emitter);
		}
		if (text != null && text.Contains("_boss"))
		{
			return "Boss";
		}
		return "Hero";
	}
}
