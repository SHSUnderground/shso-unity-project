using System.Collections.Generic;
using UnityEngine;

public class CharacterPerfTest2 : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject[] characters;

	public string animName = "emote_greet";

	public bool addCharacterLod = true;

	public SkinQuality boneInfluences = SkinQuality.Bone4;

	public int columns = 5;

	public int total = 40;

	public float columnSpacing = 1.5f;

	public float rowSpacing = 1.5f;

	public bool redraw;

	private List<GameObject> currentCharacters;

	private SkinQuality oldBoneInfluences;

	public void Awake()
	{
		currentCharacters = new List<GameObject>();
		redraw = true;
	}

	public void Update()
	{
		if (redraw)
		{
			redraw = false;
			foreach (GameObject currentCharacter in currentCharacters)
			{
				Object.Destroy(currentCharacter);
			}
			currentCharacters.Clear();
			if (characters != null && characters.Length > 0)
			{
				int num = 0;
				int num2 = 0;
				while (num < total)
				{
					for (int i = 0; i < columns; i++)
					{
						GameObject gameObject = Object.Instantiate(characters[num++ % characters.Length]) as GameObject;
						gameObject.transform.position = new Vector3((float)i * columnSpacing, 0f, (float)num2 * rowSpacing);
						currentCharacters.Add(gameObject);
						if (addCharacterLod)
						{
							gameObject.AddComponent<LodCharacter>();
						}
						if (gameObject.animation != null)
						{
							gameObject.animation[animName].wrapMode = WrapMode.Loop;
							gameObject.animation.Play(animName);
							gameObject.animation[animName].time = Random.Range(0f, gameObject.animation[animName].length);
						}
						if (num >= total)
						{
							break;
						}
					}
					num2++;
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
