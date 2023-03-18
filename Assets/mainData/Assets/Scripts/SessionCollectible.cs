using System.Collections;
using UnityEngine;

[AddComponentMenu("Social Space/Session-based Collectible Spawner")]
public class SessionCollectible : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject prefabToSpawn;

	private string _cachedUniqueID;

	private GameObject instance;

	public bool HasBeenCollected
	{
		get
		{
			if (AppShell.Instance == null || AppShell.Instance.Profile == null)
			{
				return false;
			}
			bool value;
			return AppShell.Instance.Profile.CollectedSessionCollectibles.TryGetValue(UniqueID, out value) && value;
		}
		set
		{
			if (AppShell.Instance != null && AppShell.Instance.Profile != null)
			{
				AppShell.Instance.Profile.CollectedSessionCollectibles[UniqueID] = true;
			}
		}
	}

	public string UniqueID
	{
		get
		{
			if (_cachedUniqueID == null)
			{
				_cachedUniqueID = GetLineage(base.transform, "|") + base.transform.position.ToString();
			}
			return _cachedUniqueID;
		}
	}

	public void Start()
	{
		if (!HasBeenCollected)
		{
			SpawnPrefab();
		}
	}

	public void SpawnPrefab()
	{
		if (prefabToSpawn != null && instance == null)
		{
			instance = (Object.Instantiate(prefabToSpawn) as GameObject);
			if (instance != null)
			{
				Utils.AttachGameObject(base.gameObject, instance);
				StartCoroutine(TrackInstance());
			}
		}
	}

	private IEnumerator TrackInstance()
	{
		while (instance != null)
		{
			yield return 0;
		}
		yield return 0;
		HasBeenCollected = true;
	}

	private string GetLineage(Transform node, string separator)
	{
		if (node == null)
		{
			return string.Empty;
		}
		return GetLineage(node.parent, separator) + separator + node.name;
	}
}
