using System.Runtime.CompilerServices;
using System.Xml;

public class FriendMessage : ShsEventMessage
{
	public delegate void SquadNameFetched(string squadName);

	[CompilerGenerated]
	private int _003CFriendID_003Ek__BackingField;

	[CompilerGenerated]
	private string _003CFriendName_003Ek__BackingField;

	public int FriendID
	{
		[CompilerGenerated]
		get
		{
			return _003CFriendID_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CFriendID_003Ek__BackingField = value;
		}
	}

	public string FriendName
	{
		[CompilerGenerated]
		get
		{
			return _003CFriendName_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CFriendName_003Ek__BackingField = value;
		}
	}

	public FriendMessage(int friendID, string friendName)
	{
		FriendID = friendID;
		FriendName = friendName;
	}

	public void FetchSquadName(SquadNameFetched onFetched)
	{
		onFetched(FriendName);
	}

	private string OnFetchUserData(ShsWebResponse response)
	{
		if (response.Status == 200)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(response.Body);
			XmlNode xmlNode = xmlDocument.SelectSingleNode("//profile/player_name");
			if (xmlNode != null)
			{
				return xmlNode.InnerText;
			}
		}
		return null;
	}
}
