public class HQRoomMysteryBoxItemData : MysteryBoxItemData
{
	public override string name
	{
		get
		{
			OwnableDefinition def = OwnableDefinition.getDef(ownableTypeID);
			if (def == null)
			{
				return string.Empty;
			}
			return '#' + "HQROOM_" + def.name;
		}
	}

	public override string itemTextureSource
	{
		get
		{
			OwnableDefinition def = OwnableDefinition.getDef(ownableTypeID);
			if (def == null)
			{
				return string.Empty;
			}
			return "hq_bundle|L_HQflyer_" + def.name;
		}
	}

	public HQRoomMysteryBoxItemData(int ownable_id, int quantity)
		: base(ownable_id, quantity)
	{
	}
}
