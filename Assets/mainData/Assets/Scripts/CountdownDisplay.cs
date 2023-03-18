using UnityEngine;

public class CountdownDisplay : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public MultiHotspotCoordinator hotspotCoordinator;

	public FlipBookAnimator flipBookAnimator;

	public void Update()
	{
		if (!(hotspotCoordinator == null) && !(flipBookAnimator == null))
		{
			float num = (float)flipBookAnimator.frames - hotspotCoordinator.TotalCountDownTime;
			if (hotspotCoordinator.LaunchTime > Time.time)
			{
				num += hotspotCoordinator.TotalCountDownTime - hotspotCoordinator.DisplayTime;
			}
			flipBookAnimator.time = num;
			Utils.ActivateTree(flipBookAnimator.gameObject, true);
		}
	}
}
