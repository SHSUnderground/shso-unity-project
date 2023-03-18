using UnityEngine;

public class CharacterKilledEventTrigger : EventTriggerBase
{
	public bool CheckPlayers;

	public bool CheckAI = true;

	public CharacterSpawn OnlyFromSpawner;

	private void Start()
	{
		AppShell.Instance.EventMgr.AddListener<CombatCharacterKilledMessage>(OnCharacterKilledEvent);
	}

	private void OnDisable()
	{
		AppShell.Instance.EventMgr.RemoveListener<CombatCharacterKilledMessage>(OnCharacterKilledEvent);
	}

	private void OnCharacterKilledEvent(CombatCharacterKilledMessage e)
	{
		bool flag = false;
		SpawnData spawnData = e.Character.GetComponent(typeof(SpawnData)) as SpawnData;
		if (spawnData != null && (OnlyFromSpawner == null || spawnData.spawner == OnlyFromSpawner))
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
			OnTriggered(e.Character);
		}
	}

	protected override void OnDrawGizmosSelected()
	{
		base.OnDrawGizmosSelected();
		if (OnlyFromSpawner != null)
		{
			GameObject gameObject = OnlyFromSpawner.gameObject;
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
