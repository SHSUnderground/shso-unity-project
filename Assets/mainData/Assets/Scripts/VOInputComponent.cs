using System.Collections.Generic;
using UnityEngine;

public abstract class VOInputComponent : MonoBehaviour, IVOInputResolver
{
	public void SetVOParams(string[] parameters)
	{
	}

	public abstract string ResolveVOInput(GameObject emitter, IEnumerable<string> previousInputs);
}
