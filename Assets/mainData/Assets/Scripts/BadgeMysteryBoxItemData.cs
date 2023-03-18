public class BadgeMysteryBoxItemData : MysteryBoxItemData
{
	public override bool displayInBox
	{
		get
		{
			return true;
		}
	}

	public override string itemTextureSource
	{
		get
		{
			OwnableDefinition def = OwnableDefinition.getDef(ownableTypeID);
			if (def.name.EndsWith("_2"))
			{
				return "shopping_bundle|badge";
			}
			return "shopping_bundle|badge_silver";
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

	public BadgeMysteryBoxItemData(int ownable_id, int quantity)
		: base(ownable_id, quantity)
	{
	}
}
