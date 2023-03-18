public class PlayerChangedSecondaryAttackMessage : ShsEventMessage
{
	public int whatAttack;

	public PlayerChangedSecondaryAttackMessage(int attackNum)
	{
		whatAttack = attackNum;
	}
}
