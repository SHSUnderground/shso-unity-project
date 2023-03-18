using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public int idxSpawn;

	public GameObject[] playerPrefabs;

	protected GameObject player;

	protected int idxCurrent;

	private void Awake()
	{
		StartPlayer(idxSpawn);
	}

	public void StartPlayer(int idx)
	{
		Object[] array = Resources.FindObjectsOfTypeAll(typeof(CameraTarget));
		MeshRenderer meshRenderer = GetComponent(typeof(MeshRenderer)) as MeshRenderer;
		if (meshRenderer != null)
		{
			meshRenderer.enabled = false;
		}
		if (player != null)
		{
			Object[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				CameraTarget cameraTarget = (CameraTarget)array2[i];
				if (cameraTarget.Target == player.transform)
				{
					cameraTarget.Target = null;
				}
			}
			Object.Destroy(player);
			player = null;
		}
		if (idx < 0)
		{
			idx = (idxCurrent + 1) % playerPrefabs.Length;
		}
		if (idx >= 0 && idx < playerPrefabs.Length)
		{
			GameObject gameObject = playerPrefabs[idx];
			if (gameObject != null)
			{
				player = (Object.Instantiate(gameObject, base.transform.position, base.transform.rotation) as GameObject);
				idxCurrent = idx;
			}
		}
		if (player == null)
		{
			CspUtils.DebugLog("Unable to instantiate player " + playerPrefabs[idx].name);
			return;
		}
		array = Resources.FindObjectsOfTypeAll(typeof(CameraTarget));
		Object[] array3 = array;
		for (int j = 0; j < array3.Length; j++)
		{
			CameraTarget cameraTarget2 = (CameraTarget)array3[j];
			if (cameraTarget2.Target == null || cameraTarget2.Target == base.gameObject.transform)
			{
				cameraTarget2.Target = player.transform;
			}
			if (!(cameraTarget2.gameObject == null))
			{
				CameraLite cameraLite = cameraTarget2.gameObject.GetComponent(typeof(CameraLite)) as CameraLite;
				if (cameraLite != null)
				{
					cameraLite.Reset();
				}
			}
		}
	}
}
