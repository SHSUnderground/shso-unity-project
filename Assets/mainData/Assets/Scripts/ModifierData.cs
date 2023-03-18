public class ModifierData
{
	public enum Stat
	{
		Invalid,
		AttackPower,
		SpecialPower,
		Health
	}

	protected float value;

	public float scalingFactor;

	public Stat targetStat;

	public ModifierData(float value)
	{
		this.value = value;
	}

	public ModifierData(float value, float scalingFactor, string scalingStat)
	{
		this.value = value;
		this.scalingFactor = scalingFactor;
		targetStat = statStrToEnum(scalingStat);
	}

	public ModifierData(DataWarehouse data, string baseName, float defaultValue)
	{
		value = data.TryGetFloat(baseName, defaultValue);
		scalingFactor = data.TryGetFloat(baseName + "_sf", 0f);
		targetStat = statStrToEnum(data.TryGetString(baseName + "_ss", string.Empty));
	}

	public static Stat statStrToEnum(string statAbbr)
	{
		switch (statAbbr)
		{
		case "ap":
			return Stat.AttackPower;
		case "sp":
			return Stat.SpecialPower;
		case "health":
			return Stat.Health;
		default:
			return Stat.AttackPower;
		}
	}

	public float getValue(CombatController cc = null, bool verbose = false)
	{
		if (value < 1f)
		{
			return value;
		}
		if (cc == null)
		{
			if (verbose)
			{
				CspUtils.DebugLog("getValue had no combat controller, returning " + value);
			}
			return value;
		}
		float num = 0f;
		switch (targetStat)
		{
		case Stat.AttackPower:
			num = cc.getAttackPower();
			break;
		case Stat.SpecialPower:
			num = cc.getSpecialPower();
			break;
		case Stat.Health:
			num = cc.getMaxHealth();
			break;
		}
		if (verbose)
		{
			CspUtils.DebugLog("getValue using stat " + targetStat + " " + num + " witrh base value of " + value + " and sf " + scalingFactor);
		}
		return value + num * scalingFactor;
	}
}
