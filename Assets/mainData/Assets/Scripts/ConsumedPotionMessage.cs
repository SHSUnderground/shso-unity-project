public class ConsumedPotionMessage : ShsEventMessage
{
	public bool Success;

	public string ErrorCode;

	public int OwnableTypeId;

	public int RequestId;

	public int PotionsRemaining;

	public ConsumedPotionMessage(bool success, string errorCode, int ownableTypeId)
		: this(success, errorCode, ownableTypeId, -1, -1)
	{
	}

	public ConsumedPotionMessage(bool success, string errorCode, int ownableTypeId, int requestId, int potionsLeft)
	{
		Success = success;
		ErrorCode = errorCode;
		OwnableTypeId = ownableTypeId;
		RequestId = requestId;
		PotionsRemaining = potionsLeft;
	}
}
