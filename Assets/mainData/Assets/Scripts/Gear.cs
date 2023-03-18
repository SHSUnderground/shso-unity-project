using System.Collections.Generic;

public class Gear
{
	public readonly int gearID;

	public int rating = 1;

	protected int slotTypeID;

	private string abilityString = string.Empty;

	public int heroID;

	public int validSlots;

	public List<SpecialAbility> abilities = new List<SpecialAbility>();

	public Gear(GearDataCollectionItem data)
	{
		gearID = data.gear_id;
		rating = data.level;
		slotTypeID = data.gear_slot_id;
		abilityString = data.abilities;
		heroID = data.hero_id;
		validSlots = data.valid_slots;
		CspUtils.DebugLog("gear loaded " + gearID + " " + validSlots);
	}
}
