using UnityEngine;

public class CameraLiteAL : CameraLite
{
	public override void UpdateAudioFromMgr(float deltaTime)
	{
		if (target != null)
		{
			audioPosition = target.transform.position;
			audioRotation = Quaternion.LookRotation(base.transform.forward);
		}
		else
		{
			base.UpdateAudioFromMgr(deltaTime);
		}
	}
}
