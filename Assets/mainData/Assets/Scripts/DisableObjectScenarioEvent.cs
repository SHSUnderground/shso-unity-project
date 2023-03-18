using System.Collections;
using UnityEngine;

[AddComponentMenu("ScenarioEvent/Disable Object")]
public class DisableObjectScenarioEvent : ScenarioEventHandlerEnableBase
{
	protected override void OnEnableEvent(string eventName)
	{
		StartCoroutine(DisableObject());
	}

	private IEnumerator DisableObject()
	{
		yield return 0;
		Utils.ActivateTree(base.gameObject, false);
	}
}
