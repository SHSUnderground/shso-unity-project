using UnityEngine;

public class AttackerLimiter : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	public bool inUse;

	public GameObject attacker;

	private BehaviorManager attackerBehaviorManager;

	public bool InUse
	{
		get
		{
			return attackerBehaviorManager != null;
		}
	}

	public void SetAttacker(GameObject attacker)
	{
		inUse = true;
		this.attacker = attacker;
		attackerBehaviorManager = Utils.GetComponent<BehaviorManager>(attacker);
	}

	public void Update()
	{
		if (InUse && !(attackerBehaviorManager.getBehavior() is BehaviorAttackBase))
		{
			inUse = false;
			attacker = null;
			attackerBehaviorManager = null;
		}
	}
}
