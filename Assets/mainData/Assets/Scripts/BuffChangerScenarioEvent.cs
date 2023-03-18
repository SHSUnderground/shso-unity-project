using UnityEngine;

public class BuffChangerScenarioEvent : ScenarioEventHandlerEnableBase
{
	public string targetBuff = string.Empty;

	public bool remove;

	public float radius = 1000f;

	public bool effectsPlayers;

	public bool effectsEnemies = true;

	public string specificTarget;

	public float deafTime;

	public string eventOnCollision = string.Empty;

	public bool dispatchLocalOnly;

	protected CombatController sourceCombatController;

	protected float startTime;

	protected void AddBuff(GameObject target)
	{
		CombatController component = Utils.GetComponent<CombatController>(target);
		if (!(component as ObjectCombatController != null) && component != null && ((effectsEnemies && component.faction == CombatController.Faction.Enemy) || (effectsPlayers && component.faction == CombatController.Faction.Player)))
		{
			component.createCombatEffect(targetBuff, sourceCombatController, false);
			DispatchEvent();
		}
	}

	protected void RemoveBuff(GameObject target)
	{
		CombatController component = Utils.GetComponent<CombatController>(target);
		if (!(component as ObjectCombatController != null) && component != null && ((effectsEnemies && component.IsEnemy()) || (effectsPlayers && component.IsPlayer())))
		{
			component.removeCombatEffect(targetBuff);
			DispatchEvent();
		}
	}

	protected void DispatchEvent()
	{
		if (!string.IsNullOrEmpty(eventOnCollision))
		{
			ScenarioEventManager.Instance.FireScenarioEvent(eventOnCollision, !dispatchLocalOnly);
		}
	}

	protected override void OnEnableEvent(string eventName)
	{
		if (Time.time - startTime < deafTime)
		{
			return;
		}
		Collider[] array = Physics.OverlapSphere(base.transform.position, radius, 2101248);
		if (!remove)
		{
			if (specificTarget == null || specificTarget == string.Empty)
			{
				Collider[] array2 = array;
				foreach (Collider collider in array2)
				{
					AddBuff(collider.gameObject);
				}
				return;
			}
			Collider[] array3 = array;
			foreach (Collider collider2 in array3)
			{
				if (collider2.name == specificTarget)
				{
					AddBuff(collider2.gameObject);
				}
			}
			return;
		}
		if (specificTarget == null || specificTarget == string.Empty)
		{
			Collider[] array4 = array;
			foreach (Collider collider3 in array4)
			{
				RemoveBuff(collider3.gameObject);
			}
			return;
		}
		Collider[] array5 = array;
		foreach (Collider collider4 in array5)
		{
			if (collider4.name == specificTarget)
			{
				RemoveBuff(collider4.gameObject);
			}
		}
	}

	protected override void Start()
	{
		base.Start();
		sourceCombatController = Utils.GetComponent<CombatController>(base.gameObject);
		startTime = Time.time;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "BuffChangerIcon.png");
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(base.transform.position, radius);
	}
}
