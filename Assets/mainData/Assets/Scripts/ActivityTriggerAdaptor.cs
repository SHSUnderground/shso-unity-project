using UnityEngine;

public class ActivityTriggerAdaptor : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject target;

	public string activityName;

	public void Triggered()
	{
		ISHSActivity activity = AppShell.Instance.ActivityManager.GetActivity(activityName);
		if (activity == null)
		{
			CspUtils.DebugLog("Activity: " + activityName + " Does not exist.");
			return;
		}
		if (target == null)
		{
			CspUtils.DebugLog("No associated rally path for this rally launcher");
			return;
		}
		RallyPath component = Utils.GetComponent<RallyPath>(target);
		if (component == null)
		{
			CspUtils.DebugLog("Associated path is not a RallyPath");
			return;
		}
		string text = component.duration.ToString();
		float @float = PlayerPrefs.GetFloat(activityName, 0f);
		GUIManager.Instance.ShowDialog(GUIManager.DialogTypeEnum.OkDialog, "Welcome to: " + activityName + "!\n\n Race through the rally points in " + text + " seconds, and you'll earn a ticket!!!!\n\n Previous Best Time: " + ((@float != 0f) ? (@float + " seconds") : " Never played on this machine"), new GUIDialogNotificationSink(delegate
		{
		}, delegate
		{
		}, delegate
		{
		}, delegate
		{
		}, delegate(string Id, GUIDialogWindow.DialogState state)
		{
			if (state != GUIDialogWindow.DialogState.Cancel)
			{
				activity.Start(target);
			}
		}), GUIControl.ModalLevelEnum.None);
	}
}
