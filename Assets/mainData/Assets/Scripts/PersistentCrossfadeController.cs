public class PersistentCrossfadeController : CrossfadeController
{
	private ShsAudioBase _PersistentSource;

	public ShsAudioBase PersistentSource
	{
		get
		{
			return _PersistentSource;
		}
		set
		{
			if (PersistentSource != null)
			{
				Crossfader.GetCrossfader(PersistentSource).stopOnFadeOut = true;
			}
			_PersistentSource = value;
		}
	}

	public PersistentCrossfadeController(ShsAudioBase persistentSource)
	{
		PersistentSource = persistentSource;
	}

	public override void OnUnregistered()
	{
		if (PersistentSource != null)
		{
			Crossfader.CrossfadeOut(PersistentSource, true);
		}
		base.OnUnregistered();
	}

	public override ShsAudioBase RequestCrossfade(ShsAudioBase currentMusic, ShsAudioBase audioSource)
	{
		if (audioSource == null && PersistentSource != null)
		{
			return base.RequestCrossfade(currentMusic, PersistentSource);
		}
		return base.RequestCrossfade(currentMusic, audioSource);
	}

	protected override Crossfader CrossfadeOut(ShsAudioBase source)
	{
		if (source == PersistentSource)
		{
			return Crossfader.CrossfadeOut(source, false);
		}
		return base.CrossfadeOut(source);
	}
}
