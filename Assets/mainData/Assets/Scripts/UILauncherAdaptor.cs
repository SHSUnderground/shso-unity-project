using UnityEngine;

public class UILauncherAdaptor : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	protected bool LauncherInUse;

	public LauncherTypeEnum LauncherType;

	public virtual void Triggered()
	{
		if (LauncherInUse)
		{
			CspUtils.DebugLog("Attempting to use a launcher already in use.");
			return;
		}
		LauncherInUse = true;
		LauncherSequences.InitiateLaunchSequence(this);
	}

	public virtual void LaunchSequenceComplete()
	{
		LauncherInUse = false;
	}
}
