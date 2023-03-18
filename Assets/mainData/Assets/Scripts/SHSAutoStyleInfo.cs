public class SHSAutoStyleInfo : SHSStyleInfo
{
	private static SHSAutoStyleInfo instance;

	public static SHSAutoStyleInfo Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new SHSAutoStyleInfo();
			}
			return instance;
		}
	}
}
