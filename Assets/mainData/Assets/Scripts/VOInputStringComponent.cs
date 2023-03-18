using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("VO/Inputs/String")]
public class VOInputStringComponent : VOInputComponent
{
	public string value;

	public override string ResolveVOInput(GameObject emitter, IEnumerable<string> previousInputs)
	{
		return value;
	}
}
