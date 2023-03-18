using UnityEngine;

public class MagneticActivityObjectTriggerAdapter : MagneticTriggerAdapter
{
	protected ActivityObject activityObject;

	protected override GameObject GetCollectibleObject()
	{
		activityObject = null;
		GameObject gameObject = base.gameObject;
		while (gameObject != null)
		{
			activityObject = Utils.GetComponent<ActivityObject>(gameObject);
			if (activityObject != null || gameObject.transform.parent == null)
			{
				break;
			}
			gameObject = gameObject.transform.parent.gameObject;
		}
		if (activityObject == null)
		{
			CspUtils.DebugLog("No activity object found on <" + base.gameObject.name + ">");
			return null;
		}
		return activityObject.gameObject;
	}

	protected override void OnCollected(GameObject player)
	{
		activityObject.ActionTriggered(ActivityObjectActionNameEnum.Collision);
	}
}
