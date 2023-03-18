using UnityEngine;

[AddComponentMenu("Cut Scene Clips/Image Fade")]
public class CutSceneImageFadeEvent : CutSceneImageEvent
{
	public bool fadeIn;

	public float fadeStart;

	public float fadeEnd = -1f;

	public override void StartEvent()
	{
		base.StartEvent();
		if (fadeEnd < 0f)
		{
			fadeEnd = base.EventTime;
		}
		if (fadeIn)
		{
			eventImage.Alpha = 0f;
		}
		else
		{
			eventImage.Alpha = 1f;
		}
		AppShell.Instance.EventMgr.Fire(null, new CutSceneImageFadeEventMessage(this));
	}

	public override void UpdateEvent()
	{
		base.UpdateEvent();
		if (fadeStart > base.ElapsedTime)
		{
			return;
		}
		float num = fadeEnd - fadeStart;
		float num2 = base.ElapsedTime - fadeStart;
		if (fadeIn)
		{
			eventImage.Alpha = num2 / num;
			if (eventImage.Alpha > 1f)
			{
				eventImage.Alpha = 1f;
			}
		}
		else
		{
			eventImage.Alpha = 1f - num2 / num;
			if (eventImage.Alpha < 0f)
			{
				eventImage.Alpha = 0f;
			}
		}
	}
}
