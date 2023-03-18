using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DetectLeaks : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private Dictionary<int, GameObject> objects = new Dictionary<int, GameObject>();

	private void Start()
	{
		GameObject[] array = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			objects[gameObject.GetInstanceID()] = gameObject;
		}
		InvokeRepeating("LogObjects", 15f, 15f);
		Profiler.logFile = "UnityProfile.log";
		Profiler.enabled = true;
	}

	private void LogObjects()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("Time: " + Time.time);
		stringBuilder.Append("\nAll: " + Resources.FindObjectsOfTypeAll(typeof(Object)).Length);
		stringBuilder.Append("\nTextures: " + Resources.FindObjectsOfTypeAll(typeof(Texture)).Length);
		stringBuilder.Append("\nAudioClips: " + Resources.FindObjectsOfTypeAll(typeof(AudioClip)).Length);
		stringBuilder.Append("\nMeshes: " + Resources.FindObjectsOfTypeAll(typeof(Mesh)).Length);
		stringBuilder.Append("\nMaterials: " + Resources.FindObjectsOfTypeAll(typeof(Material)).Length);
		stringBuilder.Append("\nGameObjects: " + Resources.FindObjectsOfTypeAll(typeof(GameObject)).Length);
		stringBuilder.Append("\nComponents: " + Resources.FindObjectsOfTypeAll(typeof(Component)).Length);
		int num = 0;
		int num2 = 0;
		GameObject[] array = Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[];
		GameObject[] array2 = array;
		foreach (GameObject gameObject in array2)
		{
			if (objects.ContainsKey(gameObject.GetInstanceID()))
			{
				num++;
			}
			else
			{
				num2++;
			}
		}
		stringBuilder.Append("\nOld " + num);
		stringBuilder.Append("\nNew " + num2);
		stringBuilder.Append("\nUsedHeapSize: " + Profiler.usedHeapSize);
		CspUtils.DebugLog(stringBuilder.ToString());
	}
}
