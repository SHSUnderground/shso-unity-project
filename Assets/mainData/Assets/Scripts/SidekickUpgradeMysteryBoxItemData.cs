public class SidekickUpgradeMysteryBoxItemData : MysteryBoxItemData
{
	public override bool displayInBox
	{
		get
		{
			return true;
		}
	}

	public override string name
	{
		get
		{
			OwnableDefinition def = OwnableDefinition.getDef(ownableTypeID);
			if (def == null)
			{
				return string.Empty;
			}
			return def.shoppingName;
		}
	}

	public SidekickUpgradeMysteryBoxItemData(int ownable_id, int quantity)
		: base(ownable_id, quantity)
	{
	}
}
