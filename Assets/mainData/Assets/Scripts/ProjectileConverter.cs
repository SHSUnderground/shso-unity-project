using UnityEngine;

[AddComponentMenu("Brawler/ProjectileConverter")]
public class ProjectileConverter : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string objectName;

	public GameObject projectilePrefab;

	public Vector3 spawnOffset;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public GameObject CreateProjectile()
	{
		if (projectilePrefab == null)
		{
			CspUtils.DebugLog("projectilePrefab is null for ProjectileConverter on " + base.gameObject.name);
			return null;
		}
		GameObject result = Object.Instantiate(projectilePrefab, base.transform.position + spawnOffset, base.transform.rotation) as GameObject;
		Object.Destroy(base.gameObject);
		return result;
	}
}
