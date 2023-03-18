using UnityEngine;

[AddComponentMenu("Cut Scene Clips/VO")]
public class CutSceneVOEvent : CutSceneEvent
{
	public VOObject vo;

	public override void StartEvent()
	{
		base.StartEvent();
		if (vo != null)
		{
			vo.PlayVO();
		}
	}
}
