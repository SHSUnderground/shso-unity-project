using System.Collections;
using UnityEngine;

[AddComponentMenu("Brawler/Character Spawn Indicator Suppressor")]
[RequireComponent(typeof(CharacterSpawn))]
public class CharacterSpawnIndicatorSuppressor : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public bool spawnSuppressed = true;

	public string indicatorSuppressEvent = string.Empty;

	public string indicatorAttractEvent = string.Empty;

	protected CharacterSpawn characterSpawner;

	protected void OnCharacterSpawn(GameObject obj)
	{
		if (obj != null)
		{
			ScenarioEventIndicatorSuppressor scenarioEventIndicatorSuppressor = obj.AddComponent<ScenarioEventIndicatorSuppressor>();
			if (scenarioEventIndicatorSuppressor != null)
			{
				scenarioEventIndicatorSuppressor.indicatorSuppressed = spawnSuppressed;
				scenarioEventIndicatorSuppressor.enableEvent = indicatorSuppressEvent;
				scenarioEventIndicatorSuppressor.disableEvent = indicatorAttractEvent;
			}
		}
		StartCoroutine(AttachToSpawnCallback());
	}

	private IEnumerator AttachToSpawnCallback()
	{
		yield return 0;
		if (characterSpawner != null)
		{
			characterSpawner.onSpawnCallback += OnCharacterSpawn;
		}
	}

	private void Awake()
	{
		characterSpawner = base.gameObject.GetComponent<CharacterSpawn>();
		if (characterSpawner == null)
		{
			CspUtils.DebugLog("CharacterSpawnIndicatorSuppressor: found no character spawner on its game object and will not suppress indicators for any spawned characters");
		}
		else
		{
			StartCoroutine(AttachToSpawnCallback());
		}
	}
}
