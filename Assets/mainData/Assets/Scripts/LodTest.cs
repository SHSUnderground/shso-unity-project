using UnityEngine;

public class LodTest : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject characterModel;

	public PlaceholderTest lod0;

	public PlaceholderTest lod1;

	public PlaceholderTest lod2;

	public PlaceholderTest lod3;

	public PlaceholderTest lodAnim;

	public string staticAnimation = "movement_idle";

	public string movingAnimation = "movement_run";

	public float speed = 5f;

	protected GameObject _characterModel;

	protected GameObject[] currentCharacters = new GameObject[5];

	protected float direction = 1f;

	public void Update()
	{
		if (characterModel != _characterModel)
		{
			_characterModel = characterModel;
			if (characterModel == null)
			{
				return;
			}
			GameObject gameObject;
			for (int i = 0; i < currentCharacters.Length; i++)
			{
				gameObject = currentCharacters[i];
				if (gameObject != null)
				{
					Object.Destroy(gameObject);
					currentCharacters[i] = null;
				}
			}
			gameObject = (Object.Instantiate(characterModel) as GameObject);
			gameObject.transform.parent = lod0.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			LodCharacter lodCharacter = gameObject.AddComponent<LodCharacter>();
			lodCharacter.ForceSetLod(0);
			currentCharacters[0] = gameObject;
			if (gameObject.animation[staticAnimation] != null)
			{
				gameObject.animation[staticAnimation].wrapMode = WrapMode.Loop;
				gameObject.animation.Play(staticAnimation);
			}
			gameObject = (Object.Instantiate(characterModel) as GameObject);
			gameObject.transform.parent = lod1.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			lodCharacter = gameObject.AddComponent<LodCharacter>();
			lodCharacter.ForceSetLod(1);
			currentCharacters[1] = gameObject;
			if (gameObject.animation[staticAnimation] != null)
			{
				gameObject.animation[staticAnimation].wrapMode = WrapMode.Loop;
				gameObject.animation.Play(staticAnimation);
			}
			gameObject = (Object.Instantiate(characterModel) as GameObject);
			gameObject.transform.parent = lod2.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			lodCharacter = gameObject.AddComponent<LodCharacter>();
			lodCharacter.ForceSetLod(2);
			currentCharacters[2] = gameObject;
			if (gameObject.animation[staticAnimation] != null)
			{
				gameObject.animation[staticAnimation].wrapMode = WrapMode.Loop;
				gameObject.animation.Play(staticAnimation);
			}
			gameObject = (Object.Instantiate(characterModel) as GameObject);
			gameObject.transform.parent = lod3.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			lodCharacter = gameObject.AddComponent<LodCharacter>();
			lodCharacter.ForceSetLod(3);
			currentCharacters[3] = gameObject;
			if (gameObject.animation[staticAnimation] != null)
			{
				gameObject.animation[staticAnimation].wrapMode = WrapMode.Loop;
				gameObject.animation.Play(staticAnimation);
			}
			gameObject = (Object.Instantiate(characterModel) as GameObject);
			gameObject.transform.parent = lodAnim.transform;
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			lodCharacter = gameObject.AddComponent<LodCharacter>();
			currentCharacters[4] = gameObject;
			if (gameObject.animation[movingAnimation] != null)
			{
				gameObject.animation[movingAnimation].wrapMode = WrapMode.Loop;
				gameObject.animation.Play(movingAnimation);
			}
		}
		float z = speed * Time.deltaTime * direction;
		lodAnim.transform.position += new Vector3(0f, 0f, z);
		Vector3 position = lodAnim.transform.position;
		if (position.z > 35f)
		{
			direction = -1f;
			return;
		}
		Vector3 position2 = lodAnim.transform.position;
		if (position2.z < 0f)
		{
			direction = 1f;
		}
	}
}
