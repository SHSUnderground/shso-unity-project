public class HQItemMysteryBoxItemData : MysteryBoxItemData
{
	public override string itemTextureSource
	{
		get
		{
			if (!AppShell.Instance.ItemDictionary.ContainsKey(string.Empty + ownableTypeID))
			{
				return null;
			}
			return "items_bundle|" + AppShell.Instance.ItemDictionary[string.Empty + ownableTypeID].Icon;
		}
	}

	public override string name
	{
		get
		{
			if (!AppShell.Instance.ItemDictionary.ContainsKey(string.Empty + ownableTypeID))
			{
				return null;
			}
			return AppShell.Instance.ItemDictionary[string.Empty + ownableTypeID].Name;
		}
	}

	public HQItemMysteryBoxItemData(int ownable_id, int quantity)
		: base(ownable_id, quantity)
	{
	}
}
