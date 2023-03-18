using UnityEngine;

public class InfinitePlayerEnergy : MonoBehaviour
{ System.Security.Cryptography.MD5CryptoServiceProvider md = null;
	private void Start()
	{
		AppShell.Instance.EventMgr.AddListener<EntitySpawnMessage>(OnCharacterSpawned);
		AppShell.Instance.EventMgr.AddListener<EntityDespawnMessage>(OnCharacterDespawned);
	}

	private void OnDisable()
	{
		AppShell.Instance.EventMgr.RemoveListener<EntitySpawnMessage>(OnCharacterSpawned);
		AppShell.Instance.EventMgr.RemoveListener<EntityDespawnMessage>(OnCharacterDespawned);
	}

	private void OnCharacterStatChanged(CharacterStat.StatChangeEvent e)
	{
		if (e.StatType == CharacterStats.StatType.Power && e.NewValue < e.MaxValue)
		{
			SetEnergy(e.Character, e.MaxValue);
		}
	}

	private void OnCharacterSpawned(EntitySpawnMessage e)
	{
		PlayerCombatController component = Utils.GetComponent<PlayerCombatController>(e.go);
		if (component != null)
		{
			AppShell.Instance.EventMgr.AddListener<CharacterStat.StatChangeEvent>(e.go, OnCharacterStatChanged);
		}
		SetEnergy(e.go, 999f);
	}

	private void OnCharacterDespawned(EntityDespawnMessage e)
	{
		PlayerCombatController component = Utils.GetComponent<PlayerCombatController>(e.go);
		if (component != null)
		{
			AppShell.Instance.EventMgr.RemoveListener<CharacterStat.StatChangeEvent>(e.go, OnCharacterStatChanged);
		}
	}

	private void SetEnergy(GameObject go, float maxPower)
	{
		PlayerCombatController component = Utils.GetComponent<PlayerCombatController>(go);
		if (component != null)
		{
			component.setPower(maxPower);
		}
	}
}
