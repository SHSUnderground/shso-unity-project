public class GearDataCollectionItem : ShsCollectionItem
{
	public int gear_id;

	public int gear_slot_id;

	public int level;

	public string abilities;

	public int hero_id;

	public int valid_slots;

	private OwnableDefinition definition;

	public override bool InitializeFromData(DataWarehouse data)
	{
		gear_id = data.TryGetInt("gear_id", -1);
		gear_slot_id = data.TryGetInt("gear_slot_id", -1);
		level = data.TryGetInt("level", -1);
		abilities = data.TryGetString("abilities", null);
		hero_id = data.TryGetInt("hero_id", -1);
		valid_slots = data.TryGetInt("valid_slots", -1);
		return true;
	}

	public override void UpdateFromData(DataWarehouse data)
	{
	}

	public override string GetKey()
	{
		return string.Empty + gear_id;
	}
}
