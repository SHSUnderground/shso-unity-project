public class RedeemCodeResultsMessage : ShsEventMessage
{
	public enum RedeemCodeResultsEnum
	{
		Succeeded,
		Error,
		InvalidCode,
		CodeAlreadyUsed
	}

	public readonly RedeemCodeResultsEnum result;

	public readonly int requestId;

	public readonly string message;

	public int RequestId
	{
		get
		{
			return requestId;
		}
	}

	public RedeemCodeResultsEnum Result
	{
		get
		{
			return result;
		}
	}

	public string Message
	{
		get
		{
			return message;
		}
	}

	public RedeemCodeResultsMessage(int RequestId, RedeemCodeResultsEnum Result, string Message)
	{
		requestId = RequestId;
		result = Result;
		message = Message;
	}
}
