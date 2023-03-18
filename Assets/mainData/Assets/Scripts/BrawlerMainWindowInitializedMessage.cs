using System.Runtime.CompilerServices;

public class BrawlerMainWindowInitializedMessage : ShsEventMessage
{
	[CompilerGenerated]
	private SHSBrawlerMainWindow _003CWindow_003Ek__BackingField;

	public SHSBrawlerMainWindow Window
	{
		[CompilerGenerated]
		get
		{
			return _003CWindow_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CWindow_003Ek__BackingField = value;
		}
	}

	public BrawlerMainWindowInitializedMessage(SHSBrawlerMainWindow window)
	{
		Window = window;
	}
}
