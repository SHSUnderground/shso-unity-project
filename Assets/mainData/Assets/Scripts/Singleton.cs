public class Singleton<T> where T : class, new()
{
	private static T singleton;

	public static T instance
	{
		get
		{
			if (singleton == null)
			{
				singleton = new T();
			}
			return singleton;
		}
	}
}
