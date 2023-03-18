using System.Collections.Generic;
using UnityEngine;

public interface IVOInputResolver
{
	void SetVOParams(string[] parameters);

	string ResolveVOInput(GameObject emitter, IEnumerable<string> previousInputs);
}
