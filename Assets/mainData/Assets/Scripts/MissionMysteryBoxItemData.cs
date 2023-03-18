using UnityEngine;

public class MissionMysteryBoxItemData : MysteryBoxItemData
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
			return '#' + "MIS_" + def.name + "_name";
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
			return "missionflyers_bundle|L_mission_flyer_" + def.name;
		}
	}

	public override Vector2 itemPictureSize
	{
		get
		{
			return new Vector2(262f, 328f);
		}
	}

	public override bool displayInBox
	{
		get
		{
			return false;
		}
	}

	public MissionMysteryBoxItemData(int ownable_id, int quantity)
		: base(ownable_id, quantity)
	{
	}
}
