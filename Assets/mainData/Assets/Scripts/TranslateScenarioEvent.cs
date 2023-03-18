using UnityEngine;

[AddComponentMenu("ScenarioEvent/Translate")]
public class TranslateScenarioEvent : ScenarioEventHandlerEnableBase
{
	public Vector3 translatePosition;

	public Vector3 translateRotation;

	public float translationTime = 1f;

	public bool useParentTransform;

	protected float translationEndTime;

	protected Vector3 positionStart;

	protected Vector3 rotationStart;

	protected Vector3 positionTarget;

	protected Vector3 rotationTarget;

	private void Update()
	{
		if (!(translationEndTime > 0f))
		{
			return;
		}
		float num = 1f;
		if (translationTime > 0f)
		{
			num = 1f - (translationEndTime - Time.time) / translationTime;
			if (num > 1f)
			{
				num = 1f;
			}
		}
		base.transform.localPosition = Vector3.Lerp(positionStart, positionTarget, num);
		base.transform.localEulerAngles = Vector3.Lerp(rotationStart, rotationTarget, num);
		if (num == 1f)
		{
			translationEndTime = 0f;
		}
	}

	protected override void OnEnableEvent(string eventName)
	{
		positionStart = base.transform.localPosition;
		rotationStart = base.transform.localEulerAngles;
		if (!useParentTransform)
		{
			positionTarget = base.transform.localPosition + translatePosition;
			rotationTarget = base.transform.localEulerAngles + translateRotation;
		}
		else
		{
			positionTarget = base.transform.parent.localPosition + translatePosition;
			rotationTarget = base.transform.parent.localEulerAngles + translateRotation;
		}
		translationEndTime = Time.time + translationTime;
	}
}
