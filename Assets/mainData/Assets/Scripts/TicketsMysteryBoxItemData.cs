public class TicketsMysteryBoxItemData : MysteryBoxItemData
{
	public override string itemTextureSource
	{
		get
		{
			return "shopping_bundle|shopping_ticket_item";
		}
	}

	public TicketsMysteryBoxItemData(int ownable_id, int quantity)
		: base(ownable_id, quantity)
	{
	}
}
