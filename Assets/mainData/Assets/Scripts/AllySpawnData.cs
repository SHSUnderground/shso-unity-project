public class AllySpawnData
{
	public int duration = -1;

	public bool oneShot;

	public string forcedAttackName = string.Empty;

	public string deathAnimOverride = string.Empty;

	public AllySpawnData(int duration, bool oneShot, string forcedAttackName, string deathAnimOverride)
	{
		this.duration = duration;
		this.oneShot = oneShot;
		this.forcedAttackName = forcedAttackName;
		this.deathAnimOverride = deathAnimOverride;
	}
}
