using UnityEngine;

public class SetReturnSpawnGroup : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private void Triggered()
	{
		SpawnPointGroup spawnPointGroup = null;
		GameObject gameObject = base.gameObject;
		while (gameObject != null)
		{
			spawnPointGroup = Utils.GetComponent<SpawnPointGroup>(gameObject);
			if (spawnPointGroup != null)
			{
				break;
			}
			gameObject = gameObject.transform.parent.gameObject;
		}
		if (spawnPointGroup != null)
		{
			CspUtils.DebugLog("Setting return spawn point to: " + spawnPointGroup.group);
			AppShell.Instance.SharedHashTable["SocialSpaceSpawnPoint"] = spawnPointGroup.group;
		}
		else
		{
			CspUtils.DebugLog(base.gameObject.name + " does not have an associated Spawn Point Group");
			AppShell.Instance.SharedHashTable["SocialSpaceSpawnPoint"] = null;
		}
	}
}
