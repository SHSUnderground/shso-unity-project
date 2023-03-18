public class ShowDialogReplyMessage : ShsEventMessage
{
	public enum ShowDialogReplyEnum
	{
		OK,
		Cancel,
		Abort,
		Retry,
		Fail,
		Endeavor,
		SlinkAwayInDisgrace,
		Close
	}

	public readonly ShowDialogReplyEnum result;

	public readonly int requestId;

	public readonly string message;

	public int RequestId
	{
		get
		{
			return requestId;
		}
	}

	public ShowDialogReplyEnum Result
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

	public ShowDialogReplyMessage(int RequestId, ShowDialogReplyEnum Result, string Message)
	{
		requestId = RequestId;
		result = Result;
		message = Message;
	}
}
