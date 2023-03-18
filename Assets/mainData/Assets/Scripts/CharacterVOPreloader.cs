using System.Collections;
using UnityEngine;

[AddComponentMenu("VO/Character VO Preloader")]
public class CharacterVOPreloader : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string characterName;

	private void Start()
	{
		if (!string.IsNullOrEmpty(characterName))
		{
			StartCoroutine(StartPreload());
		}
	}

	private IEnumerator StartPreload()
	{
		while (AppShell.Instance.BundleLoader.BundleGroups == null)
		{
			yield return 0;
		}
		Singleton<VOBundleLoader>.instance.LoadCharacter(characterName);
	}
}
