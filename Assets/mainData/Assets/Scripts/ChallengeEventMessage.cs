using System.Collections.Generic;

public class ChallengeEventMessage : ShsEventMessage
{
	protected string messageType;

	protected Dictionary<string, string> messageRequirements;

	protected object[] extraData;

	public string MessageType
	{
		get
		{
			return messageType;
		}
	}

	public Dictionary<string, string> MessageRequirements
	{
		get
		{
			return messageRequirements;
		}
	}

	public object[] ExtraData
	{
		get
		{
			return extraData;
		}
	}

	public ChallengeEventMessage(string messageType, params object[] extraData)
	{
		this.messageType = messageType;
		this.extraData = extraData;
	}

	public void AddRequirement<T>(string key, T value)
	{
		if (!string.IsNullOrEmpty(key) && value != null)
		{
			if (messageRequirements == null)
			{
				messageRequirements = new Dictionary<string, string>();
			}
			messageRequirements[key.ToLower()] = value.ToString().ToLower();
		}
	}
}
