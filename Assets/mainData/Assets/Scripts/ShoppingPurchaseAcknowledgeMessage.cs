public class ShoppingPurchaseAcknowledgeMessage : ShsEventMessage
{
	public string guid;

	public bool result;

	public int resultCode;

	public string resultReason;

	public ShoppingPurchaseAcknowledgeMessage(string guid, bool result, int resultCode, string resultReason)
	{
		this.guid = guid;
		this.result = result;
		this.resultCode = resultCode;
		this.resultReason = resultReason;
	}
}
