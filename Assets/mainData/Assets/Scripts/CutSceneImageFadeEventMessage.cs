using System.Runtime.CompilerServices;

public class CutSceneImageFadeEventMessage : ShsEventMessage
{
	[CompilerGenerated]
	private CutSceneImageFadeEvent _003CImageFadeEvent_003Ek__BackingField;

	public CutSceneImageFadeEvent ImageFadeEvent
	{
		[CompilerGenerated]
		get
		{
			return _003CImageFadeEvent_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CImageFadeEvent_003Ek__BackingField = value;
		}
	}

	public CutSceneImageFadeEventMessage(CutSceneImageFadeEvent imageFadeEvent)
	{
		ImageFadeEvent = imageFadeEvent;
	}
}
