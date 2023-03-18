using System.Collections;
using UnityEngine;

public class PossessedController : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public float aggroDistance = 100f;

	private Component addedComponent;

	private CharacterGlobals myCharGlobals;

	private void Start()
	{
		myCharGlobals = GetComponent<CharacterGlobals>();
		if (PlayerCombatController.GetPlayerCount() > 1)
		{
			InitMultiplayer();
		}
		else
		{
			InitSingleplayer();
		}
	}

	private void OnDestroy()
	{
		if (addedComponent != null)
		{
			Object.Destroy(addedComponent);
		}
		if (myCharGlobals != null)
		{
			myCharGlobals.motionController.stopGently();
			myCharGlobals.behaviorManager.getBehavior().destinationChanged();
		}
	}

	private void InitSingleplayer()
	{
		if (base.gameObject.GetComponent<AIControllerRandom>() == null)
		{
			AIControllerRandom aIControllerRandom = (AIControllerRandom)(addedComponent = base.gameObject.AddComponent<AIControllerRandom>());
		}
	}

	private void InitMultiplayer()
	{
		if (myCharGlobals.networkComponent.IsOwner())
		{
			StartCoroutine(AttackPlayers());
		}
	}

	private IEnumerator AttackPlayers()
	{
		yield return new WaitForSeconds(0.25f);
		while (base.enabled)
		{
			CharacterGlobals currentTarget2 = null;
			while (true)
			{
				CharacterGlobals closestPlayerTarget;
				currentTarget2 = (closestPlayerTarget = GetClosestPlayerTarget());
				if (!(closestPlayerTarget == null))
				{
					break;
				}
				yield return new WaitForSeconds(0.25f);
			}
			myCharGlobals.combatController.pursueTarget(currentTarget2.gameObject, false);
			while (IsAttacking() && currentTarget2 != null && !currentTarget2.combatController.isKilled)
			{
				yield return new WaitForSeconds(0.5f);
			}
			yield return 0;
		}
	}

	private CharacterGlobals GetClosestPlayerTarget()
	{
		CharacterGlobals[] array = Object.FindObjectsOfType(typeof(CharacterGlobals)) as CharacterGlobals[];
		CombatController.Faction enemyFaction = myCharGlobals.combatController.GetEnemyFaction();
		CharacterGlobals characterGlobals = null;
		float num = -1f;
		CharacterGlobals[] array2 = array;
		foreach (CharacterGlobals characterGlobals2 in array2)
		{
			if (characterGlobals2.combatController.faction == enemyFaction && !characterGlobals2.combatController.isKilled)
			{
				float sqrMagnitude = (characterGlobals2.transform.position - base.transform.position).sqrMagnitude;
				if (characterGlobals == null || sqrMagnitude < num)
				{
					characterGlobals = characterGlobals2;
					num = sqrMagnitude;
				}
			}
		}
		return characterGlobals;
	}

	private bool IsAttacking()
	{
		BehaviorBase behavior = myCharGlobals.behaviorManager.getBehavior();
		return behavior is BehaviorAttackApproach || behavior is BehaviorAttackBase;
	}
}
