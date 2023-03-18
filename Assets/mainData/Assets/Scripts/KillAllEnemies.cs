public class KillAllEnemies : ObjectiveBase
{
	private int enemyCount;

	private int enemyKillCount;

	public int EnemyCount
	{
		get
		{
			return enemyCount;
		}
	}

	public int EnemyKillCount
	{
		get
		{
			return enemyKillCount;
		}
	}

	public override bool IsMet()
	{
		return enemyCount > 0 && enemyKillCount == enemyCount;
	}

	protected virtual void OnEnable()
	{
		if (AppShell.Instance != null)
		{
			AppShell.Instance.EventMgr.AddListener<CombatCharacterCreatedMessage>(OnCharacterCreated);
			AppShell.Instance.EventMgr.AddListener<CombatCharacterKilledMessage>(OnCharacterKilled);
		}
		else
		{
			CspUtils.DebugLog("Couldn't subscribe to events in KillAllEnemies Objective: No AppShell instance found!");
		}
	}

	protected virtual void OnDisable()
	{
		if (AppShell.Instance != null)
		{
			AppShell.Instance.EventMgr.RemoveListener<CombatCharacterCreatedMessage>(OnCharacterCreated);
			AppShell.Instance.EventMgr.RemoveListener<CombatCharacterKilledMessage>(OnCharacterKilled);
		}
	}

	protected virtual void OnCharacterCreated(CombatCharacterCreatedMessage e)
	{
		if (e.CharacterCombat.IsEnemy())
		{
			enemyCount++;
		}
	}

	protected virtual void OnCharacterKilled(CombatCharacterKilledMessage e)
	{
		if (e.CharacterCombat.IsEnemy())
		{
			enemyKillCount++;
		}
	}

	protected void OnMobKilledWebResponse(ShsWebResponse response)
	{
		if (response.Status != 200)
		{
			CspUtils.DebugLog("Error reporting mob defeated to the SHS web service!");
		}
	}

	public override void Reset()
	{
		enemyCount = 0;
		enemyKillCount = 0;
	}
}
