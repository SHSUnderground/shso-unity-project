using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[AddComponentMenu("Brawler/Traps/Collider")]
public class TrapCollider : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private BrawlerTrapBase owningTrap;

	private Dictionary<GameObject, float> rehitTimes;

	private void Start()
	{
		rehitTimes = new Dictionary<GameObject, float>();
	}

	private void Update()
	{
	}

	public void SetOwningTrap(BrawlerTrapBase newOwningTrap)
	{
		owningTrap = newOwningTrap;
	}

	protected virtual void OnTriggerEnter(Collider other)
	{
		if (owningTrap == null)
		{
			return;
		}
		NetworkComponent component = other.gameObject.GetComponent<NetworkComponent>();
		if ((component != null && (component.IsOwnedBySomeoneElse() || (!component.IsOwner() && !AppShell.Instance.ServerConnection.IsGameHost()))) || (rehitTimes.ContainsKey(other.gameObject) && rehitTimes[other.gameObject] > Time.time))
		{
			return;
		}
		CharacterGlobals characterGlobals = other.gameObject.GetComponent(typeof(CharacterGlobals)) as CharacterGlobals;
		if (characterGlobals != null && Utils.IsCharacterSpawned(other.gameObject) && (characterGlobals.combatController == null || !characterGlobals.combatController.isHidden))
		{
			owningTrap.OnHitTargetCharacter(characterGlobals);
			if (owningTrap.collisionRehitFrequency > 0f)
			{
				if (!rehitTimes.ContainsKey(characterGlobals.gameObject))
				{
					rehitTimes.Add(characterGlobals.gameObject, Time.time + owningTrap.collisionRehitFrequency);
				}
				else
				{
					rehitTimes[characterGlobals.gameObject] = Time.time + owningTrap.collisionRehitFrequency;
				}
			}
			return;
		}
		if (owningTrap.affectsObjects)
		{
			CombatController combatController = other.gameObject.GetComponent(typeof(CombatController)) as CombatController;
			if (combatController != null)
			{
				owningTrap.OnHitTarget(combatController);
				if (owningTrap.collisionRehitFrequency > 0f)
				{
					if (!rehitTimes.ContainsKey(combatController.gameObject))
					{
						rehitTimes.Add(combatController.gameObject, Time.time + owningTrap.collisionRehitFrequency);
					}
					else
					{
						rehitTimes[combatController.gameObject] = Time.time + owningTrap.collisionRehitFrequency;
					}
				}
				return;
			}
		}
		if (owningTrap.removeOnCollision)
		{
			owningTrap.OnCollision(other);
		}
	}

	protected virtual void OnTriggerExit(Collider other)
	{
	}

	protected virtual void OnTriggerStay(Collider other)
	{
		if (rehitTimes.ContainsKey(other.gameObject) && rehitTimes[other.gameObject] < Time.time)
		{
			OnTriggerEnter(other);
			rehitTimes[other.gameObject] = Time.time + owningTrap.collisionRehitFrequency;
		}
	}
}
