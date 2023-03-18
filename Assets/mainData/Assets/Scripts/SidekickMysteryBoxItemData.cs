public class SidekickMysteryBoxItemData : MysteryBoxItemData
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
			return def.name;
		}
	}

	public SidekickMysteryBoxItemData(int ownable_id, int quantity)
		: base(ownable_id, quantity)
	{
	}
}
