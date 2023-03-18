public class HeroMysteryBoxItemData : MysteryBoxItemData
{
	public override string itemTextureSource
	{
		get
		{
			OwnableDefinition def = OwnableDefinition.getDef(ownableTypeID);
			if (def == null)
			{
				return string.Empty;
			}
			return "characters_bundle|expandedtooltip_render_" + def.name;
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
			if (!AppShell.Instance.CharacterDescriptionManager.Contains(def.name))
			{
				return null;
			}
			return AppShell.Instance.CharacterDescriptionManager[def.name].CharacterName;
		}
	}

	public HeroMysteryBoxItemData(int ownable_id, int quantity)
		: base(ownable_id, quantity)
	{
	}
}
