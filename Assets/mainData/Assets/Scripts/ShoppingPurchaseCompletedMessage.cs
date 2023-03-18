using System;

public class ShoppingPurchaseCompletedMessage : ShsEventMessage
{
	public bool success;

	public string result;

	public string guid;

	public ShoppingPurchaseCompletedMessage(object success, object result, object guid)
	{
		this.success = Convert.ToBoolean(success);
		this.result = ((result != null) ? result.ToString() : "success");
		this.guid = guid.ToString();
	}
}
