public class PowerAttackData
{
	public const int POWER_ATTACK_COUNT = 3;

	public string[] name;

	public string[] displayName;

	public PowerAttackData()
	{
		name = new string[3];
		displayName = new string[3];
		for (int i = 0; i < 3; i++)
		{
			name[i] = string.Empty;
			displayName[i] = string.Empty;
		}
	}

	public void StoreAttack(int index, string Name, string DisplayName)
	{
		name[index] = Name;
		displayName[index] = DisplayName;
	}
}
