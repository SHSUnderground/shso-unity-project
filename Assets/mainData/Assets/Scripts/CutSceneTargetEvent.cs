using UnityEngine;

internal class CutSceneTargetEvent : CutSceneEvent
{
	public string eventTarget = string.Empty;

	private GameObject mEventTarget;

	public GameObject Target
	{
		get
		{
			return mEventTarget;
		}
	}

	public override void StartEvent()
	{
		base.StartEvent();
		mEventTarget = GameObject.Find(eventTarget);
		if (mEventTarget == null)
		{
			LogEventError("Target object of event not found");
		}
	}
}
