using UnityEngine;

[AddComponentMenu("ScenarioEvent/Transition to Game World")]
public class ScenarioEventTransitionToGameWorld : ScenarioEventHandlerEnableBase
{
	protected override void OnEnableEvent(string eventName)
	{
		AppShell.Instance.Transition(GameController.ControllerType.SocialSpace);
	}
}
