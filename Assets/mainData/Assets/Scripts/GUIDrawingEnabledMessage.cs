using System.Runtime.CompilerServices;

public class GUIDrawingEnabledMessage : ShsEventMessage
{
	[CompilerGenerated]
	private bool _003CDrawingEnabled_003Ek__BackingField;

	public bool DrawingEnabled
	{
		[CompilerGenerated]
		get
		{
			return _003CDrawingEnabled_003Ek__BackingField;
		}
		[CompilerGenerated]
		private set
		{
			_003CDrawingEnabled_003Ek__BackingField = value;
		}
	}

	public GUIDrawingEnabledMessage(bool enabled)
	{
		DrawingEnabled = enabled;
	}
}
