using System.Collections.Generic;
using UnityEngine;

public class CharacterPerfTest : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject[] characters;

	public string animName = "emote_greet";

	public bool addCharacterLod = true;

	public SkinQuality boneInfluences = SkinQuality.Bone4;

	private GameObject[] oldCharacters;

	private List<GameObject> currentCharacters;

	private SkinQuality oldBoneInfluences;

	public void Awake()
	{
		currentCharacters = new List<GameObject>();
	}

	public void Update()
	{
		if (characters != oldCharacters)
		{
			oldCharacters = characters;
			oldBoneInfluences = SkinQuality.Auto;
			foreach (GameObject currentCharacter in currentCharacters)
			{
				Object.Destroy(currentCharacter);
			}
			currentCharacters.Clear();
			if (characters != null && characters.Length > 0)
			{
				int num = 0;
				Object[] array = Resources.FindObjectsOfTypeAll(typeof(PlaceholderTest));
				foreach (Object @object in array)
				{
					PlaceholderTest placeholderTest = @object as PlaceholderTest;
					if (!(placeholderTest == null))
					{
						GameObject gameObject = Object.Instantiate(characters[num++ % characters.Length]) as GameObject;
						gameObject.transform.position = placeholderTest.transform.position;
						gameObject.transform.rotation = placeholderTest.transform.rotation;
						currentCharacters.Add(gameObject);
						if (addCharacterLod)
						{
							gameObject.AddComponent<LodCharacter>();
						}
						if (gameObject.animation != null)
						{
							gameObject.animation[animName].wrapMode = WrapMode.Loop;
							gameObject.animation.Play(animName);
						}
					}
				}
			}
		}
		if (oldBoneInfluences != boneInfluences)
		{
			if (addCharacterLod)
			{
				oldBoneInfluences = SkinQuality.Auto;
				boneInfluences = SkinQuality.Auto;
			}
			else
			{
				oldBoneInfluences = boneInfluences;
				foreach (GameObject currentCharacter2 in currentCharacters)
				{
					SkinnedMeshRenderer componentInChildren = currentCharacter2.GetComponentInChildren<SkinnedMeshRenderer>();
					if (!(componentInChildren == null))
					{
						componentInChildren.quality = boneInfluences;
					}
				}
			}
		}
	}
}
