using System.Collections.Generic;
using UnityEngine;

public class VOInputLocalPlayer : IVOInputResolver
{
	public void SetVOParams(string[] parameters)
	{
	}

	public string ResolveVOInput(GameObject emitter, IEnumerable<string> previousInputs)
	{
		return GameController.GetController().LocalPlayer.name;
	}
}
