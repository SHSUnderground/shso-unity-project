using System.Collections;
using UnityEngine;

public class CharacterAggroEventTrigger : EventTriggerBase
{
	public bool CheckPlayers;

	public bool CheckAI = true;

	public CharacterSpawn[] OnlyFromSpawners;

	protected Hashtable onlyFromSpawnersSet;

	protected CombatController triggeringCombatController;

	public CombatController TriggeringCombatController
	{
		get
		{
			return triggeringCombatController;
		}
	}

	private void Start()
	{
		AppShell.Instance.EventMgr.AddListener<CombatCharacterAggroMessage>(OnCharacterAggroEvent);
		onlyFromSpawnersSet = new Hashtable();
		CharacterSpawn[] onlyFromSpawners = OnlyFromSpawners;
		foreach (CharacterSpawn characterSpawn in onlyFromSpawners)
		{
			onlyFromSpawnersSet.Add(characterSpawn, characterSpawn);
		}
	}

	private void OnDisable()
	{
		AppShell.Instance.EventMgr.RemoveListener<CombatCharacterAggroMessage>(OnCharacterAggroEvent);
	}

	private void OnCharacterAggroEvent(CombatCharacterAggroMessage e)
	{
		bool flag = false;
		SpawnData spawnData = e.CharacterCombat.gameObject.GetComponent(typeof(SpawnData)) as SpawnData;
		if (spawnData != null && (onlyFromSpawnersSet == null || onlyFromSpawnersSet.Count == 0 || (spawnData.spawner != null && onlyFromSpawnersSet.Contains(spawnData.spawner))))
		{
			if (CheckPlayers && (spawnData.spawnType & CharacterSpawn.Type.Player) != 0)
			{
				flag = true;
			}
			if (CheckAI && (spawnData.spawnType & CharacterSpawn.Type.Player) == 0)
			{
				flag = true;
			}
		}
		if (flag)
		{
			OnTriggered(e.TargetCharacterCombat.gameObject);
		}
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		CharacterSpawn[] onlyFromSpawners = OnlyFromSpawners;
		foreach (CharacterSpawn characterSpawn in onlyFromSpawners)
		{
			if (!(characterSpawn == null))
			{
				GameObject gameObject = characterSpawn.gameObject;
				Gizmos.color = Color.green;
				Gizmos.DrawLine(base.transform.position, gameObject.transform.position);
				Quaternion rotation = base.transform.rotation;
				Matrix4x4 matrix = Gizmos.matrix;
				try
				{
					base.transform.LookAt(gameObject.transform);
					Gizmos.matrix = base.transform.localToWorldMatrix;
					Gizmos.DrawFrustum(base.transform.position, 20f, 0f, 0.3f, 1f);
				}
				finally
				{
					base.transform.rotation = rotation;
					Gizmos.matrix = matrix;
				}
			}
		}
	}
}
