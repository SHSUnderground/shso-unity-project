using System.Collections;
using UnityEngine;

public class IOModelSpawner : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public string characterName;

	private InteractiveObject owner;

	private void Start()
	{
		owner = Utils.GetComponent<InteractiveObject>(base.gameObject, Utils.SearchParents);
		if (owner != null)
		{
			GameObject gameObject = Object.Instantiate(Resources.Load("Spawners/ModelSpawner"), base.transform.position, base.transform.rotation) as GameObject;
			CharacterSpawn component = gameObject.GetComponent<CharacterSpawn>();
			component.SetCharacterName(characterName);
			component.onSpawnCallback += OnSpawn;
		}
		else
		{
			CspUtils.DebugLog("No parent InteractiveObject found!  Could not spawn " + characterName);
		}
	}

	private void OnSpawn(GameObject character)
	{
		character.name = "Model";
		character.transform.parent = owner.transform;
		character.transform.localPosition = base.transform.localPosition;
		character.transform.localRotation = base.transform.localRotation;
		StartCoroutine(ConfigureOnNextFrame(character));
	}

	private IEnumerator ConfigureOnNextFrame(GameObject character)
	{
		yield return 0;
		CombatController cc = character.GetComponent<CombatController>();
		if (cc != null)
		{
			cc.faction = CombatController.Faction.Neutral;
		}
		BehaviorManager behaviorMan = character.GetComponent<BehaviorManager>();
		if (behaviorMan != null)
		{
			behaviorMan.ChangeDefaultBehavior("BehaviorWait");
			if (behaviorMan.requestChangeBehavior<BehaviorWait>(false) == null)
			{
				behaviorMan.forceChangeBehavior(typeof(BehaviorWait));
			}
		}
		owner.SetRoot(InteractiveObject.StateIdx.Model, character);
	}
}
