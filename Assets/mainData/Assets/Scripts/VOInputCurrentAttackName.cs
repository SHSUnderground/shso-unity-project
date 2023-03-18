using System.Collections.Generic;
using UnityEngine;

public class VOInputCurrentAttackName : IVOInputResolver
{
	public void SetVOParams(string[] parameters)
	{
	}

	public string ResolveVOInput(GameObject emitter, IEnumerable<string> previousInputs)
	{
		BehaviorManager component = emitter.GetComponent<BehaviorManager>();
		if (component != null)
		{
			BehaviorAttackBase behaviorAttackBase = component.getBehavior() as BehaviorAttackBase;
			if (behaviorAttackBase != null)
			{
				return behaviorAttackBase.AttackName;
			}
		}
		return string.Empty;
	}
}
