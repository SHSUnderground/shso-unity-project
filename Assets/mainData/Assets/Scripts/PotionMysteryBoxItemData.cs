public class PotionMysteryBoxItemData : MysteryBoxItemData
{
	public override bool displayInBox
	{
		get
		{
			return false;
		}
	}

	public override string itemTextureSource
	{
		get
		{
			ExpendableDefinition value = null;
			if (AppShell.Instance.ExpendablesManager.ExpendableTypes.TryGetValue(string.Empty + ownableTypeID, out value))
			{
				return value.ShoppingIcon;
			}
			return string.Empty;
		}
	}

	public override string name
	{
		get
		{
			if (!AppShell.Instance.ExpendablesManager.ExpendableTypes.ContainsKey(string.Empty + ownableTypeID))
			{
				return null;
			}
			return AppShell.Instance.ExpendablesManager.ExpendableTypes[string.Empty + ownableTypeID].Name;
		}
	}

	public PotionMysteryBoxItemData(int ownable_id, int quantity)
		: base(ownable_id, quantity)
	{
	}
}
