public class ShoppingItemPurchasedMessage : ShsEventMessage
{
	public readonly OwnableDefinition.Category ItemType;

	public readonly string OwnableId;

	public readonly string OwnableName;

	public ShoppingItemPurchasedMessage(OwnableDefinition.Category itemType, string ownableId, string ownableName)
	{
		ItemType = itemType;
		OwnableId = ownableId;
		OwnableName = ownableName;
	}
}
