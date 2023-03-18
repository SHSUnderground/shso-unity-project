using System.Collections.Generic;
using UnityEngine;

public class FXPerfTest : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject[] prefabs;

	public int columns = 5;

	public int total = 40;

	public float columnSpacing = 1.5f;

	public float rowSpacing = 1.5f;

	public bool redraw;

	protected List<GameObject> spawnedObjects;

	public void Awake()
	{
		spawnedObjects = new List<GameObject>();
	}

	public void Update()
	{
		if (!redraw)
		{
			return;
		}
		redraw = false;
		foreach (GameObject spawnedObject in spawnedObjects)
		{
			if (spawnedObject != null)
			{
				Object.Destroy(spawnedObject);
			}
		}
		spawnedObjects.Clear();
		if (prefabs == null || prefabs.Length <= 0)
		{
			return;
		}
		int num = 0;
		int num2 = 0;
		while (num < total)
		{
			for (int i = 0; i < columns; i++)
			{
				GameObject gameObject = Object.Instantiate(prefabs[num++ % prefabs.Length]) as GameObject;
				gameObject.transform.parent = base.transform;
				gameObject.transform.position = new Vector3((float)i * columnSpacing, 0f, (float)num2 * rowSpacing);
				spawnedObjects.Add(gameObject);
				if (num >= total)
				{
					break;
				}
			}
			num2++;
		}
	}
}
