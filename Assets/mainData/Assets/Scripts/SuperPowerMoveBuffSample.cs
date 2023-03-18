public class SuperPowerMoveBuffSample
{
	protected CharacterStat power;

	public SuperPowerMoveBuffSample(CharacterStats PlayerStats)
	{
		power = PlayerStats.GetStat(CharacterStats.StatType.Power);
		if (power != null)
		{
			power.StartTimedUpdates(1f, -5f);
		}
		AppShell.Instance.EventMgr.AddListener<CharacterStat.StatMinimumEvent>(OnPlayerStatMinimum);
	}

	private void OnPlayerStatMinimum(CharacterStat.StatMinimumEvent e)
	{
		if (power != null && e.StatType == CharacterStats.StatType.Power && (bool)e.Character.GetComponent("PlayerInputController"))
		{
			power.StopTimedUpdates();
			AppShell.Instance.EventMgr.RemoveListener<CharacterStat.StatMinimumEvent>(OnPlayerStatMinimum);
		}
	}
}
