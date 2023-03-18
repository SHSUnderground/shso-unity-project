using System.Collections;
using UnityEngine;

[AddComponentMenu("Lab/Character/Spawner Glue")]
[RequireComponent(typeof(CharacterSpawn))]
public class CharacterSpawnerGlue : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject SpawnedCharacter;

	private void Start()
	{
		CharacterSpawn component = Utils.GetComponent<CharacterSpawn>(this);
		AppShell.Instance.EventMgr.AddListener(component, delegate(EntitySpawnMessage msg)
		{
			StartCoroutine(ReceiveTarget(msg.go));
		});
	}

	private IEnumerator ReceiveTarget(GameObject go)
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		SpawnedCharacter = go;
	}
}
