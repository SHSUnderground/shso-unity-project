using UnityEngine;

public class ActivityObjectTriggerAdaptor : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public ActivityObjectActionNameEnum actionName;

	public void Triggered()
	{
		ActivityObject activityObject = null;
		GameObject gameObject = base.gameObject;
		while (gameObject != null)
		{
			activityObject = Utils.GetComponent<ActivityObject>(gameObject);
			if (activityObject != null)
			{
				break;
			}
			gameObject = gameObject.transform.parent.gameObject;
		}
		if (activityObject == null)
		{
			CspUtils.DebugLog("No activity object found on <" + base.gameObject.name + ">");
		}
		else if (activityObject.isActivated)
		{
			activityObject.ActionTriggered(actionName);
		}
	}
}
