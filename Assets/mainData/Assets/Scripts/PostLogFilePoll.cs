using System.Collections;
using UnityEngine;

internal class PostLogFilePoll : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public WWW www;

	public void OnDisable()
	{
		if (www != null)
		{
			www.Dispose();
			www = null;
		}
	}

	public void Start()
	{
		StartCoroutine(PollWWW());
	}

	protected IEnumerator PollWWW()
	{
		if (www != null)
		{
			yield return www;
			CspUtils.DebugLog("Finished sending log file");
		}
		PostLogFile.IsDone = true;
		yield return 0;
		Object.DestroyObject(this);
	}
}
