using UnityEngine;

public class CombatEffectPowerBarTimer : CombatEffectBase
{
	public float duration = 10f;

	protected PlayerCombatController combatController;

	protected CharacterStat playerPower;

	protected override void ReleaseEffect()
	{
		playerPower.StopTimedUpdates();
		base.ReleaseEffect();
	}

	public override void Initialize(CombatEffectData newCombatEffectData, CombatController sourceCombatController)
	{
		base.Initialize(newCombatEffectData, sourceCombatController);
		duration = combatEffectData.powerBarTimerDuration;
		GameObject gameObject = base.transform.root.gameObject;
		combatController = (gameObject.GetComponent(typeof(PlayerCombatController)) as PlayerCombatController);
		CharacterStats characterStats = gameObject.GetComponent(typeof(CharacterStats)) as CharacterStats;
		if (characterStats != null)
		{
			playerPower = characterStats.GetStat(CharacterStats.StatType.Power);
		}
		if (playerPower != null)
		{
			playerPower.StartTimedUpdates(1f, (0f - playerPower.MaximumValue) / duration);
		}
	}
}
