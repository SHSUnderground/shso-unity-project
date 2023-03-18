using UnityEngine;

public class SpawnAnimateData
{
	public GameObject spawnerSource;

	public Vector3 spawnDest;

	public SpawnAnimateData(GameObject source, Vector3 destination)
	{
		spawnerSource = source;
		spawnDest = destination;
	}
}
