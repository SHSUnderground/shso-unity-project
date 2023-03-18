using System.Runtime.CompilerServices;

public class MusicChangedMessage : ShsEventMessage
{
	[CompilerGenerated]
	private ShsAudioBase _003COldMusic_003Ek__BackingField;

	[CompilerGenerated]
	private ShsAudioBase _003CNewMusic_003Ek__BackingField;

	public ShsAudioBase OldMusic
	{
		[CompilerGenerated]
		get
		{
			return _003COldMusic_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003COldMusic_003Ek__BackingField = value;
		}
	}

	public ShsAudioBase NewMusic
	{
		[CompilerGenerated]
		get
		{
			return _003CNewMusic_003Ek__BackingField;
		}
		[CompilerGenerated]
		protected set
		{
			_003CNewMusic_003Ek__BackingField = value;
		}
	}

	public MusicChangedMessage(ShsAudioBase oldMusic, ShsAudioBase newMusic)
	{
		OldMusic = oldMusic;
		NewMusic = newMusic;
	}
}
