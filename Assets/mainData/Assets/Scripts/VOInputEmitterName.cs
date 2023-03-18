using System.Collections.Generic;
using UnityEngine;

public class VOInputEmitterName : IVOInputResolver
{
	public void SetVOParams(string[] parameters)
	{
	}

	public string ResolveVOInput(GameObject emitter, IEnumerable<string> previousInputs)
	{
		return VOEmitter.GetEmitterName(emitter);
	}
}
