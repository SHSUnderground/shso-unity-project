using UnityEngine;

public class StarPickupActivityObject : ActivityObject
{
	public GameObject soundEffectPrefab;

	public override void ActionTriggered(ActivityObjectActionNameEnum action)
	{
		if (AppShell.Instance.Profile != null)
		{
			AppShell.Instance.Profile.AddStars(1);
		}
		ShsAudioSource.PlayAutoSound(soundEffectPrefab, base.transform);
		Object.Destroy(base.gameObject);
	}
}
