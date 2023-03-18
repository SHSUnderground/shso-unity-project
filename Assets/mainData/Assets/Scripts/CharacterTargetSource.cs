using UnityEngine;

[AddComponentMenu("Lab/Character/Target Source")]
public class CharacterTargetSource : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public GameObject Target;

	public string TargetName;

	public bool TargetAnyAvailable;

	public GameObject Character;

	private void OnEnable()
	{
		Character = null;
		Update();
	}

	private void Update()
	{
		Character = FindCharacter();
		if (Character != null)
		{
			base.enabled = false;
		}
	}

	private GameObject FindCharacter()
	{
		if (Target != null && Utils.GetComponent<BehaviorManager>(Target) != null)
		{
			return Target;
		}
		if (Target != null && Utils.GetComponent<CharacterTargetSource>(Target) != null)
		{
			return Utils.GetComponent<CharacterTargetSource>(Target).Character;
		}
		if (Utils.GetComponent<CharacterSpawnerGlue>(this) != null)
		{
			return Utils.GetComponent<CharacterSpawnerGlue>(this).SpawnedCharacter;
		}
		BehaviorManager component = Utils.GetComponent<BehaviorManager>(this);
		if (component != null)
		{
			return base.gameObject;
		}
		if (Target != null && Utils.GetComponent<CharacterSpawnerGlue>(Target) != null)
		{
			return Utils.GetComponent<CharacterSpawnerGlue>(Target).SpawnedCharacter;
		}
		if (!string.IsNullOrEmpty(TargetName))
		{
			GameObject gameObject = GameObject.Find(TargetName);
			if (gameObject != null)
			{
				return gameObject;
			}
		}
		if (!TargetAnyAvailable)
		{
			return null;
		}
		component = (Object.FindObjectOfType(typeof(BehaviorManager)) as BehaviorManager);
		if (component == null)
		{
			return null;
		}
		return component.gameObject;
	}
}
