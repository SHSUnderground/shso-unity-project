using UnityEngine;

[AddComponentMenu("ScenarioEvent/Object Toggle")]
public class ObjectToggleScenarioEvent : ScenarioEventHandlerEnableBase
{
	public GameObject objectToToggle;

	public bool startDisabled = true;

	public bool invert;

	private void Awake()
	{
		if (startDisabled)
		{
			objectToToggle.active = false;
		}
	}

	protected override void OnEnableEvent(string eventName)
	{
		objectToToggle.active = !invert;
	}

	protected override void OnDisableEvent(string eventName)
	{
		objectToToggle.active = invert;
	}
}
