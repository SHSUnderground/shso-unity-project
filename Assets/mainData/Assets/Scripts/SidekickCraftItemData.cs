public class SidekickCraftItemData : MysteryBoxItemData
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
			if (def != null)
			{
				return def.iconFullPath;
			}
			return string.Empty;
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

	public SidekickCraftItemData(int ownable_id, int quantity)
		: base(ownable_id, quantity)
	{
	}
}
