public class SHSInheritedStyleInfo : SHSStyleInfo
{
	private static SHSInheritedStyleInfo instance;

	public static SHSInheritedStyleInfo Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new SHSInheritedStyleInfo();
			}
			return instance;
		}
	}
}
