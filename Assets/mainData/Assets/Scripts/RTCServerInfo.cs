public class RTCServerInfo
{
	private string name;

	private int port;

	public string Name
	{
		get
		{
			return name;
		}
	}

	public int Port
	{
		get
		{
			return port;
		}
	}

	public string HostName
	{
		get
		{
			if (string.IsNullOrEmpty(Name))
			{
				return null;
			}
			int num = Name.IndexOf('.');
			return (num != -1) ? Name.Substring(0, num) : Name;
		}
	}

	private RTCServerInfo()
	{
	}

	public static RTCServerInfo Parse(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return null;
		}
		RTCServerInfo rTCServerInfo = new RTCServerInfo();
		string[] array = s.Split(':');
		if (array.Length >= 2)
		{
			rTCServerInfo.name = array[0];
			rTCServerInfo.port = int.Parse(array[1]);
		}
		else
		{
			rTCServerInfo.name = s;
			rTCServerInfo.port = 9339;
		}
		return rTCServerInfo;
	}

	public override string ToString()
	{
		return Name + ":" + Port;
	}
}
