using UnityEngine;

public class ScenarioEventLightsOut : ScenarioEventHandlerEnableBase
{
	public LightsOutMaster targetLightControl;

	public bool toggle;

	public bool forceState;

	protected override void OnEnableEvent(string eventName)
	{
		if (targetLightControl != null)
		{
			if (toggle)
			{
				targetLightControl.lightsOn = !targetLightControl.lightsOn;
			}
			else
			{
				targetLightControl.lightsOn = forceState;
			}
			targetLightControl.UpdateLightingEffect();
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "LightsOutEvent.png");
	}
}
